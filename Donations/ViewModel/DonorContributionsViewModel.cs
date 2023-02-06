using Donations.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Donations.ViewModel
{
	/// <summary>
	/// This view model handles the functionality and the binding to the DonorContrinutionsView.xaml. The view associated
	/// with this view model is a Window that is used as a modal dialog. It is used when the operator double-clicks on an
	/// entry in a category subtotal view. What the dialog view will show is the donor's who contributed to that category
	/// in the given batch, and the amount each gave. No editing is possible on this Window.
	/// </summary>
	public class DonorContributionsViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObservableCollection<Summary>? ContributionList = new ObservableCollection<Summary>();
		public CollectionViewSource Contributions { get; set; } = new CollectionViewSource();

		private double _subTotal;
		/// <summary>
		/// The SubTotal property is bound to a ReadOnly light green TextBox which shows the computed sum
		/// of all donor contributions shown. This subtotal amount should match the category subtotal for
		/// which this is a drill down on.
		/// </summary>
		public double SubTotal
		{
			get { return _subTotal; }
			set
			{
				_subTotal = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The constructor only needs to set the Source property for the CollectionViewSource that is 
		/// bound to the DataGrid.
		/// </summary>
		public DonorContributionsViewModel()
		{
			Contributions.Source = ContributionList;
		}

		/// <summary>
		/// This method is called from the Window DonorContributionsView.xaml.cs, to supply the Batch and
		/// CategorySum to the view model, prior to calling the Window.ShowDialog().
		/// 
		/// Allowance is made to use the name directly from the Donation record if the donor Id no longer
		/// exists in the donor database.
		/// </summary>
		/// <param name="batch">This parameter is supplied to filter the Donation list down to the BatchId.</param>
		/// <param name="categorySum">In addition to filtering for donations from the batch, additional
		/// filtering is done for the category specified in this object.</param>
		public void Show(Batch? batch, CategorySum? categorySum)
		{
			if (null == batch)
			{
				throw new ArgumentNullException("batch paremter is null");
			}

			if (null == categorySum)
			{
				throw new ArgumentNullException("categorySum paremter is null");
			}

			ContributionList.Clear();

			foreach (var donation in di.Data.DonationList.Where(x => x.BatchId == batch.Id).Where(x => x.Category == categorySum.Category))
			{
				Summary summary = new Summary()
				{
					Subtotal = donation.Value
				};
				if (di.Data.DonorDict.ContainsKey(donation.DonorId))
				{
					summary.LastName = di.Data.DonorDict[donation.DonorId].LastName;
					summary.FirstName = di.Data.DonorDict[donation.DonorId].FirstName;
				}
				else
				{
					summary.LastName = donation.LastName;
					summary.FirstName = donation.FirstName;
				}
				ContributionList.Add(summary);
			}
			Contributions.View.Refresh();
			SubTotal = categorySum.Sum;
		}
	}
}
