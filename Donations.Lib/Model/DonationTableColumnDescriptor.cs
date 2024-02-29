using System.Windows;

namespace Donations.Lib.Model;

public class DonationTableColumnDescriptor : TableColumnDescriptor
{
	public DonationTableColumnDescriptor(
		EnumDonationcolumns column,
		string header,
		TextAlignment alignment = TextAlignment.Left,
		string? format = null)
	{
		_column = column;
		ColumnHeader = header;
		Alignment = alignment;
		Format = format;
	}

	public enum EnumDonationcolumns
	{
		Name,
		Id,
		DonorId,
		BatchId,
		EnvelopeId,
		LastName,
		FirstName,
		Category,
		Value,
		Date,
		TaxDeductible,
		Note,
		Method,
		Transaction,
	}

	private EnumDonationcolumns _column;

	public string? GetValue(Donation? donation)
	{
		switch (_column)
		{
			case EnumDonationcolumns.Name: return donation?.Name;
			case EnumDonationcolumns.Id: return donation?.Id.ToString();
			case EnumDonationcolumns.DonorId: return donation?.DonorId.ToString();
			case EnumDonationcolumns.BatchId: return donation?.BatchId.ToString();
			case EnumDonationcolumns.EnvelopeId: return donation?.EnvelopeId.ToString();
			case EnumDonationcolumns.LastName: return donation?.LastName;
			case EnumDonationcolumns.FirstName: return donation?.FirstName;
			case EnumDonationcolumns.Category: return donation?.Category;
			case EnumDonationcolumns.Value: return donation?.Value.ToString(Format);
			case EnumDonationcolumns.Date: return donation?.Date;
			case EnumDonationcolumns.TaxDeductible: return donation?.TaxDeductible.ToString();
			case EnumDonationcolumns.Note: return donation?.Note;
			case EnumDonationcolumns.Method: return donation?.Method.ToString();
			case EnumDonationcolumns.Transaction: return donation?.TransactionNumber;
		}

		return "Unexpected column";
	}
}
