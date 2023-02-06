using CommunityToolkit.Mvvm.Input;
using Donations.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Donations.ViewModel
{
	/// <summary>
	/// This view model handles the functionality and the binding to the ImportDonorView.xaml which
	/// is a UserControl occupying the 'Import:Donors' tab. This is a view which wants a *.csv file
	/// with specific columns, which it will then import into the Donor database. Once imported, the
	/// collection of donors can then be saved for use throughout the application. This import
	/// will overwrite any existing donor records, so make sure that is what you want to do before
	/// saving.
	/// </summary>
	public class ImportDonorViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObservableCollection<Donor> Collection = new ObservableCollection<Donor>();
		public CollectionViewSource CollectionSource { get; set; } = new CollectionViewSource();

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

		private string _fileName = "";

		/// <summary>
		/// The constructor sets the CollectionViewSource for the imported donors. And it
		/// initializes the SaveCmd to its handler.
		/// </summary>
		public ImportDonorViewModel()
		{
			HasChanges = false;
			CollectionSource.Source = Collection;
		}

		/// <summary>
		/// The SaveCmd property is bound to the 'Save...' button. This button will save the imported
		/// donors, overwriting the prior ones.
		/// </summary>
		public string? Save(bool force)
		{
			string? ret = di.Data.SaveDonors(Collection, force);
			if (null == ret)
			{
				di.Data.ReplaceDonorData(Collection);

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
		public void ReadFile(string filename)
		{
			_fileName = filename;

			Collection.Clear();

			using (StreamReader reader = di.FileSystem.File.OpenText(_fileName))
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
				}

				int lineNumber = 1;

				while (!string.IsNullOrEmpty(line = reader.ReadLine()))
				{
					var split = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
					if (columns.Count == split.Length)
					{
						Donor donor = new Donor();
						donor.Id                 = Helper.ParseInt(lineNumber, split, columns, "Id");
						donor.FamilyId           = Helper.ParseNullableInt(lineNumber, split, columns, "FamilyId");
						donor.FamilyRelationship = Helper.ParseEnum<enumFamilyRelationship>(lineNumber, split, columns, "FamilyRelationship", enumFamilyRelationship.None);
						donor.FirstName          = Helper.ParseString(lineNumber, split, columns, "FirstName");
						donor.PreferredName      = Helper.ParseString(lineNumber, split, columns, "PreferredName");
						donor.LastName           = Helper.ParseString(lineNumber, split, columns, "LastName");
						donor.Gender             = Helper.ParseEnum<enumGender>(lineNumber, split, columns, "Gender", enumGender.Unknown); 
						donor.Email              = Helper.ParseString(lineNumber, split, columns, "Email");
						donor.Email2             = Helper.ParseString(lineNumber, split, columns, "Email2");
						donor.HomePhone          = Helper.ParseString(lineNumber, split, columns, "HomePhone");
						donor.MobilePhone        = Helper.ParseString(lineNumber, split, columns, "MobilePhone");
						donor.WorkPhone          = Helper.ParseString(lineNumber, split, columns, "WorkPhone");
						donor.AddressType        = Helper.ParseEnum<enumAddressType>(lineNumber, split, columns, "AddressType", enumAddressType.Unspecified); 
						donor.Address            = Helper.ParseString(lineNumber, split, columns, "Address");
						donor.Address2           = Helper.ParseString(lineNumber, split, columns, "Address2");
						donor.City               = Helper.ParseString(lineNumber, split, columns, "City");
						donor.State              = Helper.ParseString(lineNumber, split, columns, "State");
						donor.Zip                = Helper.ParseString(lineNumber, split, columns, "Zip");
						donor.Country            = Helper.ParseString(lineNumber, split, columns, "Country");
						donor.AltAddressType     = Helper.ParseEnum<enumAddressType>(lineNumber, split, columns, "AltAddressType", enumAddressType.Unspecified);
						donor.AltAddress         = Helper.ParseString(lineNumber, split, columns, "AltAddress");
						donor.AltAddress2        = Helper.ParseString(lineNumber, split, columns, "AltAddress2");
						donor.AltCity            = Helper.ParseString(lineNumber, split, columns, "AltCity");
						donor.AltState           = Helper.ParseString(lineNumber, split, columns, "AltState");
						donor.AltZip             = Helper.ParseString(lineNumber, split, columns, "AltZip");
						donor.AltCountry         = Helper.ParseString(lineNumber, split, columns, "AltCountry");
						donor.Birthday           = Helper.ParseString(lineNumber, split, columns, "Birthday");
						donor.Baptism            = Helper.ParseString(lineNumber, split, columns, "Baptism");
						donor.Deathday           = Helper.ParseString(lineNumber, split, columns, "Deathday");
						donor.GroupGiving        = Helper.ParseNullableBool(lineNumber, split, columns, "GroupGiving"); 
						donor.ChurchMember       = Helper.ParseNullableBool(lineNumber, split, columns, "ChurchMember");
						donor.MaritalStatus      = Helper.ParseEnum<enumMaritalStatus>(lineNumber, split, columns, "MaritalStatus", enumMaritalStatus.Unknown); 
						donor.Notes              = Helper.ParseString(lineNumber, split, columns, "Notes");
						donor.ActiveGroups       = Helper.ParseString(lineNumber, split, columns, "ActiveGroups");
						donor.LastUpdated        = Helper.ParseString(lineNumber, split, columns, "LastUpdated");

						Collection.Add(donor);
					}
					else
					{
						throw new Exception($"number of fields: {split.Length}, doesn't match headers: {columns.Count}, for line number: {lineNumber}, line: {line}");
					}
					lineNumber++;
				}
			}

			HasChanges = true;
			CollectionSource.View.Refresh();
		}
	}
}
