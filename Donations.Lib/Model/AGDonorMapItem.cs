using Donations.Lib.Attributes;
using System.Windows.Media;

namespace Donations.Lib.Model;

/// <summary>
/// This class is used to map donors from the Adventist Giving (AG) csv to the local donor database if an exact match is not found.
/// When attempting to match the AG donor to the local accounting software donor Id's, if there is no exact match, the operator will
/// provide a target which will then be stored in this object with the AG donor hash (AGDonorHash) as the Dictionary key. Once the mapping is 
/// made, the AGDonorHash can easily be mapped to a local donor Id in all future imports.
/// 
/// The SqlIgnored properties are used for populating a DataGrid for maintenance of the mapping.
/// 
/// Refrenced by: MainWindow, DonorMapView, AGDonorResolutionViewModel, AGDonationSummaryViewModel, DonorMapViewModel
/// </summary>
public class AGDonorMapItem
{
	/// <summary>
	/// The AGLastName property is the LastName property provide by the AdventistGiving record.
	/// </summary>
	public string? AGLastName { get; set; }
	/// <summary>
	/// The AGFirstName property is the FirstName property provide by the AdventistGiving record.
	/// </summary>
	public string? AGFirstName { get; set; }
	/// <summary>
	/// The AGAddress property is the Address property provide by the AdventistGiving record.
	/// </summary>
	public string? AGAddress { get; set; }
	/// <summary>
	/// The AGCity property is the City property provide by the AdventistGiving record.
	/// </summary>
	public string? AGCity { get; set; }
	/// <summary>
	/// The AGState property is the State property provide by the AdventistGiving record.
	/// </summary>
	public string? AGState { get; set; }
	/// <summary>
	/// The AGZip property is the Zip code property provide by the AdventistGiving record.
	/// </summary>
	public string? AGZip { get; set; }
	/// <summary>
	/// The AGDonorHash property is the hash created for the AdventistGiving.DonorHash 
	/// field.
	/// </summary>
	public string? AGDonorHash { get; set; }
	/// <summary>
	/// The DonorId property is the local donor Id that was determined to be a match for
	/// the AG donor. Ultimately it needs to be uniquely matched with the AGDonorHash.
	/// </summary>
	public int DonorId { get; set; }
	/// <summary>
	/// The LastName property is not stored in the database, and is obtained from a lookup
	/// by DonorId in the local databse. The property is provided for visual confirmation
	/// in the table provided for editing AG to local donor mapping.
	/// </summary>
	[SqlIgnore]
	public string? LastName { get; set; }
	/// The FirstName property is not stored in the database, and is obtained from a lookup
	/// by DonorId in the local databse. The property is provided for visual confirmation
	/// in the table provided for editing AG to local donor mapping.
	[SqlIgnore]
	public string? FirstName { get; set; }
	/// The Address property is not stored in the database, and is obtained from a lookup
	/// by DonorId in the local databse. The property is provided for visual confirmation
	/// in the table provided for editing AG to local donor mapping.
	[SqlIgnore]
	public string? Address { get; set; }
	/// The City property is not stored in the database, and is obtained from a lookup
	/// by DonorId in the local databse. The property is provided for visual confirmation
	/// in the table provided for editing AG to local donor mapping.
	[SqlIgnore]
	public string? City { get; set; }
	/// The State property is not stored in the database, and is obtained from a lookup
	/// by DonorId in the local databse. The property is provided for visual confirmation
	/// in the table provided for editing AG to local donor mapping.
	[SqlIgnore]
	public string? State { get; set; }
	/// The Zip property is not stored in the database, and is obtained from a lookup
	/// by DonorId in the local databse. The property is provided for visual confirmation
	/// in the table provided for editing AG to local donor mapping.
	[SqlIgnore]
	public string? Zip { get; set; }
	/// This color property is also not stored, because it is a read only field whose value is determined
	/// by whether the LastName properties match. This is just for visual purposes.
	[SqlIgnore]
	public Brush LastColor => Helper.Equal(AGLastName, LastName) ? Brushes.White : Brushes.Yellow;
	/// This color property is also not stored, because it is a read only field whose value is determined
	/// by whether the FirstName properties match. This is just for visual purposes.
	[SqlIgnore]
	public Brush FirstColor => Helper.Equal(AGFirstName, FirstName) ? Brushes.White : Brushes.Yellow;
	/// This color property is also not stored, because it is a read only field whose value is determined
	/// by whether the Address properties match. This is just for visual purposes.
	[SqlIgnore]
	public Brush AddressColor => Helper.Equal(AGAddress, Address) ? Brushes.White : Brushes.Yellow;
	/// This color property is also not stored, because it is a read only field whose value is determined
	/// by whether the City properties match. This is just for visual purposes.
	[SqlIgnore]
	public Brush CityColor => Helper.Equal(AGCity, City) ? Brushes.White : Brushes.Yellow;
	/// This color property is also not stored, because it is a read only field whose value is determined
	/// by whether the State properties match. This is just for visual purposes.
	[SqlIgnore]
	public Brush StateColor => Helper.Equal(AGState, State, eFlags.State) ? Brushes.White : Brushes.Yellow;
	/// This color property is also not stored, because it is a read only field whose value is determined
	/// by whether the Zip code properties match. This is just for visual purposes.
	[SqlIgnore]
	public Brush ZipColor => Helper.Equal(AGZip, Zip, eFlags.Length, len: 5) ? Brushes.White : Brushes.Yellow;

	public void RefreshDonorFields(Donor donor)
	{
		LastName = donor.LastName;
		FirstName = donor.FirstName;
		Address = donor.Address;
		City = donor.City;
		State = donor.State;
		Zip = donor.Zip;
	}
}
