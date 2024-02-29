namespace Donations.Lib.Model;

/// <summary>
/// The Category is a concept necessary for the designation of funds. When money is given, is must always be
/// allocated to a specific fund. Usually the donor will specify the category. Categories are identified by
/// an Id and a word description. Generally, just the Id and not the word description would be stored in 
/// other database records because you would think that the description could always be looked up by Id.
/// However, in this case, for legacy reasons, the Id and Description will both be stored in Donation
/// records. This seems a little redundant, but what it allows, is for the current category table to be 
/// kept trimmed to the current or active categories. Then even if a category has been decomissioned,
/// older donation records will still have the description available.
/// 
/// Referenced by: MainWindow, AGCategoryResolutionView, CategoryMapView, CateogryView, DonorInputView,
///				   EnvelopeDesignView, AGCategoryResolutionViewModel, CategoryMapViewModel,
///				   CategorySelectionViewModel, CategoryViewModel, AGDonationSummaryViewModel,
///				   DonorInputViewModel, ImportCategoriesViewModel
///				   
/// </summary>
public class Category
{
	/// <summary>
	/// This Code property is number used in the accounting, associated with a word description contained
	/// in the Description property below.
	/// </summary>
	public int Code { get; set; }
	/// <summary>
	/// The Description property is the word description of the category code in the property above.
	/// </summary>
	public string Description { get; set; } = "";
	/// <summary>
	/// The TaxDeductible property is a True/False, boolean property, where True indicates that donations
	/// to the category or fund are not tax deductible.
	/// </summary>
	public bool TaxDeductible { get; set; }
}
