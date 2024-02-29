using Autofac;
using Donations.Lib;
using System.Windows;

namespace Donors;
public partial class App : Application
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	public static IContainer Container { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

	public App()
	{
		Container = ContainerConfig.Configure();
	}

	protected override void OnStartup(StartupEventArgs e)
	{
		var scope = Container!.BeginLifetimeScope();

		DependencyInjection.Scope = scope;

		var startupWindow = scope.Resolve<MainWindow>();

		startupWindow.Show();

		base.OnStartup(e);
	}
}
