// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class BoolEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            return new ToggleSwitch
            {
                Style = (Style)Application.Current.FindResource("ToggleSwitchStyle"),
                HorizontalAlignment = HorizontalAlignment.Left,
                IsEnabled = !item.IsReadOnly,
                IsThreeState = item.PropertyType == typeof(bool?),
            };
        }

        public DependencyProperty GetBindingProperty()
        {
            return ToggleSwitch.IsCheckedProperty;
        }
    }
}
