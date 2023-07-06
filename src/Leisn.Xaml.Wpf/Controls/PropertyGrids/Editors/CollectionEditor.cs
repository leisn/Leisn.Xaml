// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class CollectionEditor : Control, IPropertyEditor
    {
        public bool UseExpanderStyle => true;
        public DependencyProperty GetBindingProperty() => SourceProperty;
        public FrameworkElement CreateElement(PropertyItem item)
        {
            return this;
        }
        public FrameworkElement? GetOperationContent()
        {
            return null;
        }

        public object Source
        {
            get { return (object)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(CollectionEditor),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSourceChanged)));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }


    }
}
