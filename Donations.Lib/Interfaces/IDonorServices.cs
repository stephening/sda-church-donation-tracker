using Donations.Lib.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces;

public interface IDonorServices
{
	Task<ObservableCollection<Donor>> LoadDonors();
	Task<string?> SaveDonors(ObservableCollection<Donor> donors, bool force = false, Action<long, long>? progUpdate = null);
	void ReplaceDonorData(ObservableCollection<Donor> donorList);
	Task<ObservableCollection<Donor>> FilterDonors(int count = 100, string last = "", string first = "");
	Donor FindDonor(ref string? comment, ref int? partial, string? last, string? first, string? address, string? address2, string? city, string? state, string? zip);
	Donor GetDonorById(int id);
	Task<Donor> GetDonorByIdAsync(int id);
	Task<ObservableCollection<Donor>> GetDonorsByFamilyId(int id);
	Task<int> GetNextFamilyId();
	Task<int> AddDonor(Donor donor);
	Task DeleteDonor(Donor donor);
	Task UpdateDonor(Donor donor);
}
