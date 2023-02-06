using Donations.ViewModel;
using Xunit;

namespace Donations.Tests
{
	public class AdventistGivingViewModelTests
	{
		[Theory]
		[InlineData("This is a batch note...")]
		[InlineData(null)]
		[InlineData("")]
		public void BatchNote(string? param)
		{
			// Arronge
			AdventistGivingViewModel agvm = new AdventistGivingViewModel() { BatchNote = param };
			string expected = param;

			// Act
			string actual = agvm.BatchNote;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(10000)]
		[InlineData(0.9999999999999999)]
		[InlineData(99999999999999999)]
		[InlineData(-1000.1111111111111)]
		public void TargetTotal(double param)
		{
			// Arronge
			AdventistGivingViewModel agvm = new AdventistGivingViewModel() { TargetTotal = param };
			double expected = param;

			// Act
			double actual = agvm.TargetTotal;

			// Assert
			Assert.Equal(expected, actual);
		}

		/// <summary>
		/// Other Import tests are in SequentialTests because the nature of the mocked StreamReader
		/// doesn't allow multiple instances to run in prallel.
		/// </summary>
		/// <param name="path"></param>
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public async void AdventistGivingViewModel_ImportNullOrEmptyFilename(string? path)
		{
			// Arronge
			AdventistGivingViewModel giving = new AdventistGivingViewModel();

			// Act
			Func<Task> act = async () => await giving.Import(path);

			// Assert
			Exception ex = await Assert.ThrowsAsync<Exception>(act);
		}
	}
}
