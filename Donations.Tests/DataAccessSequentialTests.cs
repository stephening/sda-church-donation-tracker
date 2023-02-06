using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using Xunit;
using System.IO.Abstractions.TestingHelpers;

namespace Donations.Tests
{
	[CollectionDefinition(nameof(DataAccessSequentialTests), DisableParallelization = true)]
	public partial class DataAccessSequentialTests
	{

		[Fact]
		public void AdventistGiving_ImportSuccess()
		{
			// Arronge
			int i;
			var td = new TestData();
			byte[] buffer = Encoding.UTF8.GetBytes(td.AdventistGivingCsv);
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"ag.csv", new MockFileData(buffer) }
			}); 

			// Act
			var collection = AdventistGiving.Import("ag.csv");

			// Assert
			Assert.Equal(td.AdventistGivingList.Count, collection.Count);
			for (i = 0; i < collection.Count; i++)
			{
				Assert.True(Helper.Equal(td.AdventistGivingList[i].FirstName, collection[i].FirstName));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].LastName, collection[i].LastName));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].Address, collection[i].Address));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].Address2, collection[i].Address2));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].City, collection[i].City));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].State, collection[i].State));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].Zip, collection[i].Zip));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].Country, collection[i].Country));
				Assert.Equal(td.AdventistGivingList[i].TransactionId, collection[i].TransactionId);
				Assert.Equal(td.AdventistGivingList[i].TransactionType, collection[i].TransactionType);
				Assert.True(Helper.Equal(td.AdventistGivingList[i].TransactionDate, collection[i].TransactionDate, eFlags.Date));
				Assert.Equal(td.AdventistGivingList[i].TransactionTotal, collection[i].TransactionTotal);
				Assert.Equal(td.AdventistGivingList[i].CategoryCode, collection[i].CategoryCode);
				Assert.Equal(td.AdventistGivingList[i].CategoryName, collection[i].CategoryName);
				Assert.Equal(td.AdventistGivingList[i].Amount, collection[i].Amount);
				Assert.Equal(td.AdventistGivingList[i].SplitCategoryName, collection[i].SplitCategoryName);
				Assert.Equal(td.AdventistGivingList[i].DonorHash, collection[i].DonorHash);
			}
			double total = 0;
			for (i = 0; i < 2; i++)
			{
				total += collection[i].Amount;
			}
			Assert.Equal(collection[0].TransactionTotal, total);
			Assert.Equal(collection[1].TransactionTotal, total);

			total = 0;
			for (i = 3; i < 6; i++)
			{
				total += collection[i].Amount;
			}
			Assert.Equal(collection[3].TransactionTotal, total);
			Assert.Equal(collection[4].TransactionTotal, total);
			Assert.Equal(collection[5].TransactionTotal, total);
		}

		[Fact]
		public void AdventistGiving_ImportEmptyFile()
		{
			// Arronge
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"ag.csv", new MockFileData(new byte[] { }) }
			});

			// Act
			var collection = AdventistGiving.Import("ag.csv");

			// Assert
			Assert.Empty(collection);
		}

		[Fact]
		public void AdventistGiving_ImportException()
		{
			// Arronge
			string[] input = new string[]
			{
				"Organization ID,Organization Name,ACH Transfer Date,Donor ID,First Name,Last Name,Address1,Address2,City,State,Postal Code,Country,Transaction ID,Transaction Type,Transaction Created At,Transaction Total,Code,Category Name,Amount\r\n",
				"Church,,,John,Doe,1234 Acme Lane,,Horrendous,State,98765,USA,10398360,credit,2022-12-11,2000.0,10000,Tithe/Diezmo/Dîme,1000.0\r\n",
			};
			byte[] buffer = Encoding.UTF8.GetBytes(string.Join("", input));
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"ag.csv", new MockFileData(buffer) }
			});

			// Act
			Action act = () => AdventistGiving.Import("ag.csv");

			// Assert
			Exception ex = Assert.Throws<Exception>(act);
			Assert.Equal("Line split into 18 but should be 19.", ex.Message);
		}

		[Fact]
		public async Task AdventistGivingViewModel_ImportSuccess()
		{
			// Arronge
			int i;
			var td = new TestData();
			byte[] buffer = Encoding.UTF8.GetBytes(td.AdventistGivingCsv);
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"ag.csv", new MockFileData(buffer) }
			});
			AdventistGivingViewModel obj = new AdventistGivingViewModel();

			// Act
			await obj.Import("ag.csv");

			// Assert
			Assert.Equal(td.AdventistGivingList.Count, obj.TransactionList.Count);
			for (i = 0; i < obj.TransactionList.Count; i++)
			{
				Assert.True(Helper.Equal(td.AdventistGivingList[i].FirstName, obj.TransactionList[i].FirstName));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].LastName, obj.TransactionList[i].LastName));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].Address, obj.TransactionList[i].Address));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].Address2, obj.TransactionList[i].Address2));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].City, obj.TransactionList[i].City));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].State, obj.TransactionList[i].State));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].Zip, obj.TransactionList[i].Zip));
				Assert.True(Helper.Equal(td.AdventistGivingList[i].Country, obj.TransactionList[i].Country));
				Assert.Equal(td.AdventistGivingList[i].TransactionId, obj.TransactionList[i].TransactionId);
				Assert.Equal(td.AdventistGivingList[i].TransactionType, obj.TransactionList[i].TransactionType);
				Assert.True(Helper.Equal(td.AdventistGivingList[i].TransactionDate, obj.TransactionList[i].TransactionDate, eFlags.Date));
				Assert.Equal(td.AdventistGivingList[i].TransactionTotal, obj.TransactionList[i].TransactionTotal);
				Assert.Equal(td.AdventistGivingList[i].CategoryCode, obj.TransactionList[i].CategoryCode);
				Assert.Equal(td.AdventistGivingList[i].CategoryName, obj.TransactionList[i].CategoryName);
				Assert.Equal(td.AdventistGivingList[i].Amount, obj.TransactionList[i].Amount);
				Assert.Equal(td.AdventistGivingList[i].SplitCategoryName, obj.TransactionList[i].SplitCategoryName);
				Assert.Equal(td.AdventistGivingList[i].DonorHash, obj.TransactionList[i].DonorHash);
			}
			double total = 0;
			for (i = 0; i < 2; i++)
			{
				total += obj.TransactionList[i].Amount;
			}
			Assert.Equal(obj.TransactionList[0].TransactionTotal, total);
			Assert.Equal(obj.TransactionList[1].TransactionTotal, total);

			total = 0;
			for (i = 3; i < 6; i++)
			{
				total += obj.TransactionList[i].Amount;
			}
			Assert.Equal(obj.TransactionList[3].TransactionTotal, total);
			Assert.Equal(obj.TransactionList[4].TransactionTotal, total);
			Assert.Equal(obj.TransactionList[5].TransactionTotal, total);
		}

		[Fact]
		public async Task AdventistGivingViewModel_ImportReset()
		{
			// Arronge
			int i;
			var td = new TestData();
			byte[] buffer = Encoding.UTF8.GetBytes(td.AdventistGivingCsv);
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"ag.csv", new MockFileData(buffer) }
			});
			AdventistGivingViewModel giving = new AdventistGivingViewModel();

			// Act
			await giving.Import("ag.csv");
			giving.Reset();

			// Assert
			Assert.Empty(giving.TransactionList);
			Assert.Equal("", giving.BatchNote);
		}

		[Fact]
		public async Task AGCategoryResolutionViewModel_Resolve()
		{
			// Arrange
			var td = new TestData();
			di.Data.CatList = new ObservableCollection<Category>();
			di.Data.CatDict = new Dictionary<int, Category>();
			di.Data.AGCategoryMap = new Dictionary<int, AGCategoryMapItem>();

			AdventistGivingViewModel ag = new AdventistGivingViewModel();
			AGCategoryResolutionViewModel catres = new AGCategoryResolutionViewModel() { CategoryResolutionComplete = Visibility.Visible, CategoryDiffsVisibility = Visibility.Hidden };

			ag.TransactionList = td.AdventistGivingList;

			// Add some stuff to the category list so we can resolve
			// Tithe with same category code but without other languages
			di.Data.CatDict[di.AG.TransactionList[0].CategoryCode] = new Category() { Code = di.AG.TransactionList[0].CategoryCode, Description = di.AG.TransactionList[0].SplitCategoryName, TaxDeductible = true };
			di.Data.CatList.Add(di.Data.CatDict[di.AG.TransactionList[0].CategoryCode]);
			// With different category code, but part of the description, should get partial match
			di.Data.CatDict[di.AG.TransactionList[1].CategoryCode * 100] = new Category() { Code = di.AG.TransactionList[1].CategoryCode * 100, Description = di.AG.TransactionList[1].CategoryName + " stuff", TaxDeductible = true };
			di.Data.CatList.Add(di.Data.CatDict[di.AG.TransactionList[1].CategoryCode * 100]);
			// with same code but different name
			di.Data.CatDict[di.AG.TransactionList[4].CategoryCode] = new Category() { Code = di.AG.TransactionList[4].CategoryCode, Description = "Some other fund", TaxDeductible = false };
			di.Data.CatList.Add(di.Data.CatDict[di.AG.TransactionList[4].CategoryCode]);
			// exact match
			di.Data.CatDict[di.AG.TransactionList[5].CategoryCode] = new Category() { Code = di.AG.TransactionList[5].CategoryCode, Description = di.AG.TransactionList[5].CategoryName, TaxDeductible = true };
			di.Data.CatList.Add(di.Data.CatDict[di.AG.TransactionList[5].CategoryCode]);
			// different code, but superset of split name
			di.Data.CatDict[di.AG.TransactionList[7].CategoryCode * 100] = new Category() { Code = di.AG.TransactionList[7].CategoryCode * 100, Description = "pre " + di.AG.TransactionList[7].CategoryName + " post", TaxDeductible = true };
			di.Data.CatList.Add(di.Data.CatDict[di.AG.TransactionList[7].CategoryCode * 100]);

			// Act
			await catres.StartCategoryResolution();

			// Assert
			Assert.StartsWith("Record 2 of", catres.ProgressText);
			Assert.NotNull(catres.Cat);
			Assert.Contains(catres.Transaction.CategoryName, catres.Cat.Description);
			Assert.Equal(di.AG.TransactionList[1], catres.Transaction);

			// Act again
			// continue
			await catres.ContinueCategoryResolutionCmd.ExecuteAsync(null);

			// Assert again
			Assert.StartsWith("Record 5 of", catres.ProgressText);
			Assert.NotNull(catres.Cat);
			Assert.Equal(di.AG.TransactionList[4], catres.Transaction);

			// Act again
			// continue, should create a map entry for the close match above
			await catres.ContinueCategoryResolutionCmd.ExecuteAsync(null);

			// Assert again
			Assert.StartsWith("Record 8 of", catres.ProgressText);
			Assert.NotNull(catres.Cat);
			Assert.Equal(di.AG.TransactionList[7], catres.Transaction);

			// Act again
			// continue, should create a map entry for the close match above
			await catres.ContinueCategoryResolutionCmd.ExecuteAsync(null);

			// Assert
			Assert.Equal(Visibility.Visible, catres.CategoryResolutionComplete);
			Assert.Equal(Visibility.Hidden, catres.CategoryDiffsVisibility);
		}

		[Fact]
		public async Task AGDonorResolutionViewModel_Resolve()
		{
			// Arrange
			di.Data.AGDonorMap = new Dictionary<string, AGDonorMapItem>();

			AdventistGivingViewModel ag = new AdventistGivingViewModel() { TransactionList = new ObservableCollection<AdventistGiving>() };
			AGDonorResolutionViewModel dres = new AGDonorResolutionViewModel() { DonorResolutionComplete = Visibility.Visible, DonorDiffsVisibility = Visibility.Hidden };

			var td = new TestData();
			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			// first should be exact match for Donor.Id=1
			ag.TransactionList.Add(new AdventistGiving() { FirstName = "John", LastName = "Doe", Address = "1234 Acme Lane", City = "Dearborn", State = "MI", Zip = "48124", Country = "USA", TransactionId = "10398360", TransactionType = "credit", TransactionDate = "2022-12-11", TransactionTotal = 2000.0, CategoryCode = 10000, CategoryName = "Tithe/Diezmo/Dîme", Amount = 1000.0 });
			// close match to Donor.Id=2 because of middle initial
			ag.TransactionList.Add(new AdventistGiving() { FirstName = "Jane", LastName = "Doe", Address = "1235 Acme Lane", City = "Dearborn", State = "MI", Zip = "48124", Country = "USA", TransactionId = "10398361", TransactionType = "credit", TransactionDate = "2022-12-11", TransactionTotal = 2000.0, CategoryCode = 10001, CategoryName = "Church Budget", Amount = 500.0 });
			// close match to Donor.Id=3 because of address difference
			ag.TransactionList.Add(new AdventistGiving() { FirstName = "Johnny", LastName = "Doe", Address = "1236 Acme Lane", City = "Dearborn", State = "MI", Zip = "48124", Country = "USA", TransactionId = "10398362", TransactionType = "credit", TransactionDate = "2022-12-11", TransactionTotal = 2000.0, CategoryCode = 10002, CategoryName = "Student Assistance", Amount = 500.0 });
			// close match to Donor.Id=4 because of city difference
			ag.TransactionList.Add(new AdventistGiving() { FirstName = "Martin", LastName = "Luther", Address = "1430 Luther Circle", City = "Eislebon", State = "State", Zip = "98765", Country = "Germany", TransactionId = "14739380", TransactionType = "credit", TransactionDate = "2022-12-08", TransactionTotal = 5550.0, CategoryCode = 10000, CategoryName = "Tithe/Diezmo/Dîme", Amount = 1230.0 });
			// close match to Donor.Id=5 because of zip difference
			ag.TransactionList.Add(new AdventistGiving() { FirstName = "John", LastName = "Wycliffe", Address = "1330 Wycliffe Lane", City = "Oxford", State = "United Kingdom", Zip = "98766", Country = "England", TransactionId = "14739381", TransactionType = "credit", TransactionDate = "2022-12-08", TransactionTotal = 5550.0, CategoryCode = 777, CategoryName = "Building fund", Amount = 350.4 });
			// exact match for john calvin
			ag.TransactionList.Add(new AdventistGiving() { FirstName = "John", LastName = "Calvin", Address = "1509 Calvin Ct", City = "Johnsville", State = "Geneva", Zip = "56789", Country = "Switzerland", TransactionId = "14739382", TransactionType = "credit", TransactionDate = "2022-12-08", TransactionTotal = 5550.0, CategoryCode = 1235, CategoryName = "Missionary assistance", Amount = 2468.20 });
			// no match for john hus
			ag.TransactionList.Add(new AdventistGiving() { FirstName = "John", LastName = "Hus", Address = "1494 Tyndale Way", City = "Gloucester", State = "Gloucestershire", Zip = "56789", Country = "England", TransactionId = "14739383", TransactionType = "credit", TransactionDate = "2022-12-08", TransactionTotal = 5550.0, CategoryCode = 10011, CategoryName = "World Budget / Presupuesto mundial / Budget mondial", Amount = 1501.4 });

			// set donor hash which is usually set on import
			for (int i = 0; i < ag.TransactionList.Count; i++)
			{
				ag.TransactionList[i].DonorHash = AdventistGiving.AGHash(ag.TransactionList[i]);
			}

			// Act
			await dres.StartNameResolution();

			// Assert
			Assert.StartsWith("Record 2 of", dres.ProgressText);
			Assert.NotNull(dres.Donor);
			Assert.Contains(dres.Transaction.FirstName, dres.Donor.FirstName); // Lokking for "Jane" in "Jane J"
			Assert.Equal(dres.Donor.LastName, dres.Transaction.LastName);
			Assert.Equal(dres.Donor.Address, dres.Transaction.Address);
			Assert.Equal(dres.Donor.City, dres.Transaction.City);
			Assert.Equal(dres.Donor.State, dres.Transaction.State);
			Assert.Equal(dres.Donor.Zip, dres.Transaction.Zip);

			// Act again
			// continue
			await dres.ContinueDonorResolutionCmd.ExecuteAsync(null);

			// Assert again
			Assert.StartsWith("Record 3 of", dres.ProgressText);
			Assert.NotNull(dres.Donor);
			Assert.Equal(dres.Donor.FirstName, dres.Transaction.FirstName);
			Assert.Equal(dres.Donor.LastName, dres.Transaction.LastName);
			Assert.NotEqual(dres.Donor.Address, dres.Transaction.Address);
			Assert.Equal(dres.Donor.City, dres.Transaction.City);
			Assert.Equal(dres.Donor.State, dres.Transaction.State);
			Assert.Equal(dres.Donor.Zip, dres.Transaction.Zip);

			// Act again
			await dres.ContinueDonorResolutionCmd.ExecuteAsync(null);

			// Assert again
			Assert.StartsWith("Record 4 of", dres.ProgressText);
			Assert.NotNull(dres.Donor);
			Assert.Equal(dres.Donor.FirstName, dres.Transaction.FirstName);
			Assert.Equal(dres.Donor.LastName, dres.Transaction.LastName);
			Assert.Equal(dres.Donor.Address, dres.Transaction.Address);
			Assert.NotEqual(dres.Donor.City, dres.Transaction.City);
			Assert.Equal(dres.Donor.State, dres.Transaction.State);
			Assert.Equal(dres.Donor.Zip, dres.Transaction.Zip);

			// Act again
			// continue, should create a map entry for the close match above
			await dres.ContinueDonorResolutionCmd.ExecuteAsync(null);

			// Assert again
			Assert.StartsWith("Record 5 of", dres.ProgressText);
			Assert.NotNull(dres.Donor);
			Assert.Equal(dres.Donor.FirstName, dres.Transaction.FirstName);
			Assert.Equal(dres.Donor.LastName, dres.Transaction.LastName);
			Assert.Equal(dres.Donor.Address, dres.Transaction.Address);
			Assert.Equal(dres.Donor.City, dres.Transaction.City);
			Assert.Equal(dres.Donor.State, dres.Transaction.State);
			Assert.NotEqual(dres.Donor.Zip, dres.Transaction.Zip);

			// Act again
			// continue, should create a map entry for the close match above
			await dres.ContinueDonorResolutionCmd.ExecuteAsync(null);

			// Assert again
			Assert.StartsWith("Record 7 of", dres.ProgressText);
			Assert.Null(dres.Donor);

			// Act again
			dres.CopyAllCmd.Execute(null);

			// Assert
			Assert.Equal(Visibility.Visible, dres.DonorResolutionComplete);
			Assert.Equal(Visibility.Hidden, dres.DonorDiffsVisibility);
		}
	}
}
