using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

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

		OrganizationLogo = _pictureServices.GetLogo();
		_appSettings = _appSettingsServices.Get();
		SyncFusionLicenseKey = _appSettings.SyncFusionLicenseKey;
		PictureBaseUrl = _appSettings.PictureBaseUrl;
		EmailAccount = _appSettings.EmailAccount;
		EmailSmtpServer = _appSettings.EmailSmtpServer;
		EmailServerPort = _appSettings.EmailServerPort;
		EmailEnableSsl = _appSettings.EmailEnableSsl;

		_delayedUpdateSettingsTimer.Tick += new EventHandler(UpdateSettings!);
		_delayedUpdateSettingsTimer.Interval = new TimeSpan(0, 0, 2);
	}

	public new async Task Loading()
	{
		OrganizationLogo = _pictureServices.GetLogo();
		_appSettings = _appSettingsServices.Get();
		InitPasswordBox();
	}

	public new async Task Leaving()
	{
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

		await _appSettingsServices.Save();
	}

	[ObservableProperty]
	private Picture? _organizationLogo;

	private AppSettings? _appSettings;

	private void RestartUpdateTimer()
	{
		_delayedUpdateSettingsTimer.Stop();
		_delayedUpdateSettingsTimer.Start();
	}

	[ObservableProperty]
	private string? _syncFusionLicenseKey;
	partial void OnSyncFusionLicenseKeyChanged(string? value)
	{
		_appSettings!.SyncFusionLicenseKey = SyncFusionLicenseKey;
	}

	[ObservableProperty]
	private string? _pictureBaseUrl;

	partial void OnPictureBaseUrlChanged(string? value)
	{
		_appSettings.PictureBaseUrl = PictureBaseUrl;
	}

	[ObservableProperty]
	private string? _EmailSmtpServer;

	partial void OnEmailSmtpServerChanged(string? value)
	{
		_appSettings.EmailSmtpServer = EmailSmtpServer;
	}

	[ObservableProperty]
	private int? _EmailServerPort;

	partial void OnEmailServerPortChanged(int? value)
	{
		_appSettings.EmailServerPort = EmailServerPort;
	}

	[ObservableProperty]
	private bool _EmailEnableSsl;

	partial void OnEmailEnableSslChanged(bool value)
	{
		_appSettings.EmailEnableSsl = EmailEnableSsl;
	}

	[ObservableProperty]
	private string? _EmailAccount;

	partial void OnEmailAccountChanged(string? value)
	{
		_appSettings.EmailAccount = EmailAccount;
	}

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

	/// <summary>
	/// When this one second timer expires, then the settings will be written to the database.
	/// If a change in these fields is detected before the timer expires,
	/// the unexpired timer will be canceled and a new 1 second timer will be started.
	/// </summary>
	private DispatcherTimer _delayedUpdateSettingsTimer = new DispatcherTimer();

	private async void UpdateSettings(object sender, EventArgs e)
	{
		_delayedUpdateSettingsTimer.Stop();

		_appSettings!.SyncFusionLicenseKey = SyncFusionLicenseKey;
		_appSettings.PictureBaseUrl = PictureBaseUrl;

		_appSettings.EmailSmtpServer = EmailSmtpServer;
		_appSettings.EmailServerPort = EmailServerPort;
		_appSettings.EmailEnableSsl = EmailEnableSsl;
		_appSettings.EmailAccount = EmailAccount;
		if (string.IsNullOrEmpty(_passwordBox?.Password))
		{
			Persist.Default.EncryptedEmailPassword = "";
		}
		else
		{
			Persist.Default.EncryptedEmailPassword = string.Join(' ', ProtectedData.Protect(Encoding.Default.GetBytes(_passwordBox.Password), s_additionalEntropy, DataProtectionScope.CurrentUser));
		}

		Persist.Default.Save();

		await _appSettingsServices.Save();
	}
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
