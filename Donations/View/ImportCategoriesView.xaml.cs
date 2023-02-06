using Donations.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for ImportCategoriesView.xaml
	/// </summary>
	public partial class ImportCategoriesView : UserControl
	{
		private ImportCategoriesViewModel? _viewModel;

		public ImportCategoriesView()
		{
			InitializeComponent();

			_viewModel = DataContext as ImportCategoriesViewModel;
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
