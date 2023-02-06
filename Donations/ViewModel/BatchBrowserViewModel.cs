using CommunityToolkit.Mvvm.Input;
using Donations.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Donations.ViewModel
{
	public enum enumBatchFilterOptions {
		CurrentYear,
		PreviousYear,
		SelectYear,
		DateRange
	};

	/// <summary>
	/// This view model handles the functionality and the binding to the BatchBrowserView.xaml which is a
	/// UserControl under the 'Batch browser' tab.
	/// 
	/// The view associated with this view model is displayed on the default tab and the first thing the
	/// user see's when launching the application. It is a way of seeing the donations entered by batch.
	/// Donations are usually entered in batches and there is usually an expected Total that should match
	/// the sum of all the individual donations. Whether these two totals match is indicated by whether 
	/// they are colored with a red background or not.
	/// 
	/// Details of each batch can be reviewed or edited if needed by double clicking on a row in the batch
	/// browser view.
	/// </summary>
	public class BatchBrowserViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public CollectionViewSource BatchListSource { get; set; } = new CollectionViewSource();

		private enumBatchFilterOptions _batchFilterOptions;

		/// <summary>
		/// Since all donations are stored in a single database, this view model facilitates the option
		/// to filter them by year or range. The BatchFilterOption property is bound to the radio
		/// button options for 'Current year', 'Previous year', 'Select year', and 'Range'. Based on the
		/// radio button selection, the desired donation batches will be shown.
		/// </summary>
		public enumBatchFilterOptions BatchFilterOption
		{
			get { return _batchFilterOptions; }
			set
			{
				// based on changes to this property (radio button changes) we want other controls to be
				// refreshed
				_batchFilterOptions = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(SelectYearComboBoxEnabled));
				OnPropertyChanged(nameof(DateRangeEnabled));
				BatchListSource.View.Refresh();
			}
		}

		/// <summary>
		/// The AvailableYears property is a collection of years formed by filtering all the years from
		/// the donor database. This list is shown in a ComboBox if the 'Select year' radio button is
		/// chosen.
		/// </summary>
		public ObservableCollection<string>? AvailableYears { get; set; }
		/// <summary>
		/// The SelectYearComboBoxEnabled property allows the ComboBox to be enable/disable depending on
		/// which radio button is selected.
		/// </summary>
		public bool SelectYearComboBoxEnabled => BatchFilterOption == enumBatchFilterOptions.SelectYear;
		/// <summary>
		/// The DateRangeEnabled property allows the Date pickers to be enable/disable depending on
		/// which radio button is selected.
		/// </summary>
		public bool DateRangeEnabled => BatchFilterOption == enumBatchFilterOptions.DateRange;

		private string _filterYear = "";
		/// <summary>
		/// The FilterYear property is bound to the SelectedItem property of the available years ComboBox.
		/// </summary>
		public string FilterYear
		{
			get { return _filterYear; }
			set
			{
				_filterYear = value;
				OnPropertyChanged();
				BatchListSource.View.Refresh();
			}
		}

		private DateOnly _filterStartDate;
		/// <summary>
		/// The FilterStartDate property is bound to the starting date range field.
		/// </summary>
		public string FilterStartDate
		{
			get
			{
				string str = _filterStartDate.ToString("yyyy/MM/dd");
				return str.Equals("0001/01/01") ? "" : str;
			}
			set
			{
				try
				{
					if (string.IsNullOrEmpty(value))
						_filterStartDate = DateOnly.MinValue;
					else
						_filterStartDate = DateOnly.Parse(value);
					OnPropertyChanged();
					BatchListSource.View.Refresh();
				}
				catch
				{
					_filterStartDate = DateOnly.MinValue;
				}
			}
		}

		private DateOnly _filterEndDate;
		/// <summary>
		/// The FilterEndDate property is bound to the ending date range field.
		/// </summary>
		public string FilterEndDate
		{
			get
			{
				string str = _filterEndDate.ToString("yyyy/MM/dd");
				return str.Equals("0001/01/01") ? "" : str;
			}
			set
			{
				try
				{
					if (string.IsNullOrEmpty(value))
						_filterEndDate = DateOnly.MinValue;
					else
						_filterEndDate = DateOnly.Parse(value);
					OnPropertyChanged();
					BatchListSource.View.Refresh();
				}
				catch
				{
					_filterEndDate = DateOnly.MinValue;
				}
			}
		}

		// These privatge members variables will be used for the 'Current year' and 'Previous year' radio
		// button options.
		private string _thisYear = DateTime.Now.Year.ToString();
		private string _prevYear = (DateTime.Now.Year - 1).ToString();

		/// <summary>
		/// The DeleteRowCmd property is bound to the 'Delete row' context menu option. It allows
		/// for the row that was right-clicked on to be deleted.
		/// </summary>
		public ICommand DeleteRowCmd { get; }

		/// <summary>
		/// The constructor places its this pointer in the Global static object for use by other ViewModels
		/// that may need access to its public members. It also adds the year or date range Filter to the
		/// CollectionViewSource object. The it calls BatchListUpdated() to further perform initialization.
		/// </summary>
		public BatchBrowserViewModel()
		{
			di.BatchBrowser = this;

			BatchListSource.Source = di.Data.BatchList;
			BatchListSource.Filter += new FilterEventHandler(Filter);

			DeleteRowCmd = new AsyncRelayCommand(DeleteRow);

			BatchListUpdated();
		}

		/// <summary>
		/// This method is called in response to the right-click context menu:'Delete row'.
		/// This is the method bound to the Command, and it will delete the row that was 
		/// right-clicked on to get the context menu.
		/// </summary>
		public async Task DeleteRow()
		{
			Batch remove = BatchListSource.View.CurrentItem as Batch;
			if (remove == null) return;

			for (int i = di.Data.DonationList.Count - 1; i >= 0; i--)
			{
				if (di.Data.DonationList[i].BatchId == remove.Id)
				{
					di.Data.DonationDict.Remove(di.Data.DonationList[i].Id);
					di.Data.DonationList.RemoveAt(i);
				}
			}
			di.Data.BatchDict.Remove(remove.Id);
			di.Data.BatchList?.Remove(remove);

			di.Data.SaveData();
		}

		/// <summary>
		/// This method will update the AvailableYears collection using a Linq filter. It also
		/// set's the start/end range dates to the earliest and latest years from the list of batches.
		/// </summary>
		public void BatchListUpdated()
		{
			if (0 < di.Data.BatchList?.Count)
			{
				AvailableYears = new ObservableCollection<string>(di.Data.BatchList.Select((batch) => DateOnly.Parse(batch.Date).Year.ToString()).Distinct().Reverse());
				FilterYear = AvailableYears.First();

				FilterStartDate = di.Data.BatchList?.Min(x => x.Date);
				FilterEndDate = di.Data.BatchList?.Max(x => x.Date);
			}
		}
		/// <summary>
		/// This member funcion is the filter used by the CollectionViewSource object to only use donations
		/// matching the filter options.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Filter(object sender, FilterEventArgs e)
		{
			var obj = e.Item as Batch;
			if (obj != null)
			{
				switch (BatchFilterOption)
				{
					case enumBatchFilterOptions.CurrentYear:
						if (obj.Date.StartsWith(_thisYear)) e.Accepted = true;
						else e.Accepted = false;
						break;
					case enumBatchFilterOptions.PreviousYear:
						if (obj.Date.StartsWith(_prevYear)) e.Accepted = true;
						else e.Accepted = false;
						break;
					case enumBatchFilterOptions.SelectYear:
						if (obj.Date.StartsWith(FilterYear)) e.Accepted = true;
						else e.Accepted = false;
						break;
					case enumBatchFilterOptions.DateRange:
						DateOnly date = DateOnly.Parse(obj.Date);
						if (string.IsNullOrEmpty(FilterStartDate) || string.IsNullOrEmpty(FilterStartDate) 
							|| (_filterStartDate <= date && date <= _filterEndDate)) e.Accepted = true;
						else e.Accepted = false;
						break;
				}
			}
		}
		/// <summary>
		/// This method is used to update the list, if new donations have been added
		/// and the list should be updated when returning to this tab.
		/// </summary>
		public void Refresh()
		{
			BatchListSource.View.Refresh();
		}
	}
}
