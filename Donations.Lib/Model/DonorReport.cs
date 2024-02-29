using System;

namespace Donations.Lib.Model;

public class DonorReport
{
	public int DonorId { get; set; }
	/// <summary>
	/// The LastSent property is used to track if and when a donor report was sent.
	/// </summary>
	public DateTime? LastSent { get; set; }

	public string? Action { get; set; }
}
