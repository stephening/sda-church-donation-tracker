using Donations.Interfaces;
using Donations.Services;
using Donations.ViewModel;
using System.IO.Abstractions;
using System.Security.Principal;

namespace Donations
{
	/// <summary>
	/// I chose to implement dependency injection (di) in this very simple and yet functional way.
	/// </summary>
	public class di
	{
		static public string? Username { get; set; } = WindowsIdentity.GetCurrent().Name;

		/// <summary>
		/// The File dependency simply wraps some file functions which have been abstracted
		/// through the IFile (localy defined) interface, and service, partly for the benefit
		/// of dependency management but also to get the interface for mocking for unit
		/// testing.
		/// </summary>
		static public IFileSystem? FileSystem { get; set; } = new FileSystem();

		/// <summary>
		/// The File dependency simply wraps some file functions which have been abstracted
		/// through the IFile (localy defined) interface, and service, partly for the benefit
		/// of dependency management but also to get the interface for mocking for unit
		/// testing.
		/// </summary>
		static public IData? Data { get; set; } = new FileDataProvider();

		/// <summary>
		/// There are a few ViewModels that need to be accessed by other ViewModels. These
		/// have been added to the di class. The BatchBrowser vm is accessed by the
		/// ImportDonationsViewModel, after an import, so the newly imported data can be
		/// viewed and accessed without having to shutdown and restart the application.
		/// </summary>
		static public BatchBrowserViewModel? BatchBrowser { get; set; }

		/// <summary>
		/// There are a few ViewModels that need to be accessed by other ViewModels. These
		/// have been added to the di class. The AdventistGivingViewModel is actually like
		/// a parent object to the view models (tabs) under it. The button to start the
		/// import is handled by the AdventistGivingViewModel and the collection of records
		/// imported will also be contained in this object. The Donor/Category resolution
		/// and Summary tabs will need access to this data. Since this hierarchy is not
		/// implemented through inheritance, dependency injection is used to share this
		/// view model with the children.
		/// </summary>
		static public AdventistGivingViewModel? AG { get; set; }

		/// <summary>
		/// There are a few ViewModels that need to be accessed by other ViewModels. These
		/// have been added to the di class. The DonorResolution vm is accessed by the
		/// AdventistGivingViewModel, after an import has started, so it can begin the
		/// donor resolution. It is also accessed to reset the state of the donor resolution
		/// tab.
		/// </summary>
		static public AGDonorResolutionViewModel? DonorResolution { get; set; }

		/// <summary>
		/// There are a few ViewModels that need to be accessed by other ViewModels. These
		/// have been added to the di class. The CategoryResolution vm is accessed by the
		/// AdventistGivingViewModel, after an import has started, so it can begin the
		/// category resolution. It is also accessed to reset the state of the category
		/// resolution tab.
		/// </summary>
		static public AGCategoryResolutionViewModel? CategoryResolution { get; set; }

		/// <summary>
		/// There are a few ViewModels that need to be accessed by other ViewModels. These
		/// have been added to the di class. The DonationSummary vm is accessed by the
		/// AdventistGivingViewModel, to put it in the correct view state for starting
		/// or resetting am import.
		/// </summary>
		static public AGDonationSummaryViewModel? DonationSummary { get; set; }
	}
}
