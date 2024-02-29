using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using Donations.Tests.Views;
using Xunit;

namespace Donations.Tests;

public class DonorMapViewModelTests : TestBase
{
	public DonorMapViewModel? DonorMapViewModelDataContext { get; set; }

	[Theory]
	[InlineData(true)]
	[InlineData(false)]
	public void HasChanges(bool expected)
	{
		// Arrange
		DonorMapViewModel obj = DependencyInjection.Resolve<DonorMapViewModel>();
		obj.HasChanges = expected;

		// Assert
		Assert.Equal(expected, obj.HasChanges);
	}

	[StaFact]
	public async Task DonorMapViewModel()
	{
		// Arrange
		IDonorMapServices donorMapServices = DependencyInjection.Resolve<IDonorMapServices>();
		IDonorServices donorServices = DependencyInjection.Resolve<IDonorServices>();

		donorMapServices.AGDonorMap!["1"] = new AGDonorMapItem() { AGDonorHash = "1", DonorId = 1 }; donorMapServices.AGDonorMapList!.Add(donorMapServices.AGDonorMap["1"]); donorMapServices.AGDonorMap["1"].RefreshDonorFields(donorServices.GetDonorById(1));
		donorMapServices.AGDonorMap["2"] = new AGDonorMapItem() { AGDonorHash = "2", DonorId = 2 }; donorMapServices.AGDonorMapList.Add(donorMapServices.AGDonorMap["2"]); donorMapServices.AGDonorMap["2"].RefreshDonorFields(donorServices.GetDonorById(2));
		donorMapServices.AGDonorMap["3"] = new AGDonorMapItem() { AGDonorHash = "3", DonorId = 3 }; donorMapServices.AGDonorMapList.Add(donorMapServices.AGDonorMap["3"]); donorMapServices.AGDonorMap["3"].RefreshDonorFields(donorServices.GetDonorById(3));
		donorMapServices.AGDonorMap["4"] = new AGDonorMapItem() { AGDonorHash = "4", DonorId = 4 }; donorMapServices.AGDonorMapList.Add(donorMapServices.AGDonorMap["4"]); donorMapServices.AGDonorMap["4"].RefreshDonorFields(donorServices.GetDonorById(4));
		donorMapServices.AGDonorMap["5"] = new AGDonorMapItem() { AGDonorHash = "5", DonorId = 5 }; donorMapServices.AGDonorMapList.Add(donorMapServices.AGDonorMap["5"]); donorMapServices.AGDonorMap["5"].RefreshDonorFields(donorServices.GetDonorById(5));
		donorMapServices.AGDonorMap["6"] = new AGDonorMapItem() { AGDonorHash = "6", DonorId = 6 }; donorMapServices.AGDonorMapList.Add(donorMapServices.AGDonorMap["6"]); donorMapServices.AGDonorMap["6"].RefreshDonorFields(donorServices.GetDonorById(6));

		DonorMapViewModelDataContext = DependencyInjection.Resolve<DonorMapViewModel>();
		var view = new DonorMapViewTest();
		view.DataContext = this;
		DonorMapViewModel obj = DonorMapViewModelDataContext;
		view.Show();

		// Act
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		obj.Loading();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

		// Assert
		var donors = await donorServices.LoadDonors();
		Assert.Equal(6, obj.DonorMapList!.Count);
		Assert.Equal(donors![0].LastName, obj.DonorMapList[0].LastName);
		Assert.Equal(donors[1].LastName, obj.DonorMapList[1].LastName);
		Assert.Equal(donors[2].LastName, obj.DonorMapList[2].LastName);
		Assert.Equal(donors[3].LastName, obj.DonorMapList[3].LastName);
		Assert.Equal(donors[4].LastName, obj.DonorMapList[4].LastName);
		Assert.Equal(donors[5].LastName, obj.DonorMapList[5].LastName);

		// Act again
		view.UserControl.SelectGridRow(2);
		obj.DeleteRowCommand.Execute(null);

		// Assert
		Assert.Equal(5, obj.DonorMapList.Count);
		Assert.Equal(donors[0].LastName, obj.DonorMapList[0].LastName);
		Assert.Equal(donors[1].LastName, obj.DonorMapList[1].LastName);
		Assert.Equal(donors[3].LastName, obj.DonorMapList[2].LastName);
		Assert.Equal(donors[4].LastName, obj.DonorMapList[3].LastName);
		Assert.Equal(donors[5].LastName, obj.DonorMapList[4].LastName);

		// Act again
		obj.RevertCommand.Execute(null);

		// Assert
		Assert.Equal(6, obj.DonorMapList.Count);
		Assert.Equal(donors[0].LastName, obj.DonorMapList[0].LastName);
		Assert.Equal(donors[1].LastName, obj.DonorMapList[1].LastName);
		Assert.Equal(donors[2].LastName, obj.DonorMapList[2].LastName);
		Assert.Equal(donors[3].LastName, obj.DonorMapList[3].LastName);
		Assert.Equal(donors[4].LastName, obj.DonorMapList[4].LastName);
		Assert.Equal(donors[5].LastName, obj.DonorMapList[5].LastName);

		// Act again
		obj.SetDonor(obj.DonorMapList[5], donorServices.GetDonorById(7));

		// Assert
		Assert.Equal(6, obj.DonorMapList.Count);
		Assert.Equal(donors[0].LastName, obj.DonorMapList[0].LastName);
		Assert.Equal(donors[1].LastName, obj.DonorMapList[1].LastName);
		Assert.Equal(donors[2].LastName, obj.DonorMapList[2].LastName);
		Assert.Equal(donors[3].LastName, obj.DonorMapList[3].LastName);
		Assert.Equal(donors[4].LastName, obj.DonorMapList[4].LastName);
		Assert.Equal(donorServices.GetDonorById(7).LastName, obj.DonorMapList[5].LastName);

		// Act again
		obj.SaveChangesCommand.Execute(null);

		// Assert
		Assert.Equal(6, donorMapServices.AGDonorMapList.Count);
		Assert.Equal("Doe", donorMapServices.AGDonorMapList[0].LastName);
		Assert.Equal("Doe", donorMapServices.AGDonorMapList[1].LastName);
		Assert.Equal("Doe", donorMapServices.AGDonorMapList[2].LastName);
		Assert.Equal("Luther", donorMapServices.AGDonorMapList[3].LastName);
		Assert.Equal("Wycliffe", donorMapServices.AGDonorMapList[4].LastName);
		Assert.Equal("Tyndale", donorMapServices.AGDonorMapList[5].LastName);

		// Act again
		obj.DeleteAllCommand.Execute(null);

		// Assert
		Assert.Empty(obj.DonorMapList);

		// Act again
		obj.SaveChangesCommand.Execute(null);

		// Assert
		Assert.Empty(donorMapServices.AGDonorMapList);
	}
}
