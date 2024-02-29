using Donations.Lib.Model;
using Donations.Lib.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.Lib.View
{
	/// <summary>
	/// Interaction logic for DonorView.xaml
	/// </summary>
	public partial class DonorView : UserControl
	{
		private DonorViewModel? _viewModel;
		private readonly ConfirmDonorMergeView.Factory _confirmDonorMergeViewFactory;

		public delegate DonorView Factory();

		public DonorView()
		{
			InitializeComponent();

			_confirmDonorMergeViewFactory = DependencyInjection.Resolve<ConfirmDonorMergeView.Factory>();
		}

		private async void ChooseDonor_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				if (true == _viewModel?.UpdateEnabled)
				{
					if (MessageBoxResult.No == MessageBox.Show("A donor is already selected, do you with to choose another without updating changes to the current donor?", "Select donor", MessageBoxButton.YesNo, MessageBoxImage.Question))
					{
						return;
					}
				}
				DonorSelectionView dlg = DependencyInjection.DonorSelectionView;
				if (true == dlg.ShowDialog())
				{
					var SelectedName = (Donor)dlg.DonorGrid.SelectedItem;
					if (SelectedName != null)
						await _viewModel?.ChooseDonor(SelectedName.Id);
					else
						MessageBox.Show("Please reselect the donor", "Selection failed", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			catch { }
		}

		private async void ChooseRelated_Click(object sender, RoutedEventArgs e)
		{
			DonorSelectionView dlg = DependencyInjection.DonorSelectionView;
			if (true == dlg.ShowDialog())
			{
				Donor SelectedName = (Donor)dlg.DonorGrid.SelectedItem;
				var ret = await _viewModel?.ChooseRelated(SelectedName.Id);
				if (ret != null && null != SelectedName)
				{
					var ans = MessageBox.Show(ret, "Start new family", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
					if (ans == MessageBoxResult.Yes)
					{
						await _viewModel?.ChooseRelated(SelectedName.Id, true);
					}
				}
			}
		}

		private void FamilyMember_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			DataGridRow row = (DataGridRow)sender;
			Donor? familymember = (Donor?)row?.DataContext;

			DonorModalView dlg = _viewModel.Create(familymember, _viewModel.DonationsVisibility == Visibility.Visible);

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

		private async void UpdateDonor_Click(object sender, RoutedEventArgs e)
		{
			string? ret = await _viewModel?.UpdateDonor();
			if (!string.IsNullOrEmpty(ret))
			{
				MessageBox.Show(ret, "Donor field conflict", MessageBoxButton.OK, MessageBoxImage.Exclamation);
			}
		}

		private async void AddDonor_Click(object sender, RoutedEventArgs e)
		{
			string? ret = await _viewModel?.AddDonor();
			if (!string.IsNullOrEmpty(ret))
			{
				var res = MessageBox.Show(ret, "Donor may already exist in the database.",
					MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

				if (res == MessageBoxResult.Yes)
				{
					await _viewModel?.AddDonor(true);
				}
			}
		}

		private async void SelectDonorToMerge_Click(object sender, RoutedEventArgs e)
		{
			var ret = MessageBox.Show("Merging one donor record into another will not only eliminate one donor record, but it will also assign all donations from the donor being eliminated to the one being kept.\n\nIf you continue, the current donor record will be kept, and the next donor selected will be merged into the current and then deleted.\n\nAre you sure you want to continue.", "Confirm merge", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
			if (MessageBoxResult.Yes == ret)
			{
				DonorSelectionView dlg = DependencyInjection.DonorSelectionView;
				if (true == dlg.ShowDialog())
				{
					var mergeFrom = (Donor)dlg.DonorGrid.SelectedItem;
					if (mergeFrom != null)
					{
						var collection = await _viewModel?.MergeDonor(mergeFrom);
#pragma warning disable CS8604 // Possible null reference argument.
						ConfirmDonorMergeView view = _confirmDonorMergeViewFactory(_viewModel?.SelectedDonor, mergeFrom, collection);
#pragma warning restore CS8604 // Possible null reference argument.
						bool? resp = view.ShowDialog();
						if (true == resp)
						{
							await _viewModel.MergeDonor(mergeFrom, true);
						}
					}
					else
						MessageBox.Show("Please reselect the donor", "Selection failed", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private async void DeleteDonor_Click(object sender, RoutedEventArgs e)
		{
			var ret = MessageBox.Show("Are you sure you want to delete the currently selected donor?", "Confirm delete", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
			if (MessageBoxResult.Yes == ret)
			{
				string? ret2 = await _viewModel?.DeleteDonor();
				if (!string.IsNullOrEmpty(ret2))
				{
					var res = MessageBox.Show(ret2, "Confirm delete", MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.No);

					if (res == MessageBoxResult.Yes)
					{
						await _viewModel?.DeleteDonor(true);
					}
				}
			}
		}

		private void RefreshPicture_Click(object sender, RoutedEventArgs e)
		{
			_viewModel?.RefreshPicture();
		}

		private void PictureFile_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				e.Handled = true;
				((TextBox)sender).GetBindingExpression(TextBox.TextProperty).UpdateSource();
				// simulate refresh button press
				RefreshPicture_Click(sender, e);
			}
		}

		private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			_viewModel = DataContext as DonorViewModel;
		}
	}
}
