using Donations.Lib.Interfaces;
using Donations.Lib.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Donations.Lib.TestDataServices;

public class TestDataPictureServices : IPictureServices
{
	private Picture _picture;

	public TestDataPictureServices()
	{
		BitmapImage bmi = new BitmapImage(new Uri("pack://application:,,,/Donations.Lib;component/Resources/icon256.png"));

		var encoder = new PngBitmapEncoder();
		encoder.Frames.Add(BitmapFrame.Create(bmi));

		using var ms = new MemoryStream();
		encoder.Save(ms);

		_picture = new Picture() { Id = 1 };
		_picture.Image = ms.ToArray();
	}

	public Picture GetLogo()
	{
		return _picture;
	}

	public async Task<int> SaveLogo(Picture picture)
	{
		throw new NotImplementedException();
	}
}
