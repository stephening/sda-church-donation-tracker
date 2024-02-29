using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Extensions;
using Donations.Lib.Services;
using Serilog;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Donations.Lib.ViewModel;

public partial class WizardSpecifyConnectionStringViewModel : ObservableObject
{
	public WizardSpecifyConnectionStringViewModel(ILogger logger)
	{
		_logger = logger;
	}

	[ObservableProperty]
	[NotifyCanExecuteChangedFor(nameof(ConnectToDatabaseCommand))]
	private string? _connectionString = "Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;";

	[ObservableProperty]
	private string? _connectionStatus;
	private readonly ILogger _logger;

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

			try
			{
				config.Save(ConfigurationSaveMode.Modified);
				ConfigurationManager.RefreshSection("connectionStrings");
				SqlHelper.UpdateConnectionString();
			}
			catch (Exception ex)
			{
				_logger.Err(ex, $"An exception occurred when trying to add the connectionstring to the config file.");
				MessageBox.Show("The setup wizard must be run with elevated priviledges. Please right-click on the application icon and run as administrator");
				Application.Current.Shutdown();
			}

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
}
