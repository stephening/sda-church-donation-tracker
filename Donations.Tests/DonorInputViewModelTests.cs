using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Xunit;

namespace Donations.Tests;

public class DonorInputViewModelTests : TestBase
{
	[Theory]
	[InlineData(123.45)]
	[InlineData(0.99)]
	[InlineData(-12.80)]
	public void TotalSum(double expected)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.TotalSum = expected;

		// Assert
		Assert.Equal(expected, Math.Round(obj.TotalSum, 2));
	}

	[Theory]
	[InlineData(123.45)]
	[InlineData(0.99)]
	[InlineData(-12.80)]
	public void BatchTotal(double expected)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.BatchTotal = expected;

		// Assert
		Assert.Equal(expected, Math.Round(obj.BatchTotal, 2));
	}

	[Fact]
	public void RunningTotal()
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		obj.SummaryList!.Add(new Summary() { FirstName = "John", LastName = "Doe", Subtotal = 5000 });
		obj.SummaryList.Add(new Summary() { FirstName = "Jane", LastName = "Doe", Subtotal = 5000 });
		obj.SummaryList.Add(new Summary() { FirstName = "Martin", LastName = "Luther", Subtotal = 100000 });

		// Assert
		Assert.Equal(110000, Math.Round(obj.RunningTotal, 2));
	}


	[Theory]
	[InlineData("1/2/2005", "2005/01/02")]
	[InlineData("March 23, 2023", "2023/03/23")]
	[InlineData("5-5-2000", "2000/05/05")]
	[InlineData(null, "")]
	public void BatchDate(string? param, string expected)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
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
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
#pragma warning disable CS8601 // Possible null reference assignment.
		obj.BatchNote = expected;
