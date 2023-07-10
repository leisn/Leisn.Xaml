// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Leisn.Common.Collections;
using Leisn.Xaml.Wpf.Converters;
using Leisn.Xaml.Wpf.Input;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class StringCollectionEditor : IPropertyEditor
    {
        public bool UseExpanderStyle => true;

        public FrameworkElement CreateElement(PropertyItem item)
        {
            return new StringItemsView
            {
                Padding = new Thickness(5, 0, 5, 5),
                IsReadOnly = item.IsReadOnly
            };
        }

        public DependencyProperty GetBindingProperty()
        {
            return StringItemsView.ItemsSourceProperty;
        }
    }
}
