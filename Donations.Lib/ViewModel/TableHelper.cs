using Donations.Lib.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace Donations.Lib.ViewModel;

public class TableHelper
{
	/// <summary>
	/// Thi member function adds the FlowDocument contstructs to produce the Table which will display
	/// the Category subtotals. These are the values that are entered into the GL program.
	/// </summary>
	public Table? CreateCategorySumsTable(CollectionViewSource categorySumCollection, string? font, double size)
	{
		try
		{
			if (0 == categorySumCollection.View.Cast<CategorySum>().Count()) return null;

			var table = new Table();

			double[] colsize = new double[2];
			var col0 = new TableColumn();
			var col1 = new TableColumn();
			table.Columns.Add(col0);
			table.Columns.Add(col1);

			table.RowGroups.Add(new TableRowGroup());

			TableRow currentRow = new TableRow() { FontFamily = new FontFamily(font), FontSize = size, FontWeight = FontWeights.Bold };

			// Add the first (title) row.
			table.RowGroups[0].Rows.Add(currentRow);

			// Add cells with content to the second row.
			AddCell(currentRow, colsize, 0, "Category");
			AddCell(currentRow, colsize, 0, "Subtotal", TextAlignment.Right);

			int i = 0;
			foreach (var item in categorySumCollection.View)
			{
				CategorySum? catsum = item as CategorySum;

				currentRow = new TableRow() { FontFamily = new FontFamily(font), FontSize = size };

				// Add the first (title) row.
				table.RowGroups[0].Rows.Add(currentRow);

				currentRow.Background = (0 == (i % 2)) ? new SolidColorBrush(Color.FromArgb(0xff, 0xdf, 0xef, 0xdf)) : Brushes.White;

				AddCell(currentRow, colsize, 0, catsum?.Category);
				AddCell(currentRow, colsize, 1, catsum?.Sum.ToString("C2"), TextAlignment.Right);

				i++;
			}

			col0.Width = new GridLength(colsize[0] + 20, GridUnitType.Pixel);
			col1.Width = new GridLength(colsize[1] + 20, GridUnitType.Pixel);

			return table;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "TableHelper.CreateDonationDetailsTable()");
		}
		return null;
	}

	/// <summary>
	/// Thi member function adds the FlowDocument contstructs to produce the Table which will display
	/// the donation details that went into the batch. These contain all the donations by each
	/// donor, to each different category.
	/// </summary>
	public Table? CreateDonationDetailsTable(CollectionViewSource donationDetailsCollection, string? font, double size, double printAreaWidth)
	{
		try
		{
			if (0 == donationDetailsCollection.View.Cast<Donation>().Count()) return null;

			var table = new Table();
			int numCols = 6;

			double[] colsize = new double[numCols];
			TableColumn[] cols = new TableColumn[numCols];

			int i;

			for (i = 0; i < numCols; i++)
			{
				cols[i] = new TableColumn();
				table.Columns.Add(cols[i]);
			}

			table.RowGroups.Add(new TableRowGroup());

			TableRow currentRow = new TableRow() { FontFamily = new FontFamily(font), FontSize = size, FontWeight = FontWeights.Bold };

			// Add the first (title) row.
			table.RowGroups[0].Rows.Add(currentRow);

			// Global formatting for the header row.
			currentRow.FontWeight = FontWeights.Bold;

			// Add cells with content to the second row.
			i = 0;
			AddCell(currentRow, colsize, i++, "Date");
			AddCell(currentRow, colsize, i++, "Category");
			AddCell(currentRow, colsize, i++, "Amount", TextAlignment.Right);
			AddCell(currentRow, colsize, i++, "Method", TextAlignment.Right);
			AddCell(currentRow, colsize, i++, "Tx/Ck #");
			AddCell(currentRow, colsize, i++, "Note");

			int row = 0;
			foreach (var item in donationDetailsCollection.View)
			{
				Donation donation = item as Donation;

				currentRow = new TableRow() { FontFamily = new FontFamily(font), FontSize = size };

				// Add the first (title) row.
				table.RowGroups[0].Rows.Add(currentRow);

				currentRow.Background = (0 == (row % 2)) ? new SolidColorBrush(Color.FromArgb(0xff, 0xdf, 0xef, 0xdf)) : Brushes.White;

				// Add cells with content to the row.
				i = 0;

				AddCell(currentRow, colsize, i++, donation?.Date);
				AddCell(currentRow, colsize, i++, donation?.Category);
				AddCell(currentRow, colsize, i++, donation?.Value.ToString("C2"), TextAlignment.Right);
				AddCell(currentRow, colsize, i++, donation?.Method.ToString(), TextAlignment.Right);
				AddCell(currentRow, colsize, i++, donation?.TransactionNumber);
				string note = "";
				if ("Adventist Giving" != donation?.Note)
				{
					note = donation?.Note;
				}
				AddCell(currentRow, colsize, i++, note);

				row++;
			}

			var width = printAreaWidth;

			for (i = 0; i < numCols; i++)
			{
				width -= colsize[i] + 10; // add 10 extra pixels for margin
				cols[i].Width = new GridLength(colsize[i] + 10, GridUnitType.Pixel);
			}
			if (0 > width)
			{
				// table is too wide so reduce category column
				cols[1].Width = new GridLength(colsize[1] + width, GridUnitType.Pixel);
			}
			return table;
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "TableHelper.CreateDonationDetailsTable()");
		}
		return null;
	}

	/// <summary>
	/// Thi member function adds the FlowDocument contstructs to produce the Table which will display
	/// the donation details that went into the batch. These contain all the donations by each
	/// donor, to each different category.
	/// </summary>
	public Table? CreateDonationDetailsTable(CollectionViewSource donationDetailsCollection, string? font, double size, double printAreaWidth, DonationTableColumnDescriptor[] columns, int lowestPriorityCol)
	{
		if (0 == donationDetailsCollection.View.Cast<Donation>().Count()) return null;

		var table = new Table();
		int numCols = columns.Length;

		double[] colsize = new double[numCols];
		TableColumn[] cols = new TableColumn[numCols];

		int i;

		for (i = 0; i < numCols; i++)
		{
			cols[i] = new TableColumn();
			table.Columns.Add(cols[i]);
		}

		table.RowGroups.Add(new TableRowGroup());

		TableRow currentRow = new TableRow() { FontFamily = new FontFamily(font), FontSize = size, FontWeight = FontWeights.Bold };

		// Add the first (title) row.
		table.RowGroups[0].Rows.Add(currentRow);

		// Global formatting for the header row.
		currentRow.FontWeight = FontWeights.Bold;

		// Add cells with content to the second row.
		for (i = 0; i < numCols; i++)
		{
			AddCell(currentRow, colsize, i, columns[i].ColumnHeader, columns[i].Alignment);
		}

		int row = 0;
		foreach (var item in donationDetailsCollection.View)
		{
			Donation donation = item as Donation;

			currentRow = new TableRow() { FontFamily = new FontFamily(font), FontSize = size };

			// Add the first (title) row.
			table.RowGroups[0].Rows.Add(currentRow);

			currentRow.Background = (0 == (row % 2)) ? new SolidColorBrush(Color.FromArgb(0xff, 0xdf, 0xef, 0xdf)) : Brushes.White;

			// Add cells with content to the row.
			for (i = 0; i < numCols; i++)
			{
				AddCell(currentRow, colsize, i, columns[i].GetValue(donation), columns[i].Alignment);
			}

			row++;
		}

		var width = printAreaWidth;

		for (i = 0; i < numCols; i++)
		{
			width -= colsize[i] + 10; // add 10 extra pixels for margin
			cols[i].Width = new GridLength(colsize[i] + 10, GridUnitType.Pixel);
		}
		if (0 > width)
		{
			// table is too wide so reduce category column
			if (width < colsize[lowestPriorityCol])
				width = -colsize[lowestPriorityCol];

			cols[lowestPriorityCol].Width = new GridLength(colsize[lowestPriorityCol] + width, GridUnitType.Pixel);
		}

		return table;
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
	public void AddCell(TableRow row, double[] colsize, int index, string text, TextAlignment justification = TextAlignment.Left)
	{
		// measure width of cell
		TextBlock textBlock = new TextBlock() { Text = text, FontFamily = row.FontFamily, FontSize = row.FontSize, FontWeight = row.FontWeight };
		if (justification == TextAlignment.Right)
		{
			textBlock.Margin = new Thickness(0, 0, 10, 0);
		}
		textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
		var ret = textBlock.DesiredSize;
		if (ret.Width > colsize[index])
		{
			colsize[index] = ret.Width;
		}

		row.Cells.Add(new TableCell(new Paragraph(new Run(text)) { TextAlignment = justification }));
	}
}
