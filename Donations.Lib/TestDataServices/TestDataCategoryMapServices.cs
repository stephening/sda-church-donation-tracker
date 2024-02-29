using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataCategoryMapServices : ICategoryMapServices
{
	public ObservableCollection<AGCategoryMapItem>? AGCategoryMapList { get; set; }
	public Dictionary<int, AGCategoryMapItem>? AGCategoryMap { get; set; }

	public TestDataCategoryMapServices()
	{
		AGCategoryMapList = new ObservableCollection<AGCategoryMapItem>();
		AGCategoryMap = new Dictionary<int, AGCategoryMapItem>();
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string?> SaveCategoryMap(ObservableCollection<AGCategoryMapItem> categoryMapList, bool force = false)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return null;
	}
}
