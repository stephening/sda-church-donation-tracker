using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace Donations.Tests;

public partial class ImportCategoriesViewModelTests : TestWizardBase
{
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void HasChanges(bool expected)
	{
		// Arrange
		WizardImportCategoriesViewModel obj = DependencyInjection.Resolve<WizardImportCategoriesViewModel>();
		obj.HasChanges = expected;

		// Assert
		Assert.Equal(expected, obj.HasChanges);
	}

	[Fact]
	public async void Save()
	{
		// Arrange
		var td = new TestData();

		WizardImportCategoriesViewModel obj = DependencyInjection.Resolve<WizardImportCategoriesViewModel>();

		ICategoryServices categoryServices = DependencyInjection.Resolve<ICategoryServices>();
		categoryServices.CatList!.Clear();
		categoryServices.CatDict!.Clear();

#pragma warning disable CS8604 // Possible null reference argument.
		obj.Collection = new ObservableCollection<Category>(td.CatList);
#pragma warning restore CS8604 // Possible null reference argument.

		// Act
		await obj.Save(true);

		// Assert
		Assert.False(obj.HasChanges);
		for (int i = 0; i < categoryServices.CatList.Count; i++)
		{
			Assert.Equal(td.CatList![i].Code, categoryServices.CatList[i].Code);
			Assert.Equal(td.CatList[i].Description, categoryServices.CatList[i].Description);
			Assert.Equal(td.CatList[i].TaxDeductible, categoryServices.CatList[i].TaxDeductible);
		}
	}

	[Fact]
	public void ReadFile()
	{
		// Arrange
		var td = new TestData();
		byte[] buffer = Encoding.UTF8.GetBytes(td.CategoriesCsv);
		MockFileSystem mockfs = new MockFileSystem(new Dictionary<string, MockFileData>
		{
			{ "categories.csv", new MockFileData(buffer) }
		});

		AutofacRegister(mockfs);

		ICategoryServices categoryServices = DependencyInjection.Resolve<ICategoryServices>();
		categoryServices.CatDict = new Dictionary<int, Category>();
		categoryServices.CatList = new ObservableCollection<Category>();

		WizardImportCategoriesViewModel obj = DependencyInjection.Resolve<WizardImportCategoriesViewModel>();

		// Act
		obj.ReadFile("categories.csv");

		// Assert
		Assert.Equal(td.CatList!.Count, obj.Collection.Count);
		for (int i = 0; i < obj.Collection.Count; i++)
		{
			Assert.Equal(td.CatList[i].Code, obj.Collection[i].Code);
			Assert.Equal(td.CatList[i].Description, obj.Collection[i].Description);
			Assert.Equal(td.CatList[i].TaxDeductible, obj.Collection[i].TaxDeductible);
		}
	}
}
