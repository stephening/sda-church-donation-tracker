using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Donations.Lib.ViewModel;

public partial class DirectoryHtmlViewModel : BaseViewModel
{
	private readonly IFileSystem _fileSystem;
	private readonly ILogger _logger;
	private readonly IHtmlDirectoryServices _htmlDirectoryServices;
	private HtmlDirectory? _htmlData;
	private Dictionary<string, DirectoryData>? _directoryEntries;
	private bool _cancelLoading = false;
	private Semaphore _loading = new Semaphore(1, 1);

	public DirectoryHtmlViewModel(
		IFileSystem fileSystem,
		ILogger logger,
		IHtmlDirectoryServices htmlDirectoryServices
	)
    {
		_fileSystem = fileSystem;
		_logger = logger;
		_htmlDirectoryServices = htmlDirectoryServices;
	}


	[ObservableProperty]
	private string? _homePage;

	[ObservableProperty]
	private bool _nonMembers;
	partial void OnNonMembersChanged(bool value)
	{
#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
	}

	[ObservableProperty]
	private bool _orderByLast;
	partial void OnOrderByLastChanged(bool value)
	{
		if (value)
		{
			OrderByLastFilenameEnabled = true;
		}
		else
		{
			OrderByLastFilenameEnabled = false;
		}
#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
	}

	[ObservableProperty]
	private bool _orderByFirst;
	partial void OnOrderByFirstChanged(bool value)
	{
		if (value)
		{
			OrderByFirstFilenameEnabled = true;
		}
		else
		{
			OrderByFirstFilenameEnabled = false;
		}
#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
	}

	[ObservableProperty]
	private string _orderByLastFilename = "";
	partial void OnOrderByLastFilenameChanged(string value)
	{

	}

	[ObservableProperty]
	private string _orderByFirstFilename = "";
	partial void OnOrderByFirstFilenameChanged(string value)
	{
#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
	}

	[ObservableProperty]
	private string _picturePath = "";
	partial void OnPicturePathChanged(string value)
	{
#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
	}

	[ObservableProperty]
	private bool _orderByLastFilenameEnabled = true;

	[ObservableProperty]
	private bool _orderByFirstFilenameEnabled = true;

	[ObservableProperty]
	private string _header = "";
	partial void OnHeaderChanged(string value)
	{
		// Delay action until a certain time has past since the last change
		_delayedUpdateSettingsTimer.Stop();
		_delayedUpdateSettingsTimer.Start();
	}

	[ObservableProperty]
	private string _template = "";
	partial void OnTemplateChanged(string value)
	{
		// Delay action until a certain time has past since the last change
		_delayedUpdateSettingsTimer.Stop();
		_delayedUpdateSettingsTimer.Start();
	}

	[ObservableProperty]
	private string _footer = "";

	partial void OnFooterChanged(string value)
	{
		// Delay action until a certain time has past since the last change
		_delayedUpdateSettingsTimer.Stop();
		_delayedUpdateSettingsTimer.Start();
	}

	[ObservableProperty]
	private string _status = "";

	[ObservableProperty]
	private double _progress = 0;

	public void SetDirectoryEntries(Dictionary<string, DirectoryData>? directoryEntries)
	{
		_directoryEntries = directoryEntries;

#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
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

#pragma warning disable 4014
		// Loading will automatically stop a previous invocation before proceeding
		Loading();
#pragma warning restore
	}

	public void Cancel()
	{
		_cancelLoading = true;
	}

public new async Task Leaving()
	{
		var data = await _htmlDirectoryServices.GetAsync();

		bool changed = false;
		if (NonMembers != data.IncludeNonMembers)
		{
			data.IncludeNonMembers = NonMembers;
			changed = true;
		}

		if (OrderByLast != data.OrderByLast)
		{
			data.OrderByLast = OrderByLast;
			changed = true;
		}

		if (OrderByFirst != data.OrderByFirst)
		{
			data.OrderByFirst = OrderByFirst;
			changed = true;
		}

		if (OrderByLastFilename != data.OrderByLastFilename)
		{
			data.OrderByLastFilename = OrderByLastFilename;
			changed = true;
		}

		if (OrderByFirstFilename != data.OrderByFirstFilename)
		{
			data.OrderByFirstFilename = OrderByFirstFilename;
			changed = true;
		}

		if (PicturePath != data.PicturePath)
		{
			data.PicturePath = PicturePath;
			changed = true;
		}

		if (Header != data.Header)
		{
			data.Header = Header;
			changed = true;
		}

		if (Template != data.Template)
		{
			data.Template = Template;
			changed = true;
		}

		if (Footer != data.Footer)
		{
			data.Footer = Footer;
			changed = true;
		}

		if (changed)
		{
			await _htmlDirectoryServices?.Save();
		}
	}

	private string AddEntryByLast(DirectoryData record, bool[] indexArray, string basePictureUrl)
	{
		if (!NonMembers && false == record.Member)
		{
			return "";
		}

		string name = string.IsNullOrEmpty(record.Name) ? "&nbsp;" : record.Name;
		if (true == name?.Contains(','))
		{
			name = name.Split(',')[1]?.Trim();
		}

		string entry = Template
			.Replace("{SortName}", record.LastName)
			.Replace("{OtherName}", name)
			.Replace("{ThirdLine}", string.IsNullOrEmpty(record.OtherFamilyMembers) ? "&nbsp;" : record.OtherFamilyMembers);

		string pictureUrl = "";
		if (!string.IsNullOrEmpty(basePictureUrl))
		{
			if (string.IsNullOrEmpty(record.Picture))
			{
				pictureUrl = basePictureUrl + "/silhouette.jpg";
			}
			else
			{
				pictureUrl = basePictureUrl + "/" + record.Picture;
			}
		}

		entry = entry
			.Replace("{PictureUrl}", pictureUrl)
			.Replace("{FullName}", record.Name);

		if (!string.IsNullOrEmpty(record.LastName))
		{
			char indexChar = record.LastName[0];
			if (!indexArray[indexChar])
			{
				entry = entry.Replace("{IndexLetter}", $"{indexChar}");
				indexArray[indexChar] = true;
			}
			else
			{
				entry = entry.Replace("{IndexLetter}", "");
			}
		}
		else
		{
			entry = entry.Replace("{IndexLetter}", "");
		}

		return entry;
	}

