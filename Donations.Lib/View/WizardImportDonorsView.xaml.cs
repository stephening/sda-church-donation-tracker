using Donations.Lib.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for WizardImportDonorsView.xaml
	/// </summary>
	public partial class WizardImportDonorsView : UserControl
	{
		private WizardImportDonorsViewModel? _viewModel;

		public WizardImportDonorsView()
		{
			InitializeComponent();
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

		private async void Save(object sender, RoutedEventArgs e)
		{
			string? ret = await _viewModel?.Save(false);

			if (null != ret)
			{
				if (MessageBoxResult.Yes == MessageBox.Show(ret, "Overwrite database?", MessageBoxButton.YesNo, MessageBoxImage.Exclamation))
				{
					_ = await _viewModel.Save(true);
					return;
				}
			}
		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as WizardImportDonorsViewModel;
		}
	}
}
