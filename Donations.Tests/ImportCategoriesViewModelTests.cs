using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace Donations.Tests
{
	public partial class DataAccessSequentialTests
	{
		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ImportCategoriesViewModel_HasChanges(bool expected)
		{
			// Arrange
			ImportCategoriesViewModel obj = new ImportCategoriesViewModel() { HasChanges = expected };

			// Assert
			Assert.Equal(expected, obj.HasChanges);
		}

		[Fact]
		public void ImportCategoriesViewModel_Save()
		{
			// Arrange
			var td = new TestData();
			MockFileSystem mockfs = new MockFileSystem();
			di.FileSystem = mockfs;

			di.Data.CatDict = td.CatDict;
			di.Data.CatList = td.CatList;

			ImportCategoriesViewModel obj = new ImportCategoriesViewModel();

			// Act
			obj.Save(true);
			MockFileData mockOutputFile = mockfs.GetFile(di.Data.CategoriesFileName);
			// get the data written to the mock file in Save()
			mockfs.AddFile("dummy.xml", new MockFileData(mockOutputFile.TextContents));
			// use xml deserializer to read it back in for verification
			var collection = di.Data.DeserializeXml<Category>("dummy.xml");

			// Assert
			Assert.False(obj.HasChanges);
			for (int i = 0; i < collection.Count; i++)
			{
				Assert.Equal(collection[i].Code, di.Data.CatList[i].Code);
				Assert.Equal(collection[i].Description, di.Data.CatList[i].Description);
				Assert.Equal(collection[i].TaxDeductible, di.Data.CatList[i].TaxDeductible);
			}
		}

		[Fact]
		public void ImportCategoriesViewModel_ReadFile()
		{
			// Arrange
			var td = new TestData();
			byte[] buffer = Encoding.UTF8.GetBytes(td.CategoriesCsv);
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ "categories.csv", new MockFileData(buffer) }
			});

			di.Data.CatDict = new Dictionary<int, Category>();
			di.Data.CatList = new ObservableCollection<Category>();

			ImportCategoriesViewModel obj = new ImportCategoriesViewModel();

			// Act
			obj.ReadFile("categories.csv");

			// Assert
			Assert.Equal(td.CatList.Count, obj.Collection.Count);
			for (int i = 0; i < obj.Collection.Count; i++)
			{
				Assert.Equal(td.CatList[i].Code, obj.Collection[i].Code);
				Assert.Equal(td.CatList[i].Description, obj.Collection[i].Description);
				Assert.Equal(td.CatList[i].TaxDeductible, obj.Collection[i].TaxDeductible);
			}
		}
	}
}
