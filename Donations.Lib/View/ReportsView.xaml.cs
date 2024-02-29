using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using WPFFolderBrowser;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for ReportsView.xaml
/// </summary>
public partial class ReportsView : UserControl
{
	private ReportsViewModel? _viewModel;

	public ReportsView()
	{
		InitializeComponent();
	}

	private async void Choose_Donor(object sender, EventArgs e)
	{
		DonorSelectionView dlg = DependencyInjection.DonorSelectionView;
		if (true == dlg.ShowDialog())
		{
			var SelectedDonor = (Donor)dlg.DonorGrid.SelectedItem;
			await _viewModel!.SetDonor(SelectedDonor);
		}
	}

	private async void DoAction(object sender, RoutedEventArgs e)
	{
		PrintDialog pd = new PrintDialog();
		Thickness margin = new Thickness(_viewModel!.LeftMargin * PrintOptionsView._dpi,
									_viewModel.OtherMargins * PrintOptionsView._dpi,
									_viewModel.OtherMargins * PrintOptionsView._dpi,
									_viewModel.OtherMargins * PrintOptionsView._dpi);

		switch (_viewModel!.ReportOption)
		{
			case enumReportOptions.Individual:
				if (pd.ShowDialog() == true)
				{
					await _viewModel!.PrintIndividual(pd, margin);
				}
				break;
			case enumReportOptions.AllPdf:
				//WPFFolderBrowserDialog folderBrowser = new WPFFolderBrowserDialog();
				SaveFileDialog folderBrowser = new SaveFileDialog();
				if (true == folderBrowser.ShowDialog())
				{
					await _viewModel!.AllPdf(folderBrowser.FileName, pd, margin);
				}
				break;
			case enumReportOptions.Email:
				await _viewModel!.Email(pd, margin);
				break;
			case enumReportOptions.Print:
				if (pd.ShowDialog() == true)
				{
					await _viewModel!.Print(pd, margin);
				}
				break;
			case enumReportOptions.MockRun:
				await _viewModel!.MockRun();
				break;
		}
	}

	private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		dynamic selected = MergeFields.SelectedValue;
		int pos = TemplateText.CaretIndex;
		TemplateText.Text = TemplateText.Text.Insert(pos, "{" + selected.Value + "}");
		TemplateText.CaretIndex = pos + selected.Value.ToString().Length + 2;
		TemplateText.Focus();
	}

	private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		_viewModel = DataContext as ReportsViewModel;
		_viewModel!.SetFlowDoc(PrintPreview);
	}

	private async void Choose_Donor(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		DonorSelectionView dlg = DependencyInjection.DonorSelectionView;

		if (true == dlg.ShowDialog())
		{
			await _viewModel!.SetDonor((Donor)dlg.DonorGrid.SelectedItem);
		}
	}

	private void FilterText_TextChanged(object sender, TextChangedEventArgs e)
	{
		_viewModel!.TextChanged();
	}

	private async void DoubleClick_Row(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		DataGridRow row = (DataGridRow)sender;
		NamedDonorReport? donor = (NamedDonorReport?)row?.DataContext;

		try
		{
			DonorModalView dlg = await _viewModel!.CreateDonorModalView(donor.DonorId);

			dlg.ShowDialog();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Unexpected exception wtih categoryreview");
		}
	}
}
