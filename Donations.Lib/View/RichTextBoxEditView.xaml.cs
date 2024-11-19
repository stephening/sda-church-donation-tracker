using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for RichTextBoxEditView.xaml
/// https://brianlagunas.com/create-your-own-format-bar-for-the-wpf-richtextbox/
/// </summary>
public partial class RichTextBoxEditView : UserControl
{
	public RichTextBoxEditView()
	{
		InitializeComponent();

		foreach (var font in Fonts.SystemFontFamilies)
		{
			FontList.Add(font.Source);
		}
	}



	public double PageWidth
	{
		get { return (double)GetValue(PageWidthProperty); }
		set { SetValue(PageWidthProperty, value); }
	}

	// Using a DependencyProperty as the backing store for PageWidth.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty PageWidthProperty =
		DependencyProperty.Register("PageWidth", typeof(double), typeof(RichTextBoxEditView), new PropertyMetadata(OnPageSizeOrMarginsChanged));

	public double PageHeight
	{
		get { return (double)GetValue(PageHeightProperty); }
		set { SetValue(PageHeightProperty, value); }
	}

	// Using a DependencyProperty as the backing store for PageHeight.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty PageHeightProperty =
		DependencyProperty.Register("PageHeight", typeof(double), typeof(RichTextBoxEditView), new PropertyMetadata(OnPageSizeOrMarginsChanged));

	public double LeftMargin
	{
		get { return (double)GetValue(LeftMarginProperty); }
		set { SetValue(LeftMarginProperty, value); }
	}

	// Using a DependencyProperty as the backing store for LeftMargin.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty LeftMarginProperty =
		DependencyProperty.Register("LeftMargin", typeof(double), typeof(RichTextBoxEditView), new PropertyMetadata(OnPageSizeOrMarginsChanged));

	public double OtherMargins
	{
		get { return (double)GetValue(OtherMarginsProperty); }
		set { SetValue(OtherMarginsProperty, value); }
	}

	// Using a DependencyProperty as the backing store for OtherMargins.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty OtherMarginsProperty =
		DependencyProperty.Register("OtherMargins", typeof(double), typeof(RichTextBoxEditView), new PropertyMetadata(OnPageSizeOrMarginsChanged));

	private static void OnPageSizeOrMarginsChanged(DependencyObject source,
		DependencyPropertyChangedEventArgs e)
	{
		RichTextBoxEditView? richTextBoxEditView = source as RichTextBoxEditView;

		if (null != richTextBoxEditView)
		{
			richTextBoxEditView._richTextBox.Width = PrintOptionsView._dpi * (richTextBoxEditView.PageWidth - richTextBoxEditView.LeftMargin - richTextBoxEditView.OtherMargins);
		}
	}

	public RichTextBoxContainer RtbContainer
	{
		get { return (RichTextBoxContainer)GetValue(RichTextBoxProperty); }
		set { SetValue(RichTextBoxProperty, value); }
	}

	// Using a DependencyProperty as the backing store for RichTextBox.  This enables animation, styling, binding, etc...
	public static readonly DependencyProperty RichTextBoxProperty =
		DependencyProperty.Register("RtbContainer", typeof(RichTextBoxContainer), typeof(RichTextBoxEditView), new PropertyMetadata(OnRtbContainerPropertyChanged));

	private static void OnRtbContainerPropertyChanged(DependencyObject source,
		DependencyPropertyChangedEventArgs e)
	{
		RichTextBoxEditView? richTextBoxEditView = source as RichTextBoxEditView;

		if (null != richTextBoxEditView)
		{
			richTextBoxEditView.RtbContainer.RichTextBox = richTextBoxEditView._richTextBox;
		}
	}

	public ICommand RichTextChanged
	{
		get { return (ICommand)GetValue(RichTextChangedProperty); }
		set { SetValue(RichTextChangedProperty, value); }
	}

	public static readonly DependencyProperty RichTextChangedProperty = DependencyProperty.Register(
		"RichTextChanged",
		typeof(ICommand),
		typeof(RichTextBoxEditView));

	private void _richTextBox_TextChanged(object sender, TextChangedEventArgs e)
	{
		if (true == RichTextChanged?.CanExecute(null))
			RichTextChanged?.Execute(null);
	}

	public ObservableCollection<string> FontList { get; set; } = new ObservableCollection<string>();

	public double[] FontSizes
	{
		get
		{
			return new double[] {
					3.0, 4.0, 5.0, 6.0, 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5,
					10.0, 10.5, 11.0, 11.5, 12.0, 12.5, 13.0, 13.5, 14.0, 15.0,
					16.0, 17.0, 18.0, 19.0, 20.0, 22.0, 24.0, 26.0, 28.0, 30.0,
					32.0, 34.0, 36.0, 38.0, 40.0, 44.0, 48.0, 52.0, 56.0, 60.0, 64.0, 68.0, 72.0, 76.0,
					80.0, 88.0, 96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
					};
		}
	}

