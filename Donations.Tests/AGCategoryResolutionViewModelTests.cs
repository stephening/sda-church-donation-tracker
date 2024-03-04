using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using Xunit;

namespace Donations.Tests;

public class AGCategoryResolutionViewModelTests : TestBase
{
	[Fact]
	public void Cat_Success()
	{
		// Arrange
		Category category = new Category() { Code = 101, Description = "Whatever", TaxDeductible = true };
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		obj.Cat = category;
		Category expected = category;

		// Act
		Category actual = obj.Cat;

		// Assert
		Assert.Equal(expected, actual);
		Assert.Equal(category.Code, obj.Cat.Code);
		Assert.Equal(category.Description, obj.Cat.Description);
		Assert.Equal(category.TaxDeductible, obj.Cat.TaxDeductible);
	}

	[Fact]
	public void Cat_Null()
	{
		// Arrange
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		Category? expected = null;

		// Act
		Category? actual = obj.Cat;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public void Cat_NullDescription(string? param)
	{
		// Arrange
		Category category = new Category() { Code = 101, Description = param, TaxDeductible = true };
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		obj.Cat = category;
		Category expected = category;

		// Act
		Category actual = obj.Cat;

		// Assert
		Assert.Equal(expected, actual);
		Assert.Equal(category.Code, obj.Cat.Code);
		Assert.Equal(category.Description, obj.Cat.Description);
		Assert.Equal(category.TaxDeductible, obj.Cat.TaxDeductible);
	}


	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void Cat_TaxDeductible(bool param)
	{
		// Arrange
		Category category = new Category() { Code = 101, Description = "Description", TaxDeductible = param };
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		obj.Cat = category;
		bool expected = param;

		// Act
		bool actual = obj.Cat.TaxDeductible;

		// Assert
		Assert.Equal(expected, actual);
		Assert.Equal(category.Code, obj.Cat.Code);
		Assert.Equal(category.Description, obj.Cat.Description);
		Assert.Equal(category.TaxDeductible, obj.Cat.TaxDeductible);
	}

	[Fact]
	public void Transaction_Success()
	{
		// Arrange
		AdventistGiving transaction = new AdventistGiving();
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		obj.Transaction = transaction;
		AdventistGiving expected = transaction;

		// Act
		AdventistGiving actual = obj.Transaction;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Transaction_Null()
	{
		// Arrange
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		AdventistGiving? expected = null;

		// Act
		AdventistGiving? actual = obj.Transaction;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public void Transaction_NullFirstName(string? param)
	{
		// Arrange
		AdventistGiving transaction = new AdventistGiving() { FirstName = param };
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		obj.Transaction = transaction;
		string? expected = param;

		// Act
		AdventistGiving actual = obj.Transaction;

		// Assert
		Assert.Equal(transaction, actual);
		Assert.True(string.IsNullOrEmpty(obj.Transaction.LastName));
	}

	[Theory]
	[InlineData("This is progress text...")]
	[InlineData(null)]
	[InlineData("")]
	public void ProgressText(string? param)
	{
		// Arrange
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		obj.ProgressText = param;
		string? expected = param;

		// Act
		string? actual = obj.ProgressText;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ContinueEnabled_True()
	{
		// Arrange
		Category cat = new Category();
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		obj.Cat = cat;

		// Act
		bool actual = obj.ContinueEnabled;

		// Assert
		Assert.True(actual);
	}

	[Fact]
	public void ContinueEnabled_False()
	{
		// Arrange
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		obj.Cat = null;

		// Act
		bool actual = obj.ContinueEnabled;

		// Assert
		Assert.False(actual);
	}

	[Theory]
	[InlineData(Visibility.Collapsed)]
	[InlineData(Visibility.Visible)]
	[InlineData(Visibility.Hidden)]
	public void CategoryDiffsVisibility(Visibility param)
	{
		// Arrange
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		obj.CategoryDiffsVisibility = param;
		Visibility expected = param;

		// Act
		Visibility actual = obj.CategoryDiffsVisibility;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(Visibility.Collapsed)]
	[InlineData(Visibility.Visible)]
	[InlineData(Visibility.Hidden)]
	public void CategoryResolutionComplete(Visibility param)
	{
		// Arrange
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		obj.CategoryResolutionComplete = param;
		Visibility expected = param;

		// Act
		Visibility actual = obj.CategoryResolutionComplete;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(true, Visibility.Visible, Visibility.Hidden)]
	[InlineData(false, Visibility.Hidden, Visibility.Visible)]
	public void ResolutionComplete(bool flag, Visibility expectedCategoryResolutionComplete, Visibility expectedCategoryDiffsVisibility)
	{
		// Arrange
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();

		// Act
		obj.ResolutionComplete(flag);

		// Assert
		Assert.Equal(expectedCategoryResolutionComplete, obj.CategoryResolutionComplete);
		Assert.Equal(expectedCategoryDiffsVisibility, obj.CategoryDiffsVisibility);
	}

	[Fact]
	public void ChooseCategory()
	{
		// Arrange
		Category expected = new Category();
		AGCategoryResolutionViewModel obj = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();

		// Act
		Category actualBefore = obj.Cat;
		obj.ChooseCategory(expected);
		Category actualAfter = obj.Cat;

		// Assert
		Assert.Null(actualBefore);
		Assert.Equal(expected, actualAfter);
	}

	[Fact]
	public async Task AGCategoryResolutionViewModel_Resolve()
	{
		// Arrange
		var td = new TestData();
		ICategoryServices categoryServices = DependencyInjection.Resolve<ICategoryServices>();
		categoryServices!.CatList = new ObservableCollection<Category>();
		categoryServices.CatDict = new Dictionary<int, Category>();
		ICategoryMapServices categoryMapServices = DependencyInjection.Resolve<ICategoryMapServices>();
		categoryMapServices!.AGCategoryMap = new Dictionary<int, AGCategoryMapItem>();

		AdventistGivingViewModel ag = DependencyInjection.Resolve<AdventistGivingViewModel>();
		AGCategoryResolutionViewModel catres = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		catres.CategoryResolutionComplete = Visibility.Visible;
		catres.CategoryDiffsVisibility = Visibility.Hidden;

		ag.TransactionList = td.AdventistGivingList;

		// Add some stuff to the category list so we can resolve
		// Tithe with same category code but without other languages
		categoryServices.CatDict[ag!.TransactionList![0].CategoryCode] = new Category() { Code = ag.TransactionList[0].CategoryCode, Description = ag.TransactionList[0].SplitCategoryName, TaxDeductible = true };
		categoryServices.CatList.Add(categoryServices.CatDict[ag.TransactionList[0].CategoryCode]);
		// With different category code, but part of the description, should get partial match
		categoryServices.CatDict[ag.TransactionList[1].CategoryCode * 100] = new Category() { Code = ag.TransactionList[1].CategoryCode * 100, Description = ag.TransactionList[1].CategoryName + " stuff", TaxDeductible = true };
		categoryServices.CatList.Add(categoryServices.CatDict[ag.TransactionList[1].CategoryCode * 100]);
		// with same code but different name
		categoryServices.CatDict[ag.TransactionList[4].CategoryCode] = new Category() { Code = ag.TransactionList[4].CategoryCode, Description = "Some other fund", TaxDeductible = false };
		categoryServices.CatList.Add(categoryServices.CatDict[ag.TransactionList[4].CategoryCode]);
		// exact match
		categoryServices.CatDict[ag.TransactionList[5].CategoryCode] = new Category() { Code = ag.TransactionList[5].CategoryCode, Description = ag.TransactionList[5].CategoryName, TaxDeductible = true };
		categoryServices.CatList.Add(categoryServices.CatDict[ag.TransactionList[5].CategoryCode]);
		// different code, but superset of split name
		categoryServices.CatDict[ag.TransactionList[7].CategoryCode * 100] = new Category() { Code = ag.TransactionList[7].CategoryCode * 100, Description = "pre " + ag.TransactionList[7].CategoryName + " post", TaxDeductible = true };
		categoryServices.CatList.Add(categoryServices.CatDict[ag.TransactionList[7].CategoryCode * 100]);

		// Act
		await catres.StartCategoryResolution();

		// Assert
		Assert.StartsWith("Record 2 of", catres.ProgressText);
		Assert.NotNull(catres.Cat);
		Assert.Contains(catres.Transaction!.CategoryName!, catres.Cat.Description);
		Assert.Equal(ag.TransactionList[1], catres.Transaction);

		// Act again
		// continue
		await catres.ContinueCategoryResolutionCommand.ExecuteAsync(null);

		// Assert again
		Assert.StartsWith("Record 5 of", catres.ProgressText);
		Assert.NotNull(catres.Cat);
		Assert.Equal(ag.TransactionList[4], catres.Transaction);

		// Act again
		// continue, should create a map entry for the close match above
		await catres.ContinueCategoryResolutionCommand.ExecuteAsync(null);

		// Assert again
		Assert.StartsWith("Record 8 of", catres.ProgressText);
		Assert.NotNull(catres.Cat);
		Assert.Equal(ag.TransactionList[7], catres.Transaction);

		// Act again
		// continue, should create a map entry for the close match above
		await catres.ContinueCategoryResolutionCommand.ExecuteAsync(null);

		// Assert
		Assert.Equal(Visibility.Visible, catres.CategoryResolutionComplete);
		Assert.Equal(Visibility.Hidden, catres.CategoryDiffsVisibility);
	}
}
