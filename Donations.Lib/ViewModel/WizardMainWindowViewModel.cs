using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Model;
using Donations.Lib.View;
using Donations.Lib.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace Donations.Lib.ViewModel;

public enum WizardPages
{
	Introduction,
	SqlHostChoice,
	SqlCloudInstall,
	SqlLocalInstall,
	SqlSpecifyConnectionString,
	CreateTables,
	SpecifyChurchLogo,
	ImportCategories,
	ImportDonors,
	ImportDonations,
	Finished,

	NumberOfPages
}

public partial class WizardMainWindowViewModel : ObservableObject
{
	private bool _importOnly = false;

	public delegate WizardMainWindowViewModel Factory(bool donationsApp);

	public WizardMainWindowViewModel(
		WizardSqlChoiceViewModel wizardSqlChoiceViewModel,
		WizardSqlConnectViewModel wizardSqlConnectViewModel,
		WizardSpecifyConnectionStringViewModel wizardSpecifyConnectionStringViewModel,
		WizardSpecifyLogoViewModel wizardSpecifyLogoViewModel,
		WizardImportCategoriesViewModel importCategoriesViewModel,
		WizardImportDonorsViewModel importDonorsViewModel,
		WizardImportDonationsViewModel importDonationsViewModel,
		bool donationsApp
		)
	{
		DonationsApp = donationsApp;
		wizardSqlChoiceViewModel.Init(this);
		WizardSqlChoiceViewModelDataContext = wizardSqlChoiceViewModel;
		WizardSqlConnectViewModelDataContext = wizardSqlConnectViewModel;
		WizardSpecifyConnectionStringViewModelDataContext = wizardSpecifyConnectionStringViewModel;
		WizardSpecifyLogoViewModelDataContext = wizardSpecifyLogoViewModel;
		WizardImportCategoriesViewModelDataContext = importCategoriesViewModel;
		WizardImportDonorsViewModelDataContext = importDonorsViewModel;
		WizardImportDonationsViewModelDataContext = importDonationsViewModel;

		if (donationsApp)
		{
			// donations app
			_pageMap[WizardPages.Introduction] = 0;
			_pageMap[WizardPages.SqlHostChoice] = 1;
			_pageMap[WizardPages.SqlCloudInstall] = 2;
			_pageMap[WizardPages.SqlLocalInstall] = 3;
			_pageMap[WizardPages.SqlSpecifyConnectionString] = 4;
			_pageMap[WizardPages.CreateTables] = 5;
			_pageMap[WizardPages.SpecifyChurchLogo] = 6;
			_pageMap[WizardPages.ImportCategories] = 7;
			_pageMap[WizardPages.ImportDonors] = 8;
			_pageMap[WizardPages.ImportDonations] = 9;
			_pageMap[WizardPages.Finished] = 10;
		}
		else
		{
			// member maintenance app
			_pageMap[WizardPages.Introduction] = 0;
			_pageMap[WizardPages.SqlHostChoice] = 1;
			_pageMap[WizardPages.SqlCloudInstall] = 2;
			_pageMap[WizardPages.SqlLocalInstall] = 3;
			_pageMap[WizardPages.SqlSpecifyConnectionString] = 4;
			_pageMap[WizardPages.Finished] = 5;
		}
	}

	public Dictionary<WizardPages, int> _pageMap = new Dictionary<WizardPages, int>();

	public int WizardPageIndex => _pageMap[TabPage];

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(WizardPageIndex))]
	[NotifyCanExecuteChangedFor(nameof(BackCommand))]
	[NotifyCanExecuteChangedFor(nameof(NextCommand))]
	private WizardPages _tabPage = WizardPages.Introduction;

	public bool DonationsApp { get; }
	public WizardSqlChoiceViewModel WizardSqlChoiceViewModelDataContext { get; }
	public WizardSqlConnectViewModel WizardSqlConnectViewModelDataContext { get; }
	public WizardSpecifyConnectionStringViewModel WizardSpecifyConnectionStringViewModelDataContext { get; }
	public WizardSpecifyLogoViewModel WizardSpecifyLogoViewModelDataContext { get; }
	public WizardImportCategoriesViewModel WizardImportCategoriesViewModelDataContext { get; }
	public WizardImportDonorsViewModel WizardImportDonorsViewModelDataContext { get; }
	public WizardImportDonationsViewModel WizardImportDonationsViewModelDataContext { get; }

	public void Import()
	{
		_importOnly = true;
		TabPage = WizardPages.ImportCategories;
	}

	public void NotifyNext()
	{
		NextCommand.NotifyCanExecuteChanged();
	}

	[RelayCommand]
	public void Quit()
	{
		Application.Current.Shutdown();
	}

	[RelayCommand(CanExecute = nameof(CanGoBack))]
	public void Back()
	{
		if (WizardPages.Introduction < TabPage)
		{
			if (WizardPages.SqlSpecifyConnectionString == TabPage)
			{
				if (WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.Local)
				{
					TabPage = WizardPages.SqlLocalInstall;
				}
				else if (WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.Cloud)
				{
					TabPage = WizardPages.SqlCloudInstall;
				}
				else if (WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.ConnStringOnly)
				{
					TabPage = WizardPages.SqlHostChoice;
				}
			}
			else if (WizardPages.Finished == TabPage)
			{
				if (WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.Local)
				{
					TabPage = WizardPages.ImportDonations;
				}
				else if (WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.Cloud)
				{
					TabPage = WizardPages.ImportDonations;
				}
				else if (WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.ConnStringOnly)
				{
					TabPage = WizardPages.SqlSpecifyConnectionString;
				}
			}
			else if (WizardPages.SqlLocalInstall == TabPage || WizardPages.SqlCloudInstall == TabPage)
			{
				TabPage = WizardPages.SqlHostChoice;
			}
			else
				TabPage--;
		}
	}

	private bool CanGoBack()
	{
		return _importOnly ? WizardPages.ImportCategories < TabPage : WizardPages.Introduction < TabPage;
	}

	[RelayCommand(CanExecute = nameof(CanGoNext))]
	public void Next()
	{
		if (WizardPages.NumberOfPages > TabPage)
		{
			if (WizardPages.SqlHostChoice == TabPage)
			{
				if (WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.Local)
				{
					TabPage = WizardPages.SqlLocalInstall;
				}
				else if (WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.Cloud)
				{
					TabPage = WizardPages.SqlCloudInstall;
				}
				else if (WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.ConnStringOnly)
				{
					TabPage = WizardPages.SqlSpecifyConnectionString;
				}
				else if (WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.Import)
				{
					TabPage = WizardPages.ImportCategories;
				}
			}
			else if (WizardPages.SqlSpecifyConnectionString == TabPage && WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.ConnStringOnly)
			{
				TabPage = WizardPages.Finished;
			}
			else if (WizardPages.SqlLocalInstall == TabPage || WizardPages.SqlCloudInstall == TabPage)
			{
				TabPage = WizardPages.SqlSpecifyConnectionString;
			}
			else
				TabPage++;
		}
	}

	private bool CanGoNext()
	{
		return WizardPages.NumberOfPages - 1 > TabPage
			&& (WizardPages.SqlHostChoice != TabPage
				|| WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.ConnStringOnly
				|| WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.Local
				|| WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.Cloud
				|| WizardSqlChoiceViewModelDataContext.SqlChoice == enumSqlChoiceOptions.Import
				);
	}

	[RelayCommand]
	public void Finish()
	{
		Application.Current.Shutdown();
	}
}
