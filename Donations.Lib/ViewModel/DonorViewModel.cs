using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the DonorView.xaml which is a
/// UserControl, that is shown in the 'Maintenance:Donor' tab.
/// 
/// This control will allow the operator to choose a donor and then view and edit. It also
/// allows for adding a new donor.
/// 
/// There are three buttons, 'Revert changes', 'Save changes', 'Delete all', and there is a context
/// menu with a 'Delete row', 'Inser row above', and 'Insert row below' options. The Codes and
/// Descriptions can be changed inline in the DataGrid view.
/// </summary>
public partial class DonorViewModel : BaseViewModel
{
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private readonly IReflectionHelpers _reflectionHelpers;
	private readonly DonorModalView.Factory _donorViewFactory;
	private readonly IDonorServices _donorServices;
	private readonly IDonationServices _donationServices;
	private readonly IDonorMapServices _donorMapServices;
	private readonly IDonorChangeServices _donorChangeServices;
	private readonly DonorMapViewModel _donorMapViewModel;
	private readonly IAppSettingsServices _appSettingsServices;
	private char[] ro_firstNameSplitters = new char[] { ' ', '/', '&' };

	private bool _updating = false;
	private Donor? _relatedDonorToUpdate;
	private bool _dontCheckWhenChangingDonor = false;

	/// <summary>
	/// The constructor sets the CollectionViewSource for the family member list,
	/// and the donation list DataGrid's. And it initializes the CancelCmd to its handler.
	/// </summary>
	public DonorViewModel(
		IDispatcherWrapper dispatcherWrapper,
		IReflectionHelpers reflectionHelpers,
		DonorModalView.Factory donorViewFactory,
		IDonorServices donorServices,
		IDonationServices donationServices,
		IDonorMapServices donorMapServices,
		IDonorChangeServices donorChangeServices,
		DonorMapViewModel donorMapViewModel,
		IAppSettingsServices appSettingsServices
	)
	{
		_dispatcherWrapper = dispatcherWrapper;
		_reflectionHelpers = reflectionHelpers;
		_donorViewFactory = donorViewFactory;
		_donorServices = donorServices;
		_donationServices = donationServices;
		_donorMapServices = donorMapServices;
		_donorChangeServices = donorChangeServices;
		_donorMapViewModel = donorMapViewModel;
		_appSettingsServices = appSettingsServices;
		_dispatcherWrapper.BeginInvoke(RefreshLists);

		Clear();
	}

	public CollectionViewSource FamilyMembers { get; set; } = new CollectionViewSource();
	public CollectionViewSource Donations { get; set; } = new CollectionViewSource();
	public CollectionViewSource DonorChanges { get; set; } = new CollectionViewSource();

	private List<Donor>? _givingGroup;

	[ObservableProperty]
	private Visibility _donationsVisibility;

	[ObservableProperty]
	private Donor? _selectedDonor;
	/// <summary>
	/// This SelectedDonor property is bound to the DataContext of the Grid showing all the donor
	/// fields in a Form style view. Whenever this property is assigned a value, all the Grid
	/// fields will be update to show the donor properties.
	/// </summary>
	partial void OnSelectedDonorChanged(Donor? value)
	{
		_dispatcherWrapper.BeginInvoke(RefreshLists);
	}

	/// <summary>
	/// The UpdateEnabled property will control the enable/disable state of the Update button. The
	/// state is updating if a donor has been selected, and not updating otherwise. The Form fields
	/// are editable and can be filled out to add a donor.
	/// </summary>
	public bool UpdateEnabled => _updating;

	/// <summary>
	/// The AddEnabled property will control the enable/disable state of the Add button. The
	/// state is updating if a donor has been selected, and not updating otherwise. The Form fields
	/// are editable and can be filled out to add a donor.
	/// </summary>
	public bool AddEnabled => !_updating;

	/// <summary>
	/// The VerificationFailureMessage property will contain a message if some inconsistency is 
	/// found during verification of the form fields. For example, two member of the same family
	/// cannot be primary.
	/// </summary>
	public string? VerificationFailureMessage { get; set; }

	public string? PictureUrl => _appSettingsServices.Get()?.PictureBaseUrl + SelectedDonor?.PictureFile;

	public DonorModalView Create(Donor? donor, bool showDonations = true)
	{
		return _donorViewFactory(donor, showDonations);
	}

