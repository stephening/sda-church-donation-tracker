using Dapper;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlAppSettingsServices : SqlHelper, IAppSettingsServices
{
	private string _appSettingsDbName => "dbo.AppSettings";
	private AppSettings _appSettings;
	private readonly SqlCreateTables _sqlCreateTables;

	public SqlAppSettingsServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables
	) : base(logger)
	{
		try
		{
			var ret = SelectFromTable<AppSettings>(_appSettingsDbName);

			if (0 < ret.Count)
			{
				_appSettings = ret.Single();
			}
			else
			{
				_appSettings = new AppSettings();
			}
		}
		catch (SqlException ex)
		{
			_appSettings = new AppSettings();
			if (208 == ex.Number || ex.Message.Contains("Invalid object name"))
			{
				_logger.Error($"App settings table probably didn't exist, try creating it. Exception: {ex.Message}");
				sqlCreateTables!.CreateAppSettingsTable();
			}
		}
		catch (Exception ex)
		{
			_logger.Error($"Exception deserializing app settings. Exception: {ex.Message}");
			_appSettings = new AppSettings();
		}
		_sqlCreateTables = sqlCreateTables;
	}

	public AppSettings Get()
	{
		return _appSettings;
	}

	public async Task<int> Save()
	{
		// write the new T record
		try
		{
			await DropTable(_appSettingsDbName);

			using IDbConnection conn = new SqlConnection(_connString);
			{
				await _sqlCreateTables.CreateAppSettingsTable();

				var properties = Helper.PublicProperties<AppSettings>().ToList();
				properties.Remove("Id");
				string command = "";
				command += $"INSERT INTO {_appSettingsDbName} ({string.Join(',', properties)}) OUTPUT INSERTED.Id VALUES ({string.Join(',', properties.Select(x => "@" + x).ToList())})";

				int id = await conn.ExecuteScalarAsync<int>(command, _appSettings);
				return id;
			}
		}
		catch (Exception ex)
		{
			_logger.Error($"Exception serializing app settings. Exception: {ex.Message}");
		}

		return -1;
	}
}
