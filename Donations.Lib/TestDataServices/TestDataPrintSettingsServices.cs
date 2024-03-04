using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataPrintSettingsServices : IPrintSettingsServices
{
	private Dictionary<enumPrintout, PrintSettings>? _printSettingsDict;

	public TestDataPrintSettingsServices()
	{
		var td = new TestData();
		_printSettingsDict = td.PrintSettingsDict;
	}
	public PrintSettings? Get(enumPrintout printoutType)
	{
		return _printSettingsDict?[printoutType];
	}

	public async Task<string?> Save(PrintSettings printSettings)
	{
		if (null != _printSettingsDict)
		{
			_printSettingsDict[(enumPrintout)printSettings.PrintoutType] = printSettings;
		}

		return null;
	}
}
