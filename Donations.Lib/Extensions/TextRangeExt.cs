using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Donations.Lib.Extensions;

public static class TextRangeExt
{
	public static IList<Image> FindImages(this TextRange range)
	{
		IList<Image> images = new List<Image>();
		for (var position = range.Start;
			position != null && position.CompareTo(range.End) <= 0;
			position = position.GetNextContextPosition(LogicalDirection.Forward))
		{
			if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart
				&& position.GetAdjacentElement(LogicalDirection.Forward) is InlineUIContainer uic && uic.Child is Image img)
			{
				images.Add(img);
			}
		}
		return images;
	}
}
