using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the CategoryMapView.xaml which is a
/// UserControl, that is shown in the 'Maintenance:CategoryMapView' tab.
/// 
/// This control will allow the operator to see the mappings that have been created between Adventist
/// Giving (AG) categories, and the local categories. Individual mappings can be deleted and the
/// targets changed. In the view, the fields that differ are highlighted.
/// 
/// There are three buttons, 'Revert changes', 'Save changes', 'Delete all', and there is a context
/// menu with a 'Delete row' option. The target mapping can be changed by selecting an alternative
/// but no text can be edited.
/// </summary>
public partial class CategoryMapViewModel : BaseViewModel
{
	public ObservableCollection<AGCategoryMapItem>? CategoryMapList { get; set; } = new ObservableCollection<AGCategoryMapItem>();
	public CollectionViewSource CategoryMapViewSource { get; set; } = new CollectionViewSource();

	private readonly IReflectionHelpers _reflectionHelper;
	private readonly ICategoryServices _categoryServices;
	private readonly ICategoryMapServices _categoryMapServices;

	[ObservableProperty]
	private bool _hasChanges;
	/// <summary>
	/// The HasChanges property tracks the changes which allows the 'Save changes' button to
	/// be enabled or disabled accordingly.
	/// </summary>

	/// <summary>
	/// The constructor makes a copy of the category mappings to display and to allow for
	/// changes to be reverted. It also sets the CollectionViewSource for the category map
	/// DataGrid. Finally, it initializes the DeleteRowCmd, RevertCmd, SaveChangesCmd,
	/// DeleteAllCmd's to the handlers for each.
	/// </summary>
	public CategoryMapViewModel(
		IReflectionHelpers reflectionHelpers,
		ICategoryServices categoryServices,
		ICategoryMapServices categoryMapServices
	)
	{
		_reflectionHelper = reflectionHelpers;
		_categoryServices = categoryServices;
		_categoryMapServices = categoryMapServices;

		HasChanges = false;

		// make copy of category list so we can revert if we want
		foreach (var cat in _categoryMapServices!.AGCategoryMapList!)
		{
			CategoryMapList.Add(_reflectionHelper.CopyModel<AGCategoryMapItem>(cat));
		}

		CategoryMapViewSource.Source = CategoryMapList;
	}

	/// <summary>
	/// This method is called when the tab is clicked on. This member allows
	/// for things to be refreshed incase new mappings were added since this page was
	/// last viewed.
	/// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public new async Task Loading()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		Revert();
		CategoryMapViewSource.View.Refresh();
	}

	/// <summary>
	/// This method is called when another tab clicked on. This member allows for a reminder
	/// to save changes.
	/// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public new async Task Leaving()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		if (HasChanges)
		{
			var res = MessageBox.Show("If you leave the tab without saving changes, you will loose them. Do you want to save your changes?", "Category map", MessageBoxButton.YesNo);
			if (MessageBoxResult.Yes == res)
			{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				SaveChanges();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			}
		}
	}

	/// <summary>
	/// This method is called from CategoryMapView.xaml.cs when the operator clicks on
	/// a DataGrid row to change a target category. It passes in the AGCategoryMapItem
	/// item associated with the row clicked, and the new Category selected.
	/// </summary>
	/// <param name="item"></param>
	/// <param name="cat"></param>
	public void SetCategory(AGCategoryMapItem? item, Category? cat)
	{
		if (null == item || null == cat)
			return;

		item.CategoryCode = cat.Code;
		item.CategoryDescription = _categoryServices.GetCategoryDescription(cat.Code);
		CategoryMapViewSource.View.Refresh();
		HasChanges = true;
	}

	/// <summary>
	/// This method is called in response to the right-click context menu:'Delete row'.
	/// This is the method bound to the Command, and it will delete the row that was 
	/// right-clicked on to get the context menu.
	/// </summary>
	[RelayCommand]
	public void DeleteRow()
	{
		AGCategoryMapItem? item = CategoryMapViewSource!.View?.CurrentItem as AGCategoryMapItem;
		if (null == item) return;

		HasChanges = true;
		CategoryMapList?.Remove(item);
	}

	/// <summary>
	/// This method is called in reponse to a click on the 'Revert changes' button. Since the 
	/// list being viewed and potentially edited is a copy, we can simply re-copy it
	/// from the top level source.
	/// </summary>
	[RelayCommand]
	public void Revert()
	{
		HasChanges = false;

		CategoryMapList?.Clear();
		if (null != _categoryMapServices.AGCategoryMapList)
		{
			foreach (var cat in _categoryMapServices.AGCategoryMapList)
			{
				CategoryMapList?.Add(_reflectionHelper.CopyModel<AGCategoryMapItem>(cat));
			}
		}
	}

	/// <summary>
	/// This method is called in response to a click on the 'Save changes' button. Since the
	/// list is a copy, the top level map list is cleared, and the new one copied to replace
	/// the old.
	/// </summary>
	[RelayCommand]
	public async Task SaveChanges()
	{
		_categoryMapServices.AGCategoryMap?.Clear();
		_categoryMapServices.AGCategoryMapList?.Clear();

		if (null != CategoryMapList)
		{
			foreach (var entry in CategoryMapList)
			{
				_categoryMapServices.AGCategoryMap[entry.AGCategoryCode] = _reflectionHelper.CopyModel<AGCategoryMapItem>(entry);
				_categoryMapServices.AGCategoryMapList?.Add(_categoryMapServices.AGCategoryMap[entry.AGCategoryCode]);
			}
		}

		HasChanges = false;

#pragma warning disable CS8604 // Possible null reference argument.
		await _categoryMapServices.SaveCategoryMap(CategoryMapList, true);
#pragma warning restore CS8604 // Possible null reference argument.
	}

	/// <summary>
	/// This method is called in reponse to a click on the 'Delete all' button. This action
	/// will delete the local copy of the list. A 'Save changes' will be needed to clear the
	/// top level changes.
	/// </summary>
	[RelayCommand]
	public void DeleteAll()
	{
		HasChanges = true;
		CategoryMapList?.Clear();
	}
}
