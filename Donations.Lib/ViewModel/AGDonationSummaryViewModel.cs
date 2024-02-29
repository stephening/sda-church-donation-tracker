using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the AGDonationSummaryView.xaml which is a
/// UserControl under the 'Verify and submit' tab.
/// 
/// Once the donor and category matching is done, the this view will show a list of categories with their
/// subtotals. This view should be able to cross checked with the category sub totals from the Adventist
/// Giving (AG) report.
/// </summary>
public partial class AGDonationSummaryViewModel : BaseViewModel
{
	public ObservableCollection<AdventistGiving>? TransactionList => _adventistGivingViewModel?.TransactionList;

	public ObservableCollection<CategorySum>? CategorySums { get; set; } = new ObservableCollection<CategorySum>();
	public CollectionViewSource CategorySumSource { get; set; } = new CollectionViewSource();

	[ObservableProperty]
	private double _total;
	/// <summary>
	/// The Total property is bound to the light green background TextBox, showing the
	/// actual (computed) total. This property will be saved in a column of the batch.
	/// </summary>

	private DateOnly _batchDate;
	/// <summary>
	/// The BatchDate is bound to the batch DatePicker to assign a single batch date
	/// which will show in the batch browser view. This is a single date for the entire
	/// batch as opposed to the dates for each individual transaction.
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
			OnPropertyChanged();
			OnPropertyChanged(nameof(SubmitEnabled));
		}
	}

	/// <summary>
	/// The SubmitEnabled property will control whether the button 'Submit batch' button
	/// enabled or disabled. The state is based on the date being set and a non-zero sum.
	/// </summary>
	public bool SubmitEnabled => (!string.IsNullOrEmpty(BatchDate) && 0 < CategorySums?.Count && !_submitting);

	private Dictionary<string, CategorySum> _categorySumDict = new Dictionary<string, CategorySum>();
	private ObservableCollection<Donation>? _donationList = null;
	private bool _submitting = false;
	private readonly IBatchServices _batchServices;
	private readonly IDonationServices _donationServices;
	private readonly ICategoryMapServices _categoryMapServices;
	private AdventistGivingViewModel? _adventistGivingViewModel;
	private readonly AGDonorResolutionViewModel _aGDonorResolutionViewModel;
	private readonly AGCategoryResolutionViewModel _aGCategoryResolutionViewModel;
	private readonly IDonorMapServices _donorMapServices;
	private readonly ICategoryServices _categoryServices;

	/// <summary>
	/// The constructor sets the CollectionViewSource for the category sums DataGrid. And it
	/// makes sure the BatchDate is clear so it will have to be specified.
	/// </summary>
	public AGDonationSummaryViewModel(
		AGDonorResolutionViewModel aGDonorResolutionViewModel,
		AGCategoryResolutionViewModel aGCategoryResolutionViewModel,
		IBatchServices batchServices,
		IDonationServices donationServices,
		ICategoryMapServices categoryMapServices,
		IDonorMapServices donorMapServices,
		ICategoryServices categoryServices
		)
	{
		_batchServices = batchServices;
		_donationServices = donationServices;
		_categoryMapServices = categoryMapServices;
		_aGDonorResolutionViewModel = aGDonorResolutionViewModel;
		_aGCategoryResolutionViewModel = aGCategoryResolutionViewModel;
		_donorMapServices = donorMapServices;
		_categoryServices = categoryServices;

		CategorySumSource.Source = CategorySums;

		BatchDate = "";
	}

	public void SetParentViewModel(
		AdventistGivingViewModel adventistGivingViewModel)
	{
		_adventistGivingViewModel = adventistGivingViewModel;
	}

	/// <summary>
	/// This method is used to return the local category code which matched already,
	/// or comes from the category mapping performed in an earlier step. If both of
	/// those options failed (which they shouldn't), an exception is thrown.
	/// </summary>
	/// <param name="agCatId"></param>
	/// <returns></returns>
	/// <exception cref="Exception"></exception>
	private int MapCategory(int agCatId)
	{
		if (true == _categoryServices.CatDict?.ContainsKey(agCatId))
		{
			return agCatId;
		}
		else if (null != _categoryMapServices.AGCategoryMap && _categoryMapServices.AGCategoryMap.ContainsKey(agCatId))
		{
			return _categoryMapServices.AGCategoryMap[agCatId].CategoryCode;
		}
		else
		{
			throw new Exception("Unable to resolve Adventist Giving category Id");
		}
	}

	/// <summary>
	/// This method is mapped to the SubmitBatchCommand, and is where the new
	/// Donation records and the Batch record will be created and added to the top
	/// level lists. Switching to the 'Batch browser' tab should immediately show
	/// the new batch, assuming the batch date is covered by the date filters.
	/// </summary>
	[RelayCommand]
	private async Task SubmitBatch()
	{
		_submitting = true;
		OnPropertyChanged(nameof(SubmitEnabled));

		var batch = new Batch()
		{
			Source = enumSource.AdventistGiving,
			Date = DateOnly.Parse(BatchDate).ToString("yyyy/MM/dd"),
			Total = (null != _adventistGivingViewModel) ? _adventistGivingViewModel.TargetTotal : 0,
			ActualTotal = Total,
			Operator = WindowsIdentity.GetCurrent().Name,
			Note = _adventistGivingViewModel?.BatchNote,
		};

		batch.Id = await _batchServices.AddBatch(batch);

		foreach (var donation in _donationList)
			donation.BatchId = batch.Id;

		await _donationServices.AddDonations(_donationList);

		_donationList = null;
		Total = 0;
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
		BatchDate = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
		CategorySums?.Clear();
		_categorySumDict.Clear();
		TransactionList?.Clear();
		_adventistGivingViewModel?.Reset(); // this sets the visual state of the donor/category resolution tabs
		OnPropertyChanged(nameof(SubmitEnabled));

#pragma warning disable CS8604 // Possible null reference argument.
		await _categoryMapServices.SaveCategoryMap(_categoryMapServices.AGCategoryMapList, true);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
		await _donorMapServices.SaveDonorMap(_donorMapServices.AGDonorMapList, true);
#pragma warning restore CS8604 // Possible null reference argument.

		_submitting = false;
	}

	/// <summary>
	/// This method is called from the Loaded() method if a new AG csv is loaded.
	/// </summary>
	private void ComputeSum()
	{
		Total = 0;
		CategorySums.Clear();
		_categorySumDict.Clear();

		foreach (var donation in _donationList)
		{
			Total += donation.Value;

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
				CategorySums.Add(sum);
				_categorySumDict[sum.Category] = sum;
			}
#pragma warning restore CS8604 // Possible null reference argument.
		}

		CategorySumSource.View.Refresh();
		OnPropertyChanged(nameof(SubmitEnabled));
	}

	/// <summary>
	/// This method is called if a new AG csv is loaded and the sums need to be updated.
	/// </summary>
	/// <returns></returns>
	public async Task Loaded()
	{
		if (_aGDonorResolutionViewModel.DonorResolutionComplete == Visibility.Visible
			&& _aGCategoryResolutionViewModel.CategoryResolutionComplete == Visibility.Visible)
		{
			_submitting = false;
			_donationList = new ObservableCollection<Donation>();
			Total = 0;
			var donationId = await _donationServices.GetNextId();

			foreach (var tx in TransactionList)
			{
				int txCatCode = MapCategory(tx.CategoryCode);
				Donation donation = new Donation();
				donation.Id = donationId;
#pragma warning disable CS8604 // Possible null reference argument.
				if (null != tx.DonorId)
					donation.DonorId = tx.DonorId.Value;
				else if (_donorMapServices.AGDonorMap.ContainsKey(tx.DonorHash))
					donation.DonorId = _donorMapServices.AGDonorMap[tx.DonorHash].DonorId;
				else
					throw new Exception($"This shouldn't happen, but there was no donor id or map for this transaction {tx.FirstName} {tx.LastName} TransactionId: {tx.TransactionId} Category: {tx.CategoryName}, Amount: {tx.Amount}");
#pragma warning restore CS8604 // Possible null reference argument.

				string lastName = tx.LastName;
				string firstName = tx.FirstName;
				Donor donor;
				if (null != _adventistGivingViewModel && _adventistGivingViewModel._quickDonorLookup.ContainsKey(donation.DonorId))
				{
					donor = _adventistGivingViewModel._quickDonorLookup[donation.DonorId];
				}
				else
				{
					donor = null;
				}

				lastName = donor?.LastName;
				firstName = donor?.FirstName;
				donation.LastName = lastName;
				donation.FirstName = firstName;
				donation.Category = $"{txCatCode} {_categoryServices.CatDict[txCatCode].Description}";
				donation.Value = tx.Amount;
#pragma warning disable CS8604 // Possible null reference argument.
				donation.Date = DateOnly.Parse(tx.TransactionDate).ToString("yyyy/MM/dd");
#pragma warning restore CS8604 // Possible null reference argument.
				donation.Method = enumMethod.AdventistGiving;
				donation.TransactionNumber = tx.TransactionId;
				donation.TaxDeductible = _categoryServices.CatDict[txCatCode].TaxDeductible;

				_donationList.Add(donation);
			}
			ComputeSum();
		}
	}
}
