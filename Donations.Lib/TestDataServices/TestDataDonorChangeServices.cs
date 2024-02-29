using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataDonorChangeServices : IDonorChangeServices
{
	private ObservableCollection<DonorChange> _donorChanges = new ObservableCollection<DonorChange>();

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<DonorChange>> GetDonorChangesByDonorId(int donorId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return new ObservableCollection<DonorChange>(_donorChanges.Where(x => x.DonorId == donorId));
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<DonorChange>> Load()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		var td = new TestData();
#pragma warning disable CS8601 // Possible null reference assignment.
		_donorChanges = td.donorChanges;
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning disable CS8603 // Possible null reference return.
		return _donorChanges;
#pragma warning restore CS8603 // Possible null reference return.
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string?> Save(ObservableCollection<DonorChange> changes)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		throw new System.NotImplementedException();
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<int?> Save(DonorChange change)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		throw new System.NotImplementedException();
	}
}
