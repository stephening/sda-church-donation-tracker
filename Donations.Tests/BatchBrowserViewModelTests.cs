using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.Reflection;
using Xunit;

namespace Donations.Tests;

/// <summary>
/// All these test need to be in the same class so they will run synchronously
/// </summary>
public class BatchBrowserViewModelTests : TestBase
{
	[Theory]
	[InlineData(enumDateFilterOptions.CurrentYear)]
	[InlineData(enumDateFilterOptions.PreviousYear)]
	[InlineData(enumDateFilterOptions.SelectYear)]
	[InlineData(enumDateFilterOptions.DateRange)]
	public void DateFilterOption(enumDateFilterOptions option)
	{
		// Arrange
		BatchBrowserViewModel obj = DependencyInjection.Resolve<BatchBrowserViewModel>();
		obj.DateFilterOption = option;

		// Act

		// Assert
		Assert.Equal(option, obj.DateFilterOption);
	}

	[Theory]
	[InlineData("Anystring")]
	[InlineData("2022")]
	[InlineData("2020")]
	public void FilterYear(string filter)
	{
		// Arrange
		BatchBrowserViewModel obj = DependencyInjection.Resolve<BatchBrowserViewModel>();
		obj.FilterYear = filter;

		// Act

		// Assert
		Assert.Equal(filter, obj.FilterYear);
	}

	[Fact]
	public void FilterStartEndDates()
	{
		// Arrange
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();

		// hack to clear test data from batch service
		batchServices.SaveBatches(new ObservableCollection<Batch>());

		batchServices.AddBatch(new Batch() { Date = "1990/1/1" });
		batchServices.AddBatch(new Batch() { Date = "12/31/2022" });
		BatchBrowserViewModel obj = DependencyInjection.Resolve<BatchBrowserViewModel>();

		// Act

		// Assert
		Assert.Equal("1990/01/01", obj.FilterStartDate);
		Assert.Equal("2022/12/31", obj.FilterEndDate);
	}

	[Theory]
	[InlineData("1/1/2022", "2022/01/01")]
	[InlineData("02/02/2020", "2020/02/02")]
	[InlineData("April 15, 1990", "1990/04/15")]
	[InlineData("", "")]
	[InlineData("anything", "")]
	public void FilterStartDate(string filter, string expected)
	{
		// Arrange
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();

		// hack to clear test data from batch service
		batchServices.SaveBatches(new ObservableCollection<Batch>());

		BatchBrowserViewModel obj = DependencyInjection.Resolve<BatchBrowserViewModel>();
		obj.FilterStartDate = filter;

		// Act

		// Assert
		Assert.Equal(expected, obj.FilterStartDate);
	}

	[Theory]
	[InlineData("1/1/2022", "2022/01/01")]
	[InlineData("02/02/2020", "2020/02/02")]
	[InlineData("April 15, 1990", "1990/04/15")]
	[InlineData("", "")]
	[InlineData("anything", "")]
	public void FilterEndDate(string filter, string expected)
	{
		// Arrange
		IBatchServices batchServices = DependencyInjection.Resolve<IBatchServices>();

		// hack to clear test data from batch service
		batchServices.SaveBatches(new ObservableCollection<Batch>());

		BatchBrowserViewModel obj = DependencyInjection.Resolve<BatchBrowserViewModel>();
		obj.FilterEndDate = filter;

		// Act

		// Assert
		Assert.Equal(expected, obj.FilterEndDate);
	}

	[Fact]
	public async Task GetDonationByBatchId()
	{
		// Arrange
		var td = new TestData();
		BatchBrowserViewModel obj = DependencyInjection.Resolve<BatchBrowserViewModel>();
#pragma warning disable CS8604 // Possible null reference argument.
		var expected = new ObservableCollection<Donation>(td.DonationList.Where(x => x.BatchId == 1));
#pragma warning restore CS8604 // Possible null reference argument.
		bool found = false;

		// Act
		var donations = await obj.GetDonationsByBatchId(1);

		// Assert
		foreach (Donation donation in donations)
		{
			found = false;
			Assert.Equal(1, donation.BatchId);
			foreach (var exp in expected)
			{
				if (donation.Id == exp.Id)
				{
					Assert.Equal(donation.Date, exp.Date);
					Assert.Equal(donation.Name, exp.Name);
					Assert.Equal(donation.Category, exp.Category);
					Assert.Equal(donation.EnvelopeId, exp.EnvelopeId);
					Assert.Equal(donation.Method, exp.Method);
					Assert.Equal(donation.Note, exp.Note);
					Assert.Equal(donation.TaxDeductible, exp.TaxDeductible);
					Assert.Equal(donation.TransactionNumber, exp.TransactionNumber);
					Assert.Equal(donation.Value, exp.Value);
					found = true;
					break;
				}
			}

			Assert.True(found);
		}
	}

