using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Donations.Lib.View
{
	public partial class ConfirmDonorMergeView : Window
	{
		private ConfirmDonorMergeViewModel? _viewModel;
		private readonly HelpView _helpView;

		public delegate ConfirmDonorMergeView Factory(Donor mergeTo, Donor mergeFrom, ObservableCollection<Donation> donations);

		public ConfirmDonorMergeView(
			HelpView helpView,
			Donor mergeTo,
			Donor mergeFrom,
			ObservableCollection<Donation> donations
		)
		{
			InitializeComponent();

			_viewModel?.Init(mergeTo, mergeFrom, donations);
			_helpView = helpView;
		}

		private void Merge_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;

			Close();
		}

		private void Window_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as ConfirmDonorMergeViewModel;
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			_helpView.ShowTarget("Confirm-donor-merge");
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_helpView.ForceClose();
		}
	}
}
