using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xunit;

namespace Donations.Tests
{
	public partial class DataAccessSequentialTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ImportDonorViewModel_HasChanges(bool expected)
        {
            // Arrange
            ImportDonorViewModel obj = new ImportDonorViewModel() { HasChanges = expected };

            // Assert
            Assert.Equal(expected, obj.HasChanges);
        }

        [Fact]
        public void ImportDonorViewModel_Save()
        {
            // Arrange
            var td = new TestData();
            MockFileSystem mockfs = new MockFileSystem();
            di.FileSystem = mockfs;

            di.Data.DonorDict = new Dictionary<int, Donor>();
            di.Data.DonorList = new ObservableCollection<Donor>();

            ImportDonorViewModel obj = new ImportDonorViewModel();

            obj.Collection = td.DonorList;

            // Act
            obj.Save(true);
            MockFileData mockOutputFile = mockfs.GetFile(di.Data.DonorsFileName);
            // get the data written to the mock file in Save()
            mockfs.AddFile("dummy.xml", new MockFileData(mockOutputFile.TextContents));
            // use xml deserializer to read it back in for verification
            var collection = di.Data.DeserializeXml<Donor>("dummy.xml");

            // Assert
            Assert.False(obj.HasChanges);
            for (int i = 0; i < collection.Count; i++)
            {
                Assert.Equal(collection[i].Id, di.Data.DonorList[i].Id);
                Assert.Equal(collection[i].FamilyId, di.Data.DonorList[i].FamilyId);
                Assert.Equal(collection[i].FamilyRelationship, di.Data.DonorList[i].FamilyRelationship);
                Assert.Equal(collection[i].FirstName, di.Data.DonorList[i].FirstName);
                Assert.Equal(collection[i].PreferredName, di.Data.DonorList[i].PreferredName);
                Assert.Equal(collection[i].LastName, di.Data.DonorList[i].LastName);
                Assert.Equal(collection[i].Gender, di.Data.DonorList[i].Gender);
                Assert.Equal(collection[i].Email, di.Data.DonorList[i].Email);
                Assert.Equal(collection[i].Email2, di.Data.DonorList[i].Email2);
                Assert.Equal(collection[i].HomePhone, di.Data.DonorList[i].HomePhone);
                Assert.Equal(collection[i].MobilePhone, di.Data.DonorList[i].MobilePhone);
                Assert.Equal(collection[i].WorkPhone, di.Data.DonorList[i].WorkPhone);
                Assert.Equal(collection[i].AddressType, di.Data.DonorList[i].AddressType);
                Assert.Equal(collection[i].Address, di.Data.DonorList[i].Address);
                Assert.Equal(collection[i].Address2, di.Data.DonorList[i].Address2);
                Assert.Equal(collection[i].City, di.Data.DonorList[i].City);
                Assert.Equal(collection[i].State, di.Data.DonorList[i].State);
                Assert.Equal(collection[i].Zip, di.Data.DonorList[i].Zip);
                Assert.Equal(collection[i].Country, di.Data.DonorList[i].Country);
                Assert.Equal(collection[i].Birthday, di.Data.DonorList[i].Birthday);
                Assert.Equal(collection[i].Baptism, di.Data.DonorList[i].Baptism);
                Assert.Equal(collection[i].Deathday, di.Data.DonorList[i].Deathday);
                Assert.Equal(collection[i].GroupGiving, di.Data.DonorList[i].GroupGiving);
                Assert.Equal(collection[i].ChurchMember, di.Data.DonorList[i].ChurchMember);
                Assert.Equal(collection[i].MaritalStatus, di.Data.DonorList[i].MaritalStatus);
                Assert.Equal(collection[i].Notes, di.Data.DonorList[i].Notes);
                Assert.Equal(collection[i].ActiveGroups, di.Data.DonorList[i].ActiveGroups);
                Assert.Equal(collection[i].LastUpdated, di.Data.DonorList[i].LastUpdated);
            }
        }

        [Fact]
        public void ImportDonorViewModel_ReadFile()
        {
            // Arrange
            var td = new TestData();
            byte[] buffer = Encoding.UTF8.GetBytes(td.DonorsCsv);
            MockFileSystem mockfs = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { "donors.csv", new MockFileData(buffer) }
            });
            di.FileSystem = mockfs;

            ImportDonorViewModel obj = new ImportDonorViewModel();

            // Act
            obj.ReadFile("donors.csv");

            // Assert
            Assert.Equal(td.DonorList.Count, obj.Collection.Count);
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
}
