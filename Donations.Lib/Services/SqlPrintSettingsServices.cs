using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlPrintSettingsServices : SqlHelper, IPrintSettingsServices
{
	private readonly SqlCreateTables _sqlCreateTables;

	private string _dbName => "dbo.PrintSettings";

	public SqlPrintSettingsServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables
	) : base(logger)
	{
		_sqlCreateTables = sqlCreateTables;
	}

	public PrintSettings? Get(enumPrintout printoutType)
	{
		try
		{
			var settings = SelectFromTable<PrintSettings>(_dbName, where: $"WHERE PrintoutType = '{(int)printoutType}'");
			if (null != settings && settings.Count > 0)
			{
				return settings[0];
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception thrown while reading {_dbName}");
		}
		return new PrintSettings() { PrintoutType = (int)printoutType, FontFamily = "Calibri", FontSize = 12, LeftMargin = 0.5, OtherMargins = 0.5 };
	}

	public async Task<string?> Save(PrintSettings printSettings)
	{
		try
		{
			await _sqlCreateTables.CreateenumPrintoutTable();
			await _sqlCreateTables.CreatePrintSettingsTable();

			return Update<PrintSettings>(printSettings, _dbName, "PrintoutType", (int)printSettings.PrintoutType);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Unexpected error saving PrintSettings data to {_dbName}.");
		}

		return null;
	}
}
