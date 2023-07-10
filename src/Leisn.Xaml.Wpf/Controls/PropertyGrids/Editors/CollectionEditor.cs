// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    public enum CollectionEditorType
    {
        Enumerable,
        Collection,
        List,
        Dictionary
    }
    internal class CollectionEditor : Control, IPropertyEditor
    {
        private readonly Type? _genericType;
        public bool UseExpanderStyle => true;
        public DependencyProperty GetBindingProperty()
        {
            return SourceProperty;
        }

        public FrameworkElement CreateElement(PropertyItem item)
        {

            Type? type = item.PropertyType;
            if (type?.IsAssignableTo(typeof(IEnumerable)) != true)
            {
                throw new InvalidOperationException($"PropertyType `{type}`非IEnumerable类型");
            }

            IsReadOnly = true;
            if (item.IsReadOnly)
            {
                return this;
            }

            return this;
        }
        public FrameworkElement? GetOperationContent()
        {
            if (IsReadOnly)
            {
                return null;
            }

            return null;
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(CollectionEditor), new PropertyMetadata(false));

        public object Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(CollectionEditor),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Type? type = e.NewValue.GetType();
            if (type?.IsAssignableTo(typeof(IEnumerable)) != true)
            {
                throw new InvalidOperationException($"Source `{type}`非IEnumerable类型");
            }
        }


    }
}
