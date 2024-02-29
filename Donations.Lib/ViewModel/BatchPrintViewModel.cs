using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the BatchPrintView.xaml which is a
/// UserControl displayed under the 'Print' tab of the batch review window.
/// 
/// The BatchReviewView will show a non-paginated print preview assuming an 8.5 x 11 page. The print
/// will actually use the page size specified when printing. The preview uses the FontFamily, size,
/// and margins.
/// 
/// The print content is is FlowDocument and uses a table, rather than a DataGrid or Grid. The reason
/// being because a table splits nicely accross pages. DataGrid's and Grid's by default do not. The
/// only reason the left margin can be specified different from the other margins is to account for
/// three ring binding, otherwise I would only have allowed one margin to be specified for the whole
/// page.
/// </summary>
public partial class BatchPrintViewModel : BaseViewModel
{
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private readonly IDonorServices _donorServices;
	private readonly IPrintSettingsServices _printSettingsServices;
	private readonly TableHelper _tableHelper;
	private FlowDocument? _doc;
	private Batch? _batch;
	private Dictionary<string, CategorySum> _categorySumDict = new Dictionary<string, CategorySum>();
	private ObservableCollection<Donation>? _batchDonations;
	private double _total = 0;
	private double _printAreaWidth;
	DonationTableColumnDescriptor[]? _donationTableColumnDescriptors;
	DonationTableColumnDescriptor[]? _donationTableColumnDescriptorsAG;
	private string? Header => $"Batch date: {BatchDate}\r\n" +
							$"Batch total: {_total.ToString("C2")}\r\n" +
							$"Batch source: {_batch?.Source.GetDescription()}\r\n" +
							$"Batch note: {_batch?.Note}\r\n" +
							$"Created by: {Created}";
#pragma warning disable CS8604 // Possible null reference argument.
	private string? BatchDate => (null == _batch) ? null : DateOnly.Parse(_batch.Date).ToLongDateString();
#pragma warning restore CS8604 // Possible null reference argument.
	private string? Created => WindowsIdentity.GetCurrent().Name + " on " + DateTime.Now.ToString("G");


