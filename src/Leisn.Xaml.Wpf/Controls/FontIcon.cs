using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Controls
{
    public class FontIcon : Control
    {
        static FontIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(typeof(FontIcon)));
            FontFamilyProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(new FontFamily("Segoe MDL2 Assets")));
        }

        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }
        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register("Glyph", typeof(string), typeof(FontIcon), new PropertyMetadata(string.Empty));
    }
}
