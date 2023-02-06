using Donations.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Donations.ViewModel
{
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
	public class BatchPrintViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// This printing, from what I understand uses 96 dpi. I don't know if the user can change that
		/// from the print dialog.
		/// </summary>
		public const double _dpi = 96.0;

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		public ObservableCollection<CategorySum>? CategorySums { get; set; } = new ObservableCollection<CategorySum>();
		public CollectionViewSource CategorySumSource { get; set; } = new CollectionViewSource();
		public CollectionViewSource DonationDetailsSource { get; set; } = new CollectionViewSource();

		private string? Header => $"Batch date: {BatchDate}\r\n" +
								$"Batch total: {_total.ToString("C2")}\r\n" +
								$"Batch source: {_batch?.Source.GetDescription()}\r\n" +
								$"Batch number: {_batch?.Id.ToString()}\r\n" +
								$"Batch note: {_batch?.Note}\r\n" +
								$"Created by: {Created}";

		/// <summary>
		/// This is populated in the class constructor from the Fonts.SystemFontFamilies collection.
		/// The reason I did this rather than direcly populating the control is that when i did that
		/// I couldn't select by setting the bound SelectedItem.
		/// </summary>
		public ObservableCollection<string> FontList { get; set; } = new ObservableCollection<string>();

		private string? _selectedFont;
		/// <summary>
		/// The SelectedFont prperty is used to initially select the last font used, and also to
		/// receive the latest font chosen by the operator.
		/// </summary>
		public string? SelectedFont
		{
			get { return _selectedFont; }
			set
			{
				_selectedFont = value;
				OnPropertyChanged();
				Redraw();
			}
		}

		private double _selectedSize;
		/// <summary>
		/// The SelectedSize prperty is used to initially select the last font size used, and also to
		/// receive the latest font size chosen by the operator.
		/// </summary>
		public double SelectedSize
		{
			get { return _selectedSize; }
			set
			{
				_selectedSize = value;
				if (null != _doc) _doc.FontSize = value;
				OnPropertyChanged();
				Redraw();
			}
		}

		private double _leftMargin;
		/// <summary>
		/// The LeftMargin property is provided for the sole purpose of allowing a larger left margin
		/// for three ring binding. Otherwise, I would have only given a single margin parameter.
		/// </summary>
		public double LeftMargin
		{
			get { return _leftMargin; }
			set
			{
				_leftMargin = value;
				OnPropertyChanged();
			}
		}

		private double _otherMargins;
		/// <summary>
		/// The OtherMargins property contains a single value that is applied to the top/right/bottom
		/// page margins.
		/// </summary>
		public double OtherMargins
		{
			get { return _otherMargins; }
			set
			{
				_otherMargins = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The FontSize property is a simple collection of some of the most popular font sizes for
		/// this particular report printout. If other font sizes are needed, they can simply be typeed
		/// because the ComboBox has the edit capability enabled.
		/// </summary>
		public ObservableCollection<double> FontSizes { get; set; } = new ObservableCollection<double>() { 8, 9, 10, 11, 12, 13, 14, 15, 16 };

		private string? BatchDate => (null == _batch) ? null : DateOnly.Parse(_batch.Date).ToLongDateString();
		private string? Created => di.Username + " on " + DateTime.Now.ToString("G");

		private FlowDocument? _doc;
		private Batch? _batch;
		private Dictionary<string, CategorySum> _categorySumDict = new Dictionary<string, CategorySum>();
		private ObservableCollection<Donation>? _batchDonations;
		private double _total = 0;
		private double _printAreaWidth;

		/// <summary>
		/// The constructor creates the FontList which is bound to the FontFamily selection combobox.
		/// That ComboBox shows each font choice by name, displayed in the given font face. Then the
		/// preferred font face and size are set for starters, and the page margins are also restored
		/// from settings.
		/// </summary>
		public BatchPrintViewModel()
		{
			foreach (var font in Fonts.SystemFontFamilies)
			{
				FontList.Add(font.Source);
			}
			SelectedFont = Persist.Default.PrintFontFamily;
			SelectedSize = Persist.Default.PrintFontSize;
			LeftMargin = Persist.Default.PrintLeftMargin;
			OtherMargins = Persist.Default.PrintOtherMargins;
		}

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
		public void Loaded(FlowDocument doc, Batch batch, ObservableCollection<Donation> collection)
		{
			_printAreaWidth = (8.5 - LeftMargin - OtherMargins) * _dpi;

			_doc = doc;
			_batch = batch;

			_batchDonations = collection;

			CategorySumSource.Source = CategorySums;
			CategorySumSource.SortDescriptions.Add(new SortDescription() { PropertyName = "Category", Direction = ListSortDirection.Ascending });

			DonationDetailsSource.Source = _batchDonations;
			DonationDetailsSource.SortDescriptions.Add(new SortDescription() { PropertyName = "Name", Direction = ListSortDirection.Ascending });
			DonationDetailsSource.SortDescriptions.Add(new SortDescription() { PropertyName = "Category", Direction = ListSortDirection.Ascending });

			ComputeSum();

			Redraw();
		}

		/// <summary>
		/// This is called from the UserControl unloaded handler. These statements will persist the last
		/// font and margin selections by the user, so they can be restored the next time this control
		/// is brought up.
		/// </summary>
		public void Unloaded()
		{
			Persist.Default.PrintFontFamily = SelectedFont;
			Persist.Default.PrintFontSize = SelectedSize;
			Persist.Default.PrintLeftMargin = LeftMargin;
			Persist.Default.PrintOtherMargins = OtherMargins;
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

				AddCategorySumsTable(_doc);
				AddDonationDetailsTable(_doc, _batch.Source == enumSource.AdventistGiving ? true : false);
			}
			catch { }
		}

		/// <summary>
		/// Thi member function adds the FlowDocument contstructs to produce the Table which will display
		/// the Category subtotals. These are the values that are entered into the GL program.
		/// </summary>
		/// <param name="doc">The PrintArea (FlowDocument) that is being built up to contain the printed
		/// report.</param>
		private void AddCategorySumsTable(FlowDocument doc)
		{
			var table = new Table();

			double[] colsize = new double[2];
			var col0 = new TableColumn();
			var col1 = new TableColumn();
			table.Columns.Add(col0);
			table.Columns.Add(col1);

			doc.Blocks.Add(table);

			table.RowGroups.Add(new TableRowGroup());

			TableRow currentRow = new TableRow() { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize, FontWeight = FontWeights.Bold };

			// Add the first (title) row.
			table.RowGroups[0].Rows.Add(currentRow);

			// Add cells with content to the second row.
			AddCell(currentRow, colsize, 0, "Category");
			AddCell(currentRow, colsize, 0, "Subtotal", TextAlignment.Right);

			int i = 0;
			foreach (var item in CategorySumSource.View)
			{
				CategorySum? catsum = item as CategorySum;

				currentRow = new TableRow() { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize };

				// Add the first (title) row.
				table.RowGroups[0].Rows.Add(currentRow);

				currentRow.Background = (0 == (i % 2)) ? new SolidColorBrush(Color.FromArgb(0xff, 0xdf, 0xef, 0xdf)) : Brushes.White;

				AddCell(currentRow, colsize, 0, catsum?.Category);
				AddCell(currentRow, colsize, 1, catsum?.Sum.ToString("C2"), TextAlignment.Right);

				i++;
			}

			col0.Width = new GridLength(colsize[0] + 20, GridUnitType.Pixel);
			col1.Width = new GridLength(colsize[1] + 20, GridUnitType.Pixel);
		}

		/// <summary>
		/// Thi member function adds the FlowDocument contstructs to produce the Table which will display
		/// the donation details that went into the batch. These contain all the donations by each
		/// donor, to each different category.
		/// </summary>
		/// <param name="doc">The PrintArea (FlowDocument) that is being built up to contain the printed
		/// report.</param>
		/// <param name="adventistGiving">This bool allows the donation details table to be customized
		/// to not contain redundant column data. For example, there is no need to have a column that
		/// simply contains AdventistGiving, for each and every row in the details table.</param>
		private void AddDonationDetailsTable(FlowDocument doc, bool adventistGiving)
		{
			var table = new Table();
			int numCols = 5;

			double[] colsize = new double[numCols];
			TableColumn[] cols = new TableColumn[numCols];

			int i;

			for (i = 0; i < numCols; i++)
			{
				cols[i] = new TableColumn();
				table.Columns.Add(cols[i]);
			}

			doc.Blocks.Add(table);

			table.RowGroups.Add(new TableRowGroup());

			TableRow currentRow = new TableRow() { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize, FontWeight = FontWeights.Bold };

			// Add the first (title) row.
			table.RowGroups[0].Rows.Add(currentRow);

			// Global formatting for the header row.
			currentRow.FontWeight = FontWeights.Bold;

			// Add cells with content to the second row.
			i = 0;
			AddCell(currentRow, colsize, i++, "Name");
			AddCell(currentRow, colsize, i++, "Category");
			if (adventistGiving)
			{
				AddCell(currentRow, colsize, i++, "Date");
				AddCell(currentRow, colsize, i++, "Transaction #");
			}
			else
			{
				AddCell(currentRow, colsize, i++, "Check #");
			}
			AddCell(currentRow, colsize, i++, "Amount", TextAlignment.Right);
			if (!adventistGiving)
				AddCell(currentRow, colsize, i++, "Note");

			int row = 0;
			foreach (var item in DonationDetailsSource.View)
			{
				Donation donation = item as Donation;

				currentRow = new TableRow() { FontFamily = new FontFamily(SelectedFont), FontSize = SelectedSize };

				// Add the first (title) row.
				table.RowGroups[0].Rows.Add(currentRow);

				currentRow.Background = (0 == (row % 2)) ? new SolidColorBrush(Color.FromArgb(0xff, 0xdf, 0xef, 0xdf)) : Brushes.White;

				// Add cells with content to the row.
				i = 0;
				AddCell(currentRow, colsize, i++, donation?.Name);
				AddCell(currentRow, colsize, i++, donation?.Category);
				if (adventistGiving) AddCell(currentRow, colsize, i++, donation?.Date);
				string str = donation?.TransactionNumber;
				if (!adventistGiving && donation?.Method == enumMethod.Cash)
					str = "Cash";
				AddCell(currentRow, colsize, i++, str);
				AddCell(currentRow, colsize, i++, donation?.Value.ToString("C2"), TextAlignment.Right);
				if (!adventistGiving) AddCell(currentRow, colsize, i++, donation?.Note);

				row++;
			}

			var width = _printAreaWidth;

			for (i = 0; i < numCols; i++)
			{
				width -= colsize[i] + 10; // add 10 extra pixels for margin
				cols[i].Width = new GridLength(colsize[i] + 10, GridUnitType.Pixel);
			}
			if (!adventistGiving && 0 > width)
			{
				// not would overflow margin and be cropped, so shrink it so it will wrap
				cols[numCols - 1].Width = new GridLength(colsize[numCols - 1] + width, GridUnitType.Pixel);
			}
		}

		/// <summary>
		/// This private member function performs some common funcitonality for each cell that is added
		/// to the table. In addition to adding the cell, a measurement is taken of the cell contents
		/// rendered with the given font family and size. This measurement is later used to size the
		/// table column widths.
		/// </summary>
		/// <param name="row">Table row object to which the column cells are added.</param>
		/// <param name="colsize">Array of column sizes which will always contain the largest column
		/// size up to the given row.</param>
		/// <param name="index">The Index is simply the zero based calumn index.</param>
		/// <param name="text">This is the text that will be inserted into the column cell, and also
		/// measured.</param>
		/// <param name="justification">This is for indicating left or right justification specifically
		/// so that $ amounts and their column header can be riht justified.</param>
		private void AddCell(TableRow row, double[] colsize, int index, string text, TextAlignment justification = TextAlignment.Left)
		{
			// measure width of cell
			TextBlock textBlock = new TextBlock() { Text = text, FontFamily = row.FontFamily, FontSize = row.FontSize, FontWeight = row.FontWeight };
			textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			var ret = textBlock.DesiredSize;
			if (ret.Width > colsize[index])
			{
				colsize[index] = ret.Width;
			}

			row.Cells.Add(new TableCell(new Paragraph(new Run(text)) { TextAlignment = justification }));
		}

		/// <summary>
		/// This method function is called to compute the actual sum of all the donations in
		/// this batch. It should match with the target Total entered at the top of this view.
		/// </summary>
		private void ComputeSum()
		{
			CategorySums.Clear();
			_categorySumDict.Clear();
			_total = 0;

			foreach (var donation in _batchDonations)
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
					CategorySums.Add(sum);
					_categorySumDict[sum.Category] = sum;
				}
				_total += donation.Value;
			}

			CategorySumSource.View.Refresh();
		}
	}
}
