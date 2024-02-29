using Donations.Lib.Model;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IPrintSettingsServices
{
	PrintSettings? Get(enumPrintout printoutType);
	Task<string?> Save(PrintSettings printSettings);
}
