using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IDonorReportServices
{
	Task<ObservableCollection<DonorReport>> Load();
	Task<ObservableCollection<NamedDonorReport>> LoadNamed();
	Task<string?> Save(ObservableCollection<DonorReport> reports);
	Task DeleteDonorReport(int DonorId);
}
