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
	public ObservableCollection<double> FontSizes { get; set; } = new ObservableCollection<double>() { 8, 9, 10, 11, 12, 13, 14, 15, 16 };

	public PrintOptionsView()
	{
		InitializeComponent();

		foreach (var font in Fonts.SystemFontFamilies)
		{
			FontList.Add(font.Source);
		}
	}
}
