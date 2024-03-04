using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the EnvelopeDesignView.xaml which
/// is a UserControl occupying the 'Maintenance:Design tithe envelope' tab. This is an extremely
/// simple one column DataGrid. I could have just used a ListBox, but because there are so many
/// other DataGrids in this app, I figured why not.
/// </summary>
public partial class EnvelopeDesignViewModel : BaseViewModel
{
	public ObservableCollection<EnvelopeEntry> EnvelopeEntries { get; set; } = new ObservableCollection<EnvelopeEntry>();
	public CollectionViewSource EnvelopeBody { get; set; } = new CollectionViewSource();

	private readonly ITitheEnvelopeServices _titheEnvelopeServices;

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
	public EnvelopeDesignViewModel(
		ITitheEnvelopeServices titheEnvelopeServices
	)
	{
		_titheEnvelopeServices = titheEnvelopeServices;

		HasChanges = false;

		// make copy of list so we can revert if we want
		foreach (var envelope in _titheEnvelopeServices!.TitheEnvelopeDesign!)
		{
			EnvelopeEntries.Add(new EnvelopeEntry() { Code = envelope.Code });
		}

		// we want at least 20 rows in this view, so if the current length is less than 20,
		// add rows until there are 20. The designed can have more than 20 rows if they want
		// and they can have less than 20 as well. Just leave remaining rows empty. Empty
		// rows in the middle of the list will act as a spacer in the actual input view, but
		// can also be filled with other categories on the fly if desired.
		if (20 > EnvelopeEntries.Count)
		{
			for (int i = EnvelopeEntries.Count; i < 20; i++)
			{
				EnvelopeEntries.Add(new EnvelopeEntry() { Code = -1 });
			}
		}

		EnvelopeBody.Source = EnvelopeEntries;
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
			entry.Code = cat.Code;
		else
			entry.Code = -1;

		EnvelopeBody.View.Refresh();
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
		EnvelopeEntry item = EnvelopeBody.View.CurrentItem as EnvelopeEntry;
		if (null == item) return;

		HasChanges = true;
		EnvelopeEntries?.Remove(item);
	}

	/// <summary>
	/// This method is called in response to the right-click context menu:'Insert row above'.
	/// This is the method bound to the Command, and it will insert a new row above the one
	/// that was right-clicked on to get the context menu.
	/// </summary>
	[RelayCommand]
	public void InsertRowAbove()
	{
		EnvelopeEntry item = EnvelopeBody.View.CurrentItem as EnvelopeEntry;
		if (null == item) return;

		int index = EnvelopeEntries.IndexOf(item);
		if (0 < index)
		{
			EnvelopeEntries?.Insert(index, new EnvelopeEntry());
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
		EnvelopeEntry item = EnvelopeBody.View.CurrentItem as EnvelopeEntry;
		if (null == item) return;

		int index = EnvelopeEntries.IndexOf(item);
		if (0 < index)
		{
			EnvelopeEntries?.Insert(index + 1, new EnvelopeEntry());
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
		HasChanges = false;

		EnvelopeEntries?.Clear();
		foreach (var envelope in _titheEnvelopeServices.TitheEnvelopeDesign)
		{
			EnvelopeEntries.Add(new EnvelopeEntry() { Code = envelope.Code });
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
		_titheEnvelopeServices.TitheEnvelopeDesign?.Clear();

		if (null != EnvelopeEntries)
		{
			foreach (var entry in EnvelopeEntries)
			{
				_titheEnvelopeServices.TitheEnvelopeDesign?.Add(new EnvelopeEntry() { Code = entry.Code });
			}
		}

		HasChanges = false;

		await _titheEnvelopeServices.SaveTitheEnvelopeDesign(EnvelopeEntries, true);
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
		EnvelopeEntries?.Clear();
	}
}
