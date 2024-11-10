using System.ComponentModel;

namespace Donations.Lib.Model;

public enum enumGender
{
	Unknown,
	Male,
	Female,
};

public enum enumMaritalStatus
{
	Unknown,
	Single,
	Married,
};

public enum enumAddressType
{
	[Description("")]
	Unspecified,
	[Description("Residential & Mailing")]
	Both,
	[Description("Mailing")]
	Mailing,
	[Description("Residential")]
	Residential,
};

public enum enumFamilyRelationship
{
	None,
	Primary,
	Husband,
	Wife,
	Son,
	Daughter,
	Mother,
	Father,
	Brother,
	Sister,
	Grandfather,
	Grandmother,
	Granddaughter,
	Grandson,
	Stepson,
	Stepdaughter,
};

public enum enumSource
{
	[Description("Adventist giving")]
	AdventistGiving,
	[Description("Manual input")]
	DonorInput
}

public enum enumMethod
{
	Unknown,
	Cash,
	Check,
	Card,
	Mixed,
	Online,
	AdventistGiving,
};

public enum enumDateFilterOptions
{
	CurrentYear,
	PreviousYear,
	SelectYear,
	DateRange
};

public enum enumReportOptions
{
	[Description("Print")]
	Individual,
	[Description("Save PDF's")]
	AllPdf,
	[Description("Create PDF's and send email")]
	Email,
	[Description("Print to paper")]
	Print,
	[Description("Produce a report of all donors and how their donation(s) would be reported")]
	MockRun,
}

public enum enumMergeFields
{
	[Description("Start table")]
	beg_table,
	[Description("End table")]
	end_table,
	[Description("Start row")]
	beg_row,
	[Description("End row")]
	end_row,
	[Description("Start column")]
	beg_col,
	[Description("End column")]
	end_col,
	[Description("Church logo")]
	ChurchLogo,
	[Description("Donor name")]
	DonorName,
	[Description("Donor address")]
	DonorAddress,
	[Description("Date range")]
	DateRange,
	[Description("Tax-deductible details")]
	TaxDeductibleDetails,
	[Description("Tax-deductible summary")]
	TaxDeductibleSummary,
	[Description("Tax-deductible total")]
	TaxDeductibleTotal,
	[Description("Non-deductible details")]
	NonDeductibleDetails,
	[Description("Non-deductible summary")]
	NonDeductibleSummary,
	[Description("Non-deductible total")]
	NonDeductibleTotal,
	[Description("Contains deductible (condition)")]
	ContainsDeductible,
}

public enum enumPdfCover
{
	[Description("Image")]
	Image,
	[Description("Today's date")]
	Date,
	[Description("Bold")]
	b,
	[Description("Underline")]
	u,
	[Description("Italic")]
	i,
	[Description("Font family")]
	Align,
	[Description("Text alignment")]
	Font,
	[Description("Font size")]
	FontSize,
}

public enum enumPrintout
{
	BatchReport,
	DonorReport,
	CategoryReport,
};

public enum enumCategoryReviewType
{
	Batch,
	Donation,
}

public enum enumSqlChoiceOptions
{
	Unspecified,
	ConnStringOnly,
	Cloud,
	Local,
	Import
}

