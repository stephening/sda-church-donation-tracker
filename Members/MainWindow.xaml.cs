using Members;
using System;
using System.Windows;

namespace Donors
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainWindowViewModel _viewModel;

		public MainWindow(MainWindowViewModel mainWindowViewModel)
		{
			DataContext = mainWindowViewModel;

			_viewModel = mainWindowViewModel;

			InitializeComponent();

			LoadSettings();
		}

		private void LoadSettings()
		{
			_viewModel.DonorViewModel.DonationsVisibility = Visibility.Collapsed;

			Top = Persist.Default.Top;
			Left = Persist.Default.Left;
			Width = (0 <= Persist.Default.Width) ? Persist.Default.Width : Width;
			Height = (0 < Persist.Default.Height) ? Persist.Default.Height : Height;
			if (!string.IsNullOrEmpty(Persist.Default.WindowState))
				WindowState = Enum.Parse<WindowState>(Persist.Default.WindowState);
		}

		private void SaveSettings()
		{
			Persist.Default.Top = Top;
			Persist.Default.Left = Left;
			Persist.Default.Width = Width;
			Persist.Default.Height = Height;
			Persist.Default.WindowState = WindowState.ToString();

			Persist.Default.Save();

			Donations.Lib.Settings.Save();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveSettings();
		}
	}
}
