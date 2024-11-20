using Dapper;
using Serilog;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlCreateTables : SqlHelper
{
	public SqlCreateTables(ILogger logger)
		: base(logger)
	{ }

	public async Task CreateAllTables()
	{
		await CreateAppSettingsTable();
		await CreateBatchTable();
		await CreateCategoryMapTable();
		await CreateCategoriesTable();
		await CreateDonationsTable();
		await CreateDonorMapTable();
		await CreateDonorsTable();
		await CreateIndividualReportTable();
		await CreateOrganizationLogoTable();
		await CreateEnvelopeDesignTable();
		await CreatePdfDirectoryTable();
		await CreatePrintSettingsTable();
		await CreateenumAddressTypeTable();
		await CreateenumFamilyRelationshipTable();
		await CreateenumGenderTable();
		await CreateenumMaritalStatusTable();
		await CreateenumMethodTable();
		await CreateenumSourceTable();
		await CreateenumPrintoutTable();
	}

	public async Task CreateAppSettingsTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[AppSettings](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[SyncFusionLicenseKey] [nvarchar](100) NULL,
	[PictureBaseUrl] [nvarchar](100) NULL,
	[EmailSmtpServer] [nvarchar](100) NULL,
	[EmailServerPort] [int] NULL,
	[EmailEnableSsl] [bit] NOT NULL,
	[EmailAccount] [nvarchar](100) NULL,
 CONSTRAINT [PK_AppSettings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				await conn.ExecuteAsync(create_script, commandTimeout: 300);
			}
		}
		catch (Exception ex)
		{

		}
	}

	public async Task CreateBatchTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[Batches](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Source] [smallint] NULL,
	[Date] [nvarchar](10) NULL,
	[Total] [money] NOT NULL,
	[ActualTotal] [money] NOT NULL,
	[Operator] [nvarchar](100) NULL,
	[Note] [nvarchar](200) NULL,
 CONSTRAINT [PK_Batches] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				await conn.ExecuteAsync(create_script, commandTimeout: 300);
			}
		}
		catch (Exception ex)
		{

		}
	}

	public async Task CreateCategoryMapTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[CategoryMap](
	[AGCategoryCode] [int] NOT NULL,
	[AGCategoryName] [nvarchar](100) NULL,
	[CategoryCode] [int] NOT NULL,
 CONSTRAINT [PK_CategoryMap_1] PRIMARY KEY CLUSTERED 
(
	[AGCategoryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				await conn.ExecuteAsync(create_script, commandTimeout: 300);
			}
		}
		catch (Exception ex)
		{

		}
	}

	public async Task CreateCategoriesTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[Categories](
	[Code] [int] NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[TaxDeductible] [bit] NOT NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				await conn.ExecuteAsync(create_script, commandTimeout: 300);
			}
		}
		catch (Exception ex)
		{

		}
	}

	public async Task CreateDonationsTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[Donations](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DonorId] [int] NOT NULL,
	[BatchId] [int] NOT NULL,
	[EnvelopeId] [smallint] NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Category] [nvarchar](max) NULL,
	[Value] [money] NOT NULL,
	[Date] [nvarchar](10) NULL,
	[TaxDeductible] [bit] NOT NULL,
	[Note] [nvarchar](max) NULL,
	[Method] [smallint] NOT NULL,
	[TransactionNumber] [nvarchar](50) NULL,
 CONSTRAINT [PK_Donations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				await conn.ExecuteAsync(create_script, commandTimeout: 300);
			}
		}
		catch (Exception ex)
		{

		}
	}

	public async Task CreateDonorMapTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[DonorMap](
	[AGLastName] [nvarchar](50) NULL,
	[AGFirstName] [nvarchar](50) NULL,
	[AGAddress] [nvarchar](50) NULL,
	[AGCity] [nvarchar](50) NULL,
	[AGState] [nvarchar](50) NULL,
	[AGZip] [nvarchar](50) NULL,
	[AGDonorHash] [nvarchar](200) NOT NULL,
	[DonorId] [int] NOT NULL,
 CONSTRAINT [PK_DonorMap_1] PRIMARY KEY CLUSTERED 
(
	[AGDonorHash] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				await conn.ExecuteAsync(create_script, commandTimeout: 300);
			}
		}
		catch (Exception ex)
		{

		}
	}

	public async Task CreateDonorsTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[Donors](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FamilyId] [int] NULL,
	[FamilyRelationship] [smallint] NULL,
	[FirstName] [nvarchar](50) NULL,
	[PreferredName] [nvarchar](50) NULL,
	[LastName] [nvarchar](50) NULL,
	[Gender] [smallint] NULL,
	[Email] [nvarchar](200) NULL,
	[Email2] [nvarchar](200) NULL,
	[HomePhone] [nvarchar](50) NULL,
	[MobilePhone] [nvarchar](15) NULL,
	[WorkPhone] [nvarchar](50) NULL,
	[AddressType] [smallint] NOT NULL,
	[Address] [nvarchar](50) NULL,
	[Address2] [nvarchar](50) NULL,
	[City] [nvarchar](50) NULL,
	[State] [nvarchar](50) NULL,
	[Zip] [nvarchar](10) NULL,
	[Country] [nvarchar](50) NULL,
	[AltAddressType] [smallint] NULL,
	[AltAddress] [nvarchar](50) NULL,
	[AltAddress2] [nvarchar](50) NULL,
	[AltCity] [nvarchar](50) NULL,
	[AltState] [nvarchar](50) NULL,
	[AltZip] [nvarchar](50) NULL,
	[AltCountry] [nvarchar](50) NULL,
	[Birthday] [date] NULL,
	[Baptism] [date] NULL,
	[Deathday] [date] NULL,
	[GroupGiving] [bit] NULL,
	[ChurchMember] [bit] NULL,
	[MaritalStatus] [smallint] NULL,
	[Notes] [nvarchar](1000) NULL,
	[ActiveGroups] [nvarchar](200) NULL,
	[LastUpdated] [datetime] NULL,
	[PictureFile] [nvarchar](100) NULL,
	[Directory] [bit] NULL,
	[DontEmailReport] [bit] NULL,
	[Deceased] [bit] NULL,
 CONSTRAINT [PK_Donors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Donors]  WITH CHECK ADD  CONSTRAINT [FK_Addresstype] FOREIGN KEY([AddressType])
