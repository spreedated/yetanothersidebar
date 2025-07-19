using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YetAnotherMonitor.ViewLogic.Converters
{
    internal static class ImageSourceConverters
    {
        public static Icon ToIcon(this ImageSource imageSource)
        {
            BitmapSource bitmapSource = imageSource as BitmapSource;
            Bitmap bitmap;
            using (var outStream = new System.IO.MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapSource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return Icon.FromHandle(bitmap.GetHicon());
        }

        public static ImageSource ToImageSource(this string pathData)
        {
            Path path = new()
            {
                Data = Geometry.Parse(pathData),
                Stroke = System.Windows.Media.Brushes.Yellow,
                StrokeThickness = 1
            };

            RenderTargetBitmap bmp = new((int)path.Data.Bounds.Width, (int)path.Data.Bounds.Height, 96, 96, PixelFormats.Default);
            bmp.Render(path);

            return bmp;
        }
    }
}
