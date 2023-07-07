// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Leisn.Xaml.Wpf.Locales;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = ItemsControlName, Type = typeof(ItemsControl))]
    public class PropertyGrid : Control
    {
        private const string ItemsControlName = "PART_ItemsControl";
        private ItemsControl _itemsControl = null!;
        private ICollectionView? _collectionView;

        static PropertyGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGrid), new FrameworkPropertyMetadata(typeof(PropertyGrid)));
        }

        public PropertyGrid()
        {
            Lang.LangChanged += OnLangChanged;
        }

        #region properties
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(PropertyGrid), new PropertyMetadata(new CornerRadius()));

        public IPropertyEditorSelector EditorSelector
        {
            get => (IPropertyEditorSelector)GetValue(EditorSelectorProperty);
            set => SetValue(EditorSelectorProperty, value);
        }
        public static readonly DependencyProperty EditorSelectorProperty =
            DependencyProperty.Register("EditorSelector", typeof(IPropertyEditorSelector), typeof(PropertyGrid),
                new PropertyMetadata(new PropertyEditorSelector(), new PropertyChangedCallback(OnEditorSelectorChanged)));

        private static void OnEditorSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is null)
            {
                throw new ArgumentNullException(nameof(EditorSelector), "Editor Selector cannot be null");
            }
            (d as PropertyGrid)?.UpdateItems();
        }

        public object Source { get => GetValue(SourceProperty); set => SetValue(SourceProperty, value); }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(object), typeof(PropertyGrid),
                new FrameworkPropertyMetadata(default, new PropertyChangedCallback(OnSourceChanged)));
        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PropertyGrid)?.OnSourceChanged(e.OldValue, e.NewValue);
        }
        #endregion

        public static readonly RoutedEvent SourceChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(SourceChanged), RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<object>), typeof(PropertyGrid));
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<object> SourceChanged
        {
            add => AddHandler(SourceChangedEvent, value);
            remove => RemoveHandler(SourceChangedEvent, value);
        }
        protected virtual void OnSourceChanged(object oldValue, object newValue)
        {
            UpdateItems();
            RaiseEvent(new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, SourceChangedEvent));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _itemsControl = (ItemsControl)GetTemplateChild(ItemsControlName);
            UpdateItems();
        }

        protected virtual void OnLangChanged()
        {
            _ = Dispatcher.InvokeAsync(() => _collectionView?.Refresh());
        }

        protected virtual void UpdateItems()
        {
            if (Source is null || _itemsControl is null || EditorSelector is null)
            {
                return;
            }
            _collectionView = CollectionViewSource.GetDefaultView(CreatePropertyItems());
            _collectionView.GroupDescriptions.Add(new PropertyGroupDescription(PropertyItem.CategoryProperty.Name));
            _itemsControl.ItemsSource = _collectionView;
        }

        protected virtual List<PropertyItem> CreatePropertyItems()
        {
            return EditorHelper.CreatePropertyItems(Source, EditorSelector);
        }
    }
}
