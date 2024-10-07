using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.Lib.View;

public partial class MainWindowControl : UserControl
{
	private HelpView _helpView;
	private MainWindowViewModel? _mainWindowViewModel;

	public MainWindowControl()
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
		if ((MainTabControl.SelectedItem as TabItem) == ReportsTab)
		{
			// if on the Reports tab save settings
			await (ReportsView.DataContext as ReportsViewModel)!.Leaving();
		}

		if ((MainTabControl.SelectedItem as TabItem) == MaintenanceMainTab && ((MaintenanceTabs.SelectedItem as TabItem) == MaintenanceGeneralTab))
		{
			// if open to the Maintenance General tab, save settings
			await _mainWindowViewModel?.GeneralViewModel?.Leaving();
		}

		_helpView?.ForceClose();
	}

	private void DonorInputTab_Selected(object sender, RoutedEventArgs e)
	{
		(DonorInputView.DataContext as DonorInputViewModel)?.Loading();
	}

	private async void ReportsTab_Selected(object sender, RoutedEventArgs e)
	{
		IAppSettingsServices appSettingsServices = DependencyInjection.Resolve<IAppSettingsServices>();

		string? LicenseKey = appSettingsServices.Get().SyncFusionLicenseKey;

		if (!appSettingsServices.GetType().Name.Contains("TestData"))
		{
			if (!string.IsNullOrEmpty(LicenseKey))
			{
				Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(LicenseKey);
			}
			else
			{
				MessageBox.Show(
					"Unable to obtain or register a SyncFusion license key used for producing PDF reports.\n\n"
					+ "You may continue to use the program but if you attempt to produce year end reports, you may get water marks on them.\n\n"
					+ "To obtain a free license key, visit the SyncFusion community license page: https://www.syncfusion.com/sales/communitylicense"
					, "License key");
			}
		}

		await (ReportsView.DataContext as ReportsViewModel)!.Loading();
	}

	private async void ReportsTab_Unselected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.ReportsViewModel?.Leaving();
	}

	private async void BatchBrowserView_Selected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.BatchBrowserViewModel?.Loading();
	}

	private async void DonationBrowserView_Selected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.DonationBrowserViewModel?.Loading();
	}

	private async void MaintenanceTab_Selected(object sender, RoutedEventArgs e)
	{
		string? subtab = (MaintenanceTabs.SelectedItem as TabItem)?.Header.ToString();

		if (true == subtab?.Equals((string)FindResource("MaintenanceCategoryTabHeader")))
		{
			await _mainWindowViewModel?.CategoryViewModel?.Loading();
		}
		else if (true == subtab?.Equals((string)FindResource("MaintenanceCategoryMapTabHeader")))
		{
			await _mainWindowViewModel?.CategoryMapViewModel?.Loading();
		}
		else if (true == subtab?.Equals((string)FindResource("MaintenanceDonorMapTabHeader")))
		{
			await _mainWindowViewModel?.DonorMapViewModel?.Loading();
		}
		else if (true == subtab?.Equals((string)FindResource("MaintenanceGeneralTabHeader")))
		{
			await _mainWindowViewModel?.GeneralViewModel?.Loading();
		}
	}

	private async void MaintenanceCategoryTab_Selected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.CategoryViewModel?.Loading();
	}

	private async void MaintenanceDonorMapTab_Selected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.DonorMapViewModel?.Loading();
	}

	private async void MaintenanceCategorymapTab_Selected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.CategoryMapViewModel?.Loading();
	}

	private void MaintenanceTab_Unselected(object sender, RoutedEventArgs e)
	{
	}

	private async void MaintenanceCategorymapTab_Unselected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.CategoryMapViewModel?.Leaving();
		(sender as TabItem).IsSelected = false;
	}

	private async void MaintenanceCategoryTab_Unselected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.CategoryViewModel?.Leaving();
		(sender as TabItem).IsSelected = false;
	}

	private async void MaintenanceDonorMapTab_Unselected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.DonorMapViewModel?.Leaving();
		(sender as TabItem).IsSelected = false;
	}

	private async void MaintenanceGeneralTab_Selected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.GeneralViewModel?.Loading();
	}

	private async void MaintenanceGeneralTab_Unselected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.GeneralViewModel?.Leaving();
		(sender as TabItem).IsSelected = false;
	}

	private async void DirectoryTab_Selected(object sender, RoutedEventArgs e)
	{
		await _mainWindowViewModel?.DirectoryViewModel?.Loading();
	}

	private void UserControl_KeyDown(object sender, KeyEventArgs e)
	{
		if (Key.F1 == e.Key)
		{
			var tabItem = MainTabControl.SelectedItem as TabItem;
			string target = "";
			string s;

			if (true == tabItem?.Header.Equals(s = (string)FindResource("BatchBrowserTabHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("DonationBrowserTabHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("AdventistGivingTabHeader")))
			{
				string baseTarget = s.Replace(" ", "-");
				TabControl? tab = (tabItem.Content as AdventistGivingView)?.AdventistGivingTabs;
				if (null != tab)
				{
					var item = tab.SelectedItem as TabItem;

					if (-1 == tab.SelectedIndex || true == item?.Header.Equals(s = (string)FindResource("AdventistGivingDonorResolutionTabHeader")))
					{
						target = baseTarget + '-' + s.Replace(" ", "-");
					}
					else if (true == item?.Header.Equals(s = (string)FindResource("AdventistGivingCategoryResolutionTabHeader")))
					{
						target = baseTarget + '-' + s.Replace(" ", "-");
					}
					else if (true == item?.Header.Equals(s = (string)FindResource("AdventistGivingVerifyAndSubmitTabHeader")))
					{
						target = baseTarget + '-' + s.Replace(" ", "-");
					}
				}
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("DonorInputTabHeader")))
			{
				target = s.Replace(" ", "-");
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("ReportsTabHeader")))
			{
				target = s.Replace(" ", "-");
				if (_mainWindowViewModel.ReportsViewModel.ReportOption == enumReportOptions.AllPdf)
				{
					target += "-All-pdf\u0027s";
				}
				else if (_mainWindowViewModel.ReportsViewModel.ReportOption == enumReportOptions.Email)
				{
					target += "-Email-reports";
				}
				else if (_mainWindowViewModel.ReportsViewModel.ReportOption == enumReportOptions.Print)
				{
					target += "-Printed-reports";
				}
				else if (_mainWindowViewModel.ReportsViewModel.ReportOption == enumReportOptions.MockRun)
				{
					target += "-Mock-run";
				}
			}
			else if (true == tabItem?.Header.Equals(s = (string)FindResource("MaintenanceTabHeader")))
			{
				string baseTarget = s.Replace(" ", "-");
				var item = MaintenanceTabs.SelectedItem as TabItem;

				if (-1 == MaintenanceTabs.SelectedIndex || true == item?.Header.Equals(s = (string)FindResource("MaintenanceDonorTabHeader")))
				{
					target = baseTarget + '-' + s.Replace(" ", "-");
				}
				else if (true == item?.Header.Equals(s = (string)FindResource("MaintenanceCategoryTabHeader")))
				{
					target = baseTarget + '-' + s.Replace(" ", "-");
				}
				else if (true == item?.Header.Equals(s = (string)FindResource("MaintenanceCategoryMapTabHeader")))
				{
					target = baseTarget + '-' + s.Replace(" ", "-");
				}
				else if (true == item?.Header.Equals(s = (string)FindResource("MaintenanceDonorMapTabHeader")))
				{
					target = baseTarget + '-' + s.Replace(" ", "-");
				}
				else if (true == item?.Header.Equals(s = (string)FindResource("MaintenanceDesignTitheEnvelopeTabHeader")))
				{
					target = baseTarget + '-' + s.Replace(" ", "-");
				}
				else if (true == item?.Header.Equals(s = (string)FindResource("MaintenanceGeneralTabHeader")))
				{
					target = baseTarget + '-' + s.Replace(" ", "-");
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

	private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		_mainWindowViewModel = DataContext as MainWindowViewModel;
	}
}