REFERENCES [dbo].[enumAddressType] ([Id])
GO
ALTER TABLE [dbo].[Donors] CHECK CONSTRAINT [FK_Addresstype]
GO
ALTER TABLE [dbo].[Donors]  WITH CHECK ADD  CONSTRAINT [FK_AltAddressType] FOREIGN KEY([AltAddressType])
REFERENCES [dbo].[enumAddressType] ([Id])
GO
ALTER TABLE [dbo].[Donors] CHECK CONSTRAINT [FK_AltAddressType]
GO
ALTER TABLE [dbo].[Donors]  WITH CHECK ADD  CONSTRAINT [FK_FamilyRelationship] FOREIGN KEY([FamilyRelationship])
REFERENCES [dbo].[enumFamilyRelationship] ([Id])
GO
ALTER TABLE [dbo].[Donors] CHECK CONSTRAINT [FK_FamilyRelationship]
GO
ALTER TABLE [dbo].[Donors]  WITH CHECK ADD  CONSTRAINT [FK_Gender] FOREIGN KEY([Gender])
REFERENCES [dbo].[enumGender] ([Id])
GO
ALTER TABLE [dbo].[Donors] CHECK CONSTRAINT [FK_Gender]
GO
ALTER TABLE [dbo].[Donors]  WITH CHECK ADD  CONSTRAINT [FK_MaritalStatus] FOREIGN KEY([MaritalStatus])
REFERENCES [dbo].[enumMaritalStatus] ([Id])
GO
ALTER TABLE [dbo].[Donors] CHECK CONSTRAINT [FK_MaritalStatus]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				var queries = create_script.Split("GO");
				foreach (var q in queries)
				{
					try
					{
						await conn.ExecuteAsync(q);
					}
					catch (Exception e)
					{
					}
				}
			}
		}
		catch (Exception)
		{

		}
	}

	public async Task CreateDonorReportsTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[DonorReports](
	[DonorId] [int] NOT NULL,
	[LastSent] [datetime] NULL,
	[Action] [nvarchar](100) NULL,
 CONSTRAINT [PK_DonorReports_1] PRIMARY KEY CLUSTERED 
(
	[DonorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[DonorReports]  WITH CHECK ADD  CONSTRAINT [FK_DonorId] FOREIGN KEY([DonorId])
REFERENCES [dbo].[Donors] ([Id])
GO
ALTER TABLE [dbo].[DonorReports] CHECK CONSTRAINT [FK_DonorId]
GO
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				var queries = create_script.Split("GO");
				foreach (var q in queries)
				{
					try
					{
						await conn.ExecuteAsync(q);
					}
					catch (Exception e)
					{
					}
				}
			}
		}
		catch (Exception)
		{

		}
	}

	public async Task CreateIndividualReportTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[IndividualReport](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[TemplateText] [nvarchar](max) NULL,
	[EmailSubject] [nvarchar](max) NULL,
	[EmailBody] [nvarchar](max) NULL,
	[Encrypt] [bit] NULL,
 CONSTRAINT [PK_ReportTemplate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				await conn.ExecuteAsync(create_script, commandTimeout: 300);
			}
		}
		catch (Exception ex)
		{

		}
	}

	public async Task CreatePdfDirectoryTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[PdfDirectory](
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
	[PageWidth] [float] NOT NULL,
	[PageHeight] [float] NOT NULL,
	[LeftMargin] [float] NOT NULL,
	[OtherMargins] [float] NOT NULL,
	[Font] [nvarchar](50) NOT NULL,
	[FontSize] [float] NOT NULL,
	[IncludeAddress] [bit] NOT NULL,
	[IncludePhone] [bit] NOT NULL,
	[IncludeEmail] [bit] NOT NULL,
	[IncludeNonMembers] [bit] NOT NULL,
	[CoverRtf] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_PdfDirectory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				await conn.ExecuteAsync(create_script, commandTimeout: 300);
			}
		}
		catch (Exception ex)
		{

		}
	}

	public async Task CreatePrintSettingsTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[PrintSettings](
	[PrintoutType] [smallint] NOT NULL,
	[FontFamily] [nvarchar](50) NULL,
	[FontSize] [float] NULL,
	[LeftMargin] [float] NULL,
	[OtherMargins] [float] NULL,
 CONSTRAINT [PK_PrintSettings] PRIMARY KEY CLUSTERED 
(
	[PrintoutType] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[PrintSettings]  WITH CHECK ADD  CONSTRAINT [FK_Printout] FOREIGN KEY([PrintoutType])
REFERENCES [dbo].[enumPrintout] ([Id])
GO
ALTER TABLE [dbo].[PrintSettings] CHECK CONSTRAINT [FK_Printout]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				var queries = create_script.Split("GO");
				foreach (var q in queries)
				{
					try
					{
						await conn.ExecuteAsync(q);
					}
					catch (Exception e)
					{
					}
				}
			}
		}
		catch (Exception)
		{

		}
	}

	public async Task CreateOrganizationLogoTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[OrganizationLogo](
	[Image] [varbinary](max) NOT NULL,
	[Id] [tinyint] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_OrganizationLogo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				await conn.ExecuteAsync(create_script, commandTimeout: 300);
			}
		}
		catch (Exception ex)
		{

		}
	}

	public async Task CreateEnvelopeDesignTable()
	{
		var create_script = @"
CREATE TABLE [dbo].[EnvelopeDesign](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
 CONSTRAINT [PK_EnvelopeDesign] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			{
				await conn.ExecuteAsync(create_script, commandTimeout: 300);
			}
		}
		catch (Exception ex)
		{

		}
	}

	public async Task CreateenumAddressTypeTable()
	{
		var create_script = @"
/****** Object:  Table [dbo].[enumAddressType]    Script Date: 8/29/2023 1:38:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[enumAddressType](
	[Id] [smallint] NOT NULL,
	[Value] [nvarchar](15) NOT NULL,
 CONSTRAINT [PK_enumAddressType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[enumAddressType] ([Id], [Value]) VALUES (0, N'Unspecified')
GO
INSERT [dbo].[enumAddressType] ([Id], [Value]) VALUES (1, N'Both')
GO
INSERT [dbo].[enumAddressType] ([Id], [Value]) VALUES (2, N'Mailing')
GO
INSERT [dbo].[enumAddressType] ([Id], [Value]) VALUES (3, N'Residential')
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			if (null != conn)
			{
				var queries = create_script.Split("GO");
				foreach (var q in queries)
				{
					try
					{
						await conn.ExecuteAsync(q);
					}
					catch (Exception e)
					{
					}
				}
			}
			else
			{
			}
		}
		catch (Exception)
		{
		}
	}

	public async Task CreateenumFamilyRelationshipTable()
	{
		var create_script = @"
/****** Object:  Table [dbo].[enumFamilyRelationship]    Script Date: 8/29/2023 1:38:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[enumFamilyRelationship](
	[Id] [smallint] NOT NULL,
	[Value] [nvarchar](15) NOT NULL,
 CONSTRAINT [PK_enumFamilyRelationship] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (0, N'None')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (1, N'Primary')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (2, N'Husband')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (3, N'Wife')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (4, N'Son')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (5, N'Daughter')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (6, N'Mother')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (7, N'Father')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (8, N'Brother')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (9, N'Sister')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (10, N'Grandfather')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (11, N'Grandmother')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (12, N'Granddaughter')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (13, N'Grandson')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (14, N'Stepson')
GO
INSERT [dbo].[enumFamilyRelationship] ([Id], [Value]) VALUES (15, N'Stepdaughter')
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			if (null != conn)
			{
				var queries = create_script.Split("GO");
				foreach (var q in queries)
				{
					try
					{
						await conn.ExecuteAsync(q);
					}
					catch (Exception e)
					{
					}
				}
			}
			else
			{
			}
		}
		catch (Exception)
		{
		}
	}

	public async Task CreateenumGenderTable()
	{
		var create_script = @"
/****** Object:  Table [dbo].[enumGender]    Script Date: 8/29/2023 1:38:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[enumGender](
	[Id] [smallint] NOT NULL,
	[Value] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_enumGender] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[enumGender] ([Id], [Value]) VALUES (0, N'Unknown')
GO
INSERT [dbo].[enumGender] ([Id], [Value]) VALUES (1, N'Male')
GO
INSERT [dbo].[enumGender] ([Id], [Value]) VALUES (2, N'Female')
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			if (null != conn)
			{
				var queries = create_script.Split("GO");
				foreach (var q in queries)
				{
					try
					{
						await conn.ExecuteAsync(q);
					}
					catch (Exception e)
					{
					}
				}
			}
			else
			{
			}
		}
		catch (Exception)
		{
		}
	}

	public async Task CreateenumMaritalStatusTable()
	{
		var create_script = @"
/****** Object:  Table [dbo].[enumMaritalStatus]    Script Date: 8/29/2023 1:38:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[enumMaritalStatus](
	[Id] [smallint] NOT NULL,
	[Value] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_enumMaritalStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[enumMaritalStatus] ([Id], [Value]) VALUES (0, N'Unknown')
GO
INSERT [dbo].[enumMaritalStatus] ([Id], [Value]) VALUES (1, N'Single')
GO
INSERT [dbo].[enumMaritalStatus] ([Id], [Value]) VALUES (2, N'Married')
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			if (null != conn)
			{
				var queries = create_script.Split("GO");
				foreach (var q in queries)
				{
					try
					{
						await conn.ExecuteAsync(q);
					}
					catch (Exception e)
					{
					}
				}
			}
			else
			{
			}
		}
		catch (Exception)
		{
		}
	}

	public async Task CreateenumMethodTable()
	{
		var create_script = @"
/****** Object:  Table [dbo].[enumMethod]    Script Date: 8/29/2023 1:38:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[enumMethod](
	[Id] [smallint] NOT NULL,
	[Value] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_enumMethod] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[enumMethod] ([Id], [Value]) VALUES (0, N'Unknown')
GO
INSERT [dbo].[enumMethod] ([Id], [Value]) VALUES (1, N'Cash')
GO
INSERT [dbo].[enumMethod] ([Id], [Value]) VALUES (2, N'Check')
GO
INSERT [dbo].[enumMethod] ([Id], [Value]) VALUES (3, N'Card')
GO
INSERT [dbo].[enumMethod] ([Id], [Value]) VALUES (4, N'Mixed')
GO
INSERT [dbo].[enumMethod] ([Id], [Value]) VALUES (5, N'Online')
GO
INSERT [dbo].[enumMethod] ([Id], [Value]) VALUES (6, N'AdventistGiving')
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			if (null != conn)
			{
				var queries = create_script.Split("GO");
				foreach (var q in queries)
				{
					try
					{
						await conn.ExecuteAsync(q);
					}
					catch (Exception e)
					{
					}
				}
			}
			else
			{
			}
		}
		catch (Exception)
		{
		}
	}

	public async Task CreateenumSourceTable()
	{
		var create_script = @"
/****** Object:  Table [dbo].[enumSource]    Script Date: 8/29/2023 1:38:49 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[enumSource](
	[Id] [smallint] NOT NULL,
	[Value] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_enumSource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[enumSource] ([Id], [Value]) VALUES (0, N'AdventistGiving')
GO
INSERT [dbo].[enumSource] ([Id], [Value]) VALUES (1, N'DonorInput')
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			if (null != conn)
			{
				var queries = create_script.Split("GO");
				foreach (var q in queries)
				{
					try
					{
						await conn.ExecuteAsync(q);
					}
					catch (Exception e)
					{
					}
				}
			}
			else
			{
			}
		}
		catch (Exception)
		{
		}
	}

	public async Task CreateenumPrintoutTable()
	{
		var create_script = @"
/****** Object:  Table [dbo].[enumPrintout]    Script Date: 1/19/2024 1:44:35 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[enumPrintout](
	[Id] [smallint] NOT NULL,
	[Value] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_enumPrintout] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[enumPrintout] ([Id], [Value]) VALUES (0, N'BatchReport')
GO
INSERT [dbo].[enumPrintout] ([Id], [Value]) VALUES (1, N'DonorReport')
GO
INSERT [dbo].[enumPrintout] ([Id], [Value]) VALUES (2, N'CategoryReport')
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			if (null != conn)
			{
				var queries = create_script.Split("GO");
				foreach (var q in queries)
				{
					try
					{
						await conn.ExecuteAsync(q);
					}
					catch (Exception e)
					{
					}
				}
			}
			else
			{
			}
		}
		catch (Exception)
		{
		}
	}

	public async Task CreateDonorChangesTable()
	{
		var create_script = @"
CREATE TABLE[dbo].[DonorChanges] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DonorId] [int] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[WhenChanged] [datetime] NOT NULL,
	[WhatChanged] [nvarchar](max) NULL,
	[WhoChanged] [nvarchar](100) NULL,
 CONSTRAINT[PK_DonorChanges] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON[PRIMARY]
) ON[PRIMARY] TEXTIMAGE_ON[PRIMARY]
";

		try
		{
			using IDbConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[SqlHelper.DbKey]?.ConnectionString);
			if (null != conn)
			{
				var queries = create_script.Split("GO");
				foreach (var q in queries)
				{
					try
					{
						await conn.ExecuteAsync(q);
					}
					catch (Exception e)
					{
					}
				}
			}
			else
			{
			}
		}
		catch (Exception)
		{
		}
	}
}
