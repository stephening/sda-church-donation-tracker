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

public class SqlPdfDirectoryServices : SqlHelper, IPdfDirectoryServices
{
	private string _pdfDirectoryDbName => "dbo.PdfDirectory";
	private PdfDirectory? _pdfDirectory;
	private readonly SqlCreateTables _sqlCreateTables;
	private ManualResetEvent _waitHandle = new ManualResetEvent(false);

	public SqlPdfDirectoryServices(
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
			var ret = SelectFromTable<PdfDirectory>(_pdfDirectoryDbName);

			if (0 < ret.Count)
			{
				_pdfDirectory = ret.Single();
			}
			else
			{
				_pdfDirectory = new PdfDirectory();
			}
			_waitHandle.Set();
		}
		catch (SqlException ex)
		{
			_pdfDirectory = new PdfDirectory();
			if (208 == ex.Number || ex.Message.Contains("Invalid object name"))
			{
				_logger.Error($"Pdf Directory table probably didn't exist, try creating it. Exception: {ex.Message}");
				await _sqlCreateTables!.CreatePdfDirectoryTable();
			}
		}
		catch (Exception ex)
		{
			_logger.Error($"Exception deserializing app settings. Exception: {ex.Message}");
			_pdfDirectory = new PdfDirectory();
		}
	}

	public async Task<PdfDirectory> GetAsync()
	{
		await Task.Run(() => _waitHandle.WaitOne());
		return _pdfDirectory!;
	}

	public async Task<int> Save()
	{
		// write the new T record
		try
		{
			await DropTable(_pdfDirectoryDbName);

			using IDbConnection conn = new SqlConnection(_connString);
			{
				await _sqlCreateTables.CreatePdfDirectoryTable();

				var properties = Helper.PublicProperties<PdfDirectory>().ToList();
				properties.Remove("Id");
				string command = "";
				command += $"INSERT INTO {_pdfDirectoryDbName} ({string.Join(',', properties)}) OUTPUT INSERTED.Id VALUES ({string.Join(',', properties.Select(x => "@" + x).ToList())})";

				int id = await conn.ExecuteScalarAsync<int>(command, _pdfDirectory);
				return id;
			}
		}
		catch (Exception ex)
		{
			_logger.Error($"Exception serializing Pdf Directory. Exception: {ex.Message}");
		}

		return -1;
	}
}
