using Donations.Lib.ViewModel;
using System.IO.Abstractions;
using System.Windows;

namespace Donations.Lib.View;

public partial class HelpView : Window
{
	private readonly IFileSystem _fileSystem;

	//public static string? s_HelpTarget;

	private readonly HelpViewModel _helpViewModel;
	private bool _forceClose = false;

	public HelpView(
		IFileSystem fileSystem,
		HelpViewModel helpViewModel
	)
	{
		InitializeComponent();
		_fileSystem = fileSystem;
		_helpViewModel = helpViewModel;

		DataContext = helpViewModel;

		if (_fileSystem.File.Exists(helpViewModel.HtmlContent.Replace("file://", "")))
		{
			HelpBrowser.Navigate(helpViewModel.HtmlContent);
		}
	}

	private void HelpNavigation_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
	{
		HelpNavigationViewModel item = e.NewValue as HelpNavigationViewModel;

		if (null != item)
		{
			if (_fileSystem.File.Exists(_helpViewModel.HtmlContent.Replace("file://", "")))
			{
				HelpBrowser.Navigate(_helpViewModel.HtmlContent + item.Target);
			}
		}
	}

	public void ShowTarget(string target)
	{
		_helpViewModel.JumpToAnchor(HelpBrowser, target);

		// make visible if not already
		Show();

		// bring to front if not already
		Activate();
	}
	public void ForceClose()
	{
		_forceClose = true;
		Close();
	}

	private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
	{
		if (!_forceClose)
		{
			Hide();
			e.Cancel = true;
		}
	}
}
