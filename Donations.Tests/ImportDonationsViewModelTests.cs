using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace Donations.Tests;

public class ImportDonationsViewModelTests : TestWizardBase
{
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void HasChanges(bool expected)
	{
		// Arrange
		WizardImportDonationsViewModel obj = DependencyInjection.Resolve<WizardImportDonationsViewModel>();
		obj.HasChanges = expected;

		// Assert
		Assert.Equal(expected, obj.HasChanges);
	}

	[Fact]
	public async void Save()
	{
		// Arrange
		var td = new TestData();
		MockFileSystem mockfs = new MockFileSystem();

		AutofacRegister(mockfs);

		WizardImportDonationsViewModel obj = DependencyInjection.Resolve<WizardImportDonationsViewModel>();
		IDonationServices donationServices = DependencyInjection.Resolve<IDonationServices>();
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();

		// hack to clear test data from batch service
		await batchServices.SaveBatches(new ObservableCollection<Batch>());

		obj.BatchList = td.BatchList!;
		obj.BatchDict = td.BatchDict!;
		obj.Collection = td.DonationList!;

		// Act
		await obj.Save(true);

		// Assert
		var donations = await donationServices.LoadDonations();
		Assert.False(obj.HasChanges);
		for (int i = 0; i < td.DonationList!.Count; i++)
		{
			Assert.Equal(td.DonationList[i].Id, donations[i].Id);
			Assert.Equal(td.DonationList[i].DonorId, donations[i].DonorId);
			Assert.Equal(td.DonationList[i].BatchId, donations[i].BatchId);
			Assert.Equal(td.DonationList[i].Name, donations[i].Name);
			Assert.Equal(td.DonationList[i].Category, donations[i].Category);
			Assert.Equal(td.DonationList[i].Value, donations[i].Value);
			Assert.Equal(td.DonationList[i].Date, donations[i].Date);
			Assert.Equal(td.DonationList[i].TaxDeductible, donations[i].TaxDeductible);
			Assert.Equal(td.DonationList[i].Note, donations[i].Note);
			Assert.Equal(td.DonationList[i].Method, donations[i].Method);
			Assert.Equal(td.DonationList[i].TransactionNumber, donations[i].TransactionNumber);
		}
	}

	[Fact]
	public async void ReadFile()
	{
		// Arrange
		var td = new TestData();
		byte[] buffer = Encoding.UTF8.GetBytes(td.DonationsCsv);
		MockFileSystem mockfs = new MockFileSystem(new Dictionary<string, MockFileData>
		{
			{ "donations.csv", new MockFileData(buffer) }
		});

		AutofacRegister(mockfs);

		IDonationServices donationServices = DependencyInjection.Resolve<IDonationServices>();
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();

		// hack to clear test data from batch service
		await batchServices.SaveBatches(new ObservableCollection<Batch>());

		WizardImportDonationsViewModel obj = DependencyInjection.Resolve<WizardImportDonationsViewModel>();

		// Act
		await obj.ReadFile("donations.csv");

		// Assert
		Assert.Equal(td.DonationList!.Count, obj.Collection.Count);
		for (int i = 0; i < obj.Collection.Count; i++)
		{
			Assert.Equal(i + 1, obj.Collection[i].Id);
			Assert.Equal(td.DonationList[i].DonorId, obj.Collection[i].DonorId);
			Assert.Equal(td.DonationList[i].BatchId, obj.Collection[i].BatchId);
			Assert.True(Helper.Equal(td.DonationList[i].Name, obj.Collection[i].Name));
			Assert.True(Helper.Equal(td.DonationList[i].Category, obj.Collection[i].Category));
			Assert.Equal(td.DonationList[i].Value, obj.Collection[i].Value);
			Assert.True(td.DonationList[i].Date == obj.Collection[i].Date);
			Assert.Equal(td.DonationList[i].TaxDeductible, obj.Collection[i].TaxDeductible);
			Assert.True(Helper.Equal(td.DonationList[i].Note, obj.Collection[i].Note));
			Assert.Equal(td.DonationList[i].Method, obj.Collection[i].Method);
			Assert.True(Helper.Equal(td.DonationList[i].TransactionNumber, obj.Collection[i].TransactionNumber));
		}

		Assert.Equal(6, obj.BatchList.Count);
		for (int j = 0; j < obj.BatchList.Count; j++)
		{
			Assert.Equal(td.BatchList![j].Id, obj.BatchList[j].Id);
			Assert.Equal(td.BatchList[j].Source, obj.BatchList[j].Source);
			Assert.Equal(td.BatchList[j].Date, obj.BatchList[j].Date);
			Assert.Equal(Math.Round(td.BatchList[j].Total, 2), Math.Round(obj.BatchList[j].Total, 2));
			Assert.Equal(Math.Round(td.BatchList[j].ActualTotal, 2), Math.Round(obj.BatchList[j].ActualTotal, 2));
		}
	}
}
