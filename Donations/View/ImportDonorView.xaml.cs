using Donations.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for ImportDonorView.xaml
	/// </summary>
	public partial class ImportDonorView : UserControl
	{
		private ImportDonorViewModel? _viewModel;

		public ImportDonorView()
		{
			InitializeComponent();

			_viewModel = DataContext as ImportDonorViewModel;
		}

		private void ChooseFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = ".csv";
			dlg.Filter = "Donors (*.csv)|*.csv";

			var res = dlg.ShowDialog();

			if (true == res)
			{
				try
				{
					_viewModel?.ReadFile(dlg.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error parsing donor csv", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			}
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			string? ret = _viewModel?.Save(false);

			if (null != ret)
			{
				if (MessageBoxResult.Yes == MessageBox.Show(ret, "Overwrite database?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation))
				{
					_viewModel.Save(true);
					return;
				}
			}
		}
	}
}
