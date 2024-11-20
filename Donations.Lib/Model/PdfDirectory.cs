namespace Donations.Lib.Model;

public class PdfDirectory
{
    public int Id { get; set; }
	public double PageWidth { get; set; } = 8.5;
	public double PageHeight { get; set; } = 11.0;
	public double LeftMargin { get; set; } = 0.8;
	public double OtherMargins { get; set; } = 0.5;
	public string Font { get; set; } = "Calibri";
	public double FontSize { get; set; } = 14;
	public bool IncludeAddress { get; set; } = true;
	public bool IncludePhone { get; set; } = true;
	public bool IncludeEmail { get; set; } = true;
	public bool IncludeNonMembers { get; set; } = true;
	public byte[]? CoverRtf { get; set; }
}
