// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Reflection;
using System.Windows.Media;

using Leisn.Common.Attributes;
using Leisn.Xaml.Wpf.Controls.Editors;

namespace Leisn.Xaml.Wpf.Controls
{
    public class PropertyEditorSelector : IPropertyEditorSelector
    {
        public virtual IPropertyEditor CreateEditor(PropertyDescriptor propertyDescriptor)
        {
            return CreateEditor(propertyDescriptor.PropertyType, propertyDescriptor.Attributes);
        }

        public IPropertyEditor CreateEditor(Type propertyType, AttributeCollection propertyAttributes)
        {
            if (propertyAttributes.Attr<EditorAttribute>() is EditorAttribute editorAttr && !string.IsNullOrEmpty(editorAttr.EditorTypeName))
            {
                return CreateAttributeEditor(editorAttr, propertyAttributes);
            }
            return CreateCustomEditor(propertyType, propertyAttributes) ?? CreateDefalutEditor(propertyType, propertyAttributes);
        }

        protected virtual IPropertyEditor CreateAttributeEditor(EditorAttribute editorAttr, AttributeCollection propertyAttributes)
        {
            Type editorType = Type.GetType(editorAttr.EditorTypeName)!;
            return (IPropertyEditor)Activator.CreateInstance(editorType)!;
        }

        protected virtual IPropertyEditor? CreateCustomEditor(Type propertyType, AttributeCollection propertyAttributes)
        {

            if (propertyType != typeof(string) && propertyType.IsEnumerable())
            {
                return CreateCollectionEditor(propertyType, propertyAttributes);
            }

            if (propertyType == typeof(string) && propertyAttributes.Contains<PathSelectAttribute>())
            {
                return new PathSelectEditor();
            }

            if (propertyAttributes.Contains<DataProviderAttribute>())
            {
                return new ComboDataEditor();
            }

            return null;
        }

        protected virtual IPropertyEditor? CreateCollectionEditor(Type propertyType, AttributeCollection propertyAttributes)
        {
            if (propertyType.IsAssignableTo(typeof(IEnumerable<string>)))
            {
                if (propertyAttributes.Attr<PathSelectAttribute>() is PathSelectAttribute pathSelectAttribute)
                    return new PathCollectionEditor(pathSelectAttribute);
                return new StringCollectionEditor();
            }

            Type[] elementTypes = null!;
            if (propertyType.IsGenericType)
            {
                elementTypes = propertyType.GetGenericArguments();
            }
            else if (propertyType.IsArray)
            {
                int rank = propertyType.GetArrayRank();
                if (rank > 1)
                {
                    throw new NotSupportedException($"Array rank `{rank}` greate than 1 is not supported");
                }

                Type elementType = propertyType.GetElementType()
                     ?? throw new NotSupportedException($"Array must have a element type");
                elementTypes = new Type[] { elementType };
            }

            if (propertyType.GetGenericInterfaceTypeOf(typeof(IDictionary<,>)) is Type dictType)
            {
                var arguments = dictType.GetGenericArguments();
                var keyType = arguments[0];
                var valueType = arguments[1];
                if (DictionaryEditor.IsSupportType(keyType, valueType))
                    return new DictionaryEditor(keyType, valueType, propertyAttributes, this);
            }

            if (elementTypes?.Length == 1)
            {
                var elementType = elementTypes[0];
                if (elementType.IsEnum) return new ComboCollecitonEditor(elementType);
                if (propertyAttributes.Attr<DataProviderAttribute>() is DataProviderAttribute providerAttribute)
                    return new ComboCollecitonEditor(providerAttribute.ProviderType);
                if (elementType.IsNumericType()) return new NumericCollectionEditor(elementType);
                if (elementType.IsClass) return new ClassCollectionEdtior(elementType, propertyAttributes);
            }
            return null;
        }

        protected virtual IPropertyEditor CreateDefalutEditor(Type type, AttributeCollection propertyAttributes)
        {
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
