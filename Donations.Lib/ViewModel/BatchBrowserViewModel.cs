using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the BatchBrowserView.xaml which is a
/// UserControl under the 'Batch browser' tab.
/// 
/// The view associated with this view model is displayed on the default tab and the first thing the
/// user see's when launching the application. It is a way of seeing the donations entered by batch.
/// Donations are usually entered in batches and there is usually an expected Total that should match
/// the sum of all the individual donations. Whether these two totals match is indicated by whether 
/// they are colored with a red background or not.
/// 
/// Details of each batch can be reviewed or edited if needed by double clicking on a row in the batch
/// browser view.
/// </summary>
public partial class BatchBrowserViewModel : BaseTimeWindowViewModel
{
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private readonly BatchReviewView.Factory _batchReviewFactory;
	private readonly IBatchServices _batchServices;
	private readonly IDonationServices _donationServices;
	private bool _dontFilterYet = true;
	private DispatcherTimer _timer = new DispatcherTimer();
	private ObservableCollection<Batch> _batches;

	/// <summary>
	/// The constructor places its this pointer in the Global static object for use by other ViewModels
	/// that may need access to its public members. It also adds the year or date range Filter to the
	/// CollectionViewSource object. The it calls BatchListUpdated() to further perform initialization.
	/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public BatchBrowserViewModel(
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		IDispatcherWrapper dispatcherWrapper,
		BatchReviewView.Factory batchReviewFactory,
		IBatchServices batchServices,
		IDonationServices donationServices
	)
	{
		_dispatcherWrapper = dispatcherWrapper;
		_batchReviewFactory = batchReviewFactory;
		_batchServices = batchServices;
		_donationServices = donationServices;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		BatchListUpdated();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
		_timer.Tick += new EventHandler(Timer_Tick);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
		_timer.Interval = new TimeSpan(0, 0, 1);

		_timer.Start();
	}

	public CollectionViewSource BatchListSource { get; set; } = new CollectionViewSource();

	[ObservableProperty]
	private int _selectedIndex;

	/// <summary>
	/// This method invokes the AutoFac factory that create the BatchReviewView object with parameters
	/// </summary>
	/// <param name="batch">Batch to be viewed in the popup window</param>
	/// <param name="batchDonations">donation collection to be viewed in the popup window</param>
	/// <returns></returns>
	public BatchReviewView CreateBatchReviewView(Batch? batch, ObservableCollection<Donation>? batchDonations)
	{
		return _batchReviewFactory(batch, batchDonations);
	}

	/// <summary>
	/// Get donation collection by batch id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public async Task<ObservableCollection<Donation>> GetDonationsByBatchId(int id)
	{
		return await _donationServices.GetDonationsByBatchId(id);
	}

	/// <summary>
	/// Dispatcher timer tick handler
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Timer_Tick(object sender, EventArgs e)
	{
		// delay db queries for 1 sec until ui settles
		_timer.Stop();
		_dontFilterYet = false;
		_dispatcherWrapper.BeginInvoke(Batch_Filter);
	}

	/// <summary>
	/// This member is called when the BatchBrowser tab is clicked, it will update the view.
	/// </summary>
	public new async Task Loading()
	{
		await Batch_Filter();
	}

	/// <summary>
	/// This function is BeginInvoke'd, when the batchlist needs to be updated. Re-querying the list
	/// is an async await operation that can't be awaited on from some locations. It is invoked when a
	/// condition changes which will alter the batch entries shown, among them radio button selection.
	/// </summary>
	private async Task Batch_Filter()
	{
		if (_dontFilterYet) return;

		string date = "";
		string date2 = "";

		switch (DateFilterOption)
		{
			case enumDateFilterOptions.CurrentYear:
				date = _thisYear;
				break;
			case enumDateFilterOptions.PreviousYear:
				date = _prevYear;
				break;
			case enumDateFilterOptions.SelectYear:
				date = FilterYear;
				break;
			case enumDateFilterOptions.DateRange:
				date = FilterStartDate;
				date2 = FilterEndDate;
				break;
		}

#pragma warning disable CS8604 // Possible null reference argument.
		_batches = await _batchServices.FilterBatch(DateFilterOption, date, date2);
#pragma warning restore CS8604 // Possible null reference argument.
		if (null != _batches)
		{
			BatchListSource.Source = _batches;
			BatchListSource.View.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
			BatchListSource.View.Refresh();
			if (0 < _batches.Count)
			{
				SelectedIndex = 0;
			}
		}

		SelectionEnabled = true;
	}

	/// <summary>
	/// This method is called in response to the right-click context menu:'Delete row'.
	/// This is the method bound to the Command, and it will delete the row that was 
	/// right-clicked on to get the context menu.
	/// </summary>
	[RelayCommand]
	public async Task DeleteRow()
	{
		Batch remove = BatchListSource.View.CurrentItem as Batch;
		if (remove == null) return;

		// in case of review/edit we'll retain the same batchid, but remove all donations using that
		// id and then write new donation with the same id
		await _donationServices.RemoveDonationsByBatchId(remove.Id);

		await _batchServices.DeleteBatch(remove.Id);

		await Batch_Filter();
	}

	/// <summary>
	/// This method will update the AvailableYears collection using a Linq filter. It also
	/// set's the start/end range dates to the earliest and latest years from the list of batches.
	/// </summary>
	public async Task BatchListUpdated()
	{
		var list = await _batchServices.GetBatchYears();
		AvailableYears = new ObservableCollection<string>(list.OrderByDescending(i => i));
		FilterYear = AvailableYears.Max();
		FilterStartDate = await _batchServices.GetEarliestDate();
		FilterEndDate = await _batchServices.GetLatestDate();

		await Batch_Filter();
	}

	/// <summary>
	/// This method is used to update the list, if new donations have been added
	/// and the list should be updated when returning to this tab.
	/// </summary>
	public void Refresh()
	{
		BatchListSource.View.Refresh();
	}

	/// <summary>
	/// This overidden handler is invoked from the BaseTimeWindowViewModel when either the 
	/// window type changes, or the selected year or date range changes.
	/// </summary>
	/// <returns></returns>
	public override async Task TimeWindowChanged()
	{
		await Batch_Filter();
	}
}
