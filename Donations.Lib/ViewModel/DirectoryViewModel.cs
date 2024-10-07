using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Donations.Lib.ViewModel;

public partial class DirectoryViewModel : BaseViewModel
{
	private readonly IDonorServices _donorServices;
	private Dictionary<string, DirectoryData> _directoryEntries = new Dictionary<string, DirectoryData>();

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
	private double _progress = 0;

	public new async Task Loading()
	{
		Progress = 0;

		Cursor save = Mouse.OverrideCursor;
		Mouse.OverrideCursor = Cursors.Wait;

		Dictionary<int, bool> _donorDone = new Dictionary<int, bool>();

		var donors = await _donorServices.GetDonorsForDirectory();

		double total = donors.Count;

		double c = 0;
		foreach (var donor in donors)
		{
			Progress = 100 * c / total;
			c++;

			string familyName = "";
			string otherFamilyMembers = "";
			string address = "";
			string phones = "";
			string email = "";
			string picture = "";

			await Task.Yield();

			if (_donorDone.ContainsKey(donor.Id) && _donorDone[donor.Id])
			{
				continue;
			}

			if (null != donor.FamilyId && donor.FamilyRelationship != enumFamilyRelationship.None)
			{
				string wife = "";
				string husband = "";
				string primary = "";
				string lastName = "";

				var family = await _donorServices.GetDonorsByFamilyId(donor.FamilyId.Value);
				foreach (var member in family)
				{
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
				address = GetAddress(donor);
				phones = GetPhoneNumber("", donor);
				email = GetEmail("", donor);
				picture = donor.PictureFile;
			}

			var data = new DirectoryData()
			{
				Name = familyName,
				OtherFamilyMembers = otherFamilyMembers,
				Address = address,
				Phone = phones,
				Email = email,
				Picture = picture
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

		DirectoryPdfViewModel.SetDirectoryEntries(_directoryEntries);

		Mouse.OverrideCursor = save;
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