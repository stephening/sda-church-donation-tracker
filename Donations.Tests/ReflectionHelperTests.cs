using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Xunit;

namespace Donations.Tests;

public class ReflectionHelperTests : TestBase
{
	private readonly IReflectionHelpers _reflectionHelpers;

	public ReflectionHelperTests()
	{
		_reflectionHelpers = DependencyInjection.Resolve<IReflectionHelpers>();
	}

	[Fact]
	public void AdventistGivingCopy()
	{
		// Arrange
		var td = new TestData();
		var expected = td.AdventistGivingList[0];

		// Act
		var copy = _reflectionHelpers.CopyModel<AdventistGiving>(expected);

		// Assert
		Assert.True(_reflectionHelpers.SameModel<AdventistGiving>(expected, copy));
		// now change it and then make sure it is not same
		copy.LastName = "";
		Assert.False(_reflectionHelpers.SameModel<AdventistGiving>(expected, copy));
	}

	[Fact]
	public void AdventistGivingProperties()
	{
		// Arrange
		var td = new TestData();
		var expected = td.AdventistGivingList[0];

		// Act
		var copy = _reflectionHelpers.CopyModel<AdventistGiving>(expected);

		// Assert
		foreach (var item in _reflectionHelpers.ModelProperties<AdventistGiving>(copy))
		{
			Assert.Equal(item.GetValue(copy), item.GetValue(expected));
		}
	}

	[Fact]
	public void AdventistGivingCategoryMapCopy()
	{
		// Arrange
		var item = new AGCategoryMapItem() { AGCategoryCode = 100, AGCategoryName = "One hundred", CategoryCode = 1000, CategoryDescription = "Tithe" };

		// Act
		var copy = _reflectionHelpers.CopyModel<AGCategoryMapItem>(item);

		// Assert
		Assert.True(_reflectionHelpers.SameModel<AGCategoryMapItem>(item, copy));
		// now change it and then make sure it is not same
		copy.AGCategoryName = "";
		Assert.False(_reflectionHelpers.SameModel<AGCategoryMapItem>(item, copy));
	}

	[Fact]
	public void AdventistGivingCategoryMapProperties()
	{
		// Arrange
		var expected = new AGCategoryMapItem() { AGCategoryCode = 100, AGCategoryName = "One hundred", CategoryCode = 1000, CategoryDescription = "Tithe" };

		// Act
		var copy = _reflectionHelpers.CopyModel<AGCategoryMapItem>(expected);

		// Assert
		foreach (var item in _reflectionHelpers.ModelProperties<AGCategoryMapItem>(copy))
		{
			Assert.Equal(item.GetValue(copy), item.GetValue(expected));
		}
	}

	[Fact]
	public void AdventistGivingDonorMapCopy()
	{
		// Arrange
		var td = new TestData();
		var donor = td.DonorList[0];
		var item = new AGDonorMapItem()
		{
			AGLastName = donor.LastName,
			AGFirstName = donor.FirstName,
			AGAddress = donor.Address,
			AGCity = donor.City,
			AGState = donor.State,
			AGZip = donor.Zip,
			DonorId = donor.Id,
		};
		item.RefreshDonorFields(donor);

		// Act
		var copy = _reflectionHelpers.CopyModel<AGDonorMapItem>(item);

		// Assert
		Assert.True(_reflectionHelpers.SameModel<AGDonorMapItem>(item, copy));
		// now change it and then make sure it is not same
		copy.LastName = "";
		Assert.False(_reflectionHelpers.SameModel<AGDonorMapItem>(item, copy));
	}

	[Fact]
	public void AdventistGivingDonorMapProperties()
	{
		// Arrange
		var td = new TestData();
		var donor = td.DonorList[0];
		var expected = new AGDonorMapItem()
		{
			AGLastName = donor.LastName,
			AGFirstName = donor.FirstName,
			AGAddress = donor.Address,
			AGCity = donor.City,
			AGState = donor.State,
			AGZip = donor.Zip,
			DonorId = donor.Id,
		};
		expected.RefreshDonorFields(donor);

		// Act
		var copy = _reflectionHelpers.CopyModel<AGDonorMapItem>(expected);

		// Assert
		foreach (var item in _reflectionHelpers.ModelProperties<AGDonorMapItem>(copy))
		{
			Assert.Equal(item.GetValue(copy), item.GetValue(expected));
		}
	}

