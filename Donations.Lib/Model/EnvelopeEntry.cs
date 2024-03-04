using Donations.Lib.Interfaces;

namespace Donations.Lib.Model;

/// <summary>
/// This object is stored in the database as a design of your church's specific tithe envelope options.
/// When entering donations collected at church, it is convenient if the entry form resembles the tithe
/// envelope.
/// </summary>
public class EnvelopeEntry
{
	static private readonly ICategoryServices _categoryServices = DependencyInjection.CategoryServices;
	/// <summary>
	/// The Code property will contain the numeric value of the category code.
	/// </summary>
	public int Code { get; set; } = -1;
	/// <summary>
	/// The Category property will show the combined category code and description.
	/// </summary>
	public string Category => (-1 == Code || !_categoryServices.CatDict.ContainsKey(Code)) ? "" : $"{_categoryServices.CatDict[Code].Code} {_categoryServices.CatDict[Code].Description}";
	/// <summary>
	/// The TaxDeductible property will contain the TaxDeductible state of the category.
	/// </summary>
	public bool TaxDeductible => (-1 == Code || !_categoryServices.CatDict.ContainsKey(Code)) ? false : _categoryServices.CatDict[Code].TaxDeductible;
}
