using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Model;
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

	public WizardMainWindowViewModel(
		WizardSqlChoiceViewModel wizardSqlChoiceViewModel,
		WizardSqlConnectViewModel wizardSqlConnectViewModel,
		WizardSpecifyConnectionStringViewModel wizardSpecifyConnectionStringViewModel,
		WizardSpecifyLogoViewModel wizardSpecifyLogoViewModel,
		WizardImportCategoriesViewModel importCategoriesViewModel,
		WizardImportDonorsViewModel importDonorsViewModel,
		WizardImportDonationsViewModel importDonationsViewModel
		)
	{
		wizardSqlChoiceViewModel.Init(this);
		WizardSqlChoiceViewModelDataContext = wizardSqlChoiceViewModel;
		WizardSqlConnectViewModelDataContext = wizardSqlConnectViewModel;
		WizardSpecifyConnectionStringViewModelDataContext = wizardSpecifyConnectionStringViewModel;
		WizardSpecifyLogoViewModelDataContext = wizardSpecifyLogoViewModel;
		WizardImportCategoriesViewModelDataContext = importCategoriesViewModel;
		WizardImportDonorsViewModelDataContext = importDonorsViewModel;
		WizardImportDonationsViewModelDataContext = importDonationsViewModel;
	}

	public int WizardPageIndex => (int)TabPage;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(WizardPageIndex))]
	[NotifyCanExecuteChangedFor(nameof(BackCommand))]
	[NotifyCanExecuteChangedFor(nameof(NextCommand))]
	private WizardPages _tabPage = WizardPages.Introduction;

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
