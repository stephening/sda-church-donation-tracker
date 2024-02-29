using Donations.Lib;
using Donations.Lib.Interfaces;
using Donations.Lib.ViewModel;
using Xunit;

namespace Donations.Tests;

public class EnvelopeDesignViewModelTests : TestBase
{
	[Fact]
	public void DeleteAll()
	{
		// Arrange
		EnvelopeDesignViewModel obj = DependencyInjection.Resolve<EnvelopeDesignViewModel>();

		// Act
		obj.DeleteAll();

		// Assert
		Assert.Empty(obj.EnvelopeEntries);

		// Act again
		obj.Revert();

		// Assert again
		Assert.Equal(8, obj.EnvelopeEntries.Count);
	}

	[Fact]
	public async Task SaveChanges_Empty()
	{
		// Arrange
		EnvelopeDesignViewModel obj = DependencyInjection.Resolve<EnvelopeDesignViewModel>();
		ITitheEnvelopeServices titheEnvelopeServices = DependencyInjection.Resolve<ITitheEnvelopeServices>();

		// Act
		obj.DeleteAll();

		// Assert
		Assert.Empty(obj.EnvelopeEntries);

		// Act again
		await obj.SaveChanges();

		// Assert again
#pragma warning disable CS8604 // Possible null reference argument.
		Assert.Empty(titheEnvelopeServices.TitheEnvelopeDesign);
#pragma warning restore CS8604 // Possible null reference argument.
	}
}
