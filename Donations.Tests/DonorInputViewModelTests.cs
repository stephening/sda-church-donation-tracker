using Donations.Model;
using Donations.ViewModel;
using System.Collections.ObjectModel;
using Xunit;
using System.Windows.Media;
using System.IO.Abstractions.TestingHelpers;

namespace Donations.Tests
{
	public partial class DataAccessSequentialTests
	{
		[Theory]
		[InlineData(123.45)]
		[InlineData(0.99)]
		[InlineData(-12.80)]
		public void DonorInputViewModel_TotalSum(double expected)
		{
			// Arrange

			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { TotalSum = expected };

			// Assert
			Assert.Equal(expected, Math.Round(obj.TotalSum, 2));
		}

		[Theory]
		[InlineData(123.45)]
		[InlineData(0.99)]
		[InlineData(-12.80)]
		public void DonorInputViewModel_BatchTotal(double expected)
		{
			// Arrange

			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { BatchTotal = expected };

			// Assert
			Assert.Equal(expected, Math.Round(obj.BatchTotal, 2));
		}

		[Fact]
		public void DonorInputViewModel_RunningTotal()
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();
			
			obj.SummaryList.Add(new Summary() { FirstName = "John", LastName = "Doe", Subtotal = 5000 });
			obj.SummaryList.Add(new Summary() { FirstName = "Jane", LastName = "Doe", Subtotal = 5000 });
			obj.SummaryList.Add(new Summary() { FirstName = "Martin", LastName = "Luther", Subtotal = 100000 });

			// Assert
			Assert.Equal(110000, Math.Round(obj.RunningTotal, 2));
		}


		[Theory]
		[InlineData("1/2/2005", "2005/01/02")]
		[InlineData("March 23, 2023", "2023/03/23")]
		[InlineData("5-5-2000", "2000/05/05")]
		[InlineData(null, "")]
		public void DonorInputViewModel_BatchDate(string? param, string expected)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { BatchDate = param };

