using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace Donations.Tests;

public class ImportDonorViewModelTests : TestWizardBase
{
	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void HasChanges(bool expected)
	{
		// Arrange
		WizardImportDonorsViewModel obj = DependencyInjection.Resolve<WizardImportDonorsViewModel>();
		obj.HasChanges = expected;

		// Assert
		Assert.Equal(expected, obj.HasChanges);
	}

	[Fact]
	public async void Save()
	{
		// Arrange
		var td = new TestData();
		WizardImportDonorsViewModel obj = DependencyInjection.Resolve<WizardImportDonorsViewModel>();
		IDonorServices donorServices = DependencyInjection.Resolve<IDonorServices>();

		// clear donors before starting test
		await donorServices!.SaveDonors(new ObservableCollection<Donor>());

		obj.Collection = td.DonorList!;

		// Act
		await obj.Save(true);

		// Assert
		ObservableCollection<Donor> donors = await donorServices.LoadDonors();
		Assert.False(obj.HasChanges);
		for (int i = 0; i < donors.Count; i++)
		{
			Assert.Equal(td.DonorList![i].Id, donors[i].Id);
			Assert.Equal(td.DonorList[i].FamilyId, donors[i].FamilyId);
			Assert.Equal(td.DonorList[i].FamilyRelationship, donors[i].FamilyRelationship);
			Assert.Equal(td.DonorList[i].FirstName, donors[i].FirstName);
			Assert.Equal(td.DonorList[i].PreferredName, donors[i].PreferredName);
			Assert.Equal(td.DonorList[i].LastName, donors[i].LastName);
			Assert.Equal(td.DonorList[i].Gender, donors[i].Gender);
			Assert.Equal(td.DonorList[i].Email, donors[i].Email);
			Assert.Equal(td.DonorList[i].Email2, donors[i].Email2);
			Assert.Equal(td.DonorList[i].HomePhone, donors[i].HomePhone);
			Assert.Equal(td.DonorList[i].MobilePhone, donors[i].MobilePhone);
			Assert.Equal(td.DonorList[i].WorkPhone, donors[i].WorkPhone);
			Assert.Equal(td.DonorList[i].AddressType, donors[i].AddressType);
			Assert.Equal(td.DonorList[i].Address, donors[i].Address);
			Assert.Equal(td.DonorList[i].Address2, donors[i].Address2);
			Assert.Equal(td.DonorList[i].City, donors[i].City);
			Assert.Equal(td.DonorList[i].State, donors[i].State);
			Assert.Equal(td.DonorList[i].Zip, donors[i].Zip);
			Assert.Equal(td.DonorList[i].Country, donors[i].Country);
			Assert.Equal(td.DonorList[i].Birthday, donors[i].Birthday);
			Assert.Equal(td.DonorList[i].Baptism, donors[i].Baptism);
			Assert.Equal(td.DonorList[i].Deathday, donors[i].Deathday);
			Assert.Equal(td.DonorList[i].GroupGiving, donors[i].GroupGiving);
			Assert.Equal(td.DonorList[i].ChurchMember, donors[i].ChurchMember);
			Assert.Equal(td.DonorList[i].MaritalStatus, donors[i].MaritalStatus);
			Assert.Equal(td.DonorList[i].Notes, donors[i].Notes);
			Assert.Equal(td.DonorList[i].ActiveGroups, donors[i].ActiveGroups);
			Assert.Equal(td.DonorList[i].LastUpdated, donors[i].LastUpdated);
		}
	}

