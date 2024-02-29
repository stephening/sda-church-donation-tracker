using Donations.Lib.ViewModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for AdventistGivingView.xaml
	/// </summary>
	public partial class AdventistGivingView : UserControl
	{
		private AdventistGivingViewModel? _viewModel;

		public AdventistGivingView()
		{
			InitializeComponent();
		}

		private async void SelectFile_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = ".csv";
			dlg.Filter = "Adventist giving (.csv)|*.csv";

			var res = dlg.ShowDialog();

			if (true == res && null != _viewModel)
			{
				await _viewModel.Import(dlg.FileName);
			}
		}

		#region these two functions are used to select all in BatchTotal TextBox
		private void BatchTotal_SelectAll(object sender, RoutedEventArgs e)
		{
			(sender as TextBox)?.SelectAll();
		}

		private void BatchTotal_SelectivelyIgnoreMouseClick(object sender, MouseButtonEventArgs e)
		{
			if (false == (sender as TextBox)?.IsKeyboardFocusWithin)
			{
				e.Handled = true;

				(sender as TextBox)?.Focus();
			}
		}

		#endregion

		private async void DonationSummaryTab_Selected(object sender, RoutedEventArgs e)
		{
			await (DonationSummaryView.DataContext as AGDonationSummaryViewModel)?.Loaded();
		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as AdventistGivingViewModel;
		}
	}
}
