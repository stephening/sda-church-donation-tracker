namespace Donations.Model
{
	/// <summary>
	/// This object is stored in the database as a design of your church's specific tithe envelope options.
	/// When entering donations collected at church, it is convenient if the entry form resembles the tithe
	/// envelope.
	/// </summary>
	public class EnvelopeEntry
	{
		// The Category property will show the combined category code and description.
		public string Category { get; set; } = "";
	}
}
