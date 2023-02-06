using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using Xunit;
using System.IO.Abstractions.TestingHelpers;

namespace Donations.Tests
{
	public partial class DataAccessSequentialTests
	{
		[Theory]
		//                                           Id,      FamilyId,                  FamilyRelationship,         FirstName,     PreferredName,         LastName,             Gender,                Email,              Email2,         HomePhone,         MobilePhone,         WorkPhone,                   AddressType,          Address,         Address2,           City,         State,         Zip,         Country,              AltAddressType,            AltAddress,           AltAddress2,         AltCity,         AltState,         AltZip,            AltCountry,      Birthday,         Baptism,          Deathday,       GroupGiving, ChurchMember,                    MaritalStatus,               Notes,                ActiveGroups,         LastUpdated
		[InlineData(                                  1,             1,      enumFamilyRelationship.Primary,            "John",              null,            "Doe",    enumGender.Male, "john.doe@email.com",                null,              null,          "555-1212",              null,          enumAddressType.Both, "1234 Acme Lane",             null, "Pearly Gates",       "State",     "98765",           "USA",                        null,                  null,                  null,            null,             null,           null,                  null,          null,            null,              null,              null,        true,         enumMaritalStatus.Married,                null,                        null,                   "" )]
		[InlineData(                                  1,             1,      enumFamilyRelationship.Husband,            "John",          "Johhny",            "Doe",    enumGender.Male, "john.doe@email.com", "johndoe@email.com",        "555-1212",          "555-1212",        "555-1212",       enumAddressType.Mailing, "1234 Acme Lane",     "seond line", "Pearly Gates",       "State",     "98765",           "USA", enumAddressType.Residential, "Home away from home", "second address line",      "Alt city",      "Alt state",        "12345",  "where in the world",    "1889-4-8",      "1892/4/8", "January 1, 2001",              true,        true,          enumMaritalStatus.Single,        "What notes",              "What groups?",         "1/22/2023")]
		[InlineData(                                  1,             1,      enumFamilyRelationship.Brother,            "John",              null,            "Doe",    enumGender.Male,                 null,                null,              null,                null,              null,          enumAddressType.Both, "1234 Acme Lane",             null, "Pearly Gates",       "State",     "98765",           "USA",                        null,                  null,                  null,            null,             null,           null,                  null,          null,            null,              null,              null,        true,         enumMaritalStatus.Unknown,                null,                        null,                  "" )]
		[InlineData(                                  1,             1,      enumFamilyRelationship.Father,             "John",              null,             null,    enumGender.Male,                 null,                null,              null,          "555-1212",              null,   enumAddressType.Unspecified,             null,             null,           null,          null,        null,            null,                        null,                  null,                  null,            null,             null,           null,                  null,          null,            null,              null,              null,        null,         enumMaritalStatus.Married,                null,                        null,                  "")]
		public void DonorViewModel_SetDonor(int id, int? familyId, enumFamilyRelationship relationship, string? firstName, string? preferred, string? lastName, enumGender? gender,        string? email, string? email2, string? homePhone, string? mobilePhone, string? workPhone, enumAddressType addrType,  string? address, string? address2,   string? city, string? state, string? zip, string? country, enumAddressType? altAddrType, string? altAddress, string? altAddress2, string? altCity, string? altState, string? altZip, string? altCountry, string? birth, string? baptism,           string? death, bool? groupGiving, bool? member, enumMaritalStatus? maritalStatus, string? notes, string? activeGroups, string? lastUpdated)
		{
			// Arrange
			di.Data.DonorDict = new Dictionary<int, Donor>();
			di.Data.DonorList = new ObservableCollection<Donor>();

			DonorViewModel obj = new DonorViewModel();

			// Act
			var donor = new Donor() { Id = id, FamilyId = familyId, FamilyRelationship = relationship, FirstName = firstName, PreferredName = preferred, LastName = lastName, Gender = gender, Email = email, Email2 = email2, HomePhone = homePhone, MobilePhone = mobilePhone, WorkPhone = workPhone, AddressType = addrType, Address = address, Address2 = address2, City = city, State = state, Zip = zip, Country = country, AltAddressType = altAddrType, AltAddress = altAddress, AltAddress2 = altAddress2, AltCity = altCity, AltState = altState, AltZip = altZip, AltCountry = altCountry, Birthday = birth, Baptism = baptism, Deathday = death, GroupGiving = groupGiving, ChurchMember = member, MaritalStatus = maritalStatus, Notes = notes, ActiveGroups = activeGroups, LastUpdated = lastUpdated };
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
			Assert.Equal(birth, obj.SelectedDonor.Birthday);
			Assert.Equal(baptism, obj.SelectedDonor.Baptism);
			Assert.Equal(death, obj.SelectedDonor.Deathday);
			Assert.Equal(groupGiving, obj.SelectedDonor.GroupGiving);
			Assert.Equal(member, obj.SelectedDonor.ChurchMember);
			Assert.Equal(maritalStatus, obj.SelectedDonor.MaritalStatus);
			Assert.Equal(notes, obj.SelectedDonor.Notes);
			Assert.Equal(activeGroups, obj.SelectedDonor.ActiveGroups);
			Assert.Equal(String.IsNullOrEmpty(lastUpdated) ? "" : DateOnly.Parse(lastUpdated).ToString("yyyy/MM/dd"), string.IsNullOrEmpty(obj.SelectedDonor.LastUpdated) ? "" : DateOnly.Parse(obj.SelectedDonor.LastUpdated).ToString("yyyy/MM/dd"));

			// Act again
			obj.SetDonor(donor);

			// Assert again
			Assert.True(obj.UpdateEnabled);
			Assert.False(obj.AddEnabled);

			// Act again
			obj.CancelCmd.Execute(null);

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
			Assert.Equal("", obj.SelectedDonor.LastUpdated);

		}

