using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for WizardSqlCloudInstallView.xaml
/// </summary>
public partial class WizardSqlCloudInstallView : UserControl
{
	public WizardSqlCloudInstallView()
	{
		InitializeComponent();
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
		e.Handled = true;
	}
}
