using Donations.Model;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for GeneralView.xaml
	/// </summary>
	public partial class GeneralView : UserControl
    {
        public GeneralView()
        {
            InitializeComponent();
        }

        private void ExportCategoriesCsv(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Categories (*.csv)|*.csv";

            if (dlg.ShowDialog() == true)
            {
                di.Data.ExportCsv<Category>(dlg.FileName, di.Data.CatList);
            }
        }

        private void ExportDonorsCsv(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Donors (*.csv)|*.csv";

            if (dlg.ShowDialog() == true)
            {
                di.Data.ExportCsv<Donor>(dlg.FileName, di.Data.DonorList);
            }
        }

        private void ExportDonationsCsv(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Donations (*.csv)|*.csv";

            if (dlg.ShowDialog() == true)
            {
                di.Data.ExportCsv<Donation>(dlg.FileName, di.Data.DonationList);
            }
        }
    }
}
