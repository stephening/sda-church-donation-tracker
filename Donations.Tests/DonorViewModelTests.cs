using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using Xunit;

namespace Donations.Tests;

public class DonorViewModelTests : TestBase
{
	[Theory]
	//                                           Id,      FamilyId,                  FamilyRelationship,         FirstName,     PreferredName,         LastName,             Gender,                Email,              Email2,         HomePhone,         MobilePhone,         WorkPhone,                   AddressType,          Address,         Address2,           City,         State,         Zip,         Country,              AltAddressType,            AltAddress,           AltAddress2,         AltCity,         AltState,         AltZip,            AltCountry,      Birthday,         Baptism,          Deathday,       GroupGiving, ChurchMember,                    MaritalStatus,               Notes,                ActiveGroups,         LastUpdated
	[InlineData(1, 1, enumFamilyRelationship.Primary, "John", null, "Doe", enumGender.Male, "john.doe@email.com", null, null, "555-1212", null, enumAddressType.Both, "1234 Acme Lane", null, "Pearly Gates", "State", "98765", "USA", null, null, null, null, null, null, null, null, null, null, null, true, enumMaritalStatus.Married, null, null, "")]
	[InlineData(1, 1, enumFamilyRelationship.Husband, "John", "Johhny", "Doe", enumGender.Male, "john.doe@email.com", "johndoe@email.com", "555-1212", "555-1212", "555-1212", enumAddressType.Mailing, "1234 Acme Lane", "seond line", "Pearly Gates", "State", "98765", "USA", enumAddressType.Residential, "Home away from home", "second address line", "Alt city", "Alt state", "12345", "where in the world", "1889-4-8", "1892/4/8", "January 1, 2001", true, true, enumMaritalStatus.Single, "What notes", "What groups?", "1/22/2023")]
	[InlineData(1, 1, enumFamilyRelationship.Brother, "John", null, "Doe", enumGender.Male, null, null, null, null, null, enumAddressType.Both, "1234 Acme Lane", null, "Pearly Gates", "State", "98765", "USA", null, null, null, null, null, null, null, null, null, null, null, true, enumMaritalStatus.Unknown, null, null, null)]
	[InlineData(1, 1, enumFamilyRelationship.Father, "John", null, null, enumGender.Male, null, null, null, "555-1212", null, enumAddressType.Unspecified, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, enumMaritalStatus.Married, null, null, null)]
	public async void SetDonor(int id, int? familyId, enumFamilyRelationship relationship, string? firstName, string? preferred, string? lastName, enumGender? gender, string? email, string? email2, string? homePhone, string? mobilePhone, string? workPhone, enumAddressType addrType, string? address, string? address2, string? city, string? state, string? zip, string? country, enumAddressType? altAddrType, string? altAddress, string? altAddress2, string? altCity, string? altState, string? altZip, string? altCountry, string? birth, string? baptism, string? death, bool? groupGiving, bool? member, enumMaritalStatus? maritalStatus, string? notes, string? activeGroups, string? lastUpdated)
	{
		// Arrange
		AutofacRegister();

		IDonorServices _donorServices = DependencyInjection.Resolve<IDonorServices>();

		// clear donors before starting test
		await _donorServices!.SaveDonors(new ObservableCollection<Donor>());

		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();

		// Act
		var donor = new Donor() { Id = id, FamilyId = familyId, FamilyRelationship = relationship, FirstName = firstName, PreferredName = preferred, LastName = lastName, Gender = gender, Email = email, Email2 = email2, HomePhone = homePhone, MobilePhone = mobilePhone, WorkPhone = workPhone, AddressType = addrType, Address = address, Address2 = address2, City = city, State = state, Zip = zip, Country = country, AltAddressType = altAddrType, AltAddress = altAddress, AltAddress2 = altAddress2, AltCity = altCity, AltState = altState, AltZip = altZip, AltCountry = altCountry, Birthday = birth, Baptism = baptism, Deathday = death, GroupGiving = groupGiving, ChurchMember = member, MaritalStatus = maritalStatus, Notes = notes, ActiveGroups = activeGroups, LastUpdated = string.IsNullOrEmpty(lastUpdated) ? null : DateTime.Parse(lastUpdated) };
		obj.SelectedDonor = donor;

		// Assert
		Assert.Equal(id, obj.SelectedDonor.Id);
		Assert.Equal(familyId, obj.SelectedDonor.FamilyId);
		Assert.Equal(relationship, obj.SelectedDonor.FamilyRelationship);
		Assert.Equal(firstName, obj.SelectedDonor.FirstName);
		Assert.Equal(preferred, obj.SelectedDonor.PreferredName);
		Assert.Equal(lastName, obj.SelectedDonor.LastName);
		Assert.Equal(gender, obj.SelectedDonor.Gender);
		Assert.Equal(email, obj.SelectedDonor.Email);
		Assert.Equal(email2, obj.SelectedDonor.Email2);
		Assert.Equal(homePhone, obj.SelectedDonor.HomePhone);
		Assert.Equal(mobilePhone, obj.SelectedDonor.MobilePhone);
		Assert.Equal(workPhone, obj.SelectedDonor.WorkPhone);
		Assert.Equal(addrType, obj.SelectedDonor.AddressType);
		Assert.Equal(address, obj.SelectedDonor.Address);
		Assert.Equal(address2, obj.SelectedDonor.Address2);
		Assert.Equal(city, obj.SelectedDonor.City);
		Assert.Equal(state, obj.SelectedDonor.State);
		Assert.Equal(zip, obj.SelectedDonor.Zip);
		Assert.Equal(country, obj.SelectedDonor.Country);
		Assert.Equal(altAddrType, obj.SelectedDonor.AltAddressType);
		Assert.Equal(altAddress, obj.SelectedDonor.AltAddress);
		Assert.Equal(altAddress2, obj.SelectedDonor.AltAddress2);
		Assert.Equal(altCity, obj.SelectedDonor.AltCity);
		Assert.Equal(altState, obj.SelectedDonor.AltState);
		Assert.Equal(altZip, obj.SelectedDonor.AltZip);
		Assert.Equal(altCountry, obj.SelectedDonor.AltCountry);
		Assert.Equal((null != birth) ? DateOnly.Parse(birth).ToString("yyyy/MM/dd") : null, obj.SelectedDonor.Birthday);
		Assert.Equal((null != baptism) ? DateOnly.Parse(baptism).ToString("yyyy/MM/dd") : null, obj.SelectedDonor.Baptism);
		Assert.Equal((null != death) ? DateOnly.Parse(death).ToString("yyyy/MM/dd") : null, obj.SelectedDonor.Deathday);
		Assert.Equal(groupGiving, obj.SelectedDonor.GroupGiving);
		Assert.Equal(member, obj.SelectedDonor.ChurchMember);
		Assert.Equal(maritalStatus, obj.SelectedDonor.MaritalStatus);
		Assert.Equal(notes, obj.SelectedDonor.Notes);
		Assert.Equal(activeGroups, obj.SelectedDonor.ActiveGroups);
		Assert.Equal(String.IsNullOrEmpty(lastUpdated) ? null : DateTime.Parse(lastUpdated), null == obj.SelectedDonor.LastUpdated ? null : obj.SelectedDonor.LastUpdated);

		// Act again
		await obj.SetDonor(donor);

		// Assert again
		Assert.True(obj.UpdateEnabled);
		Assert.False(obj.AddEnabled);

		// Act again
		obj.CancelCommand.Execute(null);

		// Assert again
		Assert.False(obj.UpdateEnabled);
		Assert.True(obj.AddEnabled);

		Assert.Equal(0, obj.SelectedDonor.Id);
		Assert.Null(obj.SelectedDonor.FamilyId);
		Assert.Null(obj.SelectedDonor.FamilyRelationship);
		Assert.Null(obj.SelectedDonor.FirstName);
		Assert.Null(obj.SelectedDonor.PreferredName);
		Assert.Null(obj.SelectedDonor.LastName);
		Assert.Null(obj.SelectedDonor.Gender);
		Assert.Null(obj.SelectedDonor.Email);
		Assert.Null(obj.SelectedDonor.Email2);
		Assert.Null(obj.SelectedDonor.HomePhone);
		Assert.Null(obj.SelectedDonor.MobilePhone);
		Assert.Null(obj.SelectedDonor.WorkPhone);
		Assert.Equal(enumAddressType.Both, obj.SelectedDonor.AddressType);
		Assert.Null(obj.SelectedDonor.Address);
		Assert.Null(obj.SelectedDonor.Address2);
		Assert.Null(obj.SelectedDonor.City);
		Assert.Null(obj.SelectedDonor.State);
		Assert.Null(obj.SelectedDonor.Zip);
		Assert.Null(obj.SelectedDonor.Country);
		Assert.Null(obj.SelectedDonor.AltAddressType);
		Assert.Null(obj.SelectedDonor.AltAddress);
		Assert.Null(obj.SelectedDonor.AltAddress2);
		Assert.Null(obj.SelectedDonor.AltCity);
		Assert.Null(obj.SelectedDonor.AltState);
		Assert.Null(obj.SelectedDonor.AltZip);
		Assert.Null(obj.SelectedDonor.AltCountry);
		Assert.Null(obj.SelectedDonor.Birthday);
		Assert.Null(obj.SelectedDonor.Baptism);
		Assert.Null(obj.SelectedDonor.Deathday);
		Assert.Null(obj.SelectedDonor.GroupGiving);
		Assert.Null(obj.SelectedDonor.ChurchMember);
		Assert.Null(obj.SelectedDonor.MaritalStatus);
		Assert.Null(obj.SelectedDonor.Notes);
		Assert.Null(obj.SelectedDonor.ActiveGroups);
		Assert.Equal(null, obj.SelectedDonor.LastUpdated);

	}

