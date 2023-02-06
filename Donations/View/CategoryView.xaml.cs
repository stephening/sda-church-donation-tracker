using Donations.ViewModel;
using System.Windows.Controls;

namespace Donations.View
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

			_viewModel = DataContext as CategoryViewModel;
		}

		private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
		{
			if (null != _viewModel)
				_viewModel.HasChanges = true;
		}
	}
}
