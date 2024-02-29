using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for DonorModalView.xaml
	/// </summary>
	public partial class DonorModalView : Window
	{
		private readonly HelpView _helpView;
		private DonorViewModel? _viewModel;

		public delegate DonorModalView Factory(Donor? donor, bool showDonations = true);

		public DonorModalView(
			HelpView helpView,
			DonorViewModel donorViewModel,
			Donor? donor,
			bool showDonations = true)
		{
			InitializeComponent();
			_helpView = helpView;
			_viewModel = donorViewModel;

			DonorView.DataContext = _viewModel;

			if (!showDonations)
			{
				_viewModel.DonationsVisibility = Visibility.Collapsed;
			}

			_viewModel?.SetDonor(donor);
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Top = Persist.Default.DonorTop;
			Left = Persist.Default.DonorLeft;
			Width = (0 <= Persist.Default.DonorWidth) ? Persist.Default.DonorWidth : Width;
			Height = (0 < Persist.Default.DonorHeight) ? Persist.Default.DonorHeight : Height;
			if (!string.IsNullOrEmpty(Persist.Default.DonorWindowState))
				WindowState = Enum.Parse<WindowState>(Persist.Default.DonorWindowState);
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			Persist.Default.DonorTop = Top;
			Persist.Default.DonorLeft = Left;
			Persist.Default.DonorWidth = Width;
			Persist.Default.DonorHeight = Height;
			Persist.Default.DonorWindowState = WindowState.ToString();
			Persist.Default.Save();

			_helpView.ForceClose();
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			_helpView.ShowTarget("Donor-modal-view");
		}
	}
}
