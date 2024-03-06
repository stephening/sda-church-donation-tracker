using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.View;
using Donations.Lib.ViewModel;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Donations.Lib;

public class DonationsScreenShots : ScreenShotBase
{
	private readonly IFileSystem _fileSystem;
	private readonly MainWindowViewModel _mainWindowViewModel;
	private readonly IBatchServices _batchServices;
	private readonly CategoryReviewViewModel _categoryReviewViewModel;
	private readonly MainWindowControl _mainWindowControl;
	private readonly EmailAccountPasswordView _emailAccountPasswordView;
	private readonly WizardMainWindow _wizardMainWindow;
	private readonly DonorModalView.Factory _donorModalViewFactory;
	private readonly ConfirmDonorMergeView.Factory _confirmDonorMergeViewFactory;
	private readonly BatchReviewView.Factory _batchReviewViewFactory;
	private readonly PrintPreviewView.Factory _printPreviewViewFactory;

	private readonly IDonationServices _donationServices;

	public DonationsScreenShots(
		IFileSystem fileSystem,
		MainWindowViewModel mainWindowViewModel,
		IBatchServices batchServices,
		CategoryReviewViewModel categoryReviewViewModel,
		MainWindowControl mainWindowControl,
		EmailAccountPasswordView emailAccountPasswordView,
		WizardMainWindow wizardMainWindow,
		DonorModalView.Factory donorModalViewFactory,
		ConfirmDonorMergeView.Factory confirmDonorMergeViewFactory,
		BatchReviewView.Factory batchReviewViewFactory,
		PrintPreviewView.Factory printPreviewViewFactory,
		IDonationServices donationServices
	)
	{
		_fileSystem = fileSystem;
		_mainWindowViewModel = mainWindowViewModel;
		_batchServices = batchServices;
		_categoryReviewViewModel = categoryReviewViewModel;
		_mainWindowControl = mainWindowControl;
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

		_mainWindowControl.DataContext = _mainWindowViewModel;
		windowContainer.Main.Content = _mainWindowControl;
		windowContainer.Width = 1200;
		windowContainer.Height = 800;
		windowContainer.Show();

		_mainWindowViewModel.BatchBrowserViewModel.DateFilterOption = enumDateFilterOptions.SelectYear;
		_mainWindowViewModel.BatchBrowserViewModel.FilterYear = "2023";

		await SaveScreenshot(windowContainer, destination_folder, "BatchBrowserViewTab.jpg");

		Batch? batch = await _batchServices.GetBatchById(6);

		ObservableCollection<Donation> batchDonationList = await _mainWindowViewModel.BatchBrowserViewModel!.GetDonationsByBatchId(batch!.Id);

		var batchReviewView = _batchReviewViewFactory(batch, batchDonationList);

		BatchReviewViewModel? batchReviewViewModel = batchReviewView.DataContext as BatchReviewViewModel;

		batchReviewView.Show();

		await SaveScreenshot(batchReviewView, destination_folder, "BatchReviewView-ByCategoryTab.jpg");

		CategorySum catsum = batchReviewViewModel!.CategorySums![0];
		CategoryReviewView batchCatReview = batchReviewViewModel.CreateCategoryReviewView(catsum);

		batchCatReview.Show();

		await SaveScreenshot(batchCatReview, destination_folder, "BatchReviewView-CategoryReviewView.jpg");

		batchCatReview.Close();

		PrintPreviewView batchPrintPreviewView = _printPreviewViewFactory(enumPrintout.CategoryReport, "Batch-contributions-printout", CategoryReportPrintRender);

		batchPrintPreviewView.Show();

		await SaveScreenshot(batchPrintPreviewView, destination_folder, "BatchReviewView_CategoryReviewView_PrintPreviewView.jpg");

		batchPrintPreviewView.Close();

		batchReviewView.TabControl.SelectedItem = batchReviewView.ByDonorTab;

		await SaveScreenshot(batchReviewView, destination_folder, "BatchReviewView-ByDonorTab.jpg");

		batchReviewView.TabControl.SelectedItem = batchReviewView.PrintTab;

		await SaveScreenshot(batchReviewView, destination_folder, "BatchReviewView-PrintTab.jpg");

		batchReviewView.Close();

		_mainWindowViewModel.DonationBrowserViewModel.DateFilterOption = enumDateFilterOptions.SelectYear;
		_mainWindowViewModel.DonationBrowserViewModel.FilterYear = "2023";

		await Task.Delay(1500); // wait for 1 second debounce

		_mainWindowControl.MainTabControl.SelectedItem = _mainWindowControl.DonationBrowserTab;

		await SaveScreenshot(windowContainer, destination_folder, "DonationBrowserViewTab.jpg");

		CategoryReviewView categoryReviewView = _mainWindowViewModel.DonationBrowserViewModel?.CreateCategoryReviewView((_mainWindowViewModel.DonationBrowserViewModel.CategorySource.Source as ObservableCollection<CategorySum>)[0]);
		categoryReviewView.Show();

		await SaveScreenshot(categoryReviewView, destination_folder, "DonationBrowserView_CategoryReviewView.jpg");

		PrintPreviewView donationPrintPreviewView = _printPreviewViewFactory(enumPrintout.CategoryReport, "Donor-contributions-printout", CategoryReportPrintRender);

		donationPrintPreviewView.Show();

		await SaveScreenshot(donationPrintPreviewView, destination_folder, "DonationBrowserView_CategoryReviewView_PrintPreviewView.jpg");

		donationPrintPreviewView.Close();
		categoryReviewView.Close();

		DonationPopupView donationPopupView = await _mainWindowViewModel.DonationBrowserViewModel?.CreateDonationPopupView((_mainWindowViewModel.DonationBrowserViewModel.DonationSource.Source as ObservableCollection<Donation>)[0].Id);
		donationPopupView.Show();

		await SaveScreenshot(donationPopupView, destination_folder, "DonationBrowserView_DonationPopupView.jpg");

		donationPopupView.Close();

		_mainWindowControl.MainTabControl.SelectedItem = _mainWindowControl.AdventistGivingMainTab;

		_mainWindowViewModel.AdventistGivingViewModel.TransactionList = new ObservableCollection<AdventistGiving>(td.AdventistGivingList!);
		_mainWindowViewModel.AdventistGivingViewModel.TransactionList[0].CategoryCode = 101;

		_mainWindowViewModel.AGDonorResolutionViewModel?.StartNameResolution();
		_mainWindowViewModel.AGCategoryResolutionViewModel?.StartCategoryResolution();

		await SaveScreenshot(windowContainer, destination_folder, "AdventistGivingViewTab-DonorResolutionTab.jpg");

		_mainWindowViewModel.AGDonorResolutionViewModel!.ContinueDonorResolutionCommand.Execute(null);

		TabItem? tabItem = _mainWindowControl.AdventistGivingMainTab as TabItem;
		AdventistGivingView? AGView = tabItem?.Content as AdventistGivingView;
		AGView!.AdventistGivingTabs.SelectedItem = AGView.CategoryResolutionTab;

		await SaveScreenshot(windowContainer, destination_folder, "AdventistGivingViewTab-CategoryResolutionTab.jpg");

		_mainWindowViewModel.AGCategoryResolutionViewModel!.ContinueCategoryResolutionCommand.Execute(null);

		_mainWindowControl.MainTabControl.SelectedItem = AGView.VerifyAndSubmitTab;

		await SaveScreenshot(windowContainer, destination_folder, "AdventistGivingViewTab-VerifySubmitTab.jpg");

		DonorInputViewModel? donorInputViewModel = _mainWindowControl.DonorInputView.DataContext as DonorInputViewModel;
		donorInputViewModel?.ChooseDonor(1);

		_mainWindowControl.MainTabControl.SelectedItem = _mainWindowControl.DonorInputTab;

		await SaveScreenshot(windowContainer, destination_folder, "DonorInputTab.jpg");

		DonorSelectionView? donorSelectionView = DependencyInjection.DonorSelectionView;

		donorSelectionView!.Show();
		await SaveScreenshot(donorSelectionView, destination_folder, "DonorSelectionView.jpg");
		donorSelectionView.Close();

		CategorySelectionView? categorySelectionView = DependencyInjection.CategorySelectionView;

		categorySelectionView!.Show();
		await SaveScreenshot(categorySelectionView, destination_folder, "CategorySelectionView.jpg");
		categorySelectionView.Close();

		var confirmDonorMergeViewFactory = _confirmDonorMergeViewFactory(td.DonorList[0], td.DonorList[1], await _donationServices.GetDonationsByDonorId(td.DonorList[1].Id));

		confirmDonorMergeViewFactory.Show();
		await SaveScreenshot(confirmDonorMergeViewFactory, destination_folder, "ConfirmDonorMergeView.jpg");
		confirmDonorMergeViewFactory.Close();

		_mainWindowViewModel.ReportsViewModel.DateFilterOption = enumDateFilterOptions.SelectYear;
		_mainWindowViewModel.ReportsViewModel.FilterYear = "2023";

		_mainWindowControl.MainTabControl.SelectedItem = _mainWindowControl.ReportsTab;
		await _mainWindowViewModel.ReportsViewModel.SetDonor(td.DonorList[0]);

		await SaveScreenshot(windowContainer, destination_folder, "ReportsTab.jpg");

		_mainWindowViewModel.ReportsViewModel.ReportOption = enumReportOptions.AllPdf;

		await SaveScreenshot(windowContainer, destination_folder, "ReportsTab_AllPdf.jpg");

		_mainWindowViewModel.ReportsViewModel.ReportOption = enumReportOptions.Email;

		await SaveScreenshot(windowContainer, destination_folder, "ReportsTab_Email.jpg");

		_mainWindowViewModel.ReportsViewModel.ReportOption = enumReportOptions.Print;

		await SaveScreenshot(windowContainer, destination_folder, "ReportsTab_Print.jpg");

		_mainWindowViewModel.ReportsViewModel.ReportOption = enumReportOptions.MockRun;
		await _mainWindowViewModel.ReportsViewModel.MockRun();

		await SaveScreenshot(windowContainer, destination_folder, "ReportsTab_MockRun.jpg");

		_mainWindowControl.MainTabControl.SelectedItem = _mainWindowControl.MaintenanceMainTab;
		DonorViewModel? donorViewModel = _mainWindowControl.DonorView.DataContext as DonorViewModel;
		donorViewModel?.SetDonor(td.DonorList[0]);

		await SaveScreenshot(windowContainer, destination_folder, "MaintenanceTab-DonorTab.jpg");

		_mainWindowControl.MaintenanceTabs.SelectedItem = _mainWindowControl.MaintenanceCategoryTab;

		await SaveScreenshot(windowContainer, destination_folder, "MaintenanceTab-CategoryTab.jpg");

		_mainWindowControl.MaintenanceTabs.SelectedItem = _mainWindowControl.MaintenanceDonorMapTab;

		await SaveScreenshot(windowContainer, destination_folder, "MaintenanceTab-DonorMapTab.jpg");

		_mainWindowControl.MaintenanceTabs.SelectedItem = _mainWindowControl.MaintenanceCategoryMapTab;

		await SaveScreenshot(windowContainer, destination_folder, "MaintenanceTab-CategoryMapTab.jpg");

		_mainWindowControl.MaintenanceTabs.SelectedItem = _mainWindowControl.MaintenanceTitheEnvelopeDesignTab;

		await SaveScreenshot(windowContainer, destination_folder, "MaintenanceTab-DesignTitheEnvelopeTab.jpg");

		_mainWindowControl.MaintenanceTabs.SelectedItem = _mainWindowControl.MaintenanceGeneralTab;

		await SaveScreenshot(windowContainer, destination_folder, "MaintenanceTab-GeneralTab.jpg");

		_mainWindowControl.MainTabControl.SelectedItem = _mainWindowControl.AboutTab;

		await SaveScreenshot(windowContainer, destination_folder, "AboutTab.jpg");

		_emailAccountPasswordView.Show();

		await SaveScreenshot(_emailAccountPasswordView, destination_folder, "EmailAccountPasswordView.jpg");

		_emailAccountPasswordView.Close();

		var donorModalView = _donorModalViewFactory(td.DonorList[0]);

		donorModalView.Show();

		await SaveScreenshot(donorModalView, destination_folder, "DonorModalView.jpg");

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

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardSpecifyLogo;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-SqlOrganizationLogo.jpg");

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardImportCategories;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-ImportCategories.jpg");

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardImportDonors;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-ImportDonors.jpg");

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardImportDonations;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-ImportDonations.jpg");

		_wizardMainWindow.MainTabControl.SelectedItem = _wizardMainWindow.WizardFinished;

		await SaveScreenshot(_wizardMainWindow, destination_folder, "Wizard-Finished.jpg");
	}

	private void CategoryReportPrintRender(FlowDocument document, string? font, double size, double printAreaWidth)
	{
		_categoryReviewViewModel?.CreatePreview(document, font, size, printAreaWidth);
	}

	public async Task SaveScreenshot(Window window, string folder, string filename, int msDelay = 500)
	{
		// wait for screen content to load
		await Task.Delay(msDelay);

		double border = SystemParameters.ResizeFrameVerticalBorderWidth + SystemParameters.FixedFrameHorizontalBorderHeight + SystemParameters.BorderWidth * SystemParameters.Border;

		RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
			(int)(window.Width - 2 * border),
			(int)(window.Height - 2 * border - SystemParameters.CaptionHeight),
			96, 96, PixelFormats.Default);

		renderTargetBitmap.Render(window);
		JpegBitmapEncoder jpegEncoder = new JpegBitmapEncoder();
		jpegEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
		using Stream fs = File.Create(Path.Combine(folder, filename));
		jpegEncoder.Save(fs);
	}

}
