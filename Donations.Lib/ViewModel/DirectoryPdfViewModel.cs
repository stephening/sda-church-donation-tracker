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
using System.Diagnostics;
using System.Reflection.Metadata;

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

	public new async Task Loading()
	{
		if (null != _directoryEntries && null != _doc)
		{
			_doc.Blocks.Clear();
			var basePictureUrl = _appSettingsServices.Get()?.PictureBaseUrl;
			var keys = _directoryEntries.Keys.Order();
			double total = _directoryEntries.Count;

			double c = 0;

			foreach (string key in keys)
			{
				Progress = 100 * c / total;
				c++;
				DirectoryData data = _directoryEntries[key];

				var table = new Table() { BorderThickness = new Thickness(2), BorderBrush = new SolidColorBrush(Colors.Black) };
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
				_doc.Blocks.Add(table);
			}

			ReadyToSavePdf = true;
		}
	}

	public async Task SavePdf(
        FlowDocument flowDocument,
		string pdfFileName
    )
    {
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

		_doc = flowDocument;
		await SavePdf(pdfFileName);
	}

	private async Task<FlowDocument?> CopyFlowDocument(FlowDocument from)
	{
		await Task.Run(() =>
		{
			FlowDocument to = new FlowDocument();
			TextRange range = new TextRange(from.ContentStart, from.ContentEnd);
			MemoryStream stream = new MemoryStream();
			System.Windows.Markup.XamlWriter.Save(range, stream);
			_dispatcherWrapper.Invoke(() => range.Save(stream, DataFormats.XamlPackage));
			TextRange range2 = new TextRange(to.ContentStart, to.ContentEnd);
			range2.Load(stream, DataFormats.XamlPackage);

			return to;
		});
		return null;
	}

	private async Task SavePdf(string pdfFileName)
	{
		Cursor save = Mouse.OverrideCursor;
		Mouse.OverrideCursor = Cursors.Wait;

		try
		{
			//await Task.Run(() => {
				System.IO.MemoryStream s = new System.IO.MemoryStream();
				TextRange source = new TextRange(_doc.ContentStart, _doc.ContentEnd);
				source.Save(s, DataFormats.Xaml);
				FlowDocument tempDoc = new FlowDocument();
				TextRange dest = new TextRange(tempDoc.ContentStart, tempDoc.ContentEnd);
				dest.Load(s, DataFormats.Xaml);

				tempDoc.PageWidth = _doc.PageWidth;
				tempDoc.PageHeight = _doc.PageHeight;
				tempDoc.ColumnWidth = _doc.ColumnWidth;
				tempDoc.PagePadding = _doc.PagePadding;

				var xpsFileName = _fileSystem.Path.GetTempFileName();
				using XpsDocument xpsDocument = new XpsDocument(xpsFileName, FileAccess.ReadWrite);
				XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
				var docpaginator = ((IDocumentPaginatorSource)tempDoc).DocumentPaginator;
				docpaginator.PageSize = new Size(_pd.PrintableAreaWidth, _pd.PrintableAreaWidth);
				writer.Write(docpaginator);
				xpsDocument.Close();

			XPSToPdfConverter xpsToPdfConverter = new XPSToPdfConverter();

			using var xpsStream = _fileSystem.File.OpenRead(xpsFileName);
			PdfDocument pdfDocument = xpsToPdfConverter.Convert(xpsStream);

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

			//});
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message);
		}

		Mouse.OverrideCursor = save;
	}
}
