using Donations.Lib.View;
using Donations.Lib.ViewModel;
using Members;
using System;
using System.Windows;
using System.Windows.Input;

namespace Donors
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainWindowMembersViewModel _mainWindowMembersViewModel;
		private readonly HelpView _helpView;

		public MainWindow(
			MainWindowMembersViewModel mainWindowMembersViewModel,
			HelpView helpView
		)
		{
			DataContext = mainWindowMembersViewModel;

			InitializeComponent();

			_mainWindowMembersViewModel = mainWindowMembersViewModel;
			_helpView = helpView;

			LoadSettings();
		}

		private void LoadSettings()
		{
			_mainWindowMembersViewModel.DonorViewModel.DonationsVisibility = Visibility.Collapsed;

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
			_helpView.ForceClose();
		}

		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (Key.F1 == e.Key)
			{
				_helpView.ShowTarget("");
			}
		}
	}
}
