using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Extensions;
using Donations.Lib.View;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using System.Windows.Controls;

namespace Donations.Lib.ViewModel;

public partial class HelpViewModel : ObservableObject
{
	private readonly ILogger _logger;
	private readonly IFileSystem _fileSystem;

	[ObservableProperty]
	private string _htmlContent;

	[ObservableProperty]
	private ObservableCollection<HelpNavigationViewModel> _helpNavigation;

	[ObservableProperty]
	private Dictionary<string, string> _anchors;

	[ObservableProperty]
	private string _helpFolder;

	public delegate HelpViewModel Factory(string helpFolder);

	public HelpViewModel(
		ILogger logger,
		IFileSystem fileSystem
	)
	{
		_logger = logger;
		_fileSystem = fileSystem;

		if (_fileSystem.File.Exists(Persist.Default.NavTreeJsonFile))
		{
			using (var reader = _fileSystem.File.OpenText(Persist.Default.NavTreeJsonFile))
			{
				var jstring = reader.ReadToEnd();
				HelpNavigation = JsonSerializer.Deserialize<ObservableCollection<HelpNavigationViewModel>>(jstring);
			}
		}
		else
		{
			HelpNavigation = new ObservableCollection<HelpNavigationViewModel>();
		}

		if (_fileSystem.File.Exists(Persist.Default.NavAnchorsJsonFile))
		{
			using (var reader = _fileSystem.File.OpenText(Persist.Default.NavAnchorsJsonFile))
			{
				var jstring = reader.ReadToEnd();
				Anchors = JsonSerializer.Deserialize<Dictionary<string, string>>(jstring);
			}
		}
		else
		{
			Anchors = new Dictionary<string, string>();
		}

		HtmlContent = "file://" + Directory.GetCurrentDirectory().Replace("\\", "/") + "/" + Persist.Default.HtmlHelpFile.Replace("\\", "/");
	}

	public void JumpToAnchor(WebBrowser webBrowser, string? target)
	{
		if (!string.IsNullOrEmpty(target))
		{
			foreach (var key in Anchors.Keys)
			{
				if (key.EndsWith(target))
				{
					target = key;
					break;
				}
			}
			try
			{
				if (_fileSystem.File.Exists(HtmlContent.Replace("file://", "")))
				{
					webBrowser.Navigate(HtmlContent + target);
				}
			}
			catch (Exception ex)
			{
				_logger.Err(ex, $"Exception trying to navigate to tager: {target}");
				if (_fileSystem.File.Exists(HtmlContent.Replace("file://", "")))
				{
					webBrowser.Navigate(HtmlContent);
				}
			}
		}
	}
}
