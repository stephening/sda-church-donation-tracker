using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the BatchReviewView.xaml which is a
/// Window, which will be used as a modal dialog when a batch is double clicked on from the
/// BatchBrowserView.
/// 
/// The BatchReviewView actually has three tabs which show the batch in two different ways.
/// 
///   1. By category view which shows all categories and their sub totals in a DataGrid. There is no
///      editing possible on this view.
///   2. By donor, which uses the DonorInputView UserControl. There is editing possible in this view.
///   3. Print, which uses the BatchPrintView UserControl.
/// </summary>
public partial class BatchReviewViewModel : BaseViewModel
{
	private Dictionary<string, CategorySum> _categorySumDict = new Dictionary<string, CategorySum>();
	private ObservableCollection<Donation>? _batchDonations;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
	private Action _closeDialog = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private readonly IBatchServices _batchServices;
	private readonly CategoryReviewView.Factory _categoryReviewFactory;

	/// <summary>
	/// The constructor initializes the SubmitBatchCmd to the handler for batch submitting. It
	/// also sets the CollectionViewSource for the category summary DataGrid.
	/// </summary>
	public BatchReviewViewModel(
		IDispatcherWrapper dispatcherWrapper,
		IBatchServices batchServices,
		CategoryReviewView.Factory categoryReviewFactory,
		DonorInputViewModel donorInputViewModel,
		BatchPrintViewModel batchPrintViewModel
	)
	{
		_dispatcherWrapper = dispatcherWrapper;
		_batchServices = batchServices;
		_categoryReviewFactory = categoryReviewFactory;
		DonorInputViewModel = donorInputViewModel;
		BatchPrintViewModel = batchPrintViewModel;

		CategorySumSource.Source = CategorySums;
		CategorySumSource.SortDescriptions.Add(new SortDescription() { Direction = ListSortDirection.Ascending, PropertyName = "Category" });
	}

	public ObservableCollection<CategorySum>? CategorySums { get; set; } = new ObservableCollection<CategorySum>();
	public CollectionViewSource CategorySumSource { get; set; } = new CollectionViewSource();

	[ObservableProperty]
	private bool _hasChanges;
	/// <summary>
	/// The HasChanges property tracks the changes of the three fields at the top of this page.
	/// They are date, note and target total dollar amount. The submit button enable state is 
	/// controlled by the HasChanges property.
	/// </summary>

	private DateOnly _batchDate;
	/// <summary>
	/// The BatchDate property is stored in the Batch object and is displayed in a column of
	/// the batch browser.
	/// </summary>
	public string BatchDate
	{
		get
		{
			string str = _batchDate.ToString("yyyy/MM/dd");
			return str.Equals("0001/01/01") ? "" : str;
		}
		set
		{
			try
			{
				if (string.IsNullOrEmpty(value))
					_batchDate = DateOnly.MinValue;
				else
					_batchDate = DateOnly.Parse(value);
			}
			catch
			{
				_batchDate = DateOnly.MinValue;
			}
			HasChanges = true;
			OnPropertyChanged();
		}
	}

	[ObservableProperty]
	private string? _batchNote;
	/// <summary>
	/// The BatchNote property is stored in the Batch object and is displayed in a column of
	/// the batch browser.
	/// </summary>
	partial void OnBatchNoteChanged(string? value)
	{
		HasChanges = true;
	}

	[ObservableProperty]
	private double _batchTotal;
	/// <summary>
	/// The BatchTotal property is stored in the Batch object and is displayed in a column of
	/// the batch browser. It is also compared against the actual sum total and if they don't 
	/// match, they will be colored with a red background in the batch browser view.
	/// </summary>
	partial void OnBatchTotalChanged(double value)
	{
		HasChanges = true;
	}

	private Batch? _origBatch;

	public DonorInputViewModel DonorInputViewModel { get; }
	public BatchPrintViewModel BatchPrintViewModel { get; }

	public CategoryReviewView CreateCategoryReviewView(CategorySum categorySum)
	{
#pragma warning disable CS8604 // Possible null reference argument.
		return _categoryReviewFactory(categorySum, enumCategoryReviewType.Batch, new ObservableCollection<Donation>(_batchDonations.Where(x => x.Category == categorySum.Category)), BatchDate);
#pragma warning restore CS8604 // Possible null reference argument.
	}

	/// <summary>
	/// This method is called if the BatchReviewView is shown as a result of a double-click
	/// on an entry in the batch browser. It sets up a local DonationList to be used by both tabs,
	/// filted by BatchId.
	/// </summary>
	/// <param name="batch">The batch object that was clicked on in the batch browser. It is actually
	/// a reference, so if changes are made to it, they will be reflected in the top level batch
	/// collection.</param>
	public async Task Review(Batch? batch, ObservableCollection<Donation>? batchDonations, Action closeDialog)
	{
		_closeDialog = closeDialog;

		if (null == batch)
		{
			throw new Exception("batch is null");
		}
		if (null == batchDonations)
		{
			throw new Exception("batchDonations is null");
		}

		_batchDonations = batchDonations;

		_origBatch = batch;
#pragma warning disable CS8601 // Possible null reference assignment.
		BatchDate = batch.Date;
#pragma warning restore CS8601 // Possible null reference assignment.
		BatchNote = batch.Note;
		BatchTotal = batch.Total;

		HasChanges = false;

		await ComputeSum();
	}

	/// <summary>
	/// This method is the handler for the SubmitBatchCommand. If the batch is submitted,
	/// it updated the three fields from the top of this page to the batch object. Since the _origBatch
	/// is a reference to the object double-clicked on in the batch browser, simply changing values
	/// here will change them in the Batch object in the list. In that case, the changes are recorded
	/// in the top level batch collection object.
	/// </summary>
	[RelayCommand]
	public async Task SubmitBatch()
	{
		_origBatch.Date = DateOnly.Parse(BatchDate).ToString("yyyy/MM/dd");
		_origBatch.Total = BatchTotal;
		_origBatch.Note = BatchNote;

		await _batchServices.UpdateBatch(_origBatch);

		HasChanges = false;

		if (null != _closeDialog)
			_dispatcherWrapper.BeginInvoke(_closeDialog);
	}

	/// <summary>
	/// This method function is called to compute the actual sum of all the donations in
	/// this batch. It should match with the target Total entered at the top of this view.
	/// </summary>
	public async Task ComputeSum()
	{
		CategorySums.Clear();
		_categorySumDict.Clear();

		foreach (var donation in _batchDonations)
		{
#pragma warning disable CS8604 // Possible null reference argument.
			if (_categorySumDict.ContainsKey(donation.Category))
			{
				_categorySumDict[donation.Category].Sum += donation.Value;
			}
			else
			{
				CategorySum sum = new CategorySum()
				{
					Category = donation.Category,
					Sum = donation.Value
				};
				_dispatcherWrapper.Invoke(() => CategorySums.Add(sum));
				_categorySumDict[sum.Category] = sum;
			}
#pragma warning restore CS8604 // Possible null reference argument.
			await _dispatcherWrapper.Yield();
		}

		CategorySumSource.View.Refresh();
	}
}
