using Donations.Lib.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for MainWindowMembersControl.xaml
/// </summary>
public partial class MainWindowMembersControl : UserControl
{
	private HelpView _helpView;
	private MainWindowMembersViewModel? _mainWindowMembersViewModel;

	public MainWindowMembersControl()
	{
		_helpView = DependencyInjection.Resolve<HelpView>();

		Unloaded += MainWindowControl_Unloaded;

		InitializeComponent();
	}


	/// <summary>
	/// Need to close the helpView that was injected into this object,
	/// otherwise the process won't end when the main window is closed.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private async void MainWindowControl_Unloaded(object sender, RoutedEventArgs e)
	{
		//if ((MainTabControl.SelectedItem as TabItem) == ReportsTab)
		//{
		//	// if on the Reports tab save settings
		//	await (ReportsView.DataContext as ReportsViewModel)!.Leaving();
		//}

		//if ((MainTabControl.SelectedItem as TabItem) == MaintenanceMainTab && ((MaintenanceTabs.SelectedItem as TabItem) == MaintenanceGeneralTab))
		//{
		//	// if open to the Maintenance General tab, save settings
		//	await _mainWindowViewModel?.GeneralViewModel?.Leaving();
		//}

		_helpView?.ForceClose();
	}

	private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
	{
		_mainWindowMembersViewModel = DataContext as MainWindowMembersViewModel;
	}

	private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
	{

	}

	private async void DirectoryTab_Selected(object sender, RoutedEventArgs e)
	{
		await _mainWindowMembersViewModel?.DirectoryViewModel?.Loading();
	}
}
