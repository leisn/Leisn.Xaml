// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;

using Leisn.Common.Attributes;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class PathSelectEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            var attr = item.PropertyDescriptor.Attr<PathSelectAttribute>() ?? new PathSelectAttribute();
            return new PathSelector
            {
                Mode = attr.Mode,
                IsTextReadOnly = attr.IsTextReadOnly,
                DialogTitle = attr.DialogTitle!,
                FileFilter = attr.FileFilter!,
                IsEnabled = !item.IsReadOnly,
            };
        }

        public DependencyProperty GetBindingProperty() => PathSelector.PathProperty;
    }
}
