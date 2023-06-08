using Leisn.Common;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class EnumEditor : IPropertyEditor
    {
        private class EnumDesc
        {
            public Enum Value { get; set; } = null!;
            public string Name { get; set; } = null!;
            public string Desc { get; set; } = null!;
        }
        public FrameworkElement CreateElement(PropertyItem item)
        {
            var values = Enum.GetValues(item.PropertyType).OfType<Enum>().Select(x => new EnumDesc
            {
                Value = x,
                Name = x.Attr<CategoryAttribute>()?.Category ?? x.ToString(),
                Desc = x.Attr<DescriptionAttribute>()?.Description ?? x.ToString()
            });

            var contaniner = new FrameworkElementFactory(typeof(Border));
            contaniner.SetValue(Border.BackgroundProperty, Brushes.Transparent);
            contaniner.SetValue(Border.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            contaniner.SetBinding(Border.ToolTipProperty, new Binding("Desc"));
            contaniner.SetValue(ToolTip.PlacementProperty, PlacementMode.Top);
            var textBlock = new FrameworkElementFactory(typeof(TextBlock));
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            contaniner.AppendChild(textBlock);
            var dataTemplate = new DataTemplate { VisualTree = contaniner };

            var box = new ComboBox
            {
                IsReadOnly = item.IsReadOnly,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                ItemsSource = values,
                //DisplayMemberPath = "Name",
                SelectedValuePath = "Value",
                ItemTemplate = dataTemplate,
            };
            return box;
        }

        public DependencyProperty GetBindingProperty()
            => ComboBox.SelectedValueProperty;
    }
}
