using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for BatchBrowserView.xaml
/// </summary>
public partial class BatchBrowserView : UserControl
{
	private BatchBrowserViewModel? _viewModel;

	public BatchBrowserView()
	{
		InitializeComponent();
	}

	private async void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		DataGridRow row = (DataGridRow)sender;
		Batch? batch = (Batch?)row?.DataContext;

		try
		{
			ObservableCollection<Donation> batchDonationList = await _viewModel!.GetDonationsByBatchId(batch.Id);

			BatchReviewView dlg = _viewModel.CreateBatchReviewView(batch, batchDonationList);

			dlg.ShowDialog();
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Unexpected exception wtih batchreview");
		}

		_viewModel?.Refresh();
	}

	private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		_viewModel = DataContext as BatchBrowserViewModel;
	}
}
