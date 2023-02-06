using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using Xunit;

namespace Donations.Tests
{
	public class BatchPrintViewModelTests
	{
		[Theory]
		[InlineData("This is a font family name...")]
		[InlineData(null)]
		[InlineData("")]
		public void SelectedFont(string? param)
		{
			// Arronge
			BatchPrintViewModel obj = new BatchPrintViewModel() { SelectedFont = param };
			string expected = param;

			// Act
			string actual = obj.SelectedFont;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(10000)]
		[InlineData(0.9999999999999999)]
		[InlineData(99999999999999999)]
		[InlineData(-1000.1111111111111)]
		public void SelectedSize(double param)
		{
			// Arronge
			BatchPrintViewModel obj = new BatchPrintViewModel() { SelectedSize = param };
			double expected = param;

			// Act
			double actual = obj.SelectedSize;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(10000)]
		[InlineData(0.9999999999999999)]
		[InlineData(99999999999999999)]
		[InlineData(-1000.1111111111111)]
		public void LeftMargin(double param)
		{
			// Arronge
			BatchPrintViewModel obj = new BatchPrintViewModel() { LeftMargin = param };
			double expected = param;

			// Act
			double actual = obj.LeftMargin;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(10000)]
		[InlineData(0.9999999999999999)]
		[InlineData(99999999999999999)]
		[InlineData(-1000.1111111111111)]
		public void OtherMargins(double param)
		{
			// Arronge
			BatchPrintViewModel obj = new BatchPrintViewModel() { OtherMargins = param };
			double expected = param;

			// Act
			double actual = obj.OtherMargins;

			// Assert
			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("Arial")]
		[InlineData("Calibri")]
		[InlineData("Times New Roman")]
		[InlineData("Comic Sans MS")]
		[InlineData("Courier New")]
		public void FontList(string param)
		{
			// Arrange
			BatchPrintViewModel obj = new BatchPrintViewModel();

			// Act

			// Assert
			Assert.Contains(param, obj.FontList);
		}


		[Theory]
		[InlineData(8)]
		[InlineData(9)]
		[InlineData(10)]
		[InlineData(11)]
		[InlineData(12)]
		[InlineData(13)]
		[InlineData(14)]
		[InlineData(15)]
		[InlineData(16)]
		public void FontSizes(double param)
		{
			// Arronge
			BatchPrintViewModel obj = new BatchPrintViewModel();

			// Act

			// Assert
			Assert.Contains(param, obj.FontSizes);
		}

		[Fact]
		public void Loaded()
		{
			// Arrange
			int i;
			var td = new TestData();
			BatchPrintViewModel obj = new BatchPrintViewModel();
			FlowDocument flowdoc = new FlowDocument();
			Batch batch = td.BatchDict[3];
			ObservableCollection<Donation> donations = new ObservableCollection<Donation>(td.DonationList.Where(x => x.BatchId == batch.Id));

			// Act
			obj.Loaded(flowdoc, batch, donations);

			// Assert
			ObservableCollection<CategorySum> catsum = new ObservableCollection<CategorySum>(obj.CategorySumSource.View.Cast<CategorySum>().ToList());

			for (i = 0; i < donations.Count; i++)
			{
				Assert.Equal(catsum[i].Category, donations[i].Category);
				Assert.Equal(catsum[i].Sum, donations[i].Value);
			}

			i = 0;
			foreach (Donation donation in obj.DonationDetailsSource.View)
			{
				Assert.Equal(donation.Category, donations[i].Category);
				Assert.Equal(donation.Value, donations[i].Value);
				Assert.Equal(donation.Name, donations[i].Name);
				Assert.Equal(donation.Note, donations[i].Note);
				Assert.Equal(donation.Date, donations[i].Date);
				Assert.Equal(donation.TaxDeductible, donations[i].TaxDeductible);
				Assert.Equal(donation.Method, donations[i].Method);
				i++;
			}
		}
	}
}