#pragma warning restore CS8601 // Possible null reference assignment.

		// Assert
		Assert.Equal(expected, obj.BatchNote);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void HasChanges(bool expected)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.HasChanges = expected;

		// Assert
		Assert.Equal(expected, obj.HasChanges);
	}

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void CanAddRows(bool expected)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.CanAddRows = expected;

		// Assert
		Assert.Equal(expected, obj.CanAddRows);
	}

	[Fact]
	public void RunningTotalColor()
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Act
		obj.BatchTotal = 1000;

		// Assert
		Assert.Equal(Brushes.LightPink, obj.RunningTotalColor);

		// Act
		obj.BatchTotal = 0;

		// Assert
		Assert.Equal(Brushes.LightGreen, obj.RunningTotalColor);
	}

	[Theory]
	[InlineData(enumMethod.Unknown)]
	[InlineData(enumMethod.Cash)]
	[InlineData(enumMethod.Check)]
	[InlineData(enumMethod.Card)]
	[InlineData(enumMethod.Mixed)]
	[InlineData(enumMethod.Online)]
	[InlineData(enumMethod.AdventistGiving)]
	public void MethodOptions(enumMethod param)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.MethodOptions = param;

		// Assert
		Assert.Equal(param, obj.MethodOptions);
	}

	[Fact]
	public void SubmitEnabled()
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Act
		obj.NameSelectionChanged(null);

		// Assert
		Assert.False(obj.SubmitEnabled);

		// Act again
		obj.NameSelectionChanged(new Donor() { LastName = "Doe", FirstName = "John" });
		obj.TotalSum = 0;

		// Assert again
		Assert.False(obj.SubmitEnabled);

		// Act again
		obj.TotalSum = 10;
		obj.BatchDate = "";

		// Assert again
		Assert.False(obj.SubmitEnabled);

		// Act again
		obj.BatchDate = "1/1/1980";
		obj.MethodOptions = enumMethod.Unknown;

		// Assert again
		Assert.False(obj.SubmitEnabled);

		// Act again
		obj.MethodOptions = enumMethod.Cash;

		// Assert again
		Assert.True(obj.SubmitEnabled);

		// Act again
		obj.MethodOptions = enumMethod.Check;
		obj.CheckNumber = "";

		// Assert again
		Assert.False(obj.SubmitEnabled);

		// Act again
		obj.CheckNumber = "10000";

		// Assert again
		Assert.True(obj.SubmitEnabled);
	}

	[Fact]
	public void SubmitBatchEnabled()
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Assert
		Assert.False(obj.SubmitBatchEnabled);

		// Act
		obj.HasChanges = true;
		obj.BatchDate = "1/1/1979";

		// Assert
		Assert.True(obj.SubmitBatchEnabled);
	}

	[Theory]
	[InlineData("John", "Doe", "Doe, John")]
	[InlineData("", "Doe", "Doe")]
	[InlineData("John", "", "John")]
	[InlineData(null, "Doe", "Doe")]
	[InlineData("John", null, "John")]
	public void Name(string? firstName, string? lastName, string expected)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Act
		obj.NameSelectionChanged(new Donor() { FirstName = firstName, LastName = lastName });

		// Assert
		Assert.Equal(expected, obj.Name);
	}

	[Theory]
	[InlineData("This is an address")]
	[InlineData(null)]
	[InlineData("")]
	public void Address(string? param)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Act
		obj.NameSelectionChanged(new Donor() { Address = param });

		// Assert
		Assert.Equal(param, obj.Address);
	}

	[Theory]
	[InlineData("This is the city")]
	[InlineData(null)]
	[InlineData("")]
	public void City(string? param)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Act
		obj.NameSelectionChanged(new Donor() { City = param });

		// Assert
		Assert.Equal(param, obj.City);
	}

	[Theory]
	[InlineData("This is the state")]
	[InlineData(null)]
	[InlineData("")]
	public void State(string? param)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Act
		obj.NameSelectionChanged(new Donor() { State = param });

		// Assert
		Assert.Equal(param, obj.State);
	}

	[Theory]
	[InlineData("99999")]
	[InlineData(null)]
	[InlineData("")]
	public void Zip(string? param)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Act
		obj.NameSelectionChanged(new Donor() { Zip = param });

		// Assert
		Assert.Equal(param, obj.Zip);
	}

	[Fact]
	public void CheckNumberEnabled()
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Act
		obj.MethodOptions = enumMethod.Check;

		// Assert
		Assert.True(obj.CheckNumberEnabled);

		// Act again
		obj.MethodOptions = enumMethod.AdventistGiving;

		// Assert again
		Assert.False(obj.CheckNumberEnabled);

		// Act again
		obj.MethodOptions = enumMethod.Cash;

		// Assert again
		Assert.False(obj.CheckNumberEnabled);
	}

	[Theory]
	[InlineData(enumMethod.Unknown, true)]
	[InlineData(enumMethod.Cash, true)]
	[InlineData(enumMethod.Check, true)]
	[InlineData(enumMethod.Card, true)]
	[InlineData(enumMethod.Mixed, true)]
	[InlineData(enumMethod.Online, true)]
	[InlineData(enumMethod.AdventistGiving, false)]
	public void NotAdventistGiving(enumMethod method, bool expected)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.MethodOptions = method;

		// Assert
		Assert.Equal(expected, obj.NotAdventistGiving);
	}

	[Theory]
	[InlineData("99999")]
	[InlineData(null)]
	[InlineData("")]
	public void CheckNumber(string? param)
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.CheckNumber = param;

		// Act

		// Assert
		Assert.Equal(param, obj.CheckNumber);
	}

	[Fact]
	public void SubmitDonor()
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.BatchDate = "1/1/2006";
		obj.IndividualDonations!.Add(new Donation() { Category = "1 tithe", Value = 100 });
		obj.IndividualDonations.Add(new Donation() { Category = "2 church budget", Value = 100 });
		obj.IndividualDonations.Add(new Donation() { Category = "3 tuition assistance", Value = 100 });

		// Act
		obj.NameSelectionChanged(new Donor() { LastName = "Doe", FirstName = "John" });
		obj.ValueChanged();
		obj.SubmitDonor();

		// Assert
		Assert.Equal("Doe, John", obj.SummaryList[0].Name);
		Assert.Equal(300, Math.Round(obj.RunningTotal, 2));
	}

	[Fact]
	public async void SubmitBatch()
	{
		// Arrange
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();

		IDonationServices donationServices = DependencyInjection.Resolve<IDonationServices>();
		var donations = await donationServices.LoadDonations();

		// clear the test data from the donations service
		await donationServices.SaveDonations(new ObservableCollection<Donation>());
		// clear the test data from the batch service
		await batchServices.SaveBatches(new ObservableCollection<Batch>());

		IDonorServices donorServices = DependencyInjection.Resolve<IDonorServices>();

		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.SummaryList!.Add(new Summary() { DonorId = 1, FirstName = donorServices.GetDonorById(1).FirstName, LastName = donorServices.GetDonorById(1).LastName, Subtotal = 1145.50 });
		obj.SummaryList.Add(new Summary() { DonorId = 2, FirstName = donorServices.GetDonorById(2).FirstName, LastName = donorServices.GetDonorById(2).LastName, Subtotal = 687.5 });
		obj.SummaryList.Add(new Summary() { DonorId = 3, FirstName = donorServices.GetDonorById(3).FirstName, LastName = donorServices.GetDonorById(3).LastName, Subtotal = 1850 });

		double summaryTotal = 0;
		foreach (var summaryItem in obj.SummaryList)
		{
			summaryTotal += summaryItem.Subtotal;
		}

		obj.SummaryList[0].ItemizedDonations = new ObservableCollection<Donation>();
		obj.SummaryList[0].ItemizedDonations!.Add(new Donation() { Category = "1 tithe", Value = 1000 });
		obj.SummaryList[0].ItemizedDonations!.Add(new Donation() { Category = "2 church budget", Value = 100 });
		obj.SummaryList[0].ItemizedDonations!.Add(new Donation() { Category = "3 tuition assistance", Value = 45.50 });
		obj.SummaryList[1].ItemizedDonations = new ObservableCollection<Donation>();
		obj.SummaryList[1].ItemizedDonations!.Add(new Donation() { Category = "1 tithe", Value = 500, DonorId = 2 });
		obj.SummaryList[1].ItemizedDonations!.Add(new Donation() { Category = "5 building fund", Value = 100 });
		obj.SummaryList[1].ItemizedDonations!.Add(new Donation() { Category = "6 outreach", Value = 87.50 });
		obj.SummaryList[2].ItemizedDonations = new ObservableCollection<Donation>();
		obj.SummaryList[2].ItemizedDonations!.Add(new Donation() { Category = "1 tithe", Value = 1500 });
		obj.SummaryList[2].ItemizedDonations!.Add(new Donation() { Category = "7 evangelism", Value = 100 });
		obj.SummaryList[2].ItemizedDonations!.Add(new Donation() { Category = "8 maintenance", Value = 250 });

		// Act
		obj.SubmitBatchCommand.Execute(null);

		// Assert
		donations = await donationServices.LoadDonations();
		Assert.Equal(9, donations.Count);
		int donationIdx = 0;
		int donorId = 1;
		Assert.Equal(donorServices.GetDonorById(donorId).LastName, donations[donationIdx + 0].LastName);
		Assert.Equal(donorServices.GetDonorById(donorId).LastName, donations[donationIdx + 1].LastName);
		Assert.Equal(donorServices.GetDonorById(donorId).LastName, donations[donationIdx + 2].LastName);
		Assert.Equal(donorServices.GetDonorById(donorId).FirstName, donations[donationIdx + 0].FirstName);
		Assert.Equal(donorServices.GetDonorById(donorId).FirstName, donations[donationIdx + 1].FirstName);
		Assert.Equal(donorServices.GetDonorById(donorId).FirstName, donations[donationIdx + 2].FirstName);
		Assert.Equal("1 tithe", donations[donationIdx + 0].Category);
		Assert.Equal("2 church budget", donations[donationIdx + 1].Category);
		Assert.Equal("3 tuition assistance", donations[donationIdx + 2].Category);

		donationIdx += 3;
		donorId = 2;
		Assert.Equal(donorServices.GetDonorById(donorId).LastName, donations[donationIdx + 0].LastName);
		Assert.Equal(donorServices.GetDonorById(donorId).LastName, donations[donationIdx + 1].LastName);
		Assert.Equal(donorServices.GetDonorById(donorId).LastName, donations[donationIdx + 2].LastName);
		Assert.Equal(donorServices.GetDonorById(donorId).FirstName, donations[donationIdx + 0].FirstName);
		Assert.Equal(donorServices.GetDonorById(donorId).FirstName, donations[donationIdx + 1].FirstName);
		Assert.Equal(donorServices.GetDonorById(donorId).FirstName, donations[donationIdx + 2].FirstName);
		Assert.Equal("1 tithe", donations[donationIdx + 0].Category);
		Assert.Equal("5 building fund", donations[donationIdx + 1].Category);
		Assert.Equal("6 outreach", donations[donationIdx + 2].Category);

		donationIdx += 3;
		donorId = 3;
		Assert.Equal(donorServices.GetDonorById(donorId).LastName, donations[donationIdx + 0].LastName);
		Assert.Equal(donorServices.GetDonorById(donorId).LastName, donations[donationIdx + 1].LastName);
		Assert.Equal(donorServices.GetDonorById(donorId).LastName, donations[donationIdx + 2].LastName);
		Assert.Equal(donorServices.GetDonorById(donorId).FirstName, donations[donationIdx + 0].FirstName);
		Assert.Equal(donorServices.GetDonorById(donorId).FirstName, donations[donationIdx + 1].FirstName);
		Assert.Equal(donorServices.GetDonorById(donorId).FirstName, donations[donationIdx + 2].FirstName);
		Assert.Equal("1 tithe", donations[donationIdx + 0].Category);
		Assert.Equal("7 evangelism", donations[donationIdx + 1].Category);
		Assert.Equal("8 maintenance", donations[donationIdx + 2].Category);

		double total = 0;
		foreach (var donation in donations)
		{
			total += donation.Value;
		}

		var batches = await batchServices.LoadBatches();
		Assert.Equal(Math.Round(batches[0].ActualTotal, 2), Math.Round(total, 2));
		Assert.Equal(Math.Round(batches[0].ActualTotal, 2), Math.Round(summaryTotal, 2));
	}

	[Fact]
	public async Task ReviewNullParameters()
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Assert
		await Assert.ThrowsAsync<ArgumentNullException>(async () => await obj.Review(null, new ObservableCollection<Donation>(), () => { }));
		await Assert.ThrowsAsync<ArgumentNullException>(async () => await obj.Review(new Batch(), null, () => { }));
	}

	[Fact]
	public async void ReviewWithDonors()
	{
		// Arrange
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();
		IDonationServices donationServices = DependencyInjection.Resolve<IDonationServices>();

		// clear test data from batch service
		await batchServices.SaveBatches(new ObservableCollection<Batch>());

		IDonorServices _donorServices = DependencyInjection.Resolve<IDonorServices>();

		await batchServices.AddBatch(new Batch() { Id = 1, Total = 61995.38 });

		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Act
		var donations = await donationServices.LoadDonations();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		obj.Review(await batchServices.GetBatchById(1), donations, () => { });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

		// Assert
		int summaryIdx = 0;
		int donationIdx = 0;
		Assert.Equal(donations![donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(donations[donationIdx + 1].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(10100, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 2;
		Assert.Equal(donations[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(500, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 1;
		Assert.Equal(donations[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(donations[donationIdx + 1].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(donations[donationIdx + 2].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(19500, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 3;
		Assert.Equal(donations[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(donations[donationIdx + 1].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(63, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 2;
		Assert.Equal(donations[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(87.5, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 1;
		Assert.Equal(donations[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(10730, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 4;
		Assert.Equal(donations[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(934.56, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 5;
		Assert.Equal(donations[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
		Assert.Equal(20080.32, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		double summaryTotal = 0;
		foreach (var summaryItem in obj.SummaryList)
		{
			summaryTotal += summaryItem.Subtotal;
		}

		double actualTotal = 0;
		foreach (var donation in donations)
		{
			actualTotal += donation.Value;
		}

		Assert.Equal(Math.Round(summaryTotal, 2), Math.Round(actualTotal, 2));
		var batches = await batchServices.LoadBatches();
		Assert.Equal(Math.Round(summaryTotal, 2), Math.Round(batches[0].Total, 2));
	}

	[Fact]
	public async void ReviewWithoutDonors()
	{
		// Arrange
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();
		IDonationServices donationServices = DependencyInjection.Resolve<IDonationServices>();

		// clear test data from batch service
		await batchServices.SaveBatches(new ObservableCollection<Batch>());

		await batchServices.AddBatch(new Batch() { Id = 1, Total = 61995.38 });

		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Act
		var donations = await donationServices.LoadDonations();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		obj.Review(await batchServices.GetBatchById(1), donations, () => { });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

		// Assert
		int summaryIdx = 0;
		int donationIdx = 0;
		Assert.Equal("Doe, John", obj.SummaryList[summaryIdx].Name);
		Assert.Equal("Doe, John", obj.SummaryList[summaryIdx].Name);
		Assert.Equal(10100, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 2;
		Assert.Equal("Doe, Jane J", obj.SummaryList[summaryIdx].Name);
		Assert.Equal(500, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 1;
		Assert.Equal("Doe, Johnny", obj.SummaryList[summaryIdx].Name);
		Assert.Equal("Doe, Johnny", obj.SummaryList[summaryIdx].Name);
		Assert.Equal("Doe, Johnny", obj.SummaryList[summaryIdx].Name);
		Assert.Equal(19500, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 3;
		Assert.Equal("Luther, Martin", obj.SummaryList[summaryIdx].Name);
		Assert.Equal("Luther, Martin", obj.SummaryList[summaryIdx].Name);
		Assert.Equal(63, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 2;
		Assert.Equal("Wycliffe, John", obj.SummaryList[summaryIdx].Name);
		Assert.Equal(87.5, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 1;
		Assert.Equal("Doe, John", obj.SummaryList[summaryIdx].Name);
		Assert.Equal(10730, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 4;
		Assert.Equal("Doe, Jane J", obj.SummaryList[summaryIdx].Name);
		Assert.Equal(934.56, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		summaryIdx++;
		donationIdx += 5;
		Assert.Equal("Doe, Johnny", obj.SummaryList[summaryIdx].Name);
		Assert.Equal(20080.32, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

		double summaryTotal = 0;
		foreach (var summaryItem in obj.SummaryList)
		{
			summaryTotal += summaryItem.Subtotal;
		}

		double actualTotal = 0;
		foreach (var donation in donations!)
		{
			actualTotal += donation.Value;
		}

		Assert.Equal(Math.Round(summaryTotal, 2), Math.Round(actualTotal, 2));
		var batches = await batchServices.LoadBatches();
		Assert.Equal(Math.Round(summaryTotal, 2), Math.Round(batches[0].Total, 2));
	}

	[Fact]
	public async Task Loading()
	{
		// Arrange
		ITitheEnvelopeServices titheEnvelopeServices = DependencyInjection.Resolve<ITitheEnvelopeServices>();
		titheEnvelopeServices!.TitheEnvelopeDesign = new ObservableCollection<EnvelopeEntry>();

		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Assert
		Assert.Empty(obj.IndividualDonations!);

		// Act
		titheEnvelopeServices!.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 1 });
		titheEnvelopeServices!.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 2 });
		titheEnvelopeServices!.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 3 });
		titheEnvelopeServices!.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = -1 });
		titheEnvelopeServices!.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 4 });
		titheEnvelopeServices!.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 5 });
		titheEnvelopeServices!.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 6 });

		await obj.Loading();

		// Assert again
		Assert.Equal(7, obj.IndividualDonations.Count);
	}

	[Fact]
	public void ValueChanged()
	{
		// Arrange
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.BatchDate = "1/1/2006";
		obj.IndividualDonations!.Add(new Donation() { Category = "1 tithe", Value = 100 });
		obj.IndividualDonations.Add(new Donation() { Category = "2 church budget", Value = 100 });
		obj.IndividualDonations.Add(new Donation() { Category = "3 tuition assistance", Value = 100 });

		// Assert again
		Assert.Equal(0, obj.TotalSum);

		// Act
		obj.ValueChanged();

		// Assert again
		Assert.Equal(300, obj.TotalSum);
	}

	[Fact]
	public void ChangeCategory()
	{
		// Arrange
		ITitheEnvelopeServices titheEnvelopeServices = DependencyInjection.Resolve<ITitheEnvelopeServices>();
		titheEnvelopeServices!.TitheEnvelopeDesign = new ObservableCollection<EnvelopeEntry>();
		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.BatchDate = "1/1/2006";
		obj.IndividualDonations!.Add(new Donation() { Category = "1 tithe", Value = 100 });
		obj.IndividualDonations.Add(new Donation() { Category = "2 church budget", Value = 100 });
		obj.IndividualDonations.Add(new Donation() { Category = "3 tuition assistance", Value = 100 });

		// Act
		obj.ChangeCategory(obj.IndividualDonations[2], new Category() { Code = 4, Description = "international missions", TaxDeductible = true });

		// Assert again
		Assert.Equal("1 tithe", obj.IndividualDonations[0].Category);
		Assert.Equal("2 church budget", obj.IndividualDonations[1].Category);
		Assert.Equal("4 international missions", obj.IndividualDonations[2].Category);
	}

	[Fact]
	public void ChooseDonor()
	{
		// Arrange
		IDonorServices _donorServices = DependencyInjection.Resolve<IDonorServices>();

		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();

		// Act
		obj.ChooseDonor(3);

		// Assert
		Assert.Equal(_donorServices.GetDonorById(3).Name, obj.Name);
		Assert.Throws<Exception>(() => obj.ChooseDonor(10));
		Assert.Throws<Exception>(() => obj.ChooseDonor(-1));
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(2)]
	public void SummarySelectionChanged(int index)
	{
		// Arrange
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();
		IDonationServices donationServices = DependencyInjection.Resolve<IDonationServices>();

		// clear test data from batch service
		batchServices.SaveBatches(new ObservableCollection<Batch>());

		IDonorServices _donorServices = DependencyInjection.Resolve<IDonorServices>();

		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.SummaryList!.Add(new Summary() { DonorId = 1, FirstName = _donorServices.GetDonorById(1).FirstName, LastName = _donorServices.GetDonorById(1).LastName, Subtotal = 1145.50 });
		obj.SummaryList.Add(new Summary() { DonorId = 2, FirstName = _donorServices.GetDonorById(2).FirstName, LastName = _donorServices.GetDonorById(2).LastName, Subtotal = 687.5 });
		obj.SummaryList.Add(new Summary() { DonorId = 3, FirstName = _donorServices.GetDonorById(3).FirstName, LastName = _donorServices.GetDonorById(3).LastName, Subtotal = 1850 });

		double summaryTotal = 0;
		foreach (var summaryItem in obj.SummaryList)
		{
			summaryTotal += summaryItem.Subtotal;
		}

		obj.SummaryList[0].ItemizedDonations = new ObservableCollection<Donation>();
		obj.SummaryList[0].ItemizedDonations!.Add(new Donation() { Category = "1 tithe", Value = 1000, DonorId = 1, LastName = _donorServices.GetDonorById(1).LastName, FirstName = _donorServices.GetDonorById(1).FirstName });
		obj.SummaryList[0].ItemizedDonations!.Add(new Donation() { Category = "2 church budget", Value = 100, DonorId = 1, LastName = _donorServices.GetDonorById(1).LastName, FirstName = _donorServices.GetDonorById(1).FirstName });
		obj.SummaryList[0].ItemizedDonations!.Add(new Donation() { Category = "3 tuition assistance", Value = 45.50, DonorId = 1, LastName = _donorServices.GetDonorById(1).LastName, FirstName = _donorServices.GetDonorById(1).FirstName });
		obj.SummaryList[1].ItemizedDonations = new ObservableCollection<Donation>();
		obj.SummaryList[1].ItemizedDonations!.Add(new Donation() { Category = "1 tithe", Value = 500, DonorId = 2, LastName = _donorServices.GetDonorById(2).LastName, FirstName = _donorServices.GetDonorById(2).FirstName });
		obj.SummaryList[1].ItemizedDonations!.Add(new Donation() { Category = "5 building fund", Value = 100, DonorId = 2, LastName = _donorServices.GetDonorById(2).LastName, FirstName = _donorServices.GetDonorById(2).FirstName });
		obj.SummaryList[1].ItemizedDonations!.Add(new Donation() { Category = "6 outreach", Value = 87.50, DonorId = 2, LastName = _donorServices.GetDonorById(2).LastName, FirstName = _donorServices.GetDonorById(2).FirstName });
		obj.SummaryList[2].ItemizedDonations = new ObservableCollection<Donation>();
		obj.SummaryList[2].ItemizedDonations!.Add(new Donation() { Category = "1 tithe", Value = 1500, DonorId = 3, LastName = _donorServices.GetDonorById(3).LastName, FirstName = _donorServices.GetDonorById(3).FirstName });
		obj.SummaryList[2].ItemizedDonations!.Add(new Donation() { Category = "7 evangelism", Value = 100, DonorId = 3, LastName = _donorServices.GetDonorById(3).LastName, FirstName = _donorServices.GetDonorById(3).FirstName });
		obj.SummaryList[2].ItemizedDonations!.Add(new Donation() { Category = "8 maintenance", Value = 250, DonorId = 3, LastName = _donorServices.GetDonorById(3).LastName, FirstName = _donorServices.GetDonorById(3).FirstName });

		// Act
		obj.SummarySelectionChanged(index);
		obj.ValueChanged(); // add up individual sum

		// Assert
		Assert.Equal(obj.SummaryList[index].Name, obj.Name);
		Assert.Equal(Math.Round(obj.SummaryList[index].Subtotal, 2), Math.Round(obj.TotalSum, 2));

		// Act again
		// clear donor database
		_donorServices.SaveDonors(new ObservableCollection<Donor>());

		obj.SummarySelectionChanged(index);
		obj.ValueChanged(); // add up individual sum

		// Assert again
		Assert.Equal(obj.SummaryList[index].Name, obj.Name);
		Assert.Equal(Math.Round(obj.SummaryList[index].Subtotal, 2), Math.Round(obj.TotalSum, 2));
	}

	[Fact]
	public void Reset()
	{
		// Arrange
		var td = new TestData();
		ITitheEnvelopeServices titheEnvelopeServices = DependencyInjection.Resolve<ITitheEnvelopeServices>();
		titheEnvelopeServices!.TitheEnvelopeDesign = td.TitheEnvelopeDesign;

		DonorInputViewModel obj = DependencyInjection.Resolve<DonorInputViewModel>();
		obj.HasChanges = true;
		obj.BatchDate = "1/2/2023";
		obj.BatchNote = "Note";
		obj.BatchTotal = 100;
		obj.SummaryList!.Add(new Summary());
		obj.IndividualDonations!.Add(new Donation());

		// Act
		obj.Reset();

		// Assert
		Assert.False(obj.HasChanges);
		Assert.Equal("", obj.BatchDate);
		Assert.Equal("", obj.BatchNote);
		Assert.Equal(0, obj.BatchTotal);
		Assert.Empty(obj.SummaryList);
		Assert.Equal(td.TitheEnvelopeDesign!.Count, obj.IndividualDonations.Count);
	}
}
