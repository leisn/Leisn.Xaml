// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
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

        public Style TextBoxStyle
        {
            get { return (Style)GetValue(TextBoxStyleProperty); }
            set { SetValue(TextBoxStyleProperty, value); }
        }
        public static readonly DependencyProperty TextBoxStyleProperty =
            DependencyProperty.Register("TextBoxStyle", typeof(Style), typeof(StringItemsView), new PropertyMetadata(null));

        public bool IsCoerceReadOnly
        {
            get { return (bool)GetValue(IsCoerceReadOnlyProperty); }
            set { SetValue(IsCoerceReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsCoerceReadOnlyProperty =
            DependencyProperty.Register("IsCoerceReadOnly", typeof(bool), typeof(StringItemsView),
                new PropertyMetadata(false, new PropertyChangedCallback(OnIsCoerceReadOnlyChanged)));

        private static void OnIsCoerceReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sce = (StringItemsView)d;
            var value = (bool)e.NewValue;
            sce.ShowOperationButtons = !value;
            sce.IsItemReadOnly = value;
        }

        public bool IsItemReadOnly
        {
            get { return (bool)GetValue(IsItemReadOnlyProperty); }
            set { SetValue(IsItemReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsItemReadOnlyProperty =
            DependencyProperty.Register("IsItemReadOnly", typeof(bool), typeof(StringItemsView), new PropertyMetadata(false));

        public bool ShowOperationButtons
        {
            get { return (bool)GetValue(ShowOperationButtonsProperty); }
            set { SetValue(ShowOperationButtonsProperty, value); }
        }
        public static readonly DependencyProperty ShowOperationButtonsProperty =
            DependencyProperty.Register("ShowOperationButtons", typeof(bool), typeof(StringItemsView), new PropertyMetadata(true));

        public IEnumerable<string> ItemsSource
        {
            get { return (IEnumerable<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<string>), typeof(StringItemsView),
                new PropertyMetadata(null, new PropertyChangedCallback(OnItemsSourceChanged)));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sce = (StringItemsView)d;
            sce.OnItemsSourceChanged();
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

        protected virtual void OnItemsSourceChanged()
        {
            try
            {
                if (IsCoerceReadOnly)
                    return;

                if (ItemsSource is string[])
                {
                    ShowOperationButtons = false;
                    IsItemReadOnly = true;
                    return;
                }

                if (ItemsSource is ICollection<string> collection)
                {
                    IsItemReadOnly = collection.IsReadOnly;
                    ShowOperationButtons = !collection.IsReadOnly;
                    return;
                }

                ShowOperationButtons = false;
                IsItemReadOnly = true;
            }
            finally
            {
                InitItems();
            }
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
                index++;
            }
            _isInit = false;
        }

        private TextBox AppendTextBox()
        {
            var textBox = new TextBox
            {
                Tag = _itemsHost.Children.Count - 1,
            };
            textBox.SetBinding(TextBoxBase.IsReadOnlyProperty, new Binding(nameof(IsItemReadOnly)) { Source = this });
            textBox.SetBinding(StyleProperty, new Binding(nameof(TextBoxStyle)) { Source = this });
            if (!IsItemReadOnly)
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
            if (IsItemReadOnly)
                return;
            var textBox = (TextBox)sender;
            var index = (int)textBox.Tag;
            DoUpdateItem(index, textBox.Text);
        }

        protected virtual void DoDelecteItem(int index)
        {
            if (!ShowOperationButtons)
                return;
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
            if (!ShowOperationButtons)
                return;
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
            if (IsItemReadOnly)
                return;
            if (ItemsSource is IList<string> list)
            {
                list[index] = value;
                return;
            }
            if (ItemsSource is string[] strings)
            {
                strings[index] = value;
                return;
            }
            UpdateToSource();
        }

        protected virtual void UpdateToSource()
        {
            if (_isInit || ItemsSource is not ICollection<string> source)
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
