using Donations.Lib.Model;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IPdfDirectoryServices
{
	Task<PdfDirectory> GetAsync();
	Task<int> Save();
}
