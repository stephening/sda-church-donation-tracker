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
/// This view model handles the functionality and the binding to the DonorMapView.xaml which is a
/// UserControl, that is shown in the 'Maintenance:DonorMapView' tab.
/// 
/// This control will allow the operator to see the mappings that have been created between Adventist
/// Giving (AG) donors, and the local donor database. Individual mappings can be deleted and the
/// targets changed. In the view, the fields that differ are highlighted.
/// 
/// There are three buttons, 'Revert changes', 'Save changes', 'Delete all', and there is a context
/// menu with a 'Delete row' option. The target mapping can be changed by selecting an alternative
/// but no text can be edited.
/// </summary>
public partial class DonorMapViewModel : BaseViewModel
{
	public ObservableCollection<AGDonorMapItem>? DonorMapList { get; set; } = new ObservableCollection<AGDonorMapItem>();
	public CollectionViewSource DonorMapViewSource { get; set; } = new CollectionViewSource();

	private readonly IReflectionHelpers _reflectionHelpers;
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private readonly IDonorMapServices _donorMapServices;

	[ObservableProperty]
	private bool _hasChanges;
	/// <summary>
	/// The HasChanges property tracks the changes which allows the 'Save changes' button to
	/// be enabled or disabled accordingly.
	/// </summary>

	/// <summary>
	/// The constructor makes a copy of the donor mappings to display and to allow for
	/// changes to be reverted. It also sets the CollectionViewSource for the donor map
	/// DataGrid. Finally, it initializes the DeleteRowCmd, RevertCmd, SaveChangesCmd,
	/// DeleteAllCmd's to the handlers for each.
	/// </summary>
	public DonorMapViewModel(
		IReflectionHelpers reflectionHelpers,
		IDispatcherWrapper dispatcherWrapper,
		IDonorMapServices donorMapServices
	)
	{
		_reflectionHelpers = reflectionHelpers;
		_dispatcherWrapper = dispatcherWrapper;
		_donorMapServices = donorMapServices;

		HasChanges = true;

		foreach (var entry in _donorMapServices.AGDonorMapList)
		{
			DonorMapList.Add(_reflectionHelpers.CopyModel<AGDonorMapItem>(entry));
		}

		DonorMapViewSource.Source = DonorMapList;
	}

	/// <summary>
	/// This method is called when the tab is clicked on. This member allows
	/// for things to be refreshed incase new mappings were added since this page was
	/// last viewed.
	/// </summary>
	public new async Task Loading()
	{
		Revert();
		DonorMapViewSource.View.Refresh();
	}

	/// <summary>
	/// This method is called when another tab clicked on. This member allows for a reminder
	/// to save changes.
	/// </summary>
	public new async Task Leaving()
	{
		if (HasChanges)
		{
			var res = MessageBox.Show("If you leave the tab without saving changes, you will loose them. Do you want to save your changes?", "Donor map", MessageBoxButton.YesNo);
			if (MessageBoxResult.Yes == res)
			{
				await SaveChanges();
			}
		}
	}

	/// <summary>
	/// This method is called from DonorMapView.xaml.cs when the operator clicks on
	/// a DataGrid row to change a target donor. It passes in the AGDonorMapItem
	/// item associated with the row clicked, and the new Donor selected.
	/// </summary>
	/// <param name="item"></param>
	/// <param name="cat"></param>
	public void SetDonor(AGDonorMapItem? item, Donor? donor)
	{
		if (null == item || null == donor)
			return;

		item.DonorId = donor.Id;
		item.LastName = donor.LastName;
		item.FirstName = donor.FirstName;
		item.Address = donor.Address;
		item.City = donor.City;
		item.State = donor.State;
		item.Zip = donor.Zip;
		DonorMapViewSource.View.Refresh();
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
		AGDonorMapItem item = DonorMapViewSource.View.CurrentItem as AGDonorMapItem;
		if (null == item) return;

		HasChanges = true;
		DonorMapList?.Remove(item);
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

		DonorMapList.Clear();
		foreach (var entry in _donorMapServices.AGDonorMapList)
		{
			DonorMapList.Add(_reflectionHelpers.CopyModel<AGDonorMapItem>(entry));
			_dispatcherWrapper.Yield();
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
		_donorMapServices!.AGDonorMap!.Clear();
		_donorMapServices!.AGDonorMapList!.Clear();

		foreach (var entry in DonorMapList)
		{
			_donorMapServices.AGDonorMap[entry.AGDonorHash] = _reflectionHelpers.CopyModel<AGDonorMapItem>(entry);
			_donorMapServices.AGDonorMapList.Add(_donorMapServices.AGDonorMap[entry.AGDonorHash]);
		}

		HasChanges = false;

		await _donorMapServices.SaveDonorMap(DonorMapList, true);
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
		DonorMapList.Clear();
	}
}
