using Donations.Lib.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface ICategoryServices
{
	Dictionary<int, Category>? CatDict { get; set; }
	ObservableCollection<Category>? CatList { get; set; }
	Task<string?> SaveCategories(ObservableCollection<Category> categories, bool force = false);
	void ReplaceCategoryData(ObservableCollection<Category> categoryList);
	string GetCategoryDescription(int code);
}
