using Dapper;
using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlDonorReportsServices : SqlHelper, IDonorReportServices
{
	private readonly SqlCreateTables _sqlCreateTables;

	private string _donorReportsDbName => "dbo.DonorReports";
	private string _donorDbName => "dbo.Donors";

	public SqlDonorReportsServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables
		)
		: base(logger)
	{
		_sqlCreateTables = sqlCreateTables;
	}

	public async Task<ObservableCollection<DonorReport>> Load()
	{
		try
		{
			var res = await SelectFromTableAsync<DonorReport>(_donorReportsDbName);
			return new ObservableCollection<DonorReport>(res);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exeception occurred when reading {_donorReportsDbName}");
		}

		return new ObservableCollection<DonorReport>();
	}

	public async Task<ObservableCollection<NamedDonorReport>> LoadNamed()
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				string query = $"SELECT {_donorReportsDbName}.DonorId, {_donorDbName}.LastName, {_donorDbName}.FirstName, {_donorDbName}.Email, {_donorDbName}.MobilePhone, {_donorDbName}.DontEmailReport, {_donorReportsDbName}.LastSent, {_donorReportsDbName}.Action\r\n" +
					$"FROM {_donorReportsDbName}, {_donorDbName}\r\n" +
					$"WHERE {_donorDbName}.Id = {_donorReportsDbName}.DonorId";
				var enumerable = await conn.QueryAsync<NamedDonorReport>(query);
				return new ObservableCollection<NamedDonorReport>(enumerable);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exeception occurred when reading {_donorReportsDbName}");
		}

		return new ObservableCollection<NamedDonorReport>();
	}

	public async Task<string?> Save(ObservableCollection<DonorReport> reports)
	{
		try
		{
			await DropTable(_donorReportsDbName);

			await _sqlCreateTables.CreateDonorReportsTable();

			// Copy the list incase it's the same. If the same, the clear in Replace... would clear the input list as well
			return await WriteEntireTableAsync<DonorReport>(false, reports, _donorReportsDbName, reseedOnDelete: false);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while saving Donor report data to {_donorReportsDbName}.");
		}

		return null;
	}


	public async Task DeleteDonorReport(int DonorId)
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				string command = $"DELETE FROM {_donorReportsDbName} WHERE DonorId = {DonorId}";

				await conn.ExecuteAsync(command);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught deleting DonorReport from {_donorReportsDbName}.");
		}
	}
}
