using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;
using Xunit;

namespace Donations.Tests;

public class AGDonorResolutionViewModelTests : TestBase
{
	[Fact]
	public void Donor_Success()
	{
		// Arrange
		Donor donor = new Donor() { LastName = "Doe", FirstName = "John", Address = "1234 Acme Lane", City = "Pearly Gates", State = "State", Zip = "98765" };
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		obj.Donor = donor;
		Donor expected = donor;

		// Act
		Donor actual = obj.Donor;

		// Assert
		Assert.Equal(expected, actual);
		Assert.Equal(donor.LastName, obj.Donor.LastName);
		Assert.Equal(donor.FirstName, obj.Donor.FirstName);
		Assert.Equal(donor.Address, obj.Donor.Address);
	}

	[Theory]
	[InlineData("string1", "string1", Visibility.Hidden)]
	[InlineData("string1", "string2", Visibility.Visible)]
	[InlineData(null, "string2", Visibility.Visible)]
	[InlineData("", "string2", Visibility.Visible)]
	public void CopyVisible(string? param1, string? param2, Visibility expectedVisibility)
	{
		// Arrange
		Donor donor = new Donor() { LastName = param1, FirstName = param1, Address = param1, Address2 = param1, City = param1, State = param1, Zip = param1 };
		AdventistGiving transaction = new AdventistGiving() { LastName = param2, FirstName = param2, Address = param2, Address2 = param2, City = param2, State = param2, Zip = param2 };
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		obj.Donor = donor;
		obj.Transaction = transaction;

		// Act

		// Assert
		Assert.Equal(expectedVisibility, obj.LastNameCopyVisible);
		Assert.Equal(expectedVisibility, obj.FirstNameCopyVisible);
		Assert.Equal(expectedVisibility, obj.AddressCopyVisible);
		Assert.Equal(expectedVisibility, obj.Address2CopyVisible);
		Assert.Equal(expectedVisibility, obj.CityCopyVisible);
		Assert.Equal(expectedVisibility, obj.StateCopyVisible);
		Assert.Equal(expectedVisibility, obj.ZipCopyVisible);
		Assert.Equal(expectedVisibility, obj.CopyAllVisible);

		if (Visibility.Hidden == obj.CopyAllVisible)
		{
			obj.Donor.LastName = "somethig";
			Assert.Equal(Visibility.Visible, obj.CopyAllVisible);
		}
	}

	[Theory]
	[InlineData("string1")]
	[InlineData("")]
	public void CopyCommands(string? param)
	{
		// Arrange
		Donor donor = new Donor();
		AdventistGiving transaction = new AdventistGiving() { LastName = param, FirstName = param, Address = param, Address2 = param, City = param, State = param, Zip = param };
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		obj.Donor = donor;
		obj.Transaction = transaction;

		// Act

		// Assert
		Assert.NotEqual(donor.LastName, obj.Transaction.LastName);
		Assert.NotEqual(donor.FirstName, obj.Transaction.FirstName);
		Assert.NotEqual(donor.Address, obj.Transaction.Address);
		Assert.NotEqual(donor.Address2, obj.Transaction.Address2);
		Assert.NotEqual(donor.City, obj.Transaction.City);
		Assert.NotEqual(donor.State, obj.Transaction.State);
		Assert.NotEqual(donor.Zip, obj.Transaction.Zip);

		// Act again
		obj.CopyLastNameCommand.Execute(null);
		obj.CopyFirstNameCommand.Execute(null);
		obj.CopyAddressCommand.Execute(null);
		obj.CopyAddress2Command.Execute(null);
		obj.CopyCityCommand.Execute(null);
		obj.CopyStateCommand.Execute(null);
		obj.CopyZipCommand.Execute(null);

		// Assert again
		Assert.Equal(donor.LastName, obj.Transaction.LastName);
		Assert.Equal(donor.FirstName, obj.Transaction.FirstName);
		Assert.Equal(donor.Address, obj.Transaction.Address);
		Assert.Equal(donor.Address2, obj.Transaction.Address2);
		Assert.Equal(donor.City, obj.Transaction.City);
		Assert.Equal(donor.State, obj.Transaction.State);
		Assert.Equal(donor.Zip, obj.Transaction.Zip);

		// Act again
		donor = new Donor();
		obj.Donor = donor;

		// Assert
		Assert.NotEqual(donor.LastName, obj.Transaction.LastName);
		Assert.NotEqual(donor.FirstName, obj.Transaction.FirstName);
		Assert.NotEqual(donor.Address, obj.Transaction.Address);
		Assert.NotEqual(donor.Address2, obj.Transaction.Address2);
		Assert.NotEqual(donor.City, obj.Transaction.City);
		Assert.NotEqual(donor.State, obj.Transaction.State);
		Assert.NotEqual(donor.Zip, obj.Transaction.Zip);

		// Act again
		obj.CopyAllCommand.Execute(null);

		// Assert again
		Assert.Equal(donor.LastName, obj.Transaction.LastName);
		Assert.Equal(donor.FirstName, obj.Transaction.FirstName);
		Assert.Equal(donor.Address, obj.Transaction.Address);
		Assert.Equal(donor.Address2, obj.Transaction.Address2);
		Assert.Equal(donor.City, obj.Transaction.City);
		Assert.Equal(donor.State, obj.Transaction.State);
		Assert.Equal(donor.Zip, obj.Transaction.Zip);
	}

