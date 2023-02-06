using Donations.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Donations.ViewModel
{
	/// <summary>
	/// This view model handles the functionality and the binding to the DonorSelectionView.xaml which
	/// is a Window, which will be used as a modal dialog whenever the operator wishes to change a
	/// donor. The window contains separate fist/last name text filter boxes and a list of donors.
	/// The list is filtered by the text in both filters.
	/// </summary>
	public class DonorSelectionViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public CollectionViewSource ViewSource { get; set; } = new CollectionViewSource();

		/// <summary>
		/// The LastNameFilterText property is bound to the LastName filter TextBox. As changes are
		/// typed in this field, the name filter is constantly getting applied and changing the list
		/// of donors available to choose from. The filter must match from the start.
		/// </summary>
		public string LastNameFilterText { get; set; } = "";
		/// <summary>
		/// The FirstNameFilterText property is bound to the FirstName filter TextBox. As changes are
		/// typed in this field, the name filter is constantly getting applied and changing the list
		/// of donors available to choose from. The filter must match from the start.
		/// </summary>
		public string FirstNameFilterText { get; set; } = "";

		/// <summary>
		/// The OKEnabled prperty controls whether the OK button is enabled or not. The
		/// only time the OK button is disabled is if the filter yields not results, and
		/// the list is empty.
		/// </summary>
		public bool OKEnabled => SelectedDonorIndex != -1;

		private int _selectedDonorIndex = 0;
		/// <summary>
		/// The SelectedDonorIndex property is bound to the SelectedIndex property of
		/// the DataGrid. This allows the Click handler in DonorSelectionView.xaml.cs
		/// to obtain the selected donor.
		/// </summary>
		public int SelectedDonorIndex
		{
			get
			{
				return _selectedDonorIndex;
			}
			set
			{
				_selectedDonorIndex = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The constructor sets the CollectionViewSource's Source, and Filter properties.
		/// It also set's the list selection to the first item if the list is not empty.
		/// </summary>
		public DonorSelectionViewModel()
		{
			ViewSource.Source = di.Data.DonorList;
			ViewSource.Filter += new FilterEventHandler(Filter);

			ViewSource.View.Refresh();
			if (!ViewSource.View.IsEmpty)
			{
				SelectedDonorIndex = 0;
			}
		}

		/// <summary>
		/// The filter method uses the LastNameFilterText and the FirstNameFilterText on their respective
		/// name properties in the Donor record.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Filter(object sender, FilterEventArgs e)
		{
			var obj = e.Item as Donor;
			if (obj != null)
			{
				if ((string.IsNullOrEmpty(LastNameFilterText) || true == obj.LastName?.StartsWith(LastNameFilterText, System.StringComparison.OrdinalIgnoreCase))
					&& (string.IsNullOrEmpty(FirstNameFilterText) || true == obj.FirstName?.StartsWith(FirstNameFilterText, System.StringComparison.OrdinalIgnoreCase))
					)
					e.Accepted = true;
				else
					e.Accepted = false;
			}
		}

		/// <summary>
		/// This method is called in real time as the filter text is changed. As a result, the list contents are constantly
		/// changing to reflect the filter text.
		/// </summary>
		public void TextChanged()
		{
			ViewSource.View.Refresh();
			if (!ViewSource.View.IsEmpty)
			{
				SelectedDonorIndex = 0;
			}
			OnPropertyChanged(nameof(OKEnabled));
		}
	}
}
