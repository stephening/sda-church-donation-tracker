namespace Donations.Lib.Model;

/// <summary>
/// This object is not stored in the database but is created on the fly to generate a list of categories and totals.
/// This list can be used for online viewing or for creating a printed record.
/// 
/// Referenced by: BatchReviewView, BatchReviewViewModel, AGDonationSummaryViewModel, DonorContributionsViewModel
/// </summary>
public class CategorySum
{
	/// <summary>
	/// The Code is used if the code number needs to be separated out and displaying in its own column
	/// </summary>
	public int? Code { get; set; }
	/// <summary>
	/// The Description property will store just the descriptive part of the catefory stored in the Donation record.
	/// </summary>
	public string? Description { get; set; }
	/// <summary>
	/// The Category property will store the Category Code and Description.
	/// </summary>
	public string? Category { get; set; }
	/// <summary>
	/// This Sum property will contain the sum for the above specified Category for a given batch.
	/// </summary>
	public double Sum { get; set; }
}
