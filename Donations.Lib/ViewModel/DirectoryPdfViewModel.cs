using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Syncfusion.Pdf.Security;
using Syncfusion.Pdf;
using Syncfusion.XPS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using Figure = System.Windows.Documents.Figure;
using Table = System.Windows.Documents.Table;
using TableRow = System.Windows.Documents.TableRow;
using TableRowGroup = System.Windows.Documents.TableRowGroup;
using Section = System.Windows.Documents.Section;
using Paragraph = System.Windows.Documents.Paragraph;
using TableCell = System.Windows.Documents.TableCell;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
using System.IO.Abstractions;
using Serilog;
using Donations.Lib.View;
using System.Linq;
using System.Windows.Input;
using System.Windows.Documents.Serialization;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Shapes;

namespace Donations.Lib.ViewModel;

public partial class DirectoryPdfViewModel : BaseViewModel
{
	private readonly Regex _imageRePat = new Regex($"{{{enumPdfCover.Image}([ ]+[A-za-z0-9=]+)?([ ]+[A-za-z0-9=]+)?[ ]*}}");
	private readonly Regex _textRePat = new Regex(@"({\w+?})?({(\w+)=([+-]?\w+?)})?({/\w+?})?");
	private PrintDialog _pd;
	private double _width;
	private readonly IFileSystem _fileSystem;
	private readonly ILogger _logger;
	private readonly IDonorServices _donorServices;
	private readonly IAppSettingsServices _appSettingsServices;
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private Dictionary<string, DirectoryData>? _directoryEntries;
	private FlowDocument _doc;
	private FlowDocument? _pdfFlowDocument;
	private AutoResetEvent _waitHandle = new AutoResetEvent(false);
	private string _pdfFilename = "";
	private bool _cancelLoading = false;
	private Semaphore _loading = new Semaphore(1, 1);
	private Dictionary<enumPdfCover, List<object>> _formatMap = new Dictionary<enumPdfCover, List<object>>();

	public DirectoryPdfViewModel(
		IFileSystem fileSystem,
		ILogger logger,
        IDonorServices donorServices,
		IAppSettingsServices appSettingsServices,
		IDispatcherWrapper dispatcherWrapper
    )
    {
		_fileSystem = fileSystem;
		_logger = logger;
		_donorServices = donorServices;
		_appSettingsServices = appSettingsServices;
		_dispatcherWrapper = dispatcherWrapper;
	}

	[ObservableProperty]
	private string? _selectedFont = "Calibri";
	/// <summary>
	/// The SelectedFont prperty is used to initially select the last font used, and also to
	/// receive the latest font chosen by the operator.
	/// </summary>
	partial void OnSelectedFontChanged(string? value)
	{
		_formatMap[enumPdfCover.Font][0] = value;
	}

	[ObservableProperty]
	private double _selectedSize = 14;
	/// <summary>
	/// The SelectedSize prperty is used to initially select the last font size used, and also to
	/// receive the latest font size chosen by the operator.
	/// </summary>
	partial void OnSelectedSizeChanged(double value)
	{
		if (null != _doc)
		{
			_doc.FontSize = value;
			_formatMap[enumPdfCover.FontSize][0] = value.ToString();
		}
	}

	[ObservableProperty]
	private double _leftMargin = 0.8;
	/// <summary>
	/// The LeftMargin property is provided for the sole purpose of allowing a larger left margin
	/// for three ring binding. Otherwise, I would have only given a single margin parameter.
	/// </summary>

	[ObservableProperty]
	private double _otherMargins = 0.5;
	/// <summary>
	/// The OtherMargins property contains a single value that is applied to the top/right/bottom
	/// page margins.
	/// </summary>

	[ObservableProperty]
	private bool _address = Persist.Default.PdfDirectoryIncludeAddress;
	partial void OnAddressChanged(bool value)
	{
		Persist.Default.PdfDirectoryIncludeAddress = value;
		Loading();
	}

	[ObservableProperty]
	private bool _email = Persist.Default.PdfDirectoryIncludeEmail;
	partial void OnEmailChanged(bool value)
	{
		Persist.Default.PdfDirectoryIncludeEmail = value;
		Loading();
	}

	[ObservableProperty]
	private bool _phoneNumber = Persist.Default.PdfDirectoryIncludePhone;
	partial void OnPhoneNumberChanged(bool value)
	{
		Persist.Default.PdfDirectoryIncludePhone = value;
		Loading();
	}

	[ObservableProperty]
	private string _coverImage = Persist.Default.PdfDirectoryCoverImage;
	partial void OnCoverImageChanged(string value)
	{
		Persist.Default.PdfDirectoryCoverImage = value;
	}

