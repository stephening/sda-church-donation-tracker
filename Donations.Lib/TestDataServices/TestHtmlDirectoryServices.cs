using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestHtmlDirectoryServices : IHtmlDirectoryServices
{
	private HtmlDirectory? _htmlDirectory;

	public TestHtmlDirectoryServices()
	{
		var td = new TestData();
		_htmlDirectory = td.HtmlDirectory;
	}

	public async Task<HtmlDirectory> GetAsync()
	{
		return _htmlDirectory!;
	}

	public async Task<int> Save()
	{
		return 11;
	}
}
