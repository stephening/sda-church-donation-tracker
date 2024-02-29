using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the ImportCategoriesView.xaml which
/// is a UserControl occupying the 'Import:Categories' tab. This is a view which wants a *.csv file
/// with specific columns, which it will then import into the Category object. Once imported, the
/// collection of categories can then be saved for use throughout the application. This import
/// will overwrite any existing categories, so make sure that is what you want to do before saving.
/// </summary>
public partial class WizardImportCategoriesViewModel : ObservableObject
{
	public ObservableCollection<Category> Collection = new ObservableCollection<Category>();
	public CollectionViewSource CollectionSource { get; set; } = new CollectionViewSource();

	[ObservableProperty]
	private bool _hasChanges;
	/// <summary>
	/// The HasChanges property tracks the changes which allows the 'Save...' button to
	/// be enabled or disabled accordingly.
	/// </summary>

	private string _fileName;
	private readonly IFileSystem _fileSystem;
	private readonly ICategoryServices _categoryServices;

	/// <summary>
	/// The constructor sets the CollectionViewSource for the imported categories. And it
	/// initializes the SaveCmd to its handler.
	/// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public WizardImportCategoriesViewModel(
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		IFileSystem fileSystem,
		ICategoryServices categoryServices
	)
	{
		_fileSystem = fileSystem;
		_categoryServices = categoryServices;

		HasChanges = false;
		CollectionSource.Source = Collection;
	}

	/// <summary>
	/// The SaveCmd property is bound to the 'Save...' button. This button will save the new
	/// categories, overwriting the prior ones.
	/// </summary>
	public async Task<string?> Save(bool force)
	{
		string? ret = await _categoryServices.SaveCategories(Collection, force);
		if (null == ret)
		{
			HasChanges = false;
		}

		return ret;
	}

	/// <summary>
	/// This method will read the csv, parsing the rows according to the column headers in the first
	/// row. The import is expecting three specific column headers. If yours do not match, the import
	/// cannot be performed. The simple fix is to rename the first row headers in a text editor before
	/// importing.
	/// </summary>
	/// <param name="filename">Filename of the csv file to import.</param>
	/// <exception cref="Exception"></exception>
	public void ReadFile(string filename)
	{
		_fileName = filename;

		Collection.Clear();

		using (StreamReader reader = _fileSystem.File.OpenText(_fileName))
		{
			var totalsize = reader.BaseStream.Length;
			string? line = line = reader.ReadLine(); // read column headers
#pragma warning disable CS8604 // Possible null reference argument.
			var headers = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
#pragma warning restore CS8604 // Possible null reference argument.
			var columns = new Dictionary<string, int>();

			for (int i = 0; i < headers?.Length; i++)
			{
				columns[headers[i].Trim().Trim('"')] = i;
			}

			// required columns
			if (!columns.ContainsKey("Code"))
			{
				throw new Exception($"Donation csv file doesn't have a required \"Id\" column");
			}
			if (!columns.ContainsKey("Description"))
			{
				throw new Exception($"Donation csv file doesn't have a required \"Description\" column");
			}
			if (!columns.ContainsKey("TaxDeductible"))
			{
				throw new Exception($"Donation csv file doesn't have a required \"TaxDeductible\" column");
			}

			int lineNumber = 1;

			while (!string.IsNullOrEmpty(line = reader.ReadLine()))
			{
				var split = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
				if (columns.Count == split.Length)
				{
					Category category = new Category();
					category.Code = Helper.ParseInt(lineNumber, split, columns, "Code");
					category.Description = Helper.ParseString(lineNumber, split, columns, "Description");
					category.TaxDeductible = Helper.ParseBool(lineNumber, split, columns, true, "TaxDeductible");

					Collection.Add(category);
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

		HasChanges = true;
		CollectionSource.View.Refresh();
	}
}