// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

using Leisn.Common.Attributes;
using Leisn.Common.Data;
using Leisn.Xaml.Wpf.Locales;

namespace Leisn.Xaml.Wpf.Controls
{
    public class EditorHelper
    {
        public static ComboBox CreateComboBox(IEnumerable<IDataDeclaration<object>> datas)
        {
            FrameworkElementFactory contaniner = new(typeof(Border));
            contaniner.SetValue(Border.BackgroundProperty, Brushes.Transparent);
            contaniner.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            contaniner.SetBinding(FrameworkElement.ToolTipProperty, new Binding("Description"));
            contaniner.SetValue(ToolTip.PlacementProperty, PlacementMode.Top);
            FrameworkElementFactory textBlock = new(typeof(TextBlock));
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("DisplayName"));
            contaniner.AppendChild(textBlock);
            DataTemplate dataTemplate = new() { VisualTree = contaniner };

            ComboBox box = new()
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                ItemsSource = datas,
                SelectedValuePath = "Value",
                ItemTemplate = dataTemplate,
            };
            return box;
        }

        private const string Misc = "Misc";
        public static List<PropertyItem> CreatePropertyItems(object source, IPropertyEditorSelector editorSelector)
        {
            return TypeDescriptor.GetProperties(source)
                                 .OfType<PropertyDescriptor>()
                                 .Where(d => d.IsBrowsable)
                                 .Select(d => CreatePropertyItem(source, d, editorSelector))
                                 .ToList();
        }
        public static PropertyItem CreatePropertyItem(object source, PropertyDescriptor propertyDescriptor, IPropertyEditorSelector editorSelector)
        {
            PropertyItem item = new()
            {
                PropertyDescriptor = propertyDescriptor,
                Source = source,
                DefaultValue = propertyDescriptor.Attr<DefaultValueAttribute>()?.Value!,
                IsReadOnly = propertyDescriptor.IsReadOnly,
                Editor = editorSelector.CreateEditor(propertyDescriptor)
            };
            string displayName = propertyDescriptor.DisplayName ?? propertyDescriptor.Name;
            if (propertyDescriptor.IsLocalizable)
            {
                item.SetBindingLangKey(PropertyItem.CategoryProperty, propertyDescriptor.Category);
                item.SetBindingLangKey(PropertyItem.DisplayNameProperty, displayName);
                item.SetBindingLangKey(PropertyItem.DescriptionProperty, propertyDescriptor.Description);
            }
            else
            {
                item.SetBindingLangFormat(PropertyItem.CategoryProperty, Misc.Equals(propertyDescriptor.Category) ? $"{{{Misc}}}" : propertyDescriptor.Category);
                item.SetBindingLangFormat(PropertyItem.DisplayNameProperty, displayName);
                item.SetBindingLangFormat(PropertyItem.DescriptionProperty, propertyDescriptor.Description);
            }
            return item;
        }
    }
}
