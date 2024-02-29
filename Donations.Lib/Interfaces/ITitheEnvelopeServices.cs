using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface ITitheEnvelopeServices
{
	ObservableCollection<EnvelopeEntry>? TitheEnvelopeDesign { get; set; }
	Task<string?> SaveTitheEnvelopeDesign(ObservableCollection<EnvelopeEntry> envelope, bool force = false);
}
