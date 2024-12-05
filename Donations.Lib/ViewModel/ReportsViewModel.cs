using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Extensions;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using Donations.Lib.View;
using Serilog;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Security;
using Syncfusion.XPS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Paragraph = System.Windows.Documents.Paragraph;
using Table = System.Windows.Documents.Table;
using TableCell = System.Windows.Documents.TableCell;
using TableColumn = System.Windows.Documents.TableColumn;
using TableRow = System.Windows.Documents.TableRow;
using TableRowGroup = System.Windows.Documents.TableRowGroup;

namespace Donations.Lib.ViewModel;

public partial class ReportsViewModel : BaseTimeWindowViewModel
{
	private readonly Regex _logoRePat = new Regex($"{{{enumMergeFields.ChurchLogo}([ ]+Width[ ]*=[ ]*[0-9]+)?([ ]+Height[ ]*=[ ]*[0-9]+)?}}");
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private readonly ILogger _logger;
	private readonly DonorModalView.Factory _donorModalViewFactory;
	private readonly IFileSystem _fileSystem;
	private readonly IBatchServices _batchServices;
	private readonly IDonationServices _donationServices;
	private readonly IDonorServices _donorServices;
	private readonly IDonorReportServices _donorReportsServices;
	private readonly IIndividualReportServices _individualReportServices;
	private readonly IPictureServices _pictureServices;
	private readonly IAppSettingsServices _appSettingsServices;
	private readonly IPrintSettingsServices _printSettingsServices;
	private readonly TableHelper _tableHelper;
	private ObservableCollection<Donation>? _donations;
	private ObservableCollection<DonorReport>? _donorReports;
	private IndividualReport? _individualReport;
	private FlowDocument? _doc;
	private FlowDocument? _visibleFlowDoc;
	private Donor? _donor;
	private Donor? _primary;
	private Dictionary<string, CategorySum> _categorySumDict = new Dictionary<string, CategorySum>();
	private double _subTotal = 0;
	private double _printAreaWidth;
	private bool _taxDeductible = true;
	private List<Donor>? _givingGroup;
	private List<int>? _donorIds;
	private DateTime? _todaysDate;
	private ObservableCollection<CategorySum>? _categorySums = new ObservableCollection<CategorySum>();
	private ObservableCollection<NamedDonorReport>? _namedDonorReports;
	private ObservableCollection<NamedDonorReport>? _mockRunCollection;

