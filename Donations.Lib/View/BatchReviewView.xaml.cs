using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for BatchReview.xaml
/// </summary>
public partial class BatchReviewView : Window
{
	private BatchReviewViewModel? _viewModel;
	private readonly HelpView _helpView;
	private Batch? _batch;
	private ObservableCollection<Donation>? _batchDonations;

	public delegate BatchReviewView Factory(Batch? batch, ObservableCollection<Donation>? batchDonations);

	public BatchReviewView(
		BatchReviewViewModel batchReviewViewModel,
		HelpView helpView,
		Batch? batch,
		ObservableCollection<Donation>? batchDonations)
	{
		_viewModel = batchReviewViewModel;
		_helpView = helpView;
		_batch = batch;
		_batchDonations = batchDonations;

		InitializeComponent();

		DataContext = _viewModel;
	}

	private async void DataGridRow_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
	{
		DataGridRow row = (DataGridRow)sender;
		CategorySum? categorySum = (CategorySum?)row?.DataContext;

		var dlg = _viewModel?.CreateCategoryReviewView(categorySum);

		dlg.ShowDialog();
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		Width = Persist.Default.ReviewWidth;
		Height = Persist.Default.ReviewHeight;
		Top = Persist.Default.ReviewTop;
		Left = Persist.Default.ReviewLeft;
		if (!string.IsNullOrEmpty(Persist.Default.ReviewWindowState))
			WindowState = Enum.Parse<WindowState>(Persist.Default.ReviewWindowState);

		await _viewModel?.Review(_batch, _batchDonations, () => { Close(); });
		await (ReviewDonationSummary.DataContext as DonorInputViewModel)?.Review(_batch, _batchDonations, Close);
		await (PrintBatch.DataContext as BatchPrintViewModel)?.Loaded(PrintBatch.PrintArea, _batch, _batchDonations);
	}

	private void Window_Unloaded(object sender, RoutedEventArgs e)
	{
		Persist.Default.ReviewTop = Top;
		Persist.Default.ReviewLeft = Left;
		Persist.Default.ReviewWidth = Width;
		Persist.Default.ReviewHeight = Height;
		Persist.Default.ReviewWindowState = WindowState.ToString();
		Persist.Default.Save();

		_helpView.ForceClose();
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

	private void ByDonorTabClicked(object sender, RoutedEventArgs e)
	{
		(ReviewDonationSummary.DataContext as DonorInputViewModel)?.UpdateBatchParams(_viewModel.BatchDate, _viewModel.BatchNote, _viewModel.BatchTotal);
	}

	private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		_viewModel = DataContext as BatchReviewViewModel;
	}

	private void Window_KeyDown(object sender, KeyEventArgs e)
	{
		if (Key.F1 == e.Key)
		{
			string baseTarget = "Batch-review-popup";
			TabControl tab = TabControl;
			var item = tab.SelectedItem as TabItem;
			string target = "";
			string s;

			if (-1 == tab.SelectedIndex || true == item?.Header.Equals(s = (string)FindResource("BatchReviewByCategoryTabHeader")))
			{
				target = baseTarget + '-' + ((string)FindResource("BatchReviewByCategoryTabHeader")).Replace(" ", "-");
			}
			else if (true == item?.Header.Equals(s = (string)FindResource("BatchReviewByDonorTabHeader")))
			{
				target = baseTarget + '-' + s.Replace(" ", "-");
			}
			else if (true == item?.Header.Equals(s = (string)FindResource("BatchReviewPrintTabHeader")))
			{
				target = baseTarget + '-' + s.Replace(" ", "-");
			}

			_helpView.ShowTarget(target);
		}
	}
}
