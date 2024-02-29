using Donations.Lib.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IDonationServices
{
	Task<ObservableCollection<Donation>> LoadDonations();
	Task<string?> SaveDonations(ObservableCollection<Donation> donations, bool force = false, Action<long, long>? progUpdate = null);
	Task<ObservableCollection<Donation>>? GetDonationsByBatchId(int batchId, string? category = null);
	Task AddDonations(ObservableCollection<Donation> donations);
	Task RemoveDonationsByBatchId(int batchId);
	Task<ObservableCollection<Donation>> GetDonationsByDonorId(int id);
	Task<ObservableCollection<Donation>> GetDonationsByDonorIds(List<int> ids);
	Task RemapDonorId(int oldDonorId, int newDonorId);
	Task<ObservableCollection<Donation>> FilterDonationsByDate(enumDateFilterOptions dateFilter, string date, string date2);
	Task<int> GetNextId();
	Task<ObservableCollection<string>> GetDonationYears();
	Task<string> GetEarliestDate();
	Task<string> GetLatestDate();
}
