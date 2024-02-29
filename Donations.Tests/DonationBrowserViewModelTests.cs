using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using Xunit;

namespace Donations.Tests;

public class DonationBrowserViewModelTests : TestBase
{
	[Theory]
	[InlineData(enumDateFilterOptions.CurrentYear)]
	[InlineData(enumDateFilterOptions.PreviousYear)]
	[InlineData(enumDateFilterOptions.SelectYear)]
	[InlineData(enumDateFilterOptions.DateRange)]
	public void DateFilterOption(enumDateFilterOptions option)
	{
		// Arrange
		DonationBrowserViewModel viewModel = DependencyInjection.Resolve<DonationBrowserViewModel>();
		viewModel.DateFilterOption = option;

		// Act

		// Assert
		Assert.Equal(option, viewModel.DateFilterOption);
	}

	[Theory]
	[InlineData("Anystring")]
	[InlineData("2022")]
	[InlineData("2020")]
	public void FilterYear(string filter)
	{
		// Arrange
		DonationBrowserViewModel viewModel = DependencyInjection.Resolve<DonationBrowserViewModel>();
		viewModel.FilterYear = filter;

		// Act

		// Assert
		Assert.Equal(filter, viewModel.FilterYear);
	}

	[Fact]
	public async Task DonationListUpdated()
	{
		// Arrange
		IDonationServices donationServices = DependencyInjection.Resolve<IDonationServices>();

		// enter new donation list to see if filter date endpoints change
		var newDonations = new ObservableCollection<Donation>();
		newDonations.Add(new Donation() { Date = "1990/1/1" });
		newDonations.Add(new Donation() { Date = "12/31/2022" });

		await donationServices.SaveDonations(newDonations);

		DonationBrowserViewModel obj = DependencyInjection.Resolve<DonationBrowserViewModel>();

		// Act
		await obj.TimeWindowChanged();

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
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();

		// hack to clear test data from batch service
		batchServices.SaveBatches(new ObservableCollection<Batch>());

		DonationBrowserViewModel viewModel = DependencyInjection.Resolve<DonationBrowserViewModel>();
		viewModel.FilterStartDate = filter;

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
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();

		// hack to clear test data from batch service
		batchServices.SaveBatches(new ObservableCollection<Batch>());

		DonationBrowserViewModel viewModel = DependencyInjection.Resolve<DonationBrowserViewModel>();
		viewModel.FilterEndDate = filter;

		// Act

		// Assert
		Assert.Equal(expected, viewModel.FilterEndDate);
	}

	[Fact]
	public async Task Loading()
	{
		// Arrange
		var td = new TestData();
#pragma warning disable CS8604 // Possible null reference argument.
		var donationsIn2023 = td.DonationList.Where(x => x.Date.StartsWith("2023"));
#pragma warning restore CS8604 // Possible null reference argument.
		var categoriesIn2023 = donationsIn2023.Select(x => x.Category);
		var donationIdsIn2023 = donationsIn2023.Select(x => x.Id);
		DonationBrowserViewModel viewModel = DependencyInjection.Resolve<DonationBrowserViewModel>();
		viewModel.DateFilterOption = enumDateFilterOptions.SelectYear;
		viewModel.FilterYear = "2023";

		// Act
		await viewModel.Loading();

		// Assert
		var donations = viewModel.DonationSource.Source as ObservableCollection<Donation>;
		var categories = viewModel.CategorySource.Source as ObservableCollection<CategorySum>;

		foreach (var cat in categories)
		{
			Assert.Contains($"{cat.Code} {cat.Description}", categoriesIn2023);
		}

		foreach (var donation in donations)
		{
			Assert.Contains(donation.Id, donationIdsIn2023);
		}
	}

}
