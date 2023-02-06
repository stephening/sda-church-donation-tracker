using CommunityToolkit.Mvvm.Input;
using Donations.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Donations.ViewModel
{
	/// <summary>
	/// This view model handles the functionality and the binding to the AGDonationSummaryView.xaml which is a
	/// UserControl under the 'Verify and submit' tab.
	/// 
	/// Once the donor and category matching is done, the this view will show a list of categories with their
	/// subtotals. This view should be able to cross checked with the category sub totals from the Adventist
	/// Giving (AG) report.
	/// </summary>
	public class AGDonationSummaryViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObservableCollection<AdventistGiving>? TransactionList => di.AG?.TransactionList;

		public ObservableCollection<CategorySum>? CategorySums { get; set; } = new ObservableCollection<CategorySum>();
		public CollectionViewSource CategorySumSource { get; set; } = new CollectionViewSource();

		/// <summary>
		/// The SubmitBatchCmd property is bound to the 'Submit batch' button. This button will
		/// submit the AG report by creating and adding donation records for each record in the
		/// AG report, each with the same Batch id, which will group them as a batch. This
		/// program will let you submit even if the target total and the actual total don't
		/// match up, but the mismatched amounts will show up in red on the batch browser view.
		/// </summary>
		public ICommand SubmitBatchCmd { get; }

		private double _total;
		/// <summary>
		/// The Total property is bound to the light green background TextBox, showing the
		/// actual (computed) total. This property will be saved in a column of the batch.
		/// </summary>
		public double Total
		{
			get { return _total; }
			set
			{
				_total = value;
				OnPropertyChanged();
			}
		}

		private DateOnly _batchDate;
		/// <summary>
		/// The BatchDate is bound to the batch DatePicker to assign a single batch date
		/// which will show in the batch browser view. This is a single date for the entire
		/// batch as opposed to the dates for each individual transaction.
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
			}
		}

		/// <summary>
		/// The SubmitEnabled property will control whether the button 'Submit batch' button
		/// enabled or disabled. The state is based on the date being set and a non-zero sum.
		/// </summary>
		public bool SubmitEnabled => (!string.IsNullOrEmpty(BatchDate) && 0 < CategorySums?.Count);

		private Dictionary<string, CategorySum> _categorySumDict = new Dictionary<string, CategorySum>();
		private ObservableCollection<Donation>? _donationList = null;

		/// <summary>
		/// The constructor sets the CollectionViewSource for the category sums DataGrid. It
		/// also initializes the SubmitBatchCmd to the handler for batch submitting. And it
		/// makes sure the BatchDate is clear so it will have to be specified.
		/// </summary>
		public AGDonationSummaryViewModel()
		{
			di.DonationSummary = this;

			CategorySumSource.Source = CategorySums;

			BatchDate = null;

			SubmitBatchCmd = new AsyncRelayCommand(SubmitBatch);
		}

		/// <summary>
		/// This method is used to return the local category code which matched already,
		/// or comes from the category mapping performed in an earlier step. If both of
		/// those options failed (which they shouldn't), an exception is thrown.
		/// </summary>
		/// <param name="agCatId"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private int MapCategory(int agCatId)
		{
			if (true == di.Data.CatDict?.ContainsKey(agCatId))
			{
				return agCatId;
			}
			else if (null != di.Data.AGCategoryMap && di.Data.AGCategoryMap.ContainsKey(agCatId))
			{
				return di.Data.AGCategoryMap[agCatId].CategoryCode;
			}
			else
			{
				throw new Exception("Unable to resolove Adventist Giving category Id");
			}
		}

		/// <summary>
		/// This method is mapped to the SubmitBatchCmd Command, and it where the new
		/// Donation records and the Batch record will be created and added to the top
		/// level lists. Switching to the 'Batch browser' tab should immediately show
		/// the new batch, assuming the batch date is covered by the date filters.
		/// </summary>
		private async Task SubmitBatch()
		{
			var batch = new Batch()
			{
				Id = (0 < di.Data.BatchList.Count) ? di.Data.BatchList.Max(x => x.Id) + 1 : 1,
				Source = enumSource.AdventistGiving,
				Date = DateOnly.Parse(BatchDate).ToString("yyyy/MM/dd"),
				Total = di.AG.TargetTotal,
				ActualTotal = Total,
				Operator = di.Username,
				Note = di.AG.BatchNote,
			};

			di.Data.BatchList.Add(batch);

			foreach (var donation in _donationList)
			{
				donation.BatchId = batch.Id;
				di.Data.DonationList.Add(donation);
				di.Data.DonationDict[donation.Id] = donation;
			}

			_donationList = null;
			Total = 0;
			BatchDate = null;
			CategorySums?.Clear();
			_categorySumDict.Clear();
			TransactionList?.Clear();
			di.AG.Reset(); // this sets the visual state of the donor/category resolution tabs
			OnPropertyChanged(nameof(SubmitEnabled));

			di.Data.SaveData();
		}

		/// <summary>
		/// This method is called from the Loaded() method if a new AG csv is loaded.
		/// </summary>
		private void ComputeSum()
		{
			Total = 0;
			CategorySums.Clear();
			_categorySumDict.Clear();

			foreach (var donation in _donationList)
			{
				Total += donation.Value;

				if (_categorySumDict.ContainsKey(donation.Category))
				{
					_categorySumDict[donation.Category].Sum += donation.Value;
				}
				else
				{
					CategorySum sum = new CategorySum()
					{
						Category = donation.Category,
						Sum = donation.Value
					};
					CategorySums.Add(sum);
					_categorySumDict[sum.Category] = sum;
				}
			}

			CategorySumSource.View.Refresh();
			OnPropertyChanged(nameof(SubmitEnabled));
		}

		/// <summary>
		/// This method is called if a new AG csv is loaded and the sums need to be updated.
		/// </summary>
		/// <returns></returns>
		public async Task Loaded()
		{
			if (di.DonorResolution.DonorResolutionComplete == Visibility.Visible
				&& di.CategoryResolution.CategoryResolutionComplete == Visibility.Visible)
			{
				_donationList = new ObservableCollection<Donation>();
				Total = 0;
				var donationId = (0 < di.Data.DonationList.Count) ? di.Data.DonationList.Max(x => x.Id) + 1 : 1;

				foreach (var tx in TransactionList)
				{
					Donation donation = new Donation();
					donation.Id = donationId++;
					if (null != tx.DonorId)
						donation.DonorId = tx.DonorId.Value;
					else if (di.Data.AGDonorMap.ContainsKey(tx.DonorHash))
						donation.DonorId = di.Data.AGDonorMap[tx.DonorHash].DonorId;
					else
						throw new Exception($"This shouldn't happen, but there was no donor id or map for this transaction {tx.FirstName} {tx.LastName} TransactionId: {tx.TransactionId} Category: {tx.CategoryName}, Amount: {tx.Amount}");

					string lastName = tx.LastName;
					string firstName = tx.FirstName;
					if (di.Data.DonorDict.ContainsKey(donation.DonorId))
					{
						lastName = di.Data.DonorDict[donation.DonorId].LastName;
						firstName = di.Data.DonorDict[donation.DonorId].FirstName;
					}
					donation.LastName = lastName;
					donation.FirstName = firstName;
					donation.Category = $"{MapCategory(tx.CategoryCode)} {di.Data.CatDict[MapCategory(tx.CategoryCode)].Description}";
					donation.Value = tx.Amount;
					donation.Date = DateOnly.Parse(tx.TransactionDate).ToString("yyyy/MM/dd");
					donation.Method = enumMethod.AdventistGiving;
					donation.TransactionNumber = tx.TransactionId;

					_donationList.Add(donation);
				}
				ComputeSum();
			}
		}
	}
}
