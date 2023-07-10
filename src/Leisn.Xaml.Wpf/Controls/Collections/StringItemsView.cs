// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

using Leisn.Xaml.Wpf.Controls.Editors;
using Leisn.Xaml.Wpf.Converters;

using Leisn.Xaml.Wpf.Input;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = PART_ItemsHostName, Type = typeof(Panel))]
    [TemplatePart(Name = PART_AddButtonName, Type = typeof(ButtonBase))]
    public class StringItemsView : Control
    {
        const string PART_ItemsHostName = "PART_ItemsHost";
        const string PART_AddButtonName = "PART_AddButton";

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsReadOnlyProperty = TextBoxBase.IsReadOnlyProperty.AddOwner(typeof(StringItemsView));

        public Style TextBoxStyle
        {
            get { return (Style)GetValue(TextBoxStyleProperty); }
            set { SetValue(TextBoxStyleProperty, value); }
        }
        public static readonly DependencyProperty TextBoxStyleProperty =
            DependencyProperty.Register("TextBoxStyle", typeof(Style), typeof(StringItemsView), new PropertyMetadata(null));

        public IEnumerable<string> ItemsSource
        {
            get { return (IEnumerable<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<string>), typeof(StringItemsView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sce = (StringItemsView)d;
            if (e.NewValue is not ICollection<string> collection)
            {
                sce.IsReadOnly = true;
            }
            else if (!sce.IsReadOnly)
            {
                sce.IsReadOnly = collection.IsReadOnly;
            }
            sce.InitItems();
        }

        public ICommand DeleteItemCommand { get; }
        public ICommand AddItemCommand { get; }

        private Panel _itemsHost = null!;
        private ButtonBase _addButtont = null!;

        public StringItemsView()
        {
            DeleteItemCommand = new ControlCommand<int>(DoDelecteItem);
            AddItemCommand = new ControlCommand(DoAddItem);
        }

        public override void OnApplyTemplate()
        {
            if (_addButtont != null)
            {
                _addButtont.Click -= OnAddItemClicked;
            }
            base.OnApplyTemplate();
            _itemsHost = (Panel)GetTemplateChild(PART_ItemsHostName);
            _addButtont = (ButtonBase)GetTemplateChild(PART_AddButtonName);
            _addButtont.Click += OnAddItemClicked;
            InitItems();
        }

        private void OnAddItemClicked(object sender, RoutedEventArgs e)
        {
            DoAddItem();
        }


        bool _isInit;
        private void InitItems()
        {
            if (_itemsHost is null || ItemsSource is null)
                return;
            _isInit = true;
            var newCount = ItemsSource.Count();
            var oldCount = _itemsHost.Children.Count - 1;
            if (newCount >= oldCount)
            {
                for (int i = 0; i < newCount - oldCount; i++)
                {
                    AppendTextBox();
                }
            }
            else
            {
                for (int i = oldCount - newCount; i >= 0; i--)
                {
                    RemoveTextBox(i);
                }
            }

            int index = 0;
            foreach (var item in ItemsSource)
            {
                var textBox = (TextBox)_itemsHost.Children[index];
                textBox.Tag = index;
                textBox.Text = item;
                textBox.IsReadOnly = IsReadOnly;
                index++;
            }
            _isInit = false;
        }

        private TextBox AppendTextBox()
        {
            var textBox = new TextBox
            {
                Tag = _itemsHost.Children.Count - 1,
                IsReadOnly = IsReadOnly
            };
            textBox.SetBinding(StyleProperty, new Binding(nameof(TextBoxStyle)) { Source = this });
            if (!IsReadOnly)
                textBox.LostFocus += TextBox_LostFocus;
            var index = _itemsHost.Children.Count - 1;
            _itemsHost.Children.Insert(index, textBox);
            return textBox;
        }

        private void RemoveTextBox(int index)
        {
            var textBox = (TextBox)_itemsHost.Children[index];
            textBox.LostFocus -= TextBox_LostFocus;
            _itemsHost.Children.RemoveAt(index);
            for (int i = index; i < _itemsHost.Children.Count - 1; i++)
            {
                ((FrameworkElement)_itemsHost.Children[i]).Tag = i;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
                return;
            var textBox = (TextBox)sender;
            var index = (int)textBox.Tag;
            DoUpdateItem(index, textBox.Text);
        }

        protected virtual void DoDelecteItem(int index)
        {
            RemoveTextBox(index);
            if (ItemsSource is IList<string> list)
            {
                list.RemoveAt(index);
                return;
            }
            UpdateToSource();
        }

        protected virtual void DoAddItem()
        {
            var textBox = AppendTextBox();
            if (ItemsSource is ICollection<string> list)
            {
                list.Add(textBox.Text);
                return;
            }
            UpdateToSource();
        }

        protected virtual void DoUpdateItem(int index, string value)
        {
            if (ItemsSource is IList<string> list)
            {
                list[index] = value;
                return;
            }
            UpdateToSource();
        }

        protected virtual void UpdateToSource()
        {
            if (IsReadOnly || _isInit || ItemsSource is not ICollection<string> source)
                return;
            source.Clear();
            foreach (var item in _itemsHost.Children)
            {
                if (item is TextBox tb)
                {
                    source.Add(tb.Text);
                }
            }
        }
    }
}
