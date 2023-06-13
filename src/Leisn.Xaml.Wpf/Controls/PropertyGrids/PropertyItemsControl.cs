// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls
{
    public class PropertyItemsControl : ListBox
    {
        static PropertyItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyItemsControl), new FrameworkPropertyMetadata(typeof(PropertyItemsControl)));
            VirtualizingPanel.ScrollUnitProperty.OverrideMetadata(typeof(PropertyItemsControl), new FrameworkPropertyMetadata(ScrollUnit.Pixel));
            VirtualizingPanel.IsVirtualizingWhenGroupingProperty.OverrideMetadata(typeof(PropertyItemsControl), new FrameworkPropertyMetadata(true));
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is PropertyItem;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new PropertyItem();
        }
    }
}
