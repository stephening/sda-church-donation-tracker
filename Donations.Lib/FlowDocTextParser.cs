using Donations.Lib.Model;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using BlockCollection = System.Windows.Documents.BlockCollection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Donations.Lib;

public partial class FlowDocTextParser : ObservableObject
{
	private readonly Regex _imageRePat = new Regex($"{{{enumPdfCover.Image}([ ]+[A-za-z0-9=]+)?([ ]+[A-za-z0-9=]+)?[ ]*}}");
	private readonly Regex _textRePat = new Regex(@"({\w+?})?({(\w+)=([+-]?\w+?)})?({/\w+?})?");
	private Dictionary<enumPdfCover, List<object?>> _formatMap = new Dictionary<enumPdfCover, List<object?>>();

    public FlowDocTextParser(
		string? selectedFont,
		double fontSize,
		string coverImage
	)
    {
        SelectedFont = selectedFont;
		SelectedSize = fontSize;
		SelectedImage = coverImage;
    }

    [ObservableProperty]
	private string? _selectedFont;
	/// <summary>
	/// The SelectedFont prperty is used to initially select the last font used, and also to
	/// receive the latest font chosen by the operator.
	/// </summary>
	partial void OnSelectedFontChanged(string? value)
	{
		if (_formatMap.ContainsKey(enumPdfCover.Font))
		{
			_formatMap[enumPdfCover.Font][0] = value;
		}
	}

	[ObservableProperty]
	private double _selectedSize;
	/// <summary>
	/// The SelectedSize prperty is used to initially select the last font size used, and also to
	/// receive the latest font size chosen by the operator.
	/// </summary>
	partial void OnSelectedSizeChanged(double value)
	{
		if (_formatMap.ContainsKey(enumPdfCover.FontSize))
		{
			_formatMap[enumPdfCover.FontSize][0] = value.ToString();
		}
	}

	[ObservableProperty]
	private string? _selectedImage;

	public void Parse(BlockCollection blocks, string text)
	{
		string? lines = null;
		var split = text.Split("\r\n");
		_formatMap.Clear();
		foreach (enumPdfCover format in Enum.GetValues(typeof(enumPdfCover)))
		{
			_formatMap[format] = new List<object?>();
		}
		_formatMap[enumPdfCover.Font].Add(SelectedFont);
		_formatMap[enumPdfCover.FontSize].Add(SelectedSize);

		foreach (var line in split)
		{
			if (line.Contains($"{{{enumPdfCover.Image}", StringComparison.OrdinalIgnoreCase) && null != SelectedImage)
			{
				double width = 0;
				double height = 0;

				var res = _imageRePat.Match(line);
				if (null != res && res.Success && 1 <= res.Groups.Count)
				{
					for (int i = 1; i < res.Groups.Count; i++)
					{
						if (!string.IsNullOrEmpty(res.Groups[i].Value) && res.Groups[i].Value.Contains('='))
						{
							var splt = res.Groups[i].Value.Trim().Split("=");
							if (2 == splt.Length)
							{
								if (splt[0].Equals("WIDTH", StringComparison.OrdinalIgnoreCase))
								{
									width = double.Parse(splt[1]);
								}
								else if (splt[0].Equals("HEIGHT", StringComparison.OrdinalIgnoreCase))
								{
									height = double.Parse(splt[1]);
								}
							}
						}
					}
				}

				CheckDumpParagraph(ref lines, blocks);

				var imgSource = new BitmapImage(new Uri(SelectedImage));
				var cellImage = new Image() { Source = imgSource, Stretch = Stretch.Uniform, HorizontalAlignment = HorizontalAlignment.Left };

				if (0 != width)
				{
					cellImage.Width = width;
				}
				if (0 != height)
				{
					cellImage.Height = height;
				}

				blocks.Add(new BlockUIContainer(cellImage));
			}
			else if (!string.IsNullOrEmpty(line))
			{
				var res = _textRePat.Matches(line);
				foreach (Match m in res)
				{
					if (string.IsNullOrEmpty(m.Groups[0].Value))
					{
						if (m.Groups[0].Index < line.Length)
						{
							lines += line[m.Groups[0].Index];
						}
					}
					else
					{
						CheckFormats(ref lines, blocks, m.Groups);
					}
				}
				lines += '\n';
			}
			else
			{
				lines += '\n';
			}
		}

		CheckDumpParagraph(ref lines, blocks);

	}

