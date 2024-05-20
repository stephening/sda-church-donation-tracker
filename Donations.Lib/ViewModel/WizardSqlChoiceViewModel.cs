using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Model;
using System.Windows;

namespace Donations.Lib.ViewModel;

public partial class WizardSqlChoiceViewModel : ObservableObject
{
	[ObservableProperty]
	private enumSqlChoiceOptions _sqlChoice;
	[ObservableProperty]
	private Visibility _donationsVisibility = Visibility.Visible;
	[ObservableProperty]
	private bool _donationsEnabled = true;

	partial void OnSqlChoiceChanged(enumSqlChoiceOptions value)
	{
		if (enumSqlChoiceOptions.Unspecified < value)
		{
			_wizardMainWindowViewModel?.NotifyNext();
		}
	}

	private WizardMainWindowViewModel? _wizardMainWindowViewModel;

	public void Init(WizardMainWindowViewModel wizardMainWindowViewModel)
	{
		_wizardMainWindowViewModel = wizardMainWindowViewModel;
		if (_wizardMainWindowViewModel.DonationsApp)
		{
			DonationsVisibility = Visibility.Visible;
			DonationsEnabled = true;
		}
		else
		{
			DonationsVisibility = Visibility.Hidden;
			DonationsEnabled = false;
		}
	}
}
