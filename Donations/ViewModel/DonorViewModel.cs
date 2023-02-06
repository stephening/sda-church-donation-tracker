using CommunityToolkit.Mvvm.Input;
using Donations.Model;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Donations.ViewModel
{
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
	public class DonorViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private char[] ro_firstNameSplitters = new char[] { ' ', '/', '&' };

		public CollectionViewSource FamilyMembers { get; set; } = new CollectionViewSource();
		public CollectionViewSource Donations { get; set; } = new CollectionViewSource();

		private Donor? _donor;
		/// <summary>
		/// This SelectedDonor property is bound to the DataContext of the Grid showing all the donor
		/// fields in a Form style view. Whenever this property is assigned a value, all the Grid
		/// fields will be update to show the donor properties.
		/// </summary>
		public Donor? SelectedDonor
		{
			get { return _donor; }
			set
			{
				_donor = value;
				// slip this filter refresh of family list in here before the general OnPropertyChanged()
				// because that will trigger a check of the family relationships before the FamilyMembers
				// list is updated
				FamilyMembers.View.Refresh();
				OnPropertyChanged();
			}
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

		/// <summary>
		/// The CancelCmd property is bound to the 'Cancel/clear fields' button. The action is to
		/// clear all Form fields and reset to not updating state.
		/// </summary>
		public ICommand CancelCmd { get; }

		private bool _updating = false;

		/// <summary>
		/// The constructor sets the CollectionViewSource for the family member list,
		/// and the donation list DataGrid's. And it initializes the CancelCmd to its handler.
		/// </summary>
		public DonorViewModel()
		{
			CancelCmd = new RelayCommand(Clear);

			FamilyMembers.Source = di.Data.DonorList;
			FamilyMembers.Filter += new FilterEventHandler(Filter);

			Donations.Source = di.Data.DonationList;
			Donations.Filter += new FilterEventHandler(DonorFilter);

			Clear();
		}

		/// <summary>
		/// This method is called if from the DonorModalView dialog to show donor fields
		/// if a donor is double clicked on from the family list.
		/// </summary>
		/// <param name="donor">Donor object chosen from the DonorSelectionView modal dialog.</param>
		public void SetDonor(Donor? donor)
		{
			_updating = true;
			SelectedDonor = donor;
			FamilyMembers.View.Refresh();
			Donations.View.Refresh();
			OnPropertyChanged(nameof(UpdateEnabled));
			OnPropertyChanged(nameof(AddEnabled));
		}

		/// <summary>
		/// This method is called if a new Donor is selected, so that the donation list will
		/// be updated to show the new donor's donations.
		/// </summary>
		private void RefrshDonationList()
		{
			Donations.View.Refresh();
		}

		/// <summary>
		/// This method is called in response to the execution of the CancelCmd.
		/// </summary>
		public void Clear()
		{
			_updating = false;
			SelectedDonor = new Donor();
			FamilyMembers.View.Refresh();
			Donations.View.Refresh();
			OnPropertyChanged(nameof(UpdateEnabled));
			OnPropertyChanged(nameof(AddEnabled));
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
		private int? CheckForCloseExistingMatch(ref int? partial, ref string? comment)
		{
			if (null == di.Data.DonorList || null == SelectedDonor)
				return null;

			comment = null;

			foreach (var donor in di.Data.DonorList)
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
				foreach (Donor member in FamilyMembers.View)
				{
					if (member != SelectedDonor && member.FamilyRelationship == enumFamilyRelationship.Primary)
					{
						VerificationFailureMessage = "The family member designated as Primary, will receive the year end donor report for all family members participating in GroupGiving. Hence, only one family member can be marked as Primary.";
						return false;
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
		public string? UpdateDonor()
		{
			if (null != SelectedDonor && di.Data.DonorDict.ContainsKey(SelectedDonor.Id))
			{
				if (Verify())
				{
					SelectedDonor.LastUpdated = DateTime.Now.ToString("yyyy/MM/dd");

					di.Data.SaveDonors(di.Data.DonorList);

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
		public string? AddDonor(bool force = false)
		{
			if (null != SelectedDonor && !di.Data.DonorDict.ContainsKey(SelectedDonor.Id))
			{
				// first make sure there isn't a close match to an existing donor in the database
				int? partialMatch = null;
				string? comment = null;
				var ret = CheckForCloseExistingMatch(ref partialMatch, ref comment);

				if (null != ret && !force)
				{
					return $"There is a {di.Data.DonorDict[ret.Value].FirstName} {di.Data.DonorDict[ret.Value].LastName} on {di.Data.DonorDict[ret.Value].Address} already in the database. Do you wish to add them anyway?";
				}
				else if (Verify())
				{
					// get new unique Id
					SelectedDonor.Id = ((0 < di.Data.DonorList.Count) ? di.Data.DonorList.Max(x => x.Id) : 0) + 1;
					SelectedDonor.LastUpdated = DateTime.Now.ToString("yyyy/MM/dd");
					di.Data.DonorList.Add(SelectedDonor);
					di.Data.DonorDict[SelectedDonor.Id] = SelectedDonor;

					di.Data.SaveDonors(di.Data.DonorList);

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
		/// This method checks for a family relationship conflict. The only conflict possible at this
		/// time is having two family member both be primary.
		/// </summary>
		/// <param name="relationship">True for no conflict, false otherwise.</param>
		/// <returns></returns>
		public bool FamilyRelationshipConflict(string? relationship)
		{
			if (relationship == enumFamilyRelationship.Primary.ToString())
			{
				foreach (Donor member in FamilyMembers.View)
				{
					if (member != SelectedDonor && member.FamilyRelationship == enumFamilyRelationship.Primary)
					{
						return true;
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
		public string? ChooseRelated(int relatedDonorId, bool force=false)
		{
			if (di.Data.DonorDict.ContainsKey(relatedDonorId))
			{
				if (null == di.Data.DonorDict[relatedDonorId].FamilyId && !force)
				{
					return $"The selected donor, {di.Data.DonorDict[relatedDonorId].FirstName} {di.Data.DonorDict[relatedDonorId].LastName}, does not currently have any family. Do you wish to start one?";
				}
				else
				{
					if (null == di.Data.DonorDict[relatedDonorId].FamilyId)
					{
						di.Data.DonorDict[relatedDonorId].FamilyId = ((0 < di.Data.DonorList.Count) ? di.Data.DonorList.Max(x => x.FamilyId) : 0) + 1;
					}
					SelectedDonor.FamilyId = di.Data.DonorDict[relatedDonorId].FamilyId;
					FamilyMembers.View.Refresh();
					return null;
				}
			}

			return "Invalid donor Id selected";
		}

		/// <summary>
		/// This method is used to filter the entire donor list for related family member via the FamilyId.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Filter(object sender, FilterEventArgs e)
		{
			var obj = e.Item as Donor;
			if (null != obj && null != SelectedDonor)
			{
				if (SelectedDonor.FamilyId == obj.FamilyId && null != SelectedDonor.FamilyId)
					e.Accepted = true;
				else
					e.Accepted = false;
			}
			else
				e.Accepted = false;
		}

		/// <summary>
		/// This method is used to fileter all donation records for donations made by this donor.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DonorFilter(object sender, FilterEventArgs e)
		{
			var obj = e.Item as Donation;
			if (null != obj && null != SelectedDonor)
			{
				if (SelectedDonor.Id == obj.DonorId)
					e.Accepted = true;
				else
					e.Accepted = false;
			}
			else
				e.Accepted = false;
		}

		/// <summary>
		/// This method is called if a new donor is selected for this donor Form view.
		/// </summary>
		/// <param name="donorId"></param>
		public void ChooseDonor(int donorId)
		{
			SelectedDonor = di.Data.DonorDict[donorId];

			RefrshDonationList();

			_updating = true;
			OnPropertyChanged(nameof(UpdateEnabled));
			OnPropertyChanged(nameof(AddEnabled));
		}
	}
}
