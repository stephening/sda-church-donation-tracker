﻿using Donations.Lib.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Donations.Lib
{
	public class TestData
	{
		public AppSettings AppSettings { get; set; } = new AppSettings() { Id = 1, EmailSmtpServer = "smtp.gmail.com", EmailServerPort = 587, EmailEnableSsl = true, EmailAccount = "john.doe@gmail.com", SyncFusionLicenseKey = "SyncFusion license key placeholder" };
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
		public Dictionary<enumPrintout, PrintSettings>? PrintSettingsDict = new Dictionary<enumPrintout, PrintSettings>();
		public ObservableCollection<DonorReport>? donorReports = new ObservableCollection<DonorReport>();
		public ObservableCollection<DonorChange>? donorChanges = new ObservableCollection<DonorChange>();
		public ObservableCollection<NamedDonorReport>? namedDonorReports = new ObservableCollection<NamedDonorReport>();
		public PdfDirectory? PdfDirectory = new PdfDirectory();
		public HtmlDirectory? HtmlDirectory = new HtmlDirectory();

		public string AdventistGivingCsv;
		public string CategoriesCsv;
		public string DonationsCsv;
		public string DonorsCsv;

		public TestData()
		{
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 1 });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 2 });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 3 });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 4 });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = -1 });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 6 });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 7 });
			TitheEnvelopeDesign.Add(new EnvelopeEntry() { Code = 21 });

			CatDict[1] = new Category() { Code = 1, Description = "tithe", TaxDeductible = true }; CatList.Add(CatDict[1]);
			CatDict[2] = new Category() { Code = 2, Description = "two", TaxDeductible = true }; CatList.Add(CatDict[2]);
			CatDict[3] = new Category() { Code = 3, Description = "three", TaxDeductible = false }; CatList.Add(CatDict[3]);
			CatDict[4] = new Category() { Code = 4, Description = "four", TaxDeductible = false }; CatList.Add(CatDict[4]);
			CatDict[5] = new Category() { Code = 5, Description = "five", TaxDeductible = true }; CatList.Add(CatDict[5]);
			CatDict[6] = new Category() { Code = 6, Description = "six", TaxDeductible = true }; CatList.Add(CatDict[6]);
			CatDict[7] = new Category() { Code = 7, Description = "seven", TaxDeductible = true }; CatList.Add(CatDict[7]);
			CatDict[8] = new Category() { Code = 8, Description = "eight", TaxDeductible = true }; CatList.Add(CatDict[8]);
			CatDict[9] = new Category() { Code = 9, Description = "nine", TaxDeductible = true }; CatList.Add(CatDict[9]);
			CatDict[20] = new Category() { Code = 20, Description = "twenty", TaxDeductible = true }; CatList.Add(CatDict[20]);
			CatDict[21] = new Category() { Code = 21, Description = "twenty one", TaxDeductible = false }; CatList.Add(CatDict[21]);
			CatDict[22] = new Category() { Code = 22, Description = "twenty two", TaxDeductible = true }; CatList.Add(CatDict[22]);
			CatDict[23] = new Category() { Code = 23, Description = "twenty three", TaxDeductible = true }; CatList.Add(CatDict[23]);
			CatDict[24] = new Category() { Code = 24, Description = "twenty four", TaxDeductible = true }; CatList.Add(CatDict[24]);

			CategoriesCsv = "Code, Description, TaxDeductible\r\n"
				+ "1, tithe, true\r\n"
				+ "2, two, true\r\n"
				+ "3, three, false\r\n"
				+ "4, four, false\r\n"
				+ "5, five, true\r\n"
				+ "6, six, true\r\n"
				+ "7, seven, true\r\n"
				+ "8, eight, true\r\n"
				+ "9, nine, true\r\n"
				+ "20, twenty, true\r\n"
				+ "21, twenty one, false\r\n"
				+ "22, twenty two, true\r\n"
				+ "23, twenty three, true\r\n"
				+ "24, twenty four, true\r\n";

			DonorDict[1] = new Donor() { Id = 1, FamilyId = 1, FamilyRelationship = enumFamilyRelationship.Primary, FirstName = "John", PreferredName = "", LastName = "Doe", Gender = enumGender.Male, Email = "john.doe@email.com", Email2 = null, HomePhone = "666-2323", MobilePhone = "555-1212", WorkPhone = "111-222-3333", AddressType = enumAddressType.Mailing, Address = "1234 Acme Lane", Address2 = "Apt A", City = "Deer", State = "OR", Zip = "48124", Country = "USA", Birthday = "02/04/1990", Baptism = "5/7/2005", Deathday = null, GroupGiving = true, ChurchMember = true, MaritalStatus = enumMaritalStatus.Married, Notes = "John doe notes", ActiveGroups = "Church directory", LastUpdated = DateTime.Parse("2023/1/25"), AltAddressType = enumAddressType.Residential, AltAddress = "Home away from home", AltAddress2 = "second address line", AltCity = "Alt city", AltState = "Alt state", AltZip = "12345", AltCountry = "where in the world", DontEmailReport = true, }; DonorList.Add(DonorDict[1]);
			DonorDict[2] = new Donor() { Id = 2, FamilyId = 1, FamilyRelationship = enumFamilyRelationship.Wife, FirstName = "Jane J", PreferredName = "", LastName = "Doe", Gender = enumGender.Female, Email = "jane.doe@email.com", Email2 = null, HomePhone = null, MobilePhone = "555-1212", WorkPhone = "222-333-4444", AddressType = enumAddressType.Both, Address = "1235 Acme Lane", Address2 = "Apt B", City = "Deer", State = "OR", Zip = "48124", Country = "USA", Birthday = "6/7/1990", Baptism = "5/7/2005", Deathday = null, GroupGiving = true, ChurchMember = true, MaritalStatus = enumMaritalStatus.Married, Notes = "Jane doe notes", ActiveGroups = "one, two, three", LastUpdated = DateTime.Parse("2022/1/25"), AltAddressType = null, }; DonorList.Add(DonorDict[2]);
			DonorDict[3] = new Donor() { Id = 3, FamilyId = 1, FamilyRelationship = enumFamilyRelationship.Son, FirstName = "Johnny", PreferredName = "", LastName = "Doe", Gender = enumGender.Male, Email = "johnny.doe@email.com", Email2 = null, HomePhone = "777-3434", MobilePhone = "555-1212", WorkPhone = null, AddressType = enumAddressType.Both, Address = "1237 Acme Lane", Address2 = "Apt C", City = "Deer", State = "OR", Zip = "48124", Country = "USA", Birthday = null, Baptism = null, Deathday = null, GroupGiving = true, ChurchMember = false, MaritalStatus = enumMaritalStatus.Single, Notes = null, ActiveGroups = "children's ministries", LastUpdated = DateTime.Parse("2021/1/25"), AltAddressType = null, }; DonorList.Add(DonorDict[3]);
			DonorDict[4] = new Donor() { Id = 4, FamilyId = null, FirstName = "Martin", PreferredName = "Marty", LastName = "Luther", Gender = enumGender.Male, Email = "martin.luther@email.com", Email2 = null, HomePhone = null, MobilePhone = "555-1212", WorkPhone = "555-666-7777", AddressType = enumAddressType.Both, Address = "1430 Luther Circle", City = "Eisleben", State = "State", Zip = "98765", Country = "Germany", Birthday = "1483/11/10", Baptism = null, Deathday = "1546/2/18", GroupGiving = false, ChurchMember = null, MaritalStatus = enumMaritalStatus.Married, Notes = "Martin luther notes", ActiveGroups = null, LastUpdated = DateTime.Parse("2020/1/25"), AltAddressType = null, }; DonorList.Add(DonorDict[4]);
			DonorDict[5] = new Donor() { Id = 5, FamilyId = null, FirstName = "John", PreferredName = "Johnny", LastName = "Wycliffe", Gender = enumGender.Male, Email = "john.wycliffe@email.com", Email2 = null, HomePhone = "888-4545", MobilePhone = "555-1212", WorkPhone = null, AddressType = enumAddressType.Both, Address = "1330 Wycliffe Lane", City = "Oxford", State = "United Kingdom", Zip = "98765", Country = "England", Birthday = "1328", Baptism = null, Deathday = "1384/12/31", GroupGiving = null, ChurchMember = null, MaritalStatus = enumMaritalStatus.Married, Notes = "John Wycliffe notes", ActiveGroups = null, LastUpdated = DateTime.Parse("2019/1/25"), AltAddressType = null, }; DonorList.Add(DonorDict[5]);
			DonorDict[6] = new Donor() { Id = 6, FamilyId = null, FirstName = "John", PreferredName = "", LastName = "Calvin", Gender = enumGender.Male, Email = "john.calvin@email.com", Email2 = null, HomePhone = null, MobilePhone = "555-1212", WorkPhone = null, AddressType = enumAddressType.Both, Address = "1509 Calvin Ct", City = "Johnsville", State = "Geneva", Zip = "56789", Country = "Switzerland", Birthday = "1509/7/10", Baptism = null, Deathday = "5/27/1564", GroupGiving = null, ChurchMember = true, MaritalStatus = enumMaritalStatus.Unknown, Notes = "John Calvin notes", ActiveGroups = null, LastUpdated = DateTime.Parse("2018/1/25"), AltAddressType = null, }; DonorList.Add(DonorDict[6]);
			DonorDict[7] = new Donor() { Id = 7, FamilyId = null, FirstName = "William", PreferredName = "", LastName = "Tyndale", Gender = enumGender.Male, Email = "william.tyndale@email.com", Email2 = null, HomePhone = "999-5656", MobilePhone = "555-1212", WorkPhone = null, AddressType = enumAddressType.Both, Address = "1494 Tyndale Way", City = "Gloucester", State = "Gloucestershire", Zip = "56789", Country = "England", Birthday = "1494", Baptism = null, Deathday = "October 6, 1536", GroupGiving = false, ChurchMember = true, MaritalStatus = null, Notes = "William Tyndale notes", ActiveGroups = null, LastUpdated = DateTime.Parse("2017/1/25"), AltAddressType = null, }; DonorList.Add(DonorDict[7]);

			DonorsCsv = "Id,FamilyId,FamilyRelationship,FirstName,PreferredName,LastName,Gender,Email,Email2,HomePhone,MobilePhone,WorkPhone,AddressType,Address,Address2,City,State,Zip,Country,AltAddressType,AltAddress,AltAddress2,AltCity,AltState,AltZip,AltCountry,Birthday,Baptism,Deathday,GroupGiving,ChurchMember,MaritalStatus,Notes,ActiveGroups,LastUpdated\r\n"
				+ "1,1,Primary,John,,Doe,Male,john.doe@email.com,,666-2323,555-1212,111-222-3333,Mailing,1234 Acme Lane,Apt A,Deer,OR,48124,USA,Residential,Home away from home,second address line,Alt city,Alt state,12345,where in the world,02/04/1990,5/7/2005,,True,True,Married,John doe notes,Church directory,2023/1/25\r\n"
				+ "2,1,Wife,Jane J,,Doe,Female,jane.doe@email.com,,,555-1212,222-333-4444,Both,1235 Acme Lane,Apt B,Deer,OR,48124,USA,,,,,,,,6/7/1990,5/7/2005,,True,True,Married,Jane doe notes,\"one, two, three\",2022/1/25\r\n"
				+ "3,1,Son,Johnny,,Doe,Male,johnny.doe@email.com,,777-3434,555-1212,,Both,1237 Acme Lane,Apt C,Deer,OR,48124,USA,,,,,,,,,,,True,False,Single,,children's ministries,2021/1/25\r\n"
				+ "4,,,Martin,Marty,Luther,Male,martin.luther@email.com,,,555-1212,555-666-7777,Both,1430 Luther Circle,,Eisleben,State,98765,Germany,,,,,,,,1483/11/10,,1546/2/18,False,,Married,Martin luther notes,,2020/1/25\r\n"
				+ "5,,,John,Johnny,Wycliffe,Male,john.wycliffe@email.com,,888-4545,555-1212,,Both,1330 Wycliffe Lane,,Oxford,United Kingdom,98765,England,,,,,,,,1328,,1384/12/31,,,Married,John Wycliffe notes,,2019/1/25\r\n"
				+ "6,,,John,,Calvin,Male,john.calvin@email.com,,,555-1212,,Both,1509 Calvin Ct,,Johnsville,Geneva,56789,Switzerland,,,,,,,,1509/7/10,,5/27/1564,,True,Unknown,John Calvin notes,,2018/1/25\r\n"
				+ "7,,,William,,Tyndale,Male,william.tyndale@email.com,,999-5656,555-1212,,Both,1494 Tyndale Way,,Gloucester,Gloucestershire,56789,England,,,,,,,,1494,,\"October 6, 1536\",False,True,,William Tyndale notes,,2017/1/25\r\n";

			int dId = 1;
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 1, DonorId = 1, EnvelopeId = 1, FirstName = "John", LastName = "Doe", Category = "1 tithe", Value = 10000, Date = "2023/01/02", TaxDeductible = true, Note = "Note 1", Method = enumMethod.Check, TransactionNumber = "10001" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 1, DonorId = 1, EnvelopeId = 1, FirstName = "John", LastName = "Doe", Category = "2 two", Value = 100, Date = "2023/01/02", TaxDeductible = true, Note = "Note 2", Method = enumMethod.Check, TransactionNumber = "10001" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 2, DonorId = 2, EnvelopeId = 2, FirstName = "Jane J", LastName = "Doe", Category = "1 tithe", Value = 500, Date = "1776/07/04", TaxDeductible = true, Note = "Note 3", Method = enumMethod.Online, TransactionNumber = "*1212" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 3, DonorId = 3, EnvelopeId = 0, FirstName = "Johnny", LastName = "Doe", Category = "1 tithe", Value = 15000, Date = "1978/03/02", TaxDeductible = true, Note = "Note 4", Method = enumMethod.AdventistGiving, TransactionNumber = "84501346" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 3, DonorId = 3, EnvelopeId = 0, FirstName = "Johnny", LastName = "Doe", Category = "3 three", Value = 2000, Date = "1978/03/02", TaxDeductible = false, Note = "Note 5", Method = enumMethod.AdventistGiving, TransactionNumber = "84501346" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 3, DonorId = 3, EnvelopeId = 0, FirstName = "Johnny", LastName = "Doe", Category = "4 four", Value = 2500, Date = "1978/03/02", TaxDeductible = true, Note = "Note 6", Method = enumMethod.AdventistGiving, TransactionNumber = "84501346" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 4, DonorId = 4, EnvelopeId = 3, FirstName = "Martin", LastName = "Luther", Category = "1 tithe", Value = 25.50, Date = "1470/08/04", TaxDeductible = true, Note = "Note 7", Method = enumMethod.Mixed, TransactionNumber = null }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 4, DonorId = 4, EnvelopeId = 3, FirstName = "Martin", LastName = "Luther", Category = "5 five", Value = 37.50, Date = "1470/08/04", TaxDeductible = true, Note = "Note 8", Method = enumMethod.Unknown, TransactionNumber = null }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 5, DonorId = 5, EnvelopeId = 4, FirstName = "John", LastName = "Wycliffe", Category = "1 tithe", Value = 87.50, Date = "1360/12/25", TaxDeductible = true, Note = "Note 9", Method = enumMethod.Cash, TransactionNumber = null }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 1, EnvelopeId = 5, FirstName = "John", LastName = "Doe", Category = "1 tithe", Value = 10000, Date = "2023/01/12", TaxDeductible = true, Note = "Note 1", Method = enumMethod.Check, TransactionNumber = "10001" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 1, EnvelopeId = 5, FirstName = "John", LastName = "Doe", Category = "2 two", Value = 100, Date = "2023/01/12", TaxDeductible = true, Note = "Note 2", Method = enumMethod.Check, TransactionNumber = "10001" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 1, EnvelopeId = 5, FirstName = "John", LastName = "Doe", Category = "3 three", Value = 600, Date = "2023/01/12", TaxDeductible = false, Note = "Note 2", Method = enumMethod.Check, TransactionNumber = "10001" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 1, EnvelopeId = 5, FirstName = "John", LastName = "Doe", Category = "22 twenty two", Value = 30, Date = "2023/01/12", TaxDeductible = true, Note = "Note 2", Method = enumMethod.Check, TransactionNumber = "10001" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 2, EnvelopeId = 6, FirstName = "Jane J", LastName = "Doe", Category = "1 tithe", Value = 500, Date = "2023/01/12", TaxDeductible = true, Note = "Note 3", Method = enumMethod.Online, TransactionNumber = "*1212" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 2, EnvelopeId = 6, FirstName = "Jane J", LastName = "Doe", Category = "7 seven", Value = 45.75, Date = "2023/01/12", TaxDeductible = true, Note = "Note 3", Method = enumMethod.Online, TransactionNumber = "*1212" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 2, EnvelopeId = 6, FirstName = "Jane J", LastName = "Doe", Category = "8 eight", Value = 15.50, Date = "2023/01/12", TaxDeductible = true, Note = "Note 3", Method = enumMethod.Online, TransactionNumber = "*1212" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 2, EnvelopeId = 6, FirstName = "Jane J", LastName = "Doe", Category = "21 twenty one", Value = 39.98, Date = "2023/01/12", TaxDeductible = false, Note = "Note 3", Method = enumMethod.Online, TransactionNumber = "*1212" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 2, EnvelopeId = 6, FirstName = "Jane J", LastName = "Doe", Category = "24 twenty four", Value = 333.33, Date = "2023/01/12", TaxDeductible = true, Note = "Note 3", Method = enumMethod.Online, TransactionNumber = "*1212" }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 3, EnvelopeId = 7, FirstName = "Johnny", LastName = "Doe", Category = "1 tithe", Value = 15000, Date = "2023/01/12", TaxDeductible = true, Note = "Note 4", Method = enumMethod.Cash, TransactionNumber = null }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 3, EnvelopeId = 7, FirstName = "Johnny", LastName = "Doe", Category = "3 three", Value = 2000, Date = "2023/01/12", TaxDeductible = false, Note = "Note 5", Method = enumMethod.Cash, TransactionNumber = null }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 3, EnvelopeId = 7, FirstName = "Johnny", LastName = "Doe", Category = "4 four", Value = 2500, Date = "2023/01/12", TaxDeductible = true, Note = "Note 6", Method = enumMethod.Cash, TransactionNumber = null }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 3, EnvelopeId = 7, FirstName = "Johnny", LastName = "Doe", Category = "5 five", Value = 22.50, Date = "2023/01/12", TaxDeductible = true, Note = "Note 6", Method = enumMethod.Cash, TransactionNumber = null }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 3, EnvelopeId = 7, FirstName = "Johnny", LastName = "Doe", Category = "6 six", Value = 555.55, Date = "2023/01/12", TaxDeductible = true, Note = "Note 6", Method = enumMethod.Cash, TransactionNumber = null }; DonationList.Add(DonationDict[dId++]);
			DonationDict[dId] = new Donation() { Id = dId, BatchId = 6, DonorId = 3, EnvelopeId = 7, FirstName = "Johnny", LastName = "Doe", Category = "7 seven", Value = 2.27, Date = "2023/01/12", TaxDeductible = true, Note = "Note 6", Method = enumMethod.Cash, TransactionNumber = null }; DonationList.Add(DonationDict[dId++]);

			dId = 1;
			int bId = 1;
			DonationsCsv = "Id,DonorId,BatchId,EnvelopeId,LastName,FirstName,Category,Value,Date,TaxDeductible,Note,Method,TransactionNumber\r\n"
				+ $"{dId++},1,{bId},1,Doe,John,1 tithe,10000,1/2/2023,True,Note 1,Check,10001\r\n"
				+ $"{dId++},1,{bId++},1,Doe,John,2 two,100,2023/1/2,True,Note 2,Check,10001\r\n"
				+ $"{dId++},2,{bId++},2,Doe,Jane J,1 tithe,500,\"July 4, 1776\",True,Note 3,Online,*1212\r\n"
				+ $"{dId++},3,{bId},0,Doe,Johnny,1 tithe,15000,1978/3/2,True,Note 4,AdventistGiving,84501346\r\n"
				+ $"{dId++},3,{bId},0,Doe,Johnny,3 three,2000,3-2-1978,False,Note 5,AdventistGiving,84501346\r\n"
				+ $"{dId++},3,{bId++},0,Doe,Johnny,4 four,2500,\"March 2, 1978\",True,Note 6,AdventistGiving,84501346\r\n"
				+ $"{dId++},4,{bId},3,Luther,Martin,1 tithe,25.5,8/4/1470,True,Note 7,Mixed,\r\n"
				+ $"{dId++},4,{bId++},3,Luther,Martin,5 five,37.5,8/4/1470,True,Note 8,Unknown,\r\n"
				+ $"{dId++},5,{bId++},4,Wycliffe,John,1 tithe,87.5,12/25/1360,True,Note 9,Cash,\r\n"
				+ $"{dId++},1,{bId},5,Doe,John,1 tithe,10000,1/12/2023,True,Note 1,Check,10001\r\n"
				+ $"{dId++},1,{bId},5,Doe,John,2 two,100,2023/1/12,True,Note 2,Check,10001\r\n"
				+ $"{dId++},1,{bId},5,Doe,John,3 three,600,2023/1/12,False,Note 2,Check,10001\r\n"
				+ $"{dId++},1,{bId},5,Doe,John,22 twenty two,30,2023/1/12,True,Note 2,Check,10001\r\n"
				+ $"{dId++},2,{bId},6,Doe,Jane J,1 tithe,500,\"January 12, 2023\",True,Note 3,Online,*1212\r\n"
				+ $"{dId++},2,{bId},6,Doe,Jane J,7 seven,45.75,\"January 12, 2023\",True,Note 3,Online,*1212\r\n"
				+ $"{dId++},2,{bId},6,Doe,Jane J,8 eight,15.50,\"January 12, 2023\",True,Note 3,Online,*1212\r\n"
				+ $"{dId++},2,{bId},6,Doe,Jane J,21 twenty one,39.98,\"January 12, 2023\",False,Note 3,Online,*1212\r\n"
				+ $"{dId++},2,{bId},6,Doe,Jane J,24 twenty four,333.33,\"January 12, 2023\",True,Note 3,Online,*1212\r\n"
				+ $"{dId++},3,{bId},7,Doe,Johnny,1 tithe,15000,2023/1/12,True,Note 4,Cash,\r\n"
				+ $"{dId++},3,{bId},7,Doe,Johnny,3 three,2000,1-12-2023,False,Note 5,Cash,\r\n"
				+ $"{dId++},3,{bId},7,Doe,Johnny,4 four,2500,\"January 12, 2023\",True,Note 6,Cash,\r\n"
				+ $"{dId++},3,{bId},7,Doe,Johnny,5 five,22.50,\"January 12, 2023\",True,Note 6,Cash,\r\n"
				+ $"{dId++},3,{bId},7,Doe,Johnny,6 six,555.55,\"January 12, 2023\",True,Note 6,Cash,\r\n"
				+ $"{dId++},3,{bId},7,Doe,Johnny,7 seven,2.27,\"January 12, 2023\",True,Note 6,Cash,\r\n";

			BatchDict[1] = new Batch() { Id = 1, Source = enumSource.DonorInput, Date = "1/2/2023", Total = 10100, ActualTotal = 10100, Operator = "userid", Note = "Batch note 1" }; BatchList.Add(BatchDict[1]);
			BatchDict[2] = new Batch() { Id = 2, Source = enumSource.DonorInput, Date = "July 4, 1776", Total = 500, ActualTotal = 500, Operator = "userid", Note = "Batch note 2" }; BatchList.Add(BatchDict[2]);
			BatchDict[3] = new Batch() { Id = 3, Source = enumSource.AdventistGiving, Date = "1978/3/16", Total = 19500, ActualTotal = 19500, Operator = "userid", Note = "AdventistGiving" }; BatchList.Add(BatchDict[3]);
			BatchDict[4] = new Batch() { Id = 4, Source = enumSource.DonorInput, Date = "8/4/1470", Total = 63, ActualTotal = 63, Operator = "userid", Note = "Batch note 3" }; BatchList.Add(BatchDict[4]);
			BatchDict[5] = new Batch() { Id = 5, Source = enumSource.DonorInput, Date = "12/25/1360", Total = 87.5, ActualTotal = 87.5, Operator = "userid", Note = "Batch note 4" }; BatchList.Add(BatchDict[5]);
			BatchDict[6] = new Batch() { Id = 6, Source = enumSource.DonorInput, Date = "1/12/2023", Total = 31744.88, ActualTotal = 31744.88, Operator = "userid", Note = "Batch note 5" }; BatchList.Add(BatchDict[6]);

			AdventistGivingList.Add(new AdventistGiving() { DonorId = 1, FirstName = "John", LastName = "Doe", Address = "1234 Acme Lane", Address2 = "Apt A", City = "Deer", State = "OR", Zip = "48124", Country = "USA", TransactionId = "10398360", TransactionDate = "2022-12-1", TransactionTotal = 10100, Amount = 10000, CategoryCode = 1, CategoryName = "Tithe/Diezmo/Dîme", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 1, FirstName = "John", LastName = "Doe", Address = "1234 Acme Lane", Address2 = "Apt A", City = "Deer", State = "OR", Zip = "48124", Country = "USA", TransactionId = "10398360", TransactionDate = "2022-12-1", TransactionTotal = 10100, Amount = 100, CategoryCode = 2, CategoryName = "two", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 2, FirstName = "Jane", LastName = "Doe", Address = "1235 Acme Lane", Address2 = "Apt B", City = "Deer", State = "OR", Zip = "48124", Country = "USA", TransactionId = "10983360", TransactionDate = "2022-12-3", TransactionTotal = 500, Amount = 500, CategoryCode = 1, CategoryName = "Tithe/Diezmo/Dîme", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 3, FirstName = "Johnny", LastName = "Doe", Address = "1237 Acme Lane", Address2 = "Apt C", City = "Deer", State = "OR", Zip = "48124", Country = "USA", TransactionId = "61983030", TransactionDate = "2022-12-6", TransactionTotal = 19500, Amount = 15000, CategoryCode = 1, CategoryName = "Tithe/Diezmo/Dîme", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 3, FirstName = "Johnny", LastName = "Doe", Address = "1237 Acme Lane", Address2 = "Apt C", City = "Deer", State = "OR", Zip = "48124", Country = "USA", TransactionId = "61983030", TransactionDate = "2022-12-6", TransactionTotal = 19500, Amount = 2000, CategoryCode = 3, CategoryName = "three", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 3, FirstName = "Johnny", LastName = "Doe", Address = "1237 Acme Lane", Address2 = "Apt C", City = "Deer", State = "OR", Zip = "48124", Country = "USA", TransactionId = "61983030", TransactionDate = "2022-12-6", TransactionTotal = 19500, Amount = 2500, CategoryCode = 4, CategoryName = "four", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 4, FirstName = "Martin", LastName = "Luther", Address = "1430 Luther Circle", City = "Eisleben", State = "State", Zip = "98765", Country = "Germany", TransactionId = "83103960", TransactionDate = "2022-12-9", TransactionTotal = 63, Amount = 25.50, CategoryCode = 1, CategoryName = "Tithe/Diezmo/Dîme", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 4, FirstName = "Martin", LastName = "Luther", Address = "1430 Luther Circle", City = "Eisleben", State = "State", Zip = "98765", Country = "Germany", TransactionId = "83103960", TransactionDate = "2022-12-11", TransactionTotal = 63, Amount = 37.50, CategoryCode = 5, CategoryName = "five", TransactionType = "credit" });
			AdventistGivingList.Add(new AdventistGiving() { DonorId = 5, FirstName = "John", LastName = "Wycliffe", Address = "1330 Wycliffe Lane", City = "Oxford", State = "United Kingdom", Zip = "98765", Country = "England", TransactionId = "98103360", TransactionDate = "2022-12-14", TransactionTotal = 87.5, Amount = 87.50, CategoryCode = 1, CategoryName = "Tithe/Diezmo/Dîme", TransactionType = "credit" });

			foreach (var ag in AdventistGivingList)
			{
				ag.DonorHash = Helper.AGHash(ag);
			}

			AdventistGivingCsv =
				"Organization ID,Organization Name,ACH Transfer Date,Donor ID, First Name,Last Name,Address1,          Address2,City,    State,         Postal Code,Country,Transaction ID,Transaction Type, Transaction Created At,Transaction Total,Code,Category Name,    Amount\r\n"
				+ "ANIFBV,       Church,                            ,        , John,      Doe,      1234 Acme Lane,    Apt A,   Deer,OR,            48124,      USA,    10398360,      credit,           2022-12-1,             10100,            1,   Tithe/Diezmo/Dîme,10000.0\r\n"
				+ "ANIFBV,       Church,                            ,        , John,      Doe,      1234 Acme Lane,    Apt A,   Deer,OR,            48124,      USA,    10398360,      credit,           2022-12-1,             10100,            2,   two,              100.00\r\n"
				+ "ANIFBV,       Church,                            ,        , Jane,      Doe,      1235 Acme Lane,    Apt B,   Deer,OR,            48124,      USA,    10983360,      credit,           2022-12-3,             500,              1,   Tithe/Diezmo/Dîme,500.0\r\n"
				+ "ANIFBV,       Church,                            ,        , Johnny,    Doe,      1237 Acme Lane,    Apt C,   Deer,OR,            48124,      USA,    61983030,      credit,           2022-12-06,            19500,            1,   Tithe/Diezmo/Dîme,15000.0\r\n"
				+ "ANIFBV,       Church,                            ,        , Johnny,    Doe,      1237 Acme Lane,    Apt C,   Deer,OR,            48124,      USA,    61983030,      credit,           2022-12-06,            19500,            3,   three,            2000.0\r\n"
				+ "ANIFBV,       Church,                            ,        , Johnny,    Doe,      1237 Acme Lane,    Apt C,   Deer,OR,            48124,      USA,    61983030,      credit,           2022-12-06,            19500,            4,   four,             2500.0\r\n"
				+ "ANIFBV,       Church,                            ,        , Martin,    Luther,   1430 Luther Circle,     ,   Eisleben,State,         98765,      Germany,83103960,      credit,           2022-12-9,             63,               1,   Tithe/Diezmo/Dîme,25.5\r\n"
				+ "ANIFBV,       Church,                            ,        , Martin,    Luther,   1430 Luther Circle,     ,   Eisleben,State,         98765,      Germany,83103960,      credit,           2022-12-11,            63,               5,   five,             37.5\r\n"
				+ "ANIFBV,       Church,                            ,        , John,      Wycliffe, 1330 Wycliffe Lane,     ,   Oxford,  United Kingdom,98765,      England,98103360,      credit,           2022-12-14,            87.50,            1,   Tithe/Diezmo/Dîme,87.5\r\n";

			foreach (enumPrintout setting in Enum.GetValues(typeof(enumPrintout)))
			{
				PrintSettingsDict[setting] = new PrintSettings() { PrintoutType = (int)setting, FontFamily = "Calibri", FontSize = 12, LeftMargin = 0.8, OtherMargins = 0.5 };
			}

			donorReports.Add(new DonorReport() { DonorId = DonorList[0].Id, LastSent = DateTime.Now, Action = $"Email(password: {DonorList[0].MobilePhone})" });
			donorReports.Add(new DonorReport() { DonorId = DonorList[1].Id, LastSent = DateTime.Now, Action = $"Email(password: {DonorList[1].MobilePhone})" });
			donorReports.Add(new DonorReport() { DonorId = DonorList[2].Id, LastSent = DateTime.Now, Action = $"Email(password: {DonorList[2].MobilePhone})" });
			donorReports.Add(new DonorReport() { DonorId = DonorList[2].Id, LastSent = DateTime.Now, Action = $"Print" });

			namedDonorReports.Add(new NamedDonorReport() { LastName = DonorList[0].LastName, FirstName = DonorList[0].FirstName, Email = DonorList[0].Email, MobilePhone = DonorList[0].MobilePhone, DontEmailReport = DonorList[0].DontEmailReport, LastSent = DateTime.Now, Action = $"Email(password: {DonorList[0].MobilePhone})" });
			namedDonorReports.Add(new NamedDonorReport() { LastName = DonorList[1].LastName, FirstName = DonorList[1].FirstName, Email = DonorList[1].Email, MobilePhone = DonorList[1].MobilePhone, DontEmailReport = DonorList[1].DontEmailReport, LastSent = DateTime.Now, Action = $"Email(password: {DonorList[1].MobilePhone})" });
			namedDonorReports.Add(new NamedDonorReport() { LastName = DonorList[2].LastName, FirstName = DonorList[2].FirstName, Email = DonorList[2].Email, MobilePhone = DonorList[2].MobilePhone, DontEmailReport = DonorList[2].DontEmailReport, LastSent = DateTime.Now, Action = $"Email(password: {DonorList[2].MobilePhone})" });
			namedDonorReports.Add(new NamedDonorReport() { LastName = DonorList[3].LastName, FirstName = DonorList[3].FirstName, Email = DonorList[3].Email, MobilePhone = DonorList[3].MobilePhone, DontEmailReport = DonorList[2].DontEmailReport, LastSent = DateTime.Now, Action = $"Print" });

			donorChanges.Add(new DonorChange() { Id = 1, DonorId = DonorList[0].Id, Name = "Doe, John", WhenChanged = DateTime.Now, WhatChanged = "ChurchMember: false => true", WhoChanged = "ssing" });
			donorChanges.Add(new DonorChange() { Id = 2, DonorId = DonorList[1].Id, Name = "Doe, Jane", WhenChanged = DateTime.Now, WhatChanged = "MaritalStatus: single => married", WhoChanged = "ssing" });

			string hex = "504B03041400000008007C557359F342F5F46F020000C60800001200000058616D6C2F446F63756D656E742E78616D6CDD96516F9B3010C7BF0AF29E1742D6755DD544CA68B356CBAAA9B4ABB4B70BB9823563A3B369C33EFD8E9076D090FA7D7940C2FCEEEEEFBFE12E6709A64E1A1D6C0AA5ED54E4CE95A76168D31C0BB0A342A664AC7970A3D414E193D40F9B70321E1F871B285458125AD40E9A04A2C9706A4B48712AB60FE81145708B1B375732D3058353B1C4072782A5D4788932CB79655E3923822B7B599739EA6DAA0B0D2B85EBA95880B2D82656A0B3A940FDBEB2225828F3742EA955DE26BD35374D3E115C57C50A29A956D6495735C028AE94AB0813535123EE8EA50D72DD1B166677715CCF68B78042AA7A2A12CC0C067757ED6AE26AC529AF0D15A0DAA5FBDDC6BA6B89237469FE6A51FEE1D068D2DC1166642ACD7B7EB7588CB73FF6AE2E4D4650E6F52871A0D740EBA5CCA0D1C427754B15F6989873B2DB15A8B7A873695316C35B04AA3BE0CEEB0E7929AD3324D35EBE7D6CAEB5695F81B972487C840D373E20ADCBEC69FB57911D2906AB7DE368D9BC0AFBDB87523A5009BF815B603F36068B096A2B9D7CC483159A23651D324DD0453E60E2033EF880231FF0D1071CFB804F3EE0C4077CF61A35F6125E2F23AF9991D7CDC86B67E4F533F21A1A791D8DBC96465E4F27839E2E08766DEFB99774C315D81CD7BF90CC50EC77704D5BE7F4A0BE12E2EF21E802AC9B5B09FA625322B983DFC94F20869C1D14B2FB18871F72EB45E22FB5DF3BF781CEDC18805E74DECBB5CBEDDBCC92C74705D970B9E7E69A3C35EEBDD1BC0E002F87D66F80B3B31F40B0655E8FC198AF3C8466673795EE4D9718945C91EC4F922F46AD7B23E38423430EE5EB4B89FFA4DAE448CCE29C07751EB413DE501DC4E611694043B8FBF332FB0B504B03041400000000007C5573591177F10A0C0100000C0100000B0000005F72656C732F2E72656C73EFBBBF3C3F786D6C2076657273696F6E3D22312E302220656E636F64696E673D227574662D38223F3E3C52656C6174696F6E736869707320786D6C6E733D22687474703A2F2F736368656D61732E6F70656E786D6C666F726D6174732E6F72672F7061636B6167652F323030362F72656C6174696F6E7368697073223E3C52656C6174696F6E7368697020547970653D22687474703A2F2F736368656D61732E6D6963726F736F66742E636F6D2F7770662F323030352F31302F78616D6C2F656E74727922205461726765743D222F58616D6C2F446F63756D656E742E78616D6C222049643D22523262353538343634383231363462303122202F3E3C2F52656C6174696F6E73686970733E504B03041400000000007C55735935CDF5772A0100002A010000130000005B436F6E74656E745F54797065735D2E786D6CEFBBBF3C3F786D6C2076657273696F6E3D22312E302220656E636F64696E673D227574662D38223F3E3C547970657320786D6C6E733D22687474703A2F2F736368656D61732E6F70656E786D6C666F726D6174732E6F72672F7061636B6167652F323030362F636F6E74656E742D7479706573223E3C44656661756C7420457874656E73696F6E3D2278616D6C2220436F6E74656E74547970653D226170706C69636174696F6E2F766E642E6D732D7770662E78616D6C2B786D6C22202F3E3C44656661756C7420457874656E73696F6E3D2272656C732220436F6E74656E74547970653D226170706C69636174696F6E2F766E642E6F70656E786D6C666F726D6174732D7061636B6167652E72656C6174696F6E73686970732B786D6C22202F3E3C2F54797065733E504B010214001400000008007C557359F342F5F46F020000C608000012000000000000000000000000000000000058616D6C2F446F63756D656E742E78616D6C504B010214001400000000007C5573591177F10A0C0100000C0100000B000000000000000000000000009F0200005F72656C732F2E72656C73504B010214001400000000007C55735935CDF5772A0100002A0100001300000000000000000000000000D40300005B436F6E74656E745F54797065735D2E786D6C504B05060000000003000300BA0000002F0500000000";
			List<byte> bytes = new List<byte>();

			for (int i = 0; i < hex.Length; i += 2)
			{
				bytes.Add(Convert.ToByte(hex.Substring(i, 2), 16));
			}
			PdfDirectory.PageHeight = 8.0;
			PdfDirectory.PageWidth = 5.5;
			PdfDirectory.CoverRtf = bytes.ToArray();

			HtmlDirectory.IncludeNonMembers = true;
			HtmlDirectory.OrderByLast = true;
			HtmlDirectory.OrderByFirst = true;
			HtmlDirectory.Header = @"