	[Theory]
	[InlineData("Verification failure message")]
	[InlineData("")]
	[InlineData(null)]
	public void VerificationFailureMessage(string? param)
	{
		// Arrange
		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();
		obj.VerificationFailureMessage = param;

		// Assert
		Assert.Equal(param, obj.VerificationFailureMessage);
	}

	[Fact]
	public async void ChooseDonor()
	{
		// Arrange
		IDonorServices donorServices = DependencyInjection.Resolve<IDonorServices>();
		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();

		// Act
		await obj.ChooseDonor(1);
		await obj.RefreshLists();

		// Assert
		Assert.Equal(donorServices.GetDonorById(1), obj.SelectedDonor);
		Assert.Equal(21, ((ObservableCollection<Donation>)obj.Donations.Source).Count);
		Assert.Equal(3, ((ObservableCollection<Donor>)obj.FamilyMembers.Source).Count);

		// Act again
		await obj.ChooseDonor(2);
		await obj.RefreshLists();

		// Assert again
		Assert.Equal(donorServices.GetDonorById(2), obj.SelectedDonor);
		Assert.Equal(21, ((ObservableCollection<Donation>)obj.Donations.Source).Count);
		Assert.Equal(3, ((ObservableCollection<Donor>)obj.FamilyMembers.Source).Count);

		// Act again
		await obj.ChooseDonor(3);
		await obj.RefreshLists();

		// Assert again
		Assert.Equal(donorServices.GetDonorById(3), obj.SelectedDonor);
		Assert.Equal(21, ((ObservableCollection<Donation>)obj.Donations.Source).Count);
		Assert.Equal(3, ((ObservableCollection<Donor>)obj.FamilyMembers.Source).Count);

		// Act
		await obj.ChooseDonor(4);
		await obj.RefreshLists();

		// Assert
		Assert.Equal(donorServices.GetDonorById(4), obj.SelectedDonor);
		Assert.Equal(2, ((ObservableCollection<Donation>)obj.Donations.Source).Count);
		Assert.Null(obj.FamilyMembers.Source);

		// Act
		await obj.ChooseDonor(5);
		await obj.RefreshLists();

		// Assert
		Assert.Equal(donorServices.GetDonorById(5), obj.SelectedDonor);
		Assert.Single((ObservableCollection<Donation>)obj.Donations.Source);
		Assert.Null(obj.FamilyMembers.Source);
	}

