using System;

namespace Donations.Lib.Model;

public class NamedDonorReport
{
	public int DonorId { get; set; }
	/// <summary>
	/// The Name property is not saved in the database but is simply used to display the donor name
	/// in a: LastName, FirstName format.
	/// </summary>
	public string? Name => (!string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(FirstName)) ? $"{LastName}, {FirstName}"
		: ((string.IsNullOrEmpty(LastName) && string.IsNullOrEmpty(FirstName)) ? ""
			: (!string.IsNullOrEmpty(LastName) ? LastName : FirstName));
	/// <summary>
	/// The FirstName property is the given name of a donor. Since no middle name property exists, if a person
	/// wishes to include their middle name or initial, it will generally be included in this field, following the
	/// actual first name.
	/// </summary>
	public string? FirstName { get; set; }
	/// <summary>
	/// The LastName property will generally not be decorated unless there are two people with the same first and
	/// last name, living at the same address. In that case, the name might be decorated with a Jr, Sr, II, III
	/// to distinguish donors.
	/// </summary>
	public string? LastName { get; set; }
	/// <summary>
	/// The Email property is useful as a means of contacting the donor if there is some issue to resolve with
	/// their donation. The Email address might also be used to send electronic copies of the year-end donor
	/// report.
	/// </summary>
	public string? Email { get; set; }
	/// <summary>
	/// The MobilePhone property will probably be the most commonly used field now.
	/// </summary>
	public string? MobilePhone { get; set; }
	/// <summary>
	/// The DontEmailReport property is a True/False (boolean) field, which indicates that even if an email
	/// address is present, do not send the year end report by email.
	/// </summary>
	public bool? DontEmailReport { get; set; }
	public DateTime? LastSent { get; set; }

	public string? Action { get; set; }

}
