using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Model;

namespace Donations.Lib.ViewModel;

public partial class WizardSqlChoiceViewModel : ObservableObject
{
	[ObservableProperty]
	private enumSqlChoiceOptions _sqlChoice;

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
	}
}
