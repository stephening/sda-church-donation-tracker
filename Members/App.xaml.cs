using Autofac;
using Donations.Lib;
using System.Windows;

namespace Donors;
public partial class App : Application
{
	public static IContainer Container { get; private set; }

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
