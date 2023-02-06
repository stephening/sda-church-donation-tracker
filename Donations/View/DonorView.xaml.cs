using Donations.Model;
using Donations.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for DonorView.xaml
	/// </summary>
	public partial class DonorView : UserControl
	{
		private DonorViewModel? _viewModel;

		public DonorView()
		{
			InitializeComponent();

			_viewModel = DataContext as DonorViewModel;
		}

		private void ChooseDonor_Click(object sender, RoutedEventArgs e)
		{
			DonorSelectionView dlg = new DonorSelectionView();
			if (true == dlg.ShowDialog())
			{
				var SelectedName = (Donor)dlg.DonorGrid.SelectedItem;
				if (SelectedName != null)
					_viewModel?.ChooseDonor(SelectedName.Id);
				else
					MessageBox.Show("Please reselect the donor", "Selection failed", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private void ChooseRelated_Click(object sender, RoutedEventArgs e)
		{
			DonorSelectionView dlg = new DonorSelectionView();
			if (true == dlg.ShowDialog())
			{
				Donor SelectedName = (Donor)dlg.DonorGrid.SelectedItem;
				var ret = _viewModel?.ChooseRelated(SelectedName.Id);
				if (ret != null && null != SelectedName)
				{
					var ans = MessageBox.Show(ret, "Start new family", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
					if (ans != MessageBoxResult.Yes)
					{
						_viewModel?.ChooseRelated(SelectedName.Id, true);
					}
				}
			}
		}

		private void FamilyMember_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = (DataGridRow)sender;
			Donor? familymember = (Donor?)row?.DataContext;

			DonorModalView dlg = new DonorModalView(familymember);

			dlg.ShowDialog();
		}

		private void FamilyRelationship_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (true == _viewModel?.FamilyRelationshipConflict(FamilyRelationship.SelectedItem?.ToString()))
			{
				MessageBox.Show("Only one family member can be primary", "Relationship conflict", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				FamilyRelationship.SelectedItem = e.RemovedItems[0];
			}
		}

		private void UpdateDonor_Click(object sender, RoutedEventArgs e)
		{
			string? ret = _viewModel?.UpdateDonor();
			if (!string.IsNullOrEmpty(ret))
			{
				MessageBox.Show(ret, "Donor field conflict", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		private void AddDonor_Click(object sender, RoutedEventArgs e)
		{
			string? ret = _viewModel?.AddDonor();
			if (!string.IsNullOrEmpty(ret))
			{
				var res = MessageBox.Show(ret, "Donor may already exist in the database.",
					MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

				if (res == MessageBoxResult.Yes)
				{
					_viewModel?.AddDonor(true);
				}
			}
		}
	}
}
