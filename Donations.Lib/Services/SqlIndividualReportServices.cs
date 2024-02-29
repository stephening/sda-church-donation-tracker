using Dapper;
using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Serilog;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Donations.Lib.Services;

public class SqlIndividualReportServices : SqlHelper, IIndividualReportServices
{
	private string _reportTemplateDbName => "dbo.IndividualReport";
	public IndividualReport? _reportCache;
	private readonly SqlCreateTables _sqlCreateTables;

	public SqlIndividualReportServices(
		ILogger logger,
		SqlCreateTables sqlCreateTables)
		: base(logger)
	{
		_sqlCreateTables = sqlCreateTables;
	}

	public async Task<IndividualReport?> Get()
	{
		try
		{
			if (null != _reportCache)
			{
				return _reportCache;
			}

			var ret = await SelectFromTableAsync<IndividualReport>(_reportTemplateDbName);

			if (0 == ret.Count)
			{
				// Default values if no table row exists. User can modify template letter and it will be used next time.
				_reportCache = new IndividualReport()
				{
					Encrypt = true,
					EmailSubject = "Year end giving report from your church treasurer",
					EmailBody = "This is your year end giving report. " +
					"The attached Pdf file is password protected with your 10 digit phone number (with no dashes or punctuation) if we have one on record. " +
					"If we have no phone number on record for you, there should be no password. " +
					"If you are unable to open it with your current phone number please reply to the treasurer for assistance.\n" +
					"Thank you,\n" +
					"Church treasurer"
				};
				_reportCache.TemplateText = @"{beg_table}
{beg_row}
{beg_col}
{ChurchLogo Height=50}
{end_col}
{beg_col}
Your church address
{end_col}
{end_row}
{end_table}




{DonorName}
{DonorAddress}


Dear {DonorName},

This document reports your remitted tax-deductible donations for {DateRange} and any other non-deductible items remitted. Please verify donations with your personal records and contact the Church Treasury if you discover variances.

Giving period: {DateRange}

{TaxDeductibleDetails}
{TaxDeductibleSummary}
Tax-deductible total: {TaxDeductibleTotal}* No tax deductible donations were given.

{ContainsDeductible}No Goods or services were received in exchange for these gifts.
{NonDeductibleDetails}
{NonDeductibleSummary}
Non-deductible total: {NonDeductibleTotal}

Thank you for your continued support,

Your Church Treasurer
";
			}
			else
			{
				_reportCache = ret.Single();

				return _reportCache;
			}

			return _reportCache;
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught while deserializing {_reportTemplateDbName}.");
			return null;
		}
	}

	public async Task Save(IndividualReport individualReport)
	{
		try
		{
			// first delete existing rows
			await DropTable(_reportTemplateDbName);

			await _sqlCreateTables!.CreateIndividualReportTable();

			using IDbConnection conn = new SqlConnection(_connString);
			{
				var properties = Helper.PublicProperties<IndividualReport>().ToList();
				properties.Remove("Id");
				string command = "";
				command += $"INSERT INTO {_reportTemplateDbName} ({string.Join(',', properties)}) OUTPUT INSERTED.Id VALUES ({string.Join(',', properties.Select(x => "@" + x).ToList())})";

				_ = await conn.ExecuteScalarAsync<int>(command, individualReport);
			}
		}
		catch (Exception ex)
		{
			_logger.Err(ex, $"Exception caught saving to {_reportTemplateDbName}");
		}
	}
}
