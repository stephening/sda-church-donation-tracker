using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using Xunit;

namespace Donations.Tests
{
	public partial class DataAccessSequentialTests
	{

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void CategoryViewModel_HasChanges(bool expected)
		{
			// Arrange
			CategoryViewModel obj = new CategoryViewModel() { HasChanges = expected };

			// Assert
			Assert.Equal(expected, obj.HasChanges);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(100)]
		[InlineData(-1)]
		[InlineData(null)]
		public void CategoryViewModel_SelectedRowIndex(int? param)
		{
			// Arrange
			CategoryViewModel obj = new CategoryViewModel() { SelectedRowIndex = param };

			// Assert
			Assert.Equal(param, obj.SelectedRowIndex);
		}

		[Fact]
		public void CategoryViewModel()
		{
			// Arrange
			int i;
			var td = new TestData();
			di.Data.CatDict = td.CatDict;
			di.Data.CatList = td.CatList;

			CategoryViewModel obj = new CategoryViewModel();

			// Act
			obj.Loaded();

			// Assert
			Assert.Equal(td.CatList.Count, obj.CategoryList.Count);
			for (i = 0; i < obj.CategoryList.Count; i++)
			{
				Assert.Equal(td.CatList[i].Code, obj.CategoryList[i].Code);
				Assert.Equal(td.CatList[i].Description, obj.CategoryList[i].Description);
				Assert.Equal(td.CatList[i].TaxDeductible, obj.CategoryList[i].TaxDeductible);
			}

			// Act again
			obj.SelectedRowIndex = 2;
			obj.DeleteRowCmd.Execute(null);

			// Assert
			for (i = 0; i < obj.CategoryList.Count; i++)
			{
				Assert.Equal(td.CatList[2 <= i ? i + 1 : i].Code, obj.CategoryList[i].Code);
				Assert.Equal(td.CatList[2 <= i ? i + 1 : i].Description, obj.CategoryList[i].Description);
				Assert.Equal(td.CatList[2 <= i ? i + 1 : i].TaxDeductible, obj.CategoryList[i].TaxDeductible);
			}

			// Act again
			obj.RevertCmd.Execute(null);

			// Assert
			Assert.Equal(td.CatList.Count, obj.CategoryList.Count);
			for (i = 0; i < obj.CategoryList.Count; i++)
			{
				Assert.Equal(td.CatList[i].Code, obj.CategoryList[i].Code);
				Assert.Equal(td.CatList[i].Description, obj.CategoryList[i].Description);
				Assert.Equal(td.CatList[i].TaxDeductible, obj.CategoryList[i].TaxDeductible);
			}

			// Act again
			obj.SelectedRowIndex = 9;
			obj.InsertRowAboveCmd.Execute(null);
			obj.CategoryList[9].Code = 19;
			obj.CategoryList[9].Description = "nineteen";
			obj.CategoryList[9].TaxDeductible = true;

			// Assert
			Assert.Equal(td.CatList.Count + 1, obj.CategoryList.Count);
			for (i = 0; i < obj.CategoryList.Count; i++)
			{
				if (9 > i)
				{
					Assert.Equal(td.CatList[i].Code, obj.CategoryList[i].Code);
					Assert.Equal(td.CatList[i].Description, obj.CategoryList[i].Description);
					Assert.Equal(td.CatList[i].TaxDeductible, obj.CategoryList[i].TaxDeductible);
				}
				else if (9	< i)
				{
					Assert.Equal(td.CatList[i - 1].Code, obj.CategoryList[i].Code);
					Assert.Equal(td.CatList[i - 1].Description, obj.CategoryList[i].Description);
					Assert.Equal(td.CatList[i - 1].TaxDeductible, obj.CategoryList[i].TaxDeductible);
				}
				else
				{
					Assert.Equal(19, obj.CategoryList[i].Code);
					Assert.Equal("nineteen", obj.CategoryList[i].Description);
					Assert.True(obj.CategoryList[i].TaxDeductible);
				}
			}

			// Act again
			obj.SelectedRowIndex = 14;
			obj.InsertRowBelowCmd.Execute(null);
			obj.CategoryList[15].Code = 25;
			obj.CategoryList[15].Description = "twenty five";

			// Assert
			Assert.Equal(td.CatList.Count + 2, obj.CategoryList.Count);
			for (i = 0; i < obj.CategoryList.Count; i++)
			{
				if (9 > i)
				{
					Assert.Equal(td.CatList[i].Code, obj.CategoryList[i].Code);
					Assert.Equal(td.CatList[i].Description, obj.CategoryList[i].Description);
					Assert.Equal(td.CatList[i].TaxDeductible, obj.CategoryList[i].TaxDeductible);
				}
				else if (15 == i)
				{
					Assert.Equal(25, obj.CategoryList[i].Code);
					Assert.Equal("twenty five", obj.CategoryList[i].Description);
					Assert.False(obj.CategoryList[i].TaxDeductible);
				}
				else if (9 < i)
				{
					Assert.Equal(td.CatList[i - 1].Code, obj.CategoryList[i].Code);
					Assert.Equal(td.CatList[i - 1].Description, obj.CategoryList[i].Description);
					Assert.Equal(td.CatList[i - 1].TaxDeductible, obj.CategoryList[i].TaxDeductible);
				}
				else
				{
					Assert.Equal(19, obj.CategoryList[i].Code);
					Assert.Equal("nineteen", obj.CategoryList[i].Description);
					Assert.True(obj.CategoryList[i].TaxDeductible);
				}
			}

			// Act again
			obj.SaveChangesCmd.Execute(null);

			// Assert
			for (i = 0; i < obj.CategoryList.Count; i++)
			{
				Assert.Equal(td.CatList[i].Code, obj.CategoryList[i].Code);
				Assert.Equal(td.CatList[i].Description, obj.CategoryList[i].Description);
				Assert.Equal(td.CatList[i].TaxDeductible, obj.CategoryList[i].TaxDeductible);
			}

			// Act again
			obj.DeleteAllCmd.Execute(null);

			// Assert
			Assert.Equal(0, obj.CategoryList.Count);

			// Act again
			obj.SaveChangesCmd.Execute(null);
			//str = Encoding.UTF8.GetString(buffer);

			// Assert
			Assert.Equal(0, di.Data.CatList.Count);
		}
	}
}
