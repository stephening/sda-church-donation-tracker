using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for PrintOptionsView.xaml
/// </summary>
public partial class PrintOptionsView : UserControl
{
	/// <summary>
	/// This printing, from what I understand uses 96 dpi. I don't know if the user can change that
	/// from the print dialog.
	/// </summary>
	public const double _dpi = 96.0;

	/// <summary>
	/// This is populated in the class constructor from the Fonts.SystemFontFamilies collection.
	/// The reason I did this rather than direcly populating the control is that when i did that
	/// I couldn't select by setting the bound SelectedItem.
	/// </summary>
	public ObservableCollection<string> FontList { get; set; } = new ObservableCollection<string>();

	public double PageWidth
	{
		get { return (double)GetValue(PageWidthProperty); }
		set { SetValue(PageWidthProperty, value); }
	}

	// Using a DependencyProperty as the backing store for LeftMargin.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty PageWidthProperty =
		DependencyProperty.Register("PageWidth", typeof(double), typeof(PrintOptionsView), new PropertyMetadata(8.5));

	public double PageHeight
	{
		get { return (double)GetValue(PageHeightProperty); }
		set { SetValue(PageHeightProperty, value); }
	}

	// Using a DependencyProperty as the backing store for LeftMargin.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty PageHeightProperty =
		DependencyProperty.Register("PageHeight", typeof(double), typeof(PrintOptionsView), new PropertyMetadata(11.0));

	public double LeftMargin
	{
		get { return (double)GetValue(LeftMarginProperty); }
		set { SetValue(LeftMarginProperty, value); }
	}

	// Using a DependencyProperty as the backing store for LeftMargin.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty LeftMarginProperty =
		DependencyProperty.Register("LeftMargin", typeof(double), typeof(PrintOptionsView), new PropertyMetadata(0.8));

	public double OtherMargins
	{
		get { return (double)GetValue(OtherMarginsProperty); }
		set { SetValue(OtherMarginsProperty, value); }
	}

	// Using a DependencyProperty as the backing store for OtherMargins.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty OtherMarginsProperty =
		DependencyProperty.Register("OtherMargins", typeof(double), typeof(PrintOptionsView), new PropertyMetadata(0.5));

	public string SelectedFont
	{
		get { return (string)GetValue(SelectedFontProperty); }
		set { SetValue(SelectedFontProperty, value); }
	}

	// Using a DependencyProperty as the backing store for SelectedFont.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty SelectedFontProperty =
		DependencyProperty.Register("SelectedFont", typeof(string), typeof(PrintOptionsView), new PropertyMetadata("Calibri"));

	public double SelectedSize
	{
		get { return (double)GetValue(SelectedSizeProperty); }
		set { SetValue(SelectedSizeProperty, value); }
	}

	// Using a DependencyProperty as the backing store for SelectedSize.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty SelectedSizeProperty =
		DependencyProperty.Register("SelectedSize", typeof(double), typeof(PrintOptionsView), new PropertyMetadata(14.0));

	/// <summary>
	/// The FontSize property is a simple collection of some of the most popular font sizes for
	/// this particular report printout. If other font sizes are needed, they can simply be typeed
	/// because the ComboBox has the edit capability enabled.
	/// </summary>
	public ObservableCollection<double> FontSizes { get; set; } = new ObservableCollection<double>() {
					3.0, 4.0, 5.0, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5,
					10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0, 13.5, 14.0, 15.0,
					16.0, 17.0, 18.0, 19.0, 20.0, 22.0, 24.0, 26.0, 28.0, 30.0,
					32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0,
					80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
					};

	public PrintOptionsView()
	{
		InitializeComponent();

		foreach (var font in Fonts.SystemFontFamilies)
		{
			FontList.Add(font.Source);
		}
	}
}
