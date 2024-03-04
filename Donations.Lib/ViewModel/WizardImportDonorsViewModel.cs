using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Threading;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the ImportDonorView.xaml which
/// is a UserControl occupying the 'Import:Donors' tab. This is a view which wants a *.csv file
/// with specific columns, which it will then import into the Donor database. Once imported, the
/// collection of donors can then be saved for use throughout the application. This import
/// will overwrite any existing donor records, so make sure that is what you want to do before
/// saving.
/// </summary>
public partial class WizardImportDonorsViewModel : ObservableObject
{
	private string _fileName = "";
	private long _curr = 0;
	private long _total = 0;
	private DispatcherTimer _timer = new DispatcherTimer();
	private readonly ILogger _logger;
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private readonly IFileSystem _fileSystem;
	private readonly IReflectionHelpers _reflectionHelpers;
	private readonly IDonorServices _donorServices;

	/// <summary>
	/// The constructor sets the CollectionViewSource for the imported donors. And it
	/// initializes the SaveCmd to its handler.
	/// </summary>
	public WizardImportDonorsViewModel(
		ILogger logger,
		IDispatcherWrapper dispatcherWrapper,
		IFileSystem fileSystem,
		IReflectionHelpers reflectionHelpers,
		IDonorServices donorServices
	)
	{
		_logger = logger;
		_dispatcherWrapper = dispatcherWrapper;
		_fileSystem = fileSystem;
		_reflectionHelpers = reflectionHelpers;
		_donorServices = donorServices;

		HasChanges = false;
		CollectionSource.Source = Collection;
		_timer.Tick += new EventHandler(Timer_Tick);
		_timer.Interval = new TimeSpan(0, 0, 1);
	}

	public ObservableCollection<Donor> Collection = new ObservableCollection<Donor>();
	public CollectionViewSource CollectionSource { get; set; } = new CollectionViewSource();

	[ObservableProperty]
	private long _progress;
	/// <summary>
	/// The Process property is bound to a ProgressBar control which will show the progress of
	/// of the import. This is useful because the donation.csv could be pretty large.
	/// </summary>

	[ObservableProperty]
	private string? _status;
	/// <summary>
	/// The Status property will give a text description of the progress for saving.
	/// </summary>

	[ObservableProperty]
	private bool _hasChanges;
	/// <summary>
	/// The HasChanges property tracks the changes which allows the 'Save...' button to
	/// be enabled or disabled accordingly.
	/// </summary>

	private void Timer_Tick(object sender, EventArgs e)
	{
		Progress = (long)(100.0 * _curr / _total + 0.5);

		if (_curr == _total)
			Status = $"Finished uploading {_total} donor records";
		else
			Status = $"Uploading SQL donor record {_curr} of {_total}";
	}

	private void UpdateProgress(long curr, long total)
	{
		_curr = curr;
		_total = total;
	}


	/// <summary>
	/// The SaveCmd property is bound to the 'Save...' button. This button will save the imported
	/// donors, overwriting the prior ones.
	/// </summary>
	public async Task<string?> Save(bool force)
	{
		HasChanges = false;
		Progress = 0;
		_timer.Start();

		string? ret = await _donorServices.SaveDonors(Collection, force, UpdateProgress);
		_timer.Stop();
		Status = ret;

		if (null == ret)
		{
			_curr = _total;
			Timer_Tick(this, new EventArgs());

			_donorServices.ReplaceDonorData(Collection);

			Collection = new ObservableCollection<Donor>();
			CollectionSource.Source = Collection;

			HasChanges = false;
		}

		return ret;

	}

	/// <summary>
	/// This method will read the csv, parsing the rows according to the column headers in the first
	/// row. The import is expecting specific column headers. If yours do not match, the import
	/// cannot be performed. The simple fix is to rename the first row headers in a text editor before
	/// importing.
	/// </summary>
	/// <param name="filename">Filename of the csv file to import.</param>
	/// <exception cref="Exception"></exception>
	public async Task ReadFile(string filename)
	{
		_fileName = filename;

		Collection.Clear();

		await _readFile();

		HasChanges = true;

		CollectionSource.View.Refresh();
	}

