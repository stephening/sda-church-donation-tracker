using Donations.Lib.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for BatchPrintView.xaml
	/// </summary>
	public partial class BatchPrintView : UserControl
	{
		public FixedDocumentSequence Document { get; set; }

		private BatchPrintViewModel? _viewModel;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		public BatchPrintView()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
		{
			InitializeComponent();
		}

		private void Print(object sender, RoutedEventArgs e)
		{
			PrintDialog pd = new PrintDialog();
			if (pd.ShowDialog() == true)
			{
				PrintArea.PageWidth = pd.PrintableAreaWidth;
				PrintArea.PageHeight = pd.PrintableAreaHeight;
				PrintArea.ColumnWidth = PrintArea.PageWidth;
				PrintArea.PagePadding = new Thickness(_viewModel.LeftMargin * PrintOptionsView._dpi,
					_viewModel.OtherMargins * PrintOptionsView._dpi,
					_viewModel.OtherMargins * PrintOptionsView._dpi,
					_viewModel.OtherMargins * PrintOptionsView._dpi);
				var docpaginator = ((IDocumentPaginatorSource)PrintArea).DocumentPaginator;
				docpaginator.PageSize = new Size(pd.PrintableAreaWidth, pd.PrintableAreaWidth);
				IDocumentPaginatorSource idocument = PrintArea as IDocumentPaginatorSource;

				pd.PrintDocument(idocument.DocumentPaginator, "Batch report");
			}
		}

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
			_viewModel?.Unloaded();
		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as BatchPrintViewModel;
		}
	}
}
