// By Leisn (https://leisn.com , https://github.com/leisn)

using Leisn.Common.Attributes;
using Leisn.Common.Data;
using Leisn.Xaml.Wpf.Controls.Inputs;

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Controls.PropertyGrids
{
    public class PropertyEditorSelector : IPropertyEditorSelector
    {
        public virtual IPropertyEditor CreateEditor(PropertyDescriptor propertyDescriptor)
        {
            EditorAttribute? editorAttr = propertyDescriptor.Attr<EditorAttribute>();
            if (editorAttr is null || string.IsNullOrEmpty(editorAttr.EditorTypeName))
            {
                IPropertyEditor? specialEditior = CreateSpecialEditor(propertyDescriptor);
                return specialEditior is not null ? specialEditior : CreateDefalutEditor(propertyDescriptor);
            }
            return CreateCustomEditor(editorAttr, propertyDescriptor);
        }

        protected virtual IPropertyEditor CreateCustomEditor(EditorAttribute editorAttr, PropertyDescriptor propertyDescriptor)
        {
            Type editorType = Type.GetType(editorAttr.EditorTypeName)!;
            return (IPropertyEditor)UIContext.Create(editorType)!;
        }

        protected virtual IPropertyEditor CreateDefalutEditor(PropertyDescriptor propertyDescriptor)
        {
            return Type.GetTypeCode(propertyDescriptor.PropertyType) switch
            {
                TypeCode.Boolean => new BoolEditor(),
                TypeCode.SByte => new NumericEditor(sbyte.MinValue, sbyte.MaxValue, 1, NumericType.Int),
                TypeCode.Byte => new NumericEditor(byte.MinValue, byte.MaxValue, 1, NumericType.UInt),
                TypeCode.Int16 => new NumericEditor(short.MinValue, short.MaxValue, 1, NumericType.Int),
                TypeCode.UInt16 => new NumericEditor(ushort.MinValue, ushort.MaxValue, 1, NumericType.UInt),
                TypeCode.Int32 => new NumericEditor(int.MinValue, int.MaxValue, 1, NumericType.Int),
                TypeCode.UInt32 => new NumericEditor(uint.MinValue, uint.MaxValue, 1, NumericType.UInt),
                TypeCode.Int64 => new NumericEditor(long.MinValue, long.MaxValue, 1, NumericType.Int),
                TypeCode.UInt64 => new NumericEditor(ulong.MinValue, ulong.MaxValue, 1, NumericType.UInt),
                TypeCode.Single => new NumericEditor(float.MinValue, float.MaxValue, 1, NumericType.Float),
                TypeCode.Double => new NumericEditor(double.MinValue, double.MaxValue, 1, NumericType.Float),
                TypeCode.Decimal => new NumericEditor(Convert.ToDouble(decimal.MinValue), Convert.ToDouble(decimal.MaxValue), 1, NumericType.Float),
                TypeCode.DateTime => new DateTimeEditor(),
                TypeCode.String => CreatStringEditor(propertyDescriptor),
                TypeCode.Object => CreateObjectEditor(propertyDescriptor),
                _ => new ReadOnlyTextEditor()
            };
        }

        protected virtual IPropertyEditor? CreateSpecialEditor(PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.PropertyType.IsEnum
                ? new EnumEditor()
                : propertyDescriptor.Attr<DataProviderAttribute>() is not null ? new ComboDataEditor() : (IPropertyEditor?)null;
        }

        protected virtual IPropertyEditor CreatStringEditor(PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.Attr<PathSelectAttribute>() != null ? new PathSelectEditor() : new TextEditor();
        }

        protected virtual IPropertyEditor CreateObjectEditor(PropertyDescriptor propertyDescriptor)
        {
            return propertyDescriptor.PropertyType == typeof(Color)
                ? new ColorPickerEditor()
                : propertyDescriptor.PropertyType.IsAssignableTo(typeof(IEnumerable)) ? (IPropertyEditor)new CollectionEditor() : new ReadOnlyTextEditor();
        }
    }
}
