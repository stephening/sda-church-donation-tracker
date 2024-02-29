using Donations.Lib.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for WizardImportCategoriesView.xaml
	/// </summary>
	public partial class WizardImportCategoriesView : UserControl
	{
		private WizardImportCategoriesViewModel? _viewModel;

		public WizardImportCategoriesView()
		{
			InitializeComponent();
		}

		private void ChooseFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = ".csv";
			dlg.Filter = "Categories (*.csv)|*.csv";

			var res = dlg.ShowDialog();

			if (true == res)
			{
				try
				{
					_viewModel?.ReadFile(dlg.FileName);
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.Message, "Error parsing category csv", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
			_viewModel = DataContext as WizardImportCategoriesViewModel;
		}
	}
}
