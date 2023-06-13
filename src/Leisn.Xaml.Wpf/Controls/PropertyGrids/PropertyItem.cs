// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Controls
{
    public class PropertyItem : ListBoxItem
    {
        static PropertyItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyItem), new FrameworkPropertyMetadata(typeof(PropertyItem)));
        }
        public PropertyDescriptor PropertyDescriptor { get; internal set; } = null!;
        public Type PropertyType => PropertyDescriptor.PropertyType;
        public string PropertyName => PropertyDescriptor.Name;
        public string PropertyTypeName => $"{PropertyDescriptor.PropertyType.Namespace}.{PropertyType.Name}";

        public object Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(PropertyItem), new PropertyMetadata(default));
        public string Category
        {
            get => (string)GetValue(CategoryProperty);
            set => SetValue(CategoryProperty, value);
        }
        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(string), typeof(PropertyItem), new PropertyMetadata(default));
        public string DisplayName
        {
            get => (string)GetValue(DisplayNameProperty);
            set => SetValue(DisplayNameProperty, value);
        }
        public static readonly DependencyProperty DisplayNameProperty =
            DependencyProperty.Register("DisplayName", typeof(string), typeof(PropertyItem), new PropertyMetadata(default));
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(PropertyItem), new PropertyMetadata(default));
        public object DefaultValue
        {
            get => GetValue(DefaultValueProperty);
            set => SetValue(DefaultValueProperty, value);
        }
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register(nameof(DefaultValue), typeof(object), typeof(PropertyItem), new PropertyMetadata(default));
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
        public static readonly DependencyProperty IsReadOnlyProperty
            = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(PropertyItem), new PropertyMetadata(false));

        public FrameworkElement EditorElement
        {
            get => (FrameworkElement)GetValue(EditorElementProperty);
            set => SetValue(EditorElementProperty, value);
        }
        public static readonly DependencyProperty EditorElementProperty =
            DependencyProperty.Register(
                nameof(EditorElement), typeof(FrameworkElement), typeof(PropertyItem), new FrameworkPropertyMetadata(default(FrameworkElement)));
        public IPropertyEditor Editor
        {
            get => (IPropertyEditor)GetValue(EditorProperty);
            set => SetValue(EditorProperty, value);
        }
        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.Register("Editor", typeof(IPropertyEditor), typeof(PropertyItem),
                new FrameworkPropertyMetadata(default(IPropertyEditor), new PropertyChangedCallback(OnEditorChagned)));

        private static void OnEditorChagned(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyItem editor = (PropertyItem)d;
            editor.OnEditorChagned(e.OldValue as IPropertyEditor, e.NewValue as IPropertyEditor);
        }

        protected virtual void OnEditorChagned(IPropertyEditor? oldEditor, IPropertyEditor? newEditor)
        {
            if (oldEditor != null && EditorElement != null)
            {
                BindingOperations.ClearBinding(EditorElement, oldEditor.GetBindingProperty());
            }
            if (newEditor != null)
            {
                CreateElement();
            }
            else
            {
                EditorElement = null!;
            }
        }

        protected virtual void CreateElement()
        {
            if (Editor == null)
            {
                return;
            }

            EditorElement = Editor.CreateElement(this);

            Binding binding = new()
            {
                Source = Source,
                Path = new PropertyPath(PropertyName),
                Mode = IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            };
            BindingOperations.SetBinding(EditorElement, Editor.GetBindingProperty(), binding);
        }
    }
}
