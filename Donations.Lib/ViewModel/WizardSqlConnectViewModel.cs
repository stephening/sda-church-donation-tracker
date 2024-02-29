using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dapper;
using Donations.Lib.Services;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Donations.Lib.ViewModel;

public partial class WizardSqlConnectViewModel : ObservableObject
{
	public WizardSqlConnectViewModel(
		SqlCreateTables sqlCreateTables
	)
	{
		_sqlCreateTables = sqlCreateTables;
	}

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(ConnectToDatabaseCommand))]
	[NotifyCanExecuteChangedFor(nameof(CreateDatabaseCommand))]
	[NotifyCanExecuteChangedFor(nameof(CreateTablesCommand))]
	private string? _connectionString = "Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;";

	[ObservableProperty]
	private string? _connectionStatus;

	private WizardMainWindowViewModel? _wizardMainWindowViewModel;
	private readonly SqlCreateTables _sqlCreateTables;

	public void Init(WizardMainWindowViewModel wizardMainWindowViewModel)
	{
		_wizardMainWindowViewModel = wizardMainWindowViewModel;
	}

	[RelayCommand(CanExecute = nameof(ConnectionStringNotEmpty))]
	public void ConnectToDatabase()
	{
		try
		{
			// First save connection string to config file
			var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			config.ConnectionStrings.ConnectionStrings.Remove(SqlHelper.DbKey);

			string? connectionString = ConnectionString?.Replace("Database=master;", "Database=donations;");
			config.ConnectionStrings.ConnectionStrings.Add(new ConnectionStringSettings(SqlHelper.DbKey, connectionString, "System.Data.SqlClient"));
			config.Save(ConfigurationSaveMode.Modified);

			ConfigurationManager.RefreshSection("connectionStrings");

			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			if (null != conn)
			{
				ConnectionStatus = "Connection succeeded";
			}
			else
			{
				ConnectionStatus = "Connection failed";
			}
		}
		catch (Exception ex)
		{
			ConnectionStatus = ex.Message;
		}
	}

	private bool ConnectionStringNotEmpty()
	{
		return !string.IsNullOrEmpty(ConnectionString);
	}

	[RelayCommand(CanExecute = nameof(ConnectionStringNotEmpty))]
	public async void CreateDatabase()
	{
		var query = $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'donations') CREATE DATABASE [donations];";

		try
		{
			// Use Database=master connection string when creating the database
			using IDbConnection conn = new SqlConnection(ConnectionString);
			if (null != conn)
			{
				await conn.ExecuteAsync(query);
				ConnectionStatus = "Database creation succeeded";
			}
			else
			{
				ConnectionStatus = "Connection failed";
			}
		}
		catch (Exception ex)
		{
			ConnectionStatus = ex.Message;
		}
	}

	[RelayCommand(CanExecute = nameof(ConnectionStringNotEmpty))]
	public async void CreateTables()
	{
		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			if (null != conn)
			{
				await _sqlCreateTables.CreateAllTables();

				ConnectionStatus = "Tables created successfully";
			}
			else
			{
				ConnectionStatus = "Connection failed";
			}
		}
		catch (Exception ex)
		{
			ConnectionStatus = ex.Message;
		}
	}
}
