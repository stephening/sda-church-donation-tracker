using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IDonorChangeServices
{
	Task<ObservableCollection<DonorChange>> Load();
	Task<ObservableCollection<DonorChange>> GetDonorChangesByDonorId(int donorId);
	Task<string?> Save(ObservableCollection<DonorChange> changes);
	Task<int?> Save(DonorChange change);
}
