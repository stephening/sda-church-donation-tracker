using Donations.Lib.ViewModel;
using System.Windows.Data;
using System.Windows.Documents;

namespace Donations.Lib.Interfaces;

public interface IPrintPreview
{
	TableHelper TableHelper { get; }
	CollectionViewSource Source { get; }

	void CreatePreview(FlowDocument doc, string? font, double size, double printAreaWidth);
}
