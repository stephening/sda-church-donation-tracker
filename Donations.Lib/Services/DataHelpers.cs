using Donations.Lib.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Donations.Lib.Services;

public class DataHelpers : IDataHelpers
{
	private readonly IFileSystem _fileSystem;

	public DataHelpers(
		IFileSystem fileSystem)
	{
		_fileSystem = fileSystem;
	}

	public void SerializeXml<T>(string path, ObservableCollection<T>? collection, bool prettyprint = true)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<T>));
		XmlWriterSettings settings = new XmlWriterSettings() { Indent = prettyprint, NewLineOnAttributes = prettyprint };
		using var writer = _fileSystem.File.CreateText(path);
		using var xmlWriter = XmlWriter.Create(writer, settings);
		{
			serializer.Serialize(xmlWriter, collection);
		}
	}

	public async Task<string?> SerializeXmlAsync<T>(string path, ObservableCollection<T>? collection, bool prettyprint = true)
	{
		XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<T>));
		XmlWriterSettings settings = new XmlWriterSettings() { Indent = prettyprint, NewLineOnAttributes = prettyprint };
		StringBuilder stringBuillder = new StringBuilder();

		using var writer = _fileSystem.File.CreateText(path);
		using var xmlWriter = XmlWriter.Create(stringBuillder, settings);
		{
			serializer.Serialize(xmlWriter, collection);
			await writer.WriteAsync(stringBuillder);
		}

		return null;
	}

	public ObservableCollection<T>? DeserializeXml<T>(string path)
	{
		ObservableCollection<T>? ret;

		if (!string.IsNullOrEmpty(path))
		{
			using var reader = _fileSystem.File.OpenText(path);
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

	public async Task<ObservableCollection<T>>? DeserializeXmlAsync<T>(string path)
	{
		ObservableCollection<T>? ret;

		if (!string.IsNullOrEmpty(path))
		{
			using var reader = _fileSystem.File.OpenText(path);
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
		using var writer = _fileSystem.File.CreateText(path);
		{
			writer.WriteAsync(jstring);
		}
	}

	public Dictionary<Tkey, Tvalue> DeserializeJson<Tkey, Tvalue>(string path)
	{
		Dictionary<Tkey, Tvalue>? ret;

		if (!string.IsNullOrEmpty(path))
		{
			using var reader = _fileSystem.File.OpenText(path);
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
		using var writer = _fileSystem.File.CreateText(filePath);
		{
			writer.WriteLine(string.Join(",", Helper.PublicProperties<T>(false)));

			if (null != collection)
			{
				foreach (var item in collection)
				{
					writer.WriteLine(Helper.CsvLine(item));
				}
			}
		}
	}
}