	/// <summary>
	/// This method refreshes both the family members list and the donation list.
	/// </summary>
	/// <returns></returns>
	public async Task RefreshLists()
	{
		if (null != SelectedDonor)
		{
			if (null != SelectedDonor.FamilyId && true == SelectedDonor.GroupGiving)
			{
				var family = await _donorServices.GetDonorsByFamilyId(SelectedDonor.FamilyId.Value);
				FamilyMembers.Source = family;
				_givingGroup = new List<Donor>(family.Where(x => x.GroupGiving == true));
				if (_givingGroup.Any())
				{
					List<Donor> temp = new List<Donor>(_givingGroup.Where(x => x.FamilyRelationship == enumFamilyRelationship.Primary));
					if (!temp.Any())
					{
						_givingGroup = null;
					}
				}
				else
				{
					_givingGroup = null;
				}
			}
			else
			{
				FamilyMembers.Source = null;
				_givingGroup = null;
			}

			if (null != _givingGroup)
			{
				Donations.Source = await _donationServices.GetDonationsByDonorIds(_givingGroup.Select(x => x.Id).ToList());
			}
			else
			{
				Donations.Source = await _donationServices.GetDonationsByDonorId(SelectedDonor.Id);
			}

			Donations.SortDescriptions.Add(new SortDescription() { PropertyName = "Date", Direction = ListSortDirection.Descending });

			DonorChanges.Source = await _donorChangeServices.GetDonorChangesByDonorId(SelectedDonor.Id);
			DonorChanges.View.SortDescriptions.Add(new SortDescription("WhenChanged", ListSortDirection.Descending));
		}
	}

	/// <summary>
	/// This method is called if from the DonorModalView dialog to show donor fields
	/// if a donor is double clicked on from the family list.
	/// </summary>
	/// <param name="donor">Donor object chosen from the DonorSelectionView modal dialog.</param>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task SetDonor(Donor? donor)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		_dontCheckWhenChangingDonor = true;
		SelectedDonor = donor;

		OnPropertyChanged(nameof(PictureUrl));

		//await RefreshLists();

