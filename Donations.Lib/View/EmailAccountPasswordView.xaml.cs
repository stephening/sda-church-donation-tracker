using Donations.Lib.Interfaces;
using Donations.Lib.ViewModel;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for EmailAccountPasswordView.xaml
/// </summary>
public partial class EmailAccountPasswordView : Window
{
	private readonly HelpView _helpView;

	public EmailAccountPasswordView(
		HelpView helpView,
		IAppSettingsServices appSettingsServices
	)
	{
		InitializeComponent();
		Account.Text = appSettingsServices.Get().EmailAccount;
		_helpView = helpView;
	}

	private void Click_Cancel(object sender, RoutedEventArgs e)
	{
		DialogResult = false;
		Close();
	}

	private void Click_OK(object sender, RoutedEventArgs e)
	{
		DialogResult = true;
		if (true == Remember.IsChecked)
		{
			Persist.Default.EncryptedEmailPassword = string.Join(' ', ProtectedData.Protect(Encoding.Default.GetBytes(PasswordBox.Password), GeneralViewModel.s_additionalEntropy, DataProtectionScope.CurrentUser));

			Persist.Default.Save();
		}
		Close();
	}

	private void Window_KeyDown(object sender, KeyEventArgs e)
	{
		_helpView.ShowTarget("Email-account-password-popup");
	}

	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		_helpView.ForceClose();
	}
}
