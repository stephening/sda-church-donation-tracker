using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib.Interfaces
{
	public interface IDataHelpers
	{
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
		Dictionary<Tkey, Tvalue> DeserializeJson<Tkey, Tvalue>(string path);
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
		ObservableCollection<T>? DeserializeXml<T>(string path);
		Task<ObservableCollection<T>>? DeserializeXmlAsync<T>(string path);
		void ExportCsv<T>(string filePath, ObservableCollection<T>? collection);
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
		void SerializeJson<Tkey, Tvalue>(string path, Dictionary<Tkey, Tvalue>? collection, bool prettyprint = false);
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
		void SerializeXml<T>(string path, ObservableCollection<T>? collection, bool prettyprint = true);
		Task<string?> SerializeXmlAsync<T>(string path, ObservableCollection<T>? collection, bool prettyprint = true);
	}
}