// @Leisn (https://leisn.com , https://github.com/leisn)

using System.DirectoryServices.ActiveDirectory;
using System.Windows;

using Leisn.Common.Attributes;
using Leisn.Common.Helpers;
using Leisn.Xaml.Wpf.Locales;

namespace Leisn.Xaml.Wpf.Controls.Editors
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
