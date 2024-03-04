using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataBatchServices : IBatchServices
{
	private ObservableCollection<Batch>? BatchList { get; set; }

	public TestDataBatchServices()
	{
		var td = new TestData();
		BatchList = td.BatchList;
	}

	public async Task<int> AddBatch(Batch batch)
	{
		var newId = ((0 == BatchList!.Count)
								? 0
								: BatchList!.Select(x => x.Id).Max())
								+ 1;
		batch.Id = newId;
		BatchList!.Add(batch);

		return batch.Id;
	}

	public Task DeleteBatch(int batchId)
	{
		throw new NotImplementedException();
	}

	public async Task<ObservableCollection<Batch>> FilterBatch(enumDateFilterOptions batchFilter, string date, string date2)
	{
		ObservableCollection<Batch> batches = new ObservableCollection<Batch>();

		switch (batchFilter)
		{
			case enumDateFilterOptions.CurrentYear:
			case enumDateFilterOptions.PreviousYear:
			case enumDateFilterOptions.SelectYear:
				batches = new ObservableCollection<Batch>(BatchList.Where(x => x.Date.Contains(date)));
				break;
			case enumDateFilterOptions.DateRange:
				batches = new ObservableCollection<Batch>(BatchList.Where(x => DateOnly.Parse(date) <= DateOnly.Parse(x.Date) && DateOnly.Parse(x.Date) <= DateOnly.Parse(date2)));
				break;
		}

		return batches;
	}

	public async Task<Batch?> GetBatchById(int id)
	{
		var list = BatchList!.Where(x => x.Id == id);
		if (list.Count() > 0)
		{
			return list.First();
		}
		else
		{
			throw new Exception();
		}
	}

	public async Task<ObservableCollection<string>> GetBatchYears()
	{
		return new ObservableCollection<string>(BatchList!.Select(x => x.Date[..4]));
	}

	public async Task<string> GetEarliestDate()
	{
		return BatchList!.Select(x => x.Date).Min();
	}

	public async Task<string> GetLatestDate()
	{
		return BatchList!.Select(x => x.Date).Max();
	}

	public async Task<ObservableCollection<Batch>> LoadBatches()
	{
		return new ObservableCollection<Batch>(BatchList);
	}

	public async Task<string?> SaveBatches(ObservableCollection<Batch> batches, bool force = false, Action<long, long>? progUpdate = null)
	{
		BatchList = new ObservableCollection<Batch>(batches);

		return null;
	}

	//public async Task ReplaceBatchData(ObservableCollection<Batch> batchList)
	//{
	//	BatchList?.Clear();
	//	BatchDict?.Clear();

	//	foreach (var batch in batchList)
	//	{
	//		BatchList?.Add(batch);
	//		BatchDict![batch.Id] = batch;
	//	}
	//}

	public async Task UpdateBatch(Batch batch)
	{
		for (int i = 0; i < BatchList.Count; i++)
		{
			if (BatchList[i].Id == batch.Id)
			{
				BatchList[i] = batch;
			}
		}
	}
}
