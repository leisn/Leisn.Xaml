// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Shapes;

using Leisn.Xaml.Wpf.Converters;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal abstract class CollectionEditorBase<T> : ContentControl, IPropertyEditor where T : UIElement
    {
        public bool UseExpanderStyle => true;

        private readonly Panel _contanier;

        public PropertyItem PropertyItem { get; private set; } = null!;
        public CollectionEditorBase()
        {
            _contanier = GetContanier();
            DockPanel panel = new() { Margin = new Thickness(6, 0, 6, 5), };

            var operationBar = CreateOperationBar();
            DockPanel.SetDock(operationBar, Dock.Bottom);
            panel.Children.Add(operationBar);
            panel.Children.Add(_contanier);
            Content = panel;
        }

        public virtual FrameworkElement CreateElement(PropertyItem item)
        {
            PropertyItem = item;
            IsCoerceReadOnly = item.IsReadOnly;
            return this;
        }

        public DependencyProperty GetBindingProperty()
        {
            return SourceProperty;
        }

        public bool IsCoerceReadOnly
        {
            get => (bool)GetValue(IsCoerceReadOnlyProperty);
            set => SetValue(IsCoerceReadOnlyProperty, value);
        }
        public static readonly DependencyProperty IsCoerceReadOnlyProperty =
            DependencyProperty.Register("IsCoerceReadOnly", typeof(bool), typeof(CollectionEditorBase<T>),
                new PropertyMetadata(false, new PropertyChangedCallback(OnIsCoerceReadOnlyChanged)));

        private static void OnIsCoerceReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CollectionEditorBase<T> sce = (CollectionEditorBase<T>)d;
            bool value = (bool)e.NewValue;
            sce.ShowOperationButtons = !value;
            sce.IsItemReadOnly = value;
        }

        public bool IsItemReadOnly
        {
            get => (bool)GetValue(IsItemReadOnlyProperty);
            set => SetValue(IsItemReadOnlyProperty, value);
        }
        public static readonly DependencyProperty IsItemReadOnlyProperty =
            DependencyProperty.Register("IsItemReadOnly", typeof(bool), typeof(CollectionEditorBase<T>), new PropertyMetadata(false));

        public bool ShowOperationButtons
        {
            get => (bool)GetValue(ShowOperationButtonsProperty);
            set => SetValue(ShowOperationButtonsProperty, value);
        }
        public static readonly DependencyProperty ShowOperationButtonsProperty =
            DependencyProperty.Register("ShowOperationButtons", typeof(bool), typeof(CollectionEditorBase<T>), new PropertyMetadata(true));

        public IEnumerable Source
        {
            get { return (IEnumerable)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(IEnumerable), typeof(CollectionEditorBase<T>),
                new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChagned)));

        private static void OnSourceChagned(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (CollectionEditorBase<T>)d;
            v.OnSourceChanged();
        }
        protected virtual void OnSourceChanged()
        {
            try
            {
                if (Source == null)
                    return;
                if (IsCoerceReadOnly)
                {
                    ShowOperationButtons = false;
                    IsItemReadOnly = true;
                    return;
                }

                var type = Source.GetType();
                if (type.IsArray)
                {
                    ShowOperationButtons = false;
                    IsItemReadOnly = false;
                    return;
                }

                if (type.IsImplementOf(typeof(ICollection<>)))
                {
                    var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                    var property = (type.GetProperty("IsReadOnly", flags)
                        ?? type.GetProperty("System.Collections.IList.IsReadOnly", flags))
                        ?? type.GetProperty("System.Collections.Generic.ICollection<T>.IsReadOnly", flags);
                    bool isReadOnly = property?.GetValue(Source) is bool b && b;
                    ShowOperationButtons = !isReadOnly;
                    IsItemReadOnly = isReadOnly;
                    return;
                }

                ShowOperationButtons = false;
                IsItemReadOnly = true;
            }
            finally
            {
                GetContanier().Children.Clear();
                foreach (var item in Source)
                {
                    AddItem(item);
                }
            }
        }

        protected virtual Panel GetContanier()
        {
            return _contanier ?? new StackPanel
            {
                Spacing = 6,
                Orientation = Orientation.Vertical,
            };
        }

        protected virtual UIElement CreateOperationBar()
        {
            var stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 10,
                Margin = new Thickness(0, 5, 0, 0)
            };
            var addButton = new Button
            {
                Style = (Style)FindResource("AddButtonStyle"),
                Visibility = Visibility.Visible,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            addButton.SetBinding(VisibilityProperty, new Binding("ShowOperationButtons")
            {
                Source = this,
                Converter = (IValueConverter)FindResource("BoolVisibilityConverter"),
                ConverterParameter = BoolVisibilityConverterMode.FalseCollapsed
            });
            addButton.Click += AddButton_Click;
            stackPanel.Children.Add(addButton);
            return stackPanel;
        }

        protected virtual void RemoveItemAt(int index)
        {
            var grid = (Grid)GetContanier().Children[index];
            var button = (Button)grid.Children[^1];
            button.Click -= DeleteButton_Click;
            var element = GetElementAt(index);
            OnRemoveElement(element);
            GetContanier().Children.RemoveAt(index);
            var count = GetElementCount();
            for (int i = index; i < count; i++)
            {
                UpdateIndexText(i);
            }
        }

        private void AddItem(object? item)
        {
            var count = GetElementCount();
            GetContanier().Children.Add(CreateItemContaniner(count, item));
        }

        protected virtual void UpdateIndexText(int index)
        {
            var panel = (Panel)GetContanier().Children[index];
            var textBlock = (TextBlock)panel.Children[0];
            textBlock.Text = $"{index + 1}.";
        }

        protected virtual Panel CreateItemContaniner(int index, object? item)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.4, GridUnitType.Star), MinWidth = 50 });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(.6, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30, GridUnitType.Pixel) });

            var deleteButton = CreateDeleteButton(index);
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
            grid.Children.Add(deleteButton);
            return grid;
        }

        protected Button CreateDeleteButton(int index)
        {
            var button = new Button
            {
                Name = "deleteButton",
                Tag = index,
                Style = (Style)FindResource("ClearButtonStyle"),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
            button.SetBinding(VisibilityProperty, new Binding("ShowOperationButtons")
            {
                Source = this,
                Converter = (IValueConverter)FindResource("BoolVisibilityConverter"),
                ConverterParameter = BoolVisibilityConverterMode.FalseCollapsed
            });
            ControlAttach.SetShowClear(button, false);
            button.Click += DeleteButton_Click;
            return button;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var item = CreateNewItem();
            var method = Source.GetType().GetMethod("Add");
            if (method is not null)
            {
                method.Invoke(Source, new object[] { item });
                AddItem(item);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var index = (int)((Button)sender).Tag;

            var method = Source.GetType().GetMethod("RemoveAt");
            if (method is not null)
            {
                method.Invoke(Source, new object[] { index });
                RemoveItemAt(index);
            }
            else
            {
                //clear and add
                method = Source.GetType().GetMethod("Clear");
                if (method is null)
                    return;
                method.Invoke(Source, Array.Empty<object>());
                method = Source.GetType().GetMethod("Add");
                if (method is null)
                    return;
                RemoveItemAt(index);
                var count = GetElementCount();
                for (int i = 0; i < count; i++)
                {
                    var element = GetElementAt(i);
                    var value = GetElementValue(element);
                    method.Invoke(Source, new object[] { value });
                }
            }
        }

        protected int GetElementCount()
        {
            return GetContanier().Children.Count;
        }

        protected virtual T GetElementAt(int index)
        {
            var grid = (Grid)GetContanier().Children[index];
            return (T)grid.Children[1];
        }

        protected virtual int GetElementIndex(T element)
        {
            var count = GetElementCount();
            for (int i = 0; i < count; i++)
            {
                if (element == GetElementAt(i))
                    return i;
            }
            return -1;
        }

        protected void UpdateItemValue(T element)
        {
            var index = GetElementIndex(element);
            var value = GetElementValue(element);
            var type = Source.GetType();
            MethodInfo? method;
            if (type.IsArray)
            {
                method = type.GetMethod("Set");
                method!.Invoke(Source, new object[] { index, value });
                return;
            }

            method = type.GetMethod("set_Item");
            if (method is not null)
            {
                method!.Invoke(Source, new object[] { index, value });
                return;
            }

            //clear and add
            method = Source.GetType().GetMethod("Clear");
            if (method is null)
                return;
            method.Invoke(Source, Array.Empty<object>());
            method = Source.GetType().GetMethod("Add");
            if (method is null)
                return;
            var count = GetElementCount();
            for (int i = 0; i < count; i++)
            {
                var v = GetElementValue(GetElementAt(i));
                method.Invoke(Source, new object[] { v });
            }
        }

        protected abstract T CreateItemElement(int index, object? item);
        protected abstract object GetElementValue(T element);
        protected abstract object CreateNewItem();
        protected abstract void OnRemoveElement(T element);
    }
}
