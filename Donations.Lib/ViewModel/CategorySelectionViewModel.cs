using CommunityToolkit.Mvvm.ComponentModel;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System.Windows.Data;

namespace Donations.Lib.ViewModel;

/// <summary>
/// This view model handles the functionality and the binding to the CategorySelectionView.xaml which
/// is a Window, which will be used as a modal dialog whenever the operator wishes to change a
/// category. The window contains a text filter box and a list, where the list is filtered by the
/// text in the filter. The filter can be either code or description but not both.
/// </summary>
public partial class CategorySelectionViewModel : BaseViewModel
{
	public CollectionViewSource ViewSource { get; set; } = new CollectionViewSource();

	/// <summary>
	/// The FilterText property is bound to the TextBox where the filter is entered.
	/// </summary>
	public string FilterText { get; set; } = "";

	/// <summary>
	/// The OKEnabled prperty controls whether the OK button is enabled or not. The
	/// only time the OK button is disabled is if the filter yields not results, and
	/// the list is empty.
	/// </summary>
	public bool OKEnabled => SelectedCategoryIndex != -1;

	private readonly ICategoryServices _categoryServices;

	[ObservableProperty]
	private int _selectedCategoryIndex = 0;
	/// <summary>
	/// The SelectedCategoryIndex property is bound to the SelectedIndex property of
	/// the DataGrid. This allows the Click handler in CategorySelectionView.xaml.cs
	/// to obtain the selected category.
	/// </summary>

	/// <summary>
	/// The constructor sets the CollectionViewSource's Source, and Filter properties.
	/// </summary>
	public CategorySelectionViewModel(
		ICategoryServices categoryServices
	)
	{
		_categoryServices = categoryServices;

		ViewSource.Source = _categoryServices.CatList;
		ViewSource.Filter += new FilterEventHandler(Filter);
	}

	/// <summary>
	/// The filter method uses the same FilterText, looking for matches in either the Code,
	/// or the Description columns.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Filter(object sender, FilterEventArgs e)
	{
		var obj = e.Item as Category;
		if (obj != null)
		{
			if (obj.Code.ToString().Contains(FilterText, System.StringComparison.OrdinalIgnoreCase)
				|| obj.Description.Contains(FilterText, System.StringComparison.OrdinalIgnoreCase))
				e.Accepted = true;
			else
				e.Accepted = false;
		}
	}

	/// <summary>
	/// The TextChanged method is called from the CategorySelectionView.xaml.cs when the
	/// FilterText's TextChanged event is fired. This allows for realtime feedback on
	/// whether the filter text is any good or not.
	/// </summary>
	public void TextChanged()
	{
		ViewSource.View.Refresh();
		if (!ViewSource.View.IsEmpty)
		{
			SelectedCategoryIndex = 0;
		}
		OnPropertyChanged(nameof(OKEnabled));
	}
}
