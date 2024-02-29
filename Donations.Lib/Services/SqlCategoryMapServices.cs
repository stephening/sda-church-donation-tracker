using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlCategoryMapServices : SqlHelper, ICategoryMapServices
{
	private readonly SqlCreateTables _sqlCreateTables;

	private string _categoryMapDbName => "dbo.CategoryMap";
	public ObservableCollection<AGCategoryMapItem>? AGCategoryMapList { get; set; } = new ObservableCollection<AGCategoryMapItem>();
	public Dictionary<int, AGCategoryMapItem>? AGCategoryMap { get; set; } = new Dictionary<int, AGCategoryMapItem>();

	public SqlCategoryMapServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables,
		ICategoryServices categoryServices
	) : base(logger)
	{
		try
		{
			AGCategoryMapList = SelectFromTable<AGCategoryMapItem>(_categoryMapDbName);
			for (int i = 0; i < AGCategoryMapList.Count; i++)
			{
				AGCategoryMapList[i].CategoryDescription = categoryServices.GetCategoryDescription(AGCategoryMapList[i].CategoryCode);
				AGCategoryMap[AGCategoryMapList[i].AGCategoryCode] = AGCategoryMapList[i];
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception thrown while reading {_categoryMapDbName}");
		}
		_sqlCreateTables = sqlCreateTables;
	}

	public async Task<string?> SaveCategoryMap(ObservableCollection<AGCategoryMapItem> categoryMapList, bool force = false)
	{
		try
		{
			await DropTable(_categoryMapDbName);

			await _sqlCreateTables.CreateCategoryMapTable();

			return await WriteEntireTableAsync<AGCategoryMapItem>(false, categoryMapList, _categoryMapDbName, force, reseedOnDelete: false);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Unexpected error saving DonorMap data to {_categoryMapDbName}.");
		}

		return null;
	}
}
