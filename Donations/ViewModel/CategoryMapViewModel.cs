using CommunityToolkit.Mvvm.Input;
using Donations.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Donations.ViewModel
{
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
	public class CategoryMapViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObservableCollection<AGCategoryMapItem>? CategoryMapList { get; set; } = new ObservableCollection<AGCategoryMapItem>();
		public CollectionViewSource CategoryMapViewSource { get; set; } = new CollectionViewSource();

		/// <summary>
		/// The SelectedRowIndex property is bound to the SelectedIndex of the DataGrid. This is
		/// used to know which row was right-clicked on to bring up the context menu.
		/// </summary>
		public int? SelectedRowIndex { get; set; }

		private bool _hasChanges;
		/// <summary>
		/// The HasChanges property tracks the changes which allows the 'Save changes' button to
		/// be enabled or disabled accordingly.
		/// </summary>
		public bool HasChanges
		{
			get { return _hasChanges; }
			set
			{
				_hasChanges = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The DeleteRowCmd property is bound to the 'Delete row' context menu option. It allows
		/// for the row that was right-clicked on to be deleted.
		/// </summary>
		public ICommand DeleteRowCmd { get; }
		/// <summary>
		/// The RevertCmd property is bound to the 'Revert' button. This button will revert any
		/// any changes which have been made in the CategoryMapView tab.
		/// </summary>
		public ICommand RevertCmd { get; }
		/// <summary>
		/// The SaveChangesCmd property is bound to the 'Save changes' button. This button will
		/// save the changes that have been made.
		/// </summary>
		public ICommand SaveChangesCmd { get; }
		/// <summary>
		/// The DeleteAllCmd property is bound to the 'Delete all' button. This button will
		/// delete all mappings. The effect of this is that on the next Adventist Giving import
		/// all categories that don't match exactly will have to be mapped again.
		/// </summary>
		public ICommand DeleteAllCmd { get; }

		/// <summary>
		/// The constructor makes a copy of the category mappings to display and to allow for
		/// changes to be reverted. It also sets the CollectionViewSource for the category map
		/// DataGrid. Finally, it initializes the DeleteRowCmd, RevertCmd, SaveChangesCmd,
		/// DeleteAllCmd's to the handlers for each.
		/// </summary>
		public CategoryMapViewModel()
		{
			HasChanges = false;

			// make copy of category list so we can revert if we want
			foreach (var cat in di.Data.AGCategoryMapList)
			{
				CategoryMapList.Add(cat.Copy());
			}

			CategoryMapViewSource.Source = CategoryMapList;

			DeleteRowCmd = new RelayCommand(DeleteRow);
			RevertCmd = new RelayCommand(Revert);
			SaveChangesCmd = new RelayCommand(SaveChanges);
			DeleteAllCmd = new RelayCommand(DeleteAll);
		}

		/// <summary>
		/// This method is called when the tab is clicked on. This member allows
		/// for things to be refreshed incase new mappings were added since this page was
		/// last viewed.
		/// </summary>
		public void Loaded()
		{
			Revert();
			CategoryMapViewSource.View.Refresh();
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
			CategoryMapViewSource.View.Refresh();
			HasChanges = true;
		}

		/// <summary>
		/// This method is called in response to the right-click context menu:'Delete row'.
		/// This is the method bound to the Command, and it will delete the row that was 
		/// right-clicked on to get the context menu.
		/// </summary>
		public void DeleteRow()
		{
			if (null != SelectedRowIndex)
			{
				HasChanges = true;
				CategoryMapList?.RemoveAt(SelectedRowIndex.Value);
			}
		}

		/// <summary>
		/// This method is called in reponse to a click on the 'Revert changes' button. Since the 
		/// list being viewed and potentially edited is a copy, we can simply re-copy it
		/// from the top level source.
		/// </summary>
		public void Revert()
		{
			HasChanges = false;

			CategoryMapList?.Clear();
			if (null != di.Data.AGCategoryMapList)
			{
				foreach (var cat in di.Data.AGCategoryMapList)
				{
					CategoryMapList?.Add(cat.Copy());
				}
			}
		}

		/// <summary>
		/// This method is called in response to a click on the 'Save changes' button. Since the
		/// list is a copy, the top level map list is cleared, and the new one copied to replace
		/// the old.
		/// </summary>
		public void SaveChanges()
		{
			di.Data.AGCategoryMap?.Clear();
			di.Data.AGCategoryMapList?.Clear();

			if (null != CategoryMapList)
			{
				foreach (var entry in CategoryMapList)
				{
					di.Data.AGCategoryMap[entry.AGCategoryCode] = entry.Copy();
					di.Data.AGCategoryMapList?.Add(di.Data.AGCategoryMap[entry.AGCategoryCode]);
				}
			}

			HasChanges = false;
		}

		/// <summary>
		/// This method is called in reponse to a click on the 'Delete all' button. This action
		/// will delete the local copy of the list. A 'Save changes' will be needed to clear the
		/// top level changes.
		/// </summary>
		public void DeleteAll()
		{
			HasChanges = true;
			CategoryMapList?.Clear();
		}
	}
}
