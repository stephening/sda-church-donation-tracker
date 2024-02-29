using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.Collections.ObjectModel;
using System.Security.Principal;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Donations.Lib.ViewModel;

public partial class CategoryReviewViewModel : BaseViewModel, IPrintPreview
{
	private string? _categoryText;
	private string? _timeWindowText;
	CategorySum? _categorySum;
	ObservableCollection<Donation>? _donations;

	private string? Header =>
		$"{_categoryText}\r\n" +
		$"Total: {_categorySum?.Sum.ToString("C2")}\r\n" +
		$"Time range: {_timeWindowText}\r\n" +
		$"Created by: {Created}";

	private string? Created => WindowsIdentity.GetCurrent().Name + " on " + DateTime.Now.ToString("G");

	public CategoryReviewViewModel(
		TableHelper tableHelper)
	{
		TableHelper = tableHelper;
	}

	[ObservableProperty]
	private double? _sum;

	[ObservableProperty]
	private string? _categoryDescription;

	[ObservableProperty]
	private string? _date;

	public bool PrintPreviewEnabled => 0 < _donations.Count;

	public void Setup(
		CategorySum? categorySum,
		ObservableCollection<Donation>? donations,
		string timeWindowText)
	{
		_categorySum = categorySum;
		_donations = donations;
		_categoryText = categorySum?.Category;
		_timeWindowText = timeWindowText;

		CategoryDescription = categorySum?.Category;
		Date = timeWindowText;

		Sum = categorySum?.Sum;
		Source.Source = _donations;
	}

	public TableHelper TableHelper { get; }

	public CollectionViewSource Source { get; } = new CollectionViewSource();

	public void CreatePreview(FlowDocument doc, string? font, double size, double printAreaWidth)
	{
		doc.Blocks.Clear();

		doc.Blocks.Add(new Paragraph(new Run(Header)) { FontFamily = new FontFamily(font), FontSize = size + 4 });

		DonationTableColumnDescriptor[] donationTableColumnDescriptors = new DonationTableColumnDescriptor[]
		{
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Name, "Name"),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Category, "Category"),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Date, "Date"),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Method, "Method"),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Transaction, "Tx/Ck #", TextAlignment.Right),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Value, "Amount", TextAlignment.Right, "C2"),
			new DonationTableColumnDescriptor(DonationTableColumnDescriptor.EnumDonationcolumns.Note, "Note"),
		};

		Table table = TableHelper.CreateDonationDetailsTable(Source, font, size, printAreaWidth, donationTableColumnDescriptors, 1);

		doc.Blocks.Add(table);
	}

}
