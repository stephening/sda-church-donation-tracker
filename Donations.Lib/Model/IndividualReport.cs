namespace Donations.Lib.Model;

public class IndividualReport
{
	public int Id { get; set; }
	public string TemplateText { get; set; }
	public string EmailSubject { get; set; }
	public string EmailBody { get; set; }
	public bool Encrypt { get; set; }
}
