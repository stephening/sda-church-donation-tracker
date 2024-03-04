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

	public async Task<int> AddDonor(Donor donor)
	{
		var newId = ((0 == _donors!.Count)
								? 0
								: _donors!.Select(x => x.Id).Max())
								+ 1;
		donor.Id = newId;
		_donors!.Add(donor);

		return donor.Id;
	}

	public async Task DeleteDonor(Donor donor)
	{
		_donors.Remove(donor);
	}

	public async Task<ObservableCollection<Donor>> FilterDonors(int count = 100, string last = "", string first = "")
	{
		return new ObservableCollection<Donor>(_donors.Where(x => (string.IsNullOrEmpty(last) || x.LastName.StartsWith(last, StringComparison.OrdinalIgnoreCase)) && (string.IsNullOrEmpty(first) || x.FirstName.StartsWith(first, StringComparison.OrdinalIgnoreCase))));
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
		return null;
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

	public async Task<Donor> GetDonorByIdAsync(int id)
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

	public async Task<ObservableCollection<Donor>> GetDonorsByFamilyId(int id)
	{
		return new ObservableCollection<Donor>(_donors!.Where(x => x.FamilyId == id));
	}

	public async Task<int> GetNextFamilyId()
	{
		var ret = _donors.Any() ? _donors!.Select(x => x.FamilyId).Max() : 0;
		return null != ret ? ret.Value : 0;
	}

	public async Task<ObservableCollection<Donor>> LoadDonors()
	{
		return _donors;
	}

	public void ReplaceDonorData(ObservableCollection<Donor> donorList)
	{
		_donors = new ObservableCollection<Donor>(donorList);
	}

	public async Task<string?> SaveDonors(ObservableCollection<Donor> donors, bool force = false, Action<long, long>? progUpdate = null)
	{
		return null;
	}

	public async Task UpdateDonor(Donor donor)
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