		_updating = true;
		OnPropertyChanged(nameof(UpdateEnabled));
		OnPropertyChanged(nameof(AddEnabled));
		_dontCheckWhenChangingDonor = false;
	}

	/// <summary>
	/// This method is called in response to the execution of the RefreshCmd.
	/// </summary>
	public void RefreshPicture()
	{
		OnPropertyChanged(nameof(PictureUrl));
	}

	/// <summary>
	/// This method is called in response to the execution of the CancelCmd.
	/// </summary>
	[RelayCommand]
	public void Cancel()
	{
		Clear();
	}

	public void Clear()
	{
		_updating = false;

		SelectedDonor = new Donor();
		FamilyMembers.Source = null;
		Donations.Source = null;
		DonorChanges.Source = null;

		OnPropertyChanged(nameof(UpdateEnabled));
		OnPropertyChanged(nameof(AddEnabled));
		OnPropertyChanged(nameof(PictureUrl));
	}

	/// <summary>
	/// This method is called when adding a new donor, to make sure this donor isn't the same as
	/// someone already in the database. It has two ref parmeters that allow it to return some
	/// information.
	/// </summary>
	/// <param name="partial">This is set to a donor id where the last name matched but not the
	/// first</param>
	/// <param name="comment">This field will contain text about the quality of the match if it
	/// is just a partial match.</param>
	/// <returns></returns>
	private async Task<int?> CheckForCloseExistingMatch()
	{
#pragma warning disable CS8604 // Possible null reference argument.
		ObservableCollection<Donor>? donors = await _donorServices.FilterDonors(last: SelectedDonor.LastName);
#pragma warning restore CS8604 // Possible null reference argument.

		if (0 == donors?.Count || null == SelectedDonor)
			return null;

		int? partial = null;
		string? comment = null;

		comment = null;

		foreach (var donor in donors)
		{
			if (Helper.Compare(ref comment, ref partial, donor
				, SelectedDonor.LastName
				, SelectedDonor.FirstName
				, SelectedDonor.Address
				, delim: ro_firstNameSplitters))
			{
				// close match
				return donor.Id;
			}
		}

		return null;
	}

	/// <summary>
	/// This method is called prior to updating or adding a donor to make sure there are no consistency issues.
	/// </summary>
	/// <returns>True if there were no issues, false otherwise.</returns>
	private bool Verify()
	{
		// make sure that one address is marked primary or both
		if ((SelectedDonor?.AddressType == enumAddressType.Mailing || SelectedDonor?.AddressType == enumAddressType.Both)
			&& (SelectedDonor?.AltAddressType == enumAddressType.Mailing || SelectedDonor?.AltAddressType == enumAddressType.Both))
		{
			VerificationFailureMessage = "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'";
			return false;
		}

		if (SelectedDonor?.FamilyRelationship == enumFamilyRelationship.Primary)
		{
			if (null != FamilyMembers.Source as ObservableCollection<Donor>)
			{
				ObservableCollection<Donor>? family = FamilyMembers.Source as ObservableCollection<Donor>;
				foreach (Donor member in family!)
				{
					if (member.Id != SelectedDonor.Id && member.FamilyRelationship == enumFamilyRelationship.Primary)
					{
						VerificationFailureMessage = "The family member designated as Primary, will receive the year end donor report for all family members participating in GroupGiving. Hence, only one family member can be marked as Primary.";
						return false;
					}
				}
			}
		}

		return true;
	}

	/// <summary>
	/// This method is called from the Click handler to update the donor record. Some checking will occur first
	/// and if there are inconsistencies, a message and a prompt may be given to the operator. Since there is 
	/// no force parameter, there is no option to ignore the inconsistencies and update changes.
	/// </summary>
	/// <returns>The return value is null if there were no issues. If there was something the operator should
	/// be made aware of, the return value will be a string containing that message.</returns>
	public async Task<string?> UpdateDonor()
	{
		if (null != SelectedDonor && null != _donorServices.GetDonorById(SelectedDonor.Id))
		{
			if (Verify())
			{
				SelectedDonor.LastUpdated = DateTime.Now;

				await _donorServices.UpdateDonor(SelectedDonor);

				if (null != _relatedDonorToUpdate)
				{
					await _donorServices.UpdateDonor(_relatedDonorToUpdate);
					_relatedDonorToUpdate = null;
				}

				foreach (var item in _donorMapServices.AGDonorMapList!)
				{
					if (item.DonorId == SelectedDonor.Id)
					{
						item.RefreshDonorFields(SelectedDonor);
					}
				}

				// refresh donor mappings in maint. tab
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				_donorMapViewModel.Loading();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

				_updating = false;
				Clear();
			}
			else
			{
				return VerificationFailureMessage;
			}
		}

		return null;
	}

	/// <summary>
	/// This method is called from the Click handler to add donor record. Some checking will occur first
	/// and if there are inconsistencies, a message and a prompt may be given to the operator.
	/// </summary>
	/// <param name="force">If the operator wishes to add the donor inspite of the inconsistencies or
	/// warnings.</param>
	/// <returns>The return value is null if there were no issues. If there was something the operator should
	/// be made aware of, the return value will be a string containing that message.</returns>
	public async Task<string?> AddDonor(bool force = false)
	{
		if (null != SelectedDonor)
		{
			// first make sure there isn't a close match to an existing donor in the database
			var ret = await CheckForCloseExistingMatch();

			if (null != ret && !force)
			{
				Donor donor = await _donorServices.GetDonorByIdAsync(ret.Value);

				return $"There is a {donor.FirstName} {donor.LastName} on {donor.Address} already in the database. Do you wish to add them anyway?";
			}
			else if (Verify())
			{
				SelectedDonor.LastUpdated = DateTime.Now;

				await _donorServices.AddDonor(SelectedDonor);

				if (null != _relatedDonorToUpdate)
				{
					await _donorServices.UpdateDonor(_relatedDonorToUpdate);
					_relatedDonorToUpdate = null;
				}

				Clear();
			}
			else
			{
				return VerificationFailureMessage;
			}
		}

		return null;
	}

	/// <summary>
	/// This method will delete the currently selected donor only if force is set to true.
	/// </summary>
	/// <param name="force"></param>
	/// <returns></returns>
	public async Task<string?> DeleteDonor(bool force = false)
	{
		if (null != SelectedDonor)
		{
			var collection = await _donationServices.GetDonationsByDonorId(SelectedDonor.Id);
			if (0 < collection.Count && !force)
			{
				return $"\"{SelectedDonor.Name}\" has {collection.Count} donations in the database. Are you sure you want to delete \"{SelectedDonor.Name}\"";
			}

			await _donorServices.DeleteDonor(SelectedDonor);

			Clear();
		}

		return null;
	}

	/// <summary>
	/// This method will delete the currently selected donor only if force is set to true.
	/// </summary>
	/// <param name="force"></param>
	/// <returns></returns>
	public async Task<ObservableCollection<Donation>>? MergeDonor(Donor donorToMerge, bool force = false)
	{
		if (null != donorToMerge)
		{
			var collection = await _donationServices.GetDonationsByDonorId(donorToMerge.Id);
			if (0 < collection.Count && !force)
			{
				return collection;
			}

#pragma warning disable CS8604 // Possible null reference argument.
			Donor newDonor = _reflectionHelpers.CopyModel<Donor>(SelectedDonor);
#pragma warning restore CS8604 // Possible null reference argument.

			if (newDonor.FamilyId != donorToMerge.FamilyId)
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.FamilyId} with {donorToMerge.FamilyId}?", "FamilyId different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.FamilyId = donorToMerge.FamilyId;
				}
			}

			if (newDonor.FamilyRelationship != donorToMerge.FamilyRelationship)
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.FamilyRelationship} with {donorToMerge.FamilyRelationship}?", "FamilyRelationship different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.FamilyRelationship = donorToMerge.FamilyRelationship;
				}
			}

			if (!Helper.Equal(newDonor.FirstName, donorToMerge.FirstName))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.FirstName} with {donorToMerge.FirstName}?", "First name different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.FirstName = donorToMerge.FirstName;
				}
			}

			if (!Helper.Equal(newDonor.PreferredName, donorToMerge.PreferredName))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.PreferredName} with {donorToMerge.PreferredName}?", "Preferred name different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.PreferredName = donorToMerge.PreferredName;
				}
			}

			if (!Helper.Equal(newDonor.LastName, donorToMerge.LastName))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.LastName} with {donorToMerge.LastName}?", "Last name different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.LastName = donorToMerge.LastName;
				}
			}

			if (newDonor.Gender != donorToMerge.Gender)
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.Gender} with {donorToMerge.Gender}?", "Gender different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.Gender = donorToMerge.Gender;
				}
			}

			if (!Helper.Equal(newDonor.Email, donorToMerge.Email))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.Email} with {donorToMerge.Email}?", "Email different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.Email = donorToMerge?.Email;
				}
			}

			if (!Helper.Equal(newDonor.Email2, donorToMerge.Email2))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.Email2} with {donorToMerge.Email2}?", "Email2 different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.Email2 = donorToMerge.Email2;
				}
			}

			if (!Helper.Equal(newDonor.HomePhone, donorToMerge.HomePhone))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.HomePhone} with {donorToMerge.HomePhone}?", "HomePhone different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.HomePhone = donorToMerge?.HomePhone;
				}
			}

			if (!Helper.Equal(newDonor.MobilePhone, donorToMerge.MobilePhone))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.MobilePhone} with {donorToMerge.MobilePhone}?", "MobilePhone different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.MobilePhone = donorToMerge.MobilePhone;
				}
			}

			if (!Helper.Equal(newDonor.WorkPhone, donorToMerge.WorkPhone))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.WorkPhone} with {donorToMerge.WorkPhone}?", "WorkPhone different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.WorkPhone = donorToMerge.WorkPhone;
				}
			}

			if (newDonor.AddressType != donorToMerge.AddressType)
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.AddressType} with {donorToMerge.AddressType}?", "AddressType different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.AddressType = donorToMerge.AddressType;
				}
			}

			if (!Helper.Equal(newDonor.Address, donorToMerge.Address))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.Address} with {donorToMerge.Address}?", "Address different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.Address = donorToMerge?.Address;
				}
			}

			if (!Helper.Equal(newDonor.Address2, donorToMerge.Address2))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.Address2} with {donorToMerge.Address2}?", "Address2 different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.Address2 = donorToMerge?.Address2;
				}
			}

			if (!Helper.Equal(newDonor.City, donorToMerge.City))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.City} with {donorToMerge.City}?", "City different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.City = donorToMerge?.City;
				}
			}

			if (!Helper.Equal(newDonor.State, donorToMerge.State, eFlags.State))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.State} with {donorToMerge.State}?", "State different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.State = donorToMerge?.State;
				}
			}

			if (!Helper.Equal(newDonor.Zip, donorToMerge.Zip, eFlags.Length, len: 5))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.Zip} with {donorToMerge.Zip}?", "Zip different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.Zip = donorToMerge?.Zip;
				}
			}

			if (!Helper.Equal(newDonor.Country, donorToMerge.Country))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.Country} with {donorToMerge.Country}?", "Country different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.Country = donorToMerge?.Country;
				}
			}

			if (newDonor.AltAddressType != donorToMerge.AltAddressType)
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.AltAddressType} with {donorToMerge.AltAddressType}?", "AltAddressType different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.AltAddressType = donorToMerge?.AltAddressType;
				}
			}

			if (!Helper.Equal(newDonor.AltAddress, donorToMerge.AltAddress))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.AltAddress} with {donorToMerge.AltAddress}?", "AltAddress different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.AltAddress = donorToMerge?.AltAddress;
				}
			}

			if (!Helper.Equal(newDonor.AltAddress2, donorToMerge.AltAddress2))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.AltAddress2} with {donorToMerge.AltAddress2}?", "AltAddress2 different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.AltAddress2 = donorToMerge?.AltAddress2;
				}
			}

			if (!Helper.Equal(newDonor.AltCity, donorToMerge.AltCity))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.AltCity} with {donorToMerge.AltCity}?", "AltCity different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.AltCity = donorToMerge?.AltCity;
				}
			}

			if (!Helper.Equal(newDonor.AltState, donorToMerge.AltState, eFlags.State))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.AltState} with {donorToMerge.AltState}?", "AltState different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.AltState = donorToMerge?.AltState;
				}
			}

			if (!Helper.Equal(newDonor.AltZip, donorToMerge.AltZip, eFlags.Length, len: 5))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.AltZip} with {donorToMerge.AltZip}?", "AltZip different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.AltZip = donorToMerge?.AltZip;
				}
			}

			if (!Helper.Equal(newDonor.AltCountry, donorToMerge.AltCountry))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.AltCountry} with {donorToMerge.AltCountry}?", "AltCountry different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.AltCountry = donorToMerge?.AltCountry;
				}
			}

			if (!Helper.Equal(newDonor.Birthday, donorToMerge.Birthday, eFlags.Date))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.Birthday} with {donorToMerge.Birthday}?", "Birthday different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.Birthday = donorToMerge?.Birthday;
				}
			}

			if (!Helper.Equal(newDonor.Baptism, donorToMerge.Baptism, eFlags.Date))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.Baptism} with {donorToMerge.Baptism}?", "Baptism different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.Baptism = donorToMerge?.Baptism;
				}
			}

			if (!Helper.Equal(newDonor.Deathday, donorToMerge.Deathday, eFlags.Date))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.Deathday} with {donorToMerge.Deathday}?", "Deathday different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.Deathday = donorToMerge?.Deathday;
				}
			}

			if (newDonor.GroupGiving != donorToMerge.GroupGiving)
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.GroupGiving} with {donorToMerge.GroupGiving}?", "GroupGiving different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.GroupGiving = donorToMerge?.GroupGiving;
				}
			}

			if (newDonor.ChurchMember != donorToMerge.ChurchMember)
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.ChurchMember} with {donorToMerge.ChurchMember}?", "ChurchMember different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.ChurchMember = donorToMerge?.ChurchMember;
				}
			}

			if (newDonor.MaritalStatus != donorToMerge.MaritalStatus)
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.MaritalStatus} with {donorToMerge.MaritalStatus}?", "MaritalStatus different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.MaritalStatus = donorToMerge?.MaritalStatus;
				}
			}

			if (!Helper.Equal(newDonor.Notes, donorToMerge.Notes))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.Notes} with {donorToMerge.Notes}?", "Notes different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.Notes = donorToMerge?.Notes;
				}
			}

			if (!Helper.Equal(newDonor.ActiveGroups, donorToMerge.ActiveGroups))
			{
				var ret = MessageBox.Show($"Do you wish to replace {newDonor.ActiveGroups} with {donorToMerge.ActiveGroups}?", "ActiveGroups different", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.No);
#pragma warning disable CS8603 // Possible null reference return.
				if (MessageBoxResult.Cancel == ret) return null;
#pragma warning restore CS8603 // Possible null reference return.
				if (MessageBoxResult.Yes == ret)
				{
					newDonor.ActiveGroups = donorToMerge?.ActiveGroups;
				}
			}

			await _donationServices.RemapDonorId(donorToMerge.Id, SelectedDonor.Id);

			await _donorServices.DeleteDonor(donorToMerge);

			await SetDonor(newDonor);

			await UpdateDonor();
		}

