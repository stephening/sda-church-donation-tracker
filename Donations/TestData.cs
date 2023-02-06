using Donations.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donations.Tests
{
	public class TestData
	{
		public ObservableCollection<Donor>? DonorList = new ObservableCollection<Donor>();
		public Dictionary<int, Donor>? DonorDict = new Dictionary<int, Donor>();
		public ObservableCollection<Donation>? DonationList = new ObservableCollection<Donation>();
		public Dictionary<int, Donation>? DonationDict = new Dictionary<int, Donation>();
		public ObservableCollection<Batch>? BatchList = new ObservableCollection<Batch>();
		public Dictionary<int, Batch>? BatchDict = new Dictionary<int, Batch>();
		public ObservableCollection<EnvelopeEntry>? TitheEnvelopeDesign = new ObservableCollection<EnvelopeEntry>();
		public Dictionary<int, Category>? CatDict = new Dictionary<int, Category>();
		public ObservableCollection<Category>? CatList = new ObservableCollection<Category>();
		public ObservableCollection<AGDonorMapItem>? AGDonorMapList = new ObservableCollection<AGDonorMapItem>();
		public Dictionary<string, AGDonorMapItem>? AGDonorMap = new Dictionary<string, AGDonorMapItem>();
		public ObservableCollection<AGCategoryMapItem>? AGCategoryMapList = new ObservableCollection<AGCategoryMapItem>();
		public Dictionary<int, AGCategoryMapItem>? AGCategoryMap = new Dictionary<int, AGCategoryMapItem>();
		public ObservableCollection<AdventistGiving>? AdventistGivingList = new ObservableCollection<AdventistGiving>();

		public string AdventistGivingCsv;
		public string CategoriesCsv;
		public string DonationsCsv;
		public string DonorsCsv;

		public TestData()
		{
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "1 tithe" });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "2 two" });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "3 three" });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "4 four" });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "" });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "6 six" });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "7 seven" });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Category = "21 twenty one" });

			CatDict[1 ] = new Category() { Code = 1, Description = "tithe", TaxDeductible = true }; CatList.Add(CatDict[1]);
			CatDict[2 ] = new Category() { Code = 2, Description = "two", TaxDeductible = true }; CatList.Add(CatDict[2]);
			CatDict[3 ] = new Category() { Code = 3, Description = "three", TaxDeductible = true }; CatList.Add(CatDict[3]);
			CatDict[4 ] = new Category() { Code = 4, Description = "four", TaxDeductible = false }; CatList.Add(CatDict[4]);
			CatDict[5 ] = new Category() { Code = 5, Description = "five", TaxDeductible = true }; CatList.Add(CatDict[5]);
			CatDict[6 ] = new Category() { Code = 6, Description = "six", TaxDeductible = true }; CatList.Add(CatDict[6]);
			CatDict[7 ] = new Category() { Code = 7, Description = "seven", TaxDeductible = true }; CatList.Add(CatDict[7]);
			CatDict[8 ] = new Category() { Code = 8, Description = "eight", TaxDeductible = true }; CatList.Add(CatDict[8]);
			CatDict[9 ] = new Category() { Code = 9, Description = "nine", TaxDeductible = true }; CatList.Add(CatDict[9]);
			CatDict[20] = new Category() { Code = 20, Description = "twenty", TaxDeductible = false }; CatList.Add(CatDict[20]);
			CatDict[21] = new Category() { Code = 21, Description = "twenty one", TaxDeductible = true }; CatList.Add(CatDict[21]);
			CatDict[22] = new Category() { Code = 22, Description = "twenty two", TaxDeductible = true }; CatList.Add(CatDict[22]);
			CatDict[23] = new Category() { Code = 23, Description = "twenty three", TaxDeductible = true }; CatList.Add(CatDict[23]);
			CatDict[24] = new Category() { Code = 24, Description = "twenty four", TaxDeductible = true }; CatList.Add(CatDict[24]);

			CategoriesCsv = "Code, Description, TaxDeductible\r\n"
				+ "1, tithe, true\r\n"
				+ "2, two, true\r\n"
				+ "3, three, true\r\n"
				+ "4, four, false\r\n"
				+ "5, five, true\r\n"
				+ "6, six, true\r\n"
				+ "7, seven, true\r\n"
				+ "8, eight, true\r\n"
				+ "9, nine, true\r\n"
				+ "20, twenty, false\r\n"
				+ "21, twenty one, true\r\n"
				+ "22, twenty two, true\r\n"
				+ "23, twenty three, true\r\n"
				+ "24, twenty four, true\r\n";

			DonorDict[1] = new Donor() { Id = 1, FamilyId = 1,    FamilyRelationship = enumFamilyRelationship.Primary, FirstName = "John",    PreferredName = "",       LastName = "Doe",      Gender = enumGender.Male,   Email = "john.doe@email.com",        Email2 = null, HomePhone = "666-2323", MobilePhone = "555-1212", WorkPhone = "111-222-3333", AddressType = enumAddressType.Mailing, Address = "1234 Acme Lane",     Address2 = "Apt A", City = "Dearborn",   State = "MI",              Zip = "48124", Country = "USA",         Birthday = "02/04/1990", Baptism = "5/7/2005", Deathday = null,              GroupGiving = true,  ChurchMember = true,  MaritalStatus = enumMaritalStatus.Married, Notes = "John doe notes",        ActiveGroups = "Church directory",      LastUpdated = "2023/1/25", AltAddressType = enumAddressType.Residential, AltAddress = "Home away from home", AltAddress2 = "second address line", AltCity = "Alt city", AltState = "Alt state", AltZip = "12345", AltCountry = "where in the world" }; DonorList.Add(DonorDict[1]);
			DonorDict[2] = new Donor() { Id = 2, FamilyId = 1,    FamilyRelationship = enumFamilyRelationship.Wife,    FirstName = "Jane J",  PreferredName = "",       LastName = "Doe",      Gender = enumGender.Female, Email = "jane.doe@email.com",        Email2 = null, HomePhone = null,       MobilePhone = "555-1212", WorkPhone = "222-333-4444", AddressType = enumAddressType.Both,    Address = "1235 Acme Lane",     Address2 = "Apt B", City = "Dearborn",   State = "MI",              Zip = "48124", Country = "USA",         Birthday = "6/7/1990",   Baptism = "5/7/2005", Deathday = null,              GroupGiving = true,  ChurchMember = true,  MaritalStatus = enumMaritalStatus.Married, Notes = "Jane doe notes",        ActiveGroups = "one, two, three",       LastUpdated = "2022/1/25", AltAddressType = null, }; DonorList.Add(DonorDict[2]);
			DonorDict[3] = new Donor() { Id = 3, FamilyId = 1,    FamilyRelationship = enumFamilyRelationship.Son,     FirstName = "Johnny",  PreferredName = "",       LastName = "Doe",      Gender = enumGender.Male,   Email = "johnny.doe@email.com",      Email2 = null, HomePhone = "777-3434", MobilePhone = "555-1212", WorkPhone = null,           AddressType = enumAddressType.Both,    Address = "1237 Acme Lane",     Address2 = "Apt C", City = "Dearborn",   State = "MI",              Zip = "48124", Country = "USA",         Birthday = null,         Baptism = null,       Deathday = null,              GroupGiving = true,  ChurchMember = false, MaritalStatus = enumMaritalStatus.Single,  Notes = null,                    ActiveGroups = "children's ministries", LastUpdated = "2021/1/25", AltAddressType = null, }; DonorList.Add(DonorDict[3]);
			DonorDict[4] = new Donor() { Id = 4, FamilyId = null,                                                      FirstName = "Martin",  PreferredName = "Marty",  LastName = "Luther",   Gender = enumGender.Male,   Email = "martin.luther@email.com",   Email2 = null, HomePhone = null,       MobilePhone = "555-1212", WorkPhone = "555-666-7777", AddressType = enumAddressType.Both,    Address = "1430 Luther Circle",                     City = "Eisleben",   State = "State",           Zip = "98765", Country = "Germany",     Birthday = "1483/11/10", Baptism = null,       Deathday = "1546/2/18",       GroupGiving = false, ChurchMember = null,  MaritalStatus = enumMaritalStatus.Married, Notes = "Martin luther notes",   ActiveGroups = null,                    LastUpdated = "2020/1/25", AltAddressType = null, }; DonorList.Add(DonorDict[4]);
			DonorDict[5] = new Donor() { Id = 5, FamilyId = null,                                                      FirstName = "John",    PreferredName = "Johnny", LastName = "Wycliffe", Gender = enumGender.Male,   Email = "john.wycliffe@email.com",   Email2 = null, HomePhone = "888-4545", MobilePhone = "555-1212", WorkPhone = null,           AddressType = enumAddressType.Both,    Address = "1330 Wycliffe Lane",                     City = "Oxford",     State = "United Kingdom",  Zip = "98765", Country = "England",     Birthday = "1328",       Baptism = null,       Deathday = "1384/12/31",      GroupGiving = null,  ChurchMember = null,  MaritalStatus = enumMaritalStatus.Married, Notes = "John Wycliffe notes",   ActiveGroups = null,                    LastUpdated = "2019/1/25", AltAddressType = null, }; DonorList.Add(DonorDict[5]);
			DonorDict[6] = new Donor() { Id = 6, FamilyId = null,                                                      FirstName = "John",    PreferredName = "",       LastName = "Calvin",   Gender = enumGender.Male,   Email = "john.calvin@email.com",     Email2 = null, HomePhone = null,       MobilePhone = "555-1212", WorkPhone = null,           AddressType = enumAddressType.Both,    Address = "1509 Calvin Ct",                         City = "Johnsville", State = "Geneva",          Zip = "56789", Country = "Switzerland", Birthday = "1509/7/10",  Baptism = null,       Deathday = "5/27/1564",       GroupGiving = null,  ChurchMember = true,  MaritalStatus = enumMaritalStatus.Unknown, Notes = "John Calvin notes",     ActiveGroups = null,                    LastUpdated = "2018/1/25", AltAddressType = null, }; DonorList.Add(DonorDict[6]);
			DonorDict[7] = new Donor() { Id = 7, FamilyId = null,                                                      FirstName = "William", PreferredName = "",       LastName = "Tyndale",  Gender = enumGender.Male,   Email = "william.tyndale@email.com", Email2 = null, HomePhone = "999-5656", MobilePhone = "555-1212", WorkPhone = null,           AddressType = enumAddressType.Both,    Address = "1494 Tyndale Way",                       City = "Gloucester", State = "Gloucestershire", Zip = "56789", Country = "England",     Birthday = "1494",       Baptism = null,       Deathday = "October 6, 1536", GroupGiving = false, ChurchMember = true,  MaritalStatus = null,                      Notes = "William Tyndale notes", ActiveGroups = null,                    LastUpdated = "2017/1/25", AltAddressType = null, }; DonorList.Add(DonorDict[7]);

			DonorsCsv = "Id,FamilyId,FamilyRelationship,FirstName,PreferredName,LastName,Gender,Email,Email2,HomePhone,MobilePhone,WorkPhone,AddressType,Address,Address2,City,State,Zip,Country,AltAddressType,AltAddress,AltAddress2,AltCity,AltState,AltZip,AltCountry,Birthday,Baptism,Deathday,GroupGiving,ChurchMember,MaritalStatus,Notes,ActiveGroups,LastUpdated\r\n"
				+ "1,1,Primary,John,,Doe,Male,john.doe@email.com,,666-2323,555-1212,111-222-3333,Mailing,1234 Acme Lane,Apt A,Dearborn,MI,48124,USA,Residential,Home away from home,second address line,Alt city,Alt state,12345,where in the world,02/04/1990,5/7/2005,,True,True,Married,John doe notes,Church directory,2023/1/25\r\n"
				+ "2,1,Wife,Jane J,,Doe,Female,jane.doe@email.com,,,555-1212,222-333-4444,Both,1235 Acme Lane,Apt B,Dearborn,MI,48124,USA,,,,,,,,6/7/1990,5/7/2005,,True,True,Married,Jane doe notes,\"one, two, three\",2022/1/25\r\n"
				+ "3,1,Son,Johnny,,Doe,Male,johnny.doe@email.com,,777-3434,555-1212,,Both,1237 Acme Lane,Apt C,Dearborn,MI,48124,USA,,,,,,,,,,,True,False,Single,,children's ministries,2021/1/25\r\n"
				+ "4,,,Martin,Marty,Luther,Male,martin.luther@email.com,,,555-1212,555-666-7777,Both,1430 Luther Circle,,Eisleben,State,98765,Germany,,,,,,,,1483/11/10,,1546/2/18,False,,Married,Martin luther notes,,2020/1/25\r\n"
				+ "5,,,John,Johnny,Wycliffe,Male,john.wycliffe@email.com,,888-4545,555-1212,,Both,1330 Wycliffe Lane,,Oxford,United Kingdom,98765,England,,,,,,,,1328,,1384/12/31,,,Married,John Wycliffe notes,,2019/1/25\r\n"
				+ "6,,,John,,Calvin,Male,john.calvin@email.com,,,555-1212,,Both,1509 Calvin Ct,,Johnsville,Geneva,56789,Switzerland,,,,,,,,1509/7/10,,5/27/1564,,True,Unknown,John Calvin notes,,2018/1/25\r\n"
				+ "7,,,William,,Tyndale,Male,william.tyndale@email.com,,999-5656,555-1212,,Both,1494 Tyndale Way,,Gloucester,Gloucestershire,56789,England,,,,,,,,1494,,\"October 6, 1536\",False,True,,William Tyndale notes,,2017/1/25\r\n";

			DonationDict[1] = new Donation() { Id = 1, BatchId = 1, DonorId = 1, FirstName = "John",   LastName = "Doe",      Category = "1 tithe", Value = 10000, Date = "1/2/2023",      TaxDeductible = true,  Note = "Note 1", Method = enumMethod.Check,           TransactionNumber = "10001" };    DonationList.Add(DonationDict[1]);
			DonationDict[2] = new Donation() { Id = 2, BatchId = 1, DonorId = 1, FirstName = "John",   LastName = "Doe",      Category = "2 two",   Value = 100,   Date = "2023/1/2",      TaxDeductible = true,  Note = "Note 2", Method = enumMethod.Check,           TransactionNumber = "10001" };    DonationList.Add(DonationDict[2]);
			DonationDict[3] = new Donation() { Id = 3, BatchId = 2, DonorId = 2, FirstName = "Jane J", LastName = "Doe",      Category = "1 tithe", Value = 500,   Date = "July 4, 1776",  TaxDeductible = true,  Note = "Note 3", Method = enumMethod.Online,          TransactionNumber = "*1212" };    DonationList.Add(DonationDict[3]);
			DonationDict[4] = new Donation() { Id = 4, BatchId = 3, DonorId = 3, FirstName = "Johnny", LastName = "Doe",      Category = "1 tithe", Value = 15000, Date = "1978/3/2",      TaxDeductible = true,  Note = "Note 4", Method = enumMethod.AdventistGiving, TransactionNumber = "84501346" }; DonationList.Add(DonationDict[4]);
			DonationDict[5] = new Donation() { Id = 5, BatchId = 3, DonorId = 3, FirstName = "Johnny", LastName = "Doe",      Category = "3 three", Value = 2000,  Date = "3-2-1978",      TaxDeductible = true,  Note = "Note 5", Method = enumMethod.AdventistGiving, TransactionNumber = "84501346" }; DonationList.Add(DonationDict[5]);
			DonationDict[6] = new Donation() { Id = 6, BatchId = 3, DonorId = 3, FirstName = "Johnny", LastName = "Doe",      Category = "4 four",  Value = 2500,  Date = "March 2, 1978", TaxDeductible = false, Note = "Note 6", Method = enumMethod.AdventistGiving, TransactionNumber = "84501346" }; DonationList.Add(DonationDict[6]);
			DonationDict[7] = new Donation() { Id = 7, BatchId = 4, DonorId = 4, FirstName = "Martin", LastName = "Luther",   Category = "1 tithe", Value = 25.50, Date = "8/4/1470",      TaxDeductible = true,  Note = "Note 7", Method = enumMethod.Mixed,           TransactionNumber = null };       DonationList.Add(DonationDict[7]);
			DonationDict[8] = new Donation() { Id = 8, BatchId = 4, DonorId = 4, FirstName = "Martin", LastName = "Luther",   Category = "5 five",  Value = 37.50, Date = "8/4/1470",      TaxDeductible = true,  Note = "Note 8", Method = enumMethod.Unknown,         TransactionNumber = null };       DonationList.Add(DonationDict[8]);
			DonationDict[9] = new Donation() { Id = 9, BatchId = 5, DonorId = 5, FirstName = "John",   LastName = "Wycliffe", Category = "1 tithe", Value = 87.50, Date = "12/25/1360",    TaxDeductible = true,  Note = "Note 9", Method = enumMethod.Cash,            TransactionNumber = null };       DonationList.Add(DonationDict[9]);

			DonationsCsv = "Id,DonorId,BatchId,LastName,FirstName,Category,Value,Date,TaxDeductible,Note,Method,TransactionNumber\r\n"
				+ "1,1,1,Doe,John,1 tithe,10000,1/2/2023,True,Note 1,Check,10001\r\n"
				+ "2,1,1,Doe,John,2 two,100,2023/1/2,True,Note 2,Check,10001\r\n"
				+ "3,2,2,Doe,Jane J,1 tithe,500,\"July 4, 1776\",True,Note 3,Online,*1212\r\n"
				+ "4,3,3,Doe,Johnny,1 tithe,15000,1978/3/2,True,Note 4,AdventistGiving,84501346\r\n"
				+ "5,3,3,Doe,Johnny,3 three,2000,3-2-1978,True,Note 5,AdventistGiving,84501346\r\n"
				+ "6,3,3,Doe,Johnny,4 four,2500,\"March 2, 1978\",False,Note 6,AdventistGiving,84501346\r\n"
				+ "7,4,4,Luther,Martin,1 tithe,25.5,8/4/1470,True,Note 7,Mixed,\r\n"
				+ "8,4,4,Luther,Martin,5 five,37.5,8/4/1470,True,Note 8,Unknown,\r\n"
				+ "9,5,5,Wycliffe,John,1 tithe,87.5,12/25/1360,True,Note 9,Cash,\r\n";

			BatchDict[1] = new Batch() { Id = 1, Source = enumSource.DonorInput,      Date = "1/2/2023",     Total = 10100, ActualTotal = 10100, Operator = "userid", Note = "Note 1" };          BatchList.Add(BatchDict[1]);
			BatchDict[2] = new Batch() { Id = 2, Source = enumSource.DonorInput,      Date = "July 4, 1776", Total = 500,   ActualTotal = 500,   Operator = "userid", Note = "Note 2" };          BatchList.Add(BatchDict[2]);
			BatchDict[3] = new Batch() { Id = 3, Source = enumSource.AdventistGiving, Date = "1978/3/16",    Total = 19500, ActualTotal = 19500, Operator = "userid", Note = "AdventistGiving" }; BatchList.Add(BatchDict[3]);
			BatchDict[4] = new Batch() { Id = 4, Source = enumSource.DonorInput,      Date = "8/4/1470",     Total = 63,    ActualTotal = 63,    Operator = "userid", Note = "Note 3" };          BatchList.Add(BatchDict[4]);
			BatchDict[5] = new Batch() { Id = 5, Source = enumSource.DonorInput,      Date = "12/25/1360",   Total = 87.5,  ActualTotal = 87.5,  Operator = "userid", Note = "Note 4" };          BatchList.Add(BatchDict[5]);

			AdventistGivingList.Add(new AdventistGiving() { DonorId = 1, FirstName = "John",   LastName = "Doe",      Address = "1234 Acme Lane", Address2 = "Apt A", City = "Dearborn", State = "MI",             Zip = "48124", Country = "USA",     TransactionId = "10398360", TransactionDate = "2022-12-1", TransactionTotal = 10100, Amount = 10000, CategoryCode = 1, CategoryName = "Tithe/Diezmo/Dîme", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 1, FirstName = "John",   LastName = "Doe",      Address = "1234 Acme Lane", Address2 = "Apt A", City = "Dearborn", State = "MI",             Zip = "48124", Country = "USA",     TransactionId = "10398360", TransactionDate = "2022-12-1", TransactionTotal = 10100, Amount = 100,   CategoryCode = 2, CategoryName = "two", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 2, FirstName = "Jane",   LastName = "Doe",      Address = "1235 Acme Lane", Address2 = "Apt B", City = "Dearborn", State = "MI",             Zip = "48124", Country = "USA",     TransactionId = "10983360", TransactionDate = "2022-12-3", TransactionTotal = 500,   Amount = 500,   CategoryCode = 1, CategoryName = "Tithe/Diezmo/Dîme", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 3, FirstName = "Johnny", LastName = "Doe",      Address = "1237 Acme Lane", Address2 = "Apt C", City = "Dearborn", State = "MI",             Zip = "48124", Country = "USA",     TransactionId = "61983030", TransactionDate = "2022-12-6", TransactionTotal = 19500, Amount = 15000, CategoryCode = 1, CategoryName = "Tithe/Diezmo/Dîme", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 3, FirstName = "Johnny", LastName = "Doe",      Address = "1237 Acme Lane", Address2 = "Apt C", City = "Dearborn", State = "MI",             Zip = "48124", Country = "USA",     TransactionId = "61983030", TransactionDate = "2022-12-6", TransactionTotal = 19500, Amount = 2000,  CategoryCode = 3, CategoryName = "three", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 3, FirstName = "Johnny", LastName = "Doe",      Address = "1237 Acme Lane", Address2 = "Apt C", City = "Dearborn", State = "MI",             Zip = "48124", Country = "USA",     TransactionId = "61983030", TransactionDate = "2022-12-6", TransactionTotal = 19500, Amount = 2500,  CategoryCode = 4, CategoryName = "four", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 4, FirstName = "Martin", LastName = "Luther",   Address = "1430 Luther Circle",                 City = "Eisleben", State = "State",          Zip = "98765", Country = "Germany", TransactionId = "83103960", TransactionDate = "2022-12-9", TransactionTotal = 63,    Amount = 25.50, CategoryCode = 1, CategoryName = "Tithe/Diezmo/Dîme", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 4, FirstName = "Martin", LastName = "Luther",   Address = "1430 Luther Circle",                 City = "Eisleben", State = "State",          Zip = "98765", Country = "Germany", TransactionId = "83103960", TransactionDate = "2022-12-11", TransactionTotal = 63,    Amount = 37.50, CategoryCode = 5, CategoryName = "five", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 5, FirstName = "John",   LastName = "Wycliffe", Address = "1330 Wycliffe Lane",                 City = "Oxford",   State = "United Kingdom", Zip = "98765", Country = "England", TransactionId = "98103360", TransactionDate = "2022-12-14", TransactionTotal = 87.5,  Amount = 87.50, CategoryCode = 1, CategoryName = "Tithe/Diezmo/Dîme", TransactionType = "credit" });

			foreach (var ag in AdventistGivingList)
			{
				ag.DonorHash = AdventistGiving.AGHash(ag);
			}

			AdventistGivingCsv = "Organization ID,Organization Name,ACH Transfer Date,Donor ID,First Name,Last Name,Address1,Address2,City,State,Postal Code,Country,Transaction ID,Transaction Type,Transaction Created At,Transaction Total,Code,Category Name,Amount\r\n"
				+ "ANIFBV,Church,,,John,Doe,1234 Acme Lane,Apt A,Dearborn,MI,48124,USA,10398360,credit,2022-12-1,10100,1,Tithe/Diezmo/Dîme,10000.0\r\n"
				+ "ANIFBV,Church,,,John,Doe,1234 Acme Lane,Apt A,Dearborn,MI,48124,USA,10398360,credit,2022-12-1,10100,2,two,100.00\r\n"
				+ "ANIFBV,Church,,,Jane,Doe,1235 Acme Lane,Apt B,Dearborn,MI,48124,USA,10983360,credit,2022-12-3,500,1,Tithe/Diezmo/Dîme,500.0\r\n"
				+ "ANIFBV,Church,,,Johnny,Doe,1237 Acme Lane,Apt C,Dearborn,MI,48124,USA,61983030,credit,2022-12-06,19500,1,Tithe/Diezmo/Dîme,15000.0\r\n"
				+ "ANIFBV,Church,,,Johnny,Doe,1237 Acme Lane,Apt C,Dearborn,MI,48124,USA,61983030,credit,2022-12-06,19500,3,three,2000.0\r\n"
				+ "ANIFBV,Church,,,Johnny,Doe,1237 Acme Lane,Apt C,Dearborn,MI,48124,USA,61983030,credit,2022-12-06,19500,4,four,2500.0\r\n"
				+ "ANIFBV,Church,,,Martin,Luther,1430 Luther Circle,,Eisleben,State,98765,Germany,83103960,credit,2022-12-9,63,1,Tithe/Diezmo/Dîme,25.5\r\n"
				+ "ANIFBV,Church,,,Martin,Luther,1430 Luther Circle,,Eisleben,State,98765,Germany,83103960,credit,2022-12-11,63,5,five,37.5\r\n"
				+ "ANIFBV,Church,,,John,Wycliffe,1330 Wycliffe Lane,,Oxford,United Kingdom,98765,England,98103360,credit,2022-12-14,87.50,1,Tithe/Diezmo/Dîme,87.5\r\n";

		}
	}
}
