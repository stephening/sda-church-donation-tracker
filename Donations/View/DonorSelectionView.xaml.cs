using Donations.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for DonorSelectionView.xaml
	/// </summary>
	public partial class DonorSelectionView : Window
	{
		private DonorSelectionViewModel? _viewModel;

		public DonorSelectionView()
		{
			InitializeComponent();

			_viewModel = DataContext as DonorSelectionViewModel;
		}

		private void Filter_TextChanged(object sender, TextChangedEventArgs e)
		{
			_viewModel?.TextChanged();
		}

		private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DialogResult = true;

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
