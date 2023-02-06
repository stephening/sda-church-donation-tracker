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
		public void ImportDonationsViewModel_HasChanges(bool expected)
		{
			// Arrange
			ImportDonationsViewModel obj = new ImportDonationsViewModel() { HasChanges = expected };

			// Assert
			Assert.Equal(expected, obj.HasChanges);
		}

		[Fact]
		public void ImportDonationsViewModel_Save()
		{
			// Arrange
			var td = new TestData();
			MockFileSystem mockfs = new MockFileSystem();
			di.FileSystem = mockfs;

			di.Data.DonationDict = new Dictionary<int, Donation>();
			di.Data.DonationList = new ObservableCollection<Donation>();

			ImportDonationsViewModel obj = new ImportDonationsViewModel();

			obj.Collection = td.DonationList;

			// Act
			obj.Save(true);
			MockFileData mockDonations = mockfs.GetFile(di.Data.DonationsFileName);
			// get the data written to the mock file in Save()
			mockfs.AddFile(di.Data.DonationsFileName, new MockFileData(mockDonations.TextContents));
			// use xml deserializer to read it back in for verification
			var donations = di.Data.DeserializeXml<Donation>(di.Data.DonationsFileName);

			// Assert
			Assert.False(obj.HasChanges);
			for (int i = 0; i < donations.Count; i++)
			{
				Assert.Equal(donations[i].Id, di.Data.DonationList[i].Id);
				Assert.Equal(donations[i].DonorId, di.Data.DonationList[i].DonorId);
				Assert.Equal(donations[i].BatchId, di.Data.DonationList[i].BatchId);
				Assert.Equal(donations[i].Name, di.Data.DonationList[i].Name);
				Assert.Equal(donations[i].Category, di.Data.DonationList[i].Category);
				Assert.Equal(donations[i].Value, di.Data.DonationList[i].Value);
				Assert.Equal(donations[i].Date, di.Data.DonationList[i].Date);
				Assert.Equal(donations[i].TaxDeductible, di.Data.DonationList[i].TaxDeductible);
				Assert.Equal(donations[i].Note, di.Data.DonationList[i].Note);
				Assert.Equal(donations[i].Method, di.Data.DonationList[i].Method);
				Assert.Equal(donations[i].TransactionNumber, di.Data.DonationList[i].TransactionNumber);
			}
		}

		[Fact]
		public async void ImportDonationsViewModel_ReadFile()
		{
			// Arrange
			var td = new TestData();
			byte[] buffer = Encoding.UTF8.GetBytes(td.DonationsCsv);
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ "donations.csv", new MockFileData(buffer) }
			});

			di.Data.DonationDict = td.DonationDict;
			di.Data.DonationList = td.DonationList;
			di.Data.BatchDict = new Dictionary<int, Batch>();
			di.Data.BatchList = new ObservableCollection<Batch>();

			ImportDonationsViewModel obj = new ImportDonationsViewModel();

			// Act
			await obj.ReadFile("donations.csv");

			// Assert
			Assert.Equal(td.DonationList.Count, obj.Collection.Count);
			for (int i = 0; i < obj.Collection.Count; i++)
			{
				Assert.Equal(i + 1, obj.Collection[i].Id);
				Assert.Equal(td.DonationList[i].DonorId, obj.Collection[i].DonorId);
				Assert.Equal(td.DonationList[i].BatchId, obj.Collection[i].BatchId);
				Assert.True(Helper.Equal(td.DonationList[i].Name, obj.Collection[i].Name));
				Assert.True(Helper.Equal(td.DonationList[i].Category, obj.Collection[i].Category));
				Assert.Equal(td.DonationList[i].Value, obj.Collection[i].Value);
				Assert.True(Helper.Equal(td.DonationList[i].Date, obj.Collection[i].Date, eFlags.Date));
				Assert.Equal(td.DonationList[i].TaxDeductible, obj.Collection[i].TaxDeductible);
				Assert.True(Helper.Equal(td.DonationList[i].Note, obj.Collection[i].Note));
				enumMethod method = (null != td.DonationList[i].Method) ? td.DonationList[i].Method : enumMethod.Unknown;
				Assert.Equal(method, obj.Collection[i].Method);
				Assert.True(Helper.Equal(td.DonationList[i].TransactionNumber, obj.Collection[i].TransactionNumber));
			}

			Assert.Equal(5, obj.BatchList.Count);
			for (int j = 0; j < obj.BatchList.Count; j++)
			{
				Assert.Equal(td.BatchList[j].Id, obj.BatchList[j].Id);
				Assert.Equal(td.BatchList[j].Source, obj.BatchList[j].Source);
				Assert.Equal(td.BatchList[j].Date, obj.BatchList[j].Date);
				Assert.Equal(td.BatchList[j].Total, obj.BatchList[j].Total);
				Assert.Equal(td.BatchList[j].ActualTotal, obj.BatchList[j].ActualTotal);
			}
		}
	}
}
