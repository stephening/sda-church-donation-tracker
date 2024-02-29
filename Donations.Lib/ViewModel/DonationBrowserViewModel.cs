using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.View;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Donations.Lib.ViewModel;

public partial class DonationBrowserViewModel : BaseTimeWindowViewModel
{
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private readonly CategoryReviewView.Factory _categoryReviewFactory;
	private readonly DonationPopupView.Factory _donationPopupViewFactory;
	private readonly IDonorServices _donorServices;
	private readonly IDonationServices _donationServices;
	private ObservableCollection<CategorySum> _categories = new ObservableCollection<CategorySum>();
	private Dictionary<string, CategorySum> _categorySumDict = new Dictionary<string, CategorySum>();
	private ObservableCollection<Donation>? _donations;
	private string? _date1;
	private string? _date2;
#pragma warning disable CS8603 // Possible null reference return.
	private string TimeWindow => (
		DateFilterOption == enumDateFilterOptions.SelectYear ? FilterYear
		: (DateFilterOption == enumDateFilterOptions.CurrentYear ? _thisYear
		: (DateFilterOption == enumDateFilterOptions.PreviousYear ? _prevYear
		: (DateFilterOption == enumDateFilterOptions.DateRange ? $"{_date1} - {_date2}" : "")))
		);
#pragma warning restore CS8603 // Possible null reference return.

	public DonationBrowserViewModel(
		IDispatcherWrapper dispatcherWrapper,
		CategoryReviewView.Factory categoryReviewFactory,
		DonationPopupView.Factory donationPopupViewFactory,
		IDonorServices donorServices,
		IDonationServices donationServices
		)
	{
		_dispatcherWrapper = dispatcherWrapper;
		_categoryReviewFactory = categoryReviewFactory;
		_donationPopupViewFactory = donationPopupViewFactory;
		_donorServices = donorServices;
		_donationServices = donationServices;

		CategorySource.Filter += new FilterEventHandler(CategoryFilter);
		DonationSource.Filter += new FilterEventHandler(DonationFilter);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		TimeWindowChanged();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
	}

	[ObservableProperty]
	private string? _categoryFilterText = "";

	[ObservableProperty]
	private string? _donationFilterText = "";

	[ObservableProperty]
	private bool _searchName = false;

	[ObservableProperty]
	private bool _searchCategory = false;

	[ObservableProperty]
	private bool _searchDate = false;

	[ObservableProperty]
	private bool _searchMethod = false;

	[ObservableProperty]
	private bool _searchAmount = false;

	[ObservableProperty]
	private bool _searchNote = false;

	[ObservableProperty]
	private CollectionViewSource _categorySource = new CollectionViewSource();

	[ObservableProperty]
	private CollectionViewSource _donationSource = new CollectionViewSource();

	/// <summary>
	/// This method is called to invoke the CategoryReviewView factory to create
	/// the view with additional parameters
	/// </summary>
	/// <param name="categorySum"></param>
	/// <returns></returns>
	public CategoryReviewView CreateCategoryReviewView(CategorySum categorySum)
	{
		var res = _donations?.Where(x => categorySum.Category == x.Category);
#pragma warning disable CS8604 // Possible null reference argument.
		return _categoryReviewFactory(categorySum, enumCategoryReviewType.Donation, new ObservableCollection<Donation>(res), TimeWindow);
#pragma warning restore CS8604 // Possible null reference argument.
	}


	/// <summary>
	/// This member is called when the DonorBrowser tab is clicked, it will update the view.
	/// </summary>
	public new async Task Loading()
	{
		await FilterSource();
	}

	/// <summary>
	/// This method will update the AvailableYears collection using a Linq filter. It also
	/// set's the start/end range dates to the earliest and latest years from the list of batches.
	/// </summary>
	public override async Task TimeWindowChanged()
	{
		var list = await _donationServices.GetDonationYears();
		AvailableYears = new ObservableCollection<string>(list.OrderByDescending(i => i));
		FilterYear = AvailableYears.Max();
		FilterStartDate = await _donationServices.GetEarliestDate();
		FilterEndDate = await _donationServices.GetLatestDate();

		await FilterSource();
	}

