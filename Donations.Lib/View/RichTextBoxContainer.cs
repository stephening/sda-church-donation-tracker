using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Donations.Lib.View;

public class RichTextBoxContainer
{
	public RichTextBox? RichTextBox { get; set; }

	public void SetRichText(byte[]? richText)
	{
		if (null != RichTextBox)
		{
			using var stream = new MemoryStream(richText);
			var range = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
			range.Load(stream, DataFormats.XamlPackage);
		}
	}

	public byte[]? GetRichText()
	{
		if (null != RichTextBox)
		{
			TextRange range;
			range = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
			using var stream = new MemoryStream();
			range.Save(stream, DataFormats.XamlPackage);
			byte[] buffer = new byte[stream.Length];
			stream.Seek(0, SeekOrigin.Begin);
			stream.Read(buffer, 0, (int)stream.Length);

			return buffer;
		}

		return null;
	}

	public void RichTextToSection(Section section)
	{
		if (null != RichTextBox)
		{
			section.Blocks.Clear();
			TextRange range;
			range = new TextRange(RichTextBox.Document.ContentStart, RichTextBox.Document.ContentEnd);
			using var stream = new MemoryStream();
			range.Save(stream, DataFormats.XamlPackage);

			range = new TextRange(section.ContentStart, section.ContentEnd);
			range.Load(stream, DataFormats.XamlPackage);
		}
	}
}
