using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Microsoft.Win32;
using System.IO.Abstractions;

namespace Donations.Lib.ViewModel;

public partial class WizardSpecifyLogoViewModel : ObservableObject
{
	public WizardSpecifyLogoViewModel(
		IFileSystem fileSystem,
		IPictureServices pictureServices
	)
	{
		_fileSystem = fileSystem;
		_pictureServices = pictureServices;
	}

	[ObservableProperty]
	private Picture? _organizationLogo;
	private readonly IFileSystem _fileSystem;
	private readonly IPictureServices _pictureServices;

	[RelayCommand]
	public void Browse()
	{
		OpenFileDialog dlg = new OpenFileDialog();
		dlg.Filter = "Images (*.jpg;*.png)|*.jpg;*.png";

		if (dlg.ShowDialog() == true)
		{
			if (null == OrganizationLogo)
			{
				// directly set the private so the OnPropertyChanged() function is not prematurely called
				_organizationLogo = new Picture();
			}
			OrganizationLogo.Image = _fileSystem.File.ReadAllBytes(dlg.FileName);

#pragma warning disable CS8604 // Possible null reference argument.
			_pictureServices.SaveLogo(_organizationLogo);
#pragma warning restore CS8604 // Possible null reference argument.

			OnPropertyChanged(nameof(OrganizationLogo));
		}
	}
}
