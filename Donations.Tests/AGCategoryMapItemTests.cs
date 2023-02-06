using Donations.Model;
using Xunit;

namespace Donations.Tests
{
	public class AGCategoryMapItemTests
	{
		[Theory]
		[InlineData(1001)]
		[InlineData(0)]
		[InlineData(99999999)]
		public void LastName(int code)
		{
			// Arronge
			AGCategoryMapItem ag = new AGCategoryMapItem() { AGCategoryCode = code };
			int expected = code;

			// Act
			int actual = ag.AGCategoryCode;

			// Assert
			Assert.Equal(expected, actual);
		}

	}
}
