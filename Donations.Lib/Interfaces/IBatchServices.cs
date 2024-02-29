using Donations.Lib.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IBatchServices
{
	Task<ObservableCollection<Batch>> LoadBatches();
	Task<string?> SaveBatches(ObservableCollection<Batch> batches, bool force = false, Action<long, long>? progUpdate = null);
	Task<ObservableCollection<Batch>> FilterBatch(enumDateFilterOptions batchFilter, string date, string date2);
	Task<Batch?> GetBatchById(int id);
	Task<ObservableCollection<string>> GetBatchYears();
	Task<string> GetEarliestDate();
	Task<string> GetLatestDate();
	Task DeleteBatch(int batchId);
	Task<int> AddBatch(Batch batch);
	Task UpdateBatch(Batch batch);
}
