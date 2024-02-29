using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the CategoryView.xaml which is a
/// UserControl, that is shown in the 'Maintenance:Category' tab.
/// 
/// This control will allow the operator to see the current categories that are in use, as well as
/// to add/edit/delete them.
/// 
/// There are three buttons, 'Revert changes', 'Save changes', 'Delete all', and there is a context
/// menu with a 'Delete row', 'Inser row above', and 'Insert row below' options. The Codes and
/// Descriptions can be changed inline in the DataGrid view.
/// </summary>
public partial class CategoryViewModel : BaseViewModel
{
	public ObservableCollection<Category>? CategoryList { get; set; } = new ObservableCollection<Category>();
	public CollectionViewSource CollectionSource { get; set; } = new CollectionViewSource();

	private readonly IReflectionHelpers _reflectionHelpers;
	private readonly ICategoryServices _categoryServices;
	private readonly ICategoryMapServices _categoryMapServices;
	private readonly CategoryMapViewModel _categoryMapViewModel;

	[ObservableProperty]
	private bool _hasChanges;
	/// <summary>
	/// The HasChanges property tracks the changes which allows the 'Save changes' button to
	/// be enabled or disabled accordingly.
	/// </summary>


	/// <summary>
	/// The constructor makes a copy of the category list to display and to allow for
	/// changes to be reverted. It also sets the CollectionViewSource for the category list
	/// DataGrid. Finally, it initializes the DeleteRowCmd, InsertRowAboveCmd,
	/// InsertRowBelowCmd, RevertCmd, SaveChangesCmd, DeleteAllCmd's to the handlers for each.
	/// </summary>
	public CategoryViewModel(
		IReflectionHelpers reflectionHelpers,
		ICategoryServices categoryServices,
		ICategoryMapServices categoryMapServices,
		CategoryMapViewModel categoryMapViewModel
	)
	{
		_reflectionHelpers = reflectionHelpers;
		_categoryServices = categoryServices;
		_categoryMapServices = categoryMapServices;
		_categoryMapViewModel = categoryMapViewModel;
		if (null == CategoryList)
		{
			throw new InsufficientMemoryException("CategoryList is null");
		}

		HasChanges = false;

		foreach (var cat in _categoryServices.CatList)
		{
			CategoryList.Add(_reflectionHelpers.CopyModel<Category>(cat));
		}

		CollectionSource.Source = CategoryList;
	}

	/// <summary>
	/// This method is called when the tab is clicked on. This member allows for things
	/// to be refreshed incase new categories were imported since this page was last viewed.
	/// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public new async Task Loading()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		Revert();
		CollectionSource.View.Refresh();
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
			var res = MessageBox.Show("If you leave the tab without saving changes, you will loose them. Do you want to save your changes?", "Categories", MessageBoxButton.YesNo);
			if (MessageBoxResult.Yes == res)
			{
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				SaveChanges();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			}
		}
	}

	/// <summary>
	/// This method is called in response to the right-click context menu:'Delete row'.
	/// This is the method bound to the Command, and it will delete the row that was 
	/// right-clicked on to get the context menu.
	/// </summary>
	[RelayCommand]
	public void DeleteRow()
	{
		Category item = CollectionSource.View.CurrentItem as Category;
		if (null == item) return;

		HasChanges = true;
		CategoryList?.Remove(item);
	}

	/// <summary>
	/// This method is called in response to the right-click context menu:'Insert row above'.
	/// This is the method bound to the Command, and it will insert a new row above the one
	/// that was right-clicked on to get the context menu.
	/// </summary>
	[RelayCommand]
	public void InsertRowAbove()
	{
		Category item = CollectionSource.View.CurrentItem as Category;
		if (null == item) return;

		int index = CategoryList.IndexOf(item);
		if (0 <= index)
		{
			CategoryList?.Insert(index, new Category());
			HasChanges = true;
		}
	}

	/// <summary>
	/// This method is called in response to the right-click context menu:'Insert row below'.
	/// This is the method bound to the Command, and it will insert a new row below the one
	/// that was right-clicked on to get the context menu.
	/// </summary>
	[RelayCommand]
	public void InsertRowBelow()
	{
		Category item = CollectionSource.View.CurrentItem as Category;
		if (null == item) return;

		int index = CategoryList.IndexOf(item);
		if (0 < index)
		{
			CategoryList?.Insert(index + 1, new Category());
			HasChanges = true;
		}
	}

	/// <summary>
	/// This method is called in reponse to a click on the 'Revert changes' button. Since the 
	/// list being viewed and potentially edited is a copy, we can simply re-copy it
	/// from the top level source.
	/// </summary>
	[RelayCommand]
	public void Revert()
	{
		CategoryList?.Clear();

		if (null != _categoryServices.CatList)
		{
			foreach (var cat in _categoryServices.CatList)
			{
				CategoryList?.Add(_reflectionHelpers.CopyModel<Category>(cat));
			}
		}
		HasChanges = false;
	}

	/// <summary>
	/// This method is called in response to a click on the 'Save changes' button. Since the
	/// list is a copy, the top level map list is cleared, and the new one copied to replace
	/// the old.
	/// </summary>
	[RelayCommand]
	public async Task SaveChanges()
	{
#pragma warning disable CS8604 // Possible null reference argument.
		_categoryServices.ReplaceCategoryData(CategoryList);
#pragma warning restore CS8604 // Possible null reference argument.

		HasChanges = false;

		await _categoryServices.SaveCategories(CategoryList, true);

		foreach (var item in _categoryMapServices.AGCategoryMapList)
		{
			item.CategoryDescription = _categoryServices.GetCategoryDescription(item.CategoryCode);
		}

		// refresh category map in maintenance tab
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		_categoryMapViewModel.Loading();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
		CategoryList.Clear();
	}
}
