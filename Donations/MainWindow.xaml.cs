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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	private async void Window_Loaded(object sender, RoutedEventArgs e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		SaveSettings();
		_helpView.ForceClose();
	}

	private void Window_KeyDown(object sender, KeyEventArgs e)
	{
		if (Key.F1 == e.Key)
		{
			_helpView.ShowTarget("");
		}
	}
}
