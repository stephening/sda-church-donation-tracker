using Donations.Model;
using Donations.ViewModel;
using System.Windows;
using Xunit;

namespace Donations.Tests
{
	public class AGDonorResolutionViewModelTests
	{
		[Fact]
		public void Donor_Success()
		{
			// Arronge
			Donor donor = new Donor() { LastName = "Doe", FirstName = "John", Address = "1234 Acme Lane", City = "Pearly Gates", State = "State", Zip = "98765" };
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel() { Donor = donor };
			Donor expected = donor;

			// Act
			Donor actual = obj.Donor;

			// Assert
			Assert.Equal(expected, actual);
			Assert.Equal(donor.LastName, obj.Donor.LastName);
			Assert.Equal(donor.FirstName, obj.Donor.FirstName);
			Assert.Equal(donor.Address, obj.Donor.Address);
		}

		[Theory]
		[InlineData("string1", "string1", Visibility.Hidden)]
		[InlineData("string1", "string2", Visibility.Visible)]
		[InlineData(null, "string2", Visibility.Visible)]
		[InlineData("", "string2", Visibility.Visible)]
		public void CopyVisible(string? param1, string? param2, Visibility expectedVisibility)
		{
			// Arronge
			Donor donor = new Donor() { LastName = param1, FirstName = param1, Address = param1, Address2 = param1, City = param1, State = param1, Zip = param1 };
			AdventistGiving transaction = new AdventistGiving() { LastName = param2, FirstName = param2, Address = param2, Address2 = param2, City = param2, State = param2, Zip = param2 };
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel() { Donor = donor, Transaction = transaction };
			Donor expected = donor;

			// Act
			Donor actual = obj.Donor;

			// Assert
			Assert.Equal(expectedVisibility, obj.LastNameCopyVisible);
			Assert.Equal(expectedVisibility, obj.FirstNameCopyVisible);
			Assert.Equal(expectedVisibility, obj.AddressCopyVisible);
			Assert.Equal(expectedVisibility, obj.Address2CopyVisible);
			Assert.Equal(expectedVisibility, obj.CityCopyVisible);
			Assert.Equal(expectedVisibility, obj.StateCopyVisible);
			Assert.Equal(expectedVisibility, obj.ZipCopyVisible);
			Assert.Equal(expectedVisibility, obj.CopyAllVisible);

			if (Visibility.Hidden == obj.CopyAllVisible)
			{
				obj.Donor.LastName = "somethig";
				Assert.Equal(Visibility.Visible, obj.CopyAllVisible);
			}
		}

		[Theory]
		[InlineData("string1")]
		[InlineData("")]
		public void CopyCommands(string? param)
		{
			// Arronge
			Donor donor = new Donor();
			AdventistGiving transaction = new AdventistGiving() { LastName = param, FirstName = param, Address = param, Address2 = param, City = param, State = param, Zip = param };
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel() { Donor = donor, Transaction = transaction };

			// Act

			// Assert
			Assert.NotEqual(donor.LastName, obj.Transaction.LastName);
			Assert.NotEqual(donor.FirstName, obj.Transaction.FirstName);
			Assert.NotEqual(donor.Address, obj.Transaction.Address);
			Assert.NotEqual(donor.Address2, obj.Transaction.Address2);
			Assert.NotEqual(donor.City, obj.Transaction.City);
			Assert.NotEqual(donor.State, obj.Transaction.State);
			Assert.NotEqual(donor.Zip, obj.Transaction.Zip);

			// Act again
			obj.CopyLastNameCmd.Execute(null);
			obj.CopyFirstNameCmd.Execute(null);
			obj.CopyAddressCmd.Execute(null);
			obj.CopyAddress2Cmd.Execute(null);
			obj.CopyCityCmd.Execute(null);
			obj.CopyStateCmd.Execute(null);
			obj.CopyZipCmd.Execute(null);

			// Assert again
			Assert.Equal(donor.LastName, obj.Transaction.LastName);
			Assert.Equal(donor.FirstName, obj.Transaction.FirstName);
			Assert.Equal(donor.Address, obj.Transaction.Address);
			Assert.Equal(donor.Address2, obj.Transaction.Address2);
			Assert.Equal(donor.City, obj.Transaction.City);
			Assert.Equal(donor.State, obj.Transaction.State);
			Assert.Equal(donor.Zip, obj.Transaction.Zip);

			// Act again
			donor = new Donor();
			obj.Donor = donor;

			// Assert
			Assert.NotEqual(donor.LastName, obj.Transaction.LastName);
			Assert.NotEqual(donor.FirstName, obj.Transaction.FirstName);
			Assert.NotEqual(donor.Address, obj.Transaction.Address);
			Assert.NotEqual(donor.Address2, obj.Transaction.Address2);
			Assert.NotEqual(donor.City, obj.Transaction.City);
			Assert.NotEqual(donor.State, obj.Transaction.State);
			Assert.NotEqual(donor.Zip, obj.Transaction.Zip);

			// Act again
			obj.CopyAllCmd.Execute(null);

			// Assert again
			Assert.Equal(donor.LastName, obj.Transaction.LastName);
			Assert.Equal(donor.FirstName, obj.Transaction.FirstName);
			Assert.Equal(donor.Address, obj.Transaction.Address);
			Assert.Equal(donor.Address2, obj.Transaction.Address2);
			Assert.Equal(donor.City, obj.Transaction.City);
			Assert.Equal(donor.State, obj.Transaction.State);
			Assert.Equal(donor.Zip, obj.Transaction.Zip);
		}