	[Fact]
	public async void ChooseRelated()
	{
		// Arrange
		IDonorServices _donorServices = DependencyInjection.Resolve<IDonorServices>();

		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();

		// Act
		await obj.ChooseDonor(4);

		// Assert
		Assert.Equal(_donorServices.GetDonorById(4), obj.SelectedDonor);

		// Act again
		var resp = await obj.ChooseRelated(5);

		// Assert again
		Assert.Equal($"Neither the selected donor, \"{_donorServices.GetDonorById(4).LastName}, {_donorServices.GetDonorById(4).FirstName}\" or the one in question, \"{_donorServices.GetDonorById(5).LastName}, {_donorServices.GetDonorById(5).FirstName}\" are assigned to families. Do you wish to create a new family group?", resp);

		// Act again
		resp = await obj.ChooseRelated(5, true);

		// Assert again
		Assert.Equal(_donorServices.GetDonorById(4).FamilyId, _donorServices.GetDonorById(4).FamilyId);
	}

	[Fact]
	public async void FamilyRelationshipConflict()
	{
		// Arrange
		IDonorServices _donorServices = DependencyInjection.Resolve<IDonorServices>();

		_donorServices.GetDonorById(2).FamilyRelationship = enumFamilyRelationship.Primary; // set this to primary to cause a conflict

		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();

		// Act
		obj.SelectedDonor = _donorServices.GetDonorById(1);
		await obj.RefreshLists();
		bool ret = obj.FamilyRelationshipConflict("Primary");

		// Assert
		Assert.True(ret);

		// Arrange again
		_donorServices.GetDonorById(1).FamilyRelationship = enumFamilyRelationship.Husband;
		_donorServices.GetDonorById(2).FamilyRelationship = enumFamilyRelationship.Wife;
		_donorServices.GetDonorById(3).FamilyRelationship = enumFamilyRelationship.Son;

		// Act
		ret = obj.FamilyRelationshipConflict("Primary");

		// Assert
		Assert.False(ret);
	}

