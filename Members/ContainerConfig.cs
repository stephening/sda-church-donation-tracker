using Autofac;
using Donations.Lib;

namespace Donors;

public static class ContainerConfig
{
	public static IContainer Configure()
	{
		var builder = new ContainerBuilder();

		builder.SetupDonationsLib();
		builder.RegisterType<MainWindow>();
		builder.RegisterType<MainWindowViewModel>();

		return builder.Build();
	}
}
