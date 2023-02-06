using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using Xunit;

namespace Donations.Tests
{
	public partial class DataAccessSequentialTests
	{
		[Theory]
		[InlineData(0)]
		[InlineData(100)]
		[InlineData(-1)]
		[InlineData(null)]
		public void DonorMapViewModel_SelectedRowIndex(int? param)
		{
			// Arrange
			DonorMapViewModel obj = new DonorMapViewModel() { SelectedRowIndex = param };

			// Assert
			Assert.Equal(param, obj.SelectedRowIndex);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void DonorMapViewModel_HasChanges(bool expected)
		{
			// Arrange
			DonorMapViewModel obj = new DonorMapViewModel() { HasChanges = expected };

			// Assert
			Assert.Equal(expected, obj.HasChanges);
		}

		[Fact]
		public void DonorMapViewModel()
		{
			// Arrange
			var td = new TestData();
			di.Data.AGDonorMapList = new ObservableCollection<AGDonorMapItem>();
			di.Data.AGDonorMap = new Dictionary<string, AGDonorMapItem>();

			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			di.Data.AGDonorMap["1"] = new AGDonorMapItem() { AGDonorHash = "1", DonorId = 1 }; di.Data.AGDonorMapList.Add(di.Data.AGDonorMap["1"]);
			di.Data.AGDonorMap["2"] = new AGDonorMapItem() { AGDonorHash = "2", DonorId = 2 }; di.Data.AGDonorMapList.Add(di.Data.AGDonorMap["2"]);
			di.Data.AGDonorMap["3"] = new AGDonorMapItem() { AGDonorHash = "3", DonorId = 3 }; di.Data.AGDonorMapList.Add(di.Data.AGDonorMap["3"]);
			di.Data.AGDonorMap["4"] = new AGDonorMapItem() { AGDonorHash = "4", DonorId = 4 }; di.Data.AGDonorMapList.Add(di.Data.AGDonorMap["4"]);
			di.Data.AGDonorMap["5"] = new AGDonorMapItem() { AGDonorHash = "5", DonorId = 5 }; di.Data.AGDonorMapList.Add(di.Data.AGDonorMap["5"]);
			di.Data.AGDonorMap["6"] = new AGDonorMapItem() { AGDonorHash = "6", DonorId = 6 }; di.Data.AGDonorMapList.Add(di.Data.AGDonorMap["6"]);

			DonorMapViewModel obj = new DonorMapViewModel();

			// Act
			obj.Loaded();

			// Assert
			Assert.Equal(6, obj.DonorMapList.Count);
			Assert.Equal(di.Data.DonorList[0].LastName, obj.DonorMapList[0].LastName);
			Assert.Equal(di.Data.DonorList[1].LastName, obj.DonorMapList[1].LastName);
			Assert.Equal(di.Data.DonorList[2].LastName, obj.DonorMapList[2].LastName);
			Assert.Equal(di.Data.DonorList[3].LastName, obj.DonorMapList[3].LastName);
			Assert.Equal(di.Data.DonorList[4].LastName, obj.DonorMapList[4].LastName);
			Assert.Equal(di.Data.DonorList[5].LastName, obj.DonorMapList[5].LastName);

			// Act again
			obj.SelectedRowIndex = 2;
			obj.DeleteRowCmd.Execute(null);

			// Assert
			Assert.Equal(5, obj.DonorMapList.Count);
			Assert.Equal(di.Data.DonorList[0].LastName, obj.DonorMapList[0].LastName);
			Assert.Equal(di.Data.DonorList[1].LastName, obj.DonorMapList[1].LastName);
			Assert.Equal(di.Data.DonorList[3].LastName, obj.DonorMapList[2].LastName);
			Assert.Equal(di.Data.DonorList[4].LastName, obj.DonorMapList[3].LastName);
			Assert.Equal(di.Data.DonorList[5].LastName, obj.DonorMapList[4].LastName);

			// Act again
			obj.RevertCmd.Execute(null);

			// Assert
			Assert.Equal(6, obj.DonorMapList.Count);
			Assert.Equal(di.Data.DonorList[0].LastName, obj.DonorMapList[0].LastName);
			Assert.Equal(di.Data.DonorList[1].LastName, obj.DonorMapList[1].LastName);
			Assert.Equal(di.Data.DonorList[2].LastName, obj.DonorMapList[2].LastName);
			Assert.Equal(di.Data.DonorList[3].LastName, obj.DonorMapList[3].LastName);
			Assert.Equal(di.Data.DonorList[4].LastName, obj.DonorMapList[4].LastName);
			Assert.Equal(di.Data.DonorList[5].LastName, obj.DonorMapList[5].LastName);

			// Act again
			obj.SetDonor(obj.DonorMapList[5], di.Data.DonorDict[7]);

			// Assert
			Assert.Equal(6, obj.DonorMapList.Count);
			Assert.Equal(di.Data.DonorList[0].LastName, obj.DonorMapList[0].LastName);
			Assert.Equal(di.Data.DonorList[1].LastName, obj.DonorMapList[1].LastName);
			Assert.Equal(di.Data.DonorList[2].LastName, obj.DonorMapList[2].LastName);
			Assert.Equal(di.Data.DonorList[3].LastName, obj.DonorMapList[3].LastName);
			Assert.Equal(di.Data.DonorList[4].LastName, obj.DonorMapList[4].LastName);
			Assert.Equal(di.Data.DonorDict[7].LastName, obj.DonorMapList[5].LastName);

			// Act again
			obj.SaveChangesCmd.Execute(null);

			// Assert
			Assert.Equal(6, di.Data.AGDonorMapList.Count);
			Assert.Equal("Doe", di.Data.AGDonorMapList[0].LastName);
			Assert.Equal("Doe", di.Data.AGDonorMapList[1].LastName);
			Assert.Equal("Doe", di.Data.AGDonorMapList[2].LastName);
			Assert.Equal("Luther", di.Data.AGDonorMapList[3].LastName);
			Assert.Equal("Wycliffe", di.Data.AGDonorMapList[4].LastName);
			Assert.Equal("Tyndale", di.Data.AGDonorMapList[5].LastName);

			// Act again
			obj.DeleteAllCmd.Execute(null);

			// Assert
			Assert.Equal(0, obj.DonorMapList.Count);

			// Act again
			obj.SaveChangesCmd.Execute(null);

			// Assert
			Assert.Equal(0, di.Data.AGDonorMapList.Count);
		}
	}
}
