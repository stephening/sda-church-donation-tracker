using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataDonorServices : IDonorServices
{
	private ObservableCollection<Donor>? _donors;

	public TestDataDonorServices()
	{
		var td = new TestData();
		_donors = td.DonorList;
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<int> AddDonor(Donor donor)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		var newId = ((0 == _donors!.Count)
								? 0
								: _donors!.Select(x => x.Id).Max())
								+ 1;
		donor.Id = newId;
		_donors!.Add(donor);

		return donor.Id;
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task DeleteDonor(Donor donor)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		_donors.Remove(donor);
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<Donor>> FilterDonors(int count = 100, string last = "", string first = "")
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8604 // Possible null reference argument.
		return new ObservableCollection<Donor>(_donors.Where(x => (string.IsNullOrEmpty(last) || x.LastName.StartsWith(last, StringComparison.OrdinalIgnoreCase)) && (string.IsNullOrEmpty(first) || x.FirstName.StartsWith(first, StringComparison.OrdinalIgnoreCase))));
#pragma warning restore CS8604 // Possible null reference argument.
	}

	public Donor FindDonor(ref string? comment, ref int? partial, string? last, string? first, string? address, string? address2, string? city, string? state, string? zip)
	{
		// get list of Donors with same last name
		var ret = _donors!.Where(x => x.LastName == last);

		// Loop through subset with matching last name, looking for close matches.
		// If a close match is found, it is returned for examination by the operator.
		foreach (var donor in ret)
		{
			if (Helper.Compare(ref comment, ref partial, donor
				, last
				, first
				, address
				, false
				, address2
				, city
				, state
				, zip
				, Helper.ro_firstNameSplitters))
			{
				return donor;
			}
		}
#pragma warning disable CS8603 // Possible null reference return.
		return null;
#pragma warning restore CS8603 // Possible null reference return.
	}

	public Donor GetDonorById(int id)
	{
		var list = _donors!.Where(x => x.Id == id);
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
	public async Task<Donor> GetDonorByIdAsync(int id)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		var list = _donors!.Where(x => x.Id == id);
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
	public async Task<ObservableCollection<Donor>> GetDonorsByFamilyId(int id)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return new ObservableCollection<Donor>(_donors!.Where(x => x.FamilyId == id));
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<int> GetNextFamilyId()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8604 // Possible null reference argument.
		var ret = _donors.Any() ? _donors!.Select(x => x.FamilyId).Max() : 0;
#pragma warning restore CS8604 // Possible null reference argument.
		return null != ret ? ret.Value : 0;
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<ObservableCollection<Donor>> LoadDonors()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
#pragma warning disable CS8603 // Possible null reference return.
		return _donors;
#pragma warning restore CS8603 // Possible null reference return.
	}

	public void ReplaceDonorData(ObservableCollection<Donor> donorList)
	{
		_donors = new ObservableCollection<Donor>(donorList);
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<string?> SaveDonors(ObservableCollection<Donor> donors, bool force = false, Action<long, long>? progUpdate = null)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		return null;
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task UpdateDonor(Donor donor)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		for (int i = 0; i < _donors.Count; i++)
		{
			if (_donors[i].Id == donor.Id)
			{
				_donors[i] = donor;
				break;
			}
		}
	}
}
