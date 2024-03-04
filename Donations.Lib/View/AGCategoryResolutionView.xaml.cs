using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Donations.Lib.View
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
		}

		public void ChooseCategory(object sender, RoutedEventArgs e)
		{
			CategorySelectionView dlg = DependencyInjection.CategorySelectionView;

			if (true == dlg.ShowDialog())
			{
				_viewModel?.ChooseCategory((dlg.CategoryGrid.SelectedItem as Category));
			}
		}

		private async void ImportCategory(object sender, RoutedEventArgs e)
		{
			if (null != _viewModel && null != _viewModel.Transaction)
			{
				int code = _viewModel.Transaction.CategoryCode;
				string description = _viewModel.Transaction.CategoryName;

				var answer = MessageBox.Show($"Is this the category \"{description}\" tax deductible?", "Import new caetgory",
					MessageBoxButton.YesNo, MessageBoxImage.Question, defaultResult: MessageBoxResult.Yes);

				Category cat = new Category() { Code = code, Description = description };

				if (MessageBoxResult.No == answer)
				{
					cat.TaxDeductible = false;
				}
				else
				{
					cat.TaxDeductible = true;
				}

				await _viewModel.AddCategory(cat);
			}
		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as AGCategoryResolutionViewModel;
		}
	}
}
