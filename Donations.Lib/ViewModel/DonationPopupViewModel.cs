using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace Donations.Lib.ViewModel;

public partial class DonationPopupViewModel : BaseViewModel
{
	private ObservableCollection<Donation>? _donations;

	[ObservableProperty]
	private double _totalDonation;

	[ObservableProperty]
	private CollectionViewSource _donationSource = new CollectionViewSource();

	public void Initialize(ObservableCollection<Donation>? donations)
	{
		_donations = donations;

		DonationSource.Source = _donations;

#pragma warning disable CS8604 // Possible null reference argument.
		TotalDonation = _donations.Sum(x => x.Value);
#pragma warning restore CS8604 // Possible null reference argument.
	}
}
