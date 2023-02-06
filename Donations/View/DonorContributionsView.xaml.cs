using Donations.Model;
using Donations.ViewModel;
using System.Windows;

namespace Donations.View
{
	/// <summary>
	/// Interaction logic for DonorContributions.xaml
	/// </summary>
	public partial class DonorContributionsView : Window
	{
		private DonorContributionsViewModel? _viewModel;

		public DonorContributionsView()
		{
			InitializeComponent();

			_viewModel = DataContext as DonorContributionsViewModel;
		}

		public bool? ShowDialog(Batch? batch, CategorySum? categorySum)
		{
			_viewModel?.Show(batch, categorySum);

			return ShowDialog();
		}
	}
}
