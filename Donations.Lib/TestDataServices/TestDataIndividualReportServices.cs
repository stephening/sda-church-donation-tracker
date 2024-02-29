using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Threading.Tasks;

namespace Donations.Lib.TestDataServices;

public class TestDataIndividualReportServices : IIndividualReportServices
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
	private IndividualReport _individualReport;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task<IndividualReport?> Get()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
		_individualReport = new IndividualReport()
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
		_individualReport.TemplateText = @"{beg_table}
{beg_row}
{beg_col}
{ChurchLogo Height = 50}
{end_col}
{beg_col}
SDA Church
14645 Somewhere Rd.
City, ST 12345
{end_col}
{end_row}
{end_table}




{DonorName}
{DonorAddress}


Dear {DonorName},

This document reports your remitted tax-deductible donations for {DateRange}
and any other non-deductible items remitted. Please verify donations with your personal records and contact the Church Treasury if you discover variances.

Giving period: {DateRange}

{TaxDeductibleDetails}
{TaxDeductibleSummary}
Tax - deductible total: {TaxDeductibleTotal}
*No tax deductible donations were given.

{ContainsDeductible}
No Goods or services were received in exchange for these gifts.
{NonDeductibleDetails}
{NonDeductibleSummary}
Non - deductible total: {NonDeductibleTotal}

Thank you for your continued support,

The Church Treasurer
";
		return _individualReport;
	}

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
	public async Task Save(IndividualReport individualReport)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
	{
	}
}
