using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the ImportDonationsView.xaml which
/// is a UserControl occupying the 'Import:Donations' tab. This is a view which wants a *.csv file
/// with specific columns, which it will then import into the Donation database. Once imported, the
/// collection of donations can then be saved for use throughout the application. This import
/// will overwrite any existing donation records, so make sure that is what you want to do before
/// saving.
/// </summary>
public partial class WizardImportDonationsViewModel : ObservableObject
{
	private DispatcherTimer _donationStatusTimer = new DispatcherTimer();
	private DispatcherTimer _batchStatusTimer = new DispatcherTimer();
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private readonly IFileSystem _fileSystem;
	private readonly BatchBrowserViewModel _batchBrowserViewModel;
	private readonly IBatchServices _batchServices;
	private readonly IDonationServices _donationServices;
	private long _curr = 0;
	private long _total = 0;

	/// <summary>
	/// The constructor sets the CollectionViewSource for the imported donations. And it
	/// initializes the SaveCmd to its handler.
	/// </summary>
	public WizardImportDonationsViewModel(
		IDispatcherWrapper dispatcherWrapper,
		IFileSystem fileSystem,
		BatchBrowserViewModel batchBrowserViewModel,
		IBatchServices batchServices,
		IDonationServices donationServices
	)
	{
		_dispatcherWrapper = dispatcherWrapper;
		_fileSystem = fileSystem;
		_batchBrowserViewModel = batchBrowserViewModel;
		_batchServices = batchServices;
		_donationServices = donationServices;

		HasChanges = false;

		_dispatcherWrapper.Invoke(() =>
		{
			CollectionSource.Source = Collection;
		});

		_donationStatusTimer.Tick += new EventHandler(Donations_Progress_Status);
		_donationStatusTimer.Interval = new TimeSpan(0, 0, 1);
		_batchStatusTimer.Tick += new EventHandler(Batch_Progress_Status);
		_batchStatusTimer.Interval = new TimeSpan(0, 0, 1);
	}

	public ObservableCollection<Batch> BatchList = new ObservableCollection<Batch>();
	public Dictionary<int, Batch> BatchDict = new Dictionary<int, Batch>();
	public ObservableCollection<Donation> Collection = new ObservableCollection<Donation>();

	public CollectionViewSource CollectionSource { get; set; } = new CollectionViewSource();

	[ObservableProperty]
	private long _progress;
	/// <summary>
	/// The Process property is bound to a ProgressBar control which will show the progress of
	/// of the import. This is useful because the donation.csv could be pretty large.
	/// </summary>

	[ObservableProperty]
	private string? _startDate;
	/// <summary>
	/// The StartDate property if used is for the purpose of not importing records with donation
	/// dates earlier than the specifid property. If left blank, all records will be imported.
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

	private void Batch_Progress_Status(object sender, EventArgs e)
	{
		Progress = (long)(100.0 * _curr / _total + 0.5);

		if (_curr == _total)
			Status = "Deleting current donations before uploading new imported data. Can take as much as 3-4 minutes.";
		else
			Status = $"Uploading SQL batch record {_curr} of {_total}";
	}

	private void Donations_Progress_Status(object sender, EventArgs e)
	{
		Progress = (long)(100.0 * _curr / _total + 0.5);

		if (_curr == _total)
			Status = $"Finished uploading {_total} donation records";
		else if (0 < _curr)
			Status = $"Uploading SQL donation record {_curr} of {_total}";
	}

	private void UpdateProgress(long curr, long total)
	{
		_curr = curr;
		_total = total;
	}

