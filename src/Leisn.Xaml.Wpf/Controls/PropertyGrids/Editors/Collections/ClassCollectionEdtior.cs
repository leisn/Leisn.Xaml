// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
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

using Leisn.Xaml.Wpf.Locales;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ClassCollectionEdtior : CollectionEditorBase<ToggleButton>
    {
        private readonly Type _classType;
        public ClassCollectionEdtior(Type classType)
        {
            _classType = classType;
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
            return Activator.CreateInstance(_classType)!;
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
                Title = $"{Lang.Get("Edit Item")} - {PropertyItem.DisplayName}",
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
