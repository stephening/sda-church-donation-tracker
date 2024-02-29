using Donations.Lib.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IDonorMapServices
{
	ObservableCollection<AGDonorMapItem>? AGDonorMapList { get; set; }
	Dictionary<string, AGDonorMapItem>? AGDonorMap { get; set; }
	Task<string?> SaveDonorMap(ObservableCollection<AGDonorMapItem> donorMapList, bool force = false);
}