	private void OutputByLast(double step, string basePictureUrl)
	{
		bool[] indexArray = new bool[256];
		var keys = _directoryEntries.Keys.Order();

		Status = "Rendering directory entries by last name";

		using var stream = _fileSystem.File.CreateText(OrderByLastFilename);

		stream.Write(Header);

		foreach (string key in keys)
		{
			Progress += step;
			if (_cancelLoading)
			{
				Status = "Rendering directory entries by last name cancelled";
				_cancelLoading = false;
				_loading.Release();
				return;
			}

			if (string.IsNullOrEmpty(_directoryEntries[key].FirstName) || _directoryEntries[key].FirstName == ".")
			{
				continue;
			}

			stream.Write(AddEntryByLast(_directoryEntries[key], indexArray, basePictureUrl));

			Status = "Completed rendering directory entries by last name";
		}

		stream.Write(Footer);
	}

	private string AddEntryByFirst(DirectoryData record, bool[] indexArray, string basePictureUrl)
	{
		if (!NonMembers && false == record.Member)
		{
			return "";
		}

		string thirdLine = "&nbsp;";
		if (!string.IsNullOrEmpty(record.Name))
		{
			thirdLine = record.Name;
		}
		if (!string.IsNullOrEmpty(record.OtherFamilyMembers))
		{
			thirdLine += ", " + record.OtherFamilyMembers;
		}
		string entry = Template
			.Replace("{SortName}", record.FirstName)
			.Replace("{OtherName}", record.LastName)
			.Replace("{ThirdLine}", thirdLine);

		string pictureUrl = "";
		if (!string.IsNullOrEmpty(basePictureUrl))
		{
			if (string.IsNullOrEmpty(record.Picture))
			{
				pictureUrl = basePictureUrl + "/silhouette.jpg";
			}
			else
			{
				pictureUrl = basePictureUrl + "/" + record.Picture;
			}
		}

		entry = entry
			.Replace("{PictureUrl}", pictureUrl)
			.Replace("{FullName}", $"{record.LastName}, {record.FirstName}");

		if (!string.IsNullOrEmpty(record.FirstName))
		{
			char indexChar = record.FirstName[0];
			if (!indexArray[indexChar])
			{
				entry = entry.Replace("{IndexLetter}", $"{indexChar}");
				indexArray[indexChar] = true;
			}
			else
			{
				entry = entry.Replace("{IndexLetter}", "");
			}
		}
		else
		{
			entry = entry.Replace("{IndexLetter}", "");
		}

		return entry;
	}

	private void OutputByFirst(double step, string basePictureUrl)
	{
		bool[] indexArray = new bool[256];
		var byFirst = _directoryEntries.Values.OrderBy(x => x.FirstName);

		Status = "Rendering directory entries by first name";

		using var stream = _fileSystem.File.CreateText(OrderByFirstFilename);

		stream.Write(Header);

		foreach (var person in byFirst)
		{
			Progress += step;
			if (_cancelLoading)
			{
				Status = "Rendering directory entries by first name cancelled";
				_cancelLoading = false;
				_loading.Release();
				return;
			}

			if (string.IsNullOrEmpty(person.FirstName) || person.FirstName == ".")
			{
				continue;
			}

			stream.Write(AddEntryByFirst(person, indexArray, basePictureUrl));

			Status = "Completed rendering directory entries by first name";
		}

		stream.Write(Footer);
	}

	public new async Task Loading()
	{
		// we don't want multiple instances of Loading() running simultaneously,
		// so try canceling if one is running and then take the resource
		await Task.Run(() => _loading.WaitOne());

		if (null == _htmlData)
		{
			_htmlData = await _htmlDirectoryServices.GetAsync();
			NonMembers = _htmlData.IncludeNonMembers;
			OrderByLast = _htmlData.OrderByLast;
			OrderByFirst = _htmlData.OrderByFirst;
			OrderByLastFilename = _htmlData.OrderByLastFilename;
			OrderByFirstFilename = _htmlData.OrderByFirstFilename;
			PicturePath = _htmlData.PicturePath;
			Header = _htmlData.Header;
			Template = _htmlData.Template;
			Footer = _htmlData.Footer;
		}

		var length = PicturePath.Length;
		string picturePath = PicturePath;
		if (!string.IsNullOrEmpty(PicturePath) && (PicturePath[length - 1] == '/' || PicturePath[length - 1] == '\\'))
		{
			picturePath = PicturePath.Substring(0, length - 1);
		}

		string body = "";
		if (null != _directoryEntries)
		{
			double total = _directoryEntries.Count;

			double c = 0;
			double step = 1.0 / 2 * total;

			if (OrderByLast)
			{
				OutputByLast(step, picturePath);
			}

			if (OrderByFirst)
			{
				OutputByFirst(step, picturePath);
			}
		}

		_loading.Release();
	}
}
