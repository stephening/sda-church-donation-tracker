using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataAppSettingsServices : IAppSettingsServices
{
	private AppSettings _appSettings;

	public TestDataAppSettingsServices()
	{
		var td = new TestData();
		_appSettings = td.AppSettings;
	}
	public AppSettings Get()
	{
		return _appSettings;
	}

	public async Task<int> Save()
	{
		return _appSettings.Id;
	}
}