	private async Task _readFile()
	{
		using (StreamReader reader = _fileSystem.File.OpenText(_fileName))
		{
			var totalsize = reader.BaseStream.Length;
			string? line = line = reader.ReadLine(); // read column headers
			if (null == line)
				return;

			var headers = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
			var columns = new Dictionary<string, int>();

			for (int i = 0; i < headers?.Length; i++)
			{
				columns[headers[i].Trim('"')] = i;
				await _dispatcherWrapper.Yield();
			}

			int lineNumber = 1;

			while (!string.IsNullOrEmpty(line = reader.ReadLine()))
			{
				var currpos = reader.BaseStream.Position;
				var split = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
				if (columns.Count == split.Length)
				{
					Donor donor = new Donor();
					var a = donor.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
					var properties = _reflectionHelpers.ModelProperties<Donor>(donor);
					foreach (var propinfo in properties)
					{
						if (propinfo.CanWrite)
						{
							if (propinfo.PropertyType == typeof(string))
								propinfo.SetValue(donor, Helper.ParseString(lineNumber, split, columns, propinfo.Name));
							else if (propinfo.PropertyType == typeof(int))
								propinfo.SetValue(donor, Helper.ParseInt(lineNumber, split, columns, propinfo.Name));
							else if (propinfo.PropertyType == typeof(int?))
								propinfo.SetValue(donor, Helper.ParseNullableInt(lineNumber, split, columns, propinfo.Name));
							else if (propinfo.PropertyType == typeof(bool?))
								propinfo.SetValue(donor, Helper.ParseNullableBool(lineNumber, split, columns, propinfo.Name));
							else if (propinfo.PropertyType == typeof(enumFamilyRelationship?))
								propinfo.SetValue(donor, Helper.ParseEnum<enumFamilyRelationship>(lineNumber, split, columns, propinfo.Name, enumFamilyRelationship.None));
							else if (propinfo.PropertyType == typeof(enumGender?))
								propinfo.SetValue(donor, Helper.ParseEnum<enumGender>(lineNumber, split, columns, propinfo.Name, enumGender.Unknown));
							else if (propinfo.PropertyType == typeof(enumAddressType))
								propinfo.SetValue(donor, Helper.ParseEnum<enumAddressType>(lineNumber, split, columns, propinfo.Name, enumAddressType.Unspecified));
							else if (propinfo.PropertyType == typeof(enumAddressType?))
								propinfo.SetValue(donor, Helper.ParseEnum<enumAddressType>(lineNumber, split, columns, propinfo.Name, enumAddressType.Unspecified));
							else if (propinfo.PropertyType == typeof(enumMaritalStatus?))
								propinfo.SetValue(donor, Helper.ParseEnum<enumMaritalStatus>(lineNumber, split, columns, propinfo.Name, enumMaritalStatus.Unknown));
							else if (propinfo.PropertyType == typeof(DateTime?))
							{
								var dt = Helper.ParseString(lineNumber, split, columns, propinfo.Name).Replace(" 00:00:00", "");
								propinfo.SetValue(donor, string.IsNullOrEmpty(dt) ? null : DateTime.Parse(dt));
							}
							else
							{
								Debug.Assert(true);
								_logger.Error($"Property {propinfo.Name} not set");
							}
						}
					}

					_dispatcherWrapper.Invoke(() =>
					{
						Progress = 100 * currpos / totalsize;
						Collection.Add(donor);
					});

					await _dispatcherWrapper.Yield();
				}
				else
				{
					throw new Exception($"number of fields: {split.Length}, doesn't match headers: {columns.Count}, for line number: {lineNumber}, line: {line}");
				}
				lineNumber++;
			}
		}
	}
}
