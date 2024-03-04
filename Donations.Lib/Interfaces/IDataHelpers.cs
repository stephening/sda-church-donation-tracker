using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces
{
	public interface IDataHelpers
	{
		Dictionary<Tkey, Tvalue> DeserializeJson<Tkey, Tvalue>(string path);
		ObservableCollection<T>? DeserializeXml<T>(string path);
		Task<ObservableCollection<T>>? DeserializeXmlAsync<T>(string path);
		void ExportCsv<T>(string filePath, ObservableCollection<T>? collection);
		void SerializeJson<Tkey, Tvalue>(string path, Dictionary<Tkey, Tvalue>? collection, bool prettyprint = false);
		void SerializeXml<T>(string path, ObservableCollection<T>? collection, bool prettyprint = true);
		Task<string?> SerializeXmlAsync<T>(string path, ObservableCollection<T>? collection, bool prettyprint = true);
	}
}