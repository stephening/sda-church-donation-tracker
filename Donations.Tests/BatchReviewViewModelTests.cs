using Donations.Lib;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using Xunit;

namespace Donations.Tests;

public class BatchReviewViewModelTests : TestBase
{
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void HasChanges(bool expected)
	{
		// Arrange
		BatchReviewViewModel obj = DependencyInjection.Resolve<BatchReviewViewModel>();
		obj.HasChanges = expected;

		// Assert
		Assert.Equal(expected, obj.HasChanges);
	}

	[Theory]
	[InlineData("1/2/2005", "2005/01/02")]
	[InlineData("March 23, 2023", "2023/03/23")]
	[InlineData("5-5-2000", "2000/05/05")]
	[InlineData(null, "")]
	public void BatchDate(string? param, string expected)
	{
		// Arrange
		BatchReviewViewModel obj = DependencyInjection.Resolve<BatchReviewViewModel>();
#pragma warning disable CS8601 // Possible null reference assignment.
		obj.BatchDate = param;
#pragma warning restore CS8601 // Possible null reference assignment.

		// Assert
		Assert.Equal(expected, obj.BatchDate);
	}

	[Theory]
	[InlineData("this is a note")]
	[InlineData("")]
	[InlineData(null)]
	public void BatchNote(string? expected)
	{
		// Arrange
		BatchReviewViewModel obj = DependencyInjection.Resolve<BatchReviewViewModel>();
		obj.BatchNote = expected;

		// Assert
		Assert.Equal(expected, obj.BatchNote);
	}

	[Theory]
	[InlineData(123.45)]
	[InlineData(0.99)]
	[InlineData(-12.80)]
	public void BatchTotal(double expected)
	{
		// Arrange
		BatchReviewViewModel obj = DependencyInjection.Resolve<BatchReviewViewModel>();
		obj.BatchTotal = expected;

		// Assert
		Assert.Equal(expected, Math.Round(obj.BatchTotal, 2));
	}

	[Fact]
	public async Task Review()
	{
		// Arrange
		var td = new TestData();
		Batch batch = td.BatchDict![1];
		ObservableCollection<Donation> donations = td.DonationList!;
		// change test data batchid to be all the same so we will have multiple donations for a category
		foreach (var item in donations)
		{
			item.BatchId = 1;
		}
		BatchReviewViewModel obj = DependencyInjection.Resolve<BatchReviewViewModel>();
		Dictionary<string, CategorySum> catsum = new Dictionary<string, CategorySum>();
		List<CategorySum> catsumlist = new List<CategorySum>();
		foreach (var d in donations)
		{
			if (catsum.ContainsKey(d.Category!))
			{
				catsum[d.Category!].Sum += d.Value;
			}
			else
			{
				catsum[d.Category!] = new CategorySum() { Category = d.Category, Sum = d.Value };
				catsumlist.Add(catsum[d.Category!]);
			}
		}

		// Act
		await obj.Review(batch, donations, () => { });

		// Assert
		Assert.False(obj.HasChanges);
		Assert.Equal(catsumlist.Count, obj.CategorySums!.Count);
		for (int i = 0; i < catsumlist.Count; i++)
		{
			Assert.Equal(catsumlist[i].Category, obj.CategorySums[i].Category);
			Assert.Equal(Math.Round(catsumlist[i].Sum, 2), Math.Round(obj.CategorySums[i].Sum, 2));
		}

		// Act again
		obj.SubmitBatchCommand.Execute(null);

		// Assert again
		Assert.Equal(batch.Date, obj.BatchDate);
		Assert.Equal(batch.Note, obj.BatchNote);
	}
}
