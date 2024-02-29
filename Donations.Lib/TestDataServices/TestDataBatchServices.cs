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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<int> AddBatch(Batch batch)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<Batch>> FilterBatch(enumDateFilterOptions batchFilter, string date, string date2)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		ObservableCollection<Batch> batches = new ObservableCollection<Batch>();

		switch (batchFilter)
		{
			case enumDateFilterOptions.CurrentYear:
			case enumDateFilterOptions.PreviousYear:
			case enumDateFilterOptions.SelectYear:
#pragma warning disable CS8604 // Possible null reference argument.
				batches = new ObservableCollection<Batch>(BatchList.Where(x => x.Date.Contains(date)));
#pragma warning restore CS8604 // Possible null reference argument.
				break;
			case enumDateFilterOptions.DateRange:
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8604 // Possible null reference argument.
				batches = new ObservableCollection<Batch>(BatchList.Where(x => DateOnly.Parse(date) <= DateOnly.Parse(x.Date) && DateOnly.Parse(x.Date) <= DateOnly.Parse(date2)));
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8604 // Possible null reference argument.
				break;
		}

		return batches;
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<Batch?> GetBatchById(int id)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<string>> GetBatchYears()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return new ObservableCollection<string>(BatchList!.Select(x => x.Date[..4]));
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string> GetEarliestDate()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8603 // Possible null reference return.
		return BatchList!.Select(x => x.Date).Min();
#pragma warning restore CS8603 // Possible null reference return.
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string> GetLatestDate()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8603 // Possible null reference return.
		return BatchList!.Select(x => x.Date).Max();
#pragma warning restore CS8603 // Possible null reference return.
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<Batch>> LoadBatches()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8604 // Possible null reference argument.
		return new ObservableCollection<Batch>(BatchList);
#pragma warning restore CS8604 // Possible null reference argument.
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string?> SaveBatches(ObservableCollection<Batch> batches, bool force = false, Action<long, long>? progUpdate = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task UpdateBatch(Batch batch)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
