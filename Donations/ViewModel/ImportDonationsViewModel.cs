using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Donations.Model;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows;

namespace Donations.ViewModel
{
	/// <summary>
	/// This view model handles the functionality and the binding to the ImportDonationsView.xaml which
	/// is a UserControl occupying the 'Import:Donations' tab. This is a view which wants a *.csv file
	/// with specific columns, which it will then import into the Donation database. Once imported, the
	/// collection of donations can then be saved for use throughout the application. This import
	/// will overwrite any existing donation records, so make sure that is what you want to do before
	/// saving.
	/// </summary>
	public class ImportDonationsViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObservableCollection<Donation> Collection = new ObservableCollection<Donation>();
		public CollectionViewSource CollectionSource { get; set; } = new CollectionViewSource();

		public ObservableCollection<Batch> BatchList = new ObservableCollection<Batch>();
		public Dictionary<int, Batch> BatchDict = new Dictionary<int, Batch>();

		private long _progress;
		/// <summary>
		/// The Process property is bound to a ProgressBar control which will show the progress of
		/// of the import. This is useful because the donation.csv could be pretty large.
		/// </summary>
		public long Progress
		{
			get { return _progress; }
			set
			{
				_progress = value;
				OnPropertyChanged();
			}
		}

		private string? _startDate;
		/// <summary>
		/// The StartDate property if used is for the purpose of not importing records with donation
		/// dates earlier than the specifid property. If left blank, all records will be imported.
		/// </summary>
		public string? StartDate
		{
			get { return _startDate; }
			set
			{
				_startDate = value;
				OnPropertyChanged();
			}
		}

		private bool _hasChanges;
		/// <summary>
		/// The HasChanges property tracks the changes which allows the 'Save...' button to
		/// be enabled or disabled accordingly.
		/// </summary>
		public bool HasChanges
		{
			get { return _hasChanges; }
			set
			{
				_hasChanges = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The constructor sets the CollectionViewSource for the imported donations. And it
		/// initializes the SaveCmd to its handler.
		/// </summary>
		public ImportDonationsViewModel()
		{
			HasChanges = false;

			if (null != Application.Current)
			{
				// Don't need this because we if we are running a unit test, we have no view
				CollectionSource.Source = Collection;
			}
		}

		/// <summary>
		/// The SaveCmd property is bound to the 'Save...' button. This button will save the imported
		/// donations, overwriting the prior ones.
		/// </summary>
		public string? Save(bool force)
		{
			string? ret = di.Data.SaveDonations(Collection, force);
			if (null == ret)
			{
				di.Data.SaveBatches(BatchList);

				di.Data.ReplaceDonationData(Collection, BatchList);

				Collection = new ObservableCollection<Donation>();
				if (null != Application.Current)
				{
					// Don't need this because we if we are running a unit test, we have no view
					CollectionSource.Source = Collection;
				}

				HasChanges = false;
				Progress = 0;
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

			Collection.Clear();
			Dictionary<string, Batch> _batchDict = new Dictionary<string, Batch>();

			HasChanges = true;

			await Task.Run((Action)(() =>
			{
				try
				{
					int id = 1;
					int batchid = 1;

					using (StreamReader reader = di.FileSystem.File.OpenText(filename))
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
							return;
						}
						if (!columns.ContainsKey("Category"))
						{
							message = $"Donation csv file doesn't have a required \"Category\" column";
							return;
						}
						if (!columns.ContainsKey("DonorId") && !columns.ContainsKey("LastName") && !columns.ContainsKey("FirstName"))
						{
							message = $"Donation csv file doesn't have either \"DonorId\", \"LastName\" or \"FirstName\" columns";
							return;
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
								/*donation.Method = enumMethod.Unknown;
								if (donation.TransactionNumber.Equals("cash", StringComparison.OrdinalIgnoreCase))
								{
									donation.Method = enumMethod.Cash;
									donation.TransactionNumber = null;
								}
								else if (string.IsNullOrEmpty(donation.TransactionNumber) && string.IsNullOrEmpty(donation.Note) && donation.LastName.Equals("loose offering", StringComparison.OrdinalIgnoreCase))
								{
									donation.Method = enumMethod.Cash;
									donation.TransactionNumber = null;
								}

								if (!string.IsNullOrEmpty(donation.Note) && donation.Note.Equals("adventist giving", StringComparison.OrdinalIgnoreCase))
								{
									donation.Method = enumMethod.AdventistGiving;
								}
								if (string.IsNullOrEmpty(donation.Note) && !string.IsNullOrEmpty(donation.TransactionNumber) && !donation.TransactionNumber.StartsWith("**"))
								{
									donation.Method = enumMethod.Check;
								}
								if (donation.Method == enumMethod.Unknown && !string.IsNullOrEmpty(donation.TransactionNumber) && !string.IsNullOrEmpty(donation.Note) && !donation.Note.Contains("cash", StringComparison.OrdinalIgnoreCase))
								{
									donation.Method = enumMethod.Check;
								}
								if (donation.Method == enumMethod.Unknown && string.IsNullOrEmpty(donation.TransactionNumber) && !string.IsNullOrEmpty(donation.Note) && donation.Note.Contains("cash", StringComparison.OrdinalIgnoreCase))
								{
									donation.Method = enumMethod.Cash;
								}
								if (donation.Method == enumMethod.Unknown && !string.IsNullOrEmpty(donation.TransactionNumber) && !string.IsNullOrEmpty(donation.Note) && donation.Note.Contains("cash", StringComparison.OrdinalIgnoreCase))
								{
									donation.Method = enumMethod.Mixed;
								}
								if (donation.Method == enumMethod.Unknown && string.IsNullOrEmpty(donation.TransactionNumber) && !string.IsNullOrEmpty(donation.Note) && donation.Note.Contains("online", StringComparison.OrdinalIgnoreCase))
								{
									donation.Method = enumMethod.Online;
								}
								if (!string.IsNullOrEmpty(donation.TransactionNumber) && donation.TransactionNumber.StartsWith("**"))
								{
									donation.Method = enumMethod.Card;
								}
								*/
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
								if (null != Application.Current)
								{
									Application.Current.Dispatcher.Invoke(() =>
									{
										Progress = 100 * currpos / totalsize;
										Collection.Add(donation);
									});
								}
								else
								{
									// even though we are running on a different thread than the CollectionViewSource
									// was created on, we didn't set it's Source to Collection (see constructor), so this is ok
									Progress = 100 * currpos / totalsize;
									Collection.Add(donation);
								}
							}
							else
							{
								if (MessageBoxResult.Cancel == MessageBox.Show("Problem importing line:", line, MessageBoxButton.OKCancel, MessageBoxImage.Exclamation))
								{
									return;
								}
							}

							lineNumber++;
						}
					}
				}
				catch (Exception ex)
				{
					message = ex.Message;
				}
			}));

			HasChanges = true;

			return message;
		}
	}
}
