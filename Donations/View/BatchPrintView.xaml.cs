using Donations.ViewModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for BatchPrintView.xaml
	/// </summary>
	public partial class BatchPrintView : UserControl
    {
        public FixedDocumentSequence Document { get; set; }

		private BatchPrintViewModel? _viewModel;

		public BatchPrintView()
        {
            InitializeComponent();

			_viewModel = DataContext as BatchPrintViewModel;
        }

		private void Print(object sender, RoutedEventArgs e)
		{
			PrintDialog pd = new PrintDialog();
			if (pd.ShowDialog() == true)
			{
				PrintArea.PageWidth = pd.PrintableAreaWidth;
				PrintArea.PageHeight = pd.PrintableAreaHeight;
				PrintArea.ColumnWidth = PrintArea.PageWidth;
				PrintArea.PagePadding = new Thickness(_viewModel.LeftMargin * BatchPrintViewModel._dpi,
					_viewModel.OtherMargins * BatchPrintViewModel._dpi,
					_viewModel.OtherMargins * BatchPrintViewModel._dpi,
					_viewModel.OtherMargins * BatchPrintViewModel._dpi);
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
	}
}
