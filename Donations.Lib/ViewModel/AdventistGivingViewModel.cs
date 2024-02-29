using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the AdventistGivingView.xaml. This is the top level
/// page which doesn't do much besides handling the batch date and note. Most of the work is done by the user controls
/// under the tab items, and their view models. There are three tabs:
///   - Donor resolution
///   - Category resolution
///   - Verify and submit
/// Both the resolution tabs run in parallel and you can go back an forth between them until thay are both resolved.
/// When both resolved, the verify tab will show a list of categories and their totals which should match up with the
/// Adventist Giving report summary provided.
/// </summary>
public partial class AdventistGivingViewModel : BaseViewModel
{
	public ObservableCollection<AdventistGiving>? TransactionList { get; set; }

	[ObservableProperty]
	private string _batchNote = "";
	/// <summary>
	/// The BatchNode property gets transferred to the Note member and will show up in a column in the batch browser.
	/// </summary>

	[ObservableProperty]
	private double _targetTotal;
	/// <summary>
	/// The TargetTotal property should contain the expected total from the batch being imported. When the batch import
	/// is complete, there will be a computed total which will also be saved with the batch. If the two totals don't
	/// match, the will be highlighted in red in the batch browser view.
	/// </summary>

	private IFileSystem _fileSystem;
	public AGDonorResolutionViewModel AGDonorResolutionViewModel { get; set; }
	public AGCategoryResolutionViewModel AGCategoryResolutionViewModel { get; set; }
	public AGDonationSummaryViewModel AGDonationSummaryViewModel { get; set; }

	public Dictionary<int, Donor> _quickDonorLookup = new Dictionary<int, Donor>();

	/// <summary>
	/// Constructor. This reference is saved to Global.AG for other view model use.
	/// </summary>
	public AdventistGivingViewModel(
		IFileSystem fileSystem,
		AGDonorResolutionViewModel aGDonorResolutionViewModel,
		AGCategoryResolutionViewModel aGCategoryResolutionViewModel,
		AGDonationSummaryViewModel aGDonationSummaryViewModel
		)
	{
		_fileSystem = fileSystem;
		AGDonorResolutionViewModel = aGDonorResolutionViewModel;
		AGDonorResolutionViewModel!.SetParentViewModel(this);
		AGCategoryResolutionViewModel = aGCategoryResolutionViewModel;
		AGCategoryResolutionViewModel!.SetParentViewModel(this);
		AGDonationSummaryViewModel = aGDonationSummaryViewModel;
		AGDonationSummaryViewModel!.SetParentViewModel(this);
	}

	/// <summary>
	/// import task which is called from the Adventist Giving tab to import a batch of
	/// donations through the Adventist Giving (AG) system. This is an async function so the
	/// UI thread is not blocked when it is running. After importing the AG records, the
	/// name resolution is kicked off by calling the StartNameResolution() method.
	/// </summary>
	/// <param name="filePath">File path to the *.csv file</param>
	/// <returns></returns>
	public async Task Import(string? filePath)
	{
		if (string.IsNullOrEmpty(filePath))
			throw new Exception("Null or empty filename passed to Import");

		await Task.Run(() =>
		{
			TransactionList = ImportCsv(filePath);

			AGDonorResolutionViewModel?.StartNameResolution();
			AGCategoryResolutionViewModel?.StartCategoryResolution();
		});
	}

	/// <summary>
	/// Called after Submit() to return this tab to it's original ready state.
	/// </summary>
	public void Reset()
	{
		TransactionList?.Clear();
		BatchNote = "";
		TargetTotal = 0;
		AGDonorResolutionViewModel?.ResolutionComplete(false);
		AGCategoryResolutionViewModel?.ResolutionComplete(false);
	}


	/// <summary>
	/// Safely trim white space event if the string is null.
	/// </summary>
	/// <param name="s">Parameter to trim</param>
	/// <returns>The trimmed string</returns>
	private string Trim(string? s)
	{
		return string.IsNullOrEmpty(s) ? "" : s.Trim();
	}

	/// <summary>
	/// This function will import from an Adventist giving csv and return an
	/// a list of AdventistGiving objects.
	/// </summary>
	/// <param name="filePath"></param>
	/// <returns>ObservableCollection<AdventistGiving></returns>
	/// <exception cref="Exception">If the ',' delimited split of a line
	/// doesn't have the same number of elements as the header (first row)
	/// </exception>
	private ObservableCollection<AdventistGiving>? ImportCsv(string path)
	{
		ObservableCollection<AdventistGiving>? collection = new ObservableCollection<AdventistGiving>();

		using (var reader = _fileSystem.File.OpenText(path))
		{
			string? line = reader.ReadLine(); // read column headers
			if (null == line)
			{
				return collection;
			}

			var headers = line?.Split(',').Select(x => x.Trim()).ToArray();
			if (null == headers)
			{
				throw new Exception("Split(',') of {line} returned null");
			}

			while (!string.IsNullOrEmpty(line = reader.ReadLine()))
			{
				// https://stackoverflow.com/questions/3147836/c-sharp-regex-split-commas-outside-quotes
				var split = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)").Select(x => x.Trim()).ToArray();
				if (split != null && split.Length == headers.Length)
				{
					var record = new AdventistGiving();

					for (int i = 0; i < headers?.Length; i++)
					{
						if (headers[i].Equals("First Name"))
						{
							record.FirstName = Trim(split[i]);
						}
						else if (headers[i].Equals("Last Name"))
						{
							record.LastName = Trim(split[i]);
						}
						else if (headers[i].Equals("Address1"))
						{
							record.Address = Trim(split[i]);
						}
						else if (headers[i].Equals("Address2"))
						{
							record.Address2 = Trim(split[i]);
						}
						else if (headers[i].Equals("City"))
						{
							record.City = Trim(split[i]);
						}
						else if (headers[i].Equals("State"))
						{
							record.State = Trim(split[i]);
						}
						else if (headers[i].Equals("Postal Code"))
						{
							record.Zip = Trim(split[i]);
						}
						else if (headers[i].Equals("Country"))
						{
							record.Country = Trim(split[i]);
						}
						else if (headers[i].Equals("Transaction ID"))
						{
							record.TransactionId = Trim(split[i]);
						}
						else if (headers[i].Equals("Transaction Type"))
						{
							record.TransactionType = Trim(split[i]);
						}
						else if (headers[i].Equals("Transaction Created At"))
						{
							record.TransactionDate = split[i];
						}
						else if (headers[i].Equals("Transaction Total"))
						{
							record.TransactionTotal = double.Parse(split[i]);
						}
						else if (headers[i].Equals("Code"))
						{
							record.CategoryCode = int.Parse(split[i]);
						}
						else if (headers[i].Equals("Category Name"))
						{
							record.CategoryName = Trim(split[i]).Trim('"');
						}
						else if (headers[i].Equals("Amount"))
						{
							record.Amount = double.Parse(split[i]);
						}
					}

					record.DonorHash = Helper.AGHash(record);

					collection.Add(record);
				}
				else
				{
					throw new Exception($"Line split into {split.Length} but should be {headers.Length}.");
				}
			}
		}

		return collection;
	}
}
