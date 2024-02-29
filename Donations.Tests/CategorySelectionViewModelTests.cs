using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using Xunit;

namespace Donations.Tests;

public class CategorySelectionViewModelTests : TestBase
{
	[Theory]
	[InlineData("this is filter text")]
	[InlineData("")]
	public void FilterText(string param)
	{
		// Arrange
		CategorySelectionViewModel obj = DependencyInjection.Resolve<CategorySelectionViewModel>();
		obj.FilterText = param;

		// Assert
		Assert.Equal(param, obj.FilterText);
	}

	[Theory]
	[InlineData(false, -1)]
	[InlineData(true, 0)]
	[InlineData(true, 1)]
	[InlineData(true, 1000000)]
	public void OKEnabled(bool expected, int index)
	{
		// Arrange
		CategorySelectionViewModel obj = DependencyInjection.Resolve<CategorySelectionViewModel>();
		obj.SelectedCategoryIndex = index;

		// Assert
		Assert.Equal(expected, obj.OKEnabled);
	}

	[Theory]
	[InlineData(-1)]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(1000000)]
	public void SelectedCategoryIndex(int param)
	{
		// Arrange
		CategorySelectionViewModel obj = DependencyInjection.Resolve<CategorySelectionViewModel>();
		obj.SelectedCategoryIndex = param;

		// Assert
		Assert.Equal(param, obj.SelectedCategoryIndex);
	}

	[Theory]
	[InlineData("", 14)]
	[InlineData("o", 5)]
	[InlineData("on", 1)]
	[InlineData("one", 1)]
	[InlineData("s", 2)]
	[InlineData("t", 9)]
	[InlineData("tw", 6)]
	[InlineData("twe", 5)]
	[InlineData("twenty two", 1)]
	public void Filter(string param, int expected)
	{
		// Arrange
		var td = new TestData();
		ICategoryServices categoryServices = DependencyInjection.Resolve<ICategoryServices>();
		categoryServices!.CatDict = td.CatDict;
		categoryServices.CatList = td.CatList;

		CategorySelectionViewModel obj = DependencyInjection.Resolve<CategorySelectionViewModel>();
		obj.FilterText = param;

		// Act
		obj.TextChanged();

		// Assert
		Assert.Equal(expected, obj.ViewSource.View.Cast<Category>().Count());
	}
}
