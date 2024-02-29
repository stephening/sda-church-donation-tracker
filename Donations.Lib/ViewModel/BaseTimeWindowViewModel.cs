using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Donations.Lib.ViewModel;

public abstract partial class BaseTimeWindowViewModel : BaseViewModel
{
	// These privatge members variables will be used for the 'Current year' and 'Previous year' radio
	// button options.
	protected readonly string _thisYear = DateTime.Now.Year.ToString();
	protected readonly string _prevYear = (DateTime.Now.Year - 1).ToString();
	private readonly IDispatcherWrapper _dispatcherWrapper;
	private DispatcherTimer _debounceTimer = new DispatcherTimer();

	public BaseTimeWindowViewModel()
	{
		_dispatcherWrapper = DependencyInjection.Resolve<IDispatcherWrapper>();
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
		_debounceTimer.Tick += new EventHandler(Debounce_Handler);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
		_debounceTimer.Interval = new TimeSpan(0, 0, 1);
	}

	[ObservableProperty]
	private bool _selectionEnabled = true;

	[ObservableProperty]
	private ObservableCollection<string> _availableYears = new ObservableCollection<string>();

	[ObservableProperty]
	private string? _filterYear = "";

	[ObservableProperty]
	private enumDateFilterOptions _dateFilterOption = enumDateFilterOptions.CurrentYear;

	[ObservableProperty]
	private double _sum = 0;

	[ObservableProperty]
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
	private string _selectedCategory = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

	[ObservableProperty]
	private bool _printPreviewEnabled = false;

	private DateOnly _filterStartDate;
	/// <summary>
	/// The FilterStartDate property is bound to the starting date range field.
	/// </summary>
	public string FilterStartDate
	{
		get
		{
			string str = _filterStartDate.ToString("yyyy/MM/dd");
			return str.Equals("0001/01/01") ? "" : str;
		}
		set
		{
			try
			{
				if (string.IsNullOrEmpty(value))
					_filterStartDate = DateOnly.MinValue;
				else
					_filterStartDate = DateOnly.Parse(value);
				OnPropertyChanged();
			}
			catch
			{
				_filterStartDate = DateOnly.MinValue;
			}
		}
	}

	private DateOnly _filterEndDate;
	/// <summary>
	/// The FilterEndDate property is bound to the ending date range field.
	/// </summary>
	public string FilterEndDate
	{
		get
		{
			string str = _filterEndDate.ToString("yyyy/MM/dd");
			return str.Equals("0001/01/01") ? "" : str;
		}
		set
		{
			try
			{
				if (string.IsNullOrEmpty(value))
					_filterEndDate = DateOnly.MinValue;
				else
					_filterEndDate = DateOnly.Parse(value);
				OnPropertyChanged();
			}
			catch
			{
				_filterEndDate = DateOnly.MinValue;
			}
		}
	}


	[RelayCommand]
	protected void InternalTimeWindowChanged()
	{
		// start stop/start 1 sec timer and the last start will be handled in 1 sec
		_debounceTimer.Stop();
		_debounceTimer.Start();
	}

	private void Debounce_Handler(object sender, EventArgs e)
	{
		_debounceTimer.Stop();
		SelectionEnabled = false;
		_dispatcherWrapper.BeginInvoke(TimeWindowChanged);
	}

	public abstract Task TimeWindowChanged();
}
