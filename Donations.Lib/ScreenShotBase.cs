using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Donations.Lib;

public class ScreenShotBase
{
	public async Task SaveScreenshot(Window window, string folder, string filename, int msDelay = 500)
	{
		// wait for screen content to load
		await Task.Delay(msDelay);

		double border = SystemParameters.ResizeFrameVerticalBorderWidth + SystemParameters.FixedFrameHorizontalBorderHeight + SystemParameters.BorderWidth * SystemParameters.Border;

		RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
			(int)(window.Width - 2 * border),
			(int)(window.Height - 2 * border - SystemParameters.CaptionHeight),
			96, 96, PixelFormats.Default);

		renderTargetBitmap.Render(window);
		JpegBitmapEncoder jpegEncoder = new JpegBitmapEncoder();
		jpegEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
		using Stream fs = File.Create(Path.Combine(folder, filename));
		jpegEncoder.Save(fs);
	}

}
