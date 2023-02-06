using Donations.Model;
using Donations.ViewModel;
using Xunit;

namespace Donations.Tests
{
	/// <summary>
	/// All these test need to be in the same class so they will run synchronously
	/// </summary>
	public class BatchBrowserViewModelTests
	{
		[Theory]
		[InlineData(enumBatchFilterOptions.CurrentYear)]
		[InlineData(enumBatchFilterOptions.PreviousYear)]
		[InlineData(enumBatchFilterOptions.SelectYear)]
		[InlineData(enumBatchFilterOptions.DateRange)]
		public void BatchFilterOption(enumBatchFilterOptions option)
		{
			// Arrange
			BatchBrowserViewModel viewModel = new BatchBrowserViewModel() { BatchFilterOption = option };

			// Act

			// Assert
			Assert.Equal(option, viewModel.BatchFilterOption);
		}

		[Theory]
		[InlineData(enumBatchFilterOptions.CurrentYear, false)]
		[InlineData(enumBatchFilterOptions.PreviousYear, false)]
		[InlineData(enumBatchFilterOptions.SelectYear, true)]
		[InlineData(enumBatchFilterOptions.DateRange, false)]
		public void SelectYearComboBoxEnabled(enumBatchFilterOptions option, bool expected)
		{
			// Arrange
			BatchBrowserViewModel viewModel = new BatchBrowserViewModel() { BatchFilterOption = option };

			// Act

			// Assert
			Assert.Equal(expected, viewModel.SelectYearComboBoxEnabled);
		}

		[Theory]
		[InlineData(enumBatchFilterOptions.CurrentYear, false)]
		[InlineData(enumBatchFilterOptions.PreviousYear, false)]
		[InlineData(enumBatchFilterOptions.SelectYear, false)]
		[InlineData(enumBatchFilterOptions.DateRange, true)]
		public void DateRangeEnabled(enumBatchFilterOptions option, bool expected)
		{
			// Arrange
			BatchBrowserViewModel viewModel = new BatchBrowserViewModel() { BatchFilterOption = option };

			// Act

			// Assert
			Assert.Equal(expected, viewModel.DateRangeEnabled);
		}

		[Theory]
		[InlineData("Anystring")]
		[InlineData("2022")]
		[InlineData("2020")]
		public void FilterYear(string filter)
		{
			// Arrange
			BatchBrowserViewModel viewModel = new BatchBrowserViewModel() { FilterYear = filter };

			// Act

			// Assert
			Assert.Equal(filter, viewModel.FilterYear);
		}

		[Fact]
		public void BatchListUpdated()
		{
			// Arrange
			di.Data.BatchList.Clear();
			di.Data.BatchDict.Clear();

			di.Data.BatchDict[1] = new Batch() { Date = "1990/1/1" };
			di.Data.BatchList.Add(di.Data.BatchDict[1]);
			di.Data.BatchDict[2] = new Batch() { Date = "12/31/2022" };
			di.Data.BatchList.Add(di.Data.BatchDict[2]);
			BatchBrowserViewModel obj = new BatchBrowserViewModel();

			// Act

			// Assert
			Assert.Equal("1990/01/01", obj.FilterStartDate);
			Assert.Equal("2022/12/31", obj.FilterEndDate);
		}

		[Theory]
		[InlineData("1/1/2022", "2022/01/01")]
		[InlineData("02/02/2020", "2020/02/02")]
		[InlineData("April 15, 1990", "1990/04/15")]
		[InlineData("", "")]
		[InlineData("anything", "")]
		public void FilterStartDate(string filter, string expected)
		{
			// Arrange
			di.Data.BatchList.Clear();
			di.Data.BatchDict.Clear();

			BatchBrowserViewModel viewModel = new BatchBrowserViewModel() { FilterStartDate = filter };

			// Act

			// Assert
			Assert.Equal(expected, viewModel.FilterStartDate);
		}

		[Theory]
		[InlineData("1/1/2022", "2022/01/01")]
		[InlineData("02/02/2020", "2020/02/02")]
		[InlineData("April 15, 1990", "1990/04/15")]
		[InlineData("", "")]
		[InlineData("anything", "")]
		public void FilterEndDate(string filter, string expected)
		{
			// Arrange
			di.Data.BatchList.Clear();
			di.Data.BatchDict.Clear();

			BatchBrowserViewModel viewModel = new BatchBrowserViewModel() { FilterEndDate = filter };

			// Act

			// Assert
			Assert.Equal(expected, viewModel.FilterEndDate);
		}
	}
}
