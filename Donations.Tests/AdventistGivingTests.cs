using Donations.Lib;
using Donations.Lib.Model;
using Xunit;

namespace Donations.Tests;

public class AdventistGivingTests : TestBase
{
	[Theory]
	[InlineData("first", "last", "address", "99999")]
	[InlineData("first", "last", "address", null)]
	[InlineData("first", "last", null, "99999")]
	[InlineData("first", "last", null, null)]
	[InlineData("first", null, "address", "99999")]
	[InlineData("first", null, "address", null)]
	[InlineData("first", null, null, "99999")]
	[InlineData("first", null, null, null)]
	[InlineData(null, "last", "address", "99999")]
	[InlineData(null, "last", "address", null)]
	[InlineData(null, "last", null, "99999")]
	[InlineData(null, "last", null, null)]
	[InlineData(null, null, "address", "99999")]
	[InlineData(null, null, "address", null)]
	[InlineData(null, null, null, "99999")]
	[InlineData(null, null, null, null)]
	[InlineData("", "last", "address", "99999")]
	[InlineData("", "", "address", "99999")]
	[InlineData("", "", "", "99999")]
	[InlineData("", "", "", "")]
	public void AGHash(string? first, string? last, string? address, string? zip)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { FirstName = first, LastName = last, Address = address, Zip = zip };
		string expected = last + first + address + zip;

		// Act
		string actual = Helper.AGHash(ag);

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("donorHash")]
	[InlineData(null)]
	[InlineData("")]
	public void DonorHash(string? donorHash)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { DonorHash = donorHash };
		string? expected = donorHash;

		// Act
		string? actual = ag.DonorHash;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("LastName")]
	[InlineData(null)]
	[InlineData("")]
	public void LastName(string? lastName)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { LastName = lastName };
		string? expected = lastName;

		// Act
		string? actual = ag.LastName;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("FirstName")]
	[InlineData(null)]
	[InlineData("")]
	public void FirstName(string? firstName)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { FirstName = firstName };
		string? expected = firstName;

		// Act
		string? actual = ag.FirstName;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("Address")]
	[InlineData(null)]
	[InlineData("")]
	public void Address(string? address)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { Address = address };
		string? expected = address;

		// Act
		string? actual = ag.Address;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("Address2")]
	[InlineData(null)]
	[InlineData("")]
	public void Address2(string? address2)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { Address2 = address2 };
		string? expected = address2;

		// Act
		string? actual = ag.Address2;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("City")]
	[InlineData(null)]
	[InlineData("")]
	public void City(string? city)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { City = city };
		string? expected = city;

		// Act
		string? actual = ag.City;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("State")]
	[InlineData(null)]
	[InlineData("")]
	public void State(string? state)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { State = state };
		string? expected = state;

		// Act
		string? actual = ag.State;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("Zip")]
	[InlineData(null)]
	[InlineData("")]
	public void Zip(string? zip)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { Zip = zip };
		string? expected = zip;

		// Act
		string? actual = ag.Zip;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("Country")]
	[InlineData(null)]
	[InlineData("")]
	public void Country(string? country)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { Country = country };
		string? expected = country;

		// Act
		string? actual = ag.Country;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("TransactionId")]
	[InlineData(null)]
	[InlineData("")]
	public void TransactionId(string? transactionId)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { TransactionId = transactionId };
		string? expected = transactionId;

		// Act
		string? actual = ag.TransactionId;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("TransactionType")]
	[InlineData(null)]
	[InlineData("")]
	public void TransactionType(string? transactionType)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { TransactionType = transactionType };
		string? expected = transactionType;

		// Act
		string? actual = ag.TransactionType;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("TransactionDate")]
	[InlineData(null)]
	[InlineData("")]
	public void TransactionDate(string? transactionDate)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { TransactionDate = transactionDate };
		string? expected = transactionDate;

		// Act
		string? actual = ag.TransactionDate;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(12345.67)]
	[InlineData(0)]
	[InlineData(-12345.67)]
	public void TransactionTotal(double transactionTotal)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { TransactionTotal = transactionTotal };
		double expected = transactionTotal;

		// Act
		double actual = ag.TransactionTotal;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(10101)]
	[InlineData(-11111)]
	public void CategoryCode(int categoryCode)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { CategoryCode = categoryCode };
		int expected = categoryCode;

		// Act
		int actual = ag.CategoryCode;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("CategoryName")]
	[InlineData(null)]
	[InlineData("")]
	public void CategoryName(string? categoryName)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { CategoryName = categoryName };
		string? expected = categoryName;

		// Act
		string? actual = ag.CategoryName;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData(12345.67)]
	[InlineData(0)]
	[InlineData(-12345.67)]
	public void Amount(double amount)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { Amount = amount };
		double expected = amount;

		// Act
		double actual = ag.Amount;

		// Assert
		Assert.Equal(expected, actual);
	}

	[Theory]
	[InlineData("Tithe/Diezmo/Dîme", "Tithe")]
	[InlineData("World Budget / Presupuesto mundial / Budget mondial", "World Budget")]
	[InlineData("Global Mission (GM) - 10/40 Window / Misión Global (GM) - Ventana 10/40 / Mission mondiale - Fenêtre 10/40", "Global Mission (GM) - 10/40 Window")]
	[InlineData("single", "single")]
	[InlineData("", "")]
	[InlineData(null, "")]
	public void SplitCategoryName(string? categoryName, string expected)
	{
		// Arrange
		AdventistGiving ag = new AdventistGiving() { CategoryName = categoryName };

		// Act
		string actual = ag.SplitCategoryName;

		// Assert
		Assert.Equal(expected, actual);
	}
}
