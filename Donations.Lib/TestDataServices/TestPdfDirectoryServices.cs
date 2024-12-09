using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestPdfDirectoryServices : IPdfDirectoryServices
{
	private PdfDirectory? _pdfDirectory;

	public TestPdfDirectoryServices()
    {
		var td = new TestData();
		_pdfDirectory = td.PdfDirectory;
	}

	public async Task<PdfDirectory> GetAsync()
	{
		return _pdfDirectory!;
	}

	public async Task<int> Save()
	{
		return 11;
	}
}
