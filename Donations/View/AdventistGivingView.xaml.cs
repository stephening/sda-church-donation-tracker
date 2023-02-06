using Donations.ViewModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.View
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

			_viewModel = DataContext as AdventistGivingViewModel;
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

		private void AdventistGivingTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TabControl? tabcontrol = sender as TabControl;
			TabItem? tab = tabcontrol?.SelectedItem as TabItem;

			if (Equals(sender, e.OriginalSource))
			{
				(DonationSummaryView.DataContext as AGDonationSummaryViewModel)?.Loaded();
				e.Handled = true;
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


	}
}