	[Fact]
	public void DonorCopy()
	{
		// Arrange
		var td = new TestData();
		var expected = td.DonorList[0];

		// Act
		var copy = _reflectionHelpers.CopyModel<Donor>(expected);

		// Assert
		Assert.True(_reflectionHelpers.SameModel<Donor>(expected, copy));
		// now change it and then make sure it is not same
		copy.LastName = "";
		Assert.False(_reflectionHelpers.SameModel<Donor>(expected, copy));
	}

	[Fact]
	public void DonorProperties()
	{
		// Arrange
		var td = new TestData();
		var expected = td.DonorList[0];

		// Act
		var copy = _reflectionHelpers.CopyModel<Donor>(expected);

		// Assert
		foreach (var item in _reflectionHelpers.ModelProperties<Donor>(copy))
		{
			Assert.Equal(item.GetValue(copy), item.GetValue(expected));
		}
	}

	[Fact]
	public void DonationCopy()
	{
		// Arrange
		var td = new TestData();
		var expected = td.DonationList[0];

		// Act
		var copy = _reflectionHelpers.CopyModel<Donation>(expected);

		// Assert
		Assert.True(_reflectionHelpers.SameModel<Donation>(expected, copy));
		// now change it and then make sure it is not same
		copy.LastName = "";
		Assert.False(_reflectionHelpers.SameModel<Donation>(expected, copy));
	}

	[Fact]
	public void DonationProperties()
	{
		// Arrange
		var td = new TestData();
		var expected = td.DonationList[0];

		// Act
		var copy = _reflectionHelpers.CopyModel<Donation>(expected);

		// Assert
		foreach (var item in _reflectionHelpers.ModelProperties<Donation>(copy))
		{
			Assert.Equal(item.GetValue(copy), item.GetValue(expected));
		}
	}

	[Fact]
	public void AppSettingsCopy()
	{
		// Arrange
		var td = new TestData();
		var expected = td.AppSettings;

		// Act
		var copy = _reflectionHelpers.CopyModel<AppSettings>(expected);

		// Assert
		Assert.True(_reflectionHelpers.SameModel<AppSettings>(expected, copy));
		// now change it and then make sure it is not same
		copy.EmailSmtpServer = "";
		Assert.False(_reflectionHelpers.SameModel<AppSettings>(expected, copy));
	}

	[Fact]
	public void AppSettingsProperties()
	{
		// Arrange
		var td = new TestData();
		var expected = td.AppSettings;

		// Act
		var copy = _reflectionHelpers.CopyModel<AppSettings>(expected);

		// Assert
		foreach (var item in _reflectionHelpers.ModelProperties<AppSettings>(copy))
		{
			Assert.Equal(item.GetValue(copy), item.GetValue(expected));
		}
	}

	[Fact]
	public void BatchCopy()
	{
		// Arrange
		var td = new TestData();
		var expected = td.BatchList[0];

		// Act
		var copy = _reflectionHelpers.CopyModel<Batch>(expected);

		// Assert
		Assert.True(_reflectionHelpers.SameModel<Batch>(expected, copy));
		// now change it and then make sure it is not same
		copy.ActualTotal = -1;
		Assert.False(_reflectionHelpers.SameModel<Batch>(expected, copy));
	}

	[Fact]
	public void BatchProperties()
	{
		// Arrange
		var td = new TestData();
		var expected = td.BatchList[0];

		// Act
		var copy = _reflectionHelpers.CopyModel<Batch>(expected);

		// Assert
		foreach (var item in _reflectionHelpers.ModelProperties<Batch>(copy))
		{
			Assert.Equal(item.GetValue(copy), item.GetValue(expected));
		}
	}

