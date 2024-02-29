using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using Xunit;

namespace Donations.Tests;

public class AGDonationSummaryViewModelTests : TestBase
{
	[Theory]
	[InlineData(10000)]
	[InlineData(0.9999999999999999)]
	[InlineData(99999999999999999)]
	[InlineData(-1000.1111111111111)]
	public void Target(double param)
	{
		// Arrange
		AGDonationSummaryViewModel obj = DependencyInjection.Resolve<AGDonationSummaryViewModel>();
		obj.Total = param;
		double expected = param;

		// Act
		double actual = obj.Total;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("1/2/2005", "2005/01/02")]
	[InlineData("March 23, 2023", "2023/03/23")]
	[InlineData("5-5-2000", "2000/05/05")]
	[InlineData(null, "")]
	public void BatchDate(string? param, string expected)
	{
		// Arrange
		AGDonationSummaryViewModel obj = DependencyInjection.Resolve<AGDonationSummaryViewModel>();
#pragma warning disable CS8601 // Possible null reference assignment.
		obj.BatchDate = param;
#pragma warning restore CS8601 // Possible null reference assignment.

		// Act
		string? actual = obj.BatchDate;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("2005/01/02", true)]
	[InlineData("", false)]
	public void SubmitEnabled(string param, bool expected)
	{
		// Arrange
		AGDonationSummaryViewModel obj = DependencyInjection.Resolve<AGDonationSummaryViewModel>();
		obj.BatchDate = param;

		// Act
		bool actual = obj.SubmitEnabled;

		// Assert
		Assert.False(actual);

		// Arrange again
		obj.CategorySums!.Add(new CategorySum());

		// Assert
		Assert.Equal(expected, obj.SubmitEnabled);
	}

