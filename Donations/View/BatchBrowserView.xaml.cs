using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for BatchBrowserView.xaml
	/// </summary>
	public partial class BatchBrowserView : UserControl
	{
		private BatchBrowserViewModel? _viewModel;

		public BatchBrowserView()
		{
			InitializeComponent();

			_viewModel = DataContext as BatchBrowserViewModel;
		}

		private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = (DataGridRow)sender;
			Batch? batch = (Batch?)row?.DataContext;

			var batchDonationList = new ObservableCollection<Donation>(di.Data.DonationList.Where(x => x.BatchId == batch.Id));

			BatchReviewView dlg = new BatchReviewView(batch, batchDonationList);
			try
			{
				dlg.ShowDialog();
			}
			catch
			{
				MessageBox.Show("Unexpected exception wtih batchreview");
			}

			_viewModel?.Refresh();
		}
	}
}