	[ObservableProperty]
	private string _coverText = Persist.Default.PdfDirectoryCoverText;
	partial void OnCoverTextChanged(string value)
	{
		Persist.Default.PdfDirectoryCoverText = value;
	}

	[ObservableProperty]
	private bool _readyToSavePdf = false;

	[ObservableProperty]
	private string _status = "";

	[ObservableProperty]
	private double _progress = 0;

	[ObservableProperty]
	private string _pdfPassword = "";

	public void SetDirectoryEntries(Dictionary<string, DirectoryData>? directoryEntries)
	{
		_directoryEntries = directoryEntries;

		Loading();
	}

	public void SetCoverImageFile(string imageFile)
	{
		CoverImage = imageFile;
	}

	public async Task SetDocument(
		FlowDocument flowDocument
	)
	{
		_doc = flowDocument;

		_pd = new PrintDialog();
		Thickness margin = new Thickness(LeftMargin * PrintOptionsView._dpi,
									OtherMargins * PrintOptionsView._dpi,
									OtherMargins * PrintOptionsView._dpi,
									OtherMargins * PrintOptionsView._dpi);

		_doc.PageWidth = _pd.PrintableAreaWidth;
		_doc.PageHeight = _pd.PrintableAreaHeight;
		_doc.ColumnWidth = _doc.PageWidth;
		_doc.PagePadding = margin;

		_width = _doc.PageWidth - margin.Left - margin.Right;
	}

	private async Task AddEntry(Section section, string key, string basePictureUrl)
	{
		DirectoryData data = _directoryEntries[key];
		var figure = new Figure();
		var table = new Table() { BorderThickness = new Thickness(2), BorderBrush = new SolidColorBrush(Colors.Black)};
		table.RowGroups.Add(new TableRowGroup());
		var row = new TableRow() { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize };
		var column = new TableColumn() { Width = new GridLength(1, GridUnitType.Star) };
		table.Columns.Add(column);
		column = new TableColumn() { Width = new GridLength(2, GridUnitType.Star) };
		table.Columns.Add(column);

		string source = "";
		Image cellImage = new Image() { Stretch = Stretch.Uniform, Height = _width / 6, HorizontalAlignment = HorizontalAlignment.Left };
		if (!string.IsNullOrEmpty(data.Picture))
		{
			cellImage.Source = new BitmapImage(new Uri(basePictureUrl + data.Picture, UriKind.Absolute));
			source = basePictureUrl + data.Picture;
			await Task.Delay(100);
		}

		row.Cells.Add(new TableCell(new BlockUIContainer(cellImage)));

		string entry = data.Name + "\r\n" +
			data.OtherFamilyMembers;

		if (Address)
		{
			entry += "\r\n" + data.Address;
		}

		if (Email)
		{
			entry += "\r\n" + data.Email;
		}

		if (PhoneNumber)
		{
			entry += "\r\n" + data.Phone;
		}

		row.Cells.Add(new TableCell(new Paragraph(new Run(entry))) { TextAlignment = TextAlignment.Left });

		table.RowGroups[0].Rows.Add(row);
		section.Blocks.Add(table);
	}

	private async Task GenerateCoverPage(FlowDocument flowDocument)
	{
		Section section = new Section();
		flowDocument.Blocks.Add(section);
		string? lines = null;
		var split = CoverText.Split("\r\n");
		_formatMap.Clear();
		foreach (enumPdfCover format in Enum.GetValues(typeof(enumPdfCover)))
		{
			_formatMap[format] = new List<object>();
		}
		_formatMap[enumPdfCover.Font].Add(SelectedFont);
		_formatMap[enumPdfCover.FontSize].Add(SelectedSize);

		foreach (var line in split)
		{
			if (line.Contains($"{{{enumPdfCover.Image}", StringComparison.OrdinalIgnoreCase))
			{
				double width = 0;
				double height = 0;

				var res = _imageRePat.Match(line);
				if (null != res && res.Success && 1 <= res.Groups.Count)
				{
					for (int i = 1; i < res.Groups.Count; i++)
					{
						if (!string.IsNullOrEmpty(res.Groups[i].Value) && res.Groups[i].Value.Contains('='))
						{
							var splt = res.Groups[i].Value.Trim().Split("=");
							if (2 == splt.Length)
							{
								if (splt[0].Equals("WIDTH", StringComparison.OrdinalIgnoreCase))
								{
									width = double.Parse(splt[1]);
								}
								else if (splt[0].Equals("HEIGHT", StringComparison.OrdinalIgnoreCase))
								{
									height = double.Parse(splt[1]);
								}
							}
						}
					}
				}

				CheckDumpParagraph(ref lines, section);

				var imgSource = new BitmapImage(new Uri(CoverImage));
				var cellImage = new Image() { Source = imgSource, Stretch = Stretch.Uniform, HorizontalAlignment = HorizontalAlignment.Left };

				if (0 != width)
				{
					cellImage.Width = width;
				}
				if (0 != height)
				{
					cellImage.Height = height;
				}

				section.Blocks.Add(new BlockUIContainer(cellImage));
			}
			else if (!string.IsNullOrEmpty(line))
			{
				var res = _textRePat.Matches(line);
				foreach (Match m in res)
				{
					if (string.IsNullOrEmpty(m.Groups[0].Value))
					{
						if (m.Groups[0].Index < line.Length)
						{
							lines += line[m.Groups[0].Index];
						}
					}
					else
					{
						CheckFormats(ref lines, section, m.Groups);
					}
				}
				lines += '\n';
			}
			else
			{
				lines += '\n';
			}
		}

		CheckDumpParagraph(ref lines, section);
	}

