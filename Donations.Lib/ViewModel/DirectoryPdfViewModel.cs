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
using CommunityToolkit.Mvvm.Input;
using System.Windows.Threading;

namespace Donations.Lib.ViewModel;

public partial class DirectoryPdfViewModel : BaseViewModel
{
	private PrintDialog? _pd;
	private double _width;
	private readonly IFileSystem _fileSystem;
	private readonly ILogger _logger;
	private readonly IAppSettingsServices _appSettingsServices;
	private readonly IPdfDirectoryServices _pdfDirectoryServices;
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private Dictionary<string, DirectoryData>? _directoryEntries;
	private FlowDocument? _doc;
	private FlowDocument? _pdfFlowDocument;
	private AutoResetEvent _waitHandle = new AutoResetEvent(false);
	private string _pdfFilename = "";
	private bool _cancelLoading = false;
	private Semaphore _loading = new Semaphore(1, 1);
	private Section _coverSection = new Section();
	private Section _pdfCoverSection = new Section();
	private bool _initilizingRtb = false;
	private bool _rtbChanged = false;

	public DirectoryPdfViewModel(
		IFileSystem fileSystem,
		ILogger logger,
		IAppSettingsServices appSettingsServices,
		IPdfDirectoryServices pdfDirectoryServices,
		IDispatcherWrapper dispatcherWrapper
    )
    {
		_fileSystem = fileSystem;
		_logger = logger;
		_appSettingsServices = appSettingsServices;
		_pdfDirectoryServices = pdfDirectoryServices;
		_dispatcherWrapper = dispatcherWrapper;

		_delayedUpdateSettingsTimer.Tick += new EventHandler(UpdateSettings!);
		_delayedUpdateSettingsTimer.Interval = new TimeSpan(0, 0, 5);
	}

	[ObservableProperty]
	private string? _selectedFont;

	[ObservableProperty]
	private double _selectedSize;
	/// <summary>
	/// The SelectedSize prperty is used to initially select the last font size used, and also to
	/// receive the latest font size chosen by the operator.
	/// </summary>
	partial void OnSelectedSizeChanged(double value)
	{
		if (null != _doc)
		{
			_doc.FontSize = value;
		}
	}

	/// <summary>
	/// The PageWidth property is used for specifying the Pdf document page size.
	/// </summary>
	[ObservableProperty]
	private double _pageWidth;

	/// <summary>
	/// The PageHeight property is used for specifying the Pdf document page size.
	/// </summary>
	[ObservableProperty]
	private double _pageHeight;

	/// <summary>
	/// The LeftMargin property is provided for the sole purpose of allowing a larger left margin
	/// for three ring binding. Otherwise, I would have only given a single margin parameter.
	/// </summary>
	[ObservableProperty]
	private double _leftMargin;

	/// <summary>
	/// The OtherMargins property contains a single value that is applied to the top/right/bottom
	/// page margins.
	/// </summary>
	[ObservableProperty]
	private double _otherMargins;

	[ObservableProperty]
	private bool _address;
	partial void OnAddressChanged(bool value)
	{
#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
	}

	[ObservableProperty]
	private bool _email;
	partial void OnEmailChanged(bool value)
	{
#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
	}

	[ObservableProperty]
	private bool _phoneNumber;
	partial void OnPhoneNumberChanged(bool value)
	{
#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
	}

	[ObservableProperty]
	private string? _coverText;
	partial void OnCoverTextChanged(string? value)
	{
		GenerateCoverPage(_coverSection);
		GenerateCoverPage(_pdfCoverSection);
	}

	[ObservableProperty]
	private RichTextBoxContainer _rtbContainer = new RichTextBoxContainer();

	[ObservableProperty]
	private bool _readyToSavePdf = false;

	[ObservableProperty]
	private string _status = "";

	[ObservableProperty]
	private double _progress = 0;

	[ObservableProperty]
	private string _pdfPassword = "";

	[RelayCommand]
	private void RichTextChanged()
	{
		if (_initilizingRtb) return;

		_rtbChanged = true;

		_delayedUpdateSettingsTimer.Stop();
		_delayedUpdateSettingsTimer.Start();
	}

	public void SetDirectoryEntries(Dictionary<string, DirectoryData>? directoryEntries)
	{
		_directoryEntries = directoryEntries;

#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
	}

	/// <summary>
	/// When this one second timer expires, then the settings will be written to the database.
	/// If a change in these fields is detected before the timer expires,
	/// the unexpired timer will be canceled and a new 1 second timer will be started.
	/// </summary>
	private DispatcherTimer _delayedUpdateSettingsTimer = new DispatcherTimer();

	private async void UpdateSettings(object sender, EventArgs e)
	{
		_delayedUpdateSettingsTimer.Stop();

#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
	}

