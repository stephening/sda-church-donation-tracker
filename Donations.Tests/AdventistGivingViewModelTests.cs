using Donations.Lib;
using Donations.Lib.ViewModel;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace Donations.Tests;

public class AdventistGivingViewModelTests : TestBase
{
	[Theory]
	[InlineData("This is a batch note...")]
	[InlineData(null)]
	[InlineData("")]
	public void BatchNote(string? param)
	{
		// Arrange
		AdventistGivingViewModel agvm = DependencyInjection.Resolve<AdventistGivingViewModel>();
#pragma warning disable CS8601 // Possible null reference assignment.
		agvm.BatchNote = param;
#pragma warning restore CS8601 // Possible null reference assignment.
		string? expected = param;

		// Act
		string? actual = agvm.BatchNote;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(10000)]
	[InlineData(0.9999999999999999)]
	[InlineData(99999999999999999)]
	[InlineData(-1000.1111111111111)]
	public void TargetTotal(double param)
	{
		// Arrange
		AdventistGivingViewModel agvm = DependencyInjection.Resolve<AdventistGivingViewModel>();
		agvm.TargetTotal = param;
		double expected = param;

		// Act
		double actual = agvm.TargetTotal;

		// Assert
		Assert.Equal(expected, actual);
	}

	/// <summary>
	/// Other Import tests are in SequentialTests because the nature of the mocked StreamReader
	/// doesn't allow multiple instances to run in prallel.
	/// </summary>
	/// <param name="path"></param>
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public async void AdventistGivingViewModel_ImportNullOrEmptyFilename(string? path)
	{
		// Arrange
		AdventistGivingViewModel giving = DependencyInjection.Resolve<AdventistGivingViewModel>();

		// Act
		Func<Task> act = async () => await giving.Import(path);

		// Assert
		Exception ex = await Assert.ThrowsAsync<Exception>(act);
	}

	[Fact]
	public async void AdventistGiving_ImportEmptyFile()
	{
		// Arrange
		var mockfs = new MockFileSystem(new Dictionary<string, MockFileData>
						{
							{ @"ag.csv", new MockFileData(new byte[] { }) }
						});

		AutofacRegister(mockfs);

		// Act
		AdventistGivingViewModel obj = DependencyInjection.Resolve<AdventistGivingViewModel>();

		// Act
		await obj.Import("ag.csv");
		var collection = obj.TransactionList;

		// Assert
#pragma warning disable CS8604 // Possible null reference argument.
		Assert.Empty(collection);
#pragma warning restore CS8604 // Possible null reference argument.
	}

	[Fact]
	public async void AdventistGiving_ImportSuccess()
	{
		// Arrange
		int i;
		var td = new TestData();
		byte[] buffer = Encoding.UTF8.GetBytes(td.AdventistGivingCsv);
		var mockfs = new MockFileSystem(new Dictionary<string, MockFileData>
						{
							{ @"ag.csv", new MockFileData(buffer) }
						});

		AutofacRegister(mockfs);

		AdventistGivingViewModel obj = DependencyInjection.Resolve<AdventistGivingViewModel>();

		// Act
		await obj.Import("ag.csv");
		var collection = obj.TransactionList;

		// Assert
		Assert.Equal(td.AdventistGivingList!.Count, collection!.Count);
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
	public async void AdventistGiving_ImportException()
	{
		// Arrange
		string[] input = new string[]
		{
			"Organization ID,Organization Name,ACH Transfer Date,Donor ID,First Name,Last Name,Address1,Address2,City,State,Postal Code,Country,Transaction ID,Transaction Type,Transaction Created At,Transaction Total,Code,Category Name,Amount\r\n",
			"Church,,,John,Doe,1234 Acme Lane,,Horrendous,State,98765,USA,10398360,credit,2022-12-11,2000.0,10000,Tithe/Diezmo/Dîme,1000.0\r\n",
		};
		byte[] buffer = Encoding.UTF8.GetBytes(string.Join("", input));
		var mockfs = new MockFileSystem(new Dictionary<string, MockFileData>
						{
							{ @"ag.csv", new MockFileData(buffer) }
						});

		AutofacRegister(mockfs);

		AdventistGivingViewModel obj = DependencyInjection.Resolve<AdventistGivingViewModel>();
		string exMessage = "";

		// Act
		try
		{
			await obj.Import("ag.csv");
		}
		catch (Exception ex)
		{
			exMessage = ex.Message;
		}

		// Assert
		Assert.Equal("Line split into 18 but should be 19.", exMessage);
	}

	[Fact]
	public async Task AdventistGivingViewModel_ImportSuccess()
	{
		// Arrange
		int i;
		var td = new TestData();
		byte[] buffer = Encoding.UTF8.GetBytes(td.AdventistGivingCsv);
		var mockfs = new MockFileSystem(new Dictionary<string, MockFileData>
						{
							{ @"ag.csv", new MockFileData(buffer) }
						});

		AutofacRegister(mockfs);

		AdventistGivingViewModel obj = DependencyInjection.Resolve<AdventistGivingViewModel>();

		// Act
		await obj.Import("ag.csv");

		// Assert
		Assert.Equal(td.AdventistGivingList!.Count, obj.TransactionList!.Count);
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
		// Arrange
		var td = new TestData();
		byte[] buffer = Encoding.UTF8.GetBytes(td.AdventistGivingCsv);
		var mockfs = new MockFileSystem(new Dictionary<string, MockFileData>
						{
							{ @"ag.csv", new MockFileData(buffer) }
						});

		AutofacRegister(mockfs);

		AdventistGivingViewModel giving = DependencyInjection.Resolve<AdventistGivingViewModel>();

		// Act
		await giving.Import("ag.csv");
		giving.Reset();

		// Assert
		Assert.Empty(giving.TransactionList!);
		Assert.Equal("", giving.BatchNote);
	}
}
