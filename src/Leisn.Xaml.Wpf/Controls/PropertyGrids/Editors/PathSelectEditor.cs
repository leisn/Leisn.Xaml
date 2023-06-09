using Leisn.Common;
using Leisn.Common.Attributes;

using Microsoft.WindowsAPICodePack.Dialogs;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class PathSelectEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            var attr = item.PropertyDescriptor.Attr<PathSelectAttribute>() ?? new PathSelectAttribute();
            return new PathSelector
            {
                IsSelectFolder = attr.IsSelectFolder,
                IsTextReadOnly = attr.IsTextReadOnly,
                DialogTitle = attr.DialogTitle!,
                FileFilter = attr.FileFilter!,
                IsEnabled = !item.IsReadOnly,
            };
        }

        public DependencyProperty GetBindingProperty() => PathSelector.PathProperty;
    }
}
