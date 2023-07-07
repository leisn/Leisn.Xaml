// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Leisn.Common.Attributes;

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
        private Type? _genericType;
        public bool UseExpanderStyle => true;
        public DependencyProperty GetBindingProperty() => SourceProperty;
        public FrameworkElement CreateElement(PropertyItem item)
        {
            
            var type = item.PropertyType;
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
                return null;

            return null;
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(CollectionEditor), new PropertyMetadata(false));

        public object Source
        {
            get { return GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(CollectionEditor),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var type = e.NewValue.GetType();
            if (type?.IsAssignableTo(typeof(IEnumerable)) != true)
            {
                throw new InvalidOperationException($"Source `{type}`非IEnumerable类型");
            }
        }


    }
}
