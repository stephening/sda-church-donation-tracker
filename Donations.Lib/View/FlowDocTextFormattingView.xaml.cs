using Donations.Lib.Extensions;
using Donations.Lib.View;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Donations.Lib.View;

/// <summary>
/// Interaction logic for FlowDocTextFormattingView.xaml
/// https://brianlagunas.com/create-your-own-format-bar-for-the-wpf-richtextbox/
/// </summary>
public partial class FlowDocTextFormattingView : UserControl
{
	public FlowDocTextFormattingView()
	{
		InitializeComponent();

		foreach (var font in Fonts.SystemFontFamilies)
		{
			FontList.Add(font.Source);
		}

		RTB = _richTextBox;
		DataContext = this;
	}

	public RichTextBox RTB
	{
		get { return (RichTextBox) GetValue(RTBProperty); }
		set { SetValue(RTBProperty, value); }
	}

	public static readonly DependencyProperty RTBProperty =
		DependencyProperty.Register("RTB", typeof(RichTextBox), typeof(FlowDocTextFormattingView), new PropertyMetadata());

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
			double scaleFactor = 720 / img.Width;
			img.Width *= scaleFactor;
			img.Height *= scaleFactor;
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
			var imgsrc = new BitmapImage();
			imgsrc.BeginInit();
			imgsrc.StreamSource = File.Open(openFileDialog.FileName, FileMode.Open);
			imgsrc.EndInit();
			image.Source = imgsrc;
			var save = Clipboard.GetDataObject();
			Clipboard.SetImage(imgsrc);
			Clipboard.SetDataObject(save);
			_richTextBox.Paste();
		}
	}
}
