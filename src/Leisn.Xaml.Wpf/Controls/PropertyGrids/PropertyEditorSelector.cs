// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

using Leisn.Common.Attributes;
using Leisn.Common.Data;
using Leisn.Xaml.Wpf.Controls.Editors;

namespace Leisn.Xaml.Wpf.Controls
{
    public class PropertyEditorSelector : IPropertyEditorSelector
    {
        public virtual IPropertyEditor CreateEditor(PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Attr<EditorAttribute>() is EditorAttribute editorAttr)
            {
                return CreateAttributeEditor(editorAttr, propertyDescriptor);
            }

            return CreateCustomEditor(propertyDescriptor) ?? CreateDefalutEditor(propertyDescriptor);
        }

        protected virtual IPropertyEditor CreateAttributeEditor(EditorAttribute editorAttr, PropertyDescriptor propertyDescriptor)
        {
            Type editorType = Type.GetType(editorAttr.EditorTypeName)!;
            return (IPropertyEditor)UIContext.Create(editorType)!;
        }

        protected virtual IPropertyEditor? CreateCustomEditor(PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Attr<DataProviderAttribute>() is not null)
            {
                return new ComboDataEditor();
            }

            Type propertyType = propertyDescriptor.PropertyType;

            if (propertyType.IsEnum)
            {
                return new EnumEditor();
            }

            if (propertyType == typeof(bool?))
            {
                return new BoolEditor();
            }

            if (propertyType == typeof(TimeOnly))
            {
                return new TimeSelectorEditor();
            }

            if (propertyType == typeof(DateOnly))
            {
                return new DateSelectorEditor();
            }

            if (propertyType == typeof(DateTime?))
            {
                return new DateTimePickerEditor();
            }

            if (propertyType == typeof(Color))
            {
                return new ColorPickerEditor();
            }
            return null;
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
                TypeCode.DateTime => new DateTimePickerEditor(),
                TypeCode.String => propertyDescriptor.Attr<PathSelectAttribute>() != null ? new PathSelectEditor() : new TextEditor(),
                _ => CreateOtherEditor(propertyDescriptor) ?? new ReadOnlyTextEditor()
            };
        }

        //Create at last
        protected virtual IPropertyEditor? CreateOtherEditor(PropertyDescriptor propertyDescriptor)
        {
            Type propertyType = propertyDescriptor.PropertyType;


            if (propertyType.IsAssignableTo(typeof(IEnumerable)))
            {
                return CreateCollectionEditor(propertyDescriptor);
            }

            if (propertyType.IsClass)
            {
                return new ClassEditor();
            }

            return null;
        }

        protected virtual IPropertyEditor CreateCollectionEditor(PropertyDescriptor propertyDescriptor)
        {
            Type type = propertyDescriptor.PropertyType;
            if (type.IsAssignableTo(typeof(IEnumerable<string>)))
            {
                return new StringCollectionEditor();
            }
            Type[] elementTypes;
            if (type.IsGenericType)
            {
                elementTypes = type.GetGenericArguments();
            }
            else if (type.IsArray)
            {
                int rank = type.GetArrayRank();
                if (rank > 1)
                {
                    throw new NotSupportedException($"Array rank `{rank}` greate than 1 is not supported");
                }

                Type elementType = type.GetElementType()
                     ?? throw new NotSupportedException($"Array must have a element type");
                elementTypes = new Type[] { elementType };
            }
            return new CollectionEditor();
        }
    }
}
