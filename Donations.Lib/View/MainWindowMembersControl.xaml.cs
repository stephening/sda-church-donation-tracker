using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
		_helpView?.ForceClose();
	}

	private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
	{
		_mainWindowMembersViewModel = DataContext as MainWindowMembersViewModel;
	}

	private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
	{
		if (Key.F1 == e.Key)
		{
			var tabItem = MainTabControl.SelectedItem as TabItem;
			string target = "";
			string s;

			if (true == tabItem?.Header.Equals(s = (string)FindResource("MemberTabHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("DirectoryTabHeader")))
			{
				string baseTarget = s.Replace(" ", "-");
				TabControl? tab = (tabItem.Content as DirectoryTabView)?.DirectoryTabs;
				if (null != tab)
				{
					var item = tab.SelectedItem as TabItem;

					if (-1 == tab.SelectedIndex || true == item?.Header.Equals(s = (string)FindResource("DirectoryPdfTabHeader")))
					{
						target = baseTarget + '-' + s.Replace(" ", "-");
					}
					else if (true == item?.Header.Equals(s = (string)FindResource("DirectoryHtmlTabHeader")))
					{
						target = baseTarget + '-' + s.Replace(" ", "-");
					}
				}
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("AboutTabHeader")))
			{
				target = s.Replace(" ", "-");
			}

			_helpView.ShowTarget(target);
			e.Handled = true;
		}
	}

	private async void DirectoryTab_Selected(object sender, RoutedEventArgs e)
	{
		await _mainWindowMembersViewModel?.DirectoryViewModel?.Loading();
	}
}
