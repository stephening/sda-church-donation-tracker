using Donations.Model;
using Donations.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for DonorMapView.xaml
	/// </summary>
	public partial class DonorMapView : UserControl
	{
		private DonorMapViewModel? _viewModel;

		public DonorMapView()
		{
			InitializeComponent();

			_viewModel = DataContext as DonorMapViewModel;
		}

		private void DataGridRow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = (DataGridRow)sender;
			AGDonorMapItem? entry = (AGDonorMapItem?)row?.DataContext;

			DonorSelectionView dlg = new DonorSelectionView();

			var ret = dlg.ShowDialog();
			if (ret == true)
			{
				Donor? donor = dlg.DonorGrid?.SelectedItem as Donor;
				_viewModel?.SetDonor(entry, donor);
			}
		}
	}
}
