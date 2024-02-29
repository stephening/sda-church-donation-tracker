using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

namespace Donations.Lib.ViewModel;

public partial class ConfirmDonorMergeViewModel : BaseViewModel
{
	public CollectionViewSource Donations { get; set; } = new CollectionViewSource();
	public string? MergeFrom => _mergeFrom?.Name;
	public string? MergeTo => _mergeTo.Name;

	[ObservableProperty]
	private Visibility _gridVisibility;

	public string? Message
	{
		get
		{
			string ret = "";

			if (null != _donations && 0 < _donations.Count)
			{
				ret = $"{MergeFrom} has the following donations. If you continue with the merge, these donations will be transferred to {MergeTo}.";
				GridVisibility = Visibility.Visible;
			}
			else
			{
				GridVisibility = Visibility.Collapsed;
			}

			return ret;
		}
	}

	private ObservableCollection<Donation>? _donations;
	private Donor? _mergeFrom;
	private Donor? _mergeTo;

	public ConfirmDonorMergeViewModel()
	{

	}

	public void Init(Donor mergeTo, Donor mergeFrom, ObservableCollection<Donation> donations)
	{
		_mergeTo = mergeTo;
		_mergeFrom = mergeFrom;
		Donations.Source = _donations = donations;
		Donations.View.Refresh();

		OnPropertyChanged(nameof(MergeTo));
		OnPropertyChanged(nameof(MergeFrom));
		OnPropertyChanged(nameof(Message));
	}
}