	/// <summary>
	/// The constructor creates the FontList which is bound to the FontFamily selection combobox.
	/// That ComboBox shows each font choice by name, displayed in the given font face. Then the
	/// preferred font face and size are set for starters, and the page margins are also restored
	/// from settings.
	/// </summary>
	public BatchPrintViewModel(
		IDispatcherWrapper dispatcherWrapper,
		IDonorServices donorServices,
		IPrintSettingsServices printSettingsServices,
		TableHelper tableHelper
	)
	{
		_dispatcherWrapper = dispatcherWrapper;
		_donorServices = donorServices;
		_printSettingsServices = printSettingsServices;
		_tableHelper = tableHelper;
		_donationTableColumnDescriptors = new DonationTableColumnDescriptor[]
		{
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Name, "Name"),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Category, "Category"),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Transaction, "Check #", TextAlignment.Right),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Value, "Amount", TextAlignment.Right, "C2"),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Note, "Note"),
		};
		_donationTableColumnDescriptorsAG = new DonationTableColumnDescriptor[]
		{
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Name, "Name"),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Category, "Category"),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Date, "Date"),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Transaction, "Transaction #", TextAlignment.Right),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Value, "Amount", TextAlignment.Right, "C2"),
		};
	}

	public ObservableCollection<CategorySum>? CategorySums { get; set; } = new ObservableCollection<CategorySum>();
	public CollectionViewSource CategorySumSource { get; set; } = new CollectionViewSource();
	public CollectionViewSource DonationDetailsSource { get; set; } = new CollectionViewSource();

	/// <summary>
	/// This is populated in the class constructor from the Fonts.SystemFontFamilies collection.
	/// The reason I did this rather than direcly populating the control is that when i did that
	/// I couldn't select by setting the bound SelectedItem.
	/// </summary>
	public ObservableCollection<string> FontList { get; set; } = new ObservableCollection<string>();

	[ObservableProperty]
	private string? _selectedFont;
	/// <summary>
	/// The SelectedFont prperty is used to initially select the last font used, and also to
	/// receive the latest font chosen by the operator.
	/// </summary>
	partial void OnSelectedFontChanged(string? value)
	{
		Redraw();
	}

	[ObservableProperty]
	private double _selectedSize;
	/// <summary>
	/// The SelectedSize prperty is used to initially select the last font size used, and also to
	/// receive the latest font size chosen by the operator.
	/// </summary>
	partial void OnSelectedSizeChanged(double value)
	{
		if (null != _doc)
		{
			_doc.FontSize = value;
		}

		Redraw();
	}

	[ObservableProperty]
	private double _leftMargin;
	/// <summary>
	/// The LeftMargin property is provided for the sole purpose of allowing a larger left margin
	/// for three ring binding. Otherwise, I would have only given a single margin parameter.
	/// </summary>

	[ObservableProperty]
	private double _otherMargins;
	/// <summary>
	/// The OtherMargins property contains a single value that is applied to the top/right/bottom
	/// page margins.
	/// </summary>

	/// <summary>
	/// This view model is atomatically created when the view it is bound to is created. Since the
	/// view model is created automatically, no parameters are passed to the constructor, so they
	/// are passed in this Loded() member function called from the View constructuro. This function
	/// receives the FlowDocument, which is just the x:Name of the FlowDocument object in the xaml.
	/// It also recieved the current batch and the collection of donations for that batch. The
	/// FlowDocument is completely created in code behind and not in the xaml. There are some
	/// reason for that.
	/// 
	///   1. Didn't find a really nice way of sorting the lists the way I wanted.
	///   2. Used the Table object instead of the DataGrid or Grid objects, because the later two
	///      were not splitting across page boundaries like the Table object did.
	///   3. Since I used the Table object, I had do do my own column sizing based on column
	///      contents.
	/// </summary>
	/// <param name="doc">PrintArea from the BatchPrintView.xaml.</param>
	/// <param name="batch">Batch object containing parameters to print at the top of the report.</param>
	/// <param name="collection">Filtered collection of Donation records associated with the Batch.</param>
	public async Task Loaded(FlowDocument doc, Batch batch, ObservableCollection<Donation> collection)
	{
		var settings = _printSettingsServices.Get(enumPrintout.BatchReport);

		SelectedFont = settings.FontFamily;
		SelectedSize = settings.FontSize;
		LeftMargin = settings.LeftMargin;
		OtherMargins = settings.OtherMargins;

		_printAreaWidth = (8.5 - LeftMargin - OtherMargins) * PrintOptionsView._dpi;

		_doc = doc;
		_batch = batch;

		_batchDonations = collection;

		CategorySumSource.Source = CategorySums;
		CategorySumSource.SortDescriptions.Add(new SortDescription() { PropertyName = "Category", Direction = ListSortDirection.Ascending });

		DonationDetailsSource.Source = _batchDonations;
		DonationDetailsSource.SortDescriptions.Add(new SortDescription() { PropertyName = "Name", Direction = ListSortDirection.Ascending });
		if (_batch.Source == enumSource.AdventistGiving)
			DonationDetailsSource.SortDescriptions.Add(new SortDescription() { PropertyName = "TransactionNumber", Direction = ListSortDirection.Ascending });
		else
			DonationDetailsSource.SortDescriptions.Add(new SortDescription() { PropertyName = "EnvelopeId", Direction = ListSortDirection.Ascending });
		DonationDetailsSource.SortDescriptions.Add(new SortDescription() { PropertyName = "Category", Direction = ListSortDirection.Ascending });

		await ComputeSum();

		Redraw();
	}

	/// <summary>
	/// This is called from the UserControl unloaded handler. These statements will persist the last
	/// font and margin selections by the user, so they can be restored the next time this control
	/// is brought up.
	/// </summary>
	public void Unloaded()
	{
#pragma warning disable CS8601 // Possible null reference assignment.
		PrintSettings settings = new PrintSettings()
		{
			PrintoutType = (int)enumPrintout.BatchReport,
			FontFamily = SelectedFont,
			FontSize = SelectedSize,
			LeftMargin = LeftMargin,
			OtherMargins = OtherMargins
		};
#pragma warning restore CS8601 // Possible null reference assignment.
		_printSettingsServices?.Save(settings);
	}

	/// <summary>
	/// This private member function is called everytime one of the font or margin settings is changed,
	/// to reflect those changes in the FlowDocument view.
	/// </summary>
	private void Redraw()
	{
		try
		{
			if (null == _doc) return;

			_doc.Blocks.Clear();

			_doc.Blocks.Add(new Paragraph(new Run(Header)) { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize + 4 });

			_doc.Blocks.Add(_tableHelper.CreateCategorySumsTable(CategorySumSource, SelectedFont, SelectedSize));
			if (_batch.Source == enumSource.AdventistGiving)
			{
#pragma warning disable CS8604 // Possible null reference argument.
				_doc.Blocks.Add(_tableHelper.CreateDonationDetailsTable(DonationDetailsSource, SelectedFont, SelectedSize, _printAreaWidth, _donationTableColumnDescriptorsAG, (int)DonationTableColumnDescriptor.EnumDonationcolumns.Category));
#pragma warning restore CS8604 // Possible null reference argument.
			}
			else
			{
#pragma warning disable CS8604 // Possible null reference argument.
				_doc.Blocks.Add(_tableHelper.CreateDonationDetailsTable(DonationDetailsSource, SelectedFont, SelectedSize, _printAreaWidth, _donationTableColumnDescriptors, (int)DonationTableColumnDescriptor.EnumDonationcolumns.Category));
#pragma warning restore CS8604 // Possible null reference argument.
			}
		}
		catch { }
	}

	/// <summary>
	/// This method function is called to compute the actual sum of all the donations in
	/// this batch. It should match with the target Total entered at the top of this view.
	/// </summary>
	private async Task ComputeSum()
	{
		CategorySums.Clear();
		_categorySumDict.Clear();
		_total = 0;

		foreach (var donation in _batchDonations)
		{
#pragma warning disable CS8604 // Possible null reference argument.
			if (_categorySumDict.ContainsKey(donation.Category))
			{
				_categorySumDict[donation.Category].Sum += donation.Value;
			}
			else
			{
				CategorySum sum = new CategorySum()
				{
					Category = donation.Category,
					Sum = donation.Value
				};
				CategorySums.Add(sum);
				_categorySumDict[sum.Category] = sum;
			}
#pragma warning restore CS8604 // Possible null reference argument.
			_total += donation.Value;

			await _dispatcherWrapper.Yield();
		}

		CategorySumSource.View.Refresh();
	}
}
