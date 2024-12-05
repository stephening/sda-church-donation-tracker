using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Donations.Lib.ViewModel;

public partial class DirectoryViewModel : BaseViewModel
{
	private readonly IDonorServices _donorServices;
	private Dictionary<string, DirectoryData> _directoryEntries = new Dictionary<string, DirectoryData>();
	private bool _cancelLoading = false;
	private Semaphore _loading = new Semaphore(1, 1);

	public DirectoryViewModel(
		IDonorServices donorServices,
		DirectoryHtmlViewModel directoryHtmlViewModel,
		DirectoryPdfViewModel directoryPdfViewModel
	)
	{
		_donorServices = donorServices;
		DirectoryHtmlViewModel = directoryHtmlViewModel;
		DirectoryPdfViewModel = directoryPdfViewModel;
	}

	public DirectoryHtmlViewModel DirectoryHtmlViewModel { get; }
	public DirectoryPdfViewModel DirectoryPdfViewModel { get; }

	[ObservableProperty]
	private string _status = "";

	[ObservableProperty]
	private double _progress = 0;

	public new async Task Leaving()
	{
		await DirectoryPdfViewModel.Leaving();
		await DirectoryHtmlViewModel.Leaving();
	}

	public new async Task Loading()
	{
		// we don't want multiple instances of Loading() running simultaneously,
		// so try canceling if one is running and then take the resource
		_cancelLoading = true;
		await Task.Run(() => _loading.WaitOne());
		_cancelLoading = false;

		// if directory rendering is in progress, cancel it
		DirectoryPdfViewModel.Cancel();

		_directoryEntries.Clear();
		Progress = 0;

		Dictionary<int, bool> _donorDone = new Dictionary<int, bool>();

		var donors = await _donorServices.GetDonorsForDirectory();

		double total = donors.Count;

		double c = 0;
		Status = "Querying database for directory entries";

		foreach (var donor in donors)
		{
			if (_cancelLoading)
			{
				_cancelLoading = false;

				_loading.Release();
				return;
			}
			Progress = 100 * c / total;
			c++;

			string familyName = "";
			string otherFamilyMembers = "";
			string address = "";
			string phones = "";
			string email = "";
			string picture = "";
			bool churchMember = false;

			await Task.Yield();

			if (_donorDone.ContainsKey(donor.Id) && _donorDone[donor.Id])
			{
				continue;
			}

			string wife = "";
			string husband = "";
			string primary = "";
			string lastName = "";
			string firstName = donor.FirstName;

			if (null != donor.FamilyId && donor.FamilyRelationship != enumFamilyRelationship.None)
			{
				var family = await _donorServices.GetDonorsByFamilyId(donor.FamilyId.Value);
				foreach (var member in family)
				{
					if (_cancelLoading)
					{
						_cancelLoading = false;

						_loading.Release();
						return;
					}


					if (true == member.ChurchMember)
					{
						churchMember = true;
					}

					_donorDone[member.Id] = true;
					if (member.FamilyRelationship == enumFamilyRelationship.Husband)
					{
						husband = member.FirstName;
						if (string.IsNullOrEmpty(lastName))
						{
							lastName = member.LastName;
							address = GetAddress(member);
						}

						phones = GetPhoneNumber(phones, member);
						email = GetEmail(email, member);
						if (string.IsNullOrEmpty(picture))
						{
							picture = member.PictureFile;
						}
					}
					else if (member.FamilyRelationship == enumFamilyRelationship.Wife)
					{
						wife = member.FirstName;
						if (string.IsNullOrEmpty(lastName))
						{
							lastName = member.LastName;
							address = GetAddress(member);
						}
						phones = GetPhoneNumber(phones, member);
						email = GetEmail(email, member);
						if (string.IsNullOrEmpty(picture))
						{
							picture = member.PictureFile;
						}
					}
					else if (member.FamilyRelationship == enumFamilyRelationship.Primary)
					{
						primary = member.FirstName;
						lastName = member.LastName;
						address = GetAddress(member);
						phones = GetPhoneNumber(phones, member);
						email = GetEmail(email, member);
						picture = member.PictureFile;
					}
					else
					{
						if (string.IsNullOrEmpty(otherFamilyMembers))
						{
							otherFamilyMembers = member.FirstName;
						}
						else
						{
							otherFamilyMembers = otherFamilyMembers + ", " + member.FirstName;
						}
						if (string.IsNullOrEmpty(picture))
						{
							picture = member.PictureFile;
						}
					}
				}

				if (!string.IsNullOrEmpty(lastName))
				{
					familyName = lastName;
				}
				if (!string.IsNullOrEmpty(familyName))
				{
					familyName += ", ";
				}
				if (!string.IsNullOrEmpty(primary))
				{
					familyName += primary;
				}
				if (!string.IsNullOrEmpty(husband))
				{
					if (!string.IsNullOrEmpty(primary))
					{
						familyName += " & " + husband;
					}
					else
					{
						familyName += husband;
					}
				}
				if (!string.IsNullOrEmpty(wife))
				{
					familyName += " & " + wife;
				}
			}
			else
			{
				familyName = donor.Name;
				lastName = donor.LastName;
				address = GetAddress(donor);
				phones = GetPhoneNumber("", donor);
				email = GetEmail("", donor);
				picture = donor.PictureFile;
			}

			var data = new DirectoryData()
			{
				LastName = lastName,
				FirstName = firstName,
				Name = familyName,
				OtherFamilyMembers = otherFamilyMembers,
				Address = address,
				Phone = phones,
				Email = email,
				Picture = picture,
				Member = churchMember
			};

			if (familyName.Equals(""))
			{

			}

			if (_directoryEntries.ContainsKey(familyName))
			{
				for (int i = 0; ; i++)
				{
					string newKey = $"{familyName}{i}";
					if (!_directoryEntries.ContainsKey(newKey))
					{
						_directoryEntries[newKey] = data;
					}
				}
			}
			else
			{
				_directoryEntries[familyName] = data;
			}
		}

		DirectoryHtmlViewModel.SetDirectoryEntries(_directoryEntries);
		DirectoryPdfViewModel.SetDirectoryEntries(_directoryEntries);

		Status = "Completed database query";

		_loading.Release();
	}

