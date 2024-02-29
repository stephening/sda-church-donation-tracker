namespace Donations.Lib.Model;

public class AppSettings
{
	public int Id { get; set; }
	public string? SyncFusionLicenseKey { get; set; }
	public string? PictureBaseUrl { get; set; }
	public string? EmailSmtpServer { get; set; } = "smtp.gmail.com";
	public int? EmailServerPort { get; set; } = 587;
	public bool EmailEnableSsl { get; set; } = true;
	public string? EmailAccount { get; set; }
}