	[Fact]
	public void CategoryCopy()
	{
		// Arrange
		var td = new TestData();
		var expected = td.CatList[0];

		// Act
		var copy = _reflectionHelpers.CopyModel<Category>(expected);

		// Assert
		Assert.True(_reflectionHelpers.SameModel<Category>(expected, copy));
		// now change it and then make sure it is not same
		copy.Description = "";
		Assert.False(_reflectionHelpers.SameModel<Category>(expected, copy));
	}

	[Fact]
	public void CategoryProperties()
	{
		// Arrange
		var td = new TestData();
		var expected = td.CatList[0];

		// Act
		var copy = _reflectionHelpers.CopyModel<Category>(expected);

		// Assert
		foreach (var item in _reflectionHelpers.ModelProperties<Category>(copy))
		{
			Assert.Equal(item.GetValue(copy), item.GetValue(expected));
		}
	}

	[Fact]
	public void CategorySumCopy()
	{
		// Arrange
		var expected = new CategorySum() { Category = "Category", Description = "Description", Code = 100, Sum = 100 };

		// Act
		var copy = _reflectionHelpers.CopyModel<CategorySum>(expected);

		// Assert
		Assert.True(_reflectionHelpers.SameModel<CategorySum>(expected, copy));
		// now change it and then make sure it is not same
		copy.Description = "";
		Assert.False(_reflectionHelpers.SameModel<CategorySum>(expected, copy));
	}

	[Fact]
	public void CategorySumProperties()
	{
		// Arrange
		var expected = new CategorySum() { Category = "Category", Description = "Description", Code = 100, Sum = 100 };

		// Act
		var copy = _reflectionHelpers.CopyModel<CategorySum>(expected);

		// Assert
		foreach (var item in _reflectionHelpers.ModelProperties<CategorySum>(copy))
		{
			Assert.Equal(item.GetValue(copy), item.GetValue(expected));
		}
	}

	[Fact]
	public void DonorChangeCopy()
	{
		// Arrange
		var expected = new DonorChange() { WhatChanged = "What changed", WhenChanged = DateTime.Parse("1/5/2021"), WhoChanged = "Me", DonorId = 1, Id = 1, Name = "Name" };

		// Act
		var copy = _reflectionHelpers.CopyModel<DonorChange>(expected);

		// Assert
		Assert.True(_reflectionHelpers.SameModel<DonorChange>(expected, copy));
		// now change it and then make sure it is not same
		copy.WhatChanged = "";
		Assert.False(_reflectionHelpers.SameModel<DonorChange>(expected, copy));
	}

	[Fact]
	public void DonorChangeProperties()
	{
		// Arrange
		var expected = new DonorChange() { WhatChanged = "What changed", WhenChanged = DateTime.Parse("1/5/2021"), WhoChanged = "Me", DonorId = 1, Id = 1, Name = "Name" };

		// Act
		var copy = _reflectionHelpers.CopyModel<DonorChange>(expected);

		// Assert
		foreach (var item in _reflectionHelpers.ModelProperties<DonorChange>(copy))
		{
			Assert.Equal(item.GetValue(copy), item.GetValue(expected));
		}
	}

	[Fact]
	public void DonorReportCopy()
	{
		// Arrange
		var expected = new DonorReport() { Action = "Action", DonorId = 1, LastSent = DateTime.Parse("12/31/1999") };

		// Act
		var copy = _reflectionHelpers.CopyModel<DonorReport>(expected);

		// Assert
		Assert.True(_reflectionHelpers.SameModel<DonorReport>(expected, copy));
		// now change it and then make sure it is not same
		copy.Action = "";
		Assert.False(_reflectionHelpers.SameModel<DonorReport>(expected, copy));
	}

	[Fact]
	public void DonorReportProperties()
	{
		// Arrange
		var expected = new DonorReport() { Action = "Action", DonorId = 1, LastSent = DateTime.Parse("12/31/1999") };

		// Act
		var copy = _reflectionHelpers.CopyModel<DonorReport>(expected);

		// Assert
		foreach (var item in _reflectionHelpers.ModelProperties<DonorReport>(copy))
		{
			Assert.Equal(item.GetValue(copy), item.GetValue(expected));
		}
	}
}
