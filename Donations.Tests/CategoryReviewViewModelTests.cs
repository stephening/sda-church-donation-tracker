using Donations.Lib;
using Donations.Lib.Model;
using Donations.Lib.View;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using Xunit;

namespace Donations.Tests;

public partial class CategoryReviewViewModelTests : TestBase
{
	[StaTheory]
	[InlineData(1, 10000)]
	[InlineData(3, 15000)]
	[InlineData(6, 25500)]
	public void CategorySum(int batchId, double expected)
	{
		// Arrange
		var td = new TestData();
		var factory = DependencyInjection.Resolve<CategoryReviewView.Factory>();
#pragma warning disable CS8604 // Possible null reference argument.
		var donations = new ObservableCollection<Donation>(td.DonationList.Where(x => x.BatchId == batchId));
#pragma warning restore CS8604 // Possible null reference argument.
		CategorySum categorySum = new CategorySum()
		{
			Category = donations[0].Category
		};
		var filteredDonations = new ObservableCollection<Donation>(donations.Where(x => x.Category == categorySum.Category));
		categorySum.Sum = filteredDonations.Sum(x => x.Value);
#pragma warning disable CS8604 // Possible null reference argument.
		CategoryReviewView obj = factory(categorySum, enumCategoryReviewType.Batch, filteredDonations, filteredDonations[0].Date);
#pragma warning restore CS8604 // Possible null reference argument.
		CategoryReviewViewModel vm = obj.DataContext as CategoryReviewViewModel;

		// Assert
#pragma warning disable CS8629 // Nullable value type may be null.
		Assert.Equal(expected, Math.Round(vm.Sum.Value, 2));
#pragma warning restore CS8629 // Nullable value type may be null.
	}

	[Fact]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async void SetupNulls()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		// Arrange
		CategoryReviewViewModel obj = DependencyInjection.Resolve<CategoryReviewViewModel>();
		string expectedTimeWindowText = "Time window text";

		// Act
		obj.Setup(null, null, expectedTimeWindowText);

		// Assert
		Assert.Equal(expectedTimeWindowText, obj.Date);
		var donations = obj.Source.Source as ObservableCollection<Donation>;
		Assert.Null(donations);
	}

	[Theory]
	[InlineData("1 tithe")]
	[InlineData("2 two")]
	[InlineData("3 three")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async void Setup(string category)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		// Arrange
		var td = new TestData();
		CategorySum categorySum = new CategorySum()
		{
			Category = category
		};
#pragma warning disable CS8604 // Possible null reference argument.
		var filteredDonations = new ObservableCollection<Donation>(td.DonationList.Where(x => x.Category == categorySum.Category));
#pragma warning restore CS8604 // Possible null reference argument.
		categorySum.Sum = filteredDonations.Sum(x => x.Value);
		CategoryReviewViewModel obj = DependencyInjection.Resolve<CategoryReviewViewModel>();
		string expectedTimeWindowText = "Time window text";

		// Act
		obj.Setup(categorySum, filteredDonations, expectedTimeWindowText);

		// Assert
		Assert.Equal(expectedTimeWindowText, obj.Date);
		Assert.Equal(categorySum.Category, obj.CategoryDescription);
		var actual_donations = obj.Source.Source as ObservableCollection<Donation>;
#pragma warning disable CS8604 // Possible null reference argument.
		Assert.Equal(categorySum.Sum, actual_donations.Sum(x => x.Value));
#pragma warning restore CS8604 // Possible null reference argument.
	}

	[StaTheory]
	[InlineData("1 tithe", "Calibri", 14)]
	[InlineData("2 two", "Arial", 13)]
	[InlineData("3 three", "Segoe UI", 12)]
	public void CreatePreview(string category, string font, double size)
	{
		// Arrange
		var td = new TestData();
		CategorySum categorySum = new CategorySum()
		{
			Category = category
		};
#pragma warning disable CS8604 // Possible null reference argument.
		var filteredDonations = new ObservableCollection<Donation>(td.DonationList.Where(x => x.Category == categorySum.Category));
#pragma warning restore CS8604 // Possible null reference argument.
		categorySum.Sum = filteredDonations.Sum(x => x.Value);
		CategoryReviewViewModel obj = DependencyInjection.Resolve<CategoryReviewViewModel>();
		string expectedTimeWindowText = "Time window text";
		var doc = new FlowDocument();

		// Act
		obj.Setup(categorySum, filteredDonations, expectedTimeWindowText);
		obj.CreatePreview(doc, font, size, 7.5 * PrintOptionsView._dpi);

		// Assert
		Assert.Equal(2, doc.Blocks.Count);
		Assert.Equal(typeof(Paragraph), doc.Blocks.FirstBlock.GetType());
		Assert.Equal(typeof(Table), doc.Blocks.LastBlock.GetType());
		var firstPar = doc.Blocks.FirstBlock as Paragraph;
		Assert.Equal(font, firstPar.FontFamily.ToString());
		var lastPar = doc.Blocks.LastBlock as Table;
		Assert.Equal(font, lastPar.RowGroups[0].Rows[0].FontFamily.ToString());
		Assert.Equal(size, lastPar.RowGroups[0].Rows[0].FontSize);
		Assert.Equal(category, ((Run)((Paragraph)lastPar.RowGroups[0].Rows[1].Cells[1].Blocks.FirstBlock).Inlines.FirstInline).Text);
	}
}