	private void CheckFormats(ref string? lines, Section section,GroupCollection group)
	{
		for (int i = 1; i < group.Count; i++)
		{
			if (string.IsNullOrEmpty(group[i].Value)) continue;
			if (group[i].Value.Contains('=') && i + 3 < group.Count)
			{
				if (CheckSingleKVPFormat(ref lines, section, group[i + 1].Value, group[i + 2].Value, enumPdfCover.Font)) { }
				else if (CheckSingleKVPFormat(ref lines, section, group[i + 1].Value, group[i + 2].Value, enumPdfCover.FontSize)) { }
				i += 3;
			}
			else if (CheckSingleFormat(ref lines, section, group[i].Value, enumPdfCover.b)) { }
			else if (CheckSingleFormat(ref lines, section, group[i].Value, enumPdfCover.u)) { }
			else if (CheckSingleFormat(ref lines, section, group[i].Value, enumPdfCover.i)) { }
			else if (CheckSingleFormat(ref lines, section, group[i].Value, enumPdfCover.Font)) { }
			else if (CheckSingleFormat(ref lines, section, group[i].Value, enumPdfCover.FontSize)) { }
		}
	}

	private bool CheckSingleFormat(ref string? lines, Section section, string value, enumPdfCover format)
	{
		if (value.Contains($"{{{format}}}", StringComparison.OrdinalIgnoreCase))
		{
			CheckDumpParagraph(ref lines, section);

			_formatMap[format].Add(true);
			return true;
		}
		if (value.Contains($"{{/{format}}}", StringComparison.OrdinalIgnoreCase))
		{
			CheckDumpParagraph(ref lines, section);

			_formatMap[format].RemoveAt(_formatMap[format].Count - 1);
			return true;
		}
		return false;
	}

	private bool CheckSingleKVPFormat(ref string? lines, Section section, string key, string value, enumPdfCover format)
	{
		if (key.Equals($"{format}", StringComparison.OrdinalIgnoreCase))
		{
			CheckDumpParagraph(ref lines, section);

			if (enumPdfCover.FontSize == format)
			{
				if (value[0] == '+' || value[0] == '-')
				{
					double size = double.Parse(_formatMap[format][0].ToString());
					size += double.Parse(value);
					_formatMap[format].Add(size.ToString());
				}
				else
				{
					_formatMap[format].Add(value);
				}
			}
			else
			{
				_formatMap[format].Add(value);
			}

			return true;
		}
		if (key.Equals($"/{format}", StringComparison.OrdinalIgnoreCase))
		{
			CheckDumpParagraph(ref lines, section);

			_formatMap[format].RemoveAt(_formatMap[format].Count - 1);
			return true;
		}
		return false;
	}

	private void CheckDumpParagraph(ref string? lines, Section section)
	{
		if (null != lines)
		{
			var run = new Run(lines + "\n")
			{
				FontFamily = new FontFamily(_formatMap[enumPdfCover.Font].Last().ToString())
			};

			var fontsize = _formatMap[enumPdfCover.FontSize].Last().ToString();

			if (!string.IsNullOrEmpty(fontsize))
			{
				double size = SelectedSize;
				double.TryParse(fontsize, out size);
				run.FontSize = size;
				if (0 < _formatMap[enumPdfCover.b].Count)
				{
					run.FontWeight = FontWeights.Bold;
				}
				if (0 < _formatMap[enumPdfCover.i].Count)
				{
					run.FontStyle = FontStyles.Italic;
				}
				if (0 < _formatMap[enumPdfCover.u].Count)
				{
					run.TextDecorations = TextDecorations.Underline;
				}
			}

			section.Blocks.Add(new Paragraph(run));

			lines = null;
		}
	}

