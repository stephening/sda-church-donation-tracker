using Donations.Lib.Model;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IHtmlDirectoryServices
{
	Task<HtmlDirectory> GetAsync();
	Task<int> Save();
}
