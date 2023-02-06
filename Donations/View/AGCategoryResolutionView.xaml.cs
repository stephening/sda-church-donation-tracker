using Donations.Model;
using Donations.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for AGCategoryResolutionView.xaml
	/// </summary>
	public partial class AGCategoryResolutionView : UserControl
	{
		private AGCategoryResolutionViewModel? _viewModel;

		public AGCategoryResolutionView()
		{
			InitializeComponent();

			_viewModel = DataContext as AGCategoryResolutionViewModel;
		}

		public void ChooseCategory(object sender, RoutedEventArgs e)
		{
			CategorySelectionView dlg = new CategorySelectionView();

			if (true == dlg.ShowDialog())
			{
				_viewModel?.ChooseCategory((dlg.CategoryGrid.SelectedItem as Category));
			}
		}
	}
}
