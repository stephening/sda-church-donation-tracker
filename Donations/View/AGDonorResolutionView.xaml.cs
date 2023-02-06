using Donations.Model;
using Donations.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for AGDonorResolutionView.xaml
	/// </summary>
	public partial class AGDonorResolutionView : UserControl
	{
		private AGDonorResolutionViewModel? _viewModel;

		public AGDonorResolutionView()
		{
			InitializeComponent();

			_viewModel = DataContext as AGDonorResolutionViewModel;
		}

		public void ChooseDonor(object sender, RoutedEventArgs e)
		{
			DonorSelectionView dlg = new DonorSelectionView();
			int? id = _viewModel?._lastNameMatchId;

			if (null != id)
			{
				dlg.LastNameFilterText.Text = (null != di.Data.DonorDict && di.Data.DonorDict.ContainsKey(id.Value)) ? di.Data.DonorDict[id.Value].LastName : "";
			}

			if (true == dlg.ShowDialog())
			{
				_viewModel?.ChooseDonor((Donor)dlg.DonorGrid.SelectedItem);
			}
		}
	}
}
