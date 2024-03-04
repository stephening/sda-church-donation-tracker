using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataTitheEnvelopeServices : ITitheEnvelopeServices
{
	public ObservableCollection<EnvelopeEntry>? TitheEnvelopeDesign { get; set; }

	public TestDataTitheEnvelopeServices()
	{
		var td = new TestData();
		TitheEnvelopeDesign = td.TitheEnvelopeDesign;
	}

	public async Task<string?> SaveTitheEnvelopeDesign(ObservableCollection<EnvelopeEntry> envelope, bool force = false)
	{
		return null;
	}
}
