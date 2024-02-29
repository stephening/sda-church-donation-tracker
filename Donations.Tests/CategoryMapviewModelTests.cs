using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using Donations.Tests.Views;
using Xunit;

namespace Donations.Tests;

public class CategoryMapViewModelTests : TestBase
{
	public CategoryMapViewModel? CategoryMapViewModelDataContext { get; set; }

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void HasChanges(bool expected)
	{
		// Arrange
		CategoryMapViewModel obj = DependencyInjection.Resolve<CategoryMapViewModel>();
		obj.HasChanges = expected;

		// Assert
		Assert.Equal(expected, obj.HasChanges);
	}

	[StaFact]
	public void CategoryMapViewModel()
	{
		// Arrange
		int i;
		ICategoryMapServices categoryMapServices = DependencyInjection.Resolve<ICategoryMapServices>();
		categoryMapServices!.AGCategoryMapList!.Clear();
		categoryMapServices.AGCategoryMap!.Clear();
		ICategoryServices categoryServices = DependencyInjection.Resolve<ICategoryServices>();

		categoryMapServices.AGCategoryMap[1] = new AGCategoryMapItem() { AGCategoryCode = 101, CategoryCode = 1, CategoryDescription = categoryServices.GetCategoryDescription(1) }; categoryMapServices.AGCategoryMapList.Add(categoryMapServices.AGCategoryMap[1]);
		categoryMapServices.AGCategoryMap[2] = new AGCategoryMapItem() { AGCategoryCode = 102, CategoryCode = 2, CategoryDescription = categoryServices.GetCategoryDescription(2) }; categoryMapServices.AGCategoryMapList.Add(categoryMapServices.AGCategoryMap[2]);
		categoryMapServices.AGCategoryMap[3] = new AGCategoryMapItem() { AGCategoryCode = 103, CategoryCode = 3, CategoryDescription = categoryServices.GetCategoryDescription(3) }; categoryMapServices.AGCategoryMapList.Add(categoryMapServices.AGCategoryMap[3]);
		categoryMapServices.AGCategoryMap[4] = new AGCategoryMapItem() { AGCategoryCode = 104, CategoryCode = 4, CategoryDescription = categoryServices.GetCategoryDescription(4) }; categoryMapServices.AGCategoryMapList.Add(categoryMapServices.AGCategoryMap[4]);
		categoryMapServices.AGCategoryMap[5] = new AGCategoryMapItem() { AGCategoryCode = 105, CategoryCode = 5, CategoryDescription = categoryServices.GetCategoryDescription(5) }; categoryMapServices.AGCategoryMapList.Add(categoryMapServices.AGCategoryMap[5]);
		categoryMapServices.AGCategoryMap[6] = new AGCategoryMapItem() { AGCategoryCode = 106, CategoryCode = 6, CategoryDescription = categoryServices.GetCategoryDescription(6) }; categoryMapServices.AGCategoryMapList.Add(categoryMapServices.AGCategoryMap[6]);

		CategoryMapViewModelDataContext = DependencyInjection.Resolve<CategoryMapViewModel>();
		CategoryMapViewModel obj = CategoryMapViewModelDataContext;
		var view = new CategoryMapViewTest();
		view.DataContext = this;
		view.Show();

		// Act
		obj.Loading();

		// Assert
		Assert.Equal(6, obj.CategoryMapList!.Count);
		for (i = 0; i < obj.CategoryMapList.Count; i++)
		{
			Assert.Equal(categoryServices.CatDict![obj.CategoryMapList[i].CategoryCode].Description, obj.CategoryMapList[i].CategoryDescription);
		}

		// Act again
		int delItem = 2;
		view.UserControl.SelectGridRow(delItem);
		obj.DeleteRowCommand.Execute(null);

		// Assert again
		Assert.Equal(5, obj.CategoryMapList.Count);
		Assert.True(obj.HasChanges);
		for (i = 0; i < obj.CategoryMapList.Count; i++)
		{
			if (i < delItem)
			{
				Assert.Equal(categoryMapServices.AGCategoryMapList[i].AGCategoryCode, obj.CategoryMapList[i].AGCategoryCode);
				Assert.Equal(categoryMapServices.AGCategoryMapList[i].CategoryCode, obj.CategoryMapList[i].CategoryCode);
				Assert.Equal(categoryMapServices.AGCategoryMapList[i].CategoryDescription, obj.CategoryMapList[i].CategoryDescription);
			}
			else if (i >= delItem)
			{
				Assert.Equal(categoryMapServices.AGCategoryMapList[i + 1].AGCategoryCode, obj.CategoryMapList[i].AGCategoryCode);
				Assert.Equal(categoryMapServices.AGCategoryMapList[i + 1].CategoryCode, obj.CategoryMapList[i].CategoryCode);
				Assert.Equal(categoryMapServices.AGCategoryMapList[i + 1].CategoryDescription, obj.CategoryMapList[i].CategoryDescription);
			}
		}

		// Act again
		obj.RevertCommand.Execute(null);

		// Assert again
		Assert.Equal(6, obj.CategoryMapList.Count);
		Assert.False(obj.HasChanges);
		for (i = 0; i < obj.CategoryMapList.Count; i++)
		{
			Assert.Equal(categoryMapServices.AGCategoryMapList[i].AGCategoryCode, obj.CategoryMapList[i].AGCategoryCode);
			Assert.Equal(categoryMapServices.AGCategoryMapList[i].CategoryCode, obj.CategoryMapList[i].CategoryCode);
			Assert.Equal(categoryMapServices.AGCategoryMapList[i].CategoryDescription, obj.CategoryMapList[i].CategoryDescription);
		}

		// Act again
		obj.SetCategory(obj.CategoryMapList[5], new Category() { Code = 7, Description = "seven", TaxDeductible = false });

		// Assert again
		Assert.Equal(6, obj.CategoryMapList.Count);
		for (i = 0; i < obj.CategoryMapList.Count; i++)
		{
			Assert.Equal(categoryServices.CatDict![obj.CategoryMapList[i].CategoryCode].Description, obj.CategoryMapList[i].CategoryDescription);
		}

		// Act again
		obj.SaveChangesCommand.Execute(null);

		// Assert again
		Assert.Equal(6, categoryMapServices.AGCategoryMapList.Count);
		for (i = 0; i < obj.CategoryMapList.Count; i++)
		{
			Assert.Equal(categoryServices.CatDict![categoryMapServices.AGCategoryMapList[i].CategoryCode].Description, categoryMapServices.AGCategoryMapList[i].CategoryDescription);
		}

		// Act again
		obj.DeleteAllCommand.Execute(null);

		// Assert again
		Assert.Empty(obj.CategoryMapList);

		// Act again
		obj.SaveChangesCommand.Execute(null);

		// Assert
		Assert.Empty(categoryMapServices.AGCategoryMapList);
	}

}
