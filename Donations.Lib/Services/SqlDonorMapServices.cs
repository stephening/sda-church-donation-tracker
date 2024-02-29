using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlDonorMapServices : SqlHelper, IDonorMapServices
{
	private readonly SqlCreateTables _sqlCreateTables;

	public SqlDonorMapServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables,
		IDonorServices donorServices)
		: base(logger)
	{
		try
		{
			AGDonorMapList = SelectFromTable<AGDonorMapItem>(_donorMapDbName);
			for (int i = 0; i < AGDonorMapList.Count; i++)
			{
				var donor = donorServices.GetDonorById(AGDonorMapList[i].DonorId);
				AGDonorMapList[i].RefreshDonorFields(donor);
#pragma warning disable CS8604 // Possible null reference argument.
				AGDonorMap[AGDonorMapList[i].AGDonorHash] = AGDonorMapList[i];
#pragma warning restore CS8604 // Possible null reference argument.
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception thrown while reading {_donorMapDbName}");
		}
		_sqlCreateTables = sqlCreateTables;
	}

	private string _donorMapDbName => "dbo.DonorMap";
	public ObservableCollection<AGDonorMapItem>? AGDonorMapList { get; set; } = new ObservableCollection<AGDonorMapItem>();
	public Dictionary<string, AGDonorMapItem>? AGDonorMap { get; set; } = new Dictionary<string, AGDonorMapItem>();

	public async Task<string?> SaveDonorMap(ObservableCollection<AGDonorMapItem> donorMapList, bool force = false)
	{
		try
		{
			await DropTable(_donorMapDbName);

			await _sqlCreateTables.CreateDonorMapTable();

			return await WriteEntireTableAsync<AGDonorMapItem>(false, donorMapList, _donorMapDbName, force, reseedOnDelete: false);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while saving donor map to {_donorMapDbName}");
			return ex.Message;
		}
	}
}
