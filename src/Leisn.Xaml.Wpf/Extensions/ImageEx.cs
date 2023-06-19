// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Leisn.Xaml.Wpf.Extensions
{
    public class ImageEx
    {
        public static Bitmap CaptureScreen()
        {
            var width = (int)SystemParameters.PrimaryScreenWidth;
            var height = (int)SystemParameters.PrimaryScreenHeight;
            var bitmap = new Bitmap(width, height);
            using var g = Graphics.FromImage(bitmap);
            g.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(width, height));
            return bitmap;
        }

        public static bool TryCaptureScreen([NotNullWhen(true)] out ImageSource? image)
        {
            image = null;

            try
            {
                using var bitmap = CaptureScreen();
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
                using var bitmap = CaptureScreen();
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
