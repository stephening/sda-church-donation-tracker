using Donations.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for ImportDonationsView.xaml
	/// </summary>
	public partial class ImportDonationsView : UserControl
	{
		private ImportDonationsViewModel? _viewModel;

		public ImportDonationsView()
		{
			InitializeComponent();

			_viewModel = DataContext as ImportDonationsViewModel;
		}

		private async void ChooseFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = ".csv";
			dlg.Filter = "Donations (*.csv)|*.csv";

			var res = dlg.ShowDialog();

			if (true == res)
			{
				try
				{
					if (null != _viewModel)
					{
						var msg = await _viewModel.ReadFile(dlg.FileName);
						if (!string.IsNullOrEmpty(msg))
							MessageBox.Show(msg, "Error parsing donation csv", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error parsing donation csv", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				}
			}
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			string? ret = _viewModel?.Save(false);

			if (null != ret)
			{
				if (MessageBoxResult.No == MessageBox.Show(ret, "Overwrite database?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation))
				{
					return;
				}
			}

			_viewModel.Save(true);
		}
	}
}