<!DOCTYPE HTML>

<html>
<head>
 <title>Beaverton SDA Church Photo Directory</title>
 <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />
 <meta name=""robots"" content=""noindex"" />
 <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
 <script type=""text/javascript"" src=""js/jquery.js""></script>
 <script type=""text/javascript"" src=""js/jquery.fancybox.js?v=2.1.5""></script>
 <script type=""text/javascript"" src=""js/jquery.fancybox-buttons.js?v=1.0.5""></script>
 <script type=""text/javascript"" src=""js/SmoothScroll.js""></script>
 <link rel=""stylesheet"" type=""text/css"" href=""css/bootstrap.css"" />
 <link rel=""stylesheet"" type=""text/css"" href=""css/styles.css"" />
 <link rel=""stylesheet"" type=""text/css"" href=""css/jquery.fancybox.css?v=2.1.5"" media=""screen"" />
 <link rel=""stylesheet"" type=""text/css"" href=""css/jquery.fancybox-buttons.css?v=1.0.5"" />
 <script type=""text/javascript"">
  $(document).ready(function () {
   $(""body"").css(""display"", ""none"");
   $(""body"").fadeIn(700);
   $("".fancybox"").fancybox({
    padding: 10,
    margin: 5,
    openEffect: 'elastic',
    openSpeed: 250,
    closeEffect: 'elastic',
    closeSpeed: 250,
    closeClick: true,
    helpers: {
     overlay: {
      closeClick: false
     },
     title: {
      type: 'inside', position: 'bottom'
     }
    }
   });
  });
 </script>
