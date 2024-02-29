using Autofac;
using Donations.Lib;

namespace Donations;

public static class ContainerConfig
{
	public static IContainer Configure()
	{
		var builder = new ContainerBuilder();

		builder.SetupDonationsLib();
		builder.RegisterType<MainWindow>();

		return builder.Build();
	}

	public static IContainer ConfigureSetupWizard()
	{
		var builder = new ContainerBuilder();

		builder.SetupWizard();

		return builder.Build();
	}

	public static IContainer ConfigureTestData()
	{
		var builder = new ContainerBuilder();

		builder.SetupDonationsLib();
		builder.RegisterTestDataServices();
		builder.RegisterType<MainWindow>();

		return builder.Build();
	}

	public static IContainer ConfigureScreenshots()
	{
		var builder = new ContainerBuilder();

		builder.SetupDonationsLib();
		builder.SetupWizard();
		builder.RegisterTestDataServices();
		builder.RegisterType<MainWindow>();

		return builder.Build();
	}
}
