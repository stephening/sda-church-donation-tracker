using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.ViewModel;
using Donations.Tests.Views;
using Xunit;

namespace Donations.Tests;

public class CategoryViewModelTests : TestBase
{
	public CategoryViewModel? CategoryViewModelDataContext { get; set; }

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void HasChanges(bool expected)
	{
		// Arrange
		CategoryViewModel obj = DependencyInjection.Resolve<CategoryViewModel>();
		obj.HasChanges = expected;

		// Assert
		Assert.Equal(expected, obj.HasChanges);
	}

	[StaFact]
	public void CategoryViewModel()
	{
		// Arrange
		int i;
		ICategoryServices categoryServices = DependencyInjection.Resolve<ICategoryServices>();

		CategoryViewTest view = new CategoryViewTest();
		CategoryViewModelDataContext = DependencyInjection.Resolve<CategoryViewModel>();
		CategoryViewModel obj = CategoryViewModelDataContext;
		view.DataContext = this;
		view.Show();

		// Act
		obj.Loading();

		// Assert
		Assert.Equal(categoryServices.CatList!.Count, obj.CategoryList!.Count);
		for (i = 0; i < obj.CategoryList.Count; i++)
		{
			Assert.Equal(categoryServices.CatList[i].Code, obj.CategoryList[i].Code);
			Assert.Equal(categoryServices.CatList[i].Description, obj.CategoryList[i].Description);
			Assert.Equal(categoryServices.CatList[i].TaxDeductible, obj.CategoryList[i].TaxDeductible);
		}

		// Act again
		view.UserControl.SelectGridRow(9);
		obj.InsertRowAboveCommand.Execute(null);
		obj.CategoryList[9].Code = 19;
		obj.CategoryList[9].Description = "nineteen";
		obj.CategoryList[9].TaxDeductible = true;

		// Assert
		Assert.Equal(categoryServices.CatList.Count + 1, obj.CategoryList.Count);
		for (i = 0; i < obj.CategoryList.Count; i++)
		{
			if (9 > i)
			{
				Assert.Equal(categoryServices.CatList[i].Code, obj.CategoryList[i].Code);
				Assert.Equal(categoryServices.CatList[i].Description, obj.CategoryList[i].Description);
				Assert.Equal(categoryServices.CatList[i].TaxDeductible, obj.CategoryList[i].TaxDeductible);
			}
			else if (9 < i)
			{
				Assert.Equal(categoryServices.CatList[i - 1].Code, obj.CategoryList[i].Code);
				Assert.Equal(categoryServices.CatList[i - 1].Description, obj.CategoryList[i].Description);
				Assert.Equal(categoryServices.CatList[i - 1].TaxDeductible, obj.CategoryList[i].TaxDeductible);
			}
			else
			{
				Assert.Equal(19, obj.CategoryList[i].Code);
				Assert.Equal("nineteen", obj.CategoryList[i].Description);
				Assert.True(obj.CategoryList[i].TaxDeductible);
			}
		}

		// Act again
		view.UserControl.SelectGridRow(14);
		obj.InsertRowBelowCommand.Execute(null);
		obj.CategoryList[15].Code = 25;
		obj.CategoryList[15].Description = "twenty five";

		// Assert
		Assert.Equal(categoryServices.CatList.Count + 2, obj.CategoryList.Count);
		for (i = 0; i < obj.CategoryList.Count; i++)
		{
			if (9 > i)
			{
				Assert.Equal(categoryServices.CatList[i].Code, obj.CategoryList[i].Code);
				Assert.Equal(categoryServices.CatList[i].Description, obj.CategoryList[i].Description);
				Assert.Equal(categoryServices.CatList[i].TaxDeductible, obj.CategoryList[i].TaxDeductible);
			}
			else if (15 == i)
			{
				Assert.Equal(25, obj.CategoryList[i].Code);
				Assert.Equal("twenty five", obj.CategoryList[i].Description);
				Assert.False(obj.CategoryList[i].TaxDeductible);
			}
			else if (9 < i)
			{
				Assert.Equal(categoryServices.CatList[i - 1].Code, obj.CategoryList[i].Code);
				Assert.Equal(categoryServices.CatList[i - 1].Description, obj.CategoryList[i].Description);
				Assert.Equal(categoryServices.CatList[i - 1].TaxDeductible, obj.CategoryList[i].TaxDeductible);
			}
			else
			{
				Assert.Equal(19, obj.CategoryList[i].Code);
				Assert.Equal("nineteen", obj.CategoryList[i].Description);
				Assert.True(obj.CategoryList[i].TaxDeductible);
			}
		}

		// Act again
		obj.SaveChangesCommand.Execute(null);

		// Assert
		for (i = 0; i < obj.CategoryList.Count; i++)
		{
			Assert.Equal(categoryServices.CatList[i].Code, obj.CategoryList[i].Code);
			Assert.Equal(categoryServices.CatList[i].Description, obj.CategoryList[i].Description);
			Assert.Equal(categoryServices.CatList[i].TaxDeductible, obj.CategoryList[i].TaxDeductible);
		}

		// Act again
		obj.DeleteAllCommand.Execute(null);

		// Assert
		Assert.Empty(obj.CategoryList);

		// Act again
		obj.SaveChangesCommand.Execute(null);

		// Assert
		Assert.Empty(categoryServices!.CatList!);
	}
}
