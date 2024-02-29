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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<int> Save()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return _appSettings.Id;
	}
}