		[Theory]
		[InlineData("Verification failure message")]
		[InlineData("")]
		[InlineData(null)]
		public void DonorViewModel_VerificationFailureMessage(string? param)
		{
			// Arrange
			DonorViewModel obj = new DonorViewModel() { VerificationFailureMessage = param };

			// Assert
			Assert.Equal(param, obj.VerificationFailureMessage);
		}

		[Fact]
		public void DonorViewModel_ChooseDonor()
		{
			// Arrange
			var td = new TestData();
			di.Data.DonorDict = new Dictionary<int, Donor>();
			di.Data.DonorList = new ObservableCollection<Donor>();
			di.Data.DonationDict = new Dictionary<int, Donation>();
			di.Data.DonationList = new ObservableCollection<Donation>();

			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			di.Data.DonationList = td.DonationList;
			di.Data.DonationDict = td.DonationDict;

			DonorViewModel obj = new DonorViewModel();

			// Act
			obj.ChooseDonor(1);

			// Assert
			Assert.Equal(di.Data.DonorDict[1], obj.SelectedDonor);
			Assert.Equal(2, obj.Donations.View.Cast<Donation>().Count());
			Assert.Equal(3, obj.FamilyMembers.View.Cast<Donor>().Count());

			// Act again
			obj.ChooseDonor(2);

			// Assert again
			Assert.Equal(di.Data.DonorDict[2], obj.SelectedDonor);
			Assert.Equal(1, obj.Donations.View.Cast<Donation>().Count());
			Assert.Equal(3, obj.FamilyMembers.View.Cast<Donor>().Count());

			// Act again
			obj.ChooseDonor(3);

			// Assert again
			Assert.Equal(di.Data.DonorDict[3], obj.SelectedDonor);
			Assert.Equal(3, obj.Donations.View.Cast<Donation>().Count());
			Assert.Equal(3, obj.FamilyMembers.View.Cast<Donor>().Count());

			// Act
			obj.ChooseDonor(4);

			// Assert
			Assert.Equal(di.Data.DonorDict[4], obj.SelectedDonor);
			Assert.Equal(2, obj.Donations.View.Cast<Donation>().Count());
			Assert.Equal(0, obj.FamilyMembers.View.Cast<Donor>().Count());

			// Act
			obj.ChooseDonor(5);

			// Assert
			Assert.Equal(di.Data.DonorDict[5], obj.SelectedDonor);
			Assert.Equal(1, obj.Donations.View.Cast<Donation>().Count());
			Assert.Equal(0, obj.FamilyMembers.View.Cast<Donor>().Count());
		}

