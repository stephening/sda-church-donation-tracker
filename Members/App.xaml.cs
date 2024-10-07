using Autofac;
using Donations.Lib;
using Donations.Lib.Services;
using Donations.Lib.View;
using Serilog;
using System.Configuration;
using System.Diagnostics;
using System.Security.Principal;
using System;
using System.Windows;
using Donations.Lib.Extensions;

namespace Donors;
public partial class App : Application
{
	public static IContainer Container { get; private set; }

	public App()
	{
		Container = ContainerConfig.Configure();
	}

	protected override async void OnStartup(StartupEventArgs e)
	{
		if (e.Args.Length > 1 && "-screens" == e.Args[0])
		{
			Container = ContainerConfig.ConfigureScreenshots();

			var scope = Container!.BeginLifetimeScope();

			DependencyInjection.Scope = scope;

			MemberScreenShots? screens = DependencyInjection.Resolve<MemberScreenShots>();

			await screens?.AllScreens(e.Args[1]);

			Shutdown();
		}
		else if (e.Args.Length >= 1 && "-import" == e.Args[0] || null == ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString)
		{
			// connection string not setup yet
			Container = ContainerConfig.ConfigureSetupWizard();

			var scope = Container!.BeginLifetimeScope();

			DependencyInjection.Scope = scope;

			var logger = scope.Resolve<ILogger>();

			logger.Info("Missing connection string so need wizard.");

			bool import = false;
			if (e.Args.Length >= 1 && "-import" == e.Args[0])
			{
				import = true;
			}
			else
			{
				WindowsIdentity identity = WindowsIdentity.GetCurrent();
				WindowsPrincipal principal = new WindowsPrincipal(identity);
				if (false == principal.IsInRole(WindowsBuiltInRole.Administrator))
				{
					logger.Info("Not running in admin mode, so restart with elevated priviledges.");

					string? exeName = Process.GetCurrentProcess().MainModule!.FileName;
					ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
					startInfo.UseShellExecute = true;
					startInfo.Verb = "runas";
					try
					{
						Process.Start(startInfo);
					}
					catch { }
					Shutdown();
				}
			}

			var startupWindow = scope.Resolve<WizardMemberMaintenanceWindow>();

			logger.Info("Starting up in wizard mode");

			startupWindow.Show();
		}
		else
		{
			try
			{
				Container = ContainerConfig.Configure();

				var scope = Container!.BeginLifetimeScope();

				DependencyInjection.Scope = scope;

				var startupWindow = scope.Resolve<MainWindow>();

				var logger = scope.Resolve<ILogger>();

				logger.Info("Starting up in normal mode");

				startupWindow.Show();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}
	}
}
