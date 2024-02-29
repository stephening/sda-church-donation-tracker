using Dapper;
using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlBatchServices : SqlHelper, IBatchServices
{
	private readonly SqlCreateTables _sqlCreateTables;

	private string _batchesDbName => "dbo.Batches";

	public SqlBatchServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables
	)
		: base(logger)
	{
		_sqlCreateTables = sqlCreateTables;
	}

	public async Task<ObservableCollection<Batch>> LoadBatches()
	{
		try
		{
			return await SelectFromTableAsync<Batch>(_batchesDbName);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, "Exception caught while deserializing {_batchesDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task<string?> SaveBatches(ObservableCollection<Batch> batches, bool force = false, Action<long, long>? progUpdate = null)
	{
		try
		{
			// first delete existing rows
			await DropTable(_batchesDbName);

			await _sqlCreateTables!.CreateBatchTable();

			return await WriteEntireTableAsync<Batch>(true, batches, _batchesDbName, force, progUpdate);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while serializing {_batchesDbName}.");
			return ex.Message;
		}
	}

	public async Task<ObservableCollection<Batch>> FilterBatch(enumDateFilterOptions batchFilter, string date, string date2)
	{
		string where = "";

		switch (batchFilter)
		{
			case enumDateFilterOptions.CurrentYear:
			case enumDateFilterOptions.PreviousYear:
			case enumDateFilterOptions.SelectYear:
				where = $"WHERE Date LIKE '{date}%'";
				break;
			case enumDateFilterOptions.DateRange:
				where = $"WHERE Date >= '{date}' AND '{date2}' >= Date";
				break;
		}

		try
		{
			return await SelectFromTableAsync<Batch>(_batchesDbName, where: where);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while deserializing {_batchesDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task<ObservableCollection<string>> GetBatchYears()
	{
		try
		{
#pragma warning disable CS0168 // Variable is declared but never used
			ObservableCollection<string>? ret;
#pragma warning restore CS0168 // Variable is declared but never used

			using IDbConnection conn = new SqlConnection(_connString);
			{
				string query = $"SELECT DISTINCT YEAR(Date) FROM {_batchesDbName}";
				var enumerable = await conn.QueryAsync<string>(query);
				return new ObservableCollection<string>(enumerable);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught getting batch years from {_batchesDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task<string> GetEarliestDate()
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				string query = $"SELECT Date FROM {_batchesDbName} WHERE Date IN(SELECT min(Date) FROM {_batchesDbName})";
				var enumerable = await conn.QueryAsync<string>(query);
				return enumerable.First();
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught getting earliest date from {_batchesDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task<string> GetLatestDate()
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				string query = $"SELECT Date FROM {_batchesDbName} WHERE Date IN(SELECT max(Date) FROM {_batchesDbName})";
				var enumerable = await conn.QueryAsync<string>(query);
				return enumerable.First();
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught getting most recent date from {_batchesDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task DeleteBatch(int batchId)
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				string query = $"Delete FROM {_batchesDbName} WHERE Id = {batchId}";
				await conn.ExecuteAsync(query);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught deleting batch from {_batchesDbName}.");
		}
	}

	public async Task<int> AddBatch(Batch batch)
	{
		try
		{
			return await Add<Batch>(batch, _batchesDbName);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught adding batch to {_batchesDbName}.");
			return -1;
		}
	}

	public async Task UpdateBatch(Batch batch)
	{
		try
		{
			// update the batch record by id
			await Update<Batch>(batch, _batchesDbName, batch.Id);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught updating batch id {batch.Id} in {_batchesDbName}.");
		}
	}

	public async Task<Batch?> GetBatchById(int id)
	{
		try
		{
			var ret = await SelectFromTableAsync<Batch>(_batchesDbName, where: $"WHERE Id = '{id}'");

			return ret?.Single();
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught getting batch id {id} from {_batchesDbName}.");
			return null;
		}
	}
}
