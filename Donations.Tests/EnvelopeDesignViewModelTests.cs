using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using Xunit;

namespace Donations.Tests
{
	public partial class DataAccessSequentialTests
	{

		[Fact]
		public void EnvelopeDesignViewModel()
		{
			// Arrange
			int i;
			var td = new TestData();
			di.Data.TitheEnvelopeDesign = td.TitheEnvelopeDesign;
			ObservableCollection<EnvelopeEntry> copy = new ObservableCollection<EnvelopeEntry>(td.TitheEnvelopeDesign);
			EnvelopeDesignViewModel obj = new EnvelopeDesignViewModel();

			// Assert
			Assert.Equal(20, di.Data.TitheEnvelopeDesign.Count);
			for (i = 0; i < di.Data.TitheEnvelopeDesign.Count; i++)
			{
				Assert.Equal(td.TitheEnvelopeDesign[i].Category, di.Data.TitheEnvelopeDesign[i].Category);
			}

			// Act again
			int delrow = 2;
			obj.SelectedRowIndex = delrow;
			obj.DeleteRowCmd.Execute(null);

			// Assert
			Assert.Equal(19, di.Data.TitheEnvelopeDesign.Count);
			for (i = 0; i < di.Data.TitheEnvelopeDesign.Count && i < copy.Count - 1; i++)
			{
				if (delrow > i)
					Assert.Equal(copy[i].Category, di.Data.TitheEnvelopeDesign[i].Category);
				else // delrow <= i
					Assert.Equal(copy[i + 1].Category, di.Data.TitheEnvelopeDesign[i].Category);
			}

			// Act again
			int insabove = 6;
			obj.SelectedRowIndex = insabove;
			obj.InsertRowAboveCmd.Execute(null);
			di.Data.TitheEnvelopeDesign[6].Category = "66 new row six";

			// Assert
			Assert.Equal(20, di.Data.TitheEnvelopeDesign.Count);
			for (i = 0; i < di.Data.TitheEnvelopeDesign.Count && i < copy.Count; i++)
			{
				if (delrow > i)
					Assert.Equal(copy[i].Category, di.Data.TitheEnvelopeDesign[i].Category);
				else if (insabove == i)
					Assert.Equal("66 new row six", di.Data.TitheEnvelopeDesign[i].Category);
				else if (insabove < i)
					Assert.Equal(copy[i].Category, di.Data.TitheEnvelopeDesign[i].Category);
				else // delrow <= i < insabove
					Assert.Equal(copy[i + 1].Category, di.Data.TitheEnvelopeDesign[i].Category);
			}

			// Act again
			int insbelow = 7;
			obj.SelectedRowIndex = insbelow;
			obj.InsertRowBelowCmd.Execute(null);
			di.Data.TitheEnvelopeDesign[insbelow + 1].Category = "99 new row nine";

			// Assert
			Assert.Equal(21, di.Data.TitheEnvelopeDesign.Count);
			for (i = 0; i < di.Data.TitheEnvelopeDesign.Count && i < copy.Count + 1; i++)
			{
				if (delrow > i)
					Assert.Equal(copy[i].Category, di.Data.TitheEnvelopeDesign[i].Category);
				else if (insbelow + 1 == i)
					Assert.Equal("99 new row nine", di.Data.TitheEnvelopeDesign[i].Category);
				else if (insbelow + 1 < i)
					Assert.Equal(copy[i - 1].Category, di.Data.TitheEnvelopeDesign[i].Category);
				else if (insabove == i)
					Assert.Equal("66 new row six", di.Data.TitheEnvelopeDesign[i].Category);
				else if (insabove < i)
					Assert.Equal(copy[i].Category, di.Data.TitheEnvelopeDesign[i].Category);
				else // delrow <= i < insabove
					Assert.Equal(copy[i + 1].Category, di.Data.TitheEnvelopeDesign[i].Category);
			}
		}
	}
}
