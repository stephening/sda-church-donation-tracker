using System;
using System.Windows.Media;

namespace Donations.Lib.Model;

/// <summary>
/// The Donation record is a key object, storing and linking batch's, donor's, and subtotals for the accounting software.
/// The last three fields: Note, Method, TransactionNumber may be useful for record keeping and followup, but are not 
/// critical for the accounting functionality.
/// </summary>
public class Donation
{
	static SolidColorBrush _white = new SolidColorBrush(Colors.White);
	static SolidColorBrush _black = new SolidColorBrush(Colors.Black);

	/// <summary>
	/// The Name property is not saved in the database but is simply used to display the donor name
	/// in a: LastName, FirstName format.
	/// </summary>
	public string? Name => (!string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(FirstName)) ? $"{LastName}, {FirstName}"
		: ((string.IsNullOrEmpty(LastName) && string.IsNullOrEmpty(FirstName)) ? ""
			: (!string.IsNullOrEmpty(LastName) ? LastName : FirstName));
	/// <summary>
	/// The Id property is not important to users, but is a key element in database record access. It is not assigned
	/// by the user and will be unique among all Donation records.
	/// </summary>
	public int Id { get; set; }
	/// <summary>
	/// The DonorId property links the donation to a particular donor in the donor database. If for some reason the 
	/// donor database has been pruned and the donor Id no longer exists, the Donation record at least contains the
	/// first and last name of the donor.
	/// </summary>
	public int DonorId { get; set; }
	/// <summary>
	/// The BatchId property links to a Batch database record. Multiple Donation records can be linked to a single
	/// Batch record. When a batch is viewed or edited, all Donation records containing the BatchId will be involved.
	/// </summary>
	public int BatchId { get; set; }
	/// <summary>
	/// The EnvelopeId property is only needed to distinguish different tithe enveloped by the same person in the same week.
	/// </summary>
	public int? EnvelopeId { get; set; }
	/// <summary>
	/// The LastName property is only a backup donor indentification if the donor database has been pruned. This
	/// should never really be a problem, since the maintainers of the database should not remove donors unless they
	/// haven't donated in many years.
	/// </summary>
	public string? LastName { get; set; }
	/// <summary>
	/// The FirstName property is only a backup donor indentification if the donor database has been pruned. This
	/// should never really be a problem, since the maintainers of the database should not remove donors unless they
	/// haven't donated in many years.
	/// </summary>
	public string? FirstName { get; set; }
	/// <summary>
	/// The Category property will contain the category code and description. As mentioned elsewhere, the description
	/// is stored here as well because after many years, a category may be retired from the system, but the name will
	/// still be retained in the Donation record.
	/// </summary>
	public string? Category { get; set; }
	/// <summary>
	/// The Value property is the dollar amount designated to the category by the donor.
	/// </summary>
	public double Value { get; set; }
	private DateOnly _date;
	/// <summary>
	/// The Date property contains the date the donation was made. A disclaimer is that for donations entered weekly
	/// by manual entry, it is not possible to specify a date for each donor. A single date is assigned to all
	/// donations entered in a batch. On the other hand, when importing from an Adventist Giving csv, the dates for
	/// each donation are retained and stored in this property.
	/// </summary>
	public string? Date
	{
		get
		{
			string str = _date.ToString("yyyy/MM/dd");
			return str.Equals("0001/01/01") ? "" : str;
		}
		set
		{
			try
			{
				if (string.IsNullOrEmpty(value))
					_date = DateOnly.MinValue;
				else
					_date = DateOnly.Parse(value);
			}
			catch
			{
				_date = DateOnly.MinValue;
			}
		}
	}
	/// <summary>
	/// The TaxDeductible property is a True/False (boolean) field, where True means the category being donated to
	/// is tax deductible. Donations to non-deductible categories will not be included in the year-end reports sent
	/// to donors.
	/// </summary>
	public bool TaxDeductible { get; set; }
	/// <summary>
	/// The Note property is not used for Adventist Giving imports, but for manual donor entry, the operator can
	/// type a note along with each line item on the tithe envelope. Usually the Note field will be left blank.
	/// </summary>
	public string? Note { get; set; }
	/// <summary>
	/// The Method property is used to store the method giving, such as Cash, Check, AdventistGiving, etc...
	/// (see enumMethod).
	/// </summary>
	public enumMethod Method { get; set; }
	/// <summary>
	/// The TransactionNumber property will not be used for Cash donations, but is used to store Check numbers
	/// if available, AdventistGiving TransactionId's, or even potentially the last 4 digits of the credit card
	/// number for Card donations. Other online giving methoeds may have their own transaction number that could
	/// be stored here as well. Ultimately, this field is useful for record keeping and followup with donors if
	/// necessary, but is not used for the accounting.
	/// </summary>
	public string? TransactionNumber { get; set; }
}
