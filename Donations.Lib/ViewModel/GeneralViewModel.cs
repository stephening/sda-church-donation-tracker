using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Donations.Lib.ViewModel;

public partial class GeneralViewModel : BaseViewModel
{
	public static byte[] s_additionalEntropy = { 0x48, 0x10, 0x1b, 0xcb, 0xff, 0xdf, 0x43, 0x62, 0xb8, 0x91, 0x85, 0xa7, 0xa7, 0x16, 0x3f, 0xd0 };

	public ObservableCollection<Category>? CatList => _categoryServices?.CatList;
	public ObservableCollection<Donor>? _donors;

	private readonly IFileSystem _fileSystem;
	private readonly IDataHelpers _fileHelpers;
	private readonly IDonationServices _donationServices;
	private readonly IDonorServices _donorServices;
	private readonly ICategoryServices _categoryServices;
	private readonly IPictureServices _pictureServices;
	private readonly IAppSettingsServices _appSettingsServices;
	private bool _loaded = false;

	public GeneralViewModel(
		IFileSystem fileSystem,
		IDataHelpers fileHelpers,
		IDonationServices donationServices,
		IDonorServices donorServices,
		ICategoryServices categoryServices,
		IPictureServices pictureServices,
		IAppSettingsServices appSettingsServices
	)
	{
		_fileSystem = fileSystem;
		_fileHelpers = fileHelpers;
		_donationServices = donationServices;
		_donorServices = donorServices;
		_categoryServices = categoryServices;
		_pictureServices = pictureServices;
		_appSettingsServices = appSettingsServices;
	}

	public new async Task Loading()
	{
		OrganizationLogo = _pictureServices.GetLogo();
		var data = _appSettingsServices.Get();
		SyncFusionLicenseKey = data.SyncFusionLicenseKey;
		PictureBaseUrl = data.PictureBaseUrl;
		EmailAccount = data.EmailAccount;
		EmailSmtpServer = data.EmailSmtpServer;
		EmailServerPort = data.EmailServerPort;
		EmailEnableSsl = data.EmailEnableSsl;
		_loaded = true;

		InitPasswordBox();
	}

	public new async Task Leaving()
	{
		// don't save anything if we haven't even entered this tab
		if (!_loaded) { return; }

		await _pictureServices.SaveLogo(OrganizationLogo);
		if (string.IsNullOrEmpty(_passwordBox.Password))
		{
			Persist.Default.EncryptedEmailPassword = "";
		}
		else
		{
			Persist.Default.EncryptedEmailPassword = string.Join(' ', ProtectedData.Protect(Encoding.Default.GetBytes(_passwordBox.Password), s_additionalEntropy, DataProtectionScope.CurrentUser));
		}

		Persist.Default.Save();

		bool changed = false;
		var data = _appSettingsServices.Get();
		if (SyncFusionLicenseKey != data.SyncFusionLicenseKey)
		{
			_appSettingsServices.Get().SyncFusionLicenseKey = SyncFusionLicenseKey;
			changed = true;
		}
		
		if (EmailAccount != data.EmailAccount)
		{
			_appSettingsServices.Get().EmailAccount = EmailAccount;
			changed = true;
		}

		if (PictureBaseUrl != data.PictureBaseUrl)
		{
			_appSettingsServices.Get().PictureBaseUrl = PictureBaseUrl;
			changed = true;
		}

		if (EmailSmtpServer != data.EmailSmtpServer)
		{
			_appSettingsServices.Get().EmailSmtpServer = EmailSmtpServer;
			changed = true;
		}

		if (EmailServerPort != data.EmailServerPort)
		{
			_appSettingsServices.Get().EmailServerPort = EmailServerPort;
			changed = true;
		}

		if (EmailEnableSsl != data.EmailEnableSsl)
		{
			_appSettingsServices.Get().EmailEnableSsl = EmailEnableSsl;
			changed = true;
		}

		if (changed)
		{
			await _appSettingsServices.Save();
		}
	}

