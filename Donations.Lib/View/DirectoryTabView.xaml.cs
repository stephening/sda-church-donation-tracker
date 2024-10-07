using Donations.Lib.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for DirectoryTabView.xaml
/// </summary>
public partial class DirectoryTabView : UserControl
{
	private DirectoryViewModel? _viewModel;

	public DirectoryTabView(
	)
	{
		InitializeComponent();
	}


	private async void DirectoryPdfTab_Selected(object sender, RoutedEventArgs e)
	{
		await _viewModel?.DirectoryPdfViewModel?.Loading();
	}

	private async void DirectoryHtmlTab_Selected(object sender, RoutedEventArgs e)
	{
	}

	private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
	{
		_viewModel = DataContext as DirectoryViewModel;
	}
}
