namespace Donations.Lib.Model;

public class PrintSettings
{
	public int PrintoutType { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public string FontFamily { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public double FontSize { get; set; }
	public double LeftMargin { get; set; }
	public double OtherMargins { get; set; }
}
