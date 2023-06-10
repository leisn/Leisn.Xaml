﻿using Leisn.Common;

using Leisn.Common.Interfaces;
using Leisn.Common.Models;

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class EnumEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            var values = Enum.GetValues(item.PropertyType).OfType<Enum>().Select(x => new DataDeclaration
            {
                Value = x,
                DisplayName = x.Attr<CategoryAttribute>()?.Category ?? x.ToString(),
                Description = x.Attr<DescriptionAttribute>()?.Description ?? x.ToString()
            });

            ComboBox box = EditorHelper.CreateComboBox(values);
            box.IsReadOnly = item.IsReadOnly;
            return box;
        }

        public DependencyProperty GetBindingProperty()
        {
            return System.Windows.Controls.Primitives.Selector.SelectedValueProperty;
        }
    }
}