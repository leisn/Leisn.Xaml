// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Leisn.Xaml.Wpf.Extensions;

namespace Leisn.Xaml.Wpf.Controls
{
    public class ScreenshotDialog : Window
    {
        private ImageSource? _image;

        public ImageSource? Image => _image;

        Image _screenshotImage;
        public ScreenshotDialog()
        {
            Width = 1024;Height = 768;
            //WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            Margin = Padding = new Thickness(0);
            Content = _screenshotImage = new Image();
        }
    }
}
