using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the AGCategoryResolutionView.xaml which is a
/// UserControl under the 'Category resolution' tab.
/// 
/// This class will atomatically try to match Adventist Giving (AG) categories, to the local category database.
/// If an exact match is not found, it will present an option to the operator. But ultimately the operator will
/// have to decide whether to accept the propsed category, or select another one. Once the operator accepts or
/// chooses a different category, a mapping is created which will be used from then on. That category will be
/// automatically mapped from then on. If no close match is found, the user will be required to select a 
/// category themselves, and if one doesn't exist, they can go to the 'Maintenance:Category' tab and add a new
/// one. Once the category resolution is complete for the who batch, move on to verification and submit.
/// </summary>
public partial class AGCategoryResolutionViewModel : BaseViewModel
{
	public ObservableCollection<AdventistGiving>? TransactionList => _adventistGivingViewModel?.TransactionList;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ContinueEnabled))]
	[NotifyPropertyChangedFor(nameof(ImportCategoryVisible))]
	private Category? _cat;

	[ObservableProperty]
	private AdventistGiving? _transaction;
	/// <summary>
	/// The Transaction property will contain the current AG record that is needing category resolution.
	/// </summary>

	[ObservableProperty]
	private string? _progressText;
	/// <summary>
	/// The ProgressText property is displayed in the user control to show the current record number that is needing resolution.
	/// </summary>

	/// <summary>
	/// The ContinueEnabled property is a boolean that will facilitate enabling or disabling the 'Continue...' button depending
	/// on whether a mapping target is available.
	/// </summary>
	public bool ContinueEnabled => (null != Cat) ? true : false;

	[ObservableProperty]
	private Visibility _categoryDiffsVisibility = Visibility.Visible;
	/// <summary>
	/// The CategoryDiffsVisibility property controls the visibility of most of this control. While category resolution is in
	/// progress, it will be visible. Once the category resolution is complete, this propery will be set to Hidden.
	/// </summary>

	/// <summary>
	/// The ImportCategoryVisible property makes the button to import category visible in case the operator wishes to add the category
	/// to the local database.
	/// </summary>
	public Visibility ImportCategoryVisible => null == Cat ? Visibility.Visible : Cat.Code != Transaction!.CategoryCode ? Visibility.Visible : Visibility.Hidden;

	[ObservableProperty]
	private Visibility _categoryResolutionComplete = Visibility.Hidden;
	/// <summary>
	/// The CategoryResolutionComplete property is simply the opposite of the CategoryDiffsVisibility property. When the latter is
	/// Hidden, this one will be visible and vice versa.
	/// </summary>

	private int? _txIdx = null;
	private bool _categoriesAdded = false;
	private AdventistGivingViewModel? _adventistGivingViewModel;
	private readonly ICategoryServices _categoryServices;
	private readonly ICategoryMapServices _categoryMapServices;

	/// <summary>
	/// The constructor places its this pointer in the Global static object for use by other ViewModels
	/// that may need access to its public members. It also connects the method to the Command binding.
	/// </summary>
	public AGCategoryResolutionViewModel(
		ICategoryServices categoryServices,
		ICategoryMapServices categoryMapServices
	)
	{
		_categoryServices = categoryServices;
		_categoryMapServices = categoryMapServices;
	}

	public void SetParentViewModel(
		AdventistGivingViewModel adventistGivingViewModel)
	{
		_adventistGivingViewModel = adventistGivingViewModel;
	}

	/// <summary>
	/// This method simply sets the opposing Visible/Hidden properties based on a bool.
	/// </summary>
	/// <param name="flag"></param>
	public void ResolutionComplete(bool flag)
	{
		if (flag)
		{
			CategoryResolutionComplete = Visibility.Visible;
			CategoryDiffsVisibility = Visibility.Hidden;
		}
		else
		{
			CategoryResolutionComplete = Visibility.Hidden;
			CategoryDiffsVisibility = Visibility.Visible;
		}
	}

	/// <summary>
	/// This function is called from the View class if/after a category is chosen as a match for the
	/// current Adventist Giving record. If a matching category is not found, the operator would first
	/// have to go into category maintenance and the needed category, after which they will be able to
	/// select the target category.
	/// </summary>
	/// <param name="category"></param>
	public void ChooseCategory(Category? category)
	{
		Cat = category;
	}

	/// <summary>
	/// This method is called from the Adventist Giving (AG) tab to begin the Category resolution from
	/// the AG categories to the local ones. This is an async function so the UI thread is not blocked
	/// when it is running.
	/// </summary>
	/// <returns></returns>
	public async Task StartCategoryResolution()
	{
		_categoriesAdded = false;
		CategoryResolutionComplete = Visibility.Hidden;
		CategoryDiffsVisibility = Visibility.Visible;

		await Task.Run(() =>
		{
			if (null != TransactionList)
			{
				CategoryResolutionLoop(0);
			}
		});

		if (CategoryResolutionComplete == Visibility.Visible && _categoriesAdded)
		{
			await _categoryServices.SaveCategories(_categoryServices.CatList, true);
		}
	}

	/// <summary>
	/// This private function loops through the records in the Adventist Giving (AG) *.csv, searching
	/// for a category match in the the local database. If an exact match is not found, the function
	/// exits, leaving the AGCategoryResolutionView UserControl displaying the proposed category match
	/// or none if a close match could not be found. If no match was found the operator will need to
	/// click on the 'Browse for Category' button to select the target category. To continue on,
	/// the operator will click the 'Continue...' button to create the mapping and proceed to the
	/// next AG category.
	/// </summary>
	/// <param name="start">The parameter is the index corresponding to the AG record to start
	/// resolving from. To start, the value will be zero. After a resolution, upon clicking of the
	/// 'Continue...' button, the loop will resume with the next AG record index number. The current
	/// index is always contained in the _txId member variable.</param>
	private void CategoryResolutionLoop(int start)
	{
		for (_txIdx = start; _txIdx < TransactionList?.Count; _txIdx++)
		{
			ProgressText = $"Record {_txIdx.Value + 1} of {TransactionList.Count}";

			var categoryId = TransactionList[_txIdx.Value].CategoryCode;
			var tx = TransactionList[_txIdx.Value];

			// Next, check to see if a mapping already exists for this AG category code?
			if (true == _categoryMapServices.AGCategoryMap?.ContainsKey(categoryId))
			{
				// exact match through map
				Transaction = tx;
				Cat = _categoryServices.CatDict[_categoryMapServices.AGCategoryMap[categoryId].CategoryCode];
				continue;
			}

			// First check for an exact match between the AG category code and the local category list.
			if (_categoryServices.CatDict.ContainsKey(categoryId))
			{
				// code exact match
				// check description
				var txName = string.IsNullOrEmpty(tx.CategoryName) ? "" : tx.SplitCategoryName;
				var catDesc = _categoryServices.CatDict[categoryId].Description;

				if (txName.Equals(catDesc, StringComparison.OrdinalIgnoreCase))
				{
					// exact match
					Transaction = tx;
					Cat = _categoryServices.CatDict[categoryId];
					continue;
				}

				// Update the view to show the current record bein resolved. This string is updated
				// on each iteration through the loop, but it will really only be seen once the loop
				// bails for a category resolution.
				ProgressText = $"Record {_txIdx.Value + 1} of {TransactionList.Count}";

				Transaction = tx;
				Cat = _categoryServices.CatDict[categoryId];
				return;
			}

			// Update the view to show the current record bein resolved. This string is updated
			// on each iteration through the loop, but it will really only be seen once the loop
			// bails for a category resolution.
			ProgressText = $"Record {_txIdx.Value + 1} of {TransactionList.Count}";

			// If there is not eact match, the following loop will go through the local category
			// table, comparing the Descriptions, looking for a similar match. If one is found,
			// that potential match will be displayed for the operator's approval, or manual
			// selection of another.
			foreach (var item in _categoryServices.CatDict)
			{
				// The split SplitCategoryName property is used to split names that are provided
				// in multiple languages, separated by '/'. In this case, just using the English
				// for descrviption comparison
				var txName = string.IsNullOrEmpty(tx.CategoryName) ? "" : tx.SplitCategoryName;
				var catDesc = string.IsNullOrEmpty(item.Value.Description) ? "" : item.Value.Description;

				// rather than looking for an exact match, look for one side in the other or
				// vice versa
				if (txName.Contains(catDesc, StringComparison.OrdinalIgnoreCase)
					|| catDesc.Contains(txName, StringComparison.OrdinalIgnoreCase))
				{
					// possible match by description
					Transaction = tx;
					Cat = item.Value;
					return;
				}
			}

			// no match found, allow user to manually select category
			Cat = null;
			Transaction = tx;
			return;
		}

		// completed category resolution, clear container objects
		Cat = null;
		Transaction = null;

		// then hide comparison UI and show resolution complete message
		CategoryDiffsVisibility = Visibility.Hidden;
		CategoryResolutionComplete = Visibility.Visible;
	}

	/// <summary>
	/// This function is called when the user presses the Import Category button.
	/// </summary>
	/// <returns></returns>
	public async Task AddCategory(Category category)
	{
		_categoriesAdded = true;
		_categoryServices.CatList.Add(category);
		_categoryServices.CatDict[category.Code] = category;

		await ContinueCategoryResolution();
	}

	/// <summary>
	/// This function is called when the user presses a button to continue resolving categories from the
	/// Adventist Giving *.csv.
	/// </summary>
	/// <returns></returns>
	[RelayCommand]
	public async Task ContinueCategoryResolution()
	{
		await Task.Run(() =>
		{
			// store selected category in mapping table
			if (null != Cat && null != Transaction && null != _categoryMapServices.AGCategoryMap && true != _categoryMapServices.AGCategoryMap?.ContainsKey(Transaction.CategoryCode))
			{
				// add to dictionary if not there already
				_categoryMapServices.AGCategoryMap[Transaction.CategoryCode] = new AGCategoryMapItem()
				{
					AGCategoryCode = Transaction.CategoryCode,
					AGCategoryName = Transaction.CategoryName,
					CategoryCode = Cat.Code,
					CategoryDescription = _categoryServices.GetCategoryDescription(Cat.Code)
				};
				_categoryMapServices.AGCategoryMapList?.Add(_categoryMapServices.AGCategoryMap[Transaction.CategoryCode]);
			}

			_txIdx++;

			if (null != TransactionList)
			{
				CategoryResolutionLoop(_txIdx.Value);
			}
		});

		if (CategoryResolutionComplete == Visibility.Visible && _categoriesAdded)
		{
			await _categoryServices.SaveCategories(_categoryServices.CatList, true);
		}
	}
}
