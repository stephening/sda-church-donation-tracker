namespace Donations.Lib.Model;

/// <summary>
/// This data object is used to store the data imported from an Adventist Giving (AG) *.csv.
/// The members DonorHash and DonorId are not imported from the csv, but are used for the purpose of matching the donor with the local donor database.
/// 
/// When the AG csv is imported, since it doesn't contain DonorId's matching our local database, 
/// we have to try to match the name and address from this record with the correct on in the local database.
/// Once this match is made, whether automatically or with operator assistance, the information is transferred to a local Donation record.
/// 
/// Referenced by: MainWindow, AdventistGivingViewModel, AGCategoryResolutionViewModel, AGDonorResolutionViewModel, AGDonationSummaryViewModel
/// </summary>
public class AdventistGiving
{
	/// <summary>
	/// DonorHash is a string concatenation of LastName + FirstName + Address + Zip.
	/// This is used as a key into a Dictionary record linking this donor with a local database donor (DonorId).
	/// </summary>
	public string? DonorHash { get; set; }
	/// <summary>
	/// DonorId is used to temporarily store the donor Id from the local database if an exact match is found.
	/// Then when the Donation record is created, the DonorId is saved there.
	/// </summary>
	public int? DonorId { get; set; }
	/// <summary>
	/// The FirstName property here may be different from the local donor database in this way. For example, if
	/// individuals from a family have donor records, you might have a John Doe and a Jane Doe records. But
	/// if John and Jane Doe only create one record for AG, they might put John & Jane for the first name
	/// field.
	/// </summary>
	public string? FirstName { get; set; }
	/// <summary>
	/// There is not much to say about the LastName property. It should have a match in the local donor database.
	/// </summary>
	public string? LastName { get; set; }
	/// <summary>
	/// The Address property should be the start of the physical address that FirstName LastName set up in their
	/// AG account.
	/// </summary>
	public string? Address { get; set; }
	/// <summary>
	/// On rare occasions, two lines may be needed for the address. For example, someone might put the name
	/// of a retirement home in Address, then the actual street address could go into Address2. If not needed
	/// of course, it should be left blank.
	/// </summary>
	public string? Address2 { get; set; }
	/// <summary>
	/// There is really nothing to be said about the City property.
	/// </summary>
	public string? City { get; set; }
	/// <summary>
	/// There is nothing to be said about the State property either.
	/// </summary>
	public string? State { get; set; }
	/// <summary>
	/// The only thing to say about the zip property is that, when trying to match an AG donor to the local donor
	/// database, only the first 5 characters of the zip are compared. The reason is that just because one
	/// zip code is 12345-7890 and the other is 12345 doesn't mean that they are different.
	/// </summary>
	public string? Zip { get; set; }
	/// <summary>
	/// There is not much to say about the Country property except to say that when comparing name/address with
	/// the local database to find the matching donor Id, the country is not compared.
	/// </summary>
	public string? Country { get; set; }
	/// <summary>
	/// The TransactionId property is a unique number assigned to the entire transaction by one donor, which can
	/// include multiple lines (multiple categories).
	/// </summary>
	public string? TransactionId { get; set; }
	/// <summary>
	/// The TransactionType simply contains something like "credit" or whatever other types of transactions are 
	/// supported. This program doesn't really care, and infact, doesn't even use this field.
	/// </summary>
	public string? TransactionType { get; set; }
	/// <summary>
	/// The TransactionDate is the date that the donor submitted their donation to AG.
	/// </summary>
	public string? TransactionDate { get; set; }
	/// <summary>
	/// The TransactionTotal is the total amount donated, as oppsed the the Amount property below, which should
	/// all add up the to Total for a given TransactionId.
	/// </summary>
	public double TransactionTotal { get; set; }
	/// <summary>
	/// The CategoryCode is an ID property which has several different sources. Since AG is much larger than the 
	/// local church, there may be categories from different levels of the church that are defined at those
	/// levels. The local church has no control over those Id's. Hoever, the local church does have the
	/// ability to enter in their own Categories for the donor to choose from. In that case, the Id's should
	/// match the local Category Id's. In any case, there may need to be a mapping table from AG Category Id's
	/// and local church Id's. This is handled by the program.
	/// </summary>
	public int CategoryCode { get; set; }
	/// <summary>
	/// The CategoryName is simply a word description that goes along with the CategoryCode.
	/// </summary>
	public string? CategoryName { get; set; }
	/// <summary>
	/// The Amount property is a dollar amount designated for the specified Category. The Amount's for a given
	/// TransactionId, and donor, should all add up to the TransactionTotal field.
	/// </summary>
	public double Amount { get; set; }
	/// <summary>
	/// The SplitCategoryName property is a read only property that split's the CategoryName by the " / "
	/// delimiter and then just returns the first part [0] (the English description).
	/// </summary>
	public string SplitCategoryName => string.IsNullOrEmpty(CategoryName) ? ""
		: (CategoryName.StartsWith("Tithe") ? CategoryName.Split("/")[0]
		: CategoryName.Split(" / ")[0]);
}
