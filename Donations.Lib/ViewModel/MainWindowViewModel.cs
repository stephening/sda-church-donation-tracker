using System.Collections.Generic;
using System.Threading.Tasks;

namespace Donations.Lib.ViewModel;

public class MainWindowViewModel
{
	public MainWindowViewModel(
		BatchBrowserViewModel batchBrowserViewModel,
		DonationBrowserViewModel donationBrowserViewModel,
		AdventistGivingViewModel adventistGivingViewModel,
		AGDonorResolutionViewModel aGDonorResolutionViewModel,
		AGCategoryResolutionViewModel aGCategoryResolutionViewModel,
		AGDonationSummaryViewModel aGDonationSummaryViewModel,
		DonorInputViewModel donorInputViewModel,
		ReportsViewModel reportsViewModel,
		DonorViewModel donorViewModel,
		CategoryViewModel categoryViewModel,
		DonorMapViewModel donorMapViewModel,
		CategoryMapViewModel categoryMapViewModel,
		EnvelopeDesignViewModel envelopeDesignViewModel,
		GeneralViewModel generalViewModel,
		DirectoryViewModel directoryViewModel
	)
	{
		BatchBrowserViewModel = batchBrowserViewModel;
		DonationBrowserViewModel = donationBrowserViewModel;
		AdventistGivingViewModel = adventistGivingViewModel;
		AGDonorResolutionViewModel = aGDonorResolutionViewModel;
		AGCategoryResolutionViewModel = aGCategoryResolutionViewModel;
		AGDonationSummaryViewModel = aGDonationSummaryViewModel;
		DonorInputViewModel = donorInputViewModel;
		ReportsViewModel = reportsViewModel;
		DonorViewModel = donorViewModel;
		CategoryViewModel = categoryViewModel;
		DonorMapViewModel = donorMapViewModel;
		CategoryMapViewModel = categoryMapViewModel;
		EnvelopeDesignViewModel = envelopeDesignViewModel;
		GeneralViewModel = generalViewModel;
		DirectoryViewModel = directoryViewModel;
	}

	public async Task Shutdown()
	{
		List<Task> tasks = new List<Task>();

		tasks.Add(BatchBrowserViewModel!.Leaving());
		tasks.Add(DonationBrowserViewModel!.Leaving());
		tasks.Add(AdventistGivingViewModel!.Leaving());
		tasks.Add(AGDonorResolutionViewModel!.Leaving());
		tasks.Add(AGCategoryResolutionViewModel!.Leaving());
		tasks.Add(AGDonationSummaryViewModel!.Leaving());
		tasks.Add(DonorInputViewModel!.Leaving());
		tasks.Add(ReportsViewModel!.Leaving());
		tasks.Add(DonorViewModel!.Leaving());
		tasks.Add(CategoryViewModel!.Leaving());
		tasks.Add(DonorMapViewModel!.Leaving());
		tasks.Add(CategoryMapViewModel!.Leaving());
		tasks.Add(EnvelopeDesignViewModel!.Leaving());
		tasks.Add(GeneralViewModel!.Leaving());
		tasks.Add(DirectoryViewModel!.Leaving());

		await Task.WhenAll(tasks);
	}

	public double MaximumHeight => System.Windows.SystemParameters.WorkArea.Height;
	public BatchBrowserViewModel? BatchBrowserViewModel { get; }
	public DonationBrowserViewModel? DonationBrowserViewModel { get; }
	public AdventistGivingViewModel? AdventistGivingViewModel { get; }
	public AGDonorResolutionViewModel? AGDonorResolutionViewModel { get; }
	public AGCategoryResolutionViewModel? AGCategoryResolutionViewModel { get; }
	public AGDonationSummaryViewModel? AGDonationSummaryViewModel { get; }
	public DonorInputViewModel? DonorInputViewModel { get; }
	public ReportsViewModel? ReportsViewModel { get; }
	public DonorViewModel? DonorViewModel { get; }
	public CategoryViewModel? CategoryViewModel { get; }
	public DonorMapViewModel? DonorMapViewModel { get; }
	public CategoryMapViewModel? CategoryMapViewModel { get; }
	public EnvelopeDesignViewModel? EnvelopeDesignViewModel { get; }
	public GeneralViewModel? GeneralViewModel { get; }
	public DirectoryViewModel? DirectoryViewModel { get; }
}
