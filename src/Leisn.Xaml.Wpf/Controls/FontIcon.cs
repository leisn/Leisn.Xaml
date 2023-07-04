﻿using System;
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
            IsHitTestVisibleProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(false));
            FocusableProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(false));
            IsTabStopProperty.OverrideMetadata(typeof(FontIcon), new FrameworkPropertyMetadata(false));
        }

        public string Glyph
        {
            get { return (string)GetValue(GlyphProperty); }
            set { SetValue(GlyphProperty, value); }
        }
        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register("Glyph", typeof(string), typeof(FontIcon), new FrameworkPropertyMetadata(string.Empty));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(FontIcon));

    }
}