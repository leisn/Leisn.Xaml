using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls
{
    public class DateTimePickerItem : Button
    {
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(DateTimePickerItem), new PropertyMetadata(0));

        public string Subtitle
        {
            get { return (string)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }
        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(DateTimePickerItem), new PropertyMetadata(null));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(DateTimePickerItem), new PropertyMetadata(false));

        public bool IsCurrent
        {
            get { return (bool)GetValue(IsCurrentProperty); }
            set { SetValue(IsCurrentProperty, value); }
        }
        public static readonly DependencyProperty IsCurrentProperty =
            DependencyProperty.Register("IsCurrent", typeof(bool), typeof(DateTimePickerItem), new PropertyMetadata(false));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(DateTimePickerItem), new PropertyMetadata(true));


    }
}
