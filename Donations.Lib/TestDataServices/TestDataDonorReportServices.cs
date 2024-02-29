using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataDonorReportServices : IDonorReportServices
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	private ObservableCollection<DonorReport> _donorReports;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	private ObservableCollection<NamedDonorReport> _namedDonorReports;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task DeleteDonorReport(int DonorId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		_donorReports.Remove(_donorReports.Where(x => x.DonorId == DonorId).First());
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<DonorReport>> Load()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		var td = new TestData();

#pragma warning disable CS8601 // Possible null reference assignment.
		_donorReports = td.donorReports;
#pragma warning restore CS8601 // Possible null reference assignment.

#pragma warning disable CS8603 // Possible null reference return.
		return _donorReports;
#pragma warning restore CS8603 // Possible null reference return.
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<NamedDonorReport>> LoadNamed()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		var td = new TestData();

#pragma warning disable CS8601 // Possible null reference assignment.
		_namedDonorReports = td.namedDonorReports;
#pragma warning restore CS8601 // Possible null reference assignment.

#pragma warning disable CS8603 // Possible null reference return.
		return _namedDonorReports;
#pragma warning restore CS8603 // Possible null reference return.
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string?> Save(ObservableCollection<DonorReport> reports)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		_donorReports = reports;
		return null;
	}
}
