using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for DonationBrowserView.xaml
/// </summary>
public partial class DonationBrowserView : UserControl
{
	private DonationBrowserViewModel? _viewModel;

	public DonationBrowserView()
	{
		InitializeComponent();
	}

	private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
	{
		_viewModel = (DonationBrowserViewModel)DataContext;
	}

	private void CategoryFilterText_TextChanged(object sender, TextChangedEventArgs e)
	{
		_viewModel?.CategoryTextChanged();
	}

	private void DonationFilterText_TextChanged(object sender, TextChangedEventArgs e)
	{
		_viewModel?.DonationTextChanged();
	}

	private void CheckBoxChanged(object sender, RoutedEventArgs e)
	{
		_viewModel?.DonationTextChanged();
	}

	private void DoubleClick_CategoryRow(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		DataGridRow row = (DataGridRow)sender;
		CategorySum? catsum = (CategorySum?)row?.DataContext;

		try
		{
#pragma warning disable CS8604 // Possible null reference argument.
			CategoryReviewView dlg = _viewModel?.CreateCategoryReviewView(catsum);
#pragma warning restore CS8604 // Possible null reference argument.

			dlg.ShowDialog();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Unexpected exception wtih categoryreview");
		}
	}

	private async void DoubleClick_DonationRow(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		DataGridRow row = (DataGridRow)sender;
		Donation? donation = (Donation?)row?.DataContext;

		try
		{
			DonationPopupView dlg = await _viewModel?.CreateDonationPopupView(donation.Id);

			dlg.ShowDialog();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Unexpected exception wtih DonationPopupView");
		}
	}
}
