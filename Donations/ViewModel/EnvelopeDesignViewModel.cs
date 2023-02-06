using CommunityToolkit.Mvvm.Input;
using Donations.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Donations.ViewModel
{
	/// <summary>
	/// This view model handles the functionality and the binding to the EnvelopeDesignView.xaml which
	/// is a UserControl occupying the 'Maintenance:Design tithe envelope' tab. This is an extremely
	/// simple one column DataGrid. I could have just used a ListBox, but because there are so many
	/// other DataGrids in this app, I figured why not.
	/// </summary>
	public class EnvelopeDesignViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public CollectionViewSource EnvelopeBody { get; set; } = new CollectionViewSource();

		/// <summary>
		/// The SelectedRowIndex property is bound to the SelectedIndex of the DataGrid. This is
		/// used to know which row was right-clicked on to bring up the context menu.
		/// </summary>
		public int? SelectedRowIndex { get; set; }

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
		/// The constructor sets the CollectionViewSource for the envelope design. And it
		/// initializes the DeleteRowCmd, InsertRowAboveCmd, and InsertRowBelowCmd, to their
		/// handlers.
		/// </summary>
		public EnvelopeDesignViewModel()
		{
			// we want at least 20 rows in this view, so if the current length is less than 20,
			// add rows until there are 20. The designed can have more than 20 rows if they want
			// and they can have less than 20 as well. Just leave remaining rows empty. Empty
			// rows in the middle of the list will act as a spacer in the actual input view, but
			// can also be filled with other categories on the fly if desired.
			if (20 > di.Data.TitheEnvelopeDesign.Count)
			{
				for (int i = di.Data.TitheEnvelopeDesign.Count; i < 20; i++)
				{
					di.Data.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "" });
				}
			}

			EnvelopeBody.Source = di.Data.TitheEnvelopeDesign;

			DeleteRowCmd = new RelayCommand(DeleteRow);
			InsertRowAboveCmd = new RelayCommand(InsertRowAbove);
			InsertRowBelowCmd = new RelayCommand(InsertRowBelow);
		}

		/// <summary>
		/// This method is called to set a category on the current row. This view does not allow
		/// typing category text, but clicking on the row to select predefined categories. When a
		/// row is clicked on, the category selection window will come up, and the category
		/// selected will be passed, along with the row object.
		/// </summary>
		/// <param name="entry">The row object which is to be set to the selected category.</param>
		/// <param name="cat">The category object which will be used to form the row.</param>
		public void SetCategory(EnvelopeEntry? entry, Category? cat)
		{
			if (null != cat)
				entry.Category = $"{cat.Code} {cat.Description}";
			else
				entry.Category = "";

			EnvelopeBody.View.Refresh();
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
				di.Data.TitheEnvelopeDesign.RemoveAt(SelectedRowIndex.Value);
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
				di.Data.TitheEnvelopeDesign.Insert(SelectedRowIndex.Value, new EnvelopeEntry() { Category = "" });
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
				di.Data.TitheEnvelopeDesign.Insert(SelectedRowIndex.Value + 1, new EnvelopeEntry() { Category = "" });
				EnvelopeBody.View.Refresh();
			}
		}
	}
}
