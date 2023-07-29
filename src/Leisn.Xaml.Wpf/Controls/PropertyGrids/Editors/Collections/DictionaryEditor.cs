// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using Leisn.Common.Attributes;
using Leisn.Xaml.Wpf.Extensions;
using Leisn.Xaml.Wpf.Locales;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class DictionaryEditor : CollectionEditorBase<Grid>
    {
        public static bool IsSupportType(Type keyType, Type valueType)
        {
            return keyType.IsEnum || typeof(string) == keyType || keyType.IsNumericType();
        }

        private readonly Type _keyType;
        private readonly Type _valueType;
        private readonly AttributeCollection _valueAttributes;
        private readonly AttributeCollection _keyAttributes;
        private readonly IPropertyEditorSelector _editorSelector;
        private FrameworkElement _keyElement = null!;
        private FrameworkElement _valueElement = null!;
        private Button _addButton;
        public DictionaryEditor(Type keyType, Type valueType, AttributeCollection propertyAttributes, IPropertyEditorSelector editorSelector)
        {
            _keyType = keyType;
            _valueType = valueType;
            _editorSelector = editorSelector;

            //pick attributes for key
            List<Attribute> keyAttrList = new();
            List<Attribute> valueAttrList = new();
            foreach (Attribute attr in propertyAttributes)
            {
                if (attr is IDictionaryAttributeTarget forKey)
                {
                    if (forKey.DictionaryTarget is DictionaryTarget.Key)
                    {
                        keyAttrList.Add(attr);
                    }
                    else if (forKey.DictionaryTarget is DictionaryTarget.Value)
                    {
                        valueAttrList.Add(attr);
                    }
                    else if (forKey.DictionaryTarget is DictionaryTarget.Both)
                    {
                        keyAttrList.Add(attr);
                        valueAttrList.Add(attr);
                    }
                }
                else
                {
                    valueAttrList.Add(attr);
                }
            }
            _keyAttributes = new AttributeCollection(keyAttrList.ToArray());
            _valueAttributes = new AttributeCollection(valueAttrList.ToArray());
            _addButton = CreateAddButton();
            Loaded += OnEditorLoaded;
        }

        #region key,value
        public static object GetKeyValue(DependencyObject obj)
        {
            return obj.GetValue(KeyValueProperty);
        }

        public static void SetKeyValue(DependencyObject obj, object value)
        {
            obj.SetValue(KeyValueProperty, value);
        }
        public static readonly DependencyProperty KeyValueProperty =
            DependencyProperty.RegisterAttached("KeyValue", typeof(object), typeof(DictionaryEditor),
                new PropertyMetadata(null, new PropertyChangedCallback(OnKeyValueChanged)));

        public static object GetValueValue(DependencyObject obj)
        {
            return obj.GetValue(ValueValueProperty);
        }

        public static void SetValueValue(DependencyObject obj, object value)
        {
            obj.SetValue(ValueValueProperty, value);
        }
        public static readonly DependencyProperty ValueValueProperty =
            DependencyProperty.RegisterAttached("ValueValue", typeof(object), typeof(DictionaryEditor),
                new PropertyMetadata(null, new PropertyChangedCallback(OnValueValueChanged)));

        private static void OnKeyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;
            DictionaryEditor edtior = (DictionaryEditor)fe.Tag;
            edtior.UpdateCanAdd();
        }

        private static void OnValueValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = (FrameworkElement)d;
            DictionaryEditor edtior = (DictionaryEditor)fe.Tag;
            edtior.UpdateValueToSource(fe);
        }
        #endregion

        private FrameworkElement CreateKeyElement(object? key, bool readOnly)
        {
            IPropertyEditor editor = _editorSelector.CreateEditor(_keyType, _keyAttributes);
            FrameworkElement element = editor.CreateElement(new PropertyItem
            {
                Attributes = _keyAttributes,
                PropertyType = _keyType,
                IsReadOnly = readOnly,
            });
            element.Tag = this;
            SetKeyValue(element, key!);
            BindingOperations.SetBinding(element, editor.GetBindingProperty(), new Binding()
            {
                Source = element,
                Path = new PropertyPath(KeyValueProperty),
                Mode = readOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                Converter = ValueConverter.Instance,
            });
            return element;
        }

        private FrameworkElement CreateValueElement(object? value)
        {
            IPropertyEditor editor = _editorSelector.CreateEditor(_valueType, _valueAttributes);
            FrameworkElement element = editor.CreateElement(new PropertyItem
            {
                Attributes = _valueAttributes,
                PropertyType = _valueType,
                IsReadOnly = IsCoerceReadOnly,
            });
            element.Tag = this;
            SetValueValue(element, value!);
            BindingOperations.SetBinding(element, editor.GetBindingProperty(), new Binding()
            {
                Source = element,
                Path = new PropertyPath(ValueValueProperty),
                Mode = IsCoerceReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                Converter = ValueConverter.Instance,
            });
            if (editor.UseExpanderStyle)
            {
                element.Margin = new Thickness(7);
                Type elementType = element.GetType();
                if (elementType.IsTypeOf(typeof(CollectionEditorBase<>)))
                {
                    ScrollViewer.SetCanContentScroll(element, true);
                }
                Button button = new()
                {
                    ToolTip = _valueType.ToString(),
                    Tag = element
                };
                button.SetBindingLangFormat(ContentProperty, "{Edit} - " + value?.ToString());
                button.Click += EditButtonClicked;
                return button;
            }
            return element;
        }

        private void EditButtonClicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            FrameworkElement element = (FrameworkElement)button.Tag;
            Window window = new()
            {
                Owner = Window.GetWindow(this),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = element,
                Style = (Style)FindResource("DictionaryEditorWindowStyle"),
                Title = $"{Lang.Get("Edit")} - {_valueType.GetShortName()}"
            };
            window.ShowDialog();
            BindingOperations.ClearBinding(button, ContentProperty);
            button.SetBindingLangFormat(ContentProperty, "{Edit} - " + GetValueElementValue(button)?.ToString());
        }
        protected override UIElement? CreateHeader()
        {
            Grid grid = new() { Margin = new Thickness(0, 0, 0, 5) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.5, GridUnitType.Star) });
            Brush textColor = (Brush)FindResource("TextDarkBrush");
            TextBlock keyText = new() { Foreground = textColor, TextAlignment = TextAlignment.Center };
            keyText.SetBindingLangKey(TextBlock.TextProperty, "Key");
            Grid.SetColumn(keyText, 2);
            grid.Children.Add(keyText);

            TextBlock valueText = new() { Foreground = textColor, TextAlignment = TextAlignment.Center };
            valueText.SetBindingLangKey(TextBlock.TextProperty, "Value");
            Grid.SetColumn(valueText, 4);
            grid.Children.Add(valueText);
            return grid;
        }

        protected override Button CreateAddButton()
        {
            var button = base.CreateAddButton();
            button.Margin = new Thickness(0);
            return button;
        }

        protected override UIElement CreateOperationBar()
        {
            Grid grid = new() { Margin = new Thickness(0, 5, 0, 0), IsEnabled = !IsCoerceReadOnly };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.5, GridUnitType.Star) });
            grid.Children.Add(_addButton ??= CreateAddButton());

            object? defalutKey = typeof(string) == _keyType ? string.Empty : Activator.CreateInstance(_keyType);
            _keyElement = CreateKeyElement(defalutKey, false);
            if (_keyElement is ComboBox combo && combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }

            Grid.SetColumn(_keyElement, 2);
            grid.Children.Add(_keyElement);
            object? defaultValue = typeof(string) == _valueType ? string.Empty : Activator.CreateInstance(_valueType);
            _valueElement = CreateValueElement(defaultValue);
            Grid.SetColumn(_valueElement, 4);
            grid.Children.Add(_valueElement);
            return grid;
        }

        protected override Panel CreateItemContaniner(int index, object? item)
        {
            Grid grid = new();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            TextBlock textBlock = new()
            {
                TextAlignment = TextAlignment.Right,
                Margin = new Thickness(0, 0, 10, 0),
                Text = $"{index + 1}."
            };
            Grid.SetColumn(textBlock, 1);
            Grid itemElement = CreateItemElement(index, item);
            Grid.SetColumn(itemElement, 2);
            Grid.SetColumnSpan(itemElement, 2);
            grid.Children.Add(textBlock);
            grid.Children.Add(itemElement);
            grid.Children.Add(CreateDeleteButton(index));
            return grid;
        }

        protected override Grid CreateItemElement(int index, object? item)
        {
            if (item is not ITuple tuple)
            {
                //init values, KeyValuePair<,>
                Type type = item!.GetType();
                object? key = type.GetProperty("Key")!.GetValue(item);
                object? value = type.GetProperty("Value")!.GetValue(item);
                tuple = (key, value);
            }

            Grid grid = new() { IsEnabled = !IsCoerceReadOnly };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(5, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.5, GridUnitType.Star) });
            FrameworkElement keyControl = CreateKeyElement(tuple[0], true);
            Grid.SetColumn(keyControl, 0);
            grid.Children.Add(keyControl);

            FrameworkElement valueControl = CreateValueElement(tuple[1]);
            Grid.SetColumn(valueControl, 2);
            grid.Children.Add(valueControl);
            valueControl.SetBinding(WidthProperty, new Binding(nameof(ActualWidth)) { Source = keyControl });
            return grid;
        }

        protected override object GetElementValue(Grid element)
        {
            FrameworkElement keyElment = (FrameworkElement)element.Children[0];
            FrameworkElement valueElement = (FrameworkElement)element.Children[1];
            return (GetKeyElementValue(keyElment), GetValueElementValue(valueElement));
        }

        private object? GetKeyElementValue(FrameworkElement keyElement)
        {
            return EditorHelper.ConvertValue(GetKeyValue(keyElement), _keyType);
        }
        private object? GetValueElementValue(FrameworkElement valueElement)
        {
            return EditorHelper.ConvertValue(GetValueValue((valueElement is Button b) ? (FrameworkElement)b.Tag : valueElement), _valueType);
        }

        private object GetKeyFromValueElement(FrameworkElement valueElement)
        {
            Grid grid = (Grid)valueElement.GetParent()!;
            FrameworkElement keyElement = (FrameworkElement)grid.Children[0];
            return GetKeyElementValue(keyElement)!;
        }

        protected override object CreateNewItem()
        {
            return (GetKeyElementValue(_keyElement), GetValueElementValue(_valueElement));
        }

        protected override bool? AddItemToSource(object? item)
        {
            ITuple tuple = (item as ITuple)!;
            Type type = Source.GetType();
            System.Reflection.MethodInfo? method = type.GetMethod("ContainsKey");
            object? contanisKey = method?.Invoke(Source, new object[] { tuple[0]! });
            if (contanisKey is bool b && b)
            {
                return false;
            }
            method = type.GetMethod("Add");
            method?.Invoke(Source, new object[] { tuple[0]!, tuple[1]! });
            if (_valueType.IsClass && _valueType != typeof(string))
            {
                object defaultValue;
                if (item is ICloneable clone)
                {
                    defaultValue = clone.Clone();
                }
                else
                {
                    defaultValue = typeof(string) == _valueType ? string.Empty : Activator.CreateInstance(_valueType)!;
                }

                SetValueValue((_valueElement is Button button) ? (FrameworkElement)button.Tag : _valueElement, defaultValue);
            }
            UpdateCanAdd();
            return true;
        }

        protected override bool DeleteItemFromSource(int index)
        {
            object item = GetElementValue(GetElementAt(index));
            ITuple tuple = (item as ITuple)!;
            System.Reflection.MethodInfo? method = Source.GetType().GetMethod("Remove", new Type[] { _keyType });
            bool result = (bool)method?.Invoke(Source, new object[] { tuple[0]! })!;
            if (result)
            {
                UpdateCanAdd();
            }

            return result;
        }

        protected override void OnRemoveElement(Grid element)
        {
            //var keyElment = (FrameworkElement)element.Children[0];
            FrameworkElement valueElement = (FrameworkElement)element.Children[1];
            if (valueElement is Button button)
            {
                button.Click -= EditButtonClicked;
            }
        }

        private void OnEditorLoaded(object sender, RoutedEventArgs e)
        {
            UpdateCanAdd();
        }

        private void UpdateCanAdd()
        {
            if (Source == null || _keyElement == null)
            {
                return;
            }

            object? key = GetKeyElementValue(_keyElement);
            if (key == null)
            {
                _addButton.IsEnabled = false;
                return;
            }
            System.Reflection.MethodInfo? method = Source.GetType().GetMethod("ContainsKey");
            object? contanisKey = method?.Invoke(Source, new object[] { key! });
            _addButton.IsEnabled = !(contanisKey is bool b && b);
        }

        private void UpdateValueToSource(FrameworkElement valueElememt)
        {
            if (Source == null || valueElememt == _valueElement || valueElememt.GetParent() == null)
            {
                return;
            }

            object key = GetKeyFromValueElement(valueElememt);
            object? value = GetValueElementValue(valueElememt);
            System.Reflection.MethodInfo? method = Source.GetType().GetMethod("set_Item");
            method?.Invoke(Source, new object[] { key, value! });
        }
    }

    internal class ValueConverter : IValueConverter
    {
        public static ValueConverter Instance { get; } = new ValueConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EditorHelper.ConvertValue(value, targetType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return EditorHelper.ConvertValue(value, targetType);
        }
    }
}
