using CommunityToolkit.Mvvm.Input;
using Donations.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Donations.ViewModel
{
	/// <summary>
	/// This view model handles the functionality and the binding to the DonorInputView.xaml. The view associated with
	/// this view model is a UserControl placed on the 'Donor input' tab. This is the second of two ways of entering
	/// donations, Adventist Giving (AG) being the other. Unlike AG, there are not multiple tabs for this view. There
	/// are two halfs to this view, the left side being designed to look like a tithe envelope, with the default
	/// category options that are printed on the envelope. On the right side is the list of donors, as they are entered,
	/// with their totals. The operator may click on any donor in the donor summary view on the right side and it will
	/// re-populate the left side with the individual contributions, which can be reviewed or edited and then
	/// resubmitted. Once all donations from the given week are entered, hopefully the running total will equal the
	/// batch total. At that point, the batch can be submitted which will create new Donation record for each of the
	/// individual category contributions, assigning the same batch id to all of them so they will be grouped in a
	/// single batch.
	/// </summary>
	public class DonorInputViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public double TotalSum { get; set; }
		private double _batchTotal;

		/// <summary>
		/// The BatchTotal property is bound to the light green TextBox where the operator specifies the
		/// target batch total.
		/// </summary>
		public double BatchTotal
		{
			get { return _batchTotal; }
			set
			{
				_batchTotal = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SubmitEnabled));
			}
		}

		/// <summary>
		/// The RunningTotal property is also bound to a TextBox, but this one is not editable
		/// and is automatically set from the total of all donor contributions.
		/// </summary>
		public double RunningTotal
		{
			get
			{
				double sum = 0;
				foreach (var item in SummaryList)
				{
					sum += item.Subtotal;
				}
				return sum;
			}
		}

		/// <summary>
		/// The RunningTotalColor property is also bound to the RunningTotal TextBox. When the
		/// value equals the BatchTotal, the background color of the TextBox will be light green.
		/// When they don't match, it will be light pink.
		/// </summary>
		public Brush RunningTotalColor => Math.Round(RunningTotal, 2) == Math.Round(BatchTotal, 2) ? Brushes.LightGreen : Brushes.LightPink;

		public ObservableCollection<Donation>? IndividualDonations = new ObservableCollection<Donation>();
		public CollectionViewSource EnvelopeViewSource { get; set; } = new CollectionViewSource();
		public ObservableCollection<Summary>? SummaryList = new ObservableCollection<Summary>();
		public Dictionary<string, Summary>? SummaryDict = new Dictionary<string, Summary>();
		public CollectionViewSource SummaryViewSource { get; set; } = new CollectionViewSource();

		private bool _canAddRows = false;
		/// <summary>
		/// The CanAddRows property is set to false by default. It remains false during a donor input
		/// batch session. That means that additional rows for more categories cannot be added.
		/// However, any existing row/category that is not used can be changed to accomodate targetted
		/// donations to categories other than are listed by default. In review mode, only categories
		/// donated to are shown, so if an additional category is needed, a row will need to be added,
		/// hence, CanAddRows will be set to true in Review mode.
		/// </summary>
		public bool CanAddRows
		{
			get { return _canAddRows; }
			set
			{
				_canAddRows = value;
				OnPropertyChanged();
			}
		}

		private DateOnly _batchDate;
		/// <summary>
		/// The BatchDate is bound to the batch DatePicker to assign a single batch date
		/// which will show in the batch browser view. This is a single date for the entire
		/// batch regardless of any possible dates that might be written on the tithe
		/// envelope, which will be ignored.
		/// </summary>
		public string BatchDate
		{
			get
			{
				string str = _batchDate.ToString("yyyy/MM/dd");
				return str.Equals("0001/01/01") ? "" : str;
			}
			set
			{
				try
				{
					if (string.IsNullOrEmpty(value))
						_batchDate = DateOnly.MinValue;
					else
						_batchDate = DateOnly.Parse(value);
				}
				catch
				{
					_batchDate = DateOnly.MinValue;
				}
				OnPropertyChanged();
				OnPropertyChanged(nameof(SubmitEnabled));
				OnPropertyChanged(nameof(SubmitBatchEnabled));
			}
		}

		private string _batchNote;
		/// <summary>
		/// The BatchNode property gets transferred to the Note member and will show up in a
		/// column in the batch browser.
		/// </summary>
		public string BatchNote
		{
			get { return _batchNote; }
			set
			{
				_batchNote = value;
				HasChanges = true;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SubmitBatchEnabled));
			}
		}

		private enumMethod enumMethod;
		/// <summary>
		/// The MethodOptions property is bound to the Cash, Check radio buttons, through the 
		/// IValueConverter, implemented as EnumConverter. Inspite of the fact that there
		/// are methods other than Cash and Check represented in enumMethod, this page only
		/// presents those two. If more are needed, additional radio buttons could be added
		/// without much difficulty.
		/// </summary>
		public enumMethod MethodOptions
		{
			get { return enumMethod; }
			set
			{
				enumMethod = value;
				if (value == enumMethod.Cash)
					CheckNumber = "";
				OnPropertyChanged(nameof(CheckNumberEnabled));
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The SubmitEnabled property controls the enable/disable state of the individual submit button.
		/// In order to be enabled, it reuires a non-zero sum, a donor name, a BatchDate, and a method or
		/// CheckNumber, if method is Check or AdventistGiving.
		/// </summary>
		public bool SubmitEnabled => (
			!string.IsNullOrEmpty(Name) 
			&& (0 < TotalSum) 
			&& !string.IsNullOrEmpty(BatchDate) 
			&& (MethodOptions == enumMethod.Cash || !string.IsNullOrEmpty(CheckNumber) || MethodOptions == enumMethod.AdventistGiving)
			);

		private bool _hasChanges;
		/// <summary>
		/// The HasChanges property keeps track of the changes to the batch in order to enable/disable
		/// the 'Submit batch' button. This property actually feeds into the property that enables or
		/// disables the 'Submit batch' button.
		/// </summary>
		public bool HasChanges
		{
			get { return _hasChanges; }
			set
			{
				_hasChanges = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SubmitBatchEnabled));
			}
		}

		/// <summary>
		/// The SubmitBatchEnabled property enables/disables the 'Submit batch' button.
		/// </summary>
		public bool SubmitBatchEnabled => !string.IsNullOrEmpty(BatchDate) && HasChanges;

		/// <summary>
		/// The ResetEnabled property enables/disables the 'Submit batch' button.
		/// </summary>
		public bool ResetEnabled => !_review;

		/// <summary>
		/// The Name property is bound to the Name TextBox field. It constructs a
		/// Last, First value to put in that field. It will ommit the ',' if only one
		/// name is available.
		/// </summary>
		public string? Name => (!string.IsNullOrEmpty(_lastName) && !string.IsNullOrEmpty(_firstName)) ? $"{_lastName}, {_firstName}"
			: ((string.IsNullOrEmpty(_lastName) && string.IsNullOrEmpty(_firstName)) ? ""
				: (!string.IsNullOrEmpty(_lastName) ? _lastName : _firstName));
		
		/// <summary>
		/// The Address property is bound to the ReadOnly Address TextBox. This will be
		/// filled from the _selectorDonor variable if available. Operator cannot type 
		/// in this field. If they wish to enter a new donor, they should do so on the
		/// Maintenance:Donor tab.
		/// </summary>
		public string? Address { get; set; }
		/// <summary>
		/// The City property is bound to the ReadOnly Address TextBox. This will be
		/// filled from the _selectorDonor variable if available. Operator cannot type 
		/// in this field. If they wish to enter a new donor, they should do so on the
		/// Maintenance:Donor tab.
		/// </summary>
		public string? City { get; set; }
		/// <summary>
		/// The State property is bound to the ReadOnly Address TextBox. This will be
		/// filled from the _selectorDonor variable if available. Operator cannot type 
		/// in this field. If they wish to enter a new donor, they should do so on the
		/// Maintenance:Donor tab.
		/// </summary>
		public string? State { get; set; }
		/// <summary>
		/// The Zip property is bound to the ReadOnly Address TextBox. This will be
		/// filled from the _selectorDonor variable if available. Operator cannot type 
		/// in this field. If they wish to enter a new donor, they should do so on the
		/// Maintenance:Donor tab.
		/// </summary>
		public string? Zip { get; set; }

		/// <summary>
		/// The CheckNumberEnabled property allows for the TextBox to be disabled unless
		/// the Check method is selected.
		/// </summary>
		public bool CheckNumberEnabled => MethodOptions == enumMethod.Check && NotAdventistGiving;

		private string? _checkNumber;
		/// <summary>
		/// The CheckNumber property is bound to a TextBox which allows a check number
		/// to be entered.
		/// </summary>
		public string? CheckNumber
		{
			get { return _checkNumber; }
			set
			{
				_checkNumber = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SubmitEnabled));
			}
		}

		/// <summary>
		/// The NotAdventistGiving property will control the enable/disable state of the
		/// Cash/Check radio buttons. If the donation is from Adventist Giving, those
		/// radio's will be disabled.
		/// </summary>
		public bool NotAdventistGiving => MethodOptions != enumMethod.AdventistGiving;

		/// <summary>
		/// The DeleteRowCmd property is bound to the 'Delete row' context menu option. It allows
		/// for the row that was right-clicked on to be deleted.
		/// </summary>
		public ICommand DeleteRowCmd { get; }
		/// <summary>
		/// The SubmitBatchCmd property is bound to the 'Submit batch' button. This button will
		/// submit the weekly donation entry batch by creating and adding donation records for
		/// each record contribution entered, each with the same Batch id, which will group
		/// them as a batch. This program will let you submit even if the BatchTotal and the
		/// RunningTotal don't match up, but the mismatched amounts will show up in red on the
		/// batch browser view.
		/// </summary>
		public ICommand SubmitBatchCmd { get; }

		// private member variables
		private ObservableCollection<Donation> _batchDonations;
		private Donor? _selectedName;
		private string? _lastName;
		private string? _firstName;
		private int _selectedSummaryIndex = -1;
		private int _summaryDonorId = 1;
		private bool _review = false;
		private Batch? _batch;
		private bool _donorChanges = false;

		/// <summary>
		/// This constructor calls SubmitDonor() to initialize donor name/address fields. It
		/// also initializes the SubmitBatchCmd to the handler for batch submitting. And it
		/// makes sure the BatchDate is clear so it will have to be specified. It also sets
		/// the Source property of the CollectionViewSource for the donations and the summary.
		/// </summary>
		public DonorInputViewModel()
		{
			if (null == SummaryDict)
			{
				throw new InsufficientMemoryException("SummaryDict is null");
			}

			if (null == SummaryList)
			{
				throw new InsufficientMemoryException("SummaryList is null");
			}

			if (null == IndividualDonations)
			{
				throw new InsufficientMemoryException("IndividualDonations is null");
			}

			SubmitDonor();

			DeleteRowCmd = new RelayCommand(DeleteRow);
			SubmitBatchCmd = new AsyncRelayCommand(SubmitBatch);

			HasChanges = false;

			if (di.FileSystem.File.Exists("summarylist.xml") && di.FileSystem.File.Exists("date_total.txt"))
			{
				try
				{
					HasChanges = true;

					SummaryList = di.Data.DeserializeXml<Summary>("summarylist.xml");

					using var reader = di.FileSystem.File.OpenText("date_total.txt");

					BatchDate = reader.ReadLine();
					BatchTotal = double.Parse(reader.ReadLine());
				}
				catch
				{

				}
			}

			EnvelopeViewSource.Source = IndividualDonations;
			SummaryViewSource.Source = SummaryList;

		}

		/// <summary>
		/// This method is called in response to the a click on the 'Reset ...' button.
		/// It will delete the batch persistence files and reset the page.
		public void Reset()
		{
			di.FileSystem.File.Delete("summarylist.xml");
			di.FileSystem.File.Delete("date_total.txt");

			IndividualDonations.Clear();
			SummaryList.Clear();

			BatchDate = "";
			BatchNote = "";
			BatchTotal = 0;
			OnPropertyChanged(nameof(RunningTotal));
			OnPropertyChanged(nameof(ResetEnabled));

			HasChanges = false;

			Loaded();
		}

		/// <summary>
		/// This method is called in response to the right-click context menu:'Delete row'.
		/// This is the method bound to the Command, and it will delete the row that was 
		/// right-clicked on to get the context menu.
		/// </summary>
		public void DeleteRow()
		{
			Summary? summary = EnvelopeViewSource.View.CurrentItem as Summary;
			if (null == summary) return;

			HasChanges = true;
			SummaryList?.Remove(summary);
		}

		/// <summary>
		/// This method is mapped to the SubmitBatchCmd Command, and it where the new
		/// Donation records and the Batch record will be created and added to the top
		/// level lists. Switching to the 'Batch browser' tab should immediately show
		/// the new batch, assuming the batch date is covered by the date filters.
		/// </summary>
		private async Task SubmitBatch()
		{
			var donationId = (0 < di.Data.DonationList.Count) ? di.Data.DonationList.Max(x => x.Id) + 1 : 1;

			if (!_review)
			{
				var batch = new Batch()
				{
					Id = (0 < di.Data.BatchList.Count) ? di.Data.BatchList.Max(x => x.Id) + 1 : 1,
					Source = enumSource.DonorInput,
					Date = BatchDate,
					Total = BatchTotal,
					ActualTotal = RunningTotal,
					Operator = di.Username,
					Note = BatchNote,
				};

				di.Data.BatchList.Add(batch);

				foreach (var donor in SummaryList)
				{
					foreach (var item in donor.ItemizedDonations)
					{
						if (0 < item.Value)
						{
							item.Id = donationId++;
							item.DonorId = donor.DonorId;
							item.BatchId = batch.Id;
							item.LastName = donor.LastName;
							item.FirstName = donor.FirstName;
							item.Date = BatchDate;
							item.Method = donor.Method;
							item.TransactionNumber = donor.TransactionNumber;

							di.Data.DonationList.Add(item);
						}
					}
				}

				_selectedName = null;
				SummaryList?.Clear();
				SummaryDict?.Clear();
				IndividualDonations?.Clear();
				BatchTotal = 0;
				BatchNote = null;
				BatchDate = "";
				OnPropertyChanged(nameof(SubmitEnabled));
				SubmitDonor();
			}
			else
			{
				// First remove all donations for this batch from the donation database
				for (int i =  di.Data.DonationList.Count - 1; i >= 0; i--)
				{
					if (di.Data.DonationList[i].BatchId == _batch.Id)
					{
						di.Data.DonationDict.Remove(di.Data.DonationList[i].Id);
						di.Data.DonationList.Remove(di.Data.DonationList[i]);
					}
				}

				donationId = (0 < di.Data.DonationList.Count) ? di.Data.DonationList.Max(x => x.Id) + 1 : 1;

				// update existing batch
				foreach (var donor in SummaryList)
				{
					foreach (var item in donor.ItemizedDonations)
					{
						if (0 < item.Value)
						{
							item.Id = donationId++;
							item.DonorId = donor.DonorId;
							item.BatchId = _batch.Id;
							item.Date = DateOnly.Parse(BatchDate).ToString("yyyy/MM/dd");
							item.Method = donor.Method;
							item.TransactionNumber = donor.TransactionNumber;

							di.Data.DonationList.Add(item);
							di.Data.DonationDict[item.Id] = item;
						}
					}
				}

				_batch.Total = BatchTotal;
				_batch.Note = BatchNote;
				_batch.ActualTotal = RunningTotal;
			}

			_summaryDonorId = 1;
			HasChanges = false;

			di.Data.SaveData();
			di.FileSystem.File.Delete("summarylist.xml");
			di.FileSystem.File.Delete("date_total.txt");
		}

		/// <summary>
		/// This method is called when the UserControl needs to be in review/edit mode rather than
		/// new data entry mode.
		/// </summary>
		/// <param name="batch"></param>
		public void Review(Batch? batch, ObservableCollection<Donation>? batchDonations)
		{
			if (null == batch)
			{
				throw new ArgumentNullException("batch is null");
			}

			if (null == batchDonations)
			{
				throw new ArgumentNullException("batchDonations is null");
			}

			_review = true;
			_batch = batch;
			BatchTotal = batch.Total;
			BatchDate = (null != batch.Date) ? batch.Date : "";
			BatchNote = (null != batch.Note) ? batch.Note : "";
			_batchDonations = batchDonations;
			CanAddRows = true;
			SummaryList?.Clear();
			SummaryDict?.Clear();
			_summaryDonorId = 1;

			Loaded(); // load envelope design

			foreach (var donation in _batchDonations)
			{
				Donation lineItem = donation.Copy();

				var key = (donation.Method == enumMethod.AdventistGiving) ? donation.TransactionNumber : donation.DonorId.ToString();

				if (SummaryDict.ContainsKey(key))
				{
					// use DonorId to collate donations under same donor
					SummaryDict[key].ItemizedDonations.Add(lineItem);
					SummaryDict[key].Subtotal += donation.Value;
				}
				else
				{
					var summaryEntry = new Summary()
					{
						ItemizedDonations = new ObservableCollection<Donation>() { lineItem },
						DonorId = donation.DonorId,
						Subtotal = donation.Value,
						Method = donation.Method,
						TransactionNumber = donation.TransactionNumber
					};

					if (di.Data.DonorDict.ContainsKey(donation.DonorId))
					{
						summaryEntry.LastName = di.Data.DonorDict[donation.DonorId].LastName;
						summaryEntry.FirstName = di.Data.DonorDict[donation.DonorId].FirstName;
					}
					else
					{
						// no donor matching DonorId
						summaryEntry.LastName = donation.LastName;
						summaryEntry.FirstName = donation.FirstName;
					}

					SummaryList.Add(summaryEntry);
					SummaryDict[key] = summaryEntry;
				}
			}

			OnPropertyChanged(nameof(RunningTotal));
			OnPropertyChanged(nameof(RunningTotalColor));
			OnPropertyChanged(nameof(ResetEnabled));
			SummaryViewSource.View?.Refresh();
		}

		/// <summary>
		/// This method is called from the TextChanged handler for the CheckNumber TextBox. The reason
		/// this handler was added was because if the check number is that last thing being entered,
		/// the Submit button was not getting enabled until the focus left the CheckNumber field. This
		/// way, if a check number is needed, it's presence will be felt immediately as typing begins
		/// in the field.
		/// </summary>
		/// <param name="checkNumber"></param>
		public void CheckNumberChanged(string checkNumber)
		{
			CheckNumber = checkNumber;
			_donorChanges = true;
		}

		/// <summary>
		/// This method is called when the Check radio button event is fired. All it does is to enable
		/// the CheckNumber field. The caller of this method, DonorInputView.CheckOptionSelected is
		/// bound to the Check radio button's Checked property. After calling this method to enable
		/// the CheckNumber field, it also sets the focus on the CheckNumber field, directly allowing
		/// entry to that field without having to select or tab to it.
		/// </summary>
		public void UpdateSubmitEnabled()
		{
			OnPropertyChanged(nameof(SubmitEnabled));
		}

		/// <summary>
		/// This method is called through the Command binding to the 'Submit' button. This function
		/// takes the itemized donations and temporarily stores them in the Summary object.
		/// </summary>
		public void SubmitDonor()
		{
			if (SubmitEnabled)
			{
				var summaryEntry = new Summary()
				{
					ItemizedDonations = IndividualDonations,
					LastName = _lastName,
					FirstName = _firstName,
					DonorId = (null != _selectedName) ? _selectedName.Id : _summaryDonorId++,
					Subtotal = TotalSum,
					Method = MethodOptions,
					TransactionNumber = CheckNumber
				};

				_selectedName = null;
				IndividualDonations = new ObservableCollection<Donation>();
				if (null == IndividualDonations)
				{
					throw new InsufficientMemoryException("IndividualDonations = new ObservableCollection<Donation>() returned null");
				}

				if (0 > _selectedSummaryIndex)
				{
					SummaryList?.Add(summaryEntry);
				}
				else
				{
					SummaryList[_selectedSummaryIndex] = summaryEntry;
				}

				SummaryViewSource.View?.Refresh();
			}

			_selectedName = null;
			_selectedSummaryIndex = -1;
			NameSelectionChanged(null);
			IndividualDonations = new ObservableCollection<Donation>();
			if (null == IndividualDonations)
			{
				throw new InsufficientMemoryException("IndividualDonations = new ObservableCollection<Donation>() returned null");
			}
			EnvelopeViewSource.Source = IndividualDonations;
			MethodOptions = enumMethod.Cash;
			CheckNumber = "";
			TotalSum = 0;

			Loaded();

			OnPropertyChanged(nameof(SubmitBatchEnabled));
			OnPropertyChanged(nameof(RunningTotal));
			OnPropertyChanged(nameof(RunningTotalColor));
			OnPropertyChanged(nameof(TotalSum));

			HasChanges = true;

			if (0 < SummaryList.Count)
			{
				try
				{
					di.Data.SerializeXml<Summary>("summarylist.xml", SummaryList);
					using var writer = di.FileSystem.File.CreateText("date_total.txt");

					writer.WriteLine(BatchDate);
					writer.WriteLine(BatchTotal.ToString());
				}
				catch { }
			}
		}

		/// <summary>
		/// This method is called when the 'Donor input' tab is selected. This allow it to update itself is the
		/// tithe envelope design changed.
		/// </summary>
		public void Loaded()
		{
			int i = di.Data.TitheEnvelopeDesign.Count - 1;

			IndividualDonations?.Clear();

			for (; i >= 0 && string.IsNullOrEmpty(di.Data.TitheEnvelopeDesign[i].Category); i--)
			{
				// skip over empty rows from the design
			}
			for (; i >= 0; i--)
			{
				// start adding from the last non empty entry, continue inserting at zero so the order is retained
				IndividualDonations?.Insert(0, new Donation() { Category = di.Data.TitheEnvelopeDesign[i].Category });
			}
		}

		/// <summary>
		/// This method is called when a value changes, so that the sum, TotalSum property, can be computed and
		/// updated.
		/// </summary>
		public void ValueChanged()
		{
			TotalSum = 0;
			_donorChanges = true;
			foreach (var item in IndividualDonations)
			{
				TotalSum += item.Value;
			}
			OnPropertyChanged(nameof(TotalSum));
			OnPropertyChanged(nameof(SubmitEnabled));
		}

		/// <summary>
		/// This method is called whenever the donor changes. The donor is stored in the Donor object, and
		/// fields from that object are copied to the name/address fields.
		/// </summary>
		/// <param name="SelectedName">Donor object just selected from the DonorSelectionView modal dialog.</param>
		public void NameSelectionChanged(Donor? SelectedName)
		{
			if (null == SelectedName)
			{
				_lastName = "";
				_firstName = "";
				Address = "";
				City = "";
				State = "";
				Zip = "";
			}
			else
			{
				_lastName = SelectedName.LastName;
				_firstName = SelectedName.FirstName;
				Address = SelectedName.Address;
				City = SelectedName.City;
				State = SelectedName.State;
				Zip = SelectedName.Zip;
			}

			_donorChanges = true;
			OnPropertyChanged(nameof(Name));
			OnPropertyChanged(nameof(Address));
			OnPropertyChanged(nameof(City));
			OnPropertyChanged(nameof(State));
			OnPropertyChanged(nameof(Zip));
		}

		public int ChangeCategory(Donation row, Category? cat)
		{
			if (null == cat)
				row.Category = "";
			else
				row.Category = $"{cat.Code} {cat.Description}";

			EnvelopeViewSource.View.Refresh();

			_donorChanges = true;

			return IndividualDonations.IndexOf(row);
		}

		/// <summary>
		/// This method is called when a new donor is selected. This function will take care of changing
		/// the name/address fields to reflect the new donor.
		/// </summary>
		/// <param name="donorId"></param>
		public void ChooseDonor(int donorId)
		{
			if (true != di.Data.DonorDict?.ContainsKey(donorId))
			{
				throw new Exception($"DonorId = {donorId} not found");
			}

			_selectedName = di.Data.DonorDict[donorId];
			_donorChanges = true;
			NameSelectionChanged(_selectedName);
		}

		/// <summary>
		/// This method is called when the operator clicks on a name in the summary view. This allows
		/// for review or editing of the details of the donor contributions. This function needs to
		/// keep track of changes, so that it will know if selecting a donor from the summary list
		/// will overwrite current donor contributions that have not been submitted yet.
		/// </summary>
		/// <param name="index">This parameter is the index in the list of the item that was clicked
		/// on.</param>
		/// <param name="force">If the overwrite condition exists (there are change that have not
		/// been submitted) and the force parameter is false, the summary action will not take place.
		/// In stead, a message to that effect will be returned, which will be displayed in a
		/// MessageBox.</param>
		/// <returns>If successful, the return value will be null. If not, a message will be returned
		/// that should be displayed to the operator in a MessageBox.</returns>
		public string? SummarySelectionChanged(int index, bool force = false)
		{
			if (!force && _donorChanges && (null != _selectedName || 0 != TotalSum))
			{
				_selectedSummaryIndex = -1;
				return $"The selection in the summary box will overwrite the current entry that has not been submitted, do you wish to continue?";
			}

			if (index < SummaryList.Count)
			{
				_selectedSummaryIndex = index;

				var item = SummaryList[index];

				if (null != item)
				{
					// clear name to reset donor filter
					_lastName = "";
					_firstName = "";
					var id = item.DonorId;
					if (di.Data.DonorDict.ContainsKey(id))
					{
						_selectedName = di.Data.DonorDict[id];
					}
					else
					{
						_selectedName = new Donor()
						{
							Id = id,
							LastName = item.LastName,
							FirstName = item.FirstName,
						};
					}
					NameSelectionChanged(_selectedName);
					IndividualDonations = item.ItemizedDonations;
					TotalSum = item.Subtotal;
					EnvelopeViewSource.Source = IndividualDonations;
					MethodOptions = item.Method;
					CheckNumber = item?.TransactionNumber;
					_donorChanges = false;

					OnPropertyChanged(nameof(_selectedName));
					OnPropertyChanged(nameof(TotalSum));
					OnPropertyChanged(nameof(SubmitEnabled));
					OnPropertyChanged(nameof(MethodOptions));
					OnPropertyChanged(nameof(NotAdventistGiving));
					OnPropertyChanged(nameof(CheckNumberEnabled));
				}
			}
			return null;
		}
	}
}
