using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Donations.Lib.View
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
		}

		public void ChooseDonor(object sender, RoutedEventArgs e)
		{
			DonorSelectionView dlg = DependencyInjection.DonorSelectionView;
			int? id = _viewModel?._lastNameMatchId;

			if (null != id)
			{
				dlg.LastNameFilterText.Text = _viewModel?.GetDonorById(id.Value)?.LastName;
			}

			if (true == dlg.ShowDialog())
			{
				_viewModel?.ChooseDonor((Donor)dlg.DonorGrid.SelectedItem);
			}
		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as AGDonorResolutionViewModel;
		}
	}
}