	public ReportsViewModel(
		IDispatcherWrapper dispatcherWrapper,
		ILogger logger,
		DonorModalView.Factory donorModalViewFactory,
		IFileSystem fileSystem,
		IBatchServices batchServices,
		IDonationServices donationServices,
		IDonorServices donorServices,
		IDonorReportServices donorReportsServices,
		IIndividualReportServices individualReportServices,
		IPictureServices pictureServices,
		IAppSettingsServices appSettingsServices,
		IPrintSettingsServices printSettingsServices,
		TableHelper tableHelper
	)
	{
		_dispatcherWrapper = dispatcherWrapper;
		_logger = logger;
		_donorModalViewFactory = donorModalViewFactory;
		_fileSystem = fileSystem;
		_batchServices = batchServices;
		_donationServices = donationServices;
		_donorServices = donorServices;
		_donorReportsServices = donorReportsServices;
		_individualReportServices = individualReportServices;
		_pictureServices = pictureServices;
		_appSettingsServices = appSettingsServices;
		_printSettingsServices = printSettingsServices;
		_tableHelper = tableHelper;

		DatesUpdated();

		CategorySumSource.Source = _categorySums;

		DonationDetailsSource.Source = new ObservableCollection<Donation>();
		DonationDetailsSource.Filter += new FilterEventHandler(Filter);
		DonationDetailsSource.View.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));

		DonorReportView.Filter += new FilterEventHandler(DonorReportsFilter);
	}

	public CollectionViewSource DonorReportView { get; set; } = new CollectionViewSource();
	public CollectionViewSource CategorySumSource { get; set; } = new CollectionViewSource();
	public CollectionViewSource DonationDetailsSource { get; set; } = new CollectionViewSource();
	public CollectionViewSource MockRunView { get; set; } = new CollectionViewSource();

	[ObservableProperty]
	private int _selectedIndex;

	[ObservableProperty]
	private double _sum = 0;

	[ObservableProperty]
	private string _selectedCategory = null;

	[ObservableProperty]
	private bool _printPreviewEnabled = false;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ProgressVisibility))]
	[NotifyPropertyChangedFor(nameof(EmailVisibility))]
	[NotifyPropertyChangedFor(nameof(PrintVisibility))]
	[NotifyPropertyChangedFor(nameof(FlowDocVisibility))]
	[NotifyPropertyChangedFor(nameof(MockRunVisibility))]
	[NotifyPropertyChangedFor(nameof(ActionEnabled))]
	private enumReportOptions _reportOption;
	/// <summary>
	/// This property will be used with radio buttons to select the type of report to generate.
	/// </summary>
	partial void OnReportOptionChanged(enumReportOptions value)
	{
		if (enumReportOptions.Individual == value)
		{
			ActionButtonText = value.GetDescription() + "...";
			_doc = _visibleFlowDoc;
		}

		if (enumReportOptions.AllPdf == value) ActionButtonText = value.GetDescription() + "...";
		if (enumReportOptions.Email == value) ActionButtonText = value.GetDescription() + "...";
		if (enumReportOptions.Print == value) ActionButtonText = value.GetDescription() + "...";
		if (enumReportOptions.MockRun == value) ActionButtonText = value.GetDescription();

		if (enumReportOptions.Individual != value)
		{
			_donor = null;
			OnPropertyChanged(nameof(Name));
		}

		Current = Total = 0;
	}

	[ObservableProperty]
	private string _actionButtonText = enumReportOptions.Individual.GetDescription() + "...";

	[ObservableProperty]
	private string _templateText;
	partial void OnTemplateTextChanged(string value)
	{
		_dispatcherWrapper.BeginInvoke(() => FormatLetter(_donor));
	}

	public string? Name => _donor?.Name;


	[ObservableProperty]
	private string? _selectedFont;
	/// <summary>
	/// The SelectedFont prperty is used to initially select the last font used, and also to
	/// receive the latest font chosen by the operator.
	/// </summary>
	partial void OnSelectedFontChanged(string? value)
	{
		_dispatcherWrapper.BeginInvoke(() => FormatLetter(_donor));
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

		_dispatcherWrapper.BeginInvoke(() => FormatLetter(_donor));
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

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ActionEnabled))]
	private string _emailSubject;
	partial void OnEmailSubjectChanged(string value)
	{
		_individualReport.EmailSubject = value;
	}

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ActionEnabled))]
	private string _emailBody;
	partial void OnEmailBodyChanged(string value)
	{
		_individualReport.EmailBody = value;
	}

	[ObservableProperty]
	private bool _encrypt;

	[ObservableProperty]
	private int _total;

	[ObservableProperty]
	private int _current;

	[ObservableProperty]
	private string? _person;

	[ObservableProperty]
	private string? _FilterText;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(ActionEnabled))]
	[NotifyPropertyChangedFor(nameof(NotRunning))]
	private bool _running = false;

	public Visibility ProgressVisibility => ReportOption != enumReportOptions.Individual ? Visibility.Visible : Visibility.Collapsed;
	public Visibility MockRunVisibility => ReportOption == enumReportOptions.MockRun ? Visibility.Visible : Visibility.Collapsed;
	public Visibility EmailVisibility => ReportOption == enumReportOptions.Email ? Visibility.Visible : Visibility.Collapsed;
	public Visibility PrintVisibility => ReportOption == enumReportOptions.Print ? Visibility.Visible : Visibility.Collapsed;
	public Visibility FlowDocVisibility => (ReportOption == enumReportOptions.Individual || ReportOption == enumReportOptions.AllPdf) ? Visibility.Visible : Visibility.Collapsed;
	public bool ActionEnabled => !Running
		&& (ReportOption == enumReportOptions.Email ? !string.IsNullOrEmpty(EmailSubject) && !string.IsNullOrEmpty(EmailBody) :
		ReportOption == enumReportOptions.Individual ? null != _donor : true);
	public bool NotRunning => !Running;

	public async Task<DonorModalView> CreateDonorModalView(int donorId)
	{
		Donor donor = await _donorServices.GetDonorByIdAsync(donorId);

		return _donorModalViewFactory(donor);
	}

	public override async Task TimeWindowChanged()
	{
		SelectionEnabled = false;
		await DateChanged();
	}

	/// <summary>
	/// This member is called when the Reports tab is clicked, it will update the view.
	/// </summary>
	public new async Task Loading()
	{
		var settings = _printSettingsServices.Get(enumPrintout.DonorReport);

		SelectedFont = settings.FontFamily;
		SelectedSize = settings.FontSize;
		LeftMargin = settings.LeftMargin;
		OtherMargins = settings.OtherMargins;

		if (string.IsNullOrEmpty(SelectedFont))
		{
			MessageBox.Show("Font cannot be null or empty");
			return;
		}

		_individualReport = await _individualReportServices.Get();

		TemplateText = _individualReport.TemplateText;
		EmailSubject = _individualReport.EmailSubject;
		EmailBody = _individualReport.EmailBody;
		Encrypt = _individualReport.Encrypt;

		_printAreaWidth = (8.5 - LeftMargin - OtherMargins) * PrintOptionsView._dpi;

		_donorReports = await _donorReportsServices.Load();

		_namedDonorReports = new ObservableCollection<NamedDonorReport>(await _donorReportsServices.LoadNamed());
		DonorReportView.Source = _namedDonorReports;

		await DateChanged();
	}

	/// <summary>
	/// This member is called when the Reports tab is clicked, it will save model changes to the database.
	/// </summary>
	public new async Task Leaving()
	{
		PrintSettings settings = new PrintSettings()
		{
			PrintoutType = (int)enumPrintout.DonorReport,
			FontFamily = SelectedFont,
			FontSize = SelectedSize,
			LeftMargin = LeftMargin,
			OtherMargins = OtherMargins
		};
		await _printSettingsServices.Save(settings);

		if (null != _individualReport)
		{
			bool changed = false;
			if (_individualReport.TemplateText != TemplateText)
			{
				_individualReport.TemplateText = TemplateText;
				changed = true;
			}

			if (_individualReport.EmailSubject != EmailSubject)
			{
				_individualReport.EmailSubject = EmailSubject;
				changed = true;
			}

			if (_individualReport.EmailBody != EmailBody)
			{
				_individualReport.EmailBody = EmailBody;
				changed = true;
			}

			if (_individualReport.Encrypt != Encrypt)
			{
				_individualReport.Encrypt = Encrypt;
				changed = true;
			}

			if (changed)
			{
				await _individualReportServices.Save(_individualReport);
			}
		}
	}

	/// <summary>
	/// This function is BeginInvoke'd, when the donations list needs to be updated. Re-querying the list
	/// is an async await operation that can't be awaited on from some locations. It is invoked when a
	/// condition changes which will alter the donations, among them radio button selection.
	/// </summary>
	private async Task DateChanged()
	{
		string date = "";
		string date2 = "";

		switch (DateFilterOption)
		{
			case enumDateFilterOptions.CurrentYear:
				date = _thisYear;
				break;
			case enumDateFilterOptions.PreviousYear:
				date = _prevYear;
				break;
			case enumDateFilterOptions.SelectYear:
				date = FilterYear;
				break;
			case enumDateFilterOptions.DateRange:
				date = FilterStartDate;
				date2 = FilterEndDate;
				break;
		}

		_donations = await _donationServices.FilterDonationsByDate(DateFilterOption, date, date2);
		if (null != _donations && 0 < _donations.Count)
		{
			DonationDetailsSource.Source = _donations;
			DonationDetailsSource.Filter += new FilterEventHandler(Filter);
			DonationDetailsSource.View.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Ascending));

			ComputeSum();

			await FormatLetter(_donor);
		}

		SelectionEnabled = true;
	}


	/// <summary>
	/// The filter method uses the _donor.Id if one is set, to filter donations.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Filter(object sender, FilterEventArgs e)
	{
		if (null == _donor)
		{
			e.Accepted = true;
			return;
		}

		var obj = e.Item as Donation;
		if (obj != null)
		{
			if ((obj.DonorId == _donor.Id
				|| (null != _givingGroup && _givingGroup.Where(x => x.Id == obj.DonorId).Any()))
				&& obj.TaxDeductible == _taxDeductible)
				e.Accepted = true;
			else
				e.Accepted = false;
		}
	}

	/// <summary>
	/// This method will update the AvailableYears collection using a Linq filter. It also
	/// set's the start/end range dates to the earliest and latest years from the list of batches.
	/// </summary>
	public async Task DatesUpdated()
	{
		var list = await _batchServices.GetBatchYears();
		AvailableYears = new ObservableCollection<string>(list.OrderByDescending(i => i));
		FilterYear = AvailableYears.Max();
		FilterStartDate = await _batchServices.GetEarliestDate();
		FilterEndDate = await _batchServices.GetLatestDate();

		await DateChanged();
	}

	/// <summary>
	/// The FilterText method is called from the ReportsView.xaml.cs when the
	/// FilterText's TextChanged event is fired. This allows for realtime feedback on
	/// whether the filter text is any good or not.
	/// </summary>
	public void TextChanged()
	{
		DonorReportView.View.Refresh();
	}

	/// <summary>
	/// The filter method uses the same CategoryFilterText, looking for matches in either the Code,
	/// or the Description columns.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void DonorReportsFilter(object sender, FilterEventArgs e)
	{
		var obj = e.Item as NamedDonorReport;
		if (obj != null)
		{
			if (string.IsNullOrEmpty(FilterText) || obj.Name.Contains(FilterText, System.StringComparison.OrdinalIgnoreCase))
				e.Accepted = true;
			else
				e.Accepted = false;
		}
	}


	public void SetFlowDoc(FlowDocument doc)
	{
		_visibleFlowDoc = _doc = doc;
	}

	public async Task SetDonor(Donor donor)
	{
		_donor = donor;
		OnPropertyChanged(nameof(Name));
		DonationDetailsSource.View.Refresh();
		ComputeSum();

		_primary = null;
		if (null != donor.FamilyId && true == donor.GroupGiving)
		{
			var family = await _donorServices.GetDonorsByFamilyId(donor.FamilyId.Value);
			_givingGroup = new List<Donor>(family.Where(x => x.GroupGiving == true));
			if (_givingGroup.Any())
			{
				List<Donor> temp = new List<Donor>(_givingGroup.Where(x => x.FamilyRelationship == enumFamilyRelationship.Primary));
				if (temp.Any())
				{
					_primary = temp.First();
				}
				else
				{
					_givingGroup = null;
				}
			}
			else
			{
				_givingGroup = null;
			}
		}
		else
		{
			_givingGroup = null;
		}

		await FormatLetter(_donor);
	}

	private string GetDateRange()
	{
		string daterange = "";

		switch (DateFilterOption)
		{
			case enumDateFilterOptions.CurrentYear:
				daterange = _thisYear;
				break;
			case enumDateFilterOptions.PreviousYear:
				daterange = _prevYear;
				break;
			case enumDateFilterOptions.SelectYear:
				daterange = FilterYear;
				break;
			case enumDateFilterOptions.DateRange:
				daterange = $"{FilterStartDate} - {FilterEndDate}";
				break;
		}
		return daterange;
	}

	private async Task<string> GetName(Donor donor)
	{
		string name = "";
		if (null == donor.FamilyId || true != donor.GroupGiving)
		{
			name = $"{donor.FirstName} {donor.LastName}";
		}
		else
		{
			var family = await _donorServices.GetDonorsByFamilyId(donor.FamilyId.Value);
			var primary = family.Where(x => x.FamilyRelationship == enumFamilyRelationship.Primary && x.GroupGiving == true);
			IEnumerable<Donor>? not_primary;
			string last = donor.LastName;

			if (0 < primary.Count())
			{
				name = primary.Single().FirstName;
				last = primary.Single().LastName;
				not_primary = family.Where(x => x.FamilyRelationship != enumFamilyRelationship.Primary && x.GroupGiving == true);
			}
			else
			{
				not_primary = family.Where(x => x.GroupGiving == true);
			}

			foreach (var member in not_primary)
			{
				if (member.LastName == last)
				{
					name += " & " + member.FirstName;
				}
			}

			name += " " + last;
		}

		return name;
	}

	private string GetAddress(Donor donor)
	{
		string address = donor.Address;
		if (!string.IsNullOrEmpty(address))
		{
			address += "\n";
			if (!string.IsNullOrEmpty(donor.Address2))
			{
				address += donor.Address2 + "\n";
			}
		}
		address += $"{donor.City}, {donor.State}  {donor.Zip}";

		return address;
	}

	private Paragraph? GetTotalParagraph(string[] labels, double total, bool taxDeductible)
	{
		if (0 == total)
		{
			if (1 < labels.Length && !string.IsNullOrEmpty(labels[1]))
			{
				return new Paragraph(new Run(labels[1]) { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize, FontWeight = FontWeights.Bold });
			}
			return null;
		}

		var paragraph = new Paragraph();
		paragraph.Inlines.Add(new Run(labels[0]) { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize, FontWeight = FontWeights.Bold });
		paragraph.Inlines.Add(new Run(total.ToString("C2")) { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize });

		return paragraph;
	}

	private void CheckDumpParagraph(ref string lines)
	{
		if (null != lines)
		{
			_doc.Blocks.Add(new Paragraph(new Run(lines + "\n") { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize }));
			lines = null;
		}
	}

	private async Task FormatLetter(Donor donor)
	{
		if (null == _doc || string.IsNullOrEmpty(TemplateText)) return;

		string str = TemplateText;

		string daterange = GetDateRange();

		str = str.Replace($"{{{enumMergeFields.DateRange.ToString()}}}", daterange);

		if (null != donor)
		{
			string name = await GetName(donor);
			string address = GetAddress(donor);

			str = str.Replace($"{{{enumMergeFields.DonorName.ToString()}}}", name);
			str = str.Replace($"{{{enumMergeFields.DonorAddress.ToString()}}}", address);
		}

		_doc.Blocks.Clear();

		if (null != _donor)
		{
			// this bool affects the View filters which will feed into the tables for print
			_taxDeductible = false;
			// this refresh will filter the donations down to donor and non tax deductible
			DonationDetailsSource.View.Refresh();
			Table? nonDeductibleDetails = _tableHelper?.CreateDonationDetailsTable(DonationDetailsSource, SelectedFont, SelectedSize, _printAreaWidth);

			// this refresh will filter the summary for non tax deductible
			ComputeSum();
			CategorySumSource.View.Refresh();
			Table? nonDeductibleSums = _tableHelper?.CreateCategorySumsTable(CategorySumSource, SelectedFont, SelectedSize);
			double nonDeductibleTotal = _subTotal;

			// this bool affects the View filters which will feed into the tables for print
			_taxDeductible = true;
			// this refresh will filter the donations down to donor and tax deductible
			DonationDetailsSource.View.Refresh();
			Table? deductibleDetails = _tableHelper?.CreateDonationDetailsTable(DonationDetailsSource, SelectedFont, SelectedSize, _printAreaWidth);

			// this refresh will filter the summary for tax deductible
			ComputeSum();
			CategorySumSource.View.Refresh();
			Table? deductibleSums = _tableHelper?.CreateCategorySumsTable(CategorySumSource, SelectedFont, SelectedSize);
			double deductibleTotal = _subTotal;

			var split = str.Split("\r\n");

			string lines = null;
			Table? table = null;
			TableRow? row = null;
			TableColumn? column = null;
			string? cellText = null;
			List<double>? colsize = new List<double>();
			int colIndex = 0;
			Image? cellImage = null;

			foreach (string line in split)
			{
				if (line.Contains($"{{{enumMergeFields.beg_table}}}"))
				{
					table = new Table();
					colsize.Clear();
				}
				else if (line.Contains($"{{{enumMergeFields.end_table}}}"))
				{
					if (null != table)
					{
						for (int i = 0; i < table.Columns.Count; i++)
						{
							table.Columns[i].Width = new GridLength(colsize[i] + 10, GridUnitType.Pixel);
						}
						CheckDumpParagraph(ref lines);
						table.Margin = new Thickness(0);
						_doc.Blocks.Add(table);
						table = null;
					}
				}
				else if (line.Contains($"{{{enumMergeFields.beg_row}}}"))
				{
					if (null != table)
					{
						table.RowGroups.Add(new TableRowGroup());
						row = new TableRow() { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize };
						colIndex = 0;
					}
				}
				else if (line.Contains($"{{{enumMergeFields.end_row}}}"))
				{
					if (null != table && null != row)
					{
						table.RowGroups[0].Rows.Add(row);
						row = null;
					}
				}
				else if (line.Contains($"{{{enumMergeFields.beg_col}}}"))
				{
					if (null != table && null != row)
					{
						column = new TableColumn();
					}
				}
				else if (line.Contains($"{{{enumMergeFields.end_col}}}"))
				{
					if (null != table && null != row && null != column)
					{
						if (null != cellImage)
						{
							cellImage.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
							var size = cellImage.DesiredSize;
							if (colsize.Count < colIndex + 1)
							{
								colsize.Add(0);
							}

							if (size.Width > colsize[colIndex])
							{
								colsize[colIndex] = size.Width;
							}

							row.Cells.Add(new TableCell(new BlockUIContainer(cellImage)));

							if (table.Columns.Count <= colIndex)
							{
								table.Columns.Add(column);
							}
							cellImage = null;
						}
						else
						{
							TextBlock textBlock = new TextBlock() { Text = cellText, FontFamily = row.FontFamily, FontSize = row.FontSize, FontWeight = row.FontWeight };
							textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
							var size = textBlock.DesiredSize;
							if (colsize.Count < colIndex + 1)
							{
								colsize.Add(0);
							}

							if (size.Width > colsize[colIndex])
							{
								colsize[colIndex] = size.Width;
							}

							row.Cells.Add(new TableCell(new Paragraph(new Run(cellText))));

							if (table.Columns.Count <= colIndex)
							{
								table.Columns.Add(column);
							}
						}

						column = null;
						cellText = null;
						colIndex++;
					}
				}
				else if (line.Contains($"{{{enumMergeFields.ChurchLogo}"))
				{
					double width = 0;
					double height = 0;

					var res = _logoRePat.Match(line);
					if (null != res && res.Success && 3 == res.Groups.Count)
					{
						if (!string.IsNullOrEmpty(res.Groups[1].Value))
						{
							width = double.Parse(res.Groups[1].Value.Replace(" ", "").Substring("Width=".Length));
						}
						if (!string.IsNullOrEmpty(res.Groups[2].Value))
						{
							height = double.Parse(res.Groups[2].Value.Replace(" ", "").Substring("Height=".Length));
						}
					}

					if (null == table)
					{
						CheckDumpParagraph(ref lines);
					}

					BitmapImage img = new BitmapImage();
					Picture picture = _pictureServices.GetLogo();
					if (null != picture && null != picture.Image)
					{
						using (MemoryStream memStream = new MemoryStream(_pictureServices.GetLogo()?.Image))
						{
							img.BeginInit();
							img.CacheOption = BitmapCacheOption.OnLoad;
							img.StreamSource = memStream;
							img.EndInit();
							img.Freeze();
						}

						cellImage = new Image() { Source = img, Stretch = Stretch.Uniform, HorizontalAlignment = HorizontalAlignment.Left };

						if (0 != width)
						{
							cellImage.Width = width;
						}
						if (0 != height)
						{
							cellImage.Height = height;
						}

						if (!(null != table && null != row && null != column))
						{
							_doc.Blocks.Add(new BlockUIContainer(cellImage));
							cellImage = null;
						}
					}
				}
				else if (line.Contains($"{{{enumMergeFields.TaxDeductibleTotal}}}"))
				{
					CheckDumpParagraph(ref lines);
					var total = GetTotalParagraph(line.Split($"{{{enumMergeFields.TaxDeductibleTotal}}}"), deductibleTotal, true);
					if (null != total)
					{
						_doc.Blocks.Add(total);
					}
				}
				else if (line.Equals($"{{{enumMergeFields.TaxDeductibleDetails}}}"))
				{
					if (null != deductibleDetails)
					{
						CheckDumpParagraph(ref lines);
						_doc.Blocks.Add(deductibleDetails);
					}
				}
				else if (line.Equals($"{{{enumMergeFields.TaxDeductibleSummary}}}"))
				{
					if (null != deductibleSums)
					{
						CheckDumpParagraph(ref lines);
						_doc.Blocks.Add(deductibleSums);
					}
				}
				else if (line.Equals($"{{{enumMergeFields.NonDeductibleDetails}}}"))
				{
					if (null != nonDeductibleDetails)
					{
						CheckDumpParagraph(ref lines);
						_doc.Blocks.Add(nonDeductibleDetails);
					}
				}
				else if (line.Equals($"{{{enumMergeFields.NonDeductibleSummary}}}"))
				{
					if (null != nonDeductibleSums)
					{
						CheckDumpParagraph(ref lines);
						_doc.Blocks.Add(nonDeductibleSums);
					}
				}
				else if (line.Contains($"{{{enumMergeFields.NonDeductibleTotal}}}"))
				{
					if (0 < nonDeductibleTotal)
					{
						CheckDumpParagraph(ref lines);
						var total = GetTotalParagraph(line.Split($"{{{enumMergeFields.NonDeductibleTotal}}}"), nonDeductibleTotal, false);
						if (null != total)
						{
							_doc.Blocks.Add(total);
						}
					}
				}
				else
				{
					string tmpLine = line;
					if (line.Contains($"{{{enumMergeFields.ContainsDeductible}}}"))
					{
						if (null != deductibleDetails)
						{
							tmpLine = line.Replace($"{{{enumMergeFields.ContainsDeductible}}}", "");
						}
						else
						{
							// else don't include this string
							continue;
						}
					}

					if (null != column)
					{
						if (string.IsNullOrEmpty(cellText))
						{
							cellText = tmpLine;
						}
						else
						{
							cellText += "\n" + tmpLine;
						}
					}
					else if (null == lines)
					{
						lines = tmpLine;
					}
					else
					{
						lines += "\n" + tmpLine;
					}
				}
			}

			CheckDumpParagraph(ref lines);
		}

		OnPropertyChanged(nameof(ActionEnabled));
	}

	/// <summary>
	/// This method function is called to compute the actual sum of all the donations in
	/// this batch. It should match with the target Total entered at the top of this view.
	/// </summary>
	private void ComputeSum()
	{
		_categorySums.Clear();
		_categorySumDict.Clear();
		_subTotal = 0;

		foreach (Donation donation in DonationDetailsSource.View)
		{
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
				_categorySums.Add(sum);
				_categorySumDict[sum.Category] = sum;
			}
			_subTotal += donation.Value;
		}

		CategorySumSource.View.Refresh();
	}

	private async Task<string> GeneratePdf(bool tryEncrypt, Donor donor, string xpsFileName, string pdfFileName, PrintDialog pd, Thickness margins)
	{
		using XpsDocument xpsDocument = new XpsDocument(xpsFileName, FileAccess.ReadWrite);
		XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
		_doc.PageWidth = pd.PrintableAreaWidth;
		_doc.PageHeight = pd.PrintableAreaHeight;
		_doc.ColumnWidth = _doc.PageWidth;
		_doc.PagePadding = margins;
		var docpaginator = ((IDocumentPaginatorSource)_doc).DocumentPaginator;
		docpaginator.PageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaWidth);
		writer.Write(docpaginator);
		xpsDocument.Close();
		string pdfPassword = "";

		XPSToPdfConverter xpsToPdfConverter = new XPSToPdfConverter();

		using var xpsStream = _fileSystem.File.OpenRead(xpsFileName);
		PdfDocument pdfDocument = xpsToPdfConverter.Convert(xpsStream);

		_logger.Info($"Donor ID: {donor.Id}, Donor Name: {donor.Name}, try encrypt: {tryEncrypt}, mobilephone: {donor.MobilePhone}");
		if (tryEncrypt && !string.IsNullOrEmpty(donor.MobilePhone))
		{
			PdfSecurity security = pdfDocument.Security;

			security.KeySize = PdfEncryptionKeySize.Key128Bit;
			security.Algorithm = PdfEncryptionAlgorithm.RC4;
			security.UserPassword = Regex.Replace(donor.MobilePhone, "[^0-9]", "");
			pdfPassword = security.UserPassword;
			_logger.Info($"Donor ID: {donor.Id}, Donor Name: {donor.Name}, PdfPassword: {pdfPassword}");
		}

		using var pdfStream = _fileSystem.File.Create(pdfFileName);
		pdfDocument.Save(pdfStream);

		pdfDocument.Close(true);

		return pdfPassword;
	}

	public async Task AllPdf(string folder, PrintDialog pd, Thickness margins)
	{
		Running = true;
		var DonorIds = _donations.Select(x => x.DonorId).Distinct().ToList();
		Dictionary<int, bool> donorDone = new Dictionary<int, bool>();
		Total = DonorIds.Count;
		Current = 0;

		foreach (var donorId in DonorIds)
		{
			Donor donor = await _donorServices.GetDonorByIdAsync(donorId);

			await SetDonor(donor);

			if (donorDone.ContainsKey(donorId) || DeferToPrimary(donor))
			{
				Current++;
				continue;
			}

			if (null != _givingGroup)
				_givingGroup.ForEach(x => donorDone[x.Id] = true);

			string xpsFileName = folder + $"\\{Name}.xps";
			if (_fileSystem.File.Exists(xpsFileName))
				_fileSystem.File.Delete(xpsFileName);

			string pdfFileName = folder + $"\\{Name}.pdf";
			if (_fileSystem.File.Exists(pdfFileName))
				_fileSystem.File.Delete(pdfFileName);

			_ = await GeneratePdf(false, donor, xpsFileName, pdfFileName, pd, margins);

			Current++;
		}
		DirectoryInfo dirInfo = new DirectoryInfo(folder);

		dirInfo.EnumerateFiles("*.xps").ToList().ForEach(f => f.Delete());

		Running = false;
	}

	public async Task MockRun()
	{
		_todaysDate = DateTime.Now;

		Running = true;
		_donorIds = _donations.Select(x => x.DonorId).Distinct().ToList();
		Dictionary<int, bool> donorDone = new Dictionary<int, bool>();
		string? tmpFolder = _fileSystem!.Path.GetTempPath();
		string? from = _appSettingsServices.Get().EmailAccount;
		Total = _donorIds.Count;
		Current = 1;
		_mockRunCollection = new ObservableCollection<NamedDonorReport>();
		MockRunView.Source = _mockRunCollection;

		foreach (var donorId in _donorIds)
		{
			Donor donor = await _donorServices.GetDonorByIdAsync(donorId);

			await SetDonor(donor);

			NamedDonorReport namedDonorReport = new NamedDonorReport()
			{
				DonorId = donorId,
				LastName = donor.LastName,
				FirstName = donor.FirstName,
				DontEmailReport = donor.DontEmailReport,
				Email = donor.Email,
				MobilePhone = donor.MobilePhone,
			};

			// donor could have been done by inclusion in a shared family report
			if (donorDone.ContainsKey(donorId) || DeferToPrimary(donor))
			{
				var groupNames = _givingGroup?.Select(x => x.Name);
				namedDonorReport.Action = "Sent with group: " + string.Join("; ", groupNames);
				_mockRunCollection.Add(namedDonorReport);
				Current++;
				continue;
			}

			if (null != _givingGroup)
			{
				_givingGroup.ForEach(x => donorDone[x.Id] = true);
			}

			if (ShouldPrintReport(donor))
			{
				namedDonorReport.Action = $"Print";
			}
			else
			{
				namedDonorReport.Action = $"Email";
				if (Encrypt && !string.IsNullOrEmpty(donor.MobilePhone))
				{
					namedDonorReport.Action += $"; Password: {donor.MobilePhone}";
				}
			}

			_mockRunCollection.Add(namedDonorReport);

			Person = donor.Name;

			Current++;
			await _dispatcherWrapper.Yield();
		}
		Current--;

		Running = false;
	}

	public async Task PrintIndividual(PrintDialog pd, Thickness margins)
	{
		_doc.PageWidth = pd.PrintableAreaWidth;
		_doc.PageHeight = pd.PrintableAreaHeight;
		_doc.ColumnWidth = _doc.PageWidth;
		_doc.PagePadding = margins;

		await FormatLetter(_donor);

		var docpaginator = ((IDocumentPaginatorSource)_doc).DocumentPaginator;
		docpaginator.PageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaWidth);
		IDocumentPaginatorSource idocument = _doc as IDocumentPaginatorSource;

		pd.PrintDocument(idocument.DocumentPaginator, _donor.Name);
	}

	public async Task Print(PrintDialog pd, Thickness margins)
	{
		_todaysDate = DateTime.Now;

		Running = true;
		_donorIds = _donations.Select(x => x.DonorId).Distinct().ToList();
		Dictionary<int, bool> donorDone = new Dictionary<int, bool>();
		string? tmpFolder = _fileSystem!.Path.GetTempPath();
		string? from = _appSettingsServices.Get().EmailAccount;
		Total = _donorIds.Count;
		Current = 1;

		_doc = new FlowDocument();
		_doc.PageWidth = pd.PrintableAreaWidth;
		_doc.PageHeight = pd.PrintableAreaHeight;
		_doc.ColumnWidth = _doc.PageWidth;
		_doc.PagePadding = margins;

		foreach (var donorId in _donorIds)
		{
			Donor donor = await _donorServices.GetDonorByIdAsync(donorId);

			// donor could have been done by inclusion in a shared family report
			_logger.Info($"Donor ID: {donorId}, Donor Name: {donor.Name}, {Current} of {Total}");

			await SetDonor(donor);

			if (donorDone.ContainsKey(donorId) || DeferToPrimary(donor))
			{
				_logger.Info($"Donor ID: {donorId}, defer to primary for group");
				Current++;
				continue;
			}

			if (!ShouldPrintReport(donor))
			{
				// emailed report, so don't print
				_logger.Info($"Donor ID: {donorId}, Donor Name: {donor.Name}, Report will be emailed instead of printed, Email: {donor.Email} DontEmailReport: {donor.DontEmailReport}");
				Current++;
				continue;
			}

			if (null != _givingGroup)
			{
				_givingGroup.ForEach(x => donorDone[x.Id] = true);
			}

			_logger.Info($"Donor ID: {donorId}, Donor Name: {donor.Name}, {Current} of {Total}");

			var docpaginator = ((IDocumentPaginatorSource)_doc).DocumentPaginator;
			docpaginator.PageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaWidth);
			IDocumentPaginatorSource idocument = _doc as IDocumentPaginatorSource;

			pd.PrintDocument(idocument.DocumentPaginator, donor.Name);

			string action = "Print";

			_logger.Info($"Succesfully printed Donor ID: {donor.Id}, Donor Name: {donor.Name}, {Current} of {Total}");

			if (_donorReports.Any(report => report.DonorId == donor.Id))
			{
				var report = _donorReports.FirstOrDefault(report => report.DonorId == donor.Id);
				if (report != null)
				{
					report.LastSent = _todaysDate;
					report.Action = action;
				}
			}
			else
			{
				DonorReport donorReport = new DonorReport()
				{
					DonorId = donor.Id,
					LastSent = _todaysDate,
					Action = action,
				};
				_donorReports.Add(donorReport);
			}

			Person = donor.Name;
			Current++;
		}
		Current--;

		Running = false;

		await _donorReportsServices.Save(_donorReports);

		_namedDonorReports = new ObservableCollection<NamedDonorReport>(await _donorReportsServices.LoadNamed());
		DonorReportView.Source = _namedDonorReports;
	}

	private bool ShouldPrintReport(Donor donor)
	{
		return string.IsNullOrEmpty(donor.Email) || true == donor.DontEmailReport;
	}

	private bool DeferToPrimary(Donor donor)
	{
		if (null != _givingGroup)
		{
			if (donor.FamilyRelationship != enumFamilyRelationship.Primary && null != _primary && true == _donorIds?.Contains(_primary.Id))
			{
				// donor is not primary and primary donated in this time window, so skip this donor and report for primary donor
				return true;
			}
			else
			{
				// primary didn't donate in this time window, so create report for this donor
				return false;
			}
		}
		else
		{
			// not part of a group so don't defer
			return false;
		}
	}

	public async Task Email(PrintDialog pd, Thickness margins)
	{
		_todaysDate = DateTime.Now;

		Running = true;
		_donorIds = _donations.Select(x => x.DonorId).Distinct().ToList();
		Dictionary<int, bool> donorDone = new Dictionary<int, bool>();
		string? tmpFolder = _fileSystem!.Path.GetTempPath();
		string? from = _appSettingsServices.Get().EmailAccount;
		Total = _donorIds.Count;
		Current = 1;
		PasswordBox? password = null;

		if (string.IsNullOrEmpty(Persist.Default.EncryptedEmailPassword))
		{
			var dlg = DependencyInjection.Resolve<EmailAccountPasswordView>();

			dlg.Account.Text = from;

			if (false == dlg.ShowDialog())
			{
				string msg = $"Cannot perform this operation without a password for Email account {from}";
				_logger.Info(msg);
				MessageBox.Show(msg);
				return;
			}

			password = dlg.PasswordBox;
		}

		_doc = new FlowDocument();
		_doc.PageWidth = pd.PrintableAreaWidth;
		_doc.PageHeight = pd.PrintableAreaHeight;
		_doc.ColumnWidth = _doc.PageWidth;
		_doc.PagePadding = margins;

		foreach (var donorId in _donorIds)
		{
			Donor donor = await _donorServices.GetDonorByIdAsync(donorId);

			_logger.Info($"Donor ID: {donorId}, Donor Name: {donor.Name}, {Current} of {Total}");

			await SetDonor(donor);

			// donor could have been done by inclusion in a shared family report
			if (donorDone.ContainsKey(donorId) || DeferToPrimary(donor))
			{
				_logger.Info($"Donor ID: {donorId}, Already sent to this family");
				Current++;
				continue;
			}

			if (ShouldPrintReport(donor))
			{
				// print report instead of email
				_logger.Info($"Donor ID: {donorId}, Donor Name: {donor.Name}, Report will not be sent to this donor, Email: {donor.Email} DontEmailReport: {donor.DontEmailReport}");
				Current++;
				continue;
			}

			if (null != _givingGroup)
			{
				_givingGroup.ForEach(x => donorDone[x.Id] = true);
			}

			_logger.Info($"Donor ID: {donorId}, Donor Name: {donor.Name}, {Current} of {Total}");

			string xpsFileName = tmpFolder + $"\\{Name}.xps";
			if (_fileSystem.File.Exists(xpsFileName))
				_fileSystem.File.Delete(xpsFileName);

			string pdfFileName = tmpFolder + $"\\{Name}.pdf";
			if (_fileSystem.File.Exists(pdfFileName))
				_fileSystem.File.Delete(pdfFileName);

			string pdfPassword = await GeneratePdf(Encrypt, donor, xpsFileName, pdfFileName, pd, margins);

			// we don't want to await the send because it takes a lot of time and can be parallized
			await SendEmail(password, donor, from, donor.Email, pdfFileName, pdfPassword);

			Current++;
		}
		Current--;

		Running = false;

		await _donorReportsServices.Save(_donorReports);

		_namedDonorReports = new ObservableCollection<NamedDonorReport>(await _donorReportsServices.LoadNamed());
		DonorReportView.Source = _namedDonorReports;
	}

	private async Task SendEmail(PasswordBox? password, Donor donor, string from, string to, string pdfFileName, string pdfPassword)
	{
		await Task.Run(() =>
		{
			MailMessage message = new MailMessage(
										from,
										to,
										EmailSubject,
										EmailBody);

			// Create  the file attachment for this e-mail message.
			Attachment data = new Attachment(pdfFileName, MediaTypeNames.Application.Octet);
			// Add time stamp information for the file.
			ContentDisposition disposition = data.ContentDisposition;
			disposition.CreationDate = File.GetCreationTime(pdfFileName);
			disposition.ModificationDate = File.GetLastWriteTime(pdfFileName);
			disposition.ReadDate = File.GetLastAccessTime(pdfFileName);
			// Add the file attachment to this e-mail message.
			message.Attachments.Add(data);

			bool succeeded = false;

			try
			{
				succeeded = false;

				const int maxtries = 5;
				for (int i = 0; i < maxtries && !succeeded; i++)
				{
					try
					{
						var smtp = new SmtpClient
						{
							Host = _appSettingsServices.Get().EmailSmtpServer,
							Port = (null == _appSettingsServices.Get().EmailServerPort) ? 0 : _appSettingsServices.Get().EmailServerPort!.Value,
							EnableSsl = _appSettingsServices.Get().EmailEnableSsl,
							DeliveryMethod = SmtpDeliveryMethod.Network,
							UseDefaultCredentials = false,
							Credentials = new NetworkCredential(
								_appSettingsServices.Get().EmailAccount,
								null != password
									? password.Password
									: Encoding.Default.GetString(ProtectedData.Unprotect(Persist.Default.EncryptedEmailPassword.Split(' ').Select(byte.Parse).ToArray(), GeneralViewModel.s_additionalEntropy, DataProtectionScope.CurrentUser)))
						};

						smtp.Send(message);
						_logger.Info($"Succesfully sent on attempt: {i}, Donor ID: {donor.Id}, Donor Name: {donor.Name}, {Current} of {Total}");
						succeeded = true;
						break;
					}
					catch (Exception ex)
					{
						_logger.Err(ex, $"Exception on send attempt: {i}, Donor ID: {donor.Id}, Donor Name: {donor.Name}, {Current} of {Total}");
						if (i == maxtries - 1)
						{
							string logFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Donation tracker");
							MessageBox.Show($"Exception occured when sending reports via email. Check the log for details {logFolder}.");
						}
						else
						{
							MessageBox.Show($"Exception occured when sending reports via email. Click OK to retry.");
						}
					}
				}

				string action;

				if (succeeded)
				{
					if (Encrypt && !string.IsNullOrEmpty(pdfPassword))
					{
						action = $"Email(password: {pdfPassword})";
					}
					else
					{
						action = "Email";
					}

					if (true == _donorReports?.Any(report => report.DonorId == donor.Id))
					{
						var report = _donorReports.FirstOrDefault(report => report.DonorId == donor.Id);
						if (report != null)
						{
							report.LastSent = _todaysDate;
							report.Action = action;
						}
					}
					else
					{
						DonorReport donorReport = new DonorReport()
						{
							DonorId = donor.Id,
							LastSent = _todaysDate,
							Action = action,
						};
						_donorReports?.Add(donorReport);
					}
				}
				else
				{
					action = "Failed sending Email";
					if (true == _donorReports?.Any(report => report.DonorId == donor.Id))
					{
						var report = _donorReports.FirstOrDefault(report => report.DonorId == donor.Id);
						if (report != null)
						{
							report.LastSent = _todaysDate;
							report.Action = action;
						}
					}
					else
					{
						DonorReport donorReport = new DonorReport()
						{
							DonorId = donor.Id,
							LastSent = _todaysDate,
							Action = action,
						};
						_donorReports?.Add(donorReport);
					}
				}

				Person = donor.Name;
			}
			catch (Exception ex)
			{
				_logger.Err(ex, $"Exception most likely prior to send, Donor ID: {donor.Id}, Donor Name: {donor.Name}, {Current} of {Total}");
				string logFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Donation tracker");
				MessageBox.Show($"Exception occured when sending reports via email. Check the log for details {logFolder}.");
				Running = false;

			}
			data.Dispose();
		});
	}
}
