using Donations.Lib.Model;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Donations.Lib.Converters;

public class PictureToBitmapImageConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		Picture picture = value as Picture;

#pragma warning disable CS8603 // Possible null reference return.
		if (picture?.Image == null) return null;
#pragma warning restore CS8603 // Possible null reference return.

		BitmapImage img = new BitmapImage();
#pragma warning disable CS8604 // Possible null reference argument.
		using (MemoryStream memStream = new MemoryStream(picture?.Image))
		{
			img.BeginInit();
			img.CacheOption = BitmapCacheOption.OnLoad;
			img.StreamSource = memStream;
			img.EndInit();
			img.Freeze();
		}
#pragma warning restore CS8604 // Possible null reference argument.
		return img;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
#pragma warning disable CS8603 // Possible null reference return.
		return null;
#pragma warning restore CS8603 // Possible null reference return.
	}
}
