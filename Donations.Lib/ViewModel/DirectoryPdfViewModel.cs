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

namespace Donations.Lib.ViewModel;

public partial class DirectoryPdfViewModel : BaseViewModel
{
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

	private void AddHeader(FlowDocument flowDoc)
	{
		// Create a Paragraph to hold the page number information
		Paragraph paragraph = new Paragraph();
		paragraph.TextAlignment = TextAlignment.Center;
		paragraph.FontSize = 12;
		paragraph.Inlines.Add(new Run("Page "));
		paragraph.Inlines.Add(new Run(new Bold(new Run("{0}"))));
		paragraph.Inlines.Add(new Run(" of "));
		paragraph.Inlines.Add(new Run(new Bold(new Run("{1}"))));

		// Format the string using the current page number and the total page count
		string headerText = string.Format("Page {0} of {1}", "{PageNumber}", "{PageCount}");

		// Substitute the header text into the Runs that represent the page number and page count
		((Bold)paragraph.Inlines.ElementAt(1)).Inlines.Add(new Run(headerText));
		((Bold)paragraph.Inlines.ElementAt(3)).Inlines.Add(new Run(headerText));

		// Create a FixedPage to hold the Paragraph
		FixedPage fixedPage = new FixedPage()
		{
			Width = flowDoc.PageWidth,
			Height = flowDoc.PageHeight,
		};
		BlockUIContainer container = new BlockUIContainer();
		container.Child = paragraph;
		fixedPage.Children.Add(container);

		// Add the FixedPage to the PageHeader property of the FlowDocument
		flowDoc.PageHeader = fixedPage;
	}

	private async Task AddEntry(FlowDocument flowDocument, string key, string basePictureUrl)
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

		row.Cells.Add(new TableCell(new Paragraph(new Run(
			data.Name + "\r\n" +
			data.OtherFamilyMembers + "\r\n" +
			data.Address + "\r\n" +
			data.Email + "\r\n" +
			data.Phone
			)))
		{ TextAlignment = TextAlignment.Left });

		table.RowGroups[0].Rows.Add(row);
		flowDocument.Blocks.Add(table);
	}

	public new async Task Loading()
	{
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

			double c = 0;
			Status = "Rendering directory entries";

			foreach (string key in keys)
			{
				Progress = 100 * c / total;
				c++;
				await AddEntry(_doc, key, basePictureUrl);
				await AddEntry(_pdfFlowDocument, key, basePictureUrl);
			}

			ReadyToSavePdf = true;
			Status = "Completed rendering directory entries";
		}
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
