using Donations.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donations.Interfaces
{
    public interface IData
    {
		public string CategoriesFileName { get; }
		public string DonorsFileName { get; }
		public string DonationsFileName { get; }
		public string BatchesFileName { get; }
		public string DonorMapFileName { get; }
		public string CategoryMapFileName { get; }
		public string EnvelopeDesignFileName { get; }

		ObservableCollection<Donor>? DonorList { get; set; }
		Dictionary<int, Donor>? DonorDict { get; set; }
		ObservableCollection<Donation>? DonationList { get; set; }
		Dictionary<int, Donation>? DonationDict { get; set; }
		ObservableCollection<Batch>? BatchList { get; set; }
		Dictionary<int, Batch>? BatchDict { get; set; }
		ObservableCollection<EnvelopeEntry>? TitheEnvelopeDesign { get; set; }
		Dictionary<int, Category>? CatDict { get; set; }
		ObservableCollection<Category>? CatList { get; set; }
		ObservableCollection<AGDonorMapItem>? AGDonorMapList { get; set; }
		Dictionary<string, AGDonorMapItem>? AGDonorMap { get; set; }
		ObservableCollection<AGCategoryMapItem>? AGCategoryMapList { get; set; }
		Dictionary<int, AGCategoryMapItem>? AGCategoryMap { get; set; }

		void LoadData();

		string? SaveDonors(ObservableCollection<Donor> donors, bool force = false);

		string? SaveDonations(ObservableCollection<Donation> donations, bool force = false);

		void SaveBatches(ObservableCollection<Batch> batch);

		string? SaveCategories(ObservableCollection<Category> categories, bool force = false);

		void SaveData();

		void ReplaceDonorData(ObservableCollection<Donor> donorList);

		void ReplaceDonationData(ObservableCollection<Donation> donationList, ObservableCollection<Batch> batchList);

		void ReplaceCategoryData(ObservableCollection<Category> categoryList);

		void SerializeXml<T>(string path, ObservableCollection<T>? collection, bool prettyprint = true);

		ObservableCollection<T>? DeserializeXml<T>(string path);

		void SerializeJson<Tkey, Tvalue>(string path, Dictionary<Tkey, Tvalue>? collection, bool prettyprint = false);

		Dictionary<Tkey, Tvalue> DeserializeJson<Tkey, Tvalue>(string path);

		void ExportCsv<T>(string filePath, ObservableCollection<T>? collection);

		// https://csharpforums.net/threads/convert-object-properties-to-array.6464/
		string CsvHeaders<T>();

		string CsvLine<T>(T line);
	}
}
