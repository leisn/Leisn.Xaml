// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Leisn.Common.Data;
using Leisn.Xaml.Wpf.Locales;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class DictionaryEditor : CollectionEditorBase<Grid>
    {
        public static bool IsSupportType(Type keyType, Type valueType)
        {
            return !keyType.IsEnumerable() || keyType == typeof(string);
        }

        private Type _keyType;
        private Type _valueType;
        private AttributeCollection _propertyAttributes;
        private IPropertyEditorSelector _editorSelector;

        public DictionaryEditor(Type keyType, Type valueType, AttributeCollection propertyAttributes, IPropertyEditorSelector editorSelector)
        {
            _keyType = keyType;
            _valueType = valueType;
            _propertyAttributes = propertyAttributes;
            _editorSelector = editorSelector;
        }

        private UIElement CreateKeyElement(object? key)
        {
            var editor = _editorSelector.CreateEditor(_keyType, AttributeCollection.Empty);
            var element = editor.CreateElement(new PropertyItem
            {
                Attributes = AttributeCollection.Empty,
                PropertyType = _keyType,
                IsReadOnly = IsCoerceReadOnly,
            });
            element.SetValue(editor.GetBindingProperty(), key);
            return element;
        }

        private UIElement CreateValueElement(object? value)
        {
            var editor = _editorSelector.CreateEditor(_valueType, _propertyAttributes);
            var element = editor.CreateElement(new PropertyItem
            {
                Attributes = _propertyAttributes,
                PropertyType = _valueType,
                IsReadOnly = IsCoerceReadOnly,
            });
            element.SetValue(editor.GetBindingProperty(), value);
            return element;
        }

        protected override UIElement CreateOperationBar()
        {
            var grid = new Grid { Margin = new Thickness(0, 5, 0, 0) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.5, GridUnitType.Star) });
            grid.Children.Add(CreateAddButton());

            var keyControl = CreateKeyElement(null);
            Grid.SetColumn(keyControl, 2);
            grid.Children.Add(keyControl);

            var valueControl = CreateValueElement(null);
            Grid.SetColumn(valueControl, 4);
            grid.Children.Add(valueControl);
            return grid;
        }

        protected override Panel CreateItemContaniner(int index, object? item)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });

            var textBlock = new TextBlock
            {
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0),
                Text = $"{index + 1}."
            };
            Grid.SetColumn(textBlock, 1);
            var itemElement = CreateItemElement(index, item);
            Grid.SetColumn(itemElement, 2);
            Grid.SetColumnSpan(itemElement, 2);
            grid.Children.Add(textBlock);
            grid.Children.Add(itemElement);
            grid.Children.Add(CreateDeleteButton(index));
            return grid;
        }
        protected override Grid CreateItemElement(int index, object? item)
        {
            var tuple = (item as ITuple)!;
            var grid = new Grid { IsEnabled = !IsCoerceReadOnly };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.5, GridUnitType.Star) });
            var keyControl = CreateKeyElement(tuple[0]);
            Grid.SetColumn(keyControl, 0);
            grid.Children.Add(keyControl);

            var valueControl = CreateValueElement(tuple[1]);
            Grid.SetColumn(valueControl, 2);
            grid.Children.Add(valueControl);
            return grid;
        }

        protected override object GetElementValue(Grid element)
        {
            return null;
        }

        protected override object CreateNewItem()
        {
            return (Activator.CreateInstance(_keyType), _valueType == typeof(string) ? null : Activator.CreateInstance(_valueType));
        }

        protected override bool AddItemToSource(object? item)
        {
            var tuple = (item as ITuple)!;
            var method = Source.GetType().GetMethod("Add");
            method?.Invoke(Source, new object[] { tuple[0]!, tuple[1]! });
            return true;
        }

        protected override bool DeleteItemFromSource(int index)
        {
            var item = GetElementValue(GetElementAt(index));
            var tuple = (item as ITuple)!;
            var method = Source.GetType().GetMethod("Remove");
            var result = method?.Invoke(Source, new object[] { tuple[0]! });
            return result is bool b && b;
        }

        protected override void OnRemoveElement(Grid element)
        {
        }


    }

}
