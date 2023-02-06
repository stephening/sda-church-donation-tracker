using Donations.ViewModel;
using System;
using System.Windows;

namespace Donations
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Constructor. This reference is saved to Global.Main for other view model use.
		/// </summary>
		public MainWindow()
		{
			#region import data from csv
			/*
			DonorList = new ObservableCollection<Donor>();
			int donorId = 1;

			using (StreamReader reader = new StreamReader(@"individuals.csv"))
			{
				string? line = line = reader.ReadLine(); // read column headers
				var headers = Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
				var columns = new Dictionary<string, int>();

				for (int i = 0; i < headers?.Length; i++)
				{
					columns[headers[i].Trim('"')] = i;
				}

				while (!string.IsNullOrEmpty(line = reader.ReadLine()))
				{
					var split = line.Split(',');
					if (split != null)
					{
						var donor = new Donor();
						string val = "";
						int offset = 0;

						if (split.Length >= 16)
						{
							donor.Id = int.Parse(split[columns["User ID"]].Trim('"'));
							val = split[columns["Family ID"]].Trim('"');
							if (string.IsNullOrEmpty(val)) donor.FamilyId = null;
							else donor.FamilyId = int.Parse(val);
							val = split[columns["Family Relationship"]].Trim('"').Trim().ToLower();
							if (!string.IsNullOrEmpty(val))
							{
								val = Char.ToUpperInvariant(val[0]) + val.Substring(1);
								donor.FamilyRelationship = Enum.Parse<enumFamilyRelationship>(val);
							}
							else donor.FamilyRelationship = enumFamilyRelationship.None;
							donor.FirstName = split[columns["First Name"]].Trim('"').Trim();
							donor.PreferredName = split[columns["Preferred Name"]].Trim('"').Trim();
							donor.LastName = split[columns["Last Name"]].Trim('"').Trim();
							if (donor.LastName == "Kaas")
								donor.LastName = donor.LastName;

							val = split[columns["Gender"] - offset].Trim('"').Trim();
							if (val == "Male") donor.Gender = enumGender.Male;
							else if (val == "Female") donor.Gender = enumGender.Female;
							else donor.Gender = null;
							donor.Email = split[columns["Email"]].Trim('"').Trim();
							donor.Email2 = split[columns["Secondary Email"]].Trim('"').Trim();
							donor.HomePhone = split[columns["Home Phone"]].Trim('"').Trim();
							donor.MobilePhone = split[columns["Cell Phone"]].Trim('"').Trim();
							donor.WorkPhone = split[columns["Work Phone"]].Trim('"').Trim();
							donor.Address = split[columns["Address"]].Trim('"').Trim();
						}

						if (split.Length == 24)
						{
							donor.City = split[columns["City"]].Trim('"').Trim();
							donor.State = split[columns["State"]].Trim('"').Trim();
							donor.Zip = split[columns["Zip Code"]].Trim('"').Trim();
							donor.Country = split[columns["Country"]].Trim('"').Trim();
							val = split[columns["Label"]].Trim('"').Trim();
							if (string.IsNullOrEmpty(val)) donor.AddressType = enumAddressType.Both;
							else if (val.Equals("residential", StringComparison.OrdinalIgnoreCase)) donor.AddressType = enumAddressType.Residential;
							else if (val.Equals("mailing", StringComparison.OrdinalIgnoreCase)) donor.AddressType = enumAddressType.Mailing;
							else donor.AddressType = enumAddressType.Both;
							donor.AltAddress = split[columns["Alt Address"]].Trim('"').Trim();

							// must have split because of a two line alt address
							var line2 = reader.ReadLine();
							var split2 = line2.Split(',');
							offset = columns["Alt Address"];

							donor.AltAddress2 = split2[columns["Alt Address"] - offset].Trim('"').Trim();
							donor.AltCity = split2[columns["Alt City"] - offset].Trim('"').Trim();
							donor.AltState = split2[columns["Alt State"] - offset].Trim('"').Trim();
							donor.AltZip = split2[columns["Alt Zip Code"] - offset].Trim('"').Trim();
							donor.AltCountry = split2[columns["Alt Country"] - offset].Trim('"').Trim();
							val = split[columns["Alt Label"] - offset].Trim('"').Trim();
							if (string.IsNullOrEmpty(donor.AltAddress)) donor.AltAddressType = enumAddressType.Unspecified;
							else if (string.IsNullOrEmpty(val)) donor.AltAddressType = enumAddressType.Both;
							else if (val.Equals("residential", StringComparison.OrdinalIgnoreCase)) donor.AltAddressType = enumAddressType.Residential;
							else if (val.Equals("mailing", StringComparison.OrdinalIgnoreCase)) donor.AltAddressType = enumAddressType.Mailing;
							else donor.AltAddressType = enumAddressType.Both;

							donor.Birthday = split2[columns["Birthday"] - offset].Trim('"').Trim();
							donor.Baptism = split2[columns["Baptism Date"] - offset].Trim('"').Trim();
							donor.Deathday = split2[columns["Died On"] - offset].Trim('"').Trim();
							val = split2[columns["Group Giving With Family"] - offset].Trim('"').Trim();
							if (val == "1") donor.GroupGiving = true;
							else if (val == "0") donor.GroupGiving = false;
							else donor.GroupGiving = null;
							val = split2[columns["Marital Status"] - offset].Trim('"').Trim();
							if (val == "S") donor.MaritalStatus = enumMaritalStatus.Single;
							else if (val == "M") donor.MaritalStatus = enumMaritalStatus.Married;
							else donor.MaritalStatus = null;
							donor.Notes = split2[columns["notes"] - offset].Trim('"').Trim();
							donor.ActiveGroups = String.Join(',', split2[(columns["Active Groups"] - offset)..(split2.Length-1)]).Trim('"');
						}
						else if (split.Length == 16)
						{
							// must have split because of a two line address
							var line2 = reader.ReadLine();
							var split2 = line2.Split(',');
							offset = columns["Address"];

							donor.Address2 = split2[columns["Address"] - offset].Trim('"').Trim();
							donor.City = split2[columns["City"] - offset].Trim('"').Trim();
							donor.State = split2[columns["State"] - offset].Trim('"').Trim();
							donor.Zip = split2[columns["Zip Code"] - offset].Trim('"').Trim();
							donor.Country = split2[columns["Country"] - offset].Trim('"').Trim();
							val = split[columns["Label"] - offset].Trim('"').Trim();
							if (string.IsNullOrEmpty(val)) donor.AddressType = enumAddressType.Both;
							else if (val.Equals("residential", StringComparison.OrdinalIgnoreCase)) donor.AddressType = enumAddressType.Residential;
							else if (val.Equals("mailing", StringComparison.OrdinalIgnoreCase)) donor.AddressType = enumAddressType.Mailing;
							else donor.AddressType = enumAddressType.Both;
							donor.AltAddress = split2[columns["Alt Address"] - offset].Trim('"').Trim();
							donor.AltCity = split2[columns["Alt City"] - offset].Trim('"').Trim();
							donor.AltState = split2[columns["Alt State"] - offset].Trim('"').Trim();
							donor.AltZip = split2[columns["Alt Zip Code"] - offset].Trim('"').Trim();
							donor.AltCountry = split2[columns["Alt Country"] - offset].Trim('"').Trim();
							val = split[columns["Alt Label"] - offset].Trim('"').Trim();
							if (string.IsNullOrEmpty(donor.AltAddress)) donor.AltAddressType = enumAddressType.Unspecified;
							else if (string.IsNullOrEmpty(val)) donor.AltAddressType = enumAddressType.Both;
							else if (val.Equals("residential", StringComparison.OrdinalIgnoreCase)) donor.AltAddressType = enumAddressType.Residential;
							else if (val.Equals("mailing", StringComparison.OrdinalIgnoreCase)) donor.AltAddressType = enumAddressType.Mailing;
							else donor.AltAddressType = enumAddressType.Both;

							donor.Birthday = split2[columns["Birthday"] - offset].Trim('"').Trim();
							donor.Baptism = split2[columns["Baptism Date"] - offset].Trim('"').Trim();
							donor.Deathday = split2[columns["Died On"] - offset].Trim('"').Trim();
							val = split2[columns["Group Giving With Family"] - offset].Trim('"').Trim();
							if (val == "1") donor.GroupGiving = true;
							else if (val == "0") donor.GroupGiving = false;
							else donor.GroupGiving = null;
							val = split2[columns["Marital Status"] - offset].Trim('"').Trim();
							if (val == "S") donor.MaritalStatus = enumMaritalStatus.Single;
							else if (val == "M") donor.MaritalStatus = enumMaritalStatus.Married;
							else donor.MaritalStatus = null;
							donor.Notes = split2[columns["notes"] - offset].Trim('"').Trim();
							donor.ActiveGroups = String.Join(',', split2[(columns["Active Groups"] - offset)..(split2.Length-1)]).Trim('"');
						}
						else if (split[columns["Alt End"]].Trim('"').Trim() == "N/A")
						{
							donor.City = split[columns["City"]].Trim('"').Trim();
							donor.State = split[columns["State"]].Trim('"').Trim();
							donor.Zip = split[columns["Zip Code"]].Trim('"').Trim();
							donor.Country = split[columns["Country"]].Trim('"').Trim();
							val = split[columns["Label"]].Trim('"').Trim();
							if (string.IsNullOrEmpty(val)) donor.AddressType = enumAddressType.Both;
							else if (val.Equals("residential", StringComparison.OrdinalIgnoreCase)) donor.AddressType = enumAddressType.Residential;
							else if (val.Equals("mailing", StringComparison.OrdinalIgnoreCase)) donor.AddressType = enumAddressType.Mailing;
							else donor.AddressType = enumAddressType.Both;
							donor.AltAddress = split[columns["Alt Address"]].Trim('"').Trim();
							donor.AltCity = split[columns["Alt City"]].Trim('"').Trim();
							donor.AltState = split[columns["Alt State"]].Trim('"').Trim();
							donor.AltZip = split[columns["Alt Zip Code"]].Trim('"').Trim();
							donor.AltCountry = split[columns["Alt Country"]].Trim('"').Trim();
							val = split[columns["Alt Label"]].Trim('"').Trim();
							if (string.IsNullOrEmpty(donor.AltAddress)) donor.AltAddressType = enumAddressType.Unspecified;
							else if (string.IsNullOrEmpty(val)) donor.AltAddressType = enumAddressType.Both;
							else if (val.Equals("residential", StringComparison.OrdinalIgnoreCase)) donor.AltAddressType = enumAddressType.Residential;
							else if (val.Equals("mailing", StringComparison.OrdinalIgnoreCase)) donor.AltAddressType = enumAddressType.Mailing;
							else donor.AltAddressType = enumAddressType.Both;
							donor.Birthday = split[columns["Birthday"]].Trim('"').Trim();
							donor.Baptism = split[columns["Baptism Date"]].Trim('"').Trim();
							donor.Deathday = split[columns["Died On"]].Trim('"').Trim();
							val = split[columns["Group Giving With Family"]].Trim('"').Trim();
							if (val == "1") donor.GroupGiving = true;
							else if (val == "0") donor.GroupGiving = false;
							else donor.GroupGiving = null;
							val = split[columns["Marital Status"]].Trim('"').Trim();
							if (val == "S") donor.MaritalStatus = enumMaritalStatus.Single;
							else if (val == "M") donor.MaritalStatus = enumMaritalStatus.Married;
							else donor.MaritalStatus = null;
							donor.Notes = split[columns["notes"]].Trim('"').Trim();
							donor.ActiveGroups = String.Join(',', split[columns["Active Groups"]..(split.Length-1)]).Trim('"');
						}
						else
						{
							throw new Exception();
						}
						if ((donor.AddressType == enumAddressType.Both || donor.AddressType == enumAddressType.Mailing)
							&& (donor.AltAddressType == enumAddressType.Both || donor.AltAddressType == enumAddressType.Mailing))
						{
							// alt address type cannot be both or mailing
							donor.AltAddressType = enumAddressType.Unspecified;
						}

						if (0 <= Array.IndexOf(donor.ActiveGroups.Split(','), "Members"))
						{
							donor.ChurchMember = true;
						}
						else
						{
							donor.ChurchMember = false;
						}

						// convert state names to abbrev if possible
						if (donor.State.Length >= 2)
						{
							foreach (var state in Global.StateAbbrMapping)
							{
								if (donor.State.Equals(state.Value, StringComparison.OrdinalIgnoreCase))
								{
									donor.State = state.Key;
								}
							}
						}

						// convert state names to abbrev if possible
						if (donor.AltState?.Length >= 2)
						{
							foreach (var state in Global.StateAbbrMapping)
							{
								if (donor.AltState.Equals(state.Value, StringComparison.OrdinalIgnoreCase))
								{
									donor.AltState = state.Key;
								}
							}
						}
						//RemapDonorId[donor.Id] = donorId;
						//donor.Id = donorId++;
						DonorDict[donor.Id] = donor;
						DonorList.Add(donor);
					}
				}
			}
			di.Data.SaveDonors(DonorList);
			di.Data.ExportCsv<Donor>(@"donors.csv", DonorList);
			*/
			#endregion

			InitializeComponent();

			LoadSettings();
		}

		private string AddCsv(object field, bool addComma = true)
		{
			string comma = ",";
			if (!addComma) comma = "";

			if (null == field)
			{
				return $"{comma}";
			}
			else if (true == field.ToString()?.Contains(','))
			{
				// srruound with quotes
				return $"\"{field}\"{comma}";
			}
			else
			{
				return $"{field}{comma}";
			}
		}
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
		}

		private void LoadSettings()
		{
			Top = Persist.Default.Top;
			Left = Persist.Default.Left;
			Width = Persist.Default.Width;
			Height = Persist.Default.Height;
			if (!string.IsNullOrEmpty(Persist.Default.WindowState))
				WindowState = Enum.Parse<WindowState>(Persist.Default.WindowState);
		}

		private void SaveSettings()
		{
			Persist.Default.Top = Top;
			Persist.Default.Left = Left;
			Persist.Default.Width = Width;
			Persist.Default.Height = Height;
			Persist.Default.WindowState = WindowState.ToString();

			Persist.Default.Save();
		}

		private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveSettings();

			di.Data.SaveData();
		}

		private void MaintenanceTab_Selected(object sender, RoutedEventArgs e)
		{
			(CategoryView.DataContext as CategoryViewModel)?.Loaded();
			(CategoryMapView.DataContext as CategoryMapViewModel)?.Loaded();
			(DonorMapView.DataContext as DonorMapViewModel)?.Loaded();
		}

		private void DonorInputTab_Selected(object sender, RoutedEventArgs e)
		{
			(DonorInputView.DataContext as DonorInputViewModel)?.Loaded();
			//DonorInputView.BatchTotalTextBox.Focus();
		}
	}
}
