using CommunityToolkit.Mvvm.Input;
using Donations.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace Donations.ViewModel
{
	/// <summary>
	/// This view model handles the functionality and the binding to the BatchReviewView.xaml which is a
	/// Window, which will be used as a modal dialog when a batch is double clicked on from the
	/// BatchBrowserView.
	/// 
	/// The BatchReviewView actually has three tabs which show the batch in two different ways.
	/// 
	///   1. By category view which shows all categories and their sub totals in a DataGrid. There is no
	///      editing possible on this view.
	///   2. By donor, which uses the DonorInputView UserControl. There is editing possible in this view.
	///   3. Print, which uses the BatchPrintView UserControl.
	/// </summary>
	public class BatchReviewViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObservableCollection<CategorySum>? CategorySums { get; set; } = new ObservableCollection<CategorySum>();
		public CollectionViewSource CategorySumSource { get; set; } = new CollectionViewSource();

		private bool _hasChanges;
		/// <summary>
		/// The HasChanges property tracks the changes of the three fields at the top of this page.
		/// They are date, note and target total dollar amount. The submit button enable state is 
		/// controlled by the HasChanges property.
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

		private DateOnly _batchDate;
		/// <summary>
		/// The BatchDate property is stored in the Batch object and is displayed in a column of
		/// the batch browser.
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
				HasChanges = true;
				OnPropertyChanged();
			}
		}

		private string? _batchNote;

		/// <summary>
		/// The BatchNote property is stored in the Batch object and is displayed in a column of
		/// the batch browser.
		/// </summary>
		public string? BatchNote
		{
			get { return _batchNote; }
			set
			{
				_batchNote = value;
				HasChanges = true;
				OnPropertyChanged();
			}
		}

		private double _batchTotal;

		/// <summary>
		/// The BatchTotal property is stored in the Batch object and is displayed in a column of
		/// the batch browser. It is also compared against the actual sum total and if they don't 
		/// match, they will be colored with a red background in the batch browser view.
		/// </summary>
		public double BatchTotal
		{
			get { return _batchTotal; }
			set
			{
				_batchTotal = value;
				HasChanges = true;
				OnPropertyChanged();
			}
		}

		private Batch? _origBatch;

		/// <summary>
		/// The SubmitBatchCmd property is bound to the 'Submit batch' Command binding. This allows
		/// the blutton click to directly invoke the handler in the view model.
		/// </summary>
		public ICommand SubmitBatchCmd { get; }

		private Dictionary<string, CategorySum> _categorySumDict = new Dictionary<string, CategorySum>();
		private ObservableCollection<Donation>? _batchDonations;

		/// <summary>
		/// The constructor initializes the SubmitBatchCmd to the handler for batch submitting. It
		/// also sets the CollectionViewSource for the category summary DataGrid.
		/// </summary>
		public BatchReviewViewModel()
		{
			SubmitBatchCmd = new RelayCommand(SubmitBatch);

			CategorySumSource.Source = CategorySums;
		}

		/// <summary>
		/// This method is called if the BatchReviewView is shown as a result of a double-click
		/// on an entry in the batch browser. It sets up a local DonationList to be used by both tabs,
		/// filted by BatchId.
		/// </summary>
		/// <param name="batch">The batch object that was clicked on in the batch browser. It is actually
		/// a reference, so if changes are made to it, they will be reflected in the top level batch
		/// collection.</param>
		public void Review(Batch? batch, ObservableCollection<Donation>? batchDonations)
		{
			if (null == batch)
			{
				throw new Exception("batch is null");
			}
			if (null == batchDonations)
			{
				throw new Exception("batchDonations is null");
			}

			_batchDonations = batchDonations;

			_origBatch = batch;
			BatchDate = batch.Date;
			BatchNote = batch.Note;
			BatchTotal = batch.Total;

			HasChanges = false;

			ComputeSum();
		}

		/// <summary>
		/// This method is the handler for the SubmitBatchCmd binding. If the batch is submitted,
		/// it updated the three fields from the top of this page to the batch object. Since the _origBatch
		/// is a reference to the object double-clicked on in the batch browser, simply changing values
		/// here will change them in the Batch object in the list. In that case, the changes are recorded
		/// in the top level batch collection object.
		/// </summary>
		public void SubmitBatch()
		{
			_origBatch.Date = DateOnly.Parse(BatchDate).ToString("yyyy/MM/dd");
			_origBatch.Total = BatchTotal;
			_origBatch.Note = BatchNote;

			HasChanges = false;
		}

		/// <summary>
		/// This method function is called to compute the actual sum of all the donations in
		/// this batch. It should match with the target Total entered at the top of this view.
		/// </summary>
		public void ComputeSum()
		{
			CategorySums.Clear();
			_categorySumDict.Clear();

			foreach (var donation in _batchDonations)
			{
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
		}
	}
}
