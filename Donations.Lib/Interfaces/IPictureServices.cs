using Donations.Lib.Model;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IPictureServices
{
	Picture GetLogo();
	Task<int> SaveLogo(Picture picture);
}
