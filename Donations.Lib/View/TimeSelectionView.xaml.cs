using Donations.Lib.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for TimeSelectionView.xaml
/// </summary>
public partial class TimeSelectionView : UserControl, INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
	public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	public TimeSelectionView()
	{
		InitializeComponent();
	}

	public string StartDate
	{
		get { return (string)GetValue(StartDateProperty); }
		set { SetValue(StartDateProperty, value); }
	}

	// Using a DependencyProperty as the backing store for StartDate.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty StartDateProperty =
		DependencyProperty.Register("StartDate", typeof(string), typeof(TimeSelectionView), new PropertyMetadata(""));

	public string EndDate
	{
		get { return (string)GetValue(EndDateProperty); }
		set { SetValue(EndDateProperty, value); }
	}

	// Using a DependencyProperty as the backing store for EndDate.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty EndDateProperty =
		DependencyProperty.Register("EndDate", typeof(string), typeof(TimeSelectionView), new PropertyMetadata(""));

	public string FilterYear
	{
		get { return (string)GetValue(FilterYearProperty); }
		set
		{
			SetValue(FilterYearProperty, value);
			TimeSelectionChanged?.Execute(this);
		}
	}

	// Using a DependencyProperty as the backing store for FilterYear.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty FilterYearProperty =
		DependencyProperty.Register("FilterYear", typeof(string), typeof(TimeSelectionView), new PropertyMetadata(""));


	public ObservableCollection<string> AvailableYears
	{
		get { return (ObservableCollection<string>)GetValue(AvailableYearsProperty); }
		set { SetValue(AvailableYearsProperty, value); }
	}

	// Using a DependencyProperty as the backing store for AvailableYears.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty AvailableYearsProperty =
		DependencyProperty.Register("AvailableYears", typeof(ObservableCollection<string>), typeof(TimeSelectionView), new PropertyMetadata(null));


	public enumDateFilterOptions FilterOption
	{
		get { return (enumDateFilterOptions)GetValue(FilterOptionProperty); }
		set { SetValue(FilterOptionProperty, value); }
	}

	// Using a DependencyProperty as the backing store for FilterOption.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty FilterOptionProperty =
		DependencyProperty.Register("FilterOption", typeof(enumDateFilterOptions), typeof(TimeSelectionView), new PropertyMetadata(enumDateFilterOptions.CurrentYear, FilterOptionChanged));

	private static void FilterOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		TimeSelectionView view = (TimeSelectionView)d;

		view.OnPropertyChanged(nameof(SelectYearComboBoxEnabled));
		view.OnPropertyChanged(nameof(DateRangeEnabled));
		view.TimeSelectionChanged?.Execute(null);
	}

	public ICommand TimeSelectionChanged
	{
		get { return (ICommand)GetValue(TimeSelectionChangedProperty); }
		set { SetValue(TimeSelectionChangedProperty, value); }
	}

	public static readonly DependencyProperty TimeSelectionChangedProperty =
		DependencyProperty.Register("TimeSelectionChanged",
			typeof(ICommand),
			typeof(TimeSelectionView),
			new PropertyMetadata(null));

	public bool SelectionEnabled
	{
		get { return (bool)GetValue(SelectionEnabledProperty); }
		set { SetValue(SelectionEnabledProperty, value); }
	}

	// Using a DependencyProperty as the backing store for SelectionEnabled.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty SelectionEnabledProperty =
		DependencyProperty.Register("SelectionEnabled", typeof(bool), typeof(TimeSelectionView), new PropertyMetadata(true));

	/// <summary>
	/// The SelectYearComboBoxEnabled property allows the ComboBox to be enable/disable depending on
	/// which radio button is selected.
	/// </summary>
	public bool SelectYearComboBoxEnabled => FilterOption == enumDateFilterOptions.SelectYear;
	/// <summary>
	/// The DateRangeEnabled property allows the Date pickers to be enable/disable depending on
	/// which radio button is selected.
	/// </summary>
	public bool DateRangeEnabled => FilterOption == enumDateFilterOptions.DateRange;

	private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		TimeSelectionChanged?.Execute(this);
	}

	private void DatePicker_SelectedStartDateChanged(object sender, SelectionChangedEventArgs e)
	{
		TimeSelectionChanged?.Execute(this);
	}

	private void DatePicker_SelectedEndDateChanged(object sender, SelectionChangedEventArgs e)
	{
		TimeSelectionChanged?.Execute(this);
	}
}
