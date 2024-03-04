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

	public async Task<ObservableCollection<Donation>> FilterDonationsByDate(enumDateFilterOptions dateFilter, string date, string date2)
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
				donations = new ObservableCollection<Donation>(DonationList!.Where(x => DateOnly.Parse(date) <= DateOnly.Parse(x.Date) && DateOnly.Parse(x.Date) <= DateOnly.Parse(date2)));
				break;
		}

		return donations;
	}

	public async Task<ObservableCollection<Donation>>? GetDonationsByBatchId(int batchId, string? category = null)
	{
		return new ObservableCollection<Donation>(DonationList!.Where(d => d.BatchId == batchId && (string.IsNullOrEmpty(category) || d.Category == category)));
	}

	public async Task<ObservableCollection<Donation>> GetDonationsByDonorId(int id)
	{
		return new ObservableCollection<Donation>(DonationList!.Where(d => d.DonorId == id));
	}

	public async Task<ObservableCollection<Donation>> GetDonationsByDonorIds(List<int> ids)
	{
		return new ObservableCollection<Donation>(DonationList!.Where(d => ids.Contains(d.DonorId)));
	}

	public async Task<ObservableCollection<Donation>> LoadDonations()
	{
		return new ObservableCollection<Donation>(DonationList);
	}

	public Task RemapDonorId(int oldDonorId, int newDonorId)
	{
		throw new NotImplementedException();
	}

	public async Task RemoveDonationsByBatchId(int batchId)
	{
		var list = DonationList.ToList();
		list.RemoveAll(x => x.BatchId == batchId);
		DonationList = new ObservableCollection<Donation>(list);
	}

	public async Task<string?> SaveDonations(ObservableCollection<Donation> donations, bool force = false, Action<long, long>? progUpdate = null)
	{
		DonationList = new ObservableCollection<Donation>(donations);

		return null;
	}

	public async Task<int> GetNextId()
	{
		var maxId = DonationList.Any() ? DonationList!.Select(x => x.Id).Max() : 0;

		return maxId + 1;
	}

	public async Task<ObservableCollection<string>> GetDonationYears()
	{
		return new ObservableCollection<string>(DonationList!.Select(x => x.Date[..4]));
	}

	public async Task<string> GetEarliestDate()
	{
		return DonationList!.Select(x => x.Date).Min();
	}

	public async Task<string> GetLatestDate()
	{
		return DonationList!.Select(x => x.Date).Max();
	}
}
