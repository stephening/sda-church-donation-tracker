using Autofac;
using Donations.Lib;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using Xunit;

namespace Donations.Tests;

public class BatchPrintViewModelTests : TestBase
{
	[Theory]
	[InlineData("This is a font family name...")]
	[InlineData(null)]
	[InlineData("")]
	public void SelectedFont(string? param)
	{
		// Arrange
		BatchPrintViewModel obj = DependencyInjection.Resolve<BatchPrintViewModel>();
		obj.SelectedFont = param;
		string? expected = param;

		// Act
		string? actual = obj.SelectedFont;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(10000)]
	[InlineData(0.9999999999999999)]
	[InlineData(99999999999999999)]
	[InlineData(-1000.1111111111111)]
	public void SelectedSize(double param)
	{
		// Arrange
		BatchPrintViewModel obj = DependencyInjection.Resolve<BatchPrintViewModel>();
		obj.SelectedSize = param;
		double expected = param;

		// Act
		double actual = obj.SelectedSize;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(10000)]
	[InlineData(0.9999999999999999)]
	[InlineData(99999999999999999)]
	[InlineData(-1000.1111111111111)]
	public void LeftMargin(double param)
	{
		// Arrange
		BatchPrintViewModel obj = DependencyInjection.Resolve<BatchPrintViewModel>();
		obj.LeftMargin = param;
		double expected = param;

		// Act
		double actual = obj.LeftMargin;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(10000)]
	[InlineData(0.9999999999999999)]
	[InlineData(99999999999999999)]
	[InlineData(-1000.1111111111111)]
	public void OtherMargins(double param)
	{
		// Arrange
		BatchPrintViewModel obj = DependencyInjection.Resolve<BatchPrintViewModel>();
		obj.OtherMargins = param;
		double expected = param;

		// Act
		double actual = obj.OtherMargins;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Fact]
	public async Task Loaded()
	{
		// Arrange
		int i;
		var td = new TestData();
		BatchPrintViewModel obj = DependencyInjection.Resolve<BatchPrintViewModel>();
		FlowDocument flowdoc = new FlowDocument();
		Batch batch = td.BatchDict![3];
		ObservableCollection<Donation> donations = new ObservableCollection<Donation>(td.DonationList!.Where(x => x.BatchId == batch.Id));

		// Act
		await obj.Loaded(flowdoc, batch, donations);

		// Assert
		ObservableCollection<CategorySum> catsum = new ObservableCollection<CategorySum>(obj.CategorySumSource.View.Cast<CategorySum>().ToList());

		for (i = 0; i < donations.Count; i++)
		{
			Assert.Equal(catsum[i].Category, donations[i].Category);
			Assert.Equal(catsum[i].Sum, donations[i].Value);
		}

		i = 0;
		foreach (Donation donation in obj.DonationDetailsSource.View)
		{
			Assert.Equal(donation.Category, donations[i].Category);
			Assert.Equal(donation.Value, donations[i].Value);
			Assert.Equal(donation.Name, donations[i].Name);
			Assert.Equal(donation.Note, donations[i].Note);
			Assert.Equal(donation.Date, donations[i].Date);
			Assert.Equal(donation.TaxDeductible, donations[i].TaxDeductible);
			Assert.Equal(donation.Method, donations[i].Method);
			i++;
		}
	}
}