		[Fact]
		public void Donor_Null()
		{
			// Arronge
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel();
			Donor expected = null;

			// Act
			Donor actual = obj.Donor;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void Transaction_Success()
		{
			// Arronge
			AdventistGiving transaction = new AdventistGiving();
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel() { Transaction = transaction };
			AdventistGiving expected = transaction;

			// Act
			AdventistGiving actual = obj.Transaction;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void Transaction_Null()
		{
			// Arronge
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel();
			AdventistGiving expected = null;

			// Act
			AdventistGiving actual = obj.Transaction;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		public void Transaction_NullFirstName(string? param)
		{
			// Arronge
			AdventistGiving transaction = new AdventistGiving() { FirstName = param };
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel() { Transaction = transaction };
			string? expected = param;

			// Act
			AdventistGiving actual = obj.Transaction;

			// Assert
			Assert.Equal(transaction, actual);
			Assert.True(string.IsNullOrEmpty(obj.Transaction.LastName));
		}

		[Theory]
		[InlineData("This is progress text...")]
		[InlineData(null)]
		[InlineData("")]
		public void ProgressText(string? param)
		{
			// Arronge
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel() { ProgressText = param };
			string expected = param;

			// Act
			string actual = obj.ProgressText;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Fact]
		public void ContinueEnabled_True()
		{
			// Arronge
			Donor donor = new Donor();
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel() { Donor = donor };

			// Act
			bool actual = obj.ContinueEnabled;

			// Assert
			Assert.True(actual);
		}

		[Fact]
		public void ContinueEnabled_False()
		{
			// Arronge
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel() { Donor = null };

			// Act
			bool actual = obj.ContinueEnabled;

			// Assert
			Assert.False(actual);
		}

		[Theory]
		[InlineData(Visibility.Collapsed)]
		[InlineData(Visibility.Visible)]
		[InlineData(Visibility.Hidden)]
		public void DonorDiffsVisibility(Visibility param)
		{
			// Arronge
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel() { DonorDiffsVisibility = param };
			Visibility expected = param;

			// Act
			Visibility actual = obj.DonorDiffsVisibility;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(Visibility.Collapsed)]
		[InlineData(Visibility.Visible)]
		[InlineData(Visibility.Hidden)]
		public void DonorResolutionComplete(Visibility param)
		{
			// Arronge
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel() { DonorResolutionComplete = param };
			Visibility expected = param;

			// Act
			Visibility actual = obj.DonorResolutionComplete;

			// Assert
			Assert.Equal(expected, actual);
		}


		[Theory]
		[InlineData(true, Visibility.Visible, Visibility.Hidden)]
		[InlineData(false, Visibility.Hidden, Visibility.Visible)]
		public void ResolutionComplete(bool flag, Visibility expectedDonorResolutionComplete, Visibility expectedDonorDiffsVisibility)
		{
			// Arronge
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel();
			bool expected = flag;

			// Act
			obj.ResolutionComplete(flag);

			// Assert
			Assert.Equal(expectedDonorResolutionComplete, obj.DonorResolutionComplete);
			Assert.Equal(expectedDonorDiffsVisibility, obj.DonorDiffsVisibility);
		}

		[Fact]
		public void ChooseDonor()
		{
			// Arronge
			Donor expected = new Donor();
			AGDonorResolutionViewModel obj = new AGDonorResolutionViewModel();

			// Act
			Donor actualBefore = obj.Donor;
			obj.ChooseDonor(expected);
			Donor actualAfter = obj.Donor;

			// Assert
			Assert.Null(actualBefore);
			Assert.Equal(expected, actualAfter);
		}
	}
}
