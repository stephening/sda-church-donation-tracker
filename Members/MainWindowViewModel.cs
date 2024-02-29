using Donations.Lib.ViewModel;

namespace Donors;

public class MainWindowViewModel
{
	public MainWindowViewModel(
		//BatchBrowserViewModel batchBrowserViewModel,
		//AdventistGivingViewModel adventistGivingViewModel,
		//AGDonorResolutionViewModel aGDonorResolutionViewModel,
		//AGCategoryResolutionViewModel aGCategoryResolutionViewModel,
		//AGDonationSummaryViewModel aGDonationSummaryViewModel,
		//DonorInputViewModel donorInputViewModel,
		//ReportsViewModel reportsViewModel,
		DonorViewModel donorViewModel
	//CategoryViewModel categoryViewModel,
	//DonorMapViewModel donorMapViewModel,
	//CategoryMapViewModel categoryMapViewModel,
	//EnvelopeDesignViewModel envelopeDesignViewModel,
	//LoginAccountManagementViewModel loginAccountManagementViewModel,
	//GeneralViewModel generalViewModel
	)
	{
		//BatchBrowserViewModel = batchBrowserViewModel;
		//AdventistGivingViewModel = adventistGivingViewModel;
		//AGDonorResolutionViewModel = aGDonorResolutionViewModel;
		//AGCategoryResolutionViewModel = aGCategoryResolutionViewModel;
		//AGDonationSummaryViewModel = aGDonationSummaryViewModel;
		//DonorInputViewModel = donorInputViewModel;
		//ReportsViewModel = reportsViewModel;
		DonorViewModel = donorViewModel;
		//CategoryViewModel = categoryViewModel;
		//DonorMapViewModel = donorMapViewModel;
		//CategoryMapViewModel = categoryMapViewModel;
		//EnvelopeDesignViewModel = envelopeDesignViewModel;
		//LoginAccountManagementViewModel = loginAccountManagementViewModel;
		//GeneralViewModel = generalViewModel;
	}

	//public BatchBrowserViewModel BatchBrowserViewModel { get; }
	//public AdventistGivingViewModel AdventistGivingViewModel { get; }
	//public AGDonorResolutionViewModel AGDonorResolutionViewModel { get; }
	//public AGCategoryResolutionViewModel AGCategoryResolutionViewModel { get; }
	//public AGDonationSummaryViewModel AGDonationSummaryViewModel { get; }
	//public DonorInputViewModel DonorInputViewModel { get; }
	//public ReportsViewModel ReportsViewModel { get; }
	public DonorViewModel DonorViewModel { get; }
	//public CategoryViewModel CategoryViewModel { get; }
	//public DonorMapViewModel DonorMapViewModel { get; }
	//public CategoryMapViewModel CategoryMapViewModel { get; }
	//public EnvelopeDesignViewModel EnvelopeDesignViewModel { get; }
	//public LoginAccountManagementViewModel LoginAccountManagementViewModel { get; }
	//public GeneralViewModel GeneralViewModel { get; }
}
