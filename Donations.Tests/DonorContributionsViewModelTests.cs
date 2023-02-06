using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using Xunit;

namespace Donations.Tests
{
	public partial class DataAccessSequentialTests
	{
		[Theory]
		[InlineData(123.45)]
		[InlineData(0.99)]
		[InlineData(-12.80)]
		public void BatchTotal(double expected)
		{
			// Arrange
			DonorContributionsViewModel obj = new DonorContributionsViewModel() { SubTotal = expected };

			// Assert
			Assert.Equal(expected, Math.Round(obj.SubTotal, 2));
		}

		[Fact]
		public void DonorContributionsViewModel_ShowNullBatch()
		{
			// Arrange
			DonorContributionsViewModel obj = new DonorContributionsViewModel();

			// Act

			// Assert
			Assert.Throws<ArgumentNullException>(() => obj.Show(null, new CategorySum()));
		}

		[Fact]
		public void DonorContributionsViewModel_ShowNullCategorySum()
		{
			// Arrange
			DonorContributionsViewModel obj = new DonorContributionsViewModel();

			// Act

			// Assert
			Assert.Throws<ArgumentNullException>(() => obj.Show(new Batch(), null));
		}

		[Fact]
		public void DonorContributionsViewModel_ShowNoDonorNoCategoryDb()
		{
			// Arrange
			var td = new TestData();
			di.Data.BatchDict[1] = new Batch() { Id = 1 }; di.Data.BatchList.Add(di.Data.BatchDict[1]);
			di.Data.DonationDict = td.DonationDict;
			di.Data.DonationList = td.DonationList;

			CategorySum categorySum = new CategorySum() { Category = "1 tithe", Sum = 10000 };

			DonorContributionsViewModel obj = new DonorContributionsViewModel();

			// Act
			obj.Show(di.Data.BatchDict[1], categorySum);

			// Assert
			Assert.Equal(categorySum.Sum, obj.SubTotal);
			Assert.Equal(di.Data.DonationDict[1].LastName, obj.ContributionList[0].LastName);
			Assert.Equal(di.Data.DonationDict[1].FirstName, obj.ContributionList[0].FirstName);
		}
	}
}
