using Donations.Lib.Attributes;
using System.Windows.Media;

namespace Donations.Lib.Model;

/// <summary>
/// This class is used to map categories from the Adventist Giving (AG) csv to the local ctegory database if an exact match is not found.
/// When attempting to match the AG category to the local accounting software Id's, if there is no exact match, the operator will
/// provide a target which will then be stored in this object with the AG category Id as the Dictionary key. Once the mapping is 
/// made, the AG category Id can easily be mapped to a local category Id in all future imports.
/// 
/// The JsonIgnored properties are used for populating a DataGrid for maintenance of the mapping.
/// 
/// Referenced by: MainWindow, CategoryMapView, AGCategoryResolutionViewModel, CategoryMapViewModel, CategoryViewModel, AGDonationSummaryViewModel
/// </summary>
public class AGCategoryMapItem
{
	/// <summary>
	/// The AGCategoryCode is the field provided for the Category in the AG csv. As documented for AdventistGiving.CategorId:
	/// 
	/// This is an ID property which has several different sources. Since AG is much larger than the 
	/// local church, there may be categories from different levels of the church that are defined at those
	/// levels. The local church has no control over those Id's. Hoever, the local church does have the
	/// ability to enter in their own Categories for the donor to choose from. In that case, the Id's should
	/// match the local Category Id's. In any case, there may need to be a mapping table from AG Category Id's
	/// and local church Id's. This is handled by the program.
	/// </summary>
	public int AGCategoryCode { get; set; }
	/// <summary>
	/// The AGCategoryName is simply a word description that goes along with the AGCategoryCode. One additional
	/// comment to make about this field is that is only the English portion of the field supplied by the
	/// CategoryName field in the AG csv.
	/// </summary>
	public string? AGCategoryName { get; set; }
	/// <summary>
	/// The CategoryCode property is the local database category ID. From this ID, the local CategoryName can be
	/// obtained. See the readonly property CategoryDescription below.
	/// </summary>
	public int CategoryCode { get; set; }

	/// <summary>
	/// This CategoryDescription read only property provides the local Category description corresponding
	/// to the CategoryCode properties above. This property is not stored because it is always looked up by CategoryCode.
	/// </summary>
	[SqlIgnore]
	public string? CategoryDescription { get; set; }
	/// <summary>
	/// This color property is also not stored, because it is a read only field whose value is determined
	/// by whether the CategoryCode properties match. This is just for visual purposes.
	/// </summary>
	[SqlIgnore]
	public Brush CatColor => AGCategoryCode == CategoryCode ? Brushes.Transparent : Brushes.Yellow;
	/// <summary>
	/// This color property is also not stored, because it is a read only field whose value is determined
	/// by whether the Category name and description match. This is just for visual purposes.
	/// </summary>
	[SqlIgnore]
	public Brush DescColor => Helper.Equal(AGCategoryName, CategoryDescription) ? Brushes.Transparent : Brushes.Yellow;
}