	[Fact]
	public void Donor_Null()
	{
		// Arrange
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		Donor? expected = null;

		// Act
		Donor? actual = obj.Donor;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Transaction_Success()
	{
		// Arrange
		AdventistGiving transaction = new AdventistGiving();
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		obj.Transaction = transaction;
		AdventistGiving expected = transaction;

		// Act
		AdventistGiving actual = obj.Transaction;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void Transaction_Null()
	{
		// Arrange
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		AdventistGiving? expected = null;

		// Act
		AdventistGiving? actual = obj.Transaction;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("")]
	[InlineData(null)]
	public void Transaction_NullFirstName(string? param)
	{
		// Arrange
		AdventistGiving transaction = new AdventistGiving() { FirstName = param };
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		obj.Transaction = transaction;

		// Act
		AdventistGiving actual = obj.Transaction;

		// Assert
		Assert.Equal(transaction, actual);
		Assert.True(string.IsNullOrEmpty(obj.Transaction.LastName));
	}

	[Theory]
	[InlineData("This is progress text...")]
	[InlineData(null)]
	[InlineData("")]
	public void ProgressText(string? param)
	{
		// Arrange
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		obj.ProgressText = param;
		string? expected = param;

		// Act
		string? actual = obj.ProgressText;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ContinueEnabled_True()
	{
		// Arrange
		Donor donor = new Donor();
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		obj.Donor = donor;

		// Act
		bool actual = obj.ContinueEnabled;

		// Assert
		Assert.True(actual);
	}

	[Fact]
	public void ContinueEnabled_False()
	{
		// Arrange
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		obj.Donor = null;

		// Act
		bool actual = obj.ContinueEnabled;

		// Assert
		Assert.False(actual);
	}

	[Theory]
	[InlineData(Visibility.Collapsed)]
	[InlineData(Visibility.Visible)]
	[InlineData(Visibility.Hidden)]
	public void DonorDiffsVisibility(Visibility param)
	{
		// Arrange
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		obj.DonorDiffsVisibility = param;
		Visibility expected = param;

		// Act
		Visibility actual = obj.DonorDiffsVisibility;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(Visibility.Collapsed)]
	[InlineData(Visibility.Visible)]
	[InlineData(Visibility.Hidden)]
	public void DonorResolutionComplete(Visibility param)
	{
		// Arrange
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		obj.DonorResolutionComplete = param;
		Visibility expected = param;

		// Act
		Visibility actual = obj.DonorResolutionComplete;

		// Assert
		Assert.Equal(expected, actual);
	}


	[Theory]
	[InlineData(true, Visibility.Visible, Visibility.Hidden)]
	[InlineData(false, Visibility.Hidden, Visibility.Visible)]
	public void ResolutionComplete(bool flag, Visibility expectedDonorResolutionComplete, Visibility expectedDonorDiffsVisibility)
	{
		// Arrange
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		bool expected = flag;

		// Act
		obj.ResolutionComplete(flag);

		// Assert
		Assert.Equal(expectedDonorResolutionComplete, obj.DonorResolutionComplete);
		Assert.Equal(expectedDonorDiffsVisibility, obj.DonorDiffsVisibility);
	}

	[Fact]
	public void ChooseDonor()
	{
		// Arrange
		Donor expected = new Donor();
		AGDonorResolutionViewModel obj = DependencyInjection.Resolve<AGDonorResolutionViewModel>();

		// Act
		Donor? actualBefore = obj.Donor;
		obj.ChooseDonor(expected);
		Donor? actualAfter = obj.Donor;

		// Assert
		Assert.Null(actualBefore);
		Assert.Equal(expected, actualAfter);
	}

	[Fact]
	public async Task AGDonorResolutionViewModel_Resolve()
	{
		// Arrange
		IDonorMapServices donorMapServices = DependencyInjection.Resolve<IDonorMapServices>();
		donorMapServices!.AGDonorMap!.Clear();

		AdventistGivingViewModel ag = DependencyInjection.Resolve<AdventistGivingViewModel>();
		ag.TransactionList = new ObservableCollection<AdventistGiving>();
		AGDonorResolutionViewModel dres = DependencyInjection.Resolve<AGDonorResolutionViewModel>();
		dres.DonorResolutionComplete = Visibility.Visible;
		dres.DonorDiffsVisibility = Visibility.Hidden;

		// first should be exact match for Donor.Id=1
		ag.TransactionList.Add(new AdventistGiving() { FirstName = "John", LastName = "Doe", Address = "1234 Acme Lane", City = "Deer", State = "OR", Zip = "48124", Country = "USA", TransactionId = "10398360", TransactionType = "credit", TransactionDate = "2022-12-11", TransactionTotal = 2000.0, CategoryCode = 10000, CategoryName = "Tithe/Diezmo/Dîme", Amount = 1000.0 });
		// close match to Donor.Id=2 because of middle initial
		ag.TransactionList.Add(new AdventistGiving() { FirstName = "Jane", LastName = "Doe", Address = "1235 Acme Lane", City = "Deer", State = "OR", Zip = "48124", Country = "USA", TransactionId = "10398361", TransactionType = "credit", TransactionDate = "2022-12-11", TransactionTotal = 2000.0, CategoryCode = 10001, CategoryName = "Church Budget", Amount = 500.0 });
		// close match to Donor.Id=3 because of address difference
		ag.TransactionList.Add(new AdventistGiving() { FirstName = "Johnny", LastName = "Doe", Address = "1236 Acme Lane", City = "Deer", State = "OR", Zip = "48124", Country = "USA", TransactionId = "10398362", TransactionType = "credit", TransactionDate = "2022-12-11", TransactionTotal = 2000.0, CategoryCode = 10002, CategoryName = "Student Assistance", Amount = 500.0 });
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
			ag.TransactionList[i].DonorHash = Helper.AGHash(ag.TransactionList[i]);
		}

		// Act
		await dres.StartNameResolution();

		// Assert
		Assert.StartsWith("Record 2 of", dres.ProgressText);
		Assert.NotNull(dres.Donor);
		Assert.Contains(dres.Transaction!.FirstName!, dres.Donor.FirstName); // Lokking for "Jane" in "Jane J"
		Assert.Equal(dres.Donor.LastName, dres.Transaction.LastName);
		Assert.Equal(dres.Donor.Address, dres.Transaction.Address);
		Assert.Equal(dres.Donor.City, dres.Transaction.City);
		Assert.Equal(dres.Donor.State, dres.Transaction.State);
		Assert.Equal(dres.Donor.Zip, dres.Transaction.Zip);

		// Act again
		// continue
		await dres.ContinueDonorResolutionCommand.ExecuteAsync(null);

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
		await dres.ContinueDonorResolutionCommand.ExecuteAsync(null);

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
		await dres.ContinueDonorResolutionCommand.ExecuteAsync(null);

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
		await dres.ContinueDonorResolutionCommand.ExecuteAsync(null);

		// Assert again
		Assert.StartsWith("Record 7 of", dres.ProgressText);
		Assert.Null(dres.Donor);

		// Act again
		dres.CopyAllCommand.Execute(null);

		// Assert
		Assert.Equal(Visibility.Visible, dres.DonorResolutionComplete);
		Assert.Equal(Visibility.Hidden, dres.DonorDiffsVisibility);
	}
}