	private string GetAddress(Donor donor)
	{
		string address = "";

		if (!string.IsNullOrEmpty(donor.Address))
		{
			address += donor.Address;
		}
		if (!string.IsNullOrEmpty(donor.Address2))
		{
			address += "\r\n" + donor.Address2;
		}
		if (!string.IsNullOrEmpty(donor.City))
		{
			address += "\r\n" + donor.City;
		}
		if (!string.IsNullOrEmpty(donor.State))
		{
			address += ", " + donor.State;
		}
		if (!string.IsNullOrEmpty(donor.Zip))
		{
			address += "  " + donor.Zip;
		}

		return address;
	}

	private string GetPhoneNumber(string phoneNumber, Donor donor)
	{
		string number = "";

		if (!string.IsNullOrEmpty(donor.MobilePhone))
		{
			number = donor.MobilePhone;
		}
		else if (!string.IsNullOrEmpty(donor.HomePhone))
		{
			number = donor.MobilePhone;
		}
		if (!string.IsNullOrEmpty(donor.WorkPhone))
		{
			number = donor.MobilePhone;
		}

		if (!string.IsNullOrEmpty(number))
		{
			if (!string.IsNullOrEmpty(phoneNumber))
			{
				phoneNumber += ", ";
			}
			phoneNumber += donor.FirstName + ": " + number;
		}

		return phoneNumber;
	}

	private string GetEmail(string email, Donor donor)
	{
		string address = "";

		if (!string.IsNullOrEmpty(donor.Email))
		{
			address = donor.Email;
		}
		else if (!string.IsNullOrEmpty(donor.Email2))
		{
			address = donor.Email2;
		}

		if (!string.IsNullOrEmpty(address))
		{
			if (!string.IsNullOrEmpty(email))
			{
				email += ", ";
			}
			email += donor.FirstName + ": " + address;
		}

		return email;
	}
}