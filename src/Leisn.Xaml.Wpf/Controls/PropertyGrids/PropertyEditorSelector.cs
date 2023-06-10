using Leisn.Common;
using Leisn.Common.Attributes;
using Leisn.Xaml.Wpf.Controls.Editors;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var specialEditior = CreateSpecialEditor(propertyDescriptor);
                if (specialEditior is not null)
                    return specialEditior;
                return CreateDefalutEditor(propertyDescriptor);
            }
            return CreateCustomEditor(editorAttr, propertyDescriptor);
        }

        protected virtual IPropertyEditor CreateCustomEditor(EditorAttribute editorAttr, PropertyDescriptor propertyDescriptor)
        {
            var editorType = Type.GetType(editorAttr.EditorTypeName)!;
            return (IPropertyEditor)Activator.CreateInstance(editorType)!;
        }

        protected virtual IPropertyEditor CreateDefalutEditor(PropertyDescriptor propertyDescriptor) =>
            Type.GetTypeCode(propertyDescriptor.PropertyType) switch
            {
                TypeCode.Boolean => new BoolEditor(),
                TypeCode.SByte => new NumberEditor(sbyte.MinValue, sbyte.MaxValue, 1, NumericType.Int),
                TypeCode.Byte => new NumberEditor(byte.MinValue, byte.MaxValue, 1, NumericType.UInt),
                TypeCode.Int16 => new NumberEditor(short.MinValue, short.MaxValue, 1, NumericType.Int),
                TypeCode.UInt16 => new NumberEditor(ushort.MinValue, ushort.MaxValue, 1, NumericType.UInt),
                TypeCode.Int32 => new NumberEditor(int.MinValue, int.MaxValue, 1, NumericType.Int),
                TypeCode.UInt32 => new NumberEditor(uint.MinValue, uint.MaxValue, 1, NumericType.UInt),
                TypeCode.Int64 => new NumberEditor(long.MinValue, long.MaxValue, 1, NumericType.Int),
                TypeCode.UInt64 => new NumberEditor(ulong.MinValue, ulong.MaxValue, 1, NumericType.UInt),
                TypeCode.Single => new NumberEditor(float.MinValue, float.MaxValue, 1, NumericType.Float),
                TypeCode.Double => new NumberEditor(double.MinValue, double.MaxValue, 1, NumericType.Float),
                TypeCode.Decimal => new NumberEditor(Convert.ToDouble(decimal.MinValue), Convert.ToDouble(decimal.MaxValue), 1, NumericType.Float),
                TypeCode.DateTime => new DateTimeEditor(),
                TypeCode.String => CreatStringEditor(propertyDescriptor),
                TypeCode.Object => CreateObjectEditor(propertyDescriptor),
                _ => new ReadOnlyTextEditor()
            };

        protected virtual IPropertyEditor? CreateSpecialEditor(PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType.IsEnum)
                return new EnumEditor();
            if (propertyDescriptor.Attr<DataProviderAttribute>() is not null)
                return new ComboEditor();
            return null;
        }

        protected virtual IPropertyEditor CreatStringEditor(PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Attr<PathSelectAttribute>() != null)
                return new PathSelectEditor();
            return new TextEditor();
        }

        protected virtual IPropertyEditor CreateObjectEditor(PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType == typeof(Color))
                return new ColorPickerEditor();
            if (propertyDescriptor.PropertyType.IsAssignableTo(typeof(IEnumerable)))
                return new CollectionEditor();
            return new ReadOnlyTextEditor();
        }
    }
}