	/// <summary>
	/// The SaveCmd property is bound to the 'Save...' button. This button will save the imported
	/// donations, overwriting the prior ones.
	/// </summary>
	public async Task<string?> Save(bool force)
	{
		HasChanges = false;
		Progress = 0;
		_batchStatusTimer.Start();
		string? ret = await _batchServices.SaveBatches(BatchList, force, UpdateProgress);
		_batchStatusTimer.Stop();
		_curr = _total;
		Batch_Progress_Status(this, new EventArgs());
		if (!string.IsNullOrEmpty(ret))
			return ret;

		_curr = 0;
		Progress = 0;
		_donationStatusTimer.Start();
		ret = await _donationServices.SaveDonations(Collection, force, UpdateProgress);
		_donationStatusTimer.Stop();
		_curr = _total;
		Donations_Progress_Status(this, new EventArgs());

		if (null == ret)
		{
			await _batchBrowserViewModel!.BatchListUpdated();

			Collection = new ObservableCollection<Donation>();
			_dispatcherWrapper.Invoke(() =>
			{
				CollectionSource.Source = Collection;
			});
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
	public async Task<string> ReadFile(string filename)
	{
		string? message = null;
		DateOnly? startDate = null;
		if (!string.IsNullOrEmpty(StartDate))
		{
			startDate = DateOnly.Parse(StartDate);
		}

		BatchList.Clear();
		Collection.Clear();
		Dictionary<string, Batch> _batchDict = new Dictionary<string, Batch>();

		try
		{
			int id = 1;
			int batchid = 1;

			using (StreamReader reader = _fileSystem.File.OpenText(filename))
			{
				var totalsize = reader.BaseStream.Length;
				string? line = line = reader.ReadLine(); // read column headers
				var headers = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
				var columns = new Dictionary<string, int>();

				for (int i = 0; i < headers?.Length; i++)
				{
					columns[headers[i].Trim('"')] = i;
				}

				// required columns
				if (!columns.ContainsKey("Date"))
				{
					message = $"Donation csv file doesn't have a required \"Date\" column";
					return message;
				}
				if (!columns.ContainsKey("Category"))
				{
					message = $"Donation csv file doesn't have a required \"Category\" column";
					return message;
				}
				if (!columns.ContainsKey("DonorId") && !columns.ContainsKey("LastName") && !columns.ContainsKey("FirstName"))
				{
					message = $"Donation csv file doesn't have either \"DonorId\", \"LastName\" or \"FirstName\" columns";
					return message;
				}

				int lineNumber = 1;
				while (!string.IsNullOrEmpty(line = reader.ReadLine()))
				{
					var currpos = reader.BaseStream.Position;
					var split = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
					if (columns.Count == split.Length)
					{
						string date = split[columns["Date"]].Trim('"');
						DateOnly dateOnly = DateOnly.Parse(date);

						if (!string.IsNullOrEmpty(date) && null != startDate)
						{
							if (startDate >= dateOnly)
							{
								// don't include this record
								continue;
							}
						}

						Donation donation = new Donation();
						donation.DonorId = Helper.ParseInt(lineNumber, split, columns, "DonorId");
						donation.EnvelopeId = Helper.ParseNullableInt(lineNumber, split, columns, "EnvelopeId");
						donation.LastName = Helper.ParseString(lineNumber, split, columns, "LastName");
						donation.FirstName = Helper.ParseString(lineNumber, split, columns, "FirstName");
						donation.Date = Helper.ParseString(lineNumber, split, columns, "Date");
						donation.Category = Helper.ParseString(lineNumber, split, columns, "Category");
						donation.TaxDeductible = Helper.ParseBool(lineNumber, split, columns, true, "TaxDeductible");
						donation.TransactionNumber = Helper.ParseString(lineNumber, split, columns, "TransactionNumber");
						donation.Method = Helper.ParseEnum<enumMethod>(lineNumber, split, columns, "Method", enumMethod.Unknown);
						donation.Note = Helper.ParseString(lineNumber, split, columns, "Note");

						//donation.DonorId = Global.Main.RemapDonorId[donation.DonorId];
						donation.Id = id++;
						var str = split[columns["Value"]].Trim('"');
						if (str[0] == '(' && str.Last() == ')')
						{
							// negative
							donation.Value = -double.Parse(str.Substring(1, str.Length - 2).Trim('$'));
						}
						else
						{
							donation.Value = double.Parse(str.Trim('$'));
						}

						var batchDate = dateOnly.ToString("yyyy/MM/dd");
						donation.Date = batchDate;
						var source = enumSource.DonorInput;
						var key = donation.Date;

						if (donation.Method == enumMethod.AdventistGiving)
						{
							source = enumSource.AdventistGiving;
							key = dateOnly.ToString("MM/yyyy");
							if (dateOnly.Day <= 15)
							{
								key = "1:" + key;
								batchDate = $"{dateOnly.Year.ToString("0000")}/{dateOnly.Month.ToString("00")}/16";
							}
							else
							{
								key = "2:" + key;
								batchDate = $"{dateOnly.Year.ToString("0000")}/{dateOnly.Month.ToString("00")}/{DateTime.DaysInMonth(dateOnly.Year, dateOnly.Month).ToString("00")}";
							}
						}
						if (!_batchDict.ContainsKey(key))
						{
							var batch = new Batch()
							{
								Id = batchid++,
								Source = source,
								Date = batchDate,
								Total = 0,
							};

							BatchList.Add(batch);
							BatchDict[batch.Id] = batch;
							_batchDict[key] = batch;
						}
						donation.BatchId = _batchDict[key].Id;
						_batchDict[key].Total += donation.Value;
						_batchDict[key].ActualTotal = _batchDict[key].Total;

						// this check is for unit testing.
						// since this code is being executed on a task, there is no Application.Current
						_dispatcherWrapper.Invoke(() =>
						{
							Progress = 100 * currpos / totalsize;
							Collection.Add(donation);
						});
					}
					else
					{
						if (MessageBoxResult.Cancel == MessageBox.Show("Problem importing line:", line, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation))
						{
							return null;
						}
					}

					lineNumber++;

					await _dispatcherWrapper.Yield();
				}
			}
		}
		catch (Exception ex)
		{
			message = ex.Message;
		}

		HasChanges = true;

		return message;
	}
}
