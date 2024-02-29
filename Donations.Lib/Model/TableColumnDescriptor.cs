using System.Windows;

namespace Donations.Lib.Model;

public class TableColumnDescriptor
{
	public string? ColumnHeader { get; set; }
	public TextAlignment Alignment { get; set; }
	public string? Format { get; set; }
}
