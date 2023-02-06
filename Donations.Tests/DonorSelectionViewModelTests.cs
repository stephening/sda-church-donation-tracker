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
		public void DonorSelectionViewModel_LastNameFilterText(string param)
		{
			// Arrange
			DonorSelectionViewModel obj = new DonorSelectionViewModel() { LastNameFilterText = param };

			// Assert
			Assert.Equal(param, obj.LastNameFilterText);
		}

		[Theory]
		[InlineData("this is filter text")]
		[InlineData("")]
		public void DonorSelectionViewModel_FirstNameFilterText(string param)
		{
			// Arrange
			DonorSelectionViewModel obj = new DonorSelectionViewModel() { FirstNameFilterText = param };

			// Assert
			Assert.Equal(param, obj.FirstNameFilterText);
		}

		[Theory]
		[InlineData(false, -1)]
		[InlineData(true, 0)]
		[InlineData(true, 1)]
		[InlineData(true, 1000000)]
		public void DonorSelectionViewModel_OKEnabled(bool expected, int index)
		{
			// Arrange
			DonorSelectionViewModel obj = new DonorSelectionViewModel() { SelectedDonorIndex = index };

			// Assert
			Assert.Equal(expected, obj.OKEnabled);
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(1000000)]
		public void DonorSelectionViewModel_SelectedCategoryIndex(int param)
		{
			// Arrange
			DonorSelectionViewModel obj = new DonorSelectionViewModel() { SelectedDonorIndex = param };

			// Assert
			Assert.Equal(param, obj.SelectedDonorIndex);
		}

		[Theory]
		[InlineData("", "", 7)]
		[InlineData("d", "", 3)]
		[InlineData("", "j", 5)]
		[InlineData("", "jo", 4)]
		[InlineData("d", "john", 2)]
		[InlineData("", "johnn", 1)]
		public void DonorSelectionViewModel_Filter_TextChanged(string last, string first, int expected)
		{
			// Arrange
			var td = new TestData();
			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			DonorSelectionViewModel obj = new DonorSelectionViewModel() { LastNameFilterText = last, FirstNameFilterText = first };

			// Act
			obj.TextChanged();

			// Assert
			Assert.Equal(expected, obj.ViewSource.View.Cast<Donor>().Count());
		}
	}
}
