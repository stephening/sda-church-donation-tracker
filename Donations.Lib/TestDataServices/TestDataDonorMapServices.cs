using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataDonorMapServices : IDonorMapServices
{
	public ObservableCollection<AGDonorMapItem>? AGDonorMapList { get; set; }
	public Dictionary<string, AGDonorMapItem>? AGDonorMap { get; set; }

	public TestDataDonorMapServices()
	{
		AGDonorMapList = new ObservableCollection<AGDonorMapItem>();
		AGDonorMap = new Dictionary<string, AGDonorMapItem>();
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string?> SaveDonorMap(ObservableCollection<AGDonorMapItem> donorMapList, bool force = false)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return null;
	}
}
