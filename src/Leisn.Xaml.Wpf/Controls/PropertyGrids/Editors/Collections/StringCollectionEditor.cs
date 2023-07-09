// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using Leisn.Common.Collections;
using Leisn.Xaml.Wpf.Input;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class StringCollectionEditor : ItemsControl, IPropertyEditor
    {
        public ICommand DeleteItemCommand { get; }
        public ICommand AddItemCommand { get; }

        public bool UseExpanderStyle => true;

        private PropertyItem _propertyItem = null!;

        public ObservableCollection<string> Values { get; }

        public StringCollectionEditor()
        {
            ItemsSource = Values = new ObservableCollection<string>();
            DeleteItemCommand = new ControlCommand<int>(DoDelecteItem);
            AddItemCommand = new ControlCommand(DoAddItem);
        }

        [MemberNotNull(nameof(_propertyItem))]
        public FrameworkElement CreateElement(PropertyItem item)
        {
            _propertyItem = item;
            if (item.IsReadOnly)
            {
                IsReadOnly = true;
            }
            else
            {
                if (item.GetPropertyValue() is ICollection<string> collection)
                {
                    IsReadOnly = collection.IsReadOnly;
                }
            }
            return this;
        }
        public FrameworkElement? GetOperationContent()
        {
            if (IsReadOnly)
                return null;

            var button = new Button
            {
                Padding = new Thickness(),
                Height = double.NaN,
                Style = (Style)FindResource("TextButtonStyle")
            };
            var path = new Path { Data = (Geometry)FindResource("AddGeometry"), Stretch = Stretch.Uniform };
            path.SetBinding(Shape.FillProperty, new Binding("Foreground") { Source = button });
            path.SetBinding(HeightProperty, new Binding("FontSize") { Source = button });
            button.Content = path;
            button.SetBinding(WidthProperty, new Binding("ActualHeight") { Source = button });
            button.SetBinding(Button.CommandProperty, new Binding(nameof(AddItemCommand)) { Source = this });
            return button;
        }

        public DependencyProperty GetBindingProperty()
        {
            return SourceProperty;
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(StringCollectionEditor), new PropertyMetadata(false));

        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(StringCollectionEditor), new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sce = (StringCollectionEditor)d;
            sce.OnSourceChanged();
        }

        private void OnSourceChanged()
        {
            Values.Clear();
            if (Source is IEnumerable<string> strings)
            {
                strings.ForEach(s => Values.Add(s));
            }
        }

        private void DoDelecteItem(int index)
        {
            Values.RemoveAt(index);
        }

        private void DoAddItem()
        {
            Values.Add("new");
        }
    }
}
