using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using Xunit;

namespace Donations.Tests
{
	public partial class DataAccessSequentialTests
	{
		[Theory]
		[InlineData(0)]
		[InlineData(100)]
		[InlineData(-1)]
		[InlineData(null)]
		public void CategoryMapViewModel_SelectedRowIndex(int? param)
		{
			// Arrange
			CategoryMapViewModel obj = new CategoryMapViewModel() { SelectedRowIndex = param };

			// Assert
			Assert.Equal(param, obj.SelectedRowIndex);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void CategoryMapViewModel_HasChanges(bool expected)
		{
			// Arrange
			CategoryMapViewModel obj = new CategoryMapViewModel() { HasChanges = expected };

			// Assert
			Assert.Equal(expected, obj.HasChanges);
		}

		[Fact]
		public void CategoryMapViewModel()
		{
			// Arrange
			int i;
			var td = new TestData();
			di.Data.AGCategoryMapList = new ObservableCollection<AGCategoryMapItem>();
			di.Data.AGCategoryMap = new Dictionary<int, AGCategoryMapItem>();
			di.Data.CatDict = td.CatDict;
			di.Data.CatList = td.CatList;

			di.Data.AGCategoryMap[1] = new AGCategoryMapItem() { AGCategoryCode = 101, CategoryCode = 1 }; di.Data.AGCategoryMapList.Add(di.Data.AGCategoryMap[1]);
			di.Data.AGCategoryMap[2] = new AGCategoryMapItem() { AGCategoryCode = 102, CategoryCode = 2 }; di.Data.AGCategoryMapList.Add(di.Data.AGCategoryMap[2]);
			di.Data.AGCategoryMap[3] = new AGCategoryMapItem() { AGCategoryCode = 103, CategoryCode = 3 }; di.Data.AGCategoryMapList.Add(di.Data.AGCategoryMap[3]);
			di.Data.AGCategoryMap[4] = new AGCategoryMapItem() { AGCategoryCode = 104, CategoryCode = 4 }; di.Data.AGCategoryMapList.Add(di.Data.AGCategoryMap[4]);
			di.Data.AGCategoryMap[5] = new AGCategoryMapItem() { AGCategoryCode = 105, CategoryCode = 5 }; di.Data.AGCategoryMapList.Add(di.Data.AGCategoryMap[5]);
			di.Data.AGCategoryMap[6] = new AGCategoryMapItem() { AGCategoryCode = 106, CategoryCode = 6 }; di.Data.AGCategoryMapList.Add(di.Data.AGCategoryMap[6]);

			CategoryMapViewModel obj = new CategoryMapViewModel();

			// Act
			obj.Loaded();

			// Assert
			Assert.Equal(6, obj.CategoryMapList.Count);
			for (i = 0; i < obj.CategoryMapList.Count; i++)
			{
				Assert.Equal(td.CatDict[obj.CategoryMapList[i].CategoryCode].Description, obj.CategoryMapList[i].CategoryDescription);
			}

			// Act again
			obj.SelectedRowIndex = 2;
			obj.DeleteRowCmd.Execute(null);

			// Assert
			Assert.Equal(5, obj.CategoryMapList.Count);
			for (i = 0; i < obj.CategoryMapList.Count; i++)
			{
				Assert.Equal(td.CatDict[obj.CategoryMapList[i].CategoryCode].Description, obj.CategoryMapList[i].CategoryDescription);
			}

			// Act again
			obj.RevertCmd.Execute(null);

			// Assert
			Assert.Equal(6, obj.CategoryMapList.Count);
			for (i = 0; i < obj.CategoryMapList.Count; i++)
			{
				Assert.Equal(td.CatDict[obj.CategoryMapList[i].CategoryCode].Description, obj.CategoryMapList[i].CategoryDescription);
			}
			// Act again
			obj.SetCategory(obj.CategoryMapList[5], new Category() { Code = 7, Description = "seven", TaxDeductible = false });

			// Assert
			Assert.Equal(6, obj.CategoryMapList.Count);
			for (i = 0; i < obj.CategoryMapList.Count; i++)
			{
				Assert.Equal(td.CatDict[obj.CategoryMapList[i].CategoryCode].Description, obj.CategoryMapList[i].CategoryDescription);
			}

			// Act again
			obj.SaveChangesCmd.Execute(null);

			// Assert
			Assert.Equal(6, di.Data.AGCategoryMapList.Count);
			for (i = 0; i < obj.CategoryMapList.Count; i++)
			{
				Assert.Equal(td.CatDict[di.Data.AGCategoryMapList[i].CategoryCode].Description, di.Data.AGCategoryMapList[i].CategoryDescription);
			}

			// Act again
			obj.DeleteAllCmd.Execute(null);

			// Assert
			Assert.Equal(0, obj.CategoryMapList.Count);

			// Act again
			obj.SaveChangesCmd.Execute(null);

			// Assert
			Assert.Equal(0, di.Data.AGCategoryMapList.Count);
		}

	}
}
