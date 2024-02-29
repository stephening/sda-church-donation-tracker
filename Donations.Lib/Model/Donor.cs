using System;

namespace Donations.Lib.Model;

/// <summary>
/// This Donor object has many more fields than are required for the accounting function of this software.
/// The reason they are there is because the donor database they were imported from was inteded to be used
/// for other purposes as well. The necessary fields are:
///  - FamilyId
///  - FamilyRelationship
///  - FirstName
///  - LastName
///  - Email - for sending year-end donor reports
///  - AddressType - an address must be specified as Mailing or Both to send a printed year-end report
///  - Phone - not absolutely necessary, although very useful to resolve issues with a donor
///  - GroupGiving - allows the program to send a single year-end report to the Primary donor in a family
///                  participating in GroupGiving
/// </summary>
public class Donor
{
	/// <summary>
	/// The Name property is not saved in the database but is simply used to display the donor name
	/// in a: LastName, FirstName format.
	/// </summary>
	public string? Name => (!string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(FirstName)) ? $"{LastName}, {FirstName}"
		: ((string.IsNullOrEmpty(LastName) && string.IsNullOrEmpty(FirstName)) ? ""
			: (!string.IsNullOrEmpty(LastName) ? LastName : FirstName));
	/// <summary>
	/// The Id property is not important to users, but is a key element in database record access. It is not assigned
	/// by the user and will be unique among all Donor records.
	/// </summary>
	public int Id { get; set; }
	/// <summary>
	/// The FamilyId property is a unique number, only assigned to multiple donors who are grouped as a family. This
	/// property along with the FamilyRelationship, and GroupGiving properties determine whether all members of a 
	/// family participating in GroupGiving wil receive a shared year-end donor report under the name of the person
	/// who has the FamilyRelationship = Private.
	/// </summary>
	public int? FamilyId { get; set; }
	/// <summary>
	/// The only critical use of the FamilyRelationship property is that when a FamilyId exists and GroupGiving is 
	/// True, then all donations in the family from people who have GroupGiving=True, will be grouped into a single
	/// year-end donor report under the name of the person who has Primary in their FamilyRelationship field. The 
	/// enum allows many other family relationships to be specified, which may be useful for other applications
	/// sharing the donor database, but they are not applicable to the accounting software.
	/// </summary>
	public enumFamilyRelationship? FamilyRelationship { get; set; }
	/// <summary>
	/// The FirstName property is the given name of a donor. Since no middle name property exists, if a person
	/// wishes to include their middle name or initial, it will generally be included in this field, following the
	/// actual first name.
	/// </summary>
	public string? FirstName { get; set; }
	/// <summary>
	/// The PreferredName property is not used by the accounting software. For the purposes of other applications
	/// for the donor databse, this field could contain a nickname, or other name that the donor likes to go by.
	/// </summary>
	public string? PreferredName { get; set; }
	/// <summary>
	/// The LastName property will generally not be decorated unless there are two people with the same first and
	/// last name, living at the same address. In that case, the name might be decorated with a Jr, Sr, II, III
	/// to distinguish donors.
	/// </summary>
	public string? LastName { get; set; }
	/// <summary>
	/// The Gender property is not used at all by the accounting software.
	/// </summary>
	public enumGender? Gender { get; set; }
	/// <summary>
	/// The Email property is useful as a means of contacting the donor if there is some issue to resolve with
	/// their donation. The Email address might also be used to send electronic copies of the year-end donor
	/// report.
	/// </summary>
	public string? Email { get; set; }
	/// <summary>
	/// The Email2 property is only used to store a backup or secondary Email address if the donor wishes to
	/// supply one.
	/// </summary>
	public string? Email2 { get; set; }
	/// <summary>
	/// The HomePhone property used to be used for land line phone numbers. These are slowly becoming less and
	/// common as people are simply using a mobile phone.
	/// </summary>
	public string? HomePhone { get; set; }
	/// <summary>
	/// The MobilePhone property will probably be the most commonly used field now.
	/// </summary>
	public string? MobilePhone { get; set; }
	/// <summary>
	/// The WorkPhone property will probably be the least used field of the three. It is simply another way to
	/// try to reach someone if the other options fail.
	/// </summary>
	public string? WorkPhone { get; set; }
	/// <summary>
	/// The AddressType property is important if a year-end donor report need to be mailed out in physical form.
	/// The software will choose the Mailing or Both (Mailing and Residential) address to print and address the
	/// report to.
	/// </summary>
	public enumAddressType AddressType { get; set; } = enumAddressType.Both;
	/// <summary>
	/// The Address property will typically be a street address, but if a business name or other is needed, that
	/// can go in the Address property and the street address can be placed in the Address2 property.
	/// </summary>
	public string? Address { get; set; }
	/// <summary>
	/// The Address2 property will generally not be used but can be used together with the Address property as
	/// described above.
	/// </summary>
	public string? Address2 { get; set; }
	/// <summary>
	/// The City property is one of the standard address fields.
	/// </summary>
	public string? City { get; set; }
	/// <summary>
	/// The State property is one of the standard address fields and should generally be specified as a two
	/// letter abbreviation.
	/// </summary>
	public string? State { get; set; }
	/// <summary>
	/// The Zip property is the zip code and can be entered as a 5 digit number or it can include the -xxxx
	/// as well. When comparing zip codes for matching Adventist Giving donors with donors in the local database,
	/// the -xxxx will not be used in the comparison.
	/// </summary>
	public string? Zip { get; set; }
	/// <summary>
	/// The Country property is really not needed unless this application is used internationally and if it is,
	/// there are probably lots of other things that will need to be modified.
	/// </summary>
	public string? Country { get; set; }
	/// <summary>
	/// The Alt set of properties mirror the primary address fields. The Alt address will generally not be used
	/// but if it is, the AddressType and AltAddress type must not both specify Mailing or Both. If both are used
	/// the primary addres will generally be designated Mailing (where the donor report should be sent), and the
	/// Alt address might be designated Residential.
	/// </summary>
	public enumAddressType? AltAddressType { get; set; }
	public string? AltAddress { get; set; }
	public string? AltAddress2 { get; set; }
	public string? AltCity { get; set; }
	public string? AltState { get; set; }
	public string? AltZip { get; set; }
	public string? AltCountry { get; set; }
	private DateOnly _birthday;
	/// <summary>
	/// The Birthday property is not relevant to the accounting software but can be used in other applications
	/// sharing the donor database.
	/// </summary>
	public string? Birthday
	{
		get
		{
			string str = _birthday.ToString("yyyy/MM/dd");
			return str.Equals("0001/01/01") ? null : str;
		}
		set
		{
			try
			{
				if (string.IsNullOrEmpty(value))
					_birthday = DateOnly.MinValue;
				else
					_birthday = DateOnly.Parse(value);

				if (_birthday.ToString("yyyy/MM/dd") == "1900/01/01")
					_birthday = DateOnly.MinValue;
			}
			catch
			{
				_birthday = DateOnly.MinValue;
			}
		}
	}
	private DateOnly _baptism;
	/// <summary>
	/// The Baptism property is not relevant to the accounting software but can be used in other applications
	/// sharing the donor database.
	/// </summary>
	public string? Baptism
	{
		get
		{
			string str = _baptism.ToString("yyyy/MM/dd");
			return str.Equals("0001/01/01") ? null : str;
		}
		set
		{
			try
			{
				if (string.IsNullOrEmpty(value))
					_baptism = DateOnly.MinValue;
				else
					_baptism = DateOnly.Parse(value);

				if (_baptism.ToString("yyyy/MM/dd") == "1900/01/01")
					_baptism = DateOnly.MinValue;
			}
			catch
			{
				_baptism = DateOnly.MinValue;
			}
		}
	}
	private DateOnly _deathday;
	/// <summary>
	/// The Deathday property is not relevant to the accounting software but can be used in other applications
	/// sharing the donor database.
	/// </summary>
	public string? Deathday
	{
		get
		{
			string str = _deathday.ToString("yyyy/MM/dd");
			return str.Equals("0001/01/01") ? null : str;
		}
		set
		{
			try
			{
				if (string.IsNullOrEmpty(value))
					_deathday = DateOnly.MinValue;
				else
					_deathday = DateOnly.Parse(value);

				if (_deathday.ToString("yyyy/MM/dd") == "1900/01/01")
					_deathday = DateOnly.MinValue;
			}
			catch
			{
				_deathday = DateOnly.MinValue;
			}
		}
	}
	/// <summary>
	/// The GroupGiving property was mentioned above when describing the FamilyId and FamilyRelationship fields.
	/// If GroupGiving is True, all donations by family members who also have GroupGiving True, will be included
	/// in the year-end donor report of the family member designated with the FamilyRelationship = Primary.
	/// </summary>
	public bool? GroupGiving { get; set; }
	/// <summary>
	/// The ChurchMember property is a True/False (boolean) field, which is also not used by the accounting
	/// software, but may be used by other applications sharing the donor database.
	/// </summary>
	public bool? ChurchMember { get; set; }
	/// <summary>
	/// The Directory property is a True/False (boolean) field, which indicates whether the person should be
	/// included in the member directory.
	/// </summary>
	public bool? Directory { get; set; }
	/// <summary>
	/// The DontEmailReport property is a True/False (boolean) field, which indicates that even if an email
	/// address is present, do not send the year end report by email.
	/// </summary>
	public bool? DontEmailReport { get; set; }
	/// <summary>
	/// The Deceased property is a True/False (boolean) field, indicates that the person is deceased.
	/// </summary>
	public bool? Deceased { get; set; }
	/// <summary>
	/// The MaritalStatus property is a field which can take whatever values are defined in the enumMaritalStatus.
	/// If this information is not available when importing donor data, it will default to Unknown. This property
	/// is not used by the accounting software, but may be used by other applications sharing the donor database.
	/// </summary>
	public enumMaritalStatus? MaritalStatus { get; set; }
	/// <summary>
	/// The Note property is not used by the accounting software but may be used by other applications sharing
	/// the donor database.
	/// </summary>
	public string? Notes { get; set; }
	/// <summary>
	/// The ActiveGroups property is a carryover from a previous database and is not used by the accounting 
	/// software but may be used by other applications sharing the donor database.
	/// </summary>
	public string? ActiveGroups { get; set; }

	/// <summary>
	/// The PictureFile will be the filename of a picture on an http server. The base url will be supplied in
	/// settings and the filename will be appended to for the complete URL. 
	/// </summary>
	public string? PictureFile { get; set; }

	/// <summary>
	/// This Date property is specified by when the batch is submitted.
	/// </summary>
	public DateTime? LastUpdated { get; set; }
}
