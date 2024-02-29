using Dapper;
using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlDonationServices : SqlHelper, IDonationServices
{
	private readonly SqlCreateTables _sqlCreateTables;

	private string _donationsDbName => "dbo.Donations";

	public SqlDonationServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables
	)
		: base(logger)
	{
		_sqlCreateTables = sqlCreateTables;
	}

	public async Task<ObservableCollection<Donation>> LoadDonations()
	{
		try
		{
			return await SelectFromTableAsync<Donation>(_donationsDbName);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while deserializing {_donationsDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task<string?> SaveDonations(ObservableCollection<Donation> donations, bool force = false, Action<long, long>? progUpdate = null)
	{
		try
		{
			await DropTable(_donationsDbName);

			await _sqlCreateTables.CreateDonationsTable();

			return await WriteEntireTableAsync<Donation>(true, donations, _donationsDbName, force, progUpdate);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while serializing {_donationsDbName}.");
			return ex.Message;
		}
	}

	public async Task<ObservableCollection<Donation>> GetDonationsByBatchId(int batchId, string? category = null)
	{
		string where = $"WHERE BatchId = {batchId}";
		if (null != category)
			where += $" AND Category = '{category}'";

		try
		{
			return await SelectFromTableAsync<Donation>(_donationsDbName, where: where);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while deserializing {_donationsDbName} by batchId.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task<int> GetNextId()
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			string query = $"SELECT MAX(Id) AS Id FROM {_donationsDbName}";
			var value = await conn.QueryAsync<int>(query);
			var msxId = value.Any() ? value.First() : 0;

			return msxId + 1;
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while getting next Id from {_donationsDbName}.");
			return -1;
		}
	}

	public async Task AddDonations(ObservableCollection<Donation> donations)
	{
		// write the new batch record
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				var properties = Helper.PublicProperties<Donation>().ToList();
				properties.Remove("Id");
				string command = "";
				command += $"INSERT INTO {_donationsDbName} ({string.Join(',', properties)})\r\nVALUES ({string.Join(',', properties.Select(x => "@" + x).ToList())})";

				await conn.ExecuteAsync(command, donations);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while adding donations to {_donationsDbName}.");
		}
	}

	public async Task RemoveDonationsByBatchId(int batchId)
	{
		// write the new batch record
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				string command = $"DELETE FROM {_donationsDbName} WHERE BatchId = '{batchId}'";

				await conn.ExecuteAsync(command);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while removing donations by batchId {batchId} from {_donationsDbName}.");
		}
	}

	public async Task<ObservableCollection<Donation>> GetDonationsByDonorId(int id)
	{
		try
		{
			// get Donations by Donor Id
			return await SelectFromTableAsync<Donation>(_donationsDbName, where: $"WHERE DonorId = '{id}'");
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while deserializing {_donationsDbName} by donorId {id}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task<ObservableCollection<Donation>> GetDonationsByDonorIds(List<int> ids)
	{
		try
		{
			// get Donations by list of Donor Ids
			return await SelectFromTableAsync<Donation>(_donationsDbName, where: $"WHERE DonorId IN ({string.Join(",", ids)})");
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while deserializing {_donationsDbName} by donorIds.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task RemapDonorId(int oldDonorId, int newDonorId)
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				// update donorId in donation table
				string command = $"UPDATE {_donationsDbName} SET DonorId='{newDonorId}' WHERE DonorId = '{oldDonorId}'";

				await conn.ExecuteAsync(command);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while remapping donor id {oldDonorId} to {newDonorId} in {_donationsDbName}.");
		}

		return;
	}

	public async Task<ObservableCollection<Donation>> FilterDonationsByDate(enumDateFilterOptions dateFilter, string date, string date2)
	{
		string where = "";

		switch (dateFilter)
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
			return await SelectFromTableAsync<Donation>(_donationsDbName, where: where);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while deserializing {_donationsDbName} by dates.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task<ObservableCollection<string>> GetDonationYears()
	{
		try
		{
#pragma warning disable CS0168 // Variable is declared but never used
			ObservableCollection<string>? ret;
#pragma warning restore CS0168 // Variable is declared but never used

			using IDbConnection conn = new SqlConnection(_connString);
			{
				string query = $"SELECT DISTINCT YEAR(Date) FROM {_donationsDbName}";
				var enumerable = await conn.QueryAsync<string>(query);
				return new ObservableCollection<string>(enumerable);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught getting batch years from {_donationsDbName}.");
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
				string query = $"SELECT Date FROM {_donationsDbName} WHERE Date IN(SELECT min(Date) FROM {_donationsDbName})";
				var enumerable = await conn.QueryAsync<string>(query);
				return enumerable.First();
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught getting earliest date from {_donationsDbName}.");
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
				string query = $"SELECT Date FROM {_donationsDbName} WHERE Date IN(SELECT max(Date) FROM {_donationsDbName})";
				var enumerable = await conn.QueryAsync<string>(query);
				return enumerable.First();
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught getting most recent date from {_donationsDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}


}