	[ObservableProperty]
	private Picture? _organizationLogo;

	[ObservableProperty]
	private string? _syncFusionLicenseKey;

	[ObservableProperty]
	private string? _pictureBaseUrl;

	[ObservableProperty]
	private string? _EmailSmtpServer;

	[ObservableProperty]
	private int? _EmailServerPort;

	[ObservableProperty]
	private bool _EmailEnableSsl;

	[ObservableProperty]
	private string? _EmailAccount;

	private PasswordBox? _passwordBox = null;
	public void PasswordChanged(PasswordBox password)
	{
		if (string.IsNullOrEmpty(_passwordBox?.Password))
		{
			Persist.Default.EncryptedEmailPassword = "";
		}
		else
		{
			Persist.Default.EncryptedEmailPassword = string.Join(' ', ProtectedData.Protect(Encoding.Default.GetBytes(_passwordBox.Password), s_additionalEntropy, DataProtectionScope.CurrentUser));
		}
	}

	private void InitPasswordBox()
	{
		if (null != _passwordBox && !string.IsNullOrEmpty(Persist.Default.EncryptedEmailPassword))
		{
			_passwordBox.Password = Encoding.Default.GetString(ProtectedData.Unprotect(Persist.Default.EncryptedEmailPassword.Split(' ').Select(byte.Parse).ToArray(), s_additionalEntropy, DataProtectionScope.CurrentUser));
		}
	}

	public void SetPasswordObject(PasswordBox passwordBox)
	{
		_passwordBox = passwordBox;
		InitPasswordBox();
	}

	///// <summary>
	///// When this one second timer expires, then the settings will be written to the database.
	///// If a change in these fields is detected before the timer expires,
	///// the unexpired timer will be canceled and a new 1 second timer will be started.
	///// </summary>
	//private DispatcherTimer _delayedUpdateSettingsTimer = new DispatcherTimer();

	//private async void UpdateSettings(object sender, EventArgs e)
	//{
	//	_delayedUpdateSettingsTimer.Stop();

	//	_appSettings!.SyncFusionLicenseKey = SyncFusionLicenseKey;
	//	_appSettings.PictureBaseUrl = PictureBaseUrl;

	//	_appSettings.EmailSmtpServer = EmailSmtpServer;
	//	_appSettings.EmailServerPort = EmailServerPort;
	//	_appSettings.EmailEnableSsl = EmailEnableSsl;
	//	_appSettings.EmailAccount = EmailAccount;
	//	if (string.IsNullOrEmpty(_passwordBox?.Password))
	//	{
	//		Persist.Default.EncryptedEmailPassword = "";
	//	}
	//	else
	//	{
	//		Persist.Default.EncryptedEmailPassword = string.Join(' ', ProtectedData.Protect(Encoding.Default.GetBytes(_passwordBox.Password), s_additionalEntropy, DataProtectionScope.CurrentUser));
	//	}

	//	Persist.Default.Save();

	//	await _appSettingsServices.Save();
	//}
	public async Task LoadDonors()
	{
		_donors = await _donorServices.LoadDonors();
	}

	public void SetLogo(string logoFile)
	{
		if (null == OrganizationLogo)
		{
			// directly set the private so the OnPropertyChanged() function is not prematurely called
			_organizationLogo = new Picture();
		}
		OrganizationLogo.Image = _fileSystem.File.ReadAllBytes(logoFile);

		_pictureServices.SaveLogo(_organizationLogo);

		OnPropertyChanged(nameof(OrganizationLogo));
	}

	public void ExportCategories(string name)
	{
		_fileHelpers.ExportCsv<Category>(name, CatList);
	}

	public void ExportDonors(string name)
	{
		_fileHelpers.ExportCsv<Donor>(name, _donors);
	}

	public async Task ExportDonations(string name)
	{
		var donationList = await _donationServices.LoadDonations();
		_fileHelpers.ExportCsv<Donation>(name, donationList);
	}
}
