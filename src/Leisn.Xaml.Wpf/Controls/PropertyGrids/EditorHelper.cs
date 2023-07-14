// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

using Leisn.Common.Attributes;
using Leisn.Common.Data;
using Leisn.Xaml.Wpf.Controls.Editors;
using Leisn.Xaml.Wpf.Locales;

namespace Leisn.Xaml.Wpf.Controls
{
    public class EditorHelper
    {
        #region data provider
        public static IEnumerable<IDataDeclaration<object>> ResolveDataProvider(Type providerType)
        {
            if (providerType.IsEnum)
            {
                return Enum.GetValues(providerType).OfType<Enum>().Select(x => new DataDeclaration
                {
                    Value = x,
                    DisplayName = x.Attr<CategoryAttribute>()?.Category ?? x.ToString(),
                    Description = x.Attr<DescriptionAttribute>()?.Description ?? x.ToString()
                });
            }

            object? instance = AppIoc.GetRequired(providerType);
            if (instance is not IDataProvider<object> provider)
            {
                throw new InvalidCastException($"{providerType} is not IDataProvider.");
            }
            IEnumerable<object> values = provider.GetData();
            Type dataType = provider.GetDataType();
            if (values is not IEnumerable<IDataDeclaration<object>> data)
            {
                if (dataType.IsArray)
                {
                    data = values.Select(x =>
                    {
                        Array array = (Array)x;
                        object? value = array.GetValue(0);
                        return new DataDeclaration
                        {
                            Value = value,
                            DisplayName = array.Length > 1 ? array.GetValue(1)?.ToString() : value?.ToString(),
                            Description = array.Length > 2 ? array.GetValue(2)?.ToString() : null
                        };
                    });
                }
                else
                {
                    data = values.Select(x => new DataDeclaration
                    {
                        Value = x,
                        DisplayName = x.ToString(),
                    });
                }
            }
            return data;
        }
        public static ComboBox CreateComboBox(IEnumerable<IDataDeclaration<object>> datas)
        {
            FrameworkElementFactory contaniner = new(typeof(Border));
            contaniner.SetValue(Border.BackgroundProperty, Brushes.Transparent);
            contaniner.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            contaniner.SetBinding(FrameworkElement.ToolTipProperty, new Binding(nameof(DataDeclaration.Description)));
            contaniner.SetValue(ToolTip.PlacementProperty, PlacementMode.Top);
            FrameworkElementFactory textBlock = new(typeof(TextBlock));
            textBlock.SetBinding(TextBlock.TextProperty, new Binding(nameof(DataDeclaration.DisplayName)));
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
        #endregion

        #region numeric editor
        internal static NumericEditorParams ResolveAttrNumericParams(NumericEditorParams current, AttributeCollection attributes)
        {
            if (attributes.Attr<NumericUpDownAttribute>() is NumericUpDownAttribute attr)
            {
                current.Maximum = attr.Maximum;
                current.Minimum = attr.Minimum;
                current.Increment = attr.Increment;
            }
            if (attributes.Attr<RangeAttribute>() is RangeAttribute range)
            {
                current.Maximum = Convert.ToDouble(range.Maximum);
                current.Minimum = Convert.ToDouble(range.Minimum);
            }

            if (attributes.Attr<IncrementAttribute>()?.Increment is double increment)
            {
                current.Increment = increment;
            }
            if (current.Maximum - current.Minimum < current.Increment)
            {
                current.Increment = (current.Maximum - current.Minimum) / 10;
            }

            NumericFormat numberFormat = new();
            if (attributes.Attr<NumericFormatAttribute>() is NumericFormatAttribute format)
            {
                numberFormat.Suffix = format.Suffix;
                numberFormat.Decimals = format.Decimals;
            }
            current.Format = numberFormat;

            if (current.Minimum > current.Maximum)
            {
                throw new InvalidOperationException($"Minimum > Maxium: {current.Minimum} > {current.Maximum}");
            }
            return current;
        }

        internal static NumericEditorParams ResolveTypeNumericParams(Type type)
        {
            return Type.GetTypeCode(type) switch
            {
                TypeCode.SByte => new NumericEditorParams(sbyte.MinValue, sbyte.MaxValue, 1, NumericType.Int),
                TypeCode.Byte => new NumericEditorParams(byte.MinValue, byte.MaxValue, 1, NumericType.UInt),
                TypeCode.Int16 => new NumericEditorParams(short.MinValue, short.MaxValue, 1, NumericType.Int),
                TypeCode.UInt16 => new NumericEditorParams(ushort.MinValue, ushort.MaxValue, 1, NumericType.UInt),
                TypeCode.Int32 => new NumericEditorParams(int.MinValue, int.MaxValue, 1, NumericType.Int),
                TypeCode.UInt32 => new NumericEditorParams(uint.MinValue, uint.MaxValue, 1, NumericType.UInt),
                TypeCode.Int64 => new NumericEditorParams(long.MinValue, long.MaxValue, 1, NumericType.Int),
                TypeCode.UInt64 => new NumericEditorParams(ulong.MinValue, ulong.MaxValue, 1, NumericType.UInt),
                TypeCode.Single => new NumericEditorParams(float.MinValue, float.MaxValue, 1, NumericType.Float),
                TypeCode.Double => new NumericEditorParams(double.MinValue, double.MaxValue, 1, NumericType.Float),
                TypeCode.Decimal => new NumericEditorParams(Convert.ToDouble(decimal.MinValue), Convert.ToDouble(decimal.MaxValue), 1, NumericType.Float),
                _ => throw new ArithmeticException($"{nameof(type)} Not a NumericType")
            };
        }
        #endregion

        #region property item
        private const string Misc = "Misc";
        public static List<PropertyItem> CreatePropertyItems(object source, IPropertyEditorSelector editorSelector)
        {
            if (source.GetType().IsEnumerable())
                return new List<PropertyItem> { new PropertyItem { PropertyType = source.GetType()} };
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
                Attributes = propertyDescriptor.Attributes,
                PropertyName = propertyDescriptor.Name,
                PropertyType = propertyDescriptor.PropertyType,
                PropertyTypeName = $"{propertyDescriptor.PropertyType.Namespace}.{propertyDescriptor.Name}",
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
        #endregion
    }
}
