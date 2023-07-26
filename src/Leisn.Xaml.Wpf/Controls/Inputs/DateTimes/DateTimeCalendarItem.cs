using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls
{
    public class DateTimeCalendarItem : Button
    {
        public DateTime Value
        {
            get { return (DateTime)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(DateTime), typeof(DateTimeCalendarItem), new PropertyMetadata(DateTime.MinValue));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(DateTimeCalendarItem), new PropertyMetadata(string.Empty));

        public string? Subtitle
        {
            get { return (string?)GetValue(SubtitleProperty); }
            set { SetValue(SubtitleProperty, value); }
        }
        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register("Subtitle", typeof(string), typeof(DateTimeCalendarItem), new PropertyMetadata(null));

        public bool IsFirstDayOfLunisolarMonth
        {
            get { return (bool)GetValue(IsFirstDayOfLunisolarMonthProperty); }
            set { SetValue(IsFirstDayOfLunisolarMonthProperty, value); }
        }

        public static readonly DependencyProperty IsFirstDayOfLunisolarMonthProperty =
             DependencyProperty.Register("IsFirstDayOfLunisolarMonth", typeof(bool), typeof(DateTimeCalendarItem), new PropertyMetadata(false));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(DateTimeCalendarItem), new PropertyMetadata(false));

        public bool IsCurrent
        {
            get { return (bool)GetValue(IsCurrentProperty); }
            set { SetValue(IsCurrentProperty, value); }
        }
        public static readonly DependencyProperty IsCurrentProperty =
            DependencyProperty.Register("IsCurrent", typeof(bool), typeof(DateTimeCalendarItem), new PropertyMetadata(false));

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(DateTimeCalendarItem), new PropertyMetadata(true));

        public bool IsDisplaying
        {
            get { return (bool)GetValue(IsDisplayingProperty); }
            set { SetValue(IsDisplayingProperty, value); }
        }
        public static readonly DependencyProperty IsDisplayingProperty =
            DependencyProperty.Register("IsDisplaying", typeof(bool), typeof(DateTimeCalendarItem), new PropertyMetadata(false));
    }
}
