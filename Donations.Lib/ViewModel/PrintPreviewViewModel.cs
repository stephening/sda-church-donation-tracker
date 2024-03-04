using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.View;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Donations.Lib.ViewModel;

public partial class PrintPreviewViewModel : ObservableObject
{
	private readonly IIndividualReportServices _individualReportServices;
	private readonly IPrintSettingsServices _printSettingsServices;
	private FlowDocument? _doc;
	private Action<FlowDocument, string?, double, double>? _renderAction;
	private enumPrintout? _printoutType;

	[ObservableProperty]
	private string _selectedFont;
	partial void OnSelectedFontChanged(string value)
	{
		if (null != _renderAction) _renderAction(_doc, SelectedFont, SelectedSize, (8.5 - LeftMargin - OtherMargins) * PrintOptionsView._dpi);
	}

	[ObservableProperty]
	private double _selectedSize;
	partial void OnSelectedSizeChanged(double value)
	{
		if (null != _renderAction) _renderAction(_doc, SelectedFont, SelectedSize, (8.5 - LeftMargin - OtherMargins) * PrintOptionsView._dpi);
	}

	[ObservableProperty]
	private double _leftMargin;
	partial void OnLeftMarginChanged(double value)
	{
		if (null != _renderAction) _renderAction(_doc, SelectedFont, SelectedSize, (8.5 - LeftMargin - OtherMargins) * PrintOptionsView._dpi);
	}

	[ObservableProperty]
	private double _otherMargins;
	partial void OnOtherMarginsChanged(double value)
	{
		if (null != _renderAction) _renderAction(_doc, SelectedFont, SelectedSize, (8.5 - LeftMargin - OtherMargins) * PrintOptionsView._dpi);
	}

	[RelayCommand]
	private void Print()
	{
		PrintDialog pd = new PrintDialog();
		Thickness margin = new Thickness(LeftMargin * PrintOptionsView._dpi,
									OtherMargins * PrintOptionsView._dpi,
									OtherMargins * PrintOptionsView._dpi,
									OtherMargins * PrintOptionsView._dpi);

		if (pd.ShowDialog() == true)
		{
			_doc.PageWidth = pd.PrintableAreaWidth;
			_doc.PageHeight = pd.PrintableAreaHeight;
			_doc.ColumnWidth = _doc.PageWidth;
			_doc.PagePadding = margin;
			var docpaginator = ((IDocumentPaginatorSource)_doc).DocumentPaginator;
			docpaginator.PageSize = new System.Windows.Size(pd.PrintableAreaWidth, pd.PrintableAreaWidth);
			IDocumentPaginatorSource idocument = _doc as IDocumentPaginatorSource;

			pd.PrintDocument(idocument.DocumentPaginator, "Category report");
		}
	}

	public PrintPreviewViewModel(
		IIndividualReportServices individualReportServices,
		IPrintSettingsServices printSettingsServices)
	{
		_individualReportServices = individualReportServices;
		_printSettingsServices = printSettingsServices;
	}

	public void SetRenderAction(enumPrintout printoutType, Action<FlowDocument, string?, double, double> renderAction, FlowDocument doc)
	{
		_printoutType = printoutType;
		var settings = _printSettingsServices.Get(printoutType);

		SelectedFont = settings.FontFamily;
		SelectedSize = settings.FontSize;
		LeftMargin = settings.LeftMargin;
		OtherMargins = settings.OtherMargins;

		_renderAction = renderAction;
		_doc = doc;

		_renderAction(_doc, SelectedFont, SelectedSize, (8.5 - LeftMargin - OtherMargins) * PrintOptionsView._dpi);
	}

	public void SaveSettings()
	{
		if (null != _printoutType)
		{
			PrintSettings settings = new PrintSettings()
			{
				PrintoutType = (int)_printoutType,
				FontFamily = SelectedFont,
				FontSize = SelectedSize,
				LeftMargin = LeftMargin,
				OtherMargins = OtherMargins
			};
			_printSettingsServices?.Save(settings);
		}
	}
}
