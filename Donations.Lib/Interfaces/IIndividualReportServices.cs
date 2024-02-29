using Donations.Lib.Model;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IIndividualReportServices
{
	Task<IndividualReport?> Get();
	Task Save(IndividualReport individualReport);
}
