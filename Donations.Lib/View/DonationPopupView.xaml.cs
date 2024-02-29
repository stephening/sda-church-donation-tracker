using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for DonationPopupView.xaml
/// </summary>
public partial class DonationPopupView : Window
{
	private readonly DonationPopupViewModel? _viewModel;
	private readonly HelpView _helpView;

	public delegate DonationPopupView Factory(ObservableCollection<Donation>? donations);

	public DonationPopupView(
		DonationPopupViewModel donationPopupViewModel,
		ObservableCollection<Donation>? donations,
		HelpView helpView
		)
	{
		InitializeComponent();
		_viewModel = donationPopupViewModel;
		_helpView = helpView;
		DataContext = _viewModel;
		_viewModel?.Initialize(donations);
	}


	private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
	{
		_helpView.ShowTarget("Donation-popup-view");
	}

	private void Window_Loaded(object sender, RoutedEventArgs e)
	{
		Width = (0 <= Persist.Default.DonationEnvelopeViewWidth) ? Persist.Default.DonationEnvelopeViewWidth : Width;
		Height = (0 < Persist.Default.DonationEnvelopeViewHeight) ? Persist.Default.DonationEnvelopeViewHeight : Height;
		Top = Persist.Default.DonationEnvelopeViewTop;
		Left = Persist.Default.DonationEnvelopeViewLeft;
		if (!string.IsNullOrEmpty(Persist.Default.DonationEnvelopeViewWindowState))
			WindowState = Enum.Parse<WindowState>(Persist.Default.DonationEnvelopeViewWindowState);
	}

	private void Window_Unloaded(object sender, RoutedEventArgs e)
	{
		Persist.Default.DonationEnvelopeViewTop = Top;
		Persist.Default.DonationEnvelopeViewLeft = Left;
		Persist.Default.DonationEnvelopeViewWidth = Width;
		Persist.Default.DonationEnvelopeViewHeight = Height;
		Persist.Default.DonationEnvelopeViewWindowState = WindowState.ToString();
		Persist.Default.Save();

		_helpView.ForceClose();
	}

}
