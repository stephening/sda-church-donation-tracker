using Donations.Lib.ViewModel;
using System.Windows.Controls;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for CategoryView.xaml
	/// </summary>
	public partial class CategoryView : UserControl
	{
		private CategoryViewModel? _viewModel;

		public CategoryView()
		{
			InitializeComponent();
		}

		/// <summary>
		/// This method is just for unit testing because the UT cannot access the DataGrid directly.
		/// It has the same effect on the DonorMapViewModel's CollectionView that a user row click would.
		/// </summary>
		/// <param name="index"></param>
		public void SelectGridRow(int index)
		{
			CategoryGrid.SelectedIndex = index;
		}

		private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			if (null != _viewModel)
				_viewModel.HasChanges = true;
		}

		private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as CategoryViewModel;
		}

		private void CheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (null != _viewModel)
				_viewModel.HasChanges = true;
			e.Handled = false;
		}
	}
}
