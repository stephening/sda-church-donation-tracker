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

	public async Task<string?> SaveCategoryMap(ObservableCollection<AGCategoryMapItem> categoryMapList, bool force = false)
	{
		return null;
	}
}
