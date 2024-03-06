using Donations.Lib.ViewModel;

namespace Donors;

public class MainWindowViewModel
{
	public MainWindowViewModel(
		DonorViewModel donorViewModel
	)
	{
		DonorViewModel = donorViewModel;
	}

	public DonorViewModel DonorViewModel { get; }
}
