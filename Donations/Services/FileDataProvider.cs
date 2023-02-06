using Donations.Interfaces;
using Donations.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace Donations.Services
{
    public class FileDataProvider : IData
    {
		public string CategoriesFileName => "categories.xml";
		public string DonorsFileName => "donors.xml";
		public string DonationsFileName => "donations.xml";
		public string BatchesFileName => "batches.xml";
		public string DonorMapFileName => "donor_map.xml";
		public string CategoryMapFileName => "category_map.xml";
		public string EnvelopeDesignFileName => "envelope_design.xml";
		public ObservableCollection<Donor>? DonorList { get; set; } = new ObservableCollection<Donor>();
		public Dictionary<int, Donor>? DonorDict { get; set; } = new Dictionary<int, Donor>();
		public ObservableCollection<Donation>? DonationList { get; set; } = new ObservableCollection<Donation>();
		public Dictionary<int, Donation>? DonationDict { get; set; } = new Dictionary<int, Donation>();
		public ObservableCollection<Batch>? BatchList { get; set; } = new ObservableCollection<Batch>();
		public Dictionary<int, Batch>? BatchDict { get; set; } = new Dictionary<int, Batch>();
		public ObservableCollection<EnvelopeEntry>? TitheEnvelopeDesign { get; set; } = new ObservableCollection<EnvelopeEntry>();
		public Dictionary<int, Category>? CatDict { get; set; } = new Dictionary<int, Category>();
		public ObservableCollection<Category>? CatList { get; set; } = new ObservableCollection<Category>();
		public ObservableCollection<AGDonorMapItem>? AGDonorMapList { get; set; } = new ObservableCollection<AGDonorMapItem>();
		public Dictionary<string, AGDonorMapItem>? AGDonorMap { get; set; } = new Dictionary<string, AGDonorMapItem>();
		public ObservableCollection<AGCategoryMapItem>? AGCategoryMapList { get; set; } = new ObservableCollection<AGCategoryMapItem>();
		public Dictionary<int, AGCategoryMapItem>? AGCategoryMap { get; set; } = new Dictionary<int, AGCategoryMapItem>();

		public FileDataProvider()
		{
			if (null == DonorDict)
			{
				throw new OutOfMemoryException("DonorDict is null");
			}

			if (null == DonationDict)
			{
				throw new OutOfMemoryException("DonationDict is null");
			}

			if (null == BatchDict)
			{
				throw new OutOfMemoryException("BatchDict is null");
			}

			if (null == AGCategoryMapList)
			{
				throw new OutOfMemoryException("AdventistGivingCategoryMapList is null");
			}

			if (null == AGDonorMapList)
			{
				throw new OutOfMemoryException("AdventistGivingDonorMapList is null");
			}

			if (null == DonorList)
			{
				throw new OutOfMemoryException("DonorList is null");
			}

			if (null == DonationList)
			{
				throw new OutOfMemoryException("DonationList is null");
			}

			if (null == BatchList)
			{
				throw new OutOfMemoryException("BatchList is null");
			}

			if (null == AGCategoryMap)
			{
				throw new OutOfMemoryException("AdventistGivingCategoryMap is null");
			}

			if (null == AGDonorMap)
			{
				throw new OutOfMemoryException("AdventistGivingDonorMap is null");
			}

			if (null == TitheEnvelopeDesign)
			{
				throw new OutOfMemoryException("TitheEnvelopeDesign is null");
			}
		}

		public void LoadData()
		{
			string fileName = EnvelopeDesignFileName;

			try
			{
				// first backup
				string backupFolder = DateTime.Now.ToString("yyyyMMdd-HHmmss") + " Data backup";

				di.FileSystem.Directory.CreateDirectory(backupFolder);

				lock (TitheEnvelopeDesign)
				lock(EnvelopeDesignFileName)
				{
					if (di.FileSystem.File.Exists(fileName))
					{
						di.FileSystem.File.Copy(fileName, $"{backupFolder}\\{fileName}");

						TitheEnvelopeDesign = DeserializeXml<EnvelopeEntry>(fileName);
					}
				}

				lock (DonorList)
				lock (DonorsFileName)
				{
					fileName = DonorsFileName;
					if (di.FileSystem.File.Exists(fileName))
					{
						di.FileSystem.File.Copy(fileName, $"{backupFolder}\\{fileName}");

						DonorList = DeserializeXml<Donor>(fileName);
						foreach (var donor in DonorList)
						{
							DonorDict[donor.Id] = donor;
						}
					}
				}

				lock (DonationList)
				lock (DonationsFileName)
				{
					fileName = DonationsFileName;
					if (di.FileSystem.File.Exists(fileName))
					{
						di.FileSystem.File.Copy(fileName, $"{backupFolder}\\{fileName}");

						DonationList = DeserializeXml<Donation>(fileName);
						foreach (var donation in DonationList)
						{
							DonationDict[donation.Id] = donation;
						}
					}
				}

				lock (BatchList)
				lock (BatchesFileName)
				{
					fileName = BatchesFileName;
					if (di.FileSystem.File.Exists(fileName))
					{
						di.FileSystem.File.Copy(fileName, $"{backupFolder}\\{fileName}");

						BatchList = DeserializeXml<Batch>(fileName);
						foreach (var batch in BatchList)
						{
							BatchDict[batch.Id] = batch;
						}
					}
				}

				lock (CatList)
				lock (CategoriesFileName)
				{
					fileName = CategoriesFileName;
					if (di.FileSystem.File.Exists(fileName))
					{
						di.FileSystem.File.Copy(fileName, $"{backupFolder}\\{fileName}");

						CatList = DeserializeXml<Category>(fileName);
						foreach (var category in CatList)
						{
							CatDict[category.Code] = category;
						}
					}
				}

				lock (AGCategoryMapList)
				lock (CategoryMapFileName)
				{
					fileName = CategoryMapFileName;
					if (di.FileSystem.File.Exists(fileName))
					{
						di.FileSystem.File.Copy(fileName, $"{backupFolder}\\{fileName}");

						AGCategoryMapList = DeserializeXml<AGCategoryMapItem>(fileName);
						foreach (var catmap in AGCategoryMapList)
						{
							AGCategoryMap[catmap.CategoryCode] = catmap;
						}
					}
					else
					{
						fileName = "categorymap.json";

						if (di.FileSystem.File.Exists(fileName))
						{
							di.FileSystem.File.Copy(fileName, $"{backupFolder}\\{fileName}");

							AGCategoryMap = DeserializeJson<int, AGCategoryMapItem>(fileName);
							foreach (var entry in AGCategoryMap)
							{
								AGCategoryMapList?.Add(entry.Value);
							}
						}
					}
				}

				lock (AGDonorMapList)
				lock (DonorMapFileName)
				{
					fileName = DonorMapFileName;
					if (di.FileSystem.File.Exists(fileName))
					{
						di.FileSystem.File.Copy(fileName, $"{backupFolder}\\{fileName}");

						AGDonorMapList = DeserializeXml<AGDonorMapItem>(fileName);
						foreach (var donormap in AGDonorMapList)
						{
							AGDonorMap[donormap.AGDonorHash] = donormap;
						}
					}
					else
					{
						fileName = "donormap.json";
						if (di.FileSystem.File.Exists(fileName))
						{
							di.FileSystem.File.Copy(fileName, $"{backupFolder}\\{fileName}");

							AGDonorMap = DeserializeJson<string, AGDonorMapItem>("donormap.json");
							foreach (var entry in AGDonorMap)
							{
								AGDonorMapList?.Add(entry.Value);
							}
						}
					}
				}

				if (null == DonorList)
				{
					throw new OutOfMemoryException("DonorList is null");
				}

				if (null == DonationList)
				{
					throw new OutOfMemoryException("DonationList is null");
				}

				if (null == BatchList)
				{
					throw new OutOfMemoryException("BatchList is null");
				}

				if (null == AGCategoryMap)
				{
					throw new OutOfMemoryException("AdventistGivingCategoryMap is null");
				}

				if (null == AGDonorMap)
				{
					throw new OutOfMemoryException("AdventistGivingDonorMap is null");
				}

				if (null == TitheEnvelopeDesign)
				{
					throw new OutOfMemoryException("TitheEnvelopeDesign is null");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred while trying to load \"{fileName}\". Please try to replace that file with a backup.", "Unexpected error loading data", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				Application.Current.Shutdown();
			}
		}

		public string? SaveDonors(ObservableCollection<Donor> donors, bool force = false)
		{
			try
			{
				if (!force && di.FileSystem.File.Exists(DonorsFileName))
				{
					return $"There is an existing {DonorsFileName} file, are you sure you want to overwrite it?";
				}

				lock (DonorsFileName)
				{
					SerializeXml<Donor>(DonorsFileName, donors);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Unexpected error saving donor data to file", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}

			return null;
		}

		public string? SaveDonations(ObservableCollection<Donation> donations, bool force = false)
		{
			try
			{
				if (!force && di.FileSystem.File.Exists(DonationsFileName))
				{
					return $"There is an existing {DonationsFileName} file, are you sure you want to overwrite it?";
				}

				lock (DonationsFileName)
				{
					SerializeXml<Donation>(DonationsFileName, donations);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Unexpected error saving donation data to file", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}

			return null;
		}

		public void SaveBatches(ObservableCollection<Batch> batch)
		{
			try
			{
				lock (BatchesFileName)
				{
					SerializeXml<Batch>(BatchesFileName, batch);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Unexpected error saving batch data to file", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		public string? SaveCategories(ObservableCollection<Category> categories, bool force = false)
		{
			try
			{
				if (!force && di.FileSystem.File.Exists(CategoriesFileName))
				{
					return $"There is an existing {CategoriesFileName} file, are you sure you want to overwrite it?";
				}

				lock (CategoriesFileName)
				{
					SerializeXml<Category>(CategoriesFileName, categories);
				}
			}
			catch (Exception ex)
			{
				return "Unexpected error saving category data to file";
			}

			return null;
		}

		public void SaveData()
		{
			try
			{
				lock (DonorList)
				lock (DonorsFileName)
				{
					SerializeXml<Donor>(DonorsFileName, DonorList);
				}

				lock (DonationList)
				lock (DonationsFileName)
				{
					SerializeXml<Donation>(DonationsFileName, DonationList);
				}

				lock (BatchList)
				lock (BatchesFileName)
				{
					SerializeXml<Batch>(BatchesFileName, BatchList);
				}

				lock (TitheEnvelopeDesign)
				lock (EnvelopeDesignFileName)
				{
					SerializeXml<EnvelopeEntry>(EnvelopeDesignFileName, TitheEnvelopeDesign);
				}

				lock (AGCategoryMapList)
				lock (CategoryMapFileName)
				{
					SerializeXml<AGCategoryMapItem>(CategoryMapFileName, AGCategoryMapList);
				}

				lock (AGDonorMapList)
				lock (DonorMapFileName)
				{
					SerializeXml<AGDonorMapItem>(DonorMapFileName, AGDonorMapList);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Unexpected error saving data", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		public void ReplaceDonorData(ObservableCollection<Donor> donorList)
		{
			lock (DonorList)
			{
				DonorList?.Clear();
				DonorDict?.Clear();

				foreach (var donor in donorList)
				{
					DonorList?.Add(donor);
					DonorDict[donor.Id] = donor;
				}
			}
		}

		public void ReplaceDonationData(ObservableCollection<Donation> donationList, ObservableCollection<Batch> batchList)
		{
			lock (DonationList)
			{
				DonationList?.Clear();
				DonationDict?.Clear();

				foreach (var donation in donationList)
				{
					DonationList?.Add(donation);
					DonationDict[donation.Id] = donation;
				}
			}

			lock (BatchList)
			{
				BatchList?.Clear();
				BatchDict?.Clear();

				foreach (var batch in batchList)
				{
					BatchList?.Add(batch);
					BatchDict[batch.Id] = batch;
				}
			}

			di.BatchBrowser?.BatchListUpdated();
		}

		public void ReplaceCategoryData(ObservableCollection<Category> categoryList)
		{
			lock (CatList)
			{
				CatList?.Clear();
				CatDict?.Clear();

				foreach (var category in categoryList)
				{
					CatList?.Add(category);
					CatDict[category.Code] = category;
				}
			}
		}

		public void SerializeXml<T>(string path, ObservableCollection<T>? collection, bool prettyprint = true)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<T>));
			XmlWriterSettings settings = new XmlWriterSettings() { Indent = prettyprint, NewLineOnAttributes = prettyprint };
			using var writer = di.FileSystem.File.CreateText(path);
			using var xmlWriter = XmlWriter.Create(writer, settings);
			{
				serializer.Serialize(xmlWriter, collection);
			}
		}

		public ObservableCollection<T>? DeserializeXml<T>(string path)
		{
			ObservableCollection<T>? ret;

			if (!string.IsNullOrEmpty(path))
			{
				using var reader = di.FileSystem.File.OpenText(path);
				{
					XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<T>));
					ret = serializer?.Deserialize(reader) as ObservableCollection<T>;
				}
			}
			else
			{
				ret = new ObservableCollection<T>();
			}

			if (null == ret)
			{
				throw new Exception("DeserializeXml(reader) returned null");
			}

			return ret;
		}

		public void SerializeJson<Tkey, Tvalue>(string path, Dictionary<Tkey, Tvalue>? collection, bool prettyprint = false)
		{
			if (null == collection)
			{
				throw new ArgumentNullException("collection is null");
			}

			JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = prettyprint };
			var jstring = JsonSerializer.Serialize(collection, options: options);
			using var writer = di.FileSystem.File.CreateText(path);
			{
				writer.WriteAsync(jstring);
			}
		}

		public Dictionary<Tkey, Tvalue> DeserializeJson<Tkey, Tvalue>(string path)
		{
			Dictionary<Tkey, Tvalue>? ret;

			if (!string.IsNullOrEmpty(path))
			{
				using var reader = di.FileSystem.File.OpenText(path);
				{
					var jstring = reader.ReadLine();
					ret = JsonSerializer.Deserialize<Dictionary<Tkey, Tvalue>>(jstring);
				}
			}
			else
			{
				ret = new Dictionary<Tkey, Tvalue>();
			}

			if (null == ret)
			{
				throw new InsufficientMemoryException($"DeserializeJson(reader) returned null");
			}

			return ret;
		}

		public void ExportCsv<T>(string filePath, ObservableCollection<T>? collection)
		{
			using var writer = di.FileSystem.File.CreateText(filePath);
			{
				writer.WriteLine(CsvHeaders<T>());

				if (null != collection)
				{
					foreach (var item in collection)
					{
						writer.WriteLine(CsvLine(item));
					}
				}
			}
		}

		// https://csharpforums.net/threads/convert-object-properties-to-array.6464/
		public string CsvHeaders<T>()
		{
			return string.Join(",", typeof(T).GetProperties().Select(p => p.Name).ToArray());
		}

		public string CsvLine<T>(T line)
		{
			return string.Join(",", line.GetType()
										.GetProperties(BindingFlags.Instance | BindingFlags.Public)
										.Select((pi) => {
											var str = pi.GetValue(line)?.ToString();
											if (null == str) return "";
											if (str.Contains(',')) return $"\"{str}\"";
											else return $"{str}";
										})
										.ToArray());
		}
	}
}
