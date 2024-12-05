namespace Donations.Lib.Model;

public class HtmlDirectory
{
	public int Id { get; set; }
	public bool IncludeNonMembers { get; set; } = true;
	public bool OrderByLast { get; set; } = true;
	public bool OrderByFirst { get; set; } = true;
	public string OrderByFirstFilename { get; set; } = "FirstNamesPhotoDirectory.html";
	public string OrderByLastFilename { get; set; } = "LastNamesPhotoDirectory.html";
	public string OutputFolder { get; set; } = "";
	public string PicturePath { get; set; } = "./pictures/";
	public string Header { get; set; } = "";
	public string Template { get; set; } = "";
	public string Footer { get; set; } = "";

}
