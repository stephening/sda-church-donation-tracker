using Dapper;
using Donations.Lib.Extensions;
using Serilog;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Donations.Lib.Services;

public class SqlHelper
{
	public static string DbKey = "production";
#pragma warning disable CS8601 // Possible null reference assignment.
	protected static string _connString = ConfigurationManager.ConnectionStrings[DbKey]?.ConnectionString;
#pragma warning restore CS8601 // Possible null reference assignment.
	protected readonly ILogger _logger;

	public SqlHelper(ILogger logger)
	{
		_logger = logger;
	}

	public static void UpdateConnectionString()
	{
#pragma warning disable CS8601 // Possible null reference assignment.
		_connString = ConfigurationManager.ConnectionStrings[DbKey]?.ConnectionString;
#pragma warning restore CS8601 // Possible null reference assignment.
	}

	public async Task DropTable(string tableName)
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				// first delete existing rows
				string cmd = $"DROP TABLE {tableName}";

				await conn.ExecuteAsync(cmd, commandTimeout: 300);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception dropping table {tableName}.");
		}
	}

	protected async Task<string?> WriteEntireTableAsync<T>(bool ident_property, ObservableCollection<T> collection, string dbName, bool force = false, Action<long, long>? progUpdate = null, bool reseedOnDelete = true)
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				if (null != progUpdate)
				{
					// this task is not awaited, so that the following sql query will start.
					// when it starts, the following task will begin reporting progress updates.
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
					Task.Run(() =>
					{
						try
						{
							string selCountQuery = $"SELECT COUNT(*) FROM {dbName}";
							int total = collection.Count;
							int cnt = 0;

							using IDbConnection conn2 = new SqlConnection(_connString);
							do
							{
#pragma warning disable CS8605 // Unboxing a possibly null value.
								cnt = (int)conn2.ExecuteScalar(selCountQuery);
#pragma warning restore CS8605 // Unboxing a possibly null value.

								progUpdate.Invoke(cnt, total);
								Thread.Sleep(500);
							} while (cnt < total);
						}
						catch (Exception ex)
						{
							_logger.Err(ex, $"Exception getting number of records written to {dbName}.");
						}
					});
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
				}

				// finally write full collection to db table
				string command = "";
				if (ident_property)
					command += $"SET IDENTITY_INSERT {dbName}\r\nON ";

				command += $"INSERT INTO {dbName} ({string.Join(',', Helper.PublicProperties<T>())})\r\nVALUES ({string.Join(',', Helper.PublicProperties<T>().Select(x => "@" + x).ToList())})";

				await conn.ExecuteAsync(command, collection);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception writing to table {dbName}.");
		}
		return null;
	}

	protected async Task<ObservableCollection<T>> SelectFromTableAsync<T>(string dbName, string top = "", string where = "")
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				string query = $"SELECT {top} * FROM {dbName} {where}";
				var enumerable = await conn.QueryAsync<T>(query);
				return new ObservableCollection<T>(enumerable);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception reading from table {dbName}.");
		}
		return new ObservableCollection<T>();
	}

	protected ObservableCollection<T> SelectFromTable<T>(string dbName, string top = "", string where = "")
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				string query = $"SELECT {top} * FROM {dbName} {where}";
				var enumerable = conn.Query<T>(query);
				return new ObservableCollection<T>(enumerable);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception reading from table {dbName}.");
		}
		return new ObservableCollection<T>();
	}

	protected async Task<int> Add<T>(T obj, string dbName)
	{
		// write the new T record
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				var properties = Helper.PublicProperties<T>().ToList();
				properties.Remove("Id");
				string command = "";
				command += $"INSERT INTO {dbName} ({string.Join(',', properties)}) OUTPUT INSERTED.Id VALUES ({string.Join(',', properties.Select(x => "@" + x).ToList())})";

				int id = await conn.ExecuteScalarAsync<int>(command, obj);
				return id;
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception writing to {dbName}.");
			MessageBox.Show(ex.Message, $"Exception writing to {dbName}", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}

		return -1;
	}

	protected async Task Delete<T>(string dbName, int id)
	{
		// write the new T record
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				string command = $"DELETE FROM {dbName} WHERE Id = {id}";

				await conn.ExecuteAsync(command);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception deleting from {dbName}.");
			MessageBox.Show(ex.Message, $"Exception deleting from {dbName}", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}
	}

	protected async Task<string?> Update<T>(T obj, string dbName, int id)
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				// update obj to db table
				var properties = Helper.PublicProperties<T>().ToList();
				properties.Remove("Id");
				string command = $"UPDATE {dbName} SET {string.Join(',', properties.Select(x => x + "=@" + x).ToList())} WHERE Id = {id}";

				await conn.ExecuteAsync(command, obj);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception updating to {dbName}.");
			MessageBox.Show(ex.Message, $"Exception updating to {dbName}", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}

		return null;
	}

	protected string? Update<T>(T obj, string dbName, string key, int id)
	{
		try
		{
			using IDbConnection conn = new SqlConnection(_connString);
			{
				// update obj to db table
				var properties = Helper.PublicProperties<T>().ToList();
				string insert = $"INSERT INTO {dbName} ({string.Join(',', properties)}) VALUES ({string.Join(',', properties.Select(x => "@" + x).ToList())})";
				properties.Remove(key);
				string command = $"UPDATE {dbName} SET {string.Join(',', properties.Select(x => x + "=@" + x).ToList())} WHERE {key} = {id}\r\n" +
					"IF @@ROWCOUNT = 0\r\n" +
					$"BEGIN\r\n{insert}\r\nEND\r\n";

				conn.Execute(command, obj);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception updating to {dbName}.");
			MessageBox.Show(ex.Message, $"Exception updating to {dbName}", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}

		return null;
	}
}