</head>
<body>
 <script src=""js/bootstrap.js""></script>

 <div class=""row"">
";
			HtmlDirectory.Template = @"
        <div class=""col-lg-3 col-md-4 col-sm-6 col-xs-12"">
            <div class=""person-box"">
                <div class=""person-img"">
                    <a name=""{IndexLetter}""></a>
                    <a href=""{PictureUrl}"" class=""fancybox"" title=""{FullName}""><img src=""{PictureUrl}""></a>
                </div>
                <div class=""person_txt"">
                    <div class=""SortName"">{SortName}</div>
                    <div class=""OtherName"">{OtherName}</div>
                    <div class=""ThirdLine"">{ThirdLine}</div>
                </div>
            </div>
        </div>
";
			HtmlDirectory.Footer = @"
 </div>

 <nav class=""navbar navbar-inverse navbar-fixed-bottom"">
  <div class=""container"">
   <div class=""btn-group"" role=""group"">
    <a class=""btn btn-info navbar-btn"" href=""#A""><strong>A</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#B""><strong>B</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#C""><strong>C</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#D""><strong>D</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#E""><strong>E</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#F""><strong>F</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#G""><strong>G</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#H""><strong>H</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#I""><strong>I</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#J""><strong>J</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#K""><strong>K</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#L""><strong>L</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#M""><strong>M</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#N""><strong>N</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#O""><strong>O</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#P""><strong>P</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#R""><strong>R</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#S""><strong>S</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#T""><strong>T</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#U""><strong>U</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#V""><strong>V</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#W""><strong>W</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#Y""><strong>Y</strong></a>
    <a class=""btn btn-info navbar-btn"" href=""#Z""><strong>Z</strong></a>

    <a class=""btn btn-info navbar-btn"" href=""LastNamesPhotoDirectory.html"">LAST Names <span class=""glyphicon glyphicon-sort-by-alphabet"" aria-hidden=""true""></span></a>
    <a class=""btn btn-info navbar-btn"" href=""FirstNamesPhotoDirectory.html"">FIRST Names <span class=""glyphicon glyphicon-sort-by-alphabet"" aria-hidden=""true""></span></a>
   </div>
  </div>
 </nav>

</body>
</html>
";
		}

		public static implicit operator ValueTask(TestData v)
		{
			throw new NotImplementedException();
		}
	}
}
