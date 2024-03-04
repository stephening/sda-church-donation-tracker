using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for CategoryReviewView.xaml
/// </summary>
public partial class CategoryReviewView : Window
{
	private readonly CategoryReviewViewModel _viewModel;
	private readonly PrintPreviewView.Factory _printPreviewViewFactory;
	private readonly string _helpTarget;
	private readonly string _helpPrintPrevewTarget;
	private readonly HelpView _helpView;

	public delegate CategoryReviewView Factory(CategorySum? categorySum, enumCategoryReviewType categoryReviewType, ObservableCollection<Donation>? donations, string timeWindowText);

	public CategoryReviewView(
		CategoryReviewViewModel categoryReviewViewModel,
		PrintPreviewView.Factory printPreviewViewFactory,
		HelpView helpView,
		CategorySum categorySum,
		enumCategoryReviewType categoryReviewType,
		ObservableCollection<Donation> donations,
		string timeWindowText
		)
	{
		switch (categoryReviewType)
		{
			case enumCategoryReviewType.Batch:
				_helpTarget = "Batch-contributions-popup";
				_helpPrintPrevewTarget = "Batch-contributions-printout";
				break;
			case enumCategoryReviewType.Donation:
				_helpTarget = "Donor-contributions-popup";
				_helpPrintPrevewTarget = "Donor-contributions-printout";
				break;
		}
		_viewModel = categoryReviewViewModel;
		_printPreviewViewFactory = printPreviewViewFactory;
		_helpView = helpView;

		InitializeComponent();

		DataContext = _viewModel;
		_viewModel.Setup(categorySum, donations, timeWindowText);
	}

	private void Render(FlowDocument document, string? font, double size, double printAreaWidth)
	{
		_viewModel?.CreatePreview(document, font, size, printAreaWidth);
	}

	private void PrintPreview(object sender, RoutedEventArgs e)
	{
		PrintPreviewView? dlg = _printPreviewViewFactory(enumPrintout.CategoryReport, _helpPrintPrevewTarget, Render);

		dlg?.ShowDialog();
	}

	private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
	{
		_helpView.ShowTarget(_helpTarget);
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		Width = (0 <= Persist.Default.CategoryReviewWidth) ? Persist.Default.CategoryReviewWidth : Width;
		Height = (0 < Persist.Default.CategoryReviewHeight) ? Persist.Default.CategoryReviewHeight : Height;
		Top = Persist.Default.CategoryReviewTop;
		Left = Persist.Default.CategoryReviewLeft;
		if (!string.IsNullOrEmpty(Persist.Default.CategoryReviewWindowState))
			WindowState = Enum.Parse<WindowState>(Persist.Default.CategoryReviewWindowState);
	}

	private void Window_Unloaded(object sender, RoutedEventArgs e)
	{
		Persist.Default.CategoryReviewTop = Top;
		Persist.Default.CategoryReviewLeft = Left;
		Persist.Default.CategoryReviewWidth = Width;
		Persist.Default.CategoryReviewHeight = Height;
		Persist.Default.CategoryReviewWindowState = WindowState.ToString();
		Persist.Default.Save();

		_helpView.ForceClose();
	}
}
