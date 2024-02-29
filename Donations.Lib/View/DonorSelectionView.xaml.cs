using Donations.Lib.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for DonorSelectionView.xaml
	/// </summary>
	public partial class DonorSelectionView : Window
	{
		private readonly HelpView _helpView;
		private readonly DonorSelectionViewModel? _viewModel;

		public DonorSelectionView(
			HelpView helpView,
			DonorSelectionViewModel donorSelectionViewModel
		)
		{
			InitializeComponent();
			_helpView = helpView;
			_viewModel = donorSelectionViewModel;

			DataContext = donorSelectionViewModel;
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
			else if (Key.F1 == e.Key)
			{
				_helpView.ShowTarget("Donor-selection");
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Top = Persist.Default.DonorSelectTop;
			Left = Persist.Default.DonorSelectLeft;
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			Persist.Default.DonorSelectTop = Top;
			Persist.Default.DonorSelectLeft = Left;
			Persist.Default.Save();

			_helpView.ForceClose();
		}
	}
}