	[Fact]
	public async void AddDonor_Success()
	{
		// Arrange
		IDonorServices _donorServices = DependencyInjection.Resolve<IDonorServices>();

		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();

		// Act
		obj.SelectedDonor!.LastName = "Hus";
		obj.SelectedDonor.FirstName = "John";
		var ret = await obj.AddDonor();

		// Assert
		Assert.Null(ret);

		// Act again
		obj.SelectedDonor.LastName = "Doe";
		obj.SelectedDonor.FirstName = "John";
		obj.SelectedDonor.Address = "1234 Acme Lane";
		ret = await obj.AddDonor();

		// Assert again
		Assert.Equal("There is a John Doe on 1234 Acme Lane already in the database. Do you wish to add them anyway?", ret);

		// Act again
		ret = await obj.AddDonor(true); // force

		// Assert again
		Assert.Null(ret);
	}

	[Fact]
	public async void AddDonor_SameDonorConflict()
	{
		// Arrange
		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();

		// Act
		obj.SelectedDonor!.LastName = "Doe";
		obj.SelectedDonor.FirstName = "John";
		obj.SelectedDonor.Address = "1234 Acme Lane";
		var ret = await obj.AddDonor();

		// Assert
		Assert.Equal("There is a John Doe on 1234 Acme Lane already in the database. Do you wish to add them anyway?", ret);

		// Act again
		ret = await obj.AddDonor(true); // force

		// Assert again
		Assert.Null(ret);
	}

	[Fact]
	public async void AddDonor_RelationshipConflict()
	{
		// Arrange
		IDonorServices donorServices = DependencyInjection.Resolve<IDonorServices>();
		var donor = donorServices.GetDonorById(1);
		donor.FamilyRelationship = enumFamilyRelationship.Primary;
		await donorServices.UpdateDonor(donor);

		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();

		// Act
		obj.SelectedDonor!.LastName = "Hus";
		obj.SelectedDonor.FirstName = "John";
		obj.SelectedDonor.Address = "1234 Acme Lane";
		obj.SelectedDonor.FamilyRelationship = enumFamilyRelationship.Primary;
		obj.SelectedDonor.GroupGiving = true;
		var res = await obj.ChooseRelated(1);
		res = await obj.AddDonor();

		// Assert
		Assert.NotNull(res);
		Assert.Equal("The family member designated as Primary, will receive the year end donor report for all family members participating in GroupGiving. Hence, only one family member can be marked as Primary.", res);
	}

