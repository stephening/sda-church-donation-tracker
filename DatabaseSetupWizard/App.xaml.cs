using Autofac;
using Donations.Lib.View;
using Serilog;
using Donations.Lib.Extensions;
using System.Windows;

namespace DatabaseSetupWizard;

public partial class App : Application
{
	public static IContainer? Container { get; private set; }

	protected override void OnStartup(StartupEventArgs e)
	{
		base.OnStartup(e);

		Container = ContainerConfig.ConfigureSetupWizard();

		var scope = Container!.BeginLifetimeScope();

		var startupWindow = scope.Resolve<WizardMainWindow>();

		var logger = scope.Resolve<ILogger>();

		logger.Info("Starting up in wizard mode");

		startupWindow.Show();
	}
}