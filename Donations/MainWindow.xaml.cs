using Donations.Lib;
using Donations.Lib.Services;
using Donations.Lib.View;
using Donations.Lib.ViewModel;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Donations;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	private readonly MainWindowViewModel _mainWindowViewModel;
	private readonly HelpView _helpView;
	private bool _skipHandler = false;

	/// <summary>
	/// Constructor. This reference is saved to Global.Main for other view model use.
	/// </summary>
	public MainWindow(
		MainWindowViewModel mainWindowViewModel,
		HelpView helpView
	)
	{
		DataContext = mainWindowViewModel;

		InitializeComponent();

		LoadSettings();
		_mainWindowViewModel = mainWindowViewModel;
		_helpView = helpView;
	}

	private async void Window_Loaded(object sender, RoutedEventArgs e)
	{
		if (SqlHelper.DbKey != "production")
		{
			Background = Brushes.Red;
		}
	}

	private void LoadSettings()
	{
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

		Settings.Save();
	}

	private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		// shudown is kinda slow because of possible database updates,
		// so cancel it first time around, then set a flag to skip handler
		// the second time which is triggered by the Shutdown().
		if (!_skipHandler)
		{
			e.Cancel = true;
			Hide();
			SaveSettings();
			_helpView.ForceClose();
			await _mainWindowViewModel.Shutdown();
			_skipHandler = true;
			Application.Current.Shutdown();
		}
	}

	private void Window_KeyDown(object sender, KeyEventArgs e)
	{
		if (Key.F1 == e.Key)
		{
			_helpView.ShowTarget("");
		}
	}
}
