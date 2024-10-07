namespace Donations.Lib.ViewModel;

public class MainWindowMembersViewModel
{
    public MainWindowMembersViewModel(
		DonorViewModel donorViewModel,
		DirectoryViewModel directoryViewModel
	)
	{
		DonorViewModel = donorViewModel;
		DirectoryViewModel = directoryViewModel;
	}

	public DonorViewModel DonorViewModel { get; }
	public DirectoryViewModel? DirectoryViewModel { get; }
}
