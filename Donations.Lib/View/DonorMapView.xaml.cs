using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Donations.Lib.View
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

			_timer.Tick += new EventHandler(Timer_Tick);
			_timer.Interval = new TimeSpan(0, 0, 1);
		}

		/// <summary>
		/// This method is just for unit testing because the UT cannot access the DataGrid directly.
		/// It has the same effect on the DonorMapViewModel's CollectionView that a user row click would.
		/// </summary>
		/// <param name="index"></param>
		public void SelectGridRow(int index)
		{
			DonorMapDataGrid.SelectedIndex = index;
		}

		private DispatcherTimer _timer = new DispatcherTimer();

		private bool _ignoreClick = false;
		private void Timer_Tick(object sender, EventArgs e)
		{
			_timer.Stop();
			_ignoreClick = false;
		}


		private void DataGridRow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (_ignoreClick)
			{
				// double click to close category selection leaves mouse up event in the queue, ignore it
				return;
			}

			DataGridRow row = (DataGridRow)sender;
			AGDonorMapItem? entry = (AGDonorMapItem?)row?.DataContext;

			DonorSelectionView dlg = DependencyInjection.DonorSelectionView;

			var ret = dlg.ShowDialog();

			// double-clicking to close the donor selection dialog over the map list leaves a mouse click
			// in the queue, which gets handled and results in poping up the donor selection window again, so
			// ignore mouse clicks for a second on entering this function
			_timer.Start();
			_ignoreClick = true;

			if (ret == true)
			{
				Donor? donor = dlg.DonorGrid?.SelectedItem as Donor;
				_viewModel?.SetDonor(entry, donor);
			}
		}

		private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as DonorMapViewModel;
		}
	}
}
