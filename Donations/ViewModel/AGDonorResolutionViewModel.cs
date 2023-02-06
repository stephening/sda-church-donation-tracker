using Donations.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace Donations.ViewModel
{
	/// <summary>
	/// This view model handles the functionality and the binding to the AGDonorResolutionView.xaml which is a
	/// UserControl under the 'Donor resolution' tab.
	/// 
	/// This class will atomatically try to match Adventist Giving (AG) donors, to the local donor database. If
	/// an exact match is not found, it will present an option to the operator. But ultimately the operator will
	/// have to decide whether to accept the propsed donor, or select another one. Once the operator accepts or
	/// chooses a different donor, a mapping is created which will be used from then on. That donor will be
	/// automatically mapped from then on unless the AG user changes something in their name or address in their
	/// AG account. If no close match is found, the user will be required to select a donor themselves, and if
	/// one doesn't exist, they can simply press the 'Copy all...' button to create a new donor record with the
	/// AG contact info. Once the donor resolution is complete for the who batch, move on to category resolution
	/// of verification and submit.
	/// </summary>
	public class AGDonorResolutionViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObservableCollection<AdventistGiving>? TransactionList => di.AG?.TransactionList;

		private char[] ro_firstNameSplitters = new char[] { ' ', '/', '&' };

		private Donor? _donor;

		/// <summary>
		/// The Donor property if it contains something is bound to the fields on the left side, which show a 
		/// proposed donor match. If no, close match was found, the Donor will be null and no fields will be
		/// shown.
		/// </summary>
		public Donor? Donor
		{
			get { return _donor; }
			set
			{
				_donor = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ContinueEnabled));
			}
		}

		private int? _txIdx = null;

		private AdventistGiving? _transaction;

		/// <summary>
		/// The Transaction property will contain the current AG record that is needing donor resolution.
		/// </summary>
		public AdventistGiving? Transaction
		{
			get { return _transaction; }
			set
			{
				_transaction = value;
				OnPropertyChanged();
			}
		}

		private string? _progressText;

		/// <summary>
		/// The ProgressText property is displayed in the user control to show the current record number that is needing resolution.
		/// </summary>
		public string? ProgressText
		{
			get { return _progressText; }
			set
			{
				_progressText = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The LastNameCopyVisible property makes the button to copy the LastName visible if the LastName's do not match.
		/// </summary>
		public Visibility LastNameCopyVisible => Helper.Equal(Donor?.LastName, Transaction?.LastName) ? Visibility.Hidden : Visibility.Visible;
		/// <summary>
		/// The FirstNameCopyVisible property makes the button to copy the FirstName visible if the FirstName's do not match.
		/// </summary>
		public Visibility FirstNameCopyVisible => Helper.Equal(Donor?.FirstName, Transaction?.FirstName) ? Visibility.Hidden : Visibility.Visible;
		/// <summary>
		/// The AddressCopyVisible property makes the button to copy the Address visible if the Address' do not match.
		/// </summary>
		public Visibility AddressCopyVisible => Helper.Equal(Donor?.Address, Transaction?.Address) ? Visibility.Hidden : Visibility.Visible;
		/// <summary>
		/// The Address2CopyVisible property makes the button to copy the Address2 visible if the Address2's do not match.
		/// </summary>
		public Visibility Address2CopyVisible => Helper.Equal(Donor?.Address2, Transaction?.Address2) ? Visibility.Hidden : Visibility.Visible;
		/// <summary>
		/// The CityCopyVisible property makes the button to copy the City visible if the City's do not match.
		/// </summary>
		public Visibility CityCopyVisible => Helper.Equal(Donor?.City, Transaction?.City) ? Visibility.Hidden : Visibility.Visible;
		/// <summary>
		/// The StateCopyVisible property makes the button to copy the State visible if the State's do not match.
		/// </summary>
		public Visibility StateCopyVisible => Helper.Equal(Donor?.State, Transaction?.State) ? Visibility.Hidden : Visibility.Visible;
		/// <summary>
		/// The ZipCopyVisible property makes the button to copy the Zip code visible if the Zip's do not match.
		/// </summary>
		public Visibility ZipCopyVisible => Helper.Equal(Donor?.Zip, Transaction?.Zip) ? Visibility.Hidden : Visibility.Visible;
		/// <summary>
		/// The CopyAllVisible property makes the button to copy the all fields visible in case the operator wishes to
		/// just update all mismatched fields. If no partial match was found, the operator can use the 'Copy all...'
		/// button to create a new local donor record based on the AG fields.
		/// </summary>
		public Visibility CopyAllVisible => (LastNameCopyVisible == Visibility.Visible || FirstNameCopyVisible == Visibility.Visible
			|| AddressCopyVisible == Visibility.Visible || Address2CopyVisible == Visibility.Visible || CityCopyVisible == Visibility.Visible
			|| StateCopyVisible == Visibility.Visible || ZipCopyVisible == Visibility.Visible) ? Visibility.Visible : Visibility.Hidden;

		/// <summary>
		/// The ContinueEnabled property is a boolean that will facilitate enabling or disabling the 'Continue...' button depending
		/// on whether a mapping target is available.
		/// </summary>
		public bool ContinueEnabled => (null != Donor) ? true : false;

		private Visibility _donorDiffsVisibility = Visibility.Visible;

		/// <summary>
		/// The DonorDiffsVisibility property controls the visibility of most of this control. While donor resolution is in
		/// progress, it will be visible. Once the donor resolution is complete, this propery will be set to Hidden.
		/// </summary>
		public Visibility DonorDiffsVisibility
		{
			get { return _donorDiffsVisibility; }
			set
			{
				_donorDiffsVisibility = value;
				OnPropertyChanged();
			}
		}

		private Visibility _donorResolutionComplete = Visibility.Hidden;

		/// <summary>
		/// The DonorResolutionComplete property is simply the opposite of the DonorDiffsVisibility property. When the latter is
		/// Hidden, this one will be visible and vice versa.
		/// </summary>
		public Visibility DonorResolutionComplete
		{
			get { return _donorResolutionComplete; }
			set
			{
				_donorResolutionComplete = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The ContinueDonorResolutionCmd property is the Command binding for the 'Continue...' button.
		/// </summary>
		public IAsyncRelayCommand ContinueDonorResolutionCmd { get; }
		/// <summary>
		/// The CopyLastNameCmd property is bound to the button for copying the LastName if visible.
		/// </summary>
		public ICommand CopyLastNameCmd { get; }
		/// <summary>
		/// The CopyFirstNameCmd property is bound to the button for copying the FirstName if visible.
		/// </summary>
		public ICommand CopyFirstNameCmd { get; }
		/// <summary>
		/// The CopyAddressCmd property is bound to the button for copying the Address if visible.
		/// </summary>
		public ICommand CopyAddressCmd { get; }
		/// <summary>
		/// The CopyAddress2Cmd property is bound to the button for copying the Address2 if visible.
		/// </summary>
		public ICommand CopyAddress2Cmd { get; }
		/// <summary>
		/// The CopyCityCmd property is bound to the button for copying the City if visible.
		/// </summary>
		public ICommand CopyCityCmd { get; }
		/// <summary>
		/// The CopyStateCmd property is bound to the button for copying the State if visible.
		/// </summary>
		public ICommand CopyStateCmd { get; }
		/// <summary>
		/// The CopyZipCmd property is bound to the button for copying the Zip code if visible.
		/// </summary>
		public ICommand CopyZipCmd { get; }
		/// <summary>
		/// The CopyAllCmd property is bound to the button which if pressed will copy all the
		/// name/address fields from the AG record, updating the local donor fields accordingly.
		/// If no new donor match was found, the 'Copy all' command will create a new donor
		/// record from the information from the AG record. After copying, it will automatically
		/// advence to the next resolution.
		/// </summary>
		public ICommand CopyAllCmd { get; }

		#region member variables

		private string? _comment;

		public int? _lastNameMatchId;

		#endregion

		/// <summary>
		/// The constructor places its this pointer in the Global static object for use by other ViewModels
		/// that may need access to its public members. It also connects the methods to the Command bindings.
		/// </summary>
		public AGDonorResolutionViewModel()
		{
			// register this with the Global object for use by other view models
			di.DonorResolution = this;

			// Setup the command handlers for the button Command bindings
			ContinueDonorResolutionCmd = new AsyncRelayCommand(ContinueDonorResolution);
			CopyLastNameCmd = new RelayCommand(CopyLastName);
			CopyFirstNameCmd = new RelayCommand(CopyFirstName);
			CopyAddressCmd = new RelayCommand(CopyAddress);
			CopyAddress2Cmd = new RelayCommand(CopyAddress2);
			CopyCityCmd = new RelayCommand(CopyCity);
			CopyStateCmd = new RelayCommand(CopyState);
			CopyZipCmd = new RelayCommand(CopyZip);
			CopyAllCmd = new RelayCommand(CopyAll);
		}

		/// <summary>
		/// This method simply sets the opposing Visible/Hidden properties based on a bool.
		/// </summary>
		/// <param name="flag"></param>
		public void ResolutionComplete(bool flag)
		{
			if (flag)
			{
				DonorResolutionComplete = Visibility.Visible;
				DonorDiffsVisibility = Visibility.Hidden;
			}
			else
			{
				DonorResolutionComplete = Visibility.Hidden;
				DonorDiffsVisibility = Visibility.Visible;
			}
		}

		/// <summary>
		/// This function will try to find a match between a donor in the Adventist Giving *.csv
		/// and the donor database. It will do fuzzy matching of name and address.
		/// 
		/// Fuzzy matching is defined by the following.
		/// - All string comparisons are case insensitive.
		/// - Only compare the first word of the FirstName field. This is to allow for a middle
		///   initial in one but not the other.
		/// - Only compare the first 5 characters of the zip or postal code.
		/// </summary>
		/// <param name="transaction">Adventist Giving transaction to find donor id for.</param>
		/// <param name="partial">If only a partial match is found, this returns the id of a Donor record with the last name that matched.</param>
		/// <param name="comment">Parameter which returns some comments about a fuzzy match.</param>
		/// <returns>Returns null if a match is not found. Or a local donor ID if a match
		/// or fuzzy match is found. If an exact match is found the comment ref parameter
		/// will be set to null.</returns>
		private int? FindDonorId(AdventistGiving transaction, ref int? partial, ref string? comment)
		{
			// There are two immediate return conditions.
			// 1. if there are no donors in the local database, then no potential match could be found.
			// 2. the transaction should never be null but to reduce unexpected side effects, the test is performed
			if (null == transaction)
				return null;

			// Check to see if a previous match between and AG donor and the local donor database has been made.
			// If it has, we can immeditely return the donot ID for use.
			if (di.Data.AGDonorMap.ContainsKey(transaction.DonorHash))
			{
				// null comment assures it is treated as an exact match
				comment = null;
				return di.Data.AGDonorMap[transaction.DonorHash].DonorId;
			}

			comment = null;
			// If no mapping exists, then loop through the entire donor database, looking for close matches.
			// If a close match is found, it is returned for examination by the operator.
			foreach (var donor in di.Data.DonorList)
			{
				if (Helper.Compare(ref comment, ref partial, donor
					, transaction.LastName
					, transaction.FirstName
					, transaction.Address
					, false
					, transaction.Address2
					, transaction.City
					, transaction.State
					, transaction.Zip
					, ro_firstNameSplitters))
				{
					return donor.Id;
				}
			}

			return null;
		}

		/// <summary>
		/// This private function loops through the records in the Adventist Giving (AG) *.csv, searching
		/// for a donor match in the the local database. If an exact match is not found, the function
		/// exits, leaving the AGDonorResolutionView UserControl displaying the proposed donor match
		/// or none if a close match could not be found. If no match was found the operator will need to
		/// click on the 'Browse for Donor' button to select the target donor. To continue on,
		/// the operator will click the 'Continue...' button to create the mapping and proceed to the
		/// next AG category.
		/// </summary>
		/// <param name="start">The parameter is the index corresponding to the AG record to start
		/// resolving from. To start, the value will be zero. After a resolution, upon clicking of the
		/// 'Continue...' button, the loop will resume with the next AG record index number. The current
		/// index is always contained in the _txId member variable.</param>
		private void DonorResolutionLoop(int start)
		{
			int? partialMatch = null;

			for (_txIdx = start; _txIdx < TransactionList.Count; _txIdx++)
			{
				// The private FindDonorId() function is documented in this class. It takes the AG
				// transaction at index _txIdx, and the loops through the local donors, looking for a
				// match or close match.
				var donorId = FindDonorId(TransactionList[_txIdx.Value], ref partialMatch, ref _comment);

				if (null != donorId)
				{
					_lastNameMatchId = null;

					if (null == _comment)
					{
						// full name and address match
						TransactionList[_txIdx.Value].DonorId = donorId;
					}
					else
					{
						// partial match, exit loop and await operator interaction
						Transaction = TransactionList[_txIdx.Value];
						Donor = di.Data.DonorDict[donorId.Value];
						UpdateCopyButtonsVisibility();
						ProgressText = $"Record {_txIdx.Value + 1} of {TransactionList.Count}";
						return;
					}
				}
				else
				{
					// no match was found, so the target donor fields (left side) in the UserControl
					// will be blank and the 'Continue...' button will be disabled.
					Donor = null;
					Transaction = TransactionList[_txIdx.Value];
					_lastNameMatchId = partialMatch;
					UpdateCopyButtonsVisibility();
					ProgressText = $"Record {_txIdx.Value + 1} of {TransactionList.Count}";
					return;
				}
			}

			// completed donor resolution, clear container objects
			Donor = null;
			Transaction = null;

			DonorDiffsVisibility = Visibility.Hidden;
			DonorResolutionComplete = Visibility.Visible;
		}

		/// <summary>
		/// Import task which is called from the Adventist Giving tab to import a batch of
		/// donations through the Adventist Giving system. This is an async function so the
		/// UI thread is not blocked when it is running.
		/// </summary>
		/// <returns></returns>
		public async Task StartNameResolution()
		{
			DonorResolutionComplete = Visibility.Hidden;
			DonorDiffsVisibility = Visibility.Visible;
			_lastNameMatchId = null;

			await Task.Run(() =>
			{
				if (null != TransactionList)
				{
					DonorResolutionLoop(0);
				}
			});
		}

		/// <summary>
		/// This function is called when the user presses a button to continue resolving donors from the
		/// Adventist Giving *.csv.
		/// </summary>
		/// <returns></returns>
		public async Task ContinueDonorResolution()
		{
			await Task.Run(() =>
			{
				// store selected donor id with Adventist Giving AGDonorMapItem record
				if (!di.Data.AGDonorMap.ContainsKey(Transaction.DonorHash))
				{
					// add to dictionary if not there already
					di.Data.AGDonorMap[Transaction.DonorHash] = new AGDonorMapItem()
					{
						AGDonorHash = Transaction.DonorHash,
						AGLastName = Transaction.LastName,
						AGFirstName = Transaction.FirstName,
						AGAddress = Transaction.Address,
						AGCity = Transaction.City,
						AGState = Transaction.State,
						AGZip = Transaction.Zip,
						DonorId = Donor.Id,
					};
					di.Data.AGDonorMapList.Add(di.Data.AGDonorMap[Transaction.DonorHash]);
				}
				di.Data.AGDonorMap[Transaction.DonorHash].DonorId = Donor.Id;

				_txIdx++;

				if (null != TransactionList)
				{
					DonorResolutionLoop(_txIdx.Value);
				}
			});
		}

		/// <summary>
		/// This function is called when any member or property that would affect the visbility of the Copy buttons is changed.
		/// </summary>
		private void UpdateCopyButtonsVisibility()
		{
			OnPropertyChanged(nameof(LastNameCopyVisible));
			OnPropertyChanged(nameof(FirstNameCopyVisible));
			OnPropertyChanged(nameof(AddressCopyVisible));
			OnPropertyChanged(nameof(Address2CopyVisible));
			OnPropertyChanged(nameof(CityCopyVisible));
			OnPropertyChanged(nameof(StateCopyVisible));
			OnPropertyChanged(nameof(ZipCopyVisible));
			OnPropertyChanged(nameof(CopyAllVisible));
		}

		/// <summary>
		/// This function is called from the View class if/after a person from the donor database is chosen as a match for the
		/// current Adventist Giving record. If a matching donor is not found, the operator would first have to go into donor
		/// maintenance and the donor, after which they will be able to select a matching donor.
		/// </summary>
		/// <param name="donor">The parameter will contain the Donor chosen by the operator to be the target of the AG transaction.</param>
		public void ChooseDonor(Donor donor)
		{
			Donor = donor;
			UpdateCopyButtonsVisibility();
		}

		/// <summary>
		/// Copy all name/address fields from the Adventist Giving transaction field to the matching record in the donor database.
		/// Then continue with the donor resolution in the remaining Adventist Giving records.
		/// </summary>
		/// <returns></returns>
		public async Task UpdateDonor(bool createMapEntry = true)
		{
			Donor.LastName = Transaction.LastName;
			Donor.FirstName = Transaction.FirstName;
			Donor.Address = Transaction.Address;
			Donor.Address2 = Transaction.Address2;
			Donor.City = Transaction.City;
			Donor.State = Transaction.State;
			Donor.Zip = Transaction.Zip;

			if (createMapEntry)
				await ContinueDonorResolution();
			else
			{
				_txIdx++;

				if (null != TransactionList)
				{
					DonorResolutionLoop(_txIdx.Value);
				}
			}
		}

		/// <summary>
		/// Copy the LastName field from the Adventist Giving transaction field to the matching record in the donor database.
		/// </summary>
		public void CopyLastName()
		{
			if (null != Donor)
			{
				Donor.LastName = Transaction?.LastName;
				OnPropertyChanged(nameof(Donor));
				UpdateCopyButtonsVisibility();
			}
		}

		/// <summary>
		/// Copy the FirstName field from the Adventist Giving transaction field to the matching record in the donor database.
		/// </summary>
		public void CopyFirstName()
		{
			if (null != Donor)
			{
				Donor.FirstName = Transaction?.FirstName;
				OnPropertyChanged(nameof(Donor));
				UpdateCopyButtonsVisibility();
			}
		}

		/// <summary>
		/// Copy the Address field from the Adventist Giving transaction field to the matching record in the donor database.
		/// </summary>
		public void CopyAddress()
		{
			if (null != Donor)
			{
				Donor.Address = Transaction?.Address;
				OnPropertyChanged(nameof(Donor));
				UpdateCopyButtonsVisibility();
			}
		}

		/// <summary>
		/// Copy the Address2 field from the Adventist Giving transaction field to the matching record in the donor database.
		/// </summary>
		public void CopyAddress2()
		{
			if (null != Donor)
			{
				Donor.Address2 = Transaction?.Address2;
				OnPropertyChanged(nameof(Donor));
				UpdateCopyButtonsVisibility();
			}
		}

		/// <summary>
		/// Copy the State field from the Adventist Giving transaction field to the matching record in the donor database.
		/// </summary>
		public void CopyCity()
		{
			if (null != Donor)
			{
				Donor.City = Transaction?.City;
				OnPropertyChanged(nameof(Donor));
				UpdateCopyButtonsVisibility();
			}
		}

		/// <summary>
		/// Copy the State field from the Adventist Giving transaction field to the matching record in the donor database.
		/// </summary>
		public void CopyState()
		{
			if (null != Donor)
			{
				Donor.State = Transaction?.State;
				OnPropertyChanged(nameof(Donor));
				UpdateCopyButtonsVisibility();
			}
		}

		/// <summary>
		/// Copy the Zip field from the Adventist Giving transaction field to the matching record in the donor database.
		/// </summary>
		public void CopyZip()
		{
			if (null != Donor)
			{
				Donor.Zip = Transaction?.Zip;
				OnPropertyChanged(nameof(Donor));
				UpdateCopyButtonsVisibility();
			}
		}

		/// <summary>
		/// Copy all name/address fields from the Adventist Giving transaction field to the matching record in the donor database.
		/// </summary>
		public void CopyAll()
		{
			if (null == Donor)
			{
				// create a new donor
				Donor = new Donor();
				Donor.Id = ((0 < di.Data.DonorList.Count) ? di.Data.DonorList.Max(x => x.Id) : 0) + 1;
				di.Data.DonorList.Add(Donor);
				di.Data.DonorDict[Donor.Id] = Donor;
			}

			Transaction.DonorId = Donor.Id;
			UpdateDonor(false);
		}
	}
}