	[Fact]
	public async Task Loaded_SaveBatch()
	{
		// Arrange
		int i;
		var td = new TestData();
		ICategoryServices categoryServices = DependencyInjection.Resolve<ICategoryServices>();
		categoryServices!.CatList = new ObservableCollection<Category>();
		categoryServices.CatDict = new Dictionary<int, Category>();
		IDonationServices donationServices = DependencyInjection.Resolve<IDonationServices>();
		IDonorMapServices donorMapServices = DependencyInjection.Resolve<IDonorMapServices>();
		donorMapServices!.AGDonorMap = new Dictionary<string, AGDonorMapItem>();
		ICategoryMapServices categoryMapServices = DependencyInjection.Resolve<ICategoryMapServices>();
		categoryMapServices!.AGCategoryMap = new Dictionary<int, AGCategoryMapItem>();
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();

		// clear the test data from the donations service
		await donationServices.SaveDonations(new ObservableCollection<Donation>());
		// clear the test data from the batch service
		await batchServices.SaveBatches(new ObservableCollection<Batch>());

		AdventistGivingViewModel ag = DependencyInjection.Resolve<AdventistGivingViewModel>();
		ag.TransactionList = new ObservableCollection<AdventistGiving>();
		AGDonorResolutionViewModel dres = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		dres.DonorResolutionComplete = Visibility.Visible;
		dres.DonorDiffsVisibility = Visibility.Hidden;
		AGCategoryResolutionViewModel cres = DependencyInjection.Resolve<AGCategoryResolutionViewModel>();
		cres.CategoryResolutionComplete = Visibility.Visible;
		cres.CategoryDiffsVisibility = Visibility.Hidden;
		AGDonationSummaryViewModel obj = DependencyInjection.Resolve<AGDonationSummaryViewModel>();
		obj.BatchDate = "2022/12/31";

		IDonorServices donorServices = DependencyInjection.Resolve<IDonorServices>();

		donorMapServices.AGDonorMap["DoeJane1235 Acme Lane48124"] = new AGDonorMapItem()
		{
			DonorId = 2,
			LastName = donorServices.GetDonorById(2).LastName,
			FirstName = donorServices.GetDonorById(2).FirstName,
			Address = donorServices.GetDonorById(2).Address,
			City = donorServices.GetDonorById(2).City,
			State = donorServices.GetDonorById(2).State,
			Zip = donorServices.GetDonorById(2).Zip,
			AGAddress = "1235 Acme Lane",
			AGCity = "Dearborn",
			AGState = "MI",
			AGZip = "48124"
		};

		ag.TransactionList = td.AdventistGivingList;

		var txListCopy = new ObservableCollection<AdventistGiving>(ag.TransactionList!);

		double total = 0;
		Dictionary<string, CategorySum> catsum = new Dictionary<string, CategorySum>();
		List<CategorySum> catsumlist = new List<CategorySum>();

		// Add all categories
		for (i = 0; i < ag.TransactionList!.Count; i++)
		{
			ag.TransactionList[i].DonorHash = Helper.AGHash(ag.TransactionList[i]);
			total += ag.TransactionList[i].Amount;

			if (!categoryMapServices.AGCategoryMap.ContainsKey(ag.TransactionList[i].CategoryCode))
			{
				// add maps for odd
				if (0 == (i % 2))
				{
					// change category in map so it won't match and map will be used
					categoryServices.CatDict[ag.TransactionList[i].CategoryCode * 100] = new Category()
					{
						Code = ag.TransactionList[i].CategoryCode * 100,
						Description = ag.TransactionList[i].SplitCategoryName,
						TaxDeductible = true
					};
					categoryServices.CatList.Add(categoryServices.CatDict[ag.TransactionList[i].CategoryCode * 100]);

					// Add category map also to exercise that code
					categoryMapServices.AGCategoryMap[ag.TransactionList[i].CategoryCode] = new AGCategoryMapItem()
					{
						CategoryCode = ag.TransactionList[i].CategoryCode * 100,
						CategoryDescription = categoryServices.GetCategoryDescription(ag.TransactionList[i].CategoryCode * 100),
						AGCategoryCode = ag.TransactionList[i].CategoryCode,
						AGCategoryName = ag.TransactionList[i].CategoryName
					};
				}
				else
				{
					categoryServices.CatDict[ag.TransactionList[i].CategoryCode] = new Category()
					{
						Code = ag.TransactionList[i].CategoryCode,
						Description = ag.TransactionList[i].SplitCategoryName,
						TaxDeductible = true
					};
					categoryServices.CatList.Add(categoryServices.CatDict[ag.TransactionList[i].CategoryCode]);
				}
			}

			string catkey = $"{ag.TransactionList[i].CategoryCode} {ag.TransactionList[i].CategoryName}";
			if (categoryMapServices.AGCategoryMap.ContainsKey(ag.TransactionList[i].CategoryCode))
			{
				catkey = $"{categoryMapServices.AGCategoryMap[ag.TransactionList[i].CategoryCode].CategoryCode} {categoryMapServices.AGCategoryMap[ag.TransactionList[i].CategoryCode].CategoryDescription}";
			}

			if (catsum.ContainsKey(catkey))
			{
				catsum[catkey].Sum += ag.TransactionList[i].Amount;
			}
			else
			{
				catsum[catkey] = new CategorySum() { Sum = ag.TransactionList[i].Amount, Category = catkey };
				catsumlist.Add(catsum[catkey]);
			}
		}

		// Act
		await obj.Loaded();

		// Assert
		Assert.Equal(catsumlist.Count, obj.CategorySums!.Count);
		Assert.Equal(total, Math.Round(obj.Total, 2));
		for (i = 0; i < catsum.Count; i++)
		{
			Assert.Equal(catsumlist[i].Category, obj.CategorySums[i].Category);
			Assert.Equal(Math.Round(catsumlist[i].Sum, 2), Math.Round(obj.CategorySums[i].Sum, 2));
		}

		Assert.Single(donorMapServices.AGDonorMap);
		for (i = 0; i < ag.TransactionList.Count; i++)
		{
			if (donorMapServices.AGDonorMap.ContainsKey(ag.TransactionList[i].DonorHash!))
			{
				Assert.Equal(donorServices.GetDonorById(donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].DonorId).LastName, donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].LastName);
				Assert.Equal(donorServices.GetDonorById(donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].DonorId).FirstName, donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].FirstName);
				Assert.Equal(donorServices.GetDonorById(donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].DonorId).Address, donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].Address);
				Assert.Equal(donorServices.GetDonorById(donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].DonorId).City, donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].City);
				Assert.Equal(donorServices.GetDonorById(donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].DonorId).State, donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].State);
				Assert.Equal(donorServices.GetDonorById(donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].DonorId).Zip, donorMapServices.AGDonorMap[ag.TransactionList[i].DonorHash!].Zip);
			}
		}

		// Act again
		// save batch
		obj.SubmitBatchCommand.Execute(null);

		// Assert again
		var donations = await donationServices.LoadDonations();
		for (i = 0; i < txListCopy.Count; i++)
		{
			string firstName = txListCopy[i].FirstName;
			string lastName = txListCopy[i].LastName;

#pragma warning disable CS8604 // Possible null reference argument.
			if (donorMapServices.AGDonorMap.ContainsKey(txListCopy[i].DonorHash))
			{
#pragma warning disable CS8604 // Possible null reference argument.
				firstName = donorMapServices.AGDonorMap[txListCopy[i].DonorHash].FirstName;
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
				lastName = donorMapServices.AGDonorMap[txListCopy[i].DonorHash].LastName;
#pragma warning restore CS8604 // Possible null reference argument.
			}
#pragma warning restore CS8604 // Possible null reference argument.
			var donor = donorServices.GetDonorById(donations[i].DonorId);
			Assert.Equal(donor.LastName, lastName);
			Assert.Equal(donor.FirstName, firstName);
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
			Assert.Equal(DateOnly.Parse(donations[i].Date), DateOnly.Parse(txListCopy[i].TransactionDate));
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8604 // Possible null reference argument.
			Assert.Equal(donations[i].TransactionNumber, txListCopy[i].TransactionId);
			Assert.Equal(donations[i].Method, enumMethod.AdventistGiving);
			Assert.Equal(donations[i].Value, txListCopy[i].Amount);
			string category = $"{txListCopy[i].CategoryCode} {txListCopy[i].CategoryName}";
			if (categoryMapServices.AGCategoryMap.ContainsKey(txListCopy[i].CategoryCode))
			{
				category = $"{categoryMapServices.AGCategoryMap[txListCopy[i].CategoryCode].CategoryCode} {categoryMapServices.AGCategoryMap[txListCopy[i].CategoryCode].CategoryDescription}";
			}
			Assert.Equal(donations[i].Category, category);
		}

	}
}
