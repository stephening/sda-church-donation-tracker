using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataDonorChangeServices : IDonorChangeServices
{
	private ObservableCollection<DonorChange> _donorChanges = new ObservableCollection<DonorChange>();

	public async Task<ObservableCollection<DonorChange>> GetDonorChangesByDonorId(int donorId)
	{
		return new ObservableCollection<DonorChange>(_donorChanges.Where(x => x.DonorId == donorId));
	}

	public async Task<ObservableCollection<DonorChange>> Load()
	{
		var td = new TestData();
		_donorChanges = td.donorChanges;
		return _donorChanges;
	}

	public async Task<string?> Save(ObservableCollection<DonorChange> changes)
	{
		throw new System.NotImplementedException();
	}

	public async Task<int?> Save(DonorChange change)
	{
		throw new System.NotImplementedException();
	}
}
