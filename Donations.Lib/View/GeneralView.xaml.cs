using Donations.Lib.ViewModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for GeneralView.xaml
	/// </summary>
	public partial class GeneralView : UserControl
	{
		private GeneralViewModel? _viewModel;

		public GeneralView()
		{
			InitializeComponent();
		}

		private void ExportCategoriesCsv(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Categories (*.csv)|*.csv";

			if (dlg.ShowDialog() == true)
			{
				_viewModel!.ExportCategories(dlg.FileName);
			}
		}

		private async void ExportDonorsCsv(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Donors (*.csv)|*.csv";

			if (dlg.ShowDialog() == true)
			{
				await _viewModel!.LoadDonors();
				_viewModel.ExportDonors(dlg.FileName);
			}
		}

		private async void ExportDonationsCsv(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "Donations (*.csv)|*.csv";

			if (dlg.ShowDialog() == true)
			{
				await _viewModel.ExportDonations(dlg.FileName);
			}
		}

		private void LogoBrowse(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "Images (*.jpg;*.png)|*.jpg;*.png";

			if (dlg.ShowDialog() == true)
			{
				_viewModel?.SetLogo(dlg.FileName);
			}
		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as GeneralViewModel;
			_viewModel?.SetPasswordObject(Password);
		}

		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			_viewModel.PasswordChanged(sender as PasswordBox);
		}
	}
}
