using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for WizardIntroduction.xaml
/// </summary>
public partial class WizardIntroductionView : UserControl
{
	public WizardIntroductionView()
	{
		InitializeComponent();
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
		e.Handled = true;
	}
}
