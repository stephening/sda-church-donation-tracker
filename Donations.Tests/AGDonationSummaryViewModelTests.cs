using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using Xunit;

namespace Donations.Tests
{
	public partial class DataAccessSequentialTests
	{
		[Theory]
		[InlineData(10000)]
		[InlineData(0.9999999999999999)]
		[InlineData(99999999999999999)]
		[InlineData(-1000.1111111111111)]
		public void AGDonationSummaryViewModel_Target(double param)
		{
			// Arronge
			AGDonationSummaryViewModel obj = new AGDonationSummaryViewModel() { Total = param };
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
		public void AGDonationSummaryViewModel_BatchDate(string? param, string expected)
		{
			// Arronge
			AGDonationSummaryViewModel obj = new AGDonationSummaryViewModel() { BatchDate = param };

			// Act
			string actual = obj.BatchDate;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("2005/01/02", true)]
		[InlineData("", false)]
		public void AGDonationSummaryViewModel_SubmitEnabled(string param, bool expected)
		{
			// Arronge
			AGDonationSummaryViewModel obj = new AGDonationSummaryViewModel() { BatchDate = param };

			// Act
			bool actual = obj.SubmitEnabled;

			// Assert
			Assert.False(actual);

			// Arrange again
			obj.CategorySums.Add(new CategorySum());

			// Assert
			Assert.Equal(expected, obj.SubmitEnabled);
		}

		[Fact]
		public async Task AGDonationSummaryViewModel_Loaded_SaveBatch()
		{
			// Arrange
			int i;
			var td = new TestData();
			di.Data.CatList = new ObservableCollection<Category>();
			di.Data.CatDict = new Dictionary<int, Category>();
			di.Data.DonationList = new ObservableCollection<Donation>();
			di.Data.DonationDict = new Dictionary<int, Donation>();
			di.Data.AGDonorMap = new Dictionary<string, AGDonorMapItem>();
			di.Data.AGCategoryMap = new Dictionary<int, AGCategoryMapItem>();
			di.Data.BatchDict = new Dictionary<int, Batch>();
			di.Data.BatchList = new ObservableCollection<Batch>();

			AdventistGivingViewModel ag = new AdventistGivingViewModel() { TransactionList = new ObservableCollection<AdventistGiving>() };
			AGDonorResolutionViewModel dres = new AGDonorResolutionViewModel() { DonorResolutionComplete = Visibility.Visible, DonorDiffsVisibility = Visibility.Hidden };
			AGCategoryResolutionViewModel cres = new AGCategoryResolutionViewModel() { CategoryResolutionComplete = Visibility.Visible, CategoryDiffsVisibility = Visibility.Hidden };
			AGDonationSummaryViewModel obj = new AGDonationSummaryViewModel() { BatchDate = "2022/12/31" };

			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			di.Data.AGDonorMap["DoeJane1235 Acme Lane48124"] = new AGDonorMapItem() { DonorId = 2, AGAddress = "1235 Acme Lane", AGCity = "Dearborn", AGState = "MI", AGZip = "48124" };

			ag.TransactionList = td.AdventistGivingList;

			var txListCopy = new ObservableCollection<AdventistGiving>(ag.TransactionList);

			double total = 0;
			Dictionary<string, CategorySum> catsum = new Dictionary<string, CategorySum>();
			List<CategorySum> catsumlist = new List<CategorySum>();

			// Add all categories
			for (i = 0; i < ag.TransactionList.Count; i++)
			{
				ag.TransactionList[i].DonorHash = AdventistGiving.AGHash(ag.TransactionList[i]);
				total += ag.TransactionList[i].Amount;

				if (!di.Data.AGCategoryMap.ContainsKey(ag.TransactionList[i].CategoryCode))
				{
					// add maps for odd
					if (0 == (i % 2))
					{
						// Add category map also to exercise that code
						di.Data.AGCategoryMap[ag.TransactionList[i].CategoryCode] = new AGCategoryMapItem() { CategoryCode = ag.TransactionList[i].CategoryCode * 100, AGCategoryCode = ag.TransactionList[i].CategoryCode, AGCategoryName = ag.TransactionList[i].CategoryName };
						// change category in map so it won't match and map will be used
						di.Data.CatDict[ag.TransactionList[i].CategoryCode * 100] = new Category() { Code = ag.TransactionList[i].CategoryCode * 100, Description = ag.TransactionList[i].SplitCategoryName, TaxDeductible = true };
						di.Data.CatList.Add(di.Data.CatDict[ag.TransactionList[i].CategoryCode * 100]);
					}
					else
					{
						di.Data.CatDict[ag.TransactionList[i].CategoryCode] = new Category() { Code = ag.TransactionList[i].CategoryCode, Description = ag.TransactionList[i].SplitCategoryName, TaxDeductible = true };
						di.Data.CatList.Add(di.Data.CatDict[ag.TransactionList[i].CategoryCode]);
					}
				}

				string catkey = $"{ag.TransactionList[i].CategoryCode} {ag.TransactionList[i].CategoryName}";
				if (di.Data.AGCategoryMap.ContainsKey(ag.TransactionList[i].CategoryCode))
				{
					catkey = $"{di.Data.AGCategoryMap[ag.TransactionList[i].CategoryCode].CategoryCode} {di.Data.AGCategoryMap[ag.TransactionList[i].CategoryCode].CategoryDescription}";
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
			Assert.Equal(catsumlist.Count, obj.CategorySums.Count);
			Assert.Equal(total, Math.Round(obj.Total, 2));
			for (i = 0; i < catsum.Count; i++)
			{
				Assert.Equal(catsumlist[i].Category, obj.CategorySums[i].Category);
				Assert.Equal(Math.Round(catsumlist[i].Sum, 2), Math.Round(obj.CategorySums[i].Sum, 2));
			}

			Assert.Equal(1, di.Data.AGDonorMap.Count);
			for (i = 0; i < ag.TransactionList.Count; i++)
			{
				if (di.Data.AGDonorMap.ContainsKey(ag.TransactionList[i].DonorHash))
				{
					Assert.Equal(di.Data.DonorDict[di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].DonorId].LastName, di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].LastName);
					Assert.Equal(di.Data.DonorDict[di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].DonorId].FirstName, di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].FirstName);
					Assert.Equal(di.Data.DonorDict[di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].DonorId].Address, di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].Address);
					Assert.Equal(di.Data.DonorDict[di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].DonorId].City, di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].City);
					Assert.Equal(di.Data.DonorDict[di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].DonorId].State, di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].State);
					Assert.Equal(di.Data.DonorDict[di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].DonorId].Zip, di.Data.AGDonorMap[ag.TransactionList[i].DonorHash].Zip);
				}
			}

			// Act again
			// save batch
			obj.SubmitBatchCmd.Execute(null);

			// Assert again
			for (i = 0; i < txListCopy.Count; i++)
			{
				string firstName = txListCopy[i].FirstName;
				string lastName = txListCopy[i].LastName;

				if (di.Data.AGDonorMap.ContainsKey(txListCopy[i].DonorHash))
				{
					firstName = di.Data.AGDonorMap[txListCopy[i].DonorHash].FirstName;
					lastName = di.Data.AGDonorMap[txListCopy[i].DonorHash].LastName;
				}
				Assert.Equal(di.Data.DonationList[i].LastName, lastName);
				Assert.Equal(di.Data.DonationList[i].FirstName, firstName);
				Assert.Equal(DateOnly.Parse(di.Data.DonationList[i].Date), DateOnly.Parse(txListCopy[i].TransactionDate));
				Assert.Equal(di.Data.DonationList[i].TransactionNumber, txListCopy[i].TransactionId);
				Assert.Equal(di.Data.DonationList[i].Method, enumMethod.AdventistGiving);
				Assert.Equal(di.Data.DonationList[i].Value, txListCopy[i].Amount);
				string category = $"{txListCopy[i].CategoryCode} {txListCopy[i].CategoryName}";
				if (di.Data.AGCategoryMap.ContainsKey(txListCopy[i].CategoryCode))
				{
					category = $"{di.Data.AGCategoryMap[txListCopy[i].CategoryCode].CategoryCode} {di.Data.AGCategoryMap[txListCopy[i].CategoryCode].CategoryDescription}";
				}
				Assert.Equal(di.Data.DonationList[i].Category, category);
			}

		}
	}
}