	/// <summary>
	/// Updated the CollectionViewSource's when the time window changes
	/// </summary>
	/// <returns></returns>
	private async Task FilterSource()
	{
		switch (DateFilterOption)
		{
			case enumDateFilterOptions.CurrentYear:
				_date1 = _thisYear;
				break;
			case enumDateFilterOptions.PreviousYear:
				_date1 = _prevYear;
				break;
			case enumDateFilterOptions.SelectYear:
				_date1 = FilterYear;
				break;
			case enumDateFilterOptions.DateRange:
				_date1 = FilterStartDate;
				_date2 = FilterEndDate;
				break;
		}

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
		_donations = await _donationServices.FilterDonationsByDate(DateFilterOption, _date1, _date2);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8604 // Possible null reference argument.
		DonationSource.Source = _donations;

		_categories.Clear();
		_categorySumDict.Clear();
		foreach (var donation in _donations)
		{
#pragma warning disable CS8604 // Possible null reference argument.
			if (_categorySumDict.ContainsKey(donation.Category))
			{
				_categorySumDict[donation.Category].Sum += donation.Value;
			}
			else
			{
				CategorySum Cat = new CategorySum()
				{
					Category = donation.Category,
					Sum = donation.Value
				};
				try
				{
					var split = donation.Category.Split(' ');
					Cat.Code = int.Parse(split[0]);
					Cat.Description = donation.Category.Substring(split[0].Length + 1);
				}
				catch
				{
					Cat.Code = 0;
					Cat.Description = Cat.Category;
				}
				_categories.Add(Cat);
				_categorySumDict[Cat.Category] = Cat;
			}
#pragma warning restore CS8604 // Possible null reference argument.
			await _dispatcherWrapper.Yield();
		}

		CategorySource.Source = _categories;
		CategorySource.SortDescriptions.Clear();
		CategorySource.SortDescriptions.Add(new SortDescription() { PropertyName = "Code", Direction = ListSortDirection.Ascending });

		SelectionEnabled = true;
	}

	/// <summary>
	/// The CategoryFilterText method is called from the DonationBrowserView.xaml.cs when the
	/// CategoryFilterText's TextChanged event is fired. This allows for realtime feedback on
	/// whether the filter text is any good or not.
	/// </summary>
	public void CategoryTextChanged()
	{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
		SelectedCategory = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
		CategorySource.View.Refresh();
	}

	/// <summary>
	/// The filter method uses the same CategoryFilterText, looking for matches in either the Code,
	/// or the Description columns.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void CategoryFilter(object sender, FilterEventArgs e)
	{
		var obj = e.Item as CategorySum;
		if (obj != null)
		{
			if (string.IsNullOrEmpty(CategoryFilterText) || obj.Category.Contains(CategoryFilterText, System.StringComparison.OrdinalIgnoreCase))
				e.Accepted = true;
			else
				e.Accepted = false;
		}
	}

	/// <summary>
	/// The DonationTextChanged method is called from the DonationBrowserView.xaml.cs when the
	/// DonationFilterText's TextChanged event is fired. This allows for realtime feedback on
	/// whether the filter text is any good or not.
	/// </summary>
	public void DonationTextChanged()
	{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
		SelectedCategory = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
		DonationSource.View.Refresh();
	}

	/// <summary>
	/// The filter method uses the DonationFilterText, looking for matches in the checked columns.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void DonationFilter(object sender, FilterEventArgs e)
	{
		var obj = e.Item as Donation;
		if (obj != null)
		{
			if (string.IsNullOrEmpty(DonationFilterText))
				e.Accepted = true;
			else
			{
				if (SearchName && true == obj.Name?.Contains(DonationFilterText, System.StringComparison.OrdinalIgnoreCase))
					e.Accepted = true;
				else if (SearchCategory && true == obj.Category?.Contains(DonationFilterText, System.StringComparison.OrdinalIgnoreCase))
					e.Accepted = true;
				else if (SearchDate && true == obj.Date?.Contains(DonationFilterText, System.StringComparison.OrdinalIgnoreCase))
					e.Accepted = true;
				else if (SearchMethod && true == obj.Method.ToString()?.Contains(DonationFilterText, System.StringComparison.OrdinalIgnoreCase))
					e.Accepted = true;
				else if (SearchAmount && true == obj.Value.ToString("C2")?.Contains(DonationFilterText, System.StringComparison.OrdinalIgnoreCase))
					e.Accepted = true;
				else if (SearchNote && true == obj.Note?.Contains(DonationFilterText, System.StringComparison.OrdinalIgnoreCase))
					e.Accepted = true;
				else
					e.Accepted = false;
			}
		}
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<DonationPopupView> CreateDonationPopupView(int donationId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8604 // Possible null reference argument.
		Donation donation = _donations.Where(x => x.Id == donationId).First();
#pragma warning restore CS8604 // Possible null reference argument.
		var sameTime = _donations.Where(x =>
			x.Method == enumMethod.AdventistGiving ? x.TransactionNumber == donation.TransactionNumber
			: x.Name == donation.Name && x.Date == donation.Date);
		ObservableCollection<Donation> donations = new ObservableCollection<Donation>(sameTime);
		return _donationPopupViewFactory(donations);
	}
}