#pragma warning disable CS8603 // Possible null reference return.
		return null;
#pragma warning restore CS8603 // Possible null reference return.
	}

	/// <summary>
	/// This method checks for a family relationship conflict. The only conflict possible at this
	/// time is having two family member both be primary.
	/// </summary>
	/// <param name="relationship">True for no conflict, false otherwise.</param>
	/// <returns></returns>
	public bool FamilyRelationshipConflict(string? relationship)
	{
		if (_dontCheckWhenChangingDonor) return false;

		if (relationship == enumFamilyRelationship.Primary.ToString())
		{
			if (null != FamilyMembers.Source)
			{
				foreach (Donor member in FamilyMembers.Source as ObservableCollection<Donor>)
				{
					if (member.Id != SelectedDonor.Id && member.FamilyRelationship == enumFamilyRelationship.Primary)
					{
						return true;
					}
				}
			}
		}

		return false;
	}

	/// <summary>
	/// This method allows the operator to add the current person to a family by choosing another donor.
	/// If the other donor chosen is part of a family, this donor will be added to that same family. If
	/// the chosen donor is not part of a family, a new family id can be created for both donors by
	/// setting the force parameter to true.
	/// </summary>
	/// <param name="relatedDonorId">Add current donor to relatedFamilyId's family.</param>
	/// <param name="force">Establish a family relationship where none existed before.</param>
	/// <returns>If the selected related donor doesn't have any family relationships, the return value
	/// will be a message detailing that, and asking if the operator would like to start a family.</returns>
	public async Task<string?> ChooseRelated(int relatedDonorId, bool force = false)
	{
		Donor donor = _donorServices.GetDonorById(relatedDonorId);
		if (null != donor)
		{
			int? familyId = SelectedDonor.FamilyId;

			if (null != familyId && null != familyId && familyId != donor.FamilyId && !force)
			{
				return $"The selected donor \"{SelectedDonor.Name}\" and the one in question \"{donor.Name}\" each currently have different families? Do you wish to change \"{donor.Name}\" to join the family of \"{SelectedDonor.Name}\"?";
			}
			else if (null == familyId && null == donor.FamilyId && !force)
			{
				return $"Neither the selected donor, \"{SelectedDonor.Name}\" or the one in question, \"{donor.Name}\" are assigned to families. Do you wish to create a new family group?";
			}
			else
			{
				if (null != familyId && null != familyId && familyId != donor.FamilyId)
				{
					donor.FamilyId = familyId;
					_relatedDonorToUpdate = donor;
				}
				else if (null == familyId && null == donor.FamilyId)
				{
					familyId = await _donorServices.GetNextFamilyId();
					SelectedDonor.FamilyId = familyId;
					donor.FamilyId = familyId;
					_relatedDonorToUpdate = donor;
				}
				else if (null != familyId)
				{
					donor.FamilyId = familyId;
					_relatedDonorToUpdate = donor;
				}
				else
				{
					SelectedDonor.FamilyId = donor.FamilyId;
				}

				await RefreshLists();
				return null;
			}
		}

		return "Invalid donor Id selected";
	}

	/// <summary>
	/// This method is called if a new donor is selected for this donor Form view.
	/// </summary>
	/// <param name="donorId"></param>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task ChooseDonor(int donorId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		_dontCheckWhenChangingDonor = true;

		SelectedDonor = _donorServices.GetDonorById(donorId);

		OnPropertyChanged(nameof(PictureUrl));

		//await RefreshLists();

		_updating = true;
		OnPropertyChanged(nameof(UpdateEnabled));
		OnPropertyChanged(nameof(AddEnabled));
		_dontCheckWhenChangingDonor = false;
	}
}