	private void CheckFormats(ref string? lines, BlockCollection blocks, GroupCollection group)
	{
		for (int i = 1; i < group.Count; i++)
		{
			if (string.IsNullOrEmpty(group[i].Value)) continue;

			// inline tags
			if (group[i].Value.Contains($"{{{enumPdfCover.Date}}}", StringComparison.OrdinalIgnoreCase))
			{
				lines += DateTime.Now.ToString("MM/dd/yyyy");
				return;
			}

			// format tags
			if (group[i].Value.Contains('=') && i + 3 < group.Count)
			{
				if (CheckSingleKVPFormat(ref lines, blocks, group[i + 1].Value, group[i + 2].Value, enumPdfCover.Font)) { } else
				if (CheckSingleKVPFormat(ref lines, blocks, group[i + 1].Value, group[i + 2].Value, enumPdfCover.FontSize)) { } else
				if (CheckSingleKVPFormat(ref lines, blocks, group[i + 1].Value, group[i + 2].Value, enumPdfCover.Align)) { }
				i += 3;
			}
			else if (CheckSingleFormat(ref lines, blocks, group[i].Value, enumPdfCover.b)) { }
			else if (CheckSingleFormat(ref lines, blocks, group[i].Value, enumPdfCover.u)) { }
			else if (CheckSingleFormat(ref lines, blocks, group[i].Value, enumPdfCover.i)) { }
			else if (CheckSingleFormat(ref lines, blocks, group[i].Value, enumPdfCover.Font)) { }
			else if (CheckSingleFormat(ref lines, blocks, group[i].Value, enumPdfCover.FontSize)) { }
		}
	}

	private bool CheckSingleFormat(ref string? lines, BlockCollection blocks, string value, enumPdfCover format)
	{
		if (value.Contains($"{{{format}}}", StringComparison.OrdinalIgnoreCase))
		{
			CheckDumpParagraph(ref lines, blocks);

			_formatMap[format].Add(true);
			return true;
		}
		if (value.Contains($"{{/{format}}}", StringComparison.OrdinalIgnoreCase))
		{
			CheckDumpParagraph(ref lines, blocks);

			_formatMap[format].RemoveAt(_formatMap[format].Count - 1);
			return true;
		}
		return false;
	}

	private bool CheckSingleKVPFormat(ref string? lines, BlockCollection blocks, string key, string value, enumPdfCover format)
	{
		if (key.Equals($"{format}", StringComparison.OrdinalIgnoreCase))
		{
			CheckDumpParagraph(ref lines, blocks);

			if (enumPdfCover.FontSize == format)
			{
				if (value[0] == '+' || value[0] == '-')
				{
					double size = double.Parse(_formatMap[format][0]!.ToString()!);
					size += double.Parse(value);
					_formatMap[format].Add(size.ToString());
				}
				else
				{
					_formatMap[format].Add(value);
				}
			}
			else
			{
				_formatMap[format].Add(value);
			}

			return true;
		}
		if (key.Equals($"/{format}", StringComparison.OrdinalIgnoreCase))
		{
			CheckDumpParagraph(ref lines, blocks);

			_formatMap[format].RemoveAt(_formatMap[format].Count - 1);
			return true;
		}
		return false;
	}

	private void CheckDumpParagraph(ref string? lines, BlockCollection blocks)
	{
		if (null != lines)
		{
			var run = new Run(lines + "\n")
			{
				FontFamily = new FontFamily(_formatMap[enumPdfCover.Font].Last()!.ToString())
			};
			var paragraph = new Paragraph(run);

			var fontsize = _formatMap[enumPdfCover.FontSize].Last()!.ToString();

			if (!string.IsNullOrEmpty(fontsize))
			{
				double size = SelectedSize;
				double.TryParse(fontsize, out size);
				run.FontSize = size;
			}
			if (0 < _formatMap[enumPdfCover.b].Count)
			{
				run.FontWeight = FontWeights.Bold;
			}
			if (0 < _formatMap[enumPdfCover.i].Count)
			{
				run.FontStyle = FontStyles.Italic;
			}
			if (0 < _formatMap[enumPdfCover.u].Count)
			{
				run.TextDecorations = TextDecorations.Underline;
			}
			if (0 < _formatMap[enumPdfCover.Align].Count)
			{
				var value = _formatMap[enumPdfCover.Align].Last()!.ToString();

				if (value.Equals("right", StringComparison.OrdinalIgnoreCase))
				{
					paragraph.TextAlignment = TextAlignment.Right;
				}
				if (value.Equals("center", StringComparison.OrdinalIgnoreCase))
				{
					paragraph.TextAlignment = TextAlignment.Center;
				}
				if (value.Equals("left", StringComparison.OrdinalIgnoreCase))
				{
					paragraph.TextAlignment = TextAlignment.Left;
				}
			}

			blocks.Add(paragraph);

			lines = null;
		}
	}

}
