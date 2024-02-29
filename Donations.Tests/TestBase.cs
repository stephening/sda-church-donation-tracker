using Autofac;
using Donations.Lib;
using System.IO.Abstractions;

namespace Donations.Tests;

public class TestBase
{
	public IContainer Container { get; private set; }
	public TestBase()
	{
		var builder = new ContainerBuilder();

		builder.SetupDonationsLib();
		builder.RegisterTestDataServices();
		builder.Register();

		Container = builder.Build();

		var scope = Container!.BeginLifetimeScope();

		DependencyInjection.Scope = scope;
	}

	protected void AutofacRegister(IFileSystem? mockfs = null)
	{
		var builder = new ContainerBuilder();

		builder.SetupDonationsLib();
		builder.RegisterTestDataServices();
		builder.Register();

		// This must follow Register because it will replace the IFileSystem with the user supplied one
		if (mockfs != null)
		{
			builder.RegisterInstance(mockfs).As<IFileSystem>();
		}

		var container = builder.Build();

		var scope = container!.BeginLifetimeScope();

		DependencyInjection.Scope = scope;
	}

	protected void WizardRegister()
	{
		var builder = new ContainerBuilder();

		builder.SetupWizard();
		builder.Register();

		var container = builder.Build();

		var scope = container!.BeginLifetimeScope();

		DependencyInjection.Scope = scope;
	}

}
