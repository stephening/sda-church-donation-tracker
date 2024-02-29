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
using System.Security.Principal;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlDonorServices : SqlHelper, IDonorServices
{
	private string _donorsDbName => "dbo.Donors";
	private string _donorReportsDbName => "dbo.DonorReports";
	private readonly IReflectionHelpers _reflectionHelpers;
	private readonly IDonorReportServices _donorReportServices;
	private readonly IDonorChangeServices _donorChangeServices;
	private readonly SqlCreateTables _sqlCreateTables;

	public SqlDonorServices(
		ILogger logger,
		IReflectionHelpers reflectionHelpers,
		IDonorReportServices donorReportServices,
		IDonorChangeServices donorChangeServices,
		SqlCreateTables sqlCreateTables
	)
		: base(logger)
	{
		_reflectionHelpers = reflectionHelpers;
		_donorReportServices = donorReportServices;
		_donorChangeServices = donorChangeServices;
		_sqlCreateTables = sqlCreateTables;
	}

	public async Task<ObservableCollection<Donor>> LoadDonors()
	{
		try
		{
			return await SelectFromTableAsync<Donor>(_donorsDbName);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while deserializing {_donorsDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}
	public async Task<string?> SaveDonors(ObservableCollection<Donor> donors, bool force = false, Action<long, long>? progUpdate = null)
	{
		try
		{
			// need to do this before dropping the Donor table because of the key relationship
			var data = await _donorReportServices.Load();
			await DropTable(_donorReportsDbName);
			await DropTable(_donorsDbName);

			await _donorReportServices.Save(data);

			await _sqlCreateTables.CreateDonorsTable();

			return await WriteEntireTableAsync<Donor>(true, donors, _donorsDbName, force, progUpdate);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while serializing {_donorsDbName}.");
			return ex.Message;
		}
	}

	public void ReplaceDonorData(ObservableCollection<Donor> donorList)
	{
	}

	public async Task<ObservableCollection<Donor>> FilterDonors(int count = 100, string last = "", string first = "")
	{
		try
		{
			string top = (0 <= count) ? $"TOP({count})" : "";
			string where = "";
			if (!string.IsNullOrEmpty(last))
			{
				where = $"WHERE LastName LIKE '{last}%'";
			}
			if (!string.IsNullOrEmpty(first))
			{
				if (string.IsNullOrEmpty(where))
				{
					where = $"WHERE FirstName LIKE '{first}%'";
				}
				else
				{
					where += $"AND FirstName LIKE '{first}%'";
				}
			}

			return await SelectFromTableAsync<Donor>(_donorsDbName, top, where);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while getting filtered list from {_donorsDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public Donor FindDonor(ref string? comment, ref int? partial, string? last, string? first, string? address, string? address2, string? city, string? state, string? zip)
	{
		try
		{
			// get list of Donors with same last name
			var ret = SelectFromTable<Donor>(_donorsDbName, where: $"WHERE LastName = '{last}'");

			// Loop through subset with matching last name, looking for close matches.
			// If a close match is found, it is returned for examination by the operator.
			foreach (var donor in ret)
			{
				if (Helper.Compare(ref comment, ref partial, donor
					, last
					, first
					, address
					, false
					, address2
					, city
					, state
					, zip
					, Helper.ro_firstNameSplitters))
				{
					return donor;
				}
			}

#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while getting donors with last name {last} from {_donorsDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}
	public Donor GetDonorById(int id)
	{
		try
		{
			// get Donors by Id
			var ret = SelectFromTable<Donor>(_donorsDbName, where: $"WHERE Id = '{id}'");

			if (0 < ret.Count)
			{
				return ret[0];
			}

#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while getting donor by Id {id} from {_donorsDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task<Donor> GetDonorByIdAsync(int id)
	{
		try
		{
			// get Donors by Id
			var ret = await SelectFromTableAsync<Donor>(_donorsDbName, where: $"WHERE Id = '{id}'");

			if (0 < ret.Count)
			{
				return ret[0];
			}

#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while getting donor by Id {id} from {_donorsDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task<ObservableCollection<Donor>> GetDonorsByFamilyId(int id)
	{
		try
		{
			// get Donors by FamilyId
			ObservableCollection<Donor> donors = await SelectFromTableAsync<Donor>(_donorsDbName, where: $"WHERE FamilyId = '{id}'");

			return donors;
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught getting donor by familyId {id} from {_donorsDbName}.");
#pragma warning disable CS8603 // Possible null reference return.
			return null;
#pragma warning restore CS8603 // Possible null reference return.
		}
	}

	public async Task<int> AddDonor(Donor donor)
	{
		try
		{
			// write the new donor record
			int id = await Add<Donor>(donor, _donorsDbName);

#pragma warning disable CS8601 // Possible null reference assignment.
			DonorChange donorChange = new DonorChange()
			{
				DonorId = id,
				Name = donor.Name,
				WhatChanged = "New record added:\r\n" + _reflectionHelpers.ShowChanges(new Donor(), donor, new string[] { "Id", "LastUpdated" }),
				WhenChanged = DateTime.Now,
				WhoChanged = WindowsIdentity.GetCurrent().Name
			};
#pragma warning restore CS8601 // Possible null reference assignment.

			// don't need to await this
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			_donorChangeServices.Save(donorChange);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

			return id;
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught adding donor to {_donorsDbName}.");
			return -1;
		}
	}

	public async Task DeleteDonor(Donor donor)
	{
		try
		{
			await _donorReportServices.DeleteDonorReport(donor.Id);

			await Delete<Donor>(_donorsDbName, donor.Id);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught deleting donor from {_donorsDbName}.");
		}
	}

	public async Task UpdateDonor(Donor donor)
	{
		try
		{
			Donor original = await GetDonorByIdAsync(donor.Id);
			if (_reflectionHelpers.SameModel<Donor>(donor, original))
			{
				return;
			}

#pragma warning disable CS8601 // Possible null reference assignment.
			DonorChange donorChange = new DonorChange()
			{
				DonorId = donor.Id,
				Name = donor.Name,
				WhatChanged = _reflectionHelpers.ShowChanges(original, donor, new string[] { "LastUpdated" }),
				WhenChanged = DateTime.Now,
				WhoChanged = WindowsIdentity.GetCurrent().Name
			};
#pragma warning restore CS8601 // Possible null reference assignment.

			// don't need to await this
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			_donorChangeServices.Save(donorChange);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

			// update the donor record by id
			await Update<Donor>(donor, _donorsDbName, donor.Id);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught updating donor id {donor.Id} to {_donorsDbName}.");
		}
	}

	public async Task<int> GetNextFamilyId()
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			string query = $"SELECT MAX(FamilyId) AS FamilyId FROM {_donorsDbName}";
			var value = await conn.QueryAsync<int>(query);
			var maxId = value.Any() ? value.First() : 0;

			return maxId + 1;
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught getting next family id from {_donorsDbName}.");
			return 1;
		}
	}
}
