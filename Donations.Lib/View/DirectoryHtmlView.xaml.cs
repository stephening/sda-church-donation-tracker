using System.Windows;
using System.Windows.Controls;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for DirectoryHtmlView.xaml
/// </summary>
public partial class DirectoryHtmlView : UserControl
{
	public DirectoryHtmlView()
	{
		InitializeComponent();
	}

	private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
	{

	}

	private void MergeFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		dynamic selection = MergeFields.SelectedItem;

		var save = Clipboard.GetDataObject();
		Clipboard.SetText($"{{{selection.Value}}}");
		MergableTemplate.Paste();
		Clipboard.SetDataObject(save);
    }
}
