using Donations.Lib.Model;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IAppSettingsServices
{
	AppSettings Get();
	Task<int> Save();
}