		[Fact]
		public void DonorViewModel_ChooseRelated()
		{
			// Arrange
			var td = new TestData();
			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			DonorViewModel obj = new DonorViewModel();

			// Act
			obj.ChooseDonor(4);

			// Assert
			Assert.Equal(di.Data.DonorDict[4], obj.SelectedDonor);

			// Act again
			var resp = obj.ChooseRelated(5);

			// Assert again
			Assert.Equal($"The selected donor, {di.Data.DonorDict[5].FirstName} {di.Data.DonorDict[5].LastName}, does not currently have any family. Do you wish to start one?", resp);

			// Act again
			resp = obj.ChooseRelated(5, true);

			// Assert again
			Assert.Equal(di.Data.DonorDict[4].FamilyId, di.Data.DonorDict[4].FamilyId);
		}

		[Fact]
		public void DonorViewModel_FamilyRelationshipConflict()
		{
			// Arrange
			var td = new TestData();
			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;
			di.Data.DonorDict[2].FamilyRelationship = enumFamilyRelationship.Primary; // set this to primary to cause a conflict

			DonorViewModel obj = new DonorViewModel();

			// Act
			obj.SelectedDonor = di.Data.DonorDict[1];
			bool ret = obj.FamilyRelationshipConflict("Primary");

			// Assert
			Assert.True(ret);

			// Arrange again
			di.Data.DonorDict[1].FamilyRelationship = enumFamilyRelationship.Husband;
			di.Data.DonorDict[2].FamilyRelationship = enumFamilyRelationship.Wife;
			di.Data.DonorDict[3].FamilyRelationship = enumFamilyRelationship.Son;

			// Act
			ret = obj.FamilyRelationshipConflict("Primary");

			// Assert
			Assert.False(ret);
		}

		[Fact]
		public void DonorViewModel_AddDonor_Success()
		{
			// Arrange
			var td = new TestData();
			di.FileSystem = new MockFileSystem();

			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			DonorViewModel obj = new DonorViewModel();

			// Act
			obj.SelectedDonor.LastName = "Hus";
			obj.SelectedDonor.FirstName = "John";
			var ret = obj.AddDonor();

			// Assert
			Assert.Null(ret);

			// Act again
			obj.SelectedDonor.LastName = "Doe";
			obj.SelectedDonor.FirstName = "John";
			obj.SelectedDonor.Address = "1234 Acme Lane";
			ret = obj.AddDonor();

			// Assert again
			Assert.Equal("There is a John Doe on 1234 Acme Lane already in the database. Do you wish to add them anyway?", ret);

			// Act again
			ret = obj.AddDonor(true); // force

			// Assert again
			Assert.Null(ret);
		}

		[Fact]
		public void DonorViewModel_AddDonor_SameDonorConflict()
		{
			// Arrange
			var td = new TestData();
			di.FileSystem = new MockFileSystem();

			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			DonorViewModel obj = new DonorViewModel();

			// Act
			obj.SelectedDonor.LastName = "Doe";
			obj.SelectedDonor.FirstName = "John";
			obj.SelectedDonor.Address = "1234 Acme Lane";
			var ret = obj.AddDonor();

			// Assert
			Assert.Equal("There is a John Doe on 1234 Acme Lane already in the database. Do you wish to add them anyway?", ret);

			// Act again
			ret = obj.AddDonor(true); // force

			// Assert again
			Assert.Null(ret);
		}

		[Fact]
		public void DonorViewModel_AddDonor_RelationshipConflict()
		{
			// Arrange
			var td = new TestData();
			di.FileSystem = new MockFileSystem();

			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;
			di.Data.DonorList[0].FamilyRelationship = enumFamilyRelationship.Primary;

			DonorViewModel obj = new DonorViewModel();

			// Act
			obj.SelectedDonor.LastName = "Hus";
			obj.SelectedDonor.FirstName = "John";
			obj.SelectedDonor.Address = "1234 Acme Lane";
			obj.SelectedDonor.FamilyRelationship = enumFamilyRelationship.Primary;
			var res = obj.ChooseRelated(1);
			res = obj.AddDonor();

			// Assert
			Assert.NotNull(res);
			Assert.Equal("The family member designated as Primary, will receive the year end donor report for all family members participating in GroupGiving. Hence, only one family member can be marked as Primary.", res);
		}

