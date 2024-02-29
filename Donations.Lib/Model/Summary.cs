using System.Collections.ObjectModel;

namespace Donations.Lib.Model
{
	/// <summary>
	/// This object is not stored in the database but is used to show the donor totals in one or more summary views.
	/// </summary>
	public class Summary
	{
		/// <summary>
		/// The Name property is not saved in the database but is simply used to display the donor name
		/// in a: LastName, FirstName format.
		/// </summary>
		public string? Name => (!string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(FirstName)) ? $"{LastName}, {FirstName}"
			: ((string.IsNullOrEmpty(LastName) && string.IsNullOrEmpty(FirstName)) ? ""
				: (!string.IsNullOrEmpty(LastName) ? LastName : FirstName));
		/// <summary>
		/// The DonorId property links the record back to a donor in the database
		/// </summary>
		public int DonorId { get; set; }
		/// <summary>
		/// The EnvelopeId property is only needed to distinguish different tithe enveloped by the same person in the same week.
		/// </summary>
		public int EnvelopeId { get; set; }
		/// <summary>
		/// The LastName property comes from either the Donor Name property or the LastName property of the Donation
		/// record if there is no Donor record corresponding the the DonorId.
		/// </summary>
		public string? LastName { get; set; }
		/// <summary>
		/// The LastName property comes from either the Donor Name property or the LastName property of the Donation
		/// record if there is no Donor record corresponding the the DonorId.
		/// </summary>
		public string? FirstName { get; set; }
		/// <summary>
		/// The Subtotal property is a total amount donated by the donor in a batch, but it is a subtotal of the full
		/// amount recorded for the batch.
		/// </summary>
		public double Subtotal { get; set; }
		/// <summary>
		/// The Method property allows the software to track how the donation was made, whether it be Cash, Check, etc...
		/// </summary>
		public enumMethod Method { get; set; }
		/// <summary>
		/// The TransactionNumber property is used to track check numbers, Adventist Giving TransactionId's, etc...
		/// </summary>
		public string? TransactionNumber { get; set; }
		/// <summary>
		/// The ItemizedDonations property is a list of the individual categorized donations that make up the Subtotal amount. 
		/// </summary>
		public ObservableCollection<Donation>? ItemizedDonations { get; set; }
	}
}