	public new async Task Loading()
	{
		// we don't want multipla instances of Loading() running simultaneously,
		// so try canceling if one is running and then take the resource
		_cancelLoading = true;
		await Task.Run(() => _loading.WaitOne());
		_cancelLoading = false;

		if (null != _directoryEntries && null != _doc)
		{
			Thickness margin = new Thickness(LeftMargin * PrintOptionsView._dpi,
										OtherMargins * PrintOptionsView._dpi,
										OtherMargins * PrintOptionsView._dpi,
										OtherMargins * PrintOptionsView._dpi);

			_pdfFlowDocument = new FlowDocument()
			{
				PageWidth = _pd.PrintableAreaWidth,
				PageHeight = _pd.PrintableAreaHeight,
				ColumnWidth = _doc.PageWidth,
				PagePadding = margin
			};

			_doc.Blocks.Clear();
			var basePictureUrl = _appSettingsServices.Get()?.PictureBaseUrl;
			var keys = _directoryEntries.Keys.Order();
			double total = _directoryEntries.Count;

			await GenerateCoverPage(_doc);
			await GenerateCoverPage(_pdfFlowDocument);

			Section section = new Section()
			{
				BreakPageBefore = true
			};

			Section pdfSection = new Section()
			{
				BreakPageBefore = true
			};

			_doc.Blocks.Add(section);
			_pdfFlowDocument.Blocks.Add(pdfSection);

			double c = 0;
			Status = "Rendering directory entries";

			foreach (string key in keys)
			{
				Progress = 100 * c / total;
				c++;
				if (_cancelLoading)
				{
					_cancelLoading = false;
					break;
				}
				await AddEntry(section, key, basePictureUrl);
				await AddEntry(pdfSection, key, basePictureUrl);
			}

			ReadyToSavePdf = true;
			Status = "Completed rendering directory entries";
		}

		_loading.Release();
	}

	public async Task SavePdf(
		string pdfFileName
    )
    {
		_pdfFilename = pdfFileName;
		ReadyToSavePdf = false;

		string? LicenseKey = _appSettingsServices.Get().SyncFusionLicenseKey;

		if (!_appSettingsServices.GetType().Name.Contains("TestData"))
		{
			if (!string.IsNullOrEmpty(LicenseKey))
			{
				Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(LicenseKey);
			}
			else
			{
				MessageBox.Show(
					"Unable to obtain or register a SyncFusion license key used for producing PDF reports.\n\n"
					+ "You may continue to use the program but if you attempt to produce year end reports, you may get water marks on them.\n\n"
					+ "To obtain a free license key, visit the SyncFusion community license page: https://www.syncfusion.com/sales/communitylicense"
					, "License key");
			}
		}

		Cursor save = Mouse.OverrideCursor;
		Mouse.OverrideCursor = Cursors.Wait;

		try
		{
			var xpsFileName = _fileSystem.Path.GetTempFileName();
			using XpsDocument xpsDocument = new XpsDocument(xpsFileName, FileAccess.ReadWrite);
			XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
			var docpaginator = ((IDocumentPaginatorSource)_pdfFlowDocument).DocumentPaginator;
			docpaginator.PageSize = new Size(_pd.PrintableAreaWidth, _pd.PrintableAreaWidth);
			writer.WritingCompleted += new WritingCompletedEventHandler(AsyncCompleted);
			writer.WritingProgressChanged += new WritingProgressChangedEventHandler(AsyncProgress);
			writer.WriteAsync(docpaginator);

			await Task.Run(() => _waitHandle.WaitOne());

			xpsDocument.Close();

			XPSToPdfConverter xpsToPdfConverter = new XPSToPdfConverter();

			using var xpsStream = _fileSystem.File.OpenRead(xpsFileName);
			PdfDocument? pdfDocument = null;
			await Task.Run(() =>
			{
				pdfDocument = xpsToPdfConverter.Convert(xpsStream);
			});

			if (!string.IsNullOrEmpty(PdfPassword))
			{
				PdfSecurity security = pdfDocument.Security;

				security.KeySize = PdfEncryptionKeySize.Key128Bit;
				security.Algorithm = PdfEncryptionAlgorithm.RC4;
				security.UserPassword = PdfPassword;
			}

			using var pdfStream = _fileSystem.File.Create(pdfFileName);
			pdfDocument.Save(pdfStream);

			pdfDocument.Close(true);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}

		Mouse.OverrideCursor = save;
		ReadyToSavePdf = true;
	}

	private void AsyncCompleted(object sender, WritingCompletedEventArgs e)
	{
		_waitHandle.Set();
		Status = $"PDF rending complete ({_pdfFilename})";
	}

	private void AsyncProgress(object sender, WritingProgressChangedEventArgs e)
	{
		Status = $"Rendering PDF document page: {e.Number}";
	}
}