	[Fact]
	public async void ReadFile()
	{
		// Arrange
		var td = new TestData();
		byte[] buffer = Encoding.UTF8.GetBytes(td.DonorsCsv);
		MockFileSystem mockfs = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ "donors.csv", new MockFileData(buffer) }
			});

		AutofacRegister(mockfs);

		WizardImportDonorsViewModel obj = DependencyInjection.Resolve<WizardImportDonorsViewModel>();

		// Act
		await obj.ReadFile("donors.csv");

		// Assert
		Assert.Equal(td.DonorList!.Count, obj.Collection.Count);
		for (int i = 0; i < obj.Collection.Count; i++)
		{
			Assert.Equal(i + 1, obj.Collection[i].Id);
			Assert.Equal(td.DonorList[i].FamilyId, obj.Collection[i].FamilyId);
			enumFamilyRelationship relationship = (null != td.DonorList[i].FamilyRelationship) ? td.DonorList[i].FamilyRelationship.Value : enumFamilyRelationship.None;
			Assert.Equal(relationship, obj.Collection[i].FamilyRelationship);
			Assert.True(Helper.Equal(td.DonorList[i].FirstName, obj.Collection[i].FirstName));
			Assert.True(Helper.Equal(td.DonorList[i].PreferredName, obj.Collection[i].PreferredName));
			Assert.True(Helper.Equal(td.DonorList[i].LastName, obj.Collection[i].LastName));
			Assert.Equal(td.DonorList[i].Gender, obj.Collection[i].Gender);
			Assert.True(Helper.Equal(td.DonorList[i].Email, obj.Collection[i].Email));
			Assert.True(Helper.Equal(td.DonorList[i].Email2, obj.Collection[i].Email2));
			Assert.True(Helper.Equal(td.DonorList[i].HomePhone, obj.Collection[i].HomePhone));
			Assert.True(Helper.Equal(td.DonorList[i].MobilePhone, obj.Collection[i].MobilePhone));
			Assert.True(Helper.Equal(td.DonorList[i].WorkPhone, obj.Collection[i].WorkPhone));
			Assert.Equal(td.DonorList[i].AddressType, obj.Collection[i].AddressType);
			Assert.True(Helper.Equal(td.DonorList[i].Address, obj.Collection[i].Address));
			Assert.True(Helper.Equal(td.DonorList[i].Address2, obj.Collection[i].Address2));
			Assert.True(Helper.Equal(td.DonorList[i].City, obj.Collection[i].City));
			Assert.True(Helper.Equal(td.DonorList[i].State, obj.Collection[i].State));
			Assert.True(Helper.Equal(td.DonorList[i].Zip, obj.Collection[i].Zip));
			Assert.True(Helper.Equal(td.DonorList[i].Country, obj.Collection[i].Country));
			Assert.True(Helper.Equal(td.DonorList[i].Birthday, obj.Collection[i].Birthday, eFlags.Date));
			Assert.True(Helper.Equal(td.DonorList[i].Baptism, obj.Collection[i].Baptism, eFlags.Date));
			Assert.True(Helper.Equal(td.DonorList[i].Deathday, obj.Collection[i].Deathday, eFlags.Date));
			Assert.Equal(td.DonorList[i].GroupGiving, obj.Collection[i].GroupGiving);
			Assert.Equal(td.DonorList[i].ChurchMember, obj.Collection[i].ChurchMember);
			enumMaritalStatus mStatus = (null != td.DonorList[i].MaritalStatus) ? td.DonorList[i].MaritalStatus.Value : enumMaritalStatus.Unknown;
			Assert.Equal(mStatus, obj.Collection[i].MaritalStatus);
			Assert.True(Helper.Equal(td.DonorList[i].Notes, obj.Collection[i].Notes));
			Assert.True(Helper.Equal(td.DonorList[i].ActiveGroups, obj.Collection[i].ActiveGroups));
			Assert.Equal(td.DonorList[i].LastUpdated, obj.Collection[i].LastUpdated);
			enumAddressType addrType = (null != td.DonorList[i].AltAddressType) ? td.DonorList[i].AltAddressType.Value : enumAddressType.Unspecified;
			Assert.Equal(addrType, obj.Collection[i].AltAddressType);
			Assert.True(Helper.Equal(td.DonorList[i].AltAddress, obj.Collection[i].AltAddress));
			Assert.True(Helper.Equal(td.DonorList[i].AltAddress2, obj.Collection[i].AltAddress2));
			Assert.True(Helper.Equal(td.DonorList[i].AltCity, obj.Collection[i].AltCity));
			Assert.True(Helper.Equal(td.DonorList[i].AltState, obj.Collection[i].AltState));
			Assert.True(Helper.Equal(td.DonorList[i].AltZip, obj.Collection[i].AltZip));
			Assert.True(Helper.Equal(td.DonorList[i].AltCountry, obj.Collection[i].AltCountry));
		}
	}
}
