using Donations.Lib.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for WizardMainWindow.xaml
/// </summary>
public partial class WizardMainWindow : Window
{
	private readonly HelpView _helpView;

	public WizardMainWindow(
		HelpView helpView,
		WizardMainWindowViewModel.Factory wizardMainWindowViewModelFactory
	)
	{
		InitializeComponent();

		_helpView = helpView;
		WizardMainWindowViewModel = wizardMainWindowViewModelFactory(true);
		DataContext = WizardMainWindowViewModel;
	}

	public WizardMainWindowViewModel WizardMainWindowViewModel { get; }

	private void Window_KeyDown(object sender, KeyEventArgs e)
	{
		if (Key.F1 == e.Key)
		{
			var tabItem = MainTabControl.SelectedItem as TabItem;
			string target = "";
			string s;

			if (true == tabItem?.Header.Equals(s = (string)FindResource("WizardIntroductionHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("WizardSqlChoiceHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("WizardCloudInstallHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("WizardLocalhostInstallHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("WizardConnectionStringHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("WizardCreateDatabaseAndTablesHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("WizardSpecifyLogoHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("WizardImportCategoriesHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("WizardImportDonorsHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("WizardImportDonationsHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("WizardFinishedHeader")))
			{
				target = s.Replace(" ", "-");
			}

			_helpView.ShowTarget(target);
		}
	}

	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		_helpView.ForceClose();
	}
}