	[Theory]
	[InlineData(enumAddressType.Both, enumAddressType.Both, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
	[InlineData(enumAddressType.Both, enumAddressType.Mailing, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
	[InlineData(enumAddressType.Mailing, enumAddressType.Both, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
	[InlineData(enumAddressType.Mailing, enumAddressType.Mailing, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
	public async void AddDonor_AddressTypeConflict(enumAddressType addrType, enumAddressType? altAddrType, string? expected)
	{
		// Arrange
		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();

		// Act
		obj.SelectedDonor!.LastName = "Hus";
		obj.SelectedDonor.FirstName = "John";
		obj.SelectedDonor.AddressType = addrType;
		obj.SelectedDonor.AltAddressType = altAddrType;
		var res = await obj.AddDonor();

		// Assert again
		Assert.NotNull(res);
		Assert.Equal(expected, res);
	}

	[Fact]
	public async void UpdateDonor_RelationshipConflict()
	{
		// Arrange
		IDonorServices donorServices = DependencyInjection.Resolve<IDonorServices>();

		donorServices.GetDonorById(1).FamilyRelationship = enumFamilyRelationship.Primary;
		donorServices.GetDonorById(2).FamilyRelationship = enumFamilyRelationship.Primary;
		donorServices.GetDonorById(3).FamilyRelationship = enumFamilyRelationship.Primary;

		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();

		// Act
		obj.SelectedDonor = donorServices.GetDonorById(1);
		await obj.RefreshLists();
		bool ret = obj.FamilyRelationshipConflict("Primary");

		// Assert
		Assert.True(ret);

		// Arrange again
		donorServices.GetDonorById(1).FamilyRelationship = enumFamilyRelationship.Husband;
		donorServices.GetDonorById(2).FamilyRelationship = enumFamilyRelationship.Wife;
		donorServices.GetDonorById(3).FamilyRelationship = enumFamilyRelationship.Son;

		// Act again
		ret = obj.FamilyRelationshipConflict("Primary");

		// Assert again
		Assert.False(ret);
	}

	[Theory]
	[InlineData(enumAddressType.Both, enumAddressType.Both, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
	[InlineData(enumAddressType.Both, enumAddressType.Mailing, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
	[InlineData(enumAddressType.Mailing, enumAddressType.Both, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
	[InlineData(enumAddressType.Mailing, enumAddressType.Mailing, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
	public async void UpdateDonor_AddressTypeConflict(enumAddressType addrType, enumAddressType? altAddrType, string? expected)
	{
		// Arrange
		IDonorServices _donorServices = DependencyInjection.Resolve<IDonorServices>();

		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();

		// Act
		await obj.SetDonor(_donorServices.GetDonorById(1));
		// set conficting values to trigger verification failure
		obj.SelectedDonor!.AddressType = addrType;
		obj.SelectedDonor.AltAddressType = altAddrType;
		var res = await obj.UpdateDonor();

		// Assert again
		Assert.NotNull(res);
		Assert.Equal(expected, res);
	}

	[Fact]
	public async void UpdateDonor_PrimaryConflict()
	{
		// Arrange
		IDonorServices _donorServices = DependencyInjection.Resolve<IDonorServices>();

		_donorServices.GetDonorById(2).FamilyRelationship = enumFamilyRelationship.Primary;

		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();

		// Act
		// set multiple primaries to trigger verification failure
		await obj.SetDonor(_donorServices.GetDonorById(1));
		await obj.RefreshLists();
		_donorServices.GetDonorById(1).FamilyRelationship = enumFamilyRelationship.Primary;
		var res = await obj.UpdateDonor();

		// Assert again
		Assert.NotNull(res);
		Assert.Equal("The family member designated as Primary, will receive the year end donor report for all family members participating in GroupGiving. Hence, only one family member can be marked as Primary.", res);

		// Act again
		// set so primary conflict is not triggered
		_donorServices.GetDonorById(2).FamilyRelationship = enumFamilyRelationship.Wife;
		res = await obj.UpdateDonor();

		// Assert again
		Assert.Null(res);
	}

	[Fact]
	public async void UpdateDonor_Success()
	{
		// Arrange
		IDonorServices _donorServices = DependencyInjection.Resolve<IDonorServices>();

		DonorViewModel obj = DependencyInjection.Resolve<DonorViewModel>();
		await obj.SetDonor(_donorServices.GetDonorById(1));

		// Act again
		var res = await obj.UpdateDonor();

		// Assert again
		Assert.Null(res);
	}
}
