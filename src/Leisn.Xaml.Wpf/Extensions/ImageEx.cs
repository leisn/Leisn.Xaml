// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Leisn.Xaml.Wpf.Extensions
{
    public class ImageEx
    {
        public static Bitmap CaptureScreen()
        {
            int width = (int)SystemParameters.PrimaryScreenWidth;
            int height = (int)SystemParameters.PrimaryScreenHeight;
            Bitmap bitmap = new(width, height);
            using Graphics g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(width, height));
            return bitmap;
        }

        public static bool TryCaptureScreen([NotNullWhen(true)] out ImageSource? image)
        {
            image = null;

            try
            {
                using Bitmap bitmap = CaptureScreen();
                image = Imaging.CreateBitmapSourceFromHBitmap(
                    bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        public static bool TryCaptureScreen([NotNullWhen(true)] out SKBitmap? image)
        {
            image = null;
            try
            {
                using Bitmap bitmap = CaptureScreen();
                image = bitmap.ToSKBitmap();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }
    }
}
