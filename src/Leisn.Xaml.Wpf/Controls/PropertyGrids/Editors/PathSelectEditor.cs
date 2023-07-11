// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows;

using Leisn.Common.Attributes;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class PathSelectEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            PathSelectAttribute attr = item.PropertyDescriptor.Attr<PathSelectAttribute>()
                ?? throw new InvalidOperationException("No PathSelectAttribute");
            return new PathSelector
            {
                Mode = attr.Mode,
                IsTextReadOnly = attr.IsTextReadOnly,
                DialogTitle = attr.DialogTitle!,
                FileFilter = attr.FileFilter!,
                IsEnabled = !item.IsReadOnly,
            };
        }

        public DependencyProperty GetBindingProperty()
        {
            return PathSelector.PathProperty;
        }
    }
}
