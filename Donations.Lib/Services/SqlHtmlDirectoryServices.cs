using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System.Data.SqlClient;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Data;
using Dapper;
using System.Threading;

namespace Donations.Lib.Services;

public class SqlHtmlDirectoryServices : SqlHelper, IHtmlDirectoryServices
{
	private string _htmlDirectoryDbName => "dbo.HtmlDirectory";
	private HtmlDirectory? _htmlDirectory;
	private readonly SqlCreateTables _sqlCreateTables;
	private ManualResetEvent _waitHandle = new ManualResetEvent(false);

	public SqlHtmlDirectoryServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables
	) : base(logger)
	{
		_sqlCreateTables = sqlCreateTables;
		// don't await because it can take some time
		Task.Run(() => ReadFromDatabase());
	}

	private async Task ReadFromDatabase()
	{
		try
		{
			var ret = SelectFromTable<HtmlDirectory>(_htmlDirectoryDbName);

			if (0 < ret.Count)
			{
				_htmlDirectory = ret.Single();
			}
			else
			{
				_htmlDirectory = new HtmlDirectory();
			}
			_waitHandle.Set();
		}
		catch (SqlException ex)
		{
			_htmlDirectory = new HtmlDirectory();
			if (208 == ex.Number || ex.Message.Contains("Invalid object name"))
			{
				_logger.Error($"Html Directory table probably didn't exist, try creating it. Exception: {ex.Message}");
				await _sqlCreateTables!.CreateHtmlDirectoryTable();
			}
		}
		catch (Exception ex)
		{
			_logger.Error($"Exception deserializing app settings. Exception: {ex.Message}");
			_htmlDirectory = new HtmlDirectory();
		}
	}

	public async Task<HtmlDirectory> GetAsync()
	{
		await Task.Run(() => _waitHandle.WaitOne());
		return _htmlDirectory!;
	}

	public async Task<int> Save()
	{
		// write the new T record
		try
		{
			await DropTable(_htmlDirectoryDbName);

			using IDbConnection conn = new SqlConnection(_connString);
			{
				await _sqlCreateTables.CreateHtmlDirectoryTable();

				var properties = Helper.PublicProperties<HtmlDirectory>().ToList();
				properties.Remove("Id");
				string command = "";
				command += $"INSERT INTO {_htmlDirectoryDbName} ({string.Join(',', properties)}) OUTPUT INSERTED.Id VALUES ({string.Join(',', properties.Select(x => "@" + x).ToList())})";

				int id = await conn.ExecuteScalarAsync<int>(command, _htmlDirectory);
				return id;
			}
		}
		catch (Exception ex)
		{
			_logger.Error($"Exception serializing Html Directory. Exception: {ex.Message}");
		}

		return -1;
	}
}
