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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string?> SaveTitheEnvelopeDesign(ObservableCollection<EnvelopeEntry> envelope, bool force = false)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return null;
	}
}
