using Donations.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for CategorySelectionView.xaml
	/// </summary>
	public partial class CategorySelectionView : Window
	{
		private CategorySelectionViewModel? _viewModel;

		public CategorySelectionView()
		{
			InitializeComponent();

			_viewModel = DataContext as CategorySelectionViewModel;
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			_viewModel?.TextChanged();
		}

		private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DialogResult = true;
			e.Handled = true;

			Close();
		}

		private void Click_OK(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

			Close();
		}

		private void Click_Cancel(object sender, RoutedEventArgs e)
		{
			DialogResult = false;

			Close();
		}

		private void Click_Remove(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			CategoryGrid.SelectedIndex = -1;

			Close();
		}

		private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			// catch and handle enter this way so we can block it from going down a row
			// and so we can mark it handled the the keyboard ENTER doesn't go to the
			// parent window
			if (e.Key == Key.Enter)
			{
				DialogResult = true;

				Close();

				e.Handled = true;
			}
		}
	}
}
