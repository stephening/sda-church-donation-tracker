using Donations.Model;
using Donations.ViewModel;
using System;
using System.Windows;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for DonorModalView.xaml
	/// </summary>
	public partial class DonorModalView : Window
	{
		public DonorModalView(Donor? donor)
		{
			InitializeComponent();

			(DonorView.DataContext as DonorViewModel)?.SetDonor(donor);
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
		}
	}
}