	[Fact]
	public async Task Loading()
	{
		// Arrange
		var td = new TestData();
#pragma warning disable CS8604 // Possible null reference argument.
		var batchesIn2023 = td.BatchList.Where(x => x.Date.StartsWith("2023"));
#pragma warning restore CS8604 // Possible null reference argument.
		BatchBrowserViewModel obj = DependencyInjection.Resolve<BatchBrowserViewModel>();
		obj.DateFilterOption = enumDateFilterOptions.SelectYear;
		obj.FilterYear = "2023";
		Type t = typeof(BatchBrowserViewModel);
		bool found = false;

		// Act
		// call private Timer_Tick because it is a Dispatcher timer callback that get's created in the constructor, but since there is no Dispatcher, the callback doesn't run
		// it needs to run to enable the Batch filtering in the Loading() function
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
		t.InvokeMember("Timer_Tick", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { null, null });
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
		await obj.Loading();

		// Assert
		var batches = obj.BatchListSource.Source as ObservableCollection<Batch>;

		foreach (var batch in batches)
		{
			found = false;
			foreach (var item in batchesIn2023)
			{
				if (item.Id == batch.Id)
				{
					Assert.Equal(batch.Date, item.Date);
					Assert.Equal(batch.Note, item.Note);
					Assert.Equal(batch.Source, item.Source);
					Assert.Equal(batch.Total, item.Total);
					Assert.Equal(batch.ActualTotal, item.ActualTotal);
					found = true;
					break;
				}
			}
			Assert.True(found);
		}
	}

	[Theory]
	[InlineData("1/1/1776", "12/31/2023")]
	[InlineData("1/1/1500", "12/31/2022")]
	public async Task BatchListUpdated(string startDate, string endDate)
	{
		// Arrange
		var td = new TestData();
		var start = DateOnly.Parse(startDate).ToString("yyyy/MM/dd");
		var end = DateOnly.Parse(endDate).ToString("yyyy/MM/dd");
#pragma warning disable CS8604 // Possible null reference argument.
		var batchesInRange = td.BatchList.Where(x => (0 >= string.Compare(start, x.Date)) && (0 <= string.Compare(end, x.Date))).ToList();
#pragma warning restore CS8604 // Possible null reference argument.
		var batchServices = DependencyInjection.Resolve<IBatchServices>();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		batchServices.SaveBatches(new ObservableCollection<Batch>(batchesInRange));
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
		BatchBrowserViewModel obj = DependencyInjection.Resolve<BatchBrowserViewModel>();
		Type t = typeof(BatchBrowserViewModel);
		bool found = false;

		// Act
		// call private Timer_Tick because it is a Dispatcher timer callback that get's created in the constructor, but since there is no Dispatcher, the callback doesn't run
		// it needs to run to enable the Batch filtering in the Loading() function
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
		t.InvokeMember("Timer_Tick", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, obj, new object[] { null, null });
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
		obj.DateFilterOption = enumDateFilterOptions.DateRange;
		await obj.BatchListUpdated();

		// Assert
		var batches = obj.BatchListSource.Source as ObservableCollection<Batch>;

		foreach (var batch in batches)
		{
			found = false;
			foreach (var item in batchesInRange)
			{
				if (item.Id == batch.Id)
				{
					Assert.Equal(batch.Date, item.Date);
					Assert.Equal(batch.Note, item.Note);
					Assert.Equal(batch.Source, item.Source);
					Assert.Equal(batch.Total, item.Total);
					Assert.Equal(batch.ActualTotal, item.ActualTotal);
					found = true;
					break;
				}
			}
			Assert.True(found);
		}
	}
}