	public async Task SetDocument(
		FlowDocument flowDocument
	)
	{
		var data = await _pdfDirectoryServices.GetAsync();
		SelectedFont = data.Font;
		SelectedSize = data.FontSize;
		PageWidth = data.PageWidth;
		PageHeight = data.PageHeight;
		LeftMargin = data.LeftMargin;
		OtherMargins = data.OtherMargins;
		Address = data.IncludeAddress;
		Email = data.IncludeEmail;
		PhoneNumber = data.IncludePhone;

		if (null != data.CoverRtf)
		{
			_initilizingRtb = true;
			RtbContainer.SetRichText(data.CoverRtf);
			_initilizingRtb = false;
		}

		_doc = flowDocument;

		_pd = new PrintDialog();
		Thickness margin = new Thickness(LeftMargin * PrintOptionsView._dpi,
									OtherMargins * PrintOptionsView._dpi,
									OtherMargins * PrintOptionsView._dpi,
									OtherMargins * PrintOptionsView._dpi);

		_doc.PageWidth = PageWidth * PrintOptionsView._dpi;
		_doc.PageHeight = PageHeight * PrintOptionsView._dpi;
		_doc.ColumnWidth = _doc.PageWidth;
		_doc.PagePadding = margin;

		_width = _doc.PageWidth - margin.Left - margin.Right;

		_doc.Blocks.Clear();
		_doc.Blocks.Add(_coverSection);
		GenerateCoverPage(_coverSection);
	}

	private async Task AddEntry(Section section, string key, string basePictureUrl)
	{
		DirectoryData data = _directoryEntries[key];
		var table = new Table() { BorderThickness = new Thickness(2), BorderBrush = new SolidColorBrush(Colors.Black)};
		table.RowGroups.Add(new TableRowGroup());
		var row = new TableRow() { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize };
		var column = new TableColumn() { Width = new GridLength(1, GridUnitType.Star) };
		table.Columns.Add(column);
		column = new TableColumn() { Width = new GridLength(2, GridUnitType.Star) };
		table.Columns.Add(column);

		Image cellImage = new Image() { Stretch = Stretch.Uniform, Width = _width / 3, HorizontalAlignment = HorizontalAlignment.Left };
		if (!string.IsNullOrEmpty(data.Picture))
		{
			cellImage.Source = new BitmapImage(new Uri(basePictureUrl + data.Picture, UriKind.Absolute));
			await Task.Delay(20);
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

		row.Cells.Add(new TableCell(new Paragraph(new Run(entry))) { TextAlignment = TextAlignment.Left, Padding = new Thickness(PrintOptionsView._dpi * 0.05) });

		table.RowGroups[0].Rows.Add(row);
		section.Blocks.Add(table);
	}

	private void GenerateCoverPage(Section section)
	{
		RtbContainer?.RichTextToSection(section);
	}

	public new async Task Leaving()
	{
		var data = await _pdfDirectoryServices.GetAsync();

		bool changed = false;
		if (data.Font != SelectedFont)
		{
			data.Font = SelectedFont;
			changed = true;
		}

		if (data.FontSize != SelectedSize)
		{
			data.FontSize = SelectedSize;
			changed = true;
		}

		if (PageWidth != data.PageWidth)
		{
			data.PageWidth = PageWidth;
			changed = true;
		}

		if (PageHeight != data.PageHeight)
		{
			data.PageHeight = PageHeight;
			changed = true;
		}

		if (LeftMargin != data.LeftMargin)
		{
			data.LeftMargin = LeftMargin;
			changed = true;
		}

		if (OtherMargins != data.OtherMargins)
		{
			data.OtherMargins = OtherMargins;
			changed = true;
		}

		if (Address != data.IncludeAddress)
		{
			data.IncludeAddress = Address;
			changed = true;
		}

		if (Email != data.IncludeEmail)
		{
			data.IncludeAddress = Email;
			changed = true;
		}

		if (PhoneNumber != data.IncludePhone)
		{
			data.IncludePhone = PhoneNumber;
			changed = true;
		}

		if (_rtbChanged)
		{
			data.CoverRtf = RtbContainer?.GetRichText();
			changed = true;
		}

		if (changed)
		{
			await _pdfDirectoryServices?.Save();
		}
	}

	public new async Task Loading()
	{
		// we don't want multiple instances of Loading() running simultaneously,
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
				PageWidth = PageWidth * PrintOptionsView._dpi,
				PageHeight = PageHeight * PrintOptionsView._dpi,
				ColumnWidth = PageWidth * PrintOptionsView._dpi,
				PagePadding = margin
			};
			_pdfFlowDocument.Blocks.Add(_pdfCoverSection);

			_doc.Blocks.Clear();
			_doc.Blocks.Add(_coverSection);
			var basePictureUrl = _appSettingsServices.Get()?.PictureBaseUrl;
			var keys = _directoryEntries.Keys.Order();
			double total = _directoryEntries.Count;

			GenerateCoverPage(_coverSection);
			GenerateCoverPage(_pdfCoverSection);

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
			MessageBox.Show(ex.Message, "DirectoryPdfViewModel.cs");
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
