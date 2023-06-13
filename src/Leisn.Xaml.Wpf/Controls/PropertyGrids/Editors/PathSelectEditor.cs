// By Leisn (https://leisn.com , https://github.com/leisn)

using Leisn.Common.Attributes;
using Leisn.Xaml.Wpf.Controls.Inputs;

using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.PropertyGrids.Editors
{
    internal class PathSelectEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            PathSelectAttribute attr = item.PropertyDescriptor.Attr<PathSelectAttribute>() ?? new PathSelectAttribute();
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
