using Donations.Lib.Interfaces;
using Donations.Lib.View;
using Donations.Lib.ViewModel;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Donations.Lib;

public class MemberScreenShots : ScreenShotBase
{
	private readonly IFileSystem _fileSystem;
	private readonly MainWindowMembersViewModel _mainWindowMembersViewModel;
	private readonly IBatchServices _batchServices;
	private readonly CategoryReviewViewModel _categoryReviewViewModel;
	private readonly MainWindowMembersControl _mainWindowMembersControl;
	private readonly EmailAccountPasswordView _emailAccountPasswordView;
	private readonly WizardMainWindow _wizardMainWindow;
	private readonly DonorModalView.Factory _donorModalViewFactory;
	private readonly ConfirmDonorMergeView.Factory _confirmDonorMergeViewFactory;
	private readonly BatchReviewView.Factory _batchReviewViewFactory;
	private readonly PrintPreviewView.Factory _printPreviewViewFactory;

	private readonly IDonationServices _donationServices;

	public MemberScreenShots(
		IFileSystem fileSystem,
		MainWindowMembersViewModel mainWindowMembersViewModel,
		IBatchServices batchServices,
		CategoryReviewViewModel categoryReviewViewModel,
		MainWindowMembersControl mainWindowMembersControl,
		EmailAccountPasswordView emailAccountPasswordView,
		WizardMainWindow wizardMainWindow,
		DonorModalView.Factory donorModalViewFactory,
		ConfirmDonorMergeView.Factory confirmDonorMergeViewFactory,
		BatchReviewView.Factory batchReviewViewFactory,
		CategoryReviewView.Factory categoryReviewFactory,
		PrintPreviewView.Factory printPreviewViewFactory,
		IDonationServices donationServices
	)
	{
		_fileSystem = fileSystem;
		_mainWindowMembersViewModel = mainWindowMembersViewModel;
		_batchServices = batchServices;
		_categoryReviewViewModel = categoryReviewViewModel;
		_mainWindowMembersControl = mainWindowMembersControl;
		_emailAccountPasswordView = emailAccountPasswordView;
		_wizardMainWindow = wizardMainWindow;
		_donorModalViewFactory = donorModalViewFactory;
		_confirmDonorMergeViewFactory = confirmDonorMergeViewFactory;
		_batchReviewViewFactory = batchReviewViewFactory;
		_printPreviewViewFactory = printPreviewViewFactory;
		_donationServices = donationServices;
	}

	public async Task AllScreens(string destination_folder)
	{
		if (!_fileSystem.Directory.Exists(destination_folder))
		{
			Directory.CreateDirectory(destination_folder);
		}

		var td = new TestData();

		WindowContainer windowContainer = new WindowContainer();
		windowContainer.Title = "Member maintenance";

		_mainWindowMembersControl.DataContext = _mainWindowMembersViewModel;
		windowContainer.Main.Content = _mainWindowMembersControl;
		windowContainer.Width = 1200;
		windowContainer.Height = 800;
		windowContainer.Show();

		_mainWindowMembersViewModel.DonorViewModel.DonationsVisibility = Visibility.Collapsed;
		_mainWindowMembersControl.MainTabControl.SelectedItem = _mainWindowMembersControl.MemberTab;
		DonorViewModel? donorViewModel = _mainWindowMembersControl.DonorView.DataContext as DonorViewModel;
		donorViewModel?.SetDonor(td.DonorList[0]);

		await SaveScreenshot(windowContainer, destination_folder, "MemberTab.jpg");

		DonorSelectionView? donorSelectionView = DependencyInjection.DonorSelectionView;

		donorSelectionView!.Show();
		await SaveScreenshot(donorSelectionView, destination_folder, "DonorSelectionView.jpg");
		donorSelectionView.Close();

		var donorModalView = _donorModalViewFactory(td.DonorList[0], false);

		donorModalView.Show();

		await SaveScreenshot(donorModalView, destination_folder, "DonorModalView.jpg");

		donorModalView.Close();

		_mainWindowMembersControl.MainTabControl.SelectedItem = _mainWindowMembersControl.DirectoryTabs;

		await SaveScreenshot(windowContainer, destination_folder, "DirectoryTab-Pdf.jpg");

		TabItem? tabItem = _mainWindowMembersControl.DirectoryTabs as TabItem;
		DirectoryTabView? dirView = tabItem?.Content as DirectoryTabView;
		dirView!.DirectoryTabs.SelectedItem = dirView.DirectoryHtmlTab;

		await SaveScreenshot(windowContainer, destination_folder, "DirectoryTab-Html.jpg");

		_wizardMainWindow.Show();

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-Introduction.jpg");

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardSqlChoice;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-SqlChoice.jpg");

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardCloudInstall;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-SqlCloud.jpg");

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardLocalhostInstall;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-SqlLocalhost.jpg");

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardConnectionString;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-SqlConnectionString.jpg");

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardCreateDatabaseAndTables;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-SqlCreateDatabaseAndTables.jpg");

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardImportDonors;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-ImportDonors.jpg");

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardFinished;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-Finished.jpg");
	}

	private void CategoryReportPrintRender(FlowDocument document, string? font, double size, double printAreaWidth)
	{
		_categoryReviewViewModel?.CreatePreview(document, font, size, printAreaWidth);
	}
}
