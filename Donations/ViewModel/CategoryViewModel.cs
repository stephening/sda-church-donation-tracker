using CommunityToolkit.Mvvm.Input;
using Donations.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Donations.ViewModel
{
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
	public class CategoryViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObservableCollection<Category>? CategoryList { get; set; } = new ObservableCollection<Category>();
		public CollectionViewSource CollectionSource { get; set; } = new CollectionViewSource();

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
		/// The InsertRowAboveCmd property is bound to the 'Insert row above' context menu option.
		/// It allows a row to be inserted above the row that was right-clicked.
		/// </summary>
		public ICommand InsertRowAboveCmd { get; }
		/// <summary>
		/// The InsertRowBelowCmd property is bound to the 'Insert row below' context menu option.
		/// It allows a row to be inserted below the row that was right-clicked.
		/// </summary>
		public ICommand InsertRowBelowCmd { get; }
		/// <summary>
		/// The RevertCmd property is bound to the 'Revert' button. This button will revert any
		/// any changes which have been made in the CategoryView tab.
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
		/// The constructor makes a copy of the category list to display and to allow for
		/// changes to be reverted. It also sets the CollectionViewSource for the category list
		/// DataGrid. Finally, it initializes the DeleteRowCmd, InsertRowAboveCmd,
		/// InsertRowBelowCmd, RevertCmd, SaveChangesCmd, DeleteAllCmd's to the handlers for each.
		/// </summary>
		public CategoryViewModel()
		{
			if (null == CategoryList)
			{
				throw new InsufficientMemoryException("CategoryList is null");
			}
			
			HasChanges = false;

			foreach (var cat in di.Data.CatList)
			{
				CategoryList.Add(cat.Copy());
			}

			CollectionSource.Source = CategoryList;

			DeleteRowCmd = new RelayCommand(DeleteRow);
			InsertRowAboveCmd = new RelayCommand(InsertRowAbove);
			InsertRowBelowCmd = new RelayCommand(InsertRowBelow);
			RevertCmd = new RelayCommand(Revert);
			SaveChangesCmd = new RelayCommand(SaveChanges);
			DeleteAllCmd = new RelayCommand(DeleteAll);
		}

		/// <summary>
		/// This method is called when the tab is clicked on. This member allows for things
		/// to be refreshed incase new categories were imported since this page was last viewed.
		/// </summary>
		public void Loaded()
		{
			Revert();
			CollectionSource.View.Refresh();
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
				CategoryList?.RemoveAt(SelectedRowIndex.Value);
			}
		}

		/// <summary>
		/// This method is called in response to the right-click context menu:'Insert row above'.
		/// This is the method bound to the Command, and it will insert a new row above the one
		/// that was right-clicked on to get the context menu.
		/// </summary>
		public void InsertRowAbove()
		{
			if (null != SelectedRowIndex)
			{
				CategoryList?.Insert(SelectedRowIndex.Value, new Category());
			}
		}

		/// <summary>
		/// This method is called in response to the right-click context menu:'Insert row below'.
		/// This is the method bound to the Command, and it will insert a new row below the one
		/// that was right-clicked on to get the context menu.
		/// </summary>
		public void InsertRowBelow()
		{
			if (null != SelectedRowIndex)
			{
				CategoryList?.Insert(SelectedRowIndex.Value + 1, new Category());
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
			CategoryList?.Clear();

			if (null != di.Data.CatList)
			{
				foreach (var cat in di.Data.CatList)
				{
					CategoryList?.Add(cat.Copy());
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
			di.Data.ReplaceCategoryData(CategoryList);

			HasChanges = false;

			di.Data.SaveCategories(CategoryList);
		}

		/// <summary>
		/// This method is called in reponse to a click on the 'Delete all' button. This action
		/// will delete the local copy of the list. A 'Save changes' will be needed to clear the
		/// top level changes.
		/// </summary>
		public void DeleteAll()
		{
			HasChanges = true;
			CategoryList.Clear();
		}
	}
}
