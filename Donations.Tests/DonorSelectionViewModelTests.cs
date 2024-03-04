using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using Xunit;

namespace Donations.Tests;

public class DonorSelectionViewModelTest : TestBase
{
	[Theory]
	[InlineData("this is filter text")]
	[InlineData("")]
	public void LastNameFilterText(string param)
	{
		// Arrange
		DonorSelectionViewModel obj = DependencyInjection.Resolve<DonorSelectionViewModel>();
		obj.LastNameFilterText = param;

		// Assert
		Assert.Equal(param, obj.LastNameFilterText);
	}

	[Theory]
	[InlineData("this is filter text")]
	[InlineData("")]
	public void FirstNameFilterText(string param)
	{
		// Arrange
		DonorSelectionViewModel obj = DependencyInjection.Resolve<DonorSelectionViewModel>();
		obj.FirstNameFilterText = param;

		// Assert
		Assert.Equal(param, obj.FirstNameFilterText);
	}

	[Theory]
	[InlineData(false, -1)]
	[InlineData(true, 0)]
	[InlineData(true, 1)]
	[InlineData(true, 1000000)]
	public void OKEnabled(bool expected, int index)
	{
		// Arrange
		DonorSelectionViewModel obj = DependencyInjection.Resolve<DonorSelectionViewModel>();
		obj.SelectedDonorIndex = index;

		// Assert
		Assert.Equal(expected, obj.OKEnabled);
	}

	[Theory]
	[InlineData(-1)]
	[InlineData(0)]
	[InlineData(1)]
	[InlineData(1000000)]
	public void SelectedCategoryIndex(int param)
	{
		// Arrange
		DonorSelectionViewModel obj = DependencyInjection.Resolve<DonorSelectionViewModel>();
		obj.SelectedDonorIndex = param;

		// Assert
		Assert.Equal(param, obj.SelectedDonorIndex);
	}

	[Theory]
	[InlineData("", "", 7)]
	[InlineData("d", "", 3)]
	[InlineData("", "j", 5)]
	[InlineData("", "jo", 4)]
	[InlineData("d", "john", 2)]
	[InlineData("", "johnn", 1)]
	public async void Filter_TextChanged(string last, string first, int expected)
	{
		// Arrange
		IDonorServices donorServices = DependencyInjection.Resolve<IDonorServices>();

		DonorSelectionViewModel obj = DependencyInjection.Resolve<DonorSelectionViewModel>();
		obj.LastNameFilterText = last;
		obj.FirstNameFilterText = first;

		// Act
		obj.TextChanged();

		// Assert
		Assert.Equal(expected, ((ObservableCollection<Donor>)obj.ViewSource.Source).Count);
	}
}
