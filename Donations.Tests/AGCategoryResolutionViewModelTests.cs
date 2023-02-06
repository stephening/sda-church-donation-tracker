using Donations.Model;
using Donations.ViewModel;
using System.Windows;
using Xunit;

namespace Donations.Tests
{
	public class AGCategoryResolutionViewModelTests
	{
		[Fact]
		public void Cat_Success()
		{
			// Arronge
			Category category = new Category() { Code = 101, Description = "Whatever", TaxDeductible = true };
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel() { Cat = category };
			Category expected = category;

			// Act
			Category actual = obj.Cat;

			// Assert
			Assert.Equal(expected, actual);
			Assert.Equal(category.Code, obj.Cat.Code);
			Assert.Equal(category.Description, obj.Cat.Description);
			Assert.Equal(category.TaxDeductible, obj.Cat.TaxDeductible);
		}

		[Fact]
		public void Cat_Null()
		{
			// Arronge
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel();
			Category expected = null;

			// Act
			Category actual = obj.Cat;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		public void Cat_NullDescription(string? param)
		{
			// Arronge
			Category category = new Category() { Code = 101, Description = param, TaxDeductible = true };
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel() { Cat = category };
			Category expected = category;

			// Act
			Category actual = obj.Cat;

			// Assert
			Assert.Equal(expected, actual);
			Assert.Equal(category.Code, obj.Cat.Code);
			Assert.Equal(category.Description, obj.Cat.Description);
			Assert.Equal(category.TaxDeductible, obj.Cat.TaxDeductible);
		}


		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void Cat_TaxDeductible(bool param)
		{
			// Arronge
			Category category = new Category() { Code = 101, Description = "Description", TaxDeductible = param };
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel() { Cat = category };
			bool expected = param;

			// Act
			bool actual = obj.Cat.TaxDeductible;

			// Assert
			Assert.Equal(expected, actual);
			Assert.Equal(category.Code, obj.Cat.Code);
			Assert.Equal(category.Description, obj.Cat.Description);
			Assert.Equal(category.TaxDeductible, obj.Cat.TaxDeductible);
		}

		[Fact]
		public void Transaction_Success()
		{
			// Arronge
			AdventistGiving transaction = new AdventistGiving();
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel() { Transaction = transaction };
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
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel();
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
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel() { Transaction = transaction };
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
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel() { ProgressText = param };
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
			Category cat = new Category();
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel() { Cat = cat };

			// Act
			bool actual = obj.ContinueEnabled;

			// Assert
			Assert.True(actual);
		}

		[Fact]
		public void ContinueEnabled_False()
		{
			// Arronge
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel() { Cat = null };

			// Act
			bool actual = obj.ContinueEnabled;

			// Assert
			Assert.False(actual);
		}

		[Theory]
		[InlineData(Visibility.Collapsed)]
		[InlineData(Visibility.Visible)]
		[InlineData(Visibility.Hidden)]
		public void CategoryDiffsVisibility(Visibility param)
		{
			// Arronge
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel() { CategoryDiffsVisibility = param };
			Visibility expected = param;

			// Act
			Visibility actual = obj.CategoryDiffsVisibility;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(Visibility.Collapsed)]
		[InlineData(Visibility.Visible)]
		[InlineData(Visibility.Hidden)]
		public void CategoryResolutionComplete(Visibility param)
		{
			// Arronge
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel() { CategoryResolutionComplete = param };
			Visibility expected = param;

			// Act
			Visibility actual = obj.CategoryResolutionComplete;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(true, Visibility.Visible, Visibility.Hidden)]
		[InlineData(false, Visibility.Hidden, Visibility.Visible)]
		public void ResolutionComplete(bool flag, Visibility expectedCategoryResolutionComplete, Visibility expectedCategoryDiffsVisibility)
		{
			// Arronge
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel();
			bool expected = flag;

			// Act
			obj.ResolutionComplete(flag);

			// Assert
			Assert.Equal(expectedCategoryResolutionComplete, obj.CategoryResolutionComplete);
			Assert.Equal(expectedCategoryDiffsVisibility, obj.CategoryDiffsVisibility);
		}

		[Fact]
		public void ChooseCategory()
		{
			// Arronge
			Category expected = new Category();
			AGCategoryResolutionViewModel obj = new AGCategoryResolutionViewModel();

			// Act
			Category actualBefore = obj.Cat;
			obj.ChooseCategory(expected);
			Category actualAfter = obj.Cat;

			// Assert
			Assert.Null(actualBefore);
			Assert.Equal(expected, actualAfter);
		}
	}
}
