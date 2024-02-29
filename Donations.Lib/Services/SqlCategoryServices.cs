using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlCategoryServices : SqlHelper, ICategoryServices
{
	private readonly SqlCreateTables _sqlCreateTables;

	private string _categoriesDbName => "dbo.Categories";
	public Dictionary<int, Category>? CatDict { get; set; } = new Dictionary<int, Category>();
	public ObservableCollection<Category>? CatList { get; set; } = new ObservableCollection<Category>();

	public SqlCategoryServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables
	)
		: base(logger)
	{
		try
		{
			CatList = SelectFromTable<Category>(_categoriesDbName);
			foreach (var category in CatList)
			{
				CatDict[category.Code] = category;
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exeception occurred when reading {_categoriesDbName}");
		}
		_sqlCreateTables = sqlCreateTables;
	}

	public async Task<string?> SaveCategories(ObservableCollection<Category> categories, bool force = false)
	{
		try
		{
			await DropTable(_categoriesDbName);

			await _sqlCreateTables.CreateCategoriesTable();

			// Copy the list incase it's the same. If the same, the clear in Replace... would clear the input list as well
			ReplaceCategoryData(new ObservableCollection<Category>(categories));
			return await WriteEntireTableAsync<Category>(false, categories, _categoriesDbName, force, reseedOnDelete: false);
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while saving category data to {_categoriesDbName}.");
		}

		return null;
	}
	public void ReplaceCategoryData(ObservableCollection<Category> categoryList)
	{
		CatList?.Clear();
		CatDict?.Clear();

		foreach (var category in categoryList)
		{
			CatList?.Add(category);
			CatDict[category.Code] = category;
		}
	}

	public string GetCategoryDescription(int code)
	{
		return (null != CatDict && CatDict.ContainsKey(code)) ? CatDict[code].Description : "";
	}
}
