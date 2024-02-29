using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataCategoryServices : ICategoryServices
{
	public Dictionary<int, Category>? CatDict { get; set; }
	public ObservableCollection<Category>? CatList { get; set; }

	public TestDataCategoryServices()
	{
		var td = new TestData();
		CatDict = td.CatDict;
		CatList = td.CatList;
	}

	public void ReplaceCategoryData(ObservableCollection<Category> categoryList)
	{
		CatList = new ObservableCollection<Category>(categoryList);
		CatDict!.Clear();
		foreach (var category in CatList)
		{
			CatDict[category.Code] = category;
		}
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string?> SaveCategories(ObservableCollection<Category> categories, bool force = false)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		CatList = new ObservableCollection<Category>(categories);
		CatDict!.Clear();
		foreach (var category in CatList)
		{
			CatDict[category.Code] = category;
		}

		return null;
	}

	public string GetCategoryDescription(int code)
	{
		return (null != CatDict && CatDict.ContainsKey(code)) ? CatDict[code].Description : "";
	}
}
