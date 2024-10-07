using System.Windows;
using System.Windows.Controls;

namespace Donations.Lib.Services;

public class ContextSensetiveHelper
{
	public void ShowContextSensetiveHelp(FrameworkElement framework, TabControl mainTabControl)
	{
		//var tabItem = mainTabControl.SelectedItem as TabItem;
		//string target = "";
		//string s;

		//if (true == tabItem?.Header.Equals(s = (string)framework->FindResource("BatchBrowserTabHeader")))
		//{
		//	target = s.Replace(" ", "-");
		//}
		//else if (true == tabItem?.Header.Equals(s = (string)framework->FindResource("DonationBrowserTabHeader")))
		//{
		//	target = s.Replace(" ", "-");
		//}
		//else if (true == tabItem?.Header.Equals(s = (string)framework->FindResource("AdventistGivingTabHeader")))
		//{
		//	string baseTarget = s.Replace(" ", "-");
		//	TabControl? tab = (tabItem.Content as AdventistGivingView)?.AdventistGivingTabs;
		//	if (null != tab)
		//	{
		//		var item = tab.SelectedItem as TabItem;

		//		if (-1 == tab.SelectedIndex || true == item?.Header.Equals(s = (string)framework->FindResource("AdventistGivingDonorResolutionTabHeader")))
		//		{
		//			target = baseTarget + '-' + s.Replace(" ", "-");
		//		}
		//		else if (true == item?.Header.Equals(s = (string)framework->FindResource("AdventistGivingCategoryResolutionTabHeader")))
		//		{
		//			target = baseTarget + '-' + s.Replace(" ", "-");
		//		}
		//		else if (true == item?.Header.Equals(s = (string)framework->FindResource("AdventistGivingVerifyAndSubmitTabHeader")))
		//		{
		//			target = baseTarget + '-' + s.Replace(" ", "-");
		//		}
		//	}
		//}
		//else if (true == tabItem?.Header.Equals(s = (string)framework->FindResource("DonorInputTabHeader")))
		//{
		//	target = s.Replace(" ", "-");
		//}
		//else if (true == tabItem?.Header.Equals(s = (string)framework->FindResource("ReportsTabHeader")))
		//{
		//	target = s.Replace(" ", "-");
		//	if (_mainWindowViewModel.ReportsViewModel.ReportOption == enumReportOptions.AllPdf)
		//	{
		//		target += "-All-pdf\u0027s";
		//	}
		//	else if (_mainWindowViewModel.ReportsViewModel.ReportOption == enumReportOptions.Email)
		//	{
		//		target += "-Email-reports";
		//	}
		//	else if (_mainWindowViewModel.ReportsViewModel.ReportOption == enumReportOptions.Print)
		//	{
		//		target += "-Printed-reports";
		//	}
		//	else if (_mainWindowViewModel.ReportsViewModel.ReportOption == enumReportOptions.MockRun)
		//	{
		//		target += "-Mock-run";
		//	}
		//}
		//else if (true == tabItem?.Header.Equals(s = (string)framework->FindResource("MaintenanceTabHeader")))
		//{
		//	string baseTarget = s.Replace(" ", "-");
		//	var item = MaintenanceTabs.SelectedItem as TabItem;

		//	if (-1 == MaintenanceTabs.SelectedIndex || true == item?.Header.Equals(s = (string)framework->FindResource("MaintenanceDonorTabHeader")))
		//	{
		//		target = baseTarget + '-' + s.Replace(" ", "-");
		//	}
		//	else if (true == item?.Header.Equals(s = (string)framework->FindResource("MaintenanceCategoryTabHeader")))
		//	{
		//		target = baseTarget + '-' + s.Replace(" ", "-");
		//	}
		//	else if (true == item?.Header.Equals(s = (string)framework->FindResource("MaintenanceCategoryMapTabHeader")))
		//	{
		//		target = baseTarget + '-' + s.Replace(" ", "-");
		//	}
		//	else if (true == item?.Header.Equals(s = (string)framework->FindResource("MaintenanceDonorMapTabHeader")))
		//	{
		//		target = baseTarget + '-' + s.Replace(" ", "-");
		//	}
		//	else if (true == item?.Header.Equals(s = (string)framework->FindResource("MaintenanceDesignTitheEnvelopeTabHeader")))
		//	{
		//		target = baseTarget + '-' + s.Replace(" ", "-");
		//	}
		//	else if (true == item?.Header.Equals(s = (string)framework->FindResource("MaintenanceGeneralTabHeader")))
		//	{
		//		target = baseTarget + '-' + s.Replace(" ", "-");
		//	}
		//}
		//else if (true == tabItem?.Header.Equals(s = (string)framework->FindResource("AboutTabHeader")))
		//{
		//	target = s.Replace(" ", "-");
		//}

		//_helpView.ShowTarget(target);
	}
}
