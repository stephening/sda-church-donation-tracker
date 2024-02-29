using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlDonorChangeServices : SqlHelper, IDonorChangeServices
{
	private string _donorChangesDbName => "dbo.DonorChanges";
	private readonly SqlCreateTables _sqlCreateTables;

	public SqlDonorChangeServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables
		)
		: base(logger)
	{
		_sqlCreateTables = sqlCreateTables;
	}

	public async Task<ObservableCollection<DonorChange>> Load()
	{
		try
		{
			var res = await SelectFromTableAsync<DonorChange>(_donorChangesDbName);
			return new ObservableCollection<DonorChange>(res);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exeception occurred when reading {_donorChangesDbName}");
		}

		return new ObservableCollection<DonorChange>();
	}

	public async Task<ObservableCollection<DonorChange>> GetDonorChangesByDonorId(int donorId)
	{
		try
		{
			// get Donors changes by DonorId
			var ret = await SelectFromTableAsync<DonorChange>(_donorChangesDbName, where: $"WHERE DonorId = '{donorId}'");

			if (0 < ret.Count)
			{
				return ret;
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while getting donor by Id {donorId} from {_donorChangesDbName}.");
		}
		return new ObservableCollection<DonorChange>();
	}

	public async Task<string?> Save(ObservableCollection<DonorChange> changes)
	{
		try
		{
			await _sqlCreateTables.CreateDonorChangesTable();

			// Copy the list incase it's the same. If the same, the clear in Replace... would clear the input list as well
			return await WriteEntireTableAsync<DonorChange>(false, changes, _donorChangesDbName, reseedOnDelete: false);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while saving Donor changes to {_donorChangesDbName}.");
		}

		return null;
	}

	public async Task<int?> Save(DonorChange change)
	{
		return await Add<DonorChange>(change, _donorChangesDbName);
	}
}
