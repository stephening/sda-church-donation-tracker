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

	public static IContainer ConfigureSetupWizard()
	{
		var builder = new ContainerBuilder();

		builder.SetupWizard();

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