			// Assert
			Assert.Equal(expected, obj.BatchDate);
		}

		[Theory]
		[InlineData("this is a note")]
		[InlineData("")]
		[InlineData(null)]
		public void DonorInputViewModel_BatchNote(string? expected)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { BatchNote = expected };

			// Assert
			Assert.Equal(expected, obj.BatchNote);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void DonorInputViewModel_HasChanges(bool expected)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { HasChanges = expected };

			// Assert
			Assert.Equal(expected, obj.HasChanges);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void DonorInputViewModel_CanAddRows(bool expected)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { CanAddRows = expected };

			// Assert
			Assert.Equal(expected, obj.CanAddRows);
		}

		[Fact]
		public void DonorInputViewModel_RunningTotalColor()
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Act
			obj.BatchTotal = 1000;

			// Assert
			Assert.Equal(Brushes.LightPink, obj.RunningTotalColor);

			// Act
			obj.BatchTotal = 0;

			// Assert
			Assert.Equal(Brushes.LightGreen, obj.RunningTotalColor);
		}

		[Theory]
		[InlineData(enumMethod.Unknown)]
		[InlineData(enumMethod.Cash)]
		[InlineData(enumMethod.Check)]
		[InlineData(enumMethod.Card)]
		[InlineData(enumMethod.Mixed)]
		[InlineData(enumMethod.Online)]
		[InlineData(enumMethod.AdventistGiving)]
		public void DonorInputViewModel_MethodOptions(enumMethod param)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { MethodOptions = param };

			// Assert
			Assert.Equal(param, obj.MethodOptions);
		}

		[Fact]
		public void DonorInputViewModel_SubmitEnabled()
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Act
			obj.NameSelectionChanged(null);

			// Assert
			Assert.False(obj.SubmitEnabled);

			// Act again
			obj.NameSelectionChanged(new Donor() { LastName = "Doe", FirstName = "John" });
			obj.TotalSum = 0;

			// Assert again
			Assert.False(obj.SubmitEnabled);

			// Act again
			obj.TotalSum = 10;
			obj.BatchDate = "";

			// Assert again
			Assert.False(obj.SubmitEnabled);

			// Act again
			obj.BatchDate = "1/1/1980";
			obj.MethodOptions = enumMethod.Unknown;

			// Assert again
			Assert.False(obj.SubmitEnabled);

			// Act again
			obj.MethodOptions = enumMethod.Cash;

			// Assert again
			Assert.True(obj.SubmitEnabled);

			// Act again
			obj.MethodOptions = enumMethod.Check;
			obj.CheckNumber = "";

			// Assert again
			Assert.False(obj.SubmitEnabled);

			// Act again
			obj.CheckNumber = "10000";

			// Assert again
			Assert.True(obj.SubmitEnabled);
		}

		[Fact]
		public void DonorInputViewModel_SubmitBatchEnabled()
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Assert
			Assert.False(obj.SubmitBatchEnabled);

			// Act
			obj.HasChanges = true;
			obj.BatchDate = "1/1/1979";

			// Assert
			Assert.True(obj.SubmitBatchEnabled);
		}

		[Theory]
		[InlineData("John", "Doe", "Doe, John")]
		[InlineData("", "Doe", "Doe")]
		[InlineData("John", "", "John")]
		[InlineData(null, "Doe", "Doe")]
		[InlineData("John", null, "John")]
		public void DonorInputViewModel_Name(string? firstName, string? lastName, string expected)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Act
			obj.NameSelectionChanged(new Donor() { FirstName = firstName, LastName = lastName });

			// Assert
			Assert.Equal(expected, obj.Name);
		}

		[Theory]
		[InlineData("This is an address")]
		[InlineData(null)]
		[InlineData("")]
		public void DonorInputViewModel_Address(string? param)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Act
			obj.NameSelectionChanged(new Donor() { Address = param });

			// Assert
			Assert.Equal(param, obj.Address);
		}

		[Theory]
		[InlineData("This is the city")]
		[InlineData(null)]
		[InlineData("")]
		public void DonorInputViewModel_City(string? param)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Act
			obj.NameSelectionChanged(new Donor() { City = param });

			// Assert
			Assert.Equal(param, obj.City);
		}

		[Theory]
		[InlineData("This is the state")]
		[InlineData(null)]
		[InlineData("")]
		public void DonorInputViewModel_State(string? param)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Act
			obj.NameSelectionChanged(new Donor() { State = param });

			// Assert
			Assert.Equal(param, obj.State);
		}

		[Theory]
		[InlineData("99999")]
		[InlineData(null)]
		[InlineData("")]
		public void DonorInputViewModel_Zip(string? param)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Act
			obj.NameSelectionChanged(new Donor() { Zip = param });

			// Assert
			Assert.Equal(param, obj.Zip);
		}

		[Fact]
		public void DonorInputViewModel_CheckNumberEnabled()
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Act
			obj.MethodOptions = enumMethod.Check;

			// Assert
			Assert.True(obj.CheckNumberEnabled);

			// Act again
			obj.MethodOptions = enumMethod.AdventistGiving;

			// Assert again
			Assert.False(obj.CheckNumberEnabled);

			// Act again
			obj.MethodOptions = enumMethod.Cash;

			// Assert again
			Assert.False(obj.CheckNumberEnabled);
		}

		[Theory]
		[InlineData(enumMethod.Unknown, true)]
		[InlineData(enumMethod.Cash, true)]
		[InlineData(enumMethod.Check, true)]
		[InlineData(enumMethod.Card, true)]
		[InlineData(enumMethod.Mixed, true)]
		[InlineData(enumMethod.Online, true)]
		[InlineData(enumMethod.AdventistGiving, false)]
		public void DonorInputViewModel_NotAdventistGiving(enumMethod method, bool expected)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { MethodOptions = method };

			// Assert
			Assert.Equal(expected, obj.NotAdventistGiving);
		}

		[Theory]
		[InlineData("99999")]
		[InlineData(null)]
		[InlineData("")]
		public void DonorInputViewModel_CheckNumber(string? param)
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { CheckNumber = param };

			// Act

			// Assert
			Assert.Equal(param, obj.CheckNumber);
		}

		[Fact]
		public void DonorInputViewModel_SubmitDonor()
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { BatchDate = "1/1/2006" };
			obj.IndividualDonations.Add(new Donation() { Category = "1 tithe", Value = 100 });
			obj.IndividualDonations.Add(new Donation() { Category = "2 church budget", Value = 100 });
			obj.IndividualDonations.Add(new Donation() { Category = "3 tuition assistance", Value = 100 });

			// Act
			obj.NameSelectionChanged(new Donor() { LastName = "Doe", FirstName = "John" });
			obj.ValueChanged();
			obj.SubmitDonor();

			// Assert
			Assert.Equal("Doe, John", obj.SummaryList[0].Name);
			Assert.Equal(300, Math.Round(obj.RunningTotal, 2));
		}

		[Fact]
		public void DonorInputViewModel_SubmitBatch()
		{
			// Arrange
			var td = new TestData();
			di.Data.BatchDict = new Dictionary<int, Batch>();
			di.Data.BatchList = new ObservableCollection<Batch>();
			di.Data.DonationDict = new Dictionary<int, Donation>();
			di.Data.DonationList = new ObservableCollection<Donation>();

			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();
			obj.SummaryList.Add(new Summary() { DonorId = 1, FirstName = di.Data.DonorDict[1].FirstName, LastName = di.Data.DonorDict[1].LastName, Subtotal = 1145.50 });
			obj.SummaryList.Add(new Summary() { DonorId = 2, FirstName = di.Data.DonorDict[2].FirstName, LastName = di.Data.DonorDict[2].LastName, Subtotal = 687.5 });
			obj.SummaryList.Add(new Summary() { DonorId = 3, FirstName = di.Data.DonorDict[3].FirstName, LastName = di.Data.DonorDict[3].LastName, Subtotal = 1850 });

			double summaryTotal = 0;
			foreach (var summaryItem in obj.SummaryList)
			{
				summaryTotal += summaryItem.Subtotal;
			}

			obj.SummaryList[0].ItemizedDonations = new ObservableCollection<Donation>();
			obj.SummaryList[0].ItemizedDonations.Add(new Donation() { Category = "1 tithe", Value = 1000 });
			obj.SummaryList[0].ItemizedDonations.Add(new Donation() { Category = "2 church budget", Value = 100 });
			obj.SummaryList[0].ItemizedDonations.Add(new Donation() { Category = "3 tuition assistance", Value = 45.50 });
			obj.SummaryList[1].ItemizedDonations = new ObservableCollection<Donation>();
			obj.SummaryList[1].ItemizedDonations.Add(new Donation() { Category = "1 tithe", Value = 500, DonorId = 2 });
			obj.SummaryList[1].ItemizedDonations.Add(new Donation() { Category = "5 building fund", Value = 100 });
			obj.SummaryList[1].ItemizedDonations.Add(new Donation() { Category = "6 outreach", Value = 87.50 });
			obj.SummaryList[2].ItemizedDonations = new ObservableCollection<Donation>();
			obj.SummaryList[2].ItemizedDonations.Add(new Donation() { Category = "1 tithe", Value = 1500 });
			obj.SummaryList[2].ItemizedDonations.Add(new Donation() { Category = "7 evangelism", Value = 100 });
			obj.SummaryList[2].ItemizedDonations.Add(new Donation() { Category = "8 maintenance", Value = 250 });

			// Act
			obj.SubmitBatchCmd.Execute(null);

			// Assert
			Assert.Equal(9, di.Data.DonationList.Count);
			int donationIdx = 0;
			int donorId = 1;
			Assert.Equal(di.Data.DonorDict[donorId].LastName, di.Data.DonationList[donationIdx + 0].LastName);
			Assert.Equal(di.Data.DonorDict[donorId].LastName, di.Data.DonationList[donationIdx + 1].LastName);
			Assert.Equal(di.Data.DonorDict[donorId].LastName, di.Data.DonationList[donationIdx + 2].LastName);
			Assert.Equal(di.Data.DonorDict[donorId].FirstName, di.Data.DonationList[donationIdx + 0].FirstName);
			Assert.Equal(di.Data.DonorDict[donorId].FirstName, di.Data.DonationList[donationIdx + 1].FirstName);
			Assert.Equal(di.Data.DonorDict[donorId].FirstName, di.Data.DonationList[donationIdx + 2].FirstName);
			Assert.Equal("1 tithe", di.Data.DonationList[donationIdx + 0].Category);
			Assert.Equal("2 church budget", di.Data.DonationList[donationIdx + 1].Category);
			Assert.Equal("3 tuition assistance", di.Data.DonationList[donationIdx + 2].Category);

			donationIdx = 3;
			donorId = 2;
			Assert.Equal(di.Data.DonorDict[donorId].LastName, di.Data.DonationList[donationIdx + 0].LastName);
			Assert.Equal(di.Data.DonorDict[donorId].LastName, di.Data.DonationList[donationIdx + 1].LastName);
			Assert.Equal(di.Data.DonorDict[donorId].LastName, di.Data.DonationList[donationIdx + 2].LastName);
			Assert.Equal(di.Data.DonorDict[donorId].FirstName, di.Data.DonationList[donationIdx + 0].FirstName);
			Assert.Equal(di.Data.DonorDict[donorId].FirstName, di.Data.DonationList[donationIdx + 1].FirstName);
			Assert.Equal(di.Data.DonorDict[donorId].FirstName, di.Data.DonationList[donationIdx + 2].FirstName);
			Assert.Equal("1 tithe", di.Data.DonationList[donationIdx + 0].Category);
			Assert.Equal("5 building fund", di.Data.DonationList[donationIdx + 1].Category);
			Assert.Equal("6 outreach", di.Data.DonationList[donationIdx + 2].Category);

			donationIdx = 6;
			donorId = 3;
			Assert.Equal(di.Data.DonorDict[donorId].LastName, di.Data.DonationList[donationIdx + 0].LastName);
			Assert.Equal(di.Data.DonorDict[donorId].LastName, di.Data.DonationList[donationIdx + 1].LastName);
			Assert.Equal(di.Data.DonorDict[donorId].LastName, di.Data.DonationList[donationIdx + 2].LastName);
			Assert.Equal(di.Data.DonorDict[donorId].FirstName, di.Data.DonationList[donationIdx + 0].FirstName);
			Assert.Equal(di.Data.DonorDict[donorId].FirstName, di.Data.DonationList[donationIdx + 1].FirstName);
			Assert.Equal(di.Data.DonorDict[donorId].FirstName, di.Data.DonationList[donationIdx + 2].FirstName);
			Assert.Equal("1 tithe", di.Data.DonationList[donationIdx + 0].Category);
			Assert.Equal("7 evangelism", di.Data.DonationList[donationIdx + 1].Category);
			Assert.Equal("8 maintenance", di.Data.DonationList[donationIdx + 2].Category);

			double total = 0;
			foreach (var donation in di.Data.DonationList)
			{
				total += donation.Value;
			}

			Assert.Equal(Math.Round(di.Data.BatchList[0].ActualTotal, 2), Math.Round(total, 2));
			Assert.Equal(Math.Round(di.Data.BatchList[0].ActualTotal, 2), Math.Round(summaryTotal, 2));
		}

		[Fact]
		public void DonorInputViewModel_ReviewNullParameters()
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Assert
			Assert.Throws<ArgumentNullException>(() => obj.Review(null, new ObservableCollection<Donation>()));
			Assert.Throws<ArgumentNullException>(() => obj.Review(new Batch(), null));
		}

		[Fact]
		public void DonorInputViewModel_ReviewWithDonors()
		{
			// Arrange
			var td = new TestData();
			di.Data.BatchDict = new Dictionary<int, Batch>();
			di.Data.BatchList = new ObservableCollection<Batch>();

			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			di.Data.BatchDict[1] = new Batch() { Id = 1, Total = 30250.5 }; di.Data.BatchList.Add(di.Data.BatchDict[1]);
			di.Data.DonationList = td.DonationList;
			di.Data.DonationDict = td.DonationDict;

			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Act
			obj.Review(di.Data.BatchDict[1], di.Data.DonationList);

			// Assert
			int summaryIdx = 0;
			int donationIdx = 0;
			Assert.Equal(di.Data.DonationList[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
			Assert.Equal(di.Data.DonationList[donationIdx + 1].Name, obj.SummaryList[summaryIdx].Name);
			Assert.Equal(10100, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

			summaryIdx++;
			donationIdx += 2;
			Assert.Equal(di.Data.DonationList[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
			Assert.Equal(500, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

			summaryIdx++;
			donationIdx += 1;
			Assert.Equal(di.Data.DonationList[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
			Assert.Equal(di.Data.DonationList[donationIdx + 1].Name, obj.SummaryList[summaryIdx].Name);
			Assert.Equal(di.Data.DonationList[donationIdx + 2].Name, obj.SummaryList[summaryIdx].Name);
			Assert.Equal(19500, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

			summaryIdx++;
			donationIdx += 3;
			Assert.Equal(di.Data.DonationList[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
			Assert.Equal(di.Data.DonationList[donationIdx + 1].Name, obj.SummaryList[summaryIdx].Name);
			Assert.Equal(63, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

			summaryIdx++;
			donationIdx += 2;
			Assert.Equal(di.Data.DonationList[donationIdx + 0].Name, obj.SummaryList[summaryIdx].Name);
			Assert.Equal(87.5, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

			double summaryTotal = 0;
			foreach (var summaryItem in obj.SummaryList)
			{
				summaryTotal += summaryItem.Subtotal;
			}

			double actualTotal = 0;
			foreach (var donation in di.Data.DonationList)
			{
				actualTotal += donation.Value;
			}

			Assert.Equal(Math.Round(summaryTotal, 2), Math.Round(actualTotal, 2));
			Assert.Equal(Math.Round(summaryTotal, 2), Math.Round(di.Data.BatchList[0].Total, 2));
		}

		[Fact]
		public void DonorInputViewModel_ReviewWithoutDonors()
		{
			// Arrange
			var td = new TestData();
			di.Data.BatchDict = new Dictionary<int, Batch>();
			di.Data.BatchList = new ObservableCollection<Batch>();

			di.Data.BatchDict[1] = new Batch() { Id = 1, Total = 30250.5 }; di.Data.BatchList.Add(di.Data.BatchDict[1]);
			di.Data.DonationList = td.DonationList;
			di.Data.DonationDict = td.DonationDict;

			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Act
			obj.Review(di.Data.BatchDict[1], di.Data.DonationList);

			// Assert
			int summaryIdx = 0;
			int donationIdx = 0;
			Assert.Equal("Doe, John", obj.SummaryList[summaryIdx].Name);
			Assert.Equal("Doe, John", obj.SummaryList[summaryIdx].Name);
			Assert.Equal(10100, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

			summaryIdx++;
			donationIdx += 2;
			Assert.Equal("Doe, Jane J", obj.SummaryList[summaryIdx].Name);
			Assert.Equal(500, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

			summaryIdx++;
			donationIdx += 1;
			Assert.Equal("Doe, Johnny", obj.SummaryList[summaryIdx].Name);
			Assert.Equal("Doe, Johnny", obj.SummaryList[summaryIdx].Name);
			Assert.Equal("Doe, Johnny", obj.SummaryList[summaryIdx].Name);
			Assert.Equal(19500, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

			summaryIdx++;
			donationIdx += 3;
			Assert.Equal("Luther, Martin", obj.SummaryList[summaryIdx].Name);
			Assert.Equal("Luther, Martin", obj.SummaryList[summaryIdx].Name);
			Assert.Equal(63, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

			summaryIdx++;
			donationIdx += 2;
			Assert.Equal("Wycliffe, John", obj.SummaryList[summaryIdx].Name);
			Assert.Equal(87.5, Math.Round(obj.SummaryList[summaryIdx].Subtotal, 2));

			double summaryTotal = 0;
			foreach (var summaryItem in obj.SummaryList)
			{
				summaryTotal += summaryItem.Subtotal;
			}

			double actualTotal = 0;
			foreach (var donation in di.Data.DonationList)
			{
				actualTotal += donation.Value;
			}

			Assert.Equal(Math.Round(summaryTotal, 2), Math.Round(actualTotal, 2));
			Assert.Equal(Math.Round(summaryTotal, 2), Math.Round(di.Data.BatchList[0].Total, 2));
		}

		[Fact]
		public void DonorInputViewModel_Loaded()
		{
			// Arrange
			di.Data.TitheEnvelopeDesign = new ObservableCollection<EnvelopeEntry>();
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Assert
			Assert.Empty(obj.IndividualDonations);

			// Act
			di.Data.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "1 tithe" });
			di.Data.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "2 local church budget" });
			di.Data.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "3 student aid" });
			di.Data.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "" });
			di.Data.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "4 financial assistance" });
			di.Data.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "5 building fund" });
			di.Data.TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "6 world budget" });

			obj.Loaded();

			// Assert again
			Assert.Equal(7, obj.IndividualDonations.Count);
		}

		[Fact]
		public void DonorInputViewModel_ValueChanged()
		{
			// Arrange
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { BatchDate = "1/1/2006" };
			obj.IndividualDonations.Add(new Donation() { Category = "1 tithe", Value = 100 });
			obj.IndividualDonations.Add(new Donation() { Category = "2 church budget", Value = 100 });
			obj.IndividualDonations.Add(new Donation() { Category = "3 tuition assistance", Value = 100 });

			// Assert again
			Assert.Equal(0, obj.TotalSum);

			// Act
			obj.ValueChanged();

			// Assert again
			Assert.Equal(300, obj.TotalSum);
		}

		[Fact]
		public void DonorInputViewModel_ChangeCategory()
		{
			// Arrange
			di.Data.TitheEnvelopeDesign = new ObservableCollection<EnvelopeEntry>();
			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel() { BatchDate = "1/1/2006" };
			obj.IndividualDonations.Add(new Donation() { Category = "1 tithe", Value = 100 });
			obj.IndividualDonations.Add(new Donation() { Category = "2 church budget", Value = 100 });
			obj.IndividualDonations.Add(new Donation() { Category = "3 tuition assistance", Value = 100 });

			// Act
			obj.ChangeCategory(obj.IndividualDonations[2], new Category() { Code = 4, Description = "international missions", TaxDeductible = true });

			// Assert again
			Assert.Equal("1 tithe", obj.IndividualDonations[0].Category);
			Assert.Equal("2 church budget", obj.IndividualDonations[1].Category);
			Assert.Equal("4 international missions", obj.IndividualDonations[2].Category);
		}

		[Fact]
		public void DonorInputViewModel_ChooseDonor()
		{
			// Arrange
			var td = new TestData();
			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});
			DonorInputViewModel obj = new DonorInputViewModel();

			// Act
			obj.ChooseDonor(3);

			// Assert
			Assert.Equal(di.Data.DonorDict[3].Name, obj.Name);
			Assert.Throws<Exception>(() => obj.ChooseDonor(10));
			Assert.Throws<Exception>(() => obj.ChooseDonor(-1));
		}

		[Theory]
		[InlineData(0)]
		[InlineData(1)]
		[InlineData(2)]
		public void DonorInputViewModel_SummarySelectionChanged(int index)
		{
			// Arrange
			var td = new TestData();
			di.Data.BatchDict = new Dictionary<int, Batch>();
			di.Data.BatchList = new ObservableCollection<Batch>();
			di.Data.DonationDict = new Dictionary<int, Donation>();
			di.Data.DonationList = new ObservableCollection<Donation>();

			di.Data.DonorDict = td.DonorDict;
			di.Data.DonorList = td.DonorList;

			// https://github.com/TestableIO/System.IO.Abstractions
			di.FileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
			{
				{ @"summarylist.xml", new MockFileData("") },
				{ @"date_total.txt", new MockFileData("") }
			});

			DonorInputViewModel obj = new DonorInputViewModel();
			obj.SummaryList.Add(new Summary() { DonorId = 1, FirstName = di.Data.DonorDict[1].FirstName, LastName = di.Data.DonorDict[1].LastName, Subtotal = 1145.50 });
			obj.SummaryList.Add(new Summary() { DonorId = 2, FirstName = di.Data.DonorDict[2].FirstName, LastName = di.Data.DonorDict[2].LastName, Subtotal = 687.5 });
			obj.SummaryList.Add(new Summary() { DonorId = 3, FirstName = di.Data.DonorDict[3].FirstName, LastName = di.Data.DonorDict[3].LastName, Subtotal = 1850 });

			double summaryTotal = 0;
			foreach (var summaryItem in obj.SummaryList)
			{
				summaryTotal += summaryItem.Subtotal;
			}

			obj.SummaryList[0].ItemizedDonations = new ObservableCollection<Donation>();
			obj.SummaryList[0].ItemizedDonations.Add(new Donation() { Category = "1 tithe", Value = 1000, DonorId = 1, LastName = di.Data.DonorDict[1].LastName, FirstName = di.Data.DonorDict[1].FirstName });
			obj.SummaryList[0].ItemizedDonations.Add(new Donation() { Category = "2 church budget", Value = 100, DonorId = 1, LastName = di.Data.DonorDict[1].LastName, FirstName = di.Data.DonorDict[1].FirstName });
			obj.SummaryList[0].ItemizedDonations.Add(new Donation() { Category = "3 tuition assistance", Value = 45.50, DonorId = 1, LastName = di.Data.DonorDict[1].LastName, FirstName = di.Data.DonorDict[1].FirstName });
			obj.SummaryList[1].ItemizedDonations = new ObservableCollection<Donation>();
			obj.SummaryList[1].ItemizedDonations.Add(new Donation() { Category = "1 tithe", Value = 500, DonorId = 2, LastName = di.Data.DonorDict[2].LastName, FirstName = di.Data.DonorDict[2].FirstName });
			obj.SummaryList[1].ItemizedDonations.Add(new Donation() { Category = "5 building fund", Value = 100, DonorId = 2, LastName = di.Data.DonorDict[2].LastName, FirstName = di.Data.DonorDict[2].FirstName });
			obj.SummaryList[1].ItemizedDonations.Add(new Donation() { Category = "6 outreach", Value = 87.50, DonorId = 2, LastName = di.Data.DonorDict[2].LastName, FirstName = di.Data.DonorDict[2].FirstName });
			obj.SummaryList[2].ItemizedDonations = new ObservableCollection<Donation>();
			obj.SummaryList[2].ItemizedDonations.Add(new Donation() { Category = "1 tithe", Value = 1500, DonorId = 3, LastName = di.Data.DonorDict[3].LastName, FirstName = di.Data.DonorDict[3].FirstName });
			obj.SummaryList[2].ItemizedDonations.Add(new Donation() { Category = "7 evangelism", Value = 100, DonorId = 3, LastName = di.Data.DonorDict[3].LastName, FirstName = di.Data.DonorDict[3].FirstName });
			obj.SummaryList[2].ItemizedDonations.Add(new Donation() { Category = "8 maintenance", Value = 250, DonorId = 3, LastName = di.Data.DonorDict[3].LastName, FirstName = di.Data.DonorDict[3].FirstName });

			// Act
			obj.SummarySelectionChanged(index);
			obj.ValueChanged(); // add up individual sum

			// Assert
			Assert.Equal(obj.SummaryList[index].Name, obj.Name);
			Assert.Equal(Math.Round(obj.SummaryList[index].Subtotal, 2), Math.Round(obj.TotalSum, 2));

			// Act again
			di.Data.DonorDict.Clear();
			di.Data.DonorList.Clear();
			obj.SummarySelectionChanged(index);
			obj.ValueChanged(); // add up individual sum

			// Assert again
			Assert.Equal(obj.SummaryList[index].Name, obj.Name);
			Assert.Equal(Math.Round(obj.SummaryList[index].Subtotal, 2), Math.Round(obj.TotalSum, 2));
		}

		[Fact]
		public void Reset()
		{
			// Arrange
			var td = new TestData();
			di.Data.TitheEnvelopeDesign = td.TitheEnvelopeDesign;

			DonorInputViewModel obj = new DonorInputViewModel() { HasChanges = true, BatchDate = "1/2/2023", BatchNote = "Note", BatchTotal = 100 };
			obj.SummaryList.Add(new Summary());
			obj.IndividualDonations.Add(new Donation());

			// Act
			obj.Reset();

			// Assert
			Assert.False(obj.HasChanges);
			Assert.Equal("", obj.BatchDate);
			Assert.Equal("", obj.BatchNote);
			Assert.Equal(0, obj.BatchTotal);
			Assert.Empty(obj.SummaryList);
			Assert.Equal(td.TitheEnvelopeDesign.Count, obj.IndividualDonations.Count);
		}
	}
}
