using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for WizardMemberIntroductionView.xaml
/// </summary>
public partial class WizardMemberIntroductionView : UserControl
{
	public WizardMemberIntroductionView()
	{
		InitializeComponent();
	}

	private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
	{
		Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
		e.Handled = true;
	}
}
