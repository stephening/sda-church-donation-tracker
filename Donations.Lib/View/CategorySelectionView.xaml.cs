using Donations.Lib.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for CategorySelectionView.xaml
	/// </summary>
	public partial class CategorySelectionView : Window
	{
		private readonly HelpView _helpView;
		private readonly CategorySelectionViewModel? _viewModel;
		private int _initialIndex = 0;

		public CategorySelectionView(
			HelpView helpView,
			CategorySelectionViewModel categorySelectionViewModel
		)
		{
			InitializeComponent();
			_helpView = helpView;
			_viewModel = categorySelectionViewModel;
			_initialIndex = _viewModel.SelectedCategoryIndex;

			DataContext = _viewModel;

			FilterText.Text = _viewModel.FilterText;

			FilterText.SelectAll();
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			_viewModel?.TextChanged();

			if (0 != _initialIndex)
			{
				CategoryGrid.SelectedIndex = _initialIndex;
				_initialIndex = 0;
			}
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
			else if (Key.F1 == e.Key)
			{
				_helpView.ShowTarget("Category-selection");
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Top = Persist.Default.CategorySelectTop;
			Left = Persist.Default.CategorySelectLeft;
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			Persist.Default.CategorySelectTop = Top;
			Persist.Default.CategorySelectLeft = Left;
			Persist.Default.Save();

			_helpView.ForceClose();
		}
	}
}
