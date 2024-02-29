using System;
using System.Windows.Media;

namespace Donations.Lib.Model;

/// <summary>
/// The Batch object is used to group donations entered on a single day or at the same time, producing a total
/// amount, and category subtotals which will be entered into the GL accounting software. The Id is a unique
/// database Id, and all Donation records with the same batch Id will be considered part of a single batch.
/// 
/// Refrenced by: MainWindow, BatchBrowserView, BatchReviewView, BatchBrowserViewModel, BatchReviewViewModel,
///				  AGDonationSummaryViewModel, DonorContributionsViewModel, DonorInputViewModel, DonorViewModel,
///				  ImportDonationsViewModel
/// </summary>
public class Batch
{
	/// <summary>
	/// This Id property is unique, and is placed in all the Donation records that belong to the same batch.
	/// Each batch is a line item in the batch browser view.
	/// </summary>
	public int Id { get; set; }
	/// <summary>
	/// The Source property can take on one of the enumSource values.
	/// </summary>
	public enumSource Source { get; set; } = enumSource.DonorInput;

	private DateOnly _date;
	/// <summary>
	/// This Date property is specified by when the batch is submitted.
	/// </summary>
	public string? Date
	{
		get
		{
			string str = _date.ToString("yyyy/MM/dd");
			return str.Equals("0001/01/01") ? "" : str;
		}
		set
		{
			try
			{
				if (string.IsNullOrEmpty(value))
					_date = DateOnly.MinValue;
				else
					_date = DateOnly.Parse(value);
			}
			catch
			{
				_date = DateOnly.MinValue;
			}
		}
	}
	/// <summary>
	/// This Total property is a dollar amount that should match a total counted before submission
	/// or in the case of Adventist Giving, a total amount supplied to go with the csv.
	/// </summary>
	public double Total { get; set; }
	/// <summary>
	/// This RunningTotal property is the actual dollar amount of the donations that should match 
	/// the total.
	/// </summary>
	public double ActualTotal { get; set; }
	/// <summary>
	/// The Operator property will be the login name of the person entering the donation batches.
	/// </summary>
	public string? Operator { get; set; } = "";
	/// <summary>
	/// The Note property will be displayed in the batch browser view.
	/// </summary>
	public string? Note { get; set; }
	/// <summary>
	/// The CellBackgroundColor property should color the batch row if the Totals don't match.
	/// </summary>
	public Brush CellBackgroundColor => 0 != Math.Round(Total - ActualTotal, 2) ? Brushes.Red : Brushes.Transparent;
	/// <summary>
	/// The CellBackgroundColor property should color the batch row if the Totals don't match.
	/// </summary>
	public Brush CellForegroundColor => 0 != Math.Round(Total - ActualTotal, 2) ? Brushes.White : Brushes.Black;
}
