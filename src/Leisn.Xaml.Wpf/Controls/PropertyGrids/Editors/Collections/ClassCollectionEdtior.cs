// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Leisn.Common.Attributes;
using Leisn.Common.Collections;
using Leisn.Common.Data;
using Leisn.Xaml.Wpf.Locales;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ClassCollectionEdtior : CollectionEditorBase<ToggleButton>
    {
        private Type _elementType;
        private readonly List<Type> _instanceTypes;
        public ClassCollectionEdtior(Type elementType, AttributeCollection propertyAttributes)
        {
            _elementType = elementType;
            _instanceTypes = new();
            if (!(_elementType.IsInterface || _elementType.IsAbstract)
                || _elementType.GetConstructor(Array.Empty<Type>()) is null)
            {
                _instanceTypes.Add(_elementType);
            }

            propertyAttributes.OfType<InstanceTypesAttribute>()
                .ForEach(attrType => attrType.InstanceTypes.ForEach(AddInstanceType));

            if (propertyAttributes.Attr<InstanceTypeProviderAttribute>() is InstanceTypeProviderAttribute attr)
            {
                var provider = AppIoc.GetRequired(attr.ProviderType) as IDataProvider<Type>
                    ?? throw new ArgumentException($"{attr.ProviderType} is null or not IDataProvider<Type>");
                provider.GetData().ForEach(AddInstanceType);
            }

            void AddInstanceType(Type type)
            {
                if (!type.IsTypeOf(elementType))
                    throw new InvalidOperationException($"{type} is not a subclass or implement of {elementType}");
                if (type.GetConstructor(Type.EmptyTypes) is null)
                    throw new InvalidOperationException($"{type} must have a non-parameter constructor {elementType}");
                _instanceTypes.Add(type);
            }
        }

        protected override UIElement CreateOperationBar()
        {
            var grid = (Grid)base.CreateOperationBar();
            var typeTitle = new TextBlock
            {
                Margin = new Thickness(0, 0, 10, 0),
                TextAlignment = TextAlignment.Right,
                TextTrimming = TextTrimming.CharacterEllipsis,
                Foreground = (Brush)FindResource("TextDarkBrush")
            };
            Grid.SetColumn(typeTitle, 1);
            typeTitle.SetBindingLangKey(TextBlock.TextProperty, "Type");
            grid.Children.Add(typeTitle);
            var typeList = new List<DataDeclaration>();
            _instanceTypes.ForEach(type => typeList.Add(new DataDeclaration
            {
                DisplayName = type.Name,
                Description = type.FullName,
                Value = type
            }));
            var comboBox = EditorHelper.CreateComboBox(typeList);
            comboBox.SelectedValue = _instanceTypes.FirstOrDefault();
            comboBox.SelectionChanged += TypeSelectionChanged;
            Grid.SetColumn(comboBox, 2);
            Grid.SetColumnSpan(comboBox, 2);
            grid.Children.Add(comboBox);
            return grid;
        }

        private void TypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            _elementType = (Type)comboBox.SelectedValue;
        }

        protected override void UpdateIndexText(int index)
        {
            var panel = (Panel)GetContanier().Children[index];
            var textBlock = (TextBlock)panel.Children[0];
            var value = GetElementValue(GetElementAt(index));
            textBlock.Text = $"{index + 1}. {value?.ToString()}";
            //textBlock.ToolTip = value;
        }

        protected override Panel CreateItemContaniner(int index, object? item)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });

            var deleteButton = CreateDeleteButton(index);
            var textBlock = new TextBlock
            {
                TextAlignment = TextAlignment.Left,
                Margin = new Thickness(0, 0, 10, 0),
                Text = $"{index + 1}. {item?.ToString()}",
                TextTrimming = TextTrimming.CharacterEllipsis,
                //ToolTip = item
            };
            //ToolTipService.SetPlacement(textBlock, System.Windows.Controls.Primitives.PlacementMode.Top);
            Grid.SetColumn(textBlock, 1);
            var itemElement = CreateItemElement(index, item);
            Grid.SetColumn(itemElement, 2);

            grid.Children.Add(textBlock);
            grid.Children.Add(itemElement);
            grid.Children.Add(deleteButton);
            return grid;
        }

        protected override ToggleButton CreateItemElement(int index, object? item)
        {
            var path = new Path { Data = (Geometry)FindResource("EditGeometry"), Stretch = Stretch.Uniform };
            var button = new ToggleButton
            {
                Content = path,
                IsEnabled = !IsCoerceReadOnly,
                Tag = item,
                CommandParameter = index,
                Padding = new Thickness(3),
            };
            path.SetBinding(Shape.FillProperty, new Binding(nameof(Foreground)) { Source = button });
            button.SetBinding(WidthProperty, new Binding(nameof(ActualHeight)) { RelativeSource = new RelativeSource(RelativeSourceMode.Self) });
            button.Click += EditButtonClicked;
            return button;
        }

        protected override object CreateNewItem()
        {
            return Activator.CreateInstance(_elementType)!;
        }

        protected override object GetElementValue(ToggleButton element)
        {
            return element.Tag;
        }

        protected override void OnRemoveElement(ToggleButton element)
        {
            element.Click -= EditButtonClicked;
        }

        private void EditButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = (ToggleButton)sender;
            var index = (int)button.CommandParameter;
            var window = new Window
            {
                Title = $"{Lang.Get("Edit_Item")} {index + 1} - {_elementType.Name} - {PropertyItem.DisplayName}",
                Owner = Window.GetWindow(this),
                DataContext = button.Tag,
                Style = (Style)FindResource("ClassEditorWindowStyle"),
            };
            var pos = button.PointToScreen(new Point(0, 0));
            pos.X -= window.Width + 10;
            pos.Y -= window.Height / 2;
            pos.X = Math.Max(0, pos.X);
            pos.Y = Math.Max(0, pos.Y);
            window.Left = pos.X;
            window.Top = pos.Y;
            window.ShowDialog();
            button.IsChecked = false;
            UpdateIndexText(index);
        }

    }
}
