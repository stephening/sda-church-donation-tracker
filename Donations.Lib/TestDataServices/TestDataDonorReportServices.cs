using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataDonorReportServices : IDonorReportServices
{
	private ObservableCollection<DonorReport> _donorReports;
	private ObservableCollection<NamedDonorReport> _namedDonorReports;

	public async Task DeleteDonorReport(int DonorId)
	{
		_donorReports.Remove(_donorReports.Where(x => x.DonorId == DonorId).First());
	}

	public async Task<ObservableCollection<DonorReport>> Load()
	{
		var td = new TestData();

		_donorReports = td.donorReports;

		return _donorReports;
	}

	public async Task<ObservableCollection<NamedDonorReport>> LoadNamed()
	{
		var td = new TestData();

		_namedDonorReports = td.namedDonorReports;

		return _namedDonorReports;
	}

	public async Task<string?> Save(ObservableCollection<DonorReport> reports)
	{
		_donorReports = reports;
		return null;
	}
}
