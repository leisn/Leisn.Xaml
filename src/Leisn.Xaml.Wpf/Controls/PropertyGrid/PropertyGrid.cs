using Leisn.Common;
using Leisn.Common.Attributes;
using Leisn.Xaml.Wpf.Controls.Editors;
using Leisn.Xaml.Wpf.Locales;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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
                Editor = CreateEditor(propertyDescriptor)
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

        private IPropertyEditor CreateEditor(PropertyDescriptor propertyDescriptor)
        {
            EditorAttribute? editorAttr = propertyDescriptor.Attr<EditorAttribute>();
            if (editorAttr is null || string.IsNullOrEmpty(editorAttr.EditorTypeName))
            {
                return CreateDefalutEditor(propertyDescriptor);
            }
            return CreateEditor(Type.GetType(editorAttr.EditorTypeName)!, propertyDescriptor);
        }

        protected virtual IPropertyEditor CreateEditor(Type editorType, PropertyDescriptor propertyDescriptor)
        {
            return (IPropertyEditor)Activator.CreateInstance(editorType)!;
        }

        protected virtual IPropertyEditor CreateDefalutEditor(PropertyDescriptor propertyDescriptor)
        {
            if(propertyDescriptor.PropertyType.IsEnum)
                return new EnumEditor();
            return Type.GetTypeCode(propertyDescriptor.PropertyType) switch
            {
                TypeCode.Boolean => new BoolEditor(),
                TypeCode.SByte => new NumberEditor(sbyte.MinValue, sbyte.MaxValue, 1, NumericType.Int),
                TypeCode.Byte => new NumberEditor(byte.MinValue, byte.MaxValue, 1, NumericType.UInt),
                TypeCode.Int16 => new NumberEditor(short.MinValue, short.MaxValue, 1, NumericType.Int),
                TypeCode.UInt16 => new NumberEditor(ushort.MinValue, ushort.MaxValue, 1, NumericType.UInt),
                TypeCode.Int32 => new NumberEditor(int.MinValue, int.MaxValue, 1, NumericType.Int),
                TypeCode.UInt32 => new NumberEditor(uint.MinValue, uint.MaxValue, 1, NumericType.UInt),
                TypeCode.Int64 => new NumberEditor(long.MinValue, long.MaxValue, 1, NumericType.Int),
                TypeCode.UInt64 => new NumberEditor(ulong.MinValue, ulong.MaxValue, 1, NumericType.UInt),
                TypeCode.Single => new NumberEditor(float.MinValue, float.MaxValue, 1, NumericType.Float),
                TypeCode.Double => new NumberEditor(double.MinValue, double.MaxValue, 1, NumericType.Float),
                TypeCode.Decimal => new NumberEditor(Convert.ToDouble(decimal.MinValue), Convert.ToDouble(decimal.MaxValue), 1, NumericType.Float),
                TypeCode.DateTime => new DateTimeEditor(),
                TypeCode.String => CreatStringEditor(propertyDescriptor),
                TypeCode.Object => CreateObjectEditor(propertyDescriptor),
                _ => new ReadOnlyTextEditor()
            };
        }



        protected virtual IPropertyEditor CreatStringEditor(PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.Attr<PathSelectAttribute>() != null)
                return new PathSelectEditor();
            return new TextEditor();
        }
        protected virtual IPropertyEditor CreateObjectEditor(PropertyDescriptor propertyDescriptor)
        {
            if (propertyDescriptor.PropertyType.IsAssignableTo(typeof(IEnumerable)))
            {
                return new CollectionEditor();
            }
            return new ReadOnlyTextEditor();
        }
    }
}
