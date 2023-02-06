using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using Xunit;

namespace Donations.Tests
{
	public partial class DataAccessSequentialTests
	{
		[Theory]
		[InlineData("this is filter text")]
		[InlineData("")]
		public void CategorySelectionViewModel_FilterText(string param)
		{
			// Arrange
			CategorySelectionViewModel obj = new CategorySelectionViewModel() { FilterText = param };

			// Assert
			Assert.Equal(param, obj.FilterText);
		}

		[Theory]
		[InlineData(false, -1)]
		[InlineData(true, 0)]
		[InlineData(true, 1)]
		[InlineData(true, 1000000)]
		public void CategorySelectionViewModel_OKEnabled(bool expected, int index)
		{
			// Arrange
			CategorySelectionViewModel obj = new CategorySelectionViewModel() { SelectedCategoryIndex = index };

			// Assert
			Assert.Equal(expected, obj.OKEnabled);
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(1000000)]
		public void CategorySelectionViewModel_SelectedCategoryIndex(int param)
		{
			// Arrange
			CategorySelectionViewModel obj = new CategorySelectionViewModel() { SelectedCategoryIndex = param };

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
		public void CategorySelectionViewModel_Filter(string param, int expected)
		{
			// Arrange
			var td = new TestData();
			di.Data.CatDict = td.CatDict;
			di.Data.CatList = td.CatList;

			CategorySelectionViewModel obj = new CategorySelectionViewModel() { FilterText = param };

			// Act
			obj.TextChanged();

			// Assert
			Assert.Equal(expected, obj.ViewSource.View.Cast<Category>().Count());
		}
	}
}
