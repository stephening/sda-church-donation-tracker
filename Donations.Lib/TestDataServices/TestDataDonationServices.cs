using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataDonationServices : IDonationServices
{
	private ObservableCollection<Donation>? DonationList { get; set; }

	public TestDataDonationServices()
	{
		var td = new TestData();
		DonationList = td.DonationList;
	}

	public async Task AddDonations(ObservableCollection<Donation> donations)
	{
		int newId = await GetNextId();

		foreach (var donation in donations)
		{
			donation.Id = newId++;
			DonationList!.Add(donation);
		}
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<Donation>> FilterDonationsByDate(enumDateFilterOptions dateFilter, string date, string date2)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		ObservableCollection<Donation> donations = new ObservableCollection<Donation>();

		switch (dateFilter)
		{
			case enumDateFilterOptions.CurrentYear:
			case enumDateFilterOptions.PreviousYear:
			case enumDateFilterOptions.SelectYear:
				donations = new ObservableCollection<Donation>(DonationList!.Where(x => x.Date.Contains(date)));
				break;
			case enumDateFilterOptions.DateRange:
#pragma warning disable CS8604 // Possible null reference argument.
				donations = new ObservableCollection<Donation>(DonationList!.Where(x => DateOnly.Parse(date) <= DateOnly.Parse(x.Date) && DateOnly.Parse(x.Date) <= DateOnly.Parse(date2)));
#pragma warning restore CS8604 // Possible null reference argument.
				break;
		}

		return donations;
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<Donation>>? GetDonationsByBatchId(int batchId, string? category = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return new ObservableCollection<Donation>(DonationList!.Where(d => d.BatchId == batchId && (string.IsNullOrEmpty(category) || d.Category == category)));
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<Donation>> GetDonationsByDonorId(int id)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return new ObservableCollection<Donation>(DonationList!.Where(d => d.DonorId == id));
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<Donation>> GetDonationsByDonorIds(List<int> ids)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return new ObservableCollection<Donation>(DonationList!.Where(d => ids.Contains(d.DonorId)));
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<Donation>> LoadDonations()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8604 // Possible null reference argument.
		return new ObservableCollection<Donation>(DonationList);
#pragma warning restore CS8604 // Possible null reference argument.
	}

	public Task RemapDonorId(int oldDonorId, int newDonorId)
	{
		throw new NotImplementedException();
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task RemoveDonationsByBatchId(int batchId)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8604 // Possible null reference argument.
		var list = DonationList.ToList();
#pragma warning restore CS8604 // Possible null reference argument.
		list.RemoveAll(x => x.BatchId == batchId);
		DonationList = new ObservableCollection<Donation>(list);
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string?> SaveDonations(ObservableCollection<Donation> donations, bool force = false, Action<long, long>? progUpdate = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		DonationList = new ObservableCollection<Donation>(donations);

		return null;
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<int> GetNextId()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8604 // Possible null reference argument.
		var maxId = DonationList.Any() ? DonationList!.Select(x => x.Id).Max() : 0;
#pragma warning restore CS8604 // Possible null reference argument.

		return maxId + 1;
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<string>> GetDonationYears()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return new ObservableCollection<string>(DonationList!.Select(x => x.Date[..4]));
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string> GetEarliestDate()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8603 // Possible null reference return.
		return DonationList!.Select(x => x.Date).Min();
#pragma warning restore CS8603 // Possible null reference return.
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string> GetLatestDate()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8603 // Possible null reference return.
		return DonationList!.Select(x => x.Date).Max();
#pragma warning restore CS8603 // Possible null reference return.
	}
}
