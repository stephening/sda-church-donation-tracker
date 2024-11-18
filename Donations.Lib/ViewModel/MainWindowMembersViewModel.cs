using System.Collections.Generic;
using System.Threading.Tasks;

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

	public async Task Shutdown()
	{
		List<Task> tasks = new List<Task>();

		tasks.Add(DonorViewModel!.Leaving());
		tasks.Add(DirectoryViewModel!.Leaving());

		await Task.WhenAll(tasks);
	}

}
