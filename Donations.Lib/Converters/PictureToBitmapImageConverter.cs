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

		if (picture?.Image == null) return null;

		BitmapImage img = new BitmapImage();
		using (MemoryStream memStream = new MemoryStream(picture?.Image))
		{
			img.BeginInit();
			img.CacheOption = BitmapCacheOption.OnLoad;
			img.StreamSource = memStream;
			img.EndInit();
			img.Freeze();
		}
		return img;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return null;
	}
}
