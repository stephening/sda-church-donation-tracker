using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for CategoryMapView.xaml
	/// </summary>
	public partial class CategoryMapView : UserControl
	{
		private CategoryMapViewModel? _viewModel;
		public CategoryMapView()
		{
			InitializeComponent();

#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
			_timer.Tick += new EventHandler(Timer_Tick);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
			_timer.Interval = new TimeSpan(0, 0, 1);
		}

		/// <summary>
		/// This method is just for unit testing because the UT cannot access the DataGrid directly.
		/// It has the same effect on the DonorMapViewModel's CollectionView that a user row click would.
		/// </summary>
		/// <param name="index"></param>
		public void SelectGridRow(int index)
		{
			CategoryMapDataGrid.SelectedIndex = index;
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
			AGCategoryMapItem? entry = (AGCategoryMapItem?)row?.DataContext;

			CategorySelectionView dlg = DependencyInjection.CategorySelectionView;

			var ret = dlg.ShowDialog();

			// double-clicking to close the category selection dialog over the category list leaves a mouse click
			// in the queue, which gets handled and results in poping up the category selection window again, so
			// ignore mouse clicks for a second on entering this function
			_timer.Start();
			_ignoreClick = true;

			if (ret == true)
			{
				Category? cat = dlg.CategoryGrid?.SelectedItem as Category;
				_viewModel?.SetCategory(entry, cat);
			}
		}

		private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as CategoryMapViewModel;
		}
	}
}
