using Leisn.Common;
using Leisn.Common.Attributes;
using Leisn.Xaml.Wpf.Controls.Editors;
using Leisn.Xaml.Wpf.Controls.PropertyGrids;
using Leisn.Xaml.Wpf.Locales;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = ItemsControlName, Type = typeof(ItemsControl))]
    public class PropertyGrid : Control
    {
        private const string ItemsControlName = "PART_ItemsControl";
        private ItemsControl _itemsControl = null!;

        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PropertyGrid), new PropertyMetadata(new CornerRadius()));

        public static readonly RoutedEvent SourceChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedObjectChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<object>), typeof(PropertyGrid));
        public event RoutedPropertyChangedEventHandler<object> SourceChanged
        {
            add => AddHandler(SourceChangedEvent, value);
            remove => RemoveHandler(SourceChangedEvent, value);
        }

        public IPropertyEditorSelector EditorSelector
        {
            get { return (IPropertyEditorSelector)GetValue(EditorSelectorProperty); }
            set { SetValue(EditorSelectorProperty, value); }
        }
        public static readonly DependencyProperty EditorSelectorProperty =
            DependencyProperty.Register("EditorSelector", typeof(IPropertyEditorSelector), typeof(PropertyGrid),
                new PropertyMetadata(new PropertyEditorSelector(), new PropertyChangedCallback(OnEditorSelectorChanged)));

        private static void OnEditorSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is null)
                throw new ArgumentNullException(nameof(EditorSelector), "Editor Selector cannot be null");
            var pg = (PropertyGrid)d;
            pg.UpdateItems(pg.Source);
        }

        public object Source { get => GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(default, new PropertyChangedCallback(OnSourceChanged)));
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PropertyGrid)?.OnSourceChanged(e.OldValue, e.NewValue);
        }

        protected virtual void OnSourceChanged(object oldValue, object newValue)
        {
            UpdateItems(newValue);
            RaiseEvent(new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, SourceChangedEvent));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _itemsControl = (ItemsControl)GetTemplateChild(ItemsControlName);
        }

        const string OTHER_KEY = "Misc";
        protected virtual void UpdateItems(object newValue)
        {
            if (newValue is null || _itemsControl is null)
            {
                return;
            }

            List<PropertyItem> items = TypeDescriptor.GetProperties(newValue.GetType())
                 .OfType<PropertyDescriptor>()
                 .Where(x => x.IsBrowsable)
                 .Select(CreatePropertyItem).ToList();
            //var miscs = items.Where(x => string.IsNullOrEmpty(x.PropertyDescriptor.Category)
            //                            || OTHER_KEY.Equals(x.PropertyDescriptor.Category))
            //                 .ToList();
            //if (miscs != null)
            //{
            //    foreach (var item in miscs)
            //    {
            //        items.Remove(item);
            //        items.Add(item);
            //    }
            //}
            ICollectionView view = CollectionViewSource.GetDefaultView(items);
            view.GroupDescriptions.Add(new PropertyGroupDescription(PropertyItem.CategoryProperty.Name));
            _itemsControl.ItemsSource = view;
        }

        protected virtual PropertyItem CreatePropertyItem(PropertyDescriptor propertyDescriptor)
        {
            PropertyItem item = new()
            {
                PropertyDescriptor = propertyDescriptor,
                Source = Source,
                DefaultValue = propertyDescriptor.Attr<DefaultValueAttribute>()?.Value!,
                IsReadOnly = propertyDescriptor.IsReadOnly,
                Editor = EditorSelector.CreateEditor(propertyDescriptor)
            };
            var displayName = propertyDescriptor.DisplayName ?? propertyDescriptor.Name;
            if (propertyDescriptor.IsLocalizable)
            {
                LangExtension.SetBinding(item, PropertyItem.CategoryProperty, propertyDescriptor.Category);
                LangExtension.SetBinding(item, PropertyItem.DisplayNameProperty, displayName);
                LangExtension.SetBinding(item, PropertyItem.DescriptionProperty, propertyDescriptor.Description);
            }
            else
            {
                LangExtension.SetBindingFormat(item, PropertyItem.CategoryProperty, OTHER_KEY.Equals(propertyDescriptor.Category) ? $"{{{OTHER_KEY}}}" : propertyDescriptor.Category);
                LangExtension.SetBindingFormat(item, PropertyItem.DisplayNameProperty, displayName);
                LangExtension.SetBindingFormat(item, PropertyItem.DescriptionProperty, propertyDescriptor.Description);
            }
            return item;
        }
    }
}
