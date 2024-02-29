using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Donations.Lib.ViewModel;

public partial class HelpNavigationViewModel : ObservableObject
{
	[ObservableProperty]
	private string? _label;

	[ObservableProperty]
	private string? _target;

	[ObservableProperty]
	private int? _level;

	public ObservableCollection<HelpNavigationViewModel>? Children { get; set; } = new ObservableCollection<HelpNavigationViewModel>();
}