		[Theory]
		[InlineData(enumAddressType.Both, enumAddressType.Both, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
		[InlineData(enumAddressType.Both, enumAddressType.Mailing, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
		[InlineData(enumAddressType.Mailing, enumAddressType.Both, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
		[InlineData(enumAddressType.Mailing, enumAddressType.Mailing, "The address marked 'Mailing' is where the year end donor report is sent, if a hard copy is required. Hence, the primary and alternate addresses cannot both be designated 'Mailing.'")]
		public void DonorViewModel_AddDonor_AddressTypeConflict(enumAddressType addrType, enumAddressType? altAddrType, string? expected)
		{
			// Arrange
			var td = new TestData();
			di.FileSystem = new MockFileSystem();

			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			DonorViewModel obj = new DonorViewModel();

			// Act
			obj.SelectedDonor.LastName = "Hus";
			obj.SelectedDonor.FirstName = "John";
			obj.SelectedDonor.AddressType = addrType;
			obj.SelectedDonor.AltAddressType = altAddrType;
			var res = obj.AddDonor();

			// Assert again
			Assert.NotNull(res);
			Assert.Equal(expected, res);
		}

		[Fact]
		public void DonorViewModel_UpdateDonor_RelationshipConflict()
		{
			// Arrange
			var td = new TestData();
			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;
			di.Data.DonorDict[1].FamilyRelationship = enumFamilyRelationship.Primary;
			di.Data.DonorDict[2].FamilyRelationship = enumFamilyRelationship.Primary;
			di.Data.DonorDict[3].FamilyRelationship = enumFamilyRelationship.Primary;

			DonorViewModel obj = new DonorViewModel();

			// Act
			obj.SelectedDonor = di.Data.DonorDict[1];
			bool ret = obj.FamilyRelationshipConflict("Primary");

			// Assert
			Assert.True(ret);

			// Arrange again
			di.Data.DonorDict[1].FamilyRelationship = enumFamilyRelationship.Husband;
			di.Data.DonorDict[2].FamilyRelationship = enumFamilyRelationship.Wife;
			di.Data.DonorDict[3].FamilyRelationship = enumFamilyRelationship.Son;

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
		public void DonorViewModel_UpdateDonor_AddressTypeConflict(enumAddressType addrType, enumAddressType? altAddrType, string? expected)
		{
			// Arrange
			var td = new TestData();
			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			DonorViewModel obj = new DonorViewModel();

			// Act
			obj.SetDonor(di.Data.DonorDict[1]);
			// set conficting values to trigger verification failure
			obj.SelectedDonor.AddressType = addrType;
			obj.SelectedDonor.AltAddressType = altAddrType;
			var res = obj.UpdateDonor();

			// Assert again
			Assert.NotNull(res);
			Assert.Equal(expected, res);
		}

		[Fact]
		public void DonorViewModel_UpdateDonor_PrimaryConflict()
		{
			// Arrange
			var td = new TestData();
			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;
			di.Data.DonorDict[2].FamilyRelationship = enumFamilyRelationship.Primary;

			DonorViewModel obj = new DonorViewModel();

			// Act again
			// set multiple primaries to trigger verification failure
			obj.SetDonor(di.Data.DonorDict[1]);
			di.Data.DonorDict[1].FamilyRelationship = enumFamilyRelationship.Primary;
			var res = obj.UpdateDonor();

			// Assert again
			Assert.NotNull(res);
			Assert.Equal("The family member designated as Primary, will receive the year end donor report for all family members participating in GroupGiving. Hence, only one family member can be marked as Primary.", res);

			// Act again
			// set so primary conflict is not triggered
			di.Data.DonorDict[2].FamilyRelationship = enumFamilyRelationship.Wife;
			res = obj.UpdateDonor();

			// Assert again
			Assert.Null(res);
		}

		[Fact]
		public void DonorViewModel_UpdateDonor_Success()
		{
			// Arrange
			var td = new TestData();
			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			DonorViewModel obj = new DonorViewModel();

			// Act again
			var res = obj.UpdateDonor();

			// Assert again
			Assert.Null(res);
		}
	}
}
