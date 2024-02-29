using Donations.Lib.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface ICategoryMapServices
{
	ObservableCollection<AGCategoryMapItem>? AGCategoryMapList { get; set; }
	Dictionary<int, AGCategoryMapItem>? AGCategoryMap { get; set; }
	Task<string?> SaveCategoryMap(ObservableCollection<AGCategoryMapItem> categoryMapList, bool force = false);
}
