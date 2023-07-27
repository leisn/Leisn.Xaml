// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Leisn.Common.Attributes;
using Leisn.Common.Collections;
using Leisn.Common.Data;
using Leisn.Xaml.Wpf.Locales;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ClassCollectionEdtior : CollectionEditorBase<PropertyGrid>
    {
        private Type _elementType;
        private readonly List<Type> _instanceTypes;
        public ClassCollectionEdtior(Type elementType, AttributeCollection propertyAttributes)
        {
            Padding = new Thickness(0, 0, 0, 5);
            _elementType = elementType;
            _instanceTypes = new();
            if (!(_elementType.IsInterface || _elementType.IsAbstract)
                || _elementType.GetConstructor(Type.EmptyTypes) is null)
            {
                _instanceTypes.Add(_elementType);
            }

            propertyAttributes.OfType<InstanceTypesAttribute>()
                .ForEach(attrType => attrType.InstanceTypes.ForEach(AddInstanceType));

            if (propertyAttributes.Attr<InstanceTypeProviderAttribute>() is InstanceTypeProviderAttribute attr)
            {
                IDataProvider<Type> provider = AppIoc.GetRequired(attr.ProviderType) as IDataProvider<Type>
                    ?? throw new ArgumentException($"{attr.ProviderType} is null or not IDataProvider<Type>");
                provider.GetData().ForEach(AddInstanceType);
            }

            void AddInstanceType(Type type)
            {
                if (!type.IsTypeOf(elementType))
                {
                    throw new InvalidOperationException($"{type} is not a subclass or implement of {elementType}");
                }

                if (type.GetConstructor(Type.EmptyTypes) is null)
                {
                    throw new InvalidOperationException($"{type} must have a non-parameter constructor {elementType}");
                }

                _instanceTypes.Add(type);
            }
        }

        protected override UIElement CreateOperationBar()
        {
            Grid grid = (Grid)base.CreateOperationBar();
            grid.Margin = new Thickness(7, 6, 7, 0);
            TextBlock typeTitle = new()
            {
                Margin = new Thickness(0, 0, 10, 0),
                TextAlignment = TextAlignment.Right,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Foreground = (Brush)FindResource("TextDarkBrush")
            };
            Grid.SetColumn(typeTitle, 1);
            typeTitle.SetBindingLangKey(TextBlock.TextProperty, "Type");
            grid.Children.Add(typeTitle);
            List<DataDeclaration> typeList = new();
            _instanceTypes.ForEach(type => typeList.Add(new DataDeclaration
            {
                DisplayName = type.Name,
                Description = type.FullName,
                Value = type
            }));
            ComboBox comboBox = EditorHelper.CreateComboBox(typeList);
            comboBox.SelectedValue = _instanceTypes.FirstOrDefault();
            comboBox.SelectionChanged += TypeSelectionChanged;
            Grid.SetColumn(comboBox, 2);
            Grid.SetColumnSpan(comboBox, 2);
            grid.Children.Add(comboBox);
            return grid;
        }

        private void TypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            _elementType = (Type)comboBox.SelectedValue;
        }


        protected override Panel CreateItemContaniner(int index, object? item)
        {
            Grid grid = new();
            StackPanel stackPanel = new() { Spacing = 10, Orientation = Orientation.Horizontal };
            stackPanel.Children.Add(new TextBlock { Text = $"{index + 1}." });
            stackPanel.Children.Add(new TextBlock { Text = $"{item?.GetType().GetShortName()}" });

            Expander expander = new()
            {
                Header = stackPanel,
                Content = CreateItemElement(index, item),
                Padding = new Thickness(0, 5, 0, 0)
            };
            ControlAttach.SetPadding(expander, new Thickness(37, 3, 0, 0));
            Grid.SetColumn(expander, 1);
            grid.Children.Add(expander);
            Button deleteButton = CreateDeleteButton(index);
            deleteButton.VerticalAlignment = VerticalAlignment.Top;
            deleteButton.Margin = new Thickness(7, 0, 0, 0);
            grid.Children.Add(deleteButton);
            return grid;
        }

        protected override PropertyGrid GetElementAt(int index)
        {
            Grid grid = (Grid)GetContanier().Children[index];
            Expander expaner = (Expander)grid.Children[0];
            return (PropertyGrid)expaner.Content;
        }

        protected override void UpdateIndexText(int index)
        {
            Panel panel = (Panel)GetContanier().Children[index];
            Expander expaner = (Expander)panel.Children[0];
            TextBlock textBlock = (TextBlock)((StackPanel)expaner.Header).Children[0];
            textBlock.Text = $"{index + 1}.";
        }

        protected override PropertyGrid CreateItemElement(int index, object? item)
        {
            return new PropertyGrid { Source = item!, Style = (Style)FindResource("SubPropertyGridBaseStyle") };
        }

        protected override object CreateNewItem()
        {
            return Activator.CreateInstance(_elementType)!;
        }

        protected override object GetElementValue(PropertyGrid element)
        {
            return element.Source;
        }

        protected override void OnRemoveElement(PropertyGrid element)
        {
        }
    }
}
