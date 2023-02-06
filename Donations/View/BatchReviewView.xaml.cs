using Donations.Model;
using Donations.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for BatchReview.xaml
	/// </summary>
	public partial class BatchReviewView : Window
	{
		private BatchReviewViewModel? _viewModel;
		private Batch? _batch;
		private ObservableCollection<Donation>? _batchDonations;

		public BatchReviewView(Batch? batch, ObservableCollection<Donation>? batchDonations)
		{
			_batch = batch;
			_batchDonations = batchDonations;

			InitializeComponent();

			_viewModel = DataContext as BatchReviewViewModel;
		}

		private void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			DataGridRow row = (DataGridRow)sender;
			CategorySum? categorySum = (CategorySum?)row?.DataContext;

			DonorContributionsView dlg = new DonorContributionsView();

			dlg.ShowDialog(_batch, categorySum);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Width = Persist.Default.ReviewWidth;
			Height = Persist.Default.ReviewHeight;
			Top = Persist.Default.Top;
			Left = Persist.Default.Left;
			if (!string.IsNullOrEmpty(Persist.Default.ReviewWindowState))
				WindowState = Enum.Parse<WindowState>(Persist.Default.ReviewWindowState);

			_viewModel?.Review(_batch, _batchDonations);
			(ReviewDonationSummary.DataContext as DonorInputViewModel)?.Review(_batch, _batchDonations);
			(PrintBatch.DataContext as BatchPrintViewModel)?.Loaded(PrintBatch.PrintArea, _batch, _batchDonations);
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			Persist.Default.Top = Top;
			Persist.Default.Left = Left;
			Persist.Default.ReviewWidth = Width;
			Persist.Default.ReviewHeight = Height;
			Persist.Default.WindowState = WindowState.ToString();
		}

		#region these two functions are used to select all in BatchTotal TextBox
		private void BatchTotal_SelectAll(object sender, MouseButtonEventArgs e)
		{
			(sender as TextBox)?.SelectAll();
		}

		private void BatchTotal_SelectAll(object sender, KeyboardFocusChangedEventArgs e)
		{
			(sender as TextBox)?.SelectAll();
		}

		private void BatchTotal_SelectivelyIgnoreMouseClick(object sender, MouseButtonEventArgs e)
		{
			if (false == (sender as TextBox)?.IsKeyboardFocusWithin)
			{
				e.Handled = true;

				(sender as TextBox)?.Focus();
			}
		}
		#endregion
	}
}
