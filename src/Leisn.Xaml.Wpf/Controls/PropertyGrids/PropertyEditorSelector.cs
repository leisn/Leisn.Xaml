// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
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
            if (propertyDescriptor.Attr<EditorAttribute>() is EditorAttribute editorAttr && !string.IsNullOrEmpty(editorAttr.EditorTypeName))
            {
                return CreateAttributeEditor(editorAttr, propertyDescriptor);
            }

            return CreateCustomEditor(propertyDescriptor) ?? CreateDefalutEditor(propertyDescriptor);
        }

        protected virtual IPropertyEditor CreateAttributeEditor(EditorAttribute editorAttr, PropertyDescriptor propertyDescriptor)
        {
            Type editorType = Type.GetType(editorAttr.EditorTypeName)!;
            return (IPropertyEditor)Activator.CreateInstance(editorType)!;
        }

        protected virtual IPropertyEditor? CreateCustomEditor(PropertyDescriptor propertyDescriptor)
        {
            Type propertyType = propertyDescriptor.PropertyType;

            if (propertyDescriptor.Attr<PathSelectAttribute>() is not null)
            {
                return new PathSelectEditor();
            }

            if (propertyDescriptor.Attr<DataProviderAttribute>() is not null)
            {
                if (propertyType != typeof(string) && propertyType.IsEnumerable())
                    return new ComboCollecitonEditor();
                return new ComboDataEditor();
            }

            if (propertyType != typeof(string) && propertyType.IsEnumerable())
            {
                return CreateCollectionEditor(propertyDescriptor);
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

            Type[] elementTypes = null!;
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

            if (elementTypes?.Length == 1 && elementTypes[0].IsNumericType())
            {
                return new NumericCollectionEditor(elementTypes[0]);
            }

            return new CollectionEditor();
        }

        protected virtual IPropertyEditor CreateDefalutEditor(PropertyDescriptor propertyDescriptor)
        {
            var type = propertyDescriptor.PropertyType;
            if (type.IsEnum) return new EnumEditor();
            if (type.IsNumericType()) return new NumericEditor(EditorHelper.ResolveTypeNumericParams(type));
            if (type == typeof(string)) return new TextEditor();
            if (type == typeof(bool) || type == typeof(bool?)) return new BoolEditor();
            if (type == typeof(DateTime) || type == typeof(DateTime?)) return new DateTimePickerEditor();
            if (type == typeof(TimeOnly)) return new TimeSelectorEditor();
            if (type == typeof(DateOnly)) return new DateSelectorEditor();
            if (type == typeof(Color)) return new ColorPickerEditor();
            if (type.IsClass) return new ClassEditor();
            return new ReadOnlyTextEditor();
        }

    }
}