	private void _fontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			if (0 < e.AddedItems.Count)
			{
				FontFamily editValue = new FontFamily(e.AddedItems[0].ToString());
				ApplyPropertyValueToSelectedText(TextElement.FontFamilyProperty, editValue);
			}
		}
		catch (Exception ex)
		{

		}
	}

	private void _fontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			if (0 < e.AddedItems.Count)
			{
				ApplyPropertyValueToSelectedText(TextElement.FontSizeProperty, e.AddedItems[0]);
			}
		}
		catch (Exception ex)
		{

		}
	}

	#region Methods

	private void UpdateVisualState()
	{
		UpdateToggleButtonState();
		UpdateSelectionListType();
		UpdateSelectedFontFamily();
		UpdateSelectedFontSize();
		_richTextBox.UpdateLayout();
	}

	private void UpdateToggleButtonState()
	{
		UpdateItemCheckedState(_btnBold, TextElement.FontWeightProperty, FontWeights.Bold);
		UpdateItemCheckedState(_btnItalic, TextElement.FontStyleProperty, FontStyles.Italic);
		UpdateDecorationsItemCheckedState(_btnUnderline, Inline.TextDecorationsProperty, TextDecorations.Underline);

		UpdateItemCheckedState(_btnAlignLeft, Paragraph.TextAlignmentProperty, TextAlignment.Left);
		UpdateItemCheckedState(_btnAlignCenter, Paragraph.TextAlignmentProperty, TextAlignment.Center);
		UpdateItemCheckedState(_btnAlignRight, Paragraph.TextAlignmentProperty, TextAlignment.Right);
		UpdateItemCheckedState(_btnAlignJustify, Paragraph.TextAlignmentProperty, TextAlignment.Right);
	}

	void UpdateItemCheckedState(ToggleButton button, DependencyProperty formattingProperty, object expectedValue)
	{
		object currentValue = _richTextBox.Selection.GetPropertyValue(formattingProperty);
		button.IsChecked = (currentValue == DependencyProperty.UnsetValue) ? false : currentValue != null && currentValue.Equals(expectedValue);
	}

	void UpdateDecorationsItemCheckedState(ToggleButton button, DependencyProperty formattingProperty, object expectedValue)
	{
		object currentValue = _richTextBox.Selection.GetPropertyValue(formattingProperty);
		var expected = expectedValue as TextDecorationCollection;
		var collection = currentValue as TextDecorationCollection;
		button.IsChecked = false;
		if (null != collection && null != expected)
        {
			foreach (TextDecoration expectedDecoration in expected)
			{
				foreach (TextDecoration textDecoration in collection)
				{
					if (expectedDecoration == textDecoration)
					{
						button.IsChecked = true;
						return;
					}
				}
			}
		}
	}

	private void UpdateSelectionListType()
	{
		Paragraph startParagraph = _richTextBox.Selection.Start.Paragraph;
		Paragraph endParagraph = _richTextBox.Selection.End.Paragraph;
		if (startParagraph != null && endParagraph != null && (startParagraph.Parent is ListItem) && (endParagraph.Parent is ListItem) && object.ReferenceEquals(((ListItem)startParagraph.Parent).List, ((ListItem)endParagraph.Parent).List))
		{
			TextMarkerStyle markerStyle = ((ListItem)startParagraph.Parent).List.MarkerStyle;
			if (markerStyle == TextMarkerStyle.Disc) //bullets
			{
				_btnBullets.IsChecked = true;
			}
			else if (markerStyle == TextMarkerStyle.Decimal) //numbers
			{
				_btnNumbers.IsChecked = true;
			}
		}
		else
		{
			_btnBullets.IsChecked = false;
			_btnNumbers.IsChecked = false;
		}
	}

	private void UpdateSelectedFontFamily()
	{
		object value = _richTextBox.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
		FontFamily? currentFontFamily = (FontFamily?)((value == DependencyProperty.UnsetValue) ? null : value);
		if (currentFontFamily != null)
		{
			_fontFamily.SelectedItem = currentFontFamily.ToString();
		}
	}

	private void UpdateSelectedFontSize()
	{
		object value = _richTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty);
		_fontSize.SelectedValue = (value == DependencyProperty.UnsetValue) ? null : value;
	}


	void ApplyPropertyValueToSelectedText(DependencyProperty formattingProperty, object value)
	{
		if (value == null)
			return;

		_richTextBox.Selection.ApplyPropertyValue(formattingProperty, value);
	}

	#endregion

	private void _richTextBox_SelectionChanged(object sender, RoutedEventArgs e)
	{
		var img = (_richTextBox.Selection?.Start?.Parent as BlockUIContainer)?.Child as Image;
		if (null != img)
		{
			double scaleFactor = PrintOptionsView._dpi * (PageWidth - LeftMargin - OtherMargins - 0.25) / img.Width;
			if (1 > scaleFactor)
			{
				img.Width *= scaleFactor;
				img.Height *= scaleFactor;
			}
		}
		else
		{
			UpdateVisualState();
		}
	}

	private void InsertImage(object sender, RoutedEventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog()
		{
			Filter = "Image files (*.png;*.jpg)|*.png;*.jpg"
		};

		if (true == openFileDialog.ShowDialog())
		{
			Image image = new Image();
			var imgsrc = new BitmapImage(new Uri(openFileDialog.FileName));
			image.Source = imgsrc;
			var save = Clipboard.GetDataObject();
			Clipboard.SetImage(imgsrc);
			_richTextBox.Paste();
			Clipboard.SetDataObject(save);
		}
	}
}
