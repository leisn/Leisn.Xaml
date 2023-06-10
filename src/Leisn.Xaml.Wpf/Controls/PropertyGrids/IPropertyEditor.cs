﻿using System.Windows;

namespace Leisn.Xaml.Wpf.Controls
{
    public interface IPropertyEditor
    {
        FrameworkElement CreateElement(PropertyItem item);

        DependencyProperty GetBindingProperty();
    }
}