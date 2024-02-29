using Autofac;
using Donations.Lib;

namespace DatabaseSetupWizard;

public static class ContainerConfig
{
	public static IContainer ConfigureSetupWizard()
	{
		var builder = new ContainerBuilder();

		builder.SetupWizard();

		return builder.Build();
	}
}
