using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls
{
    public class DateSelector : Control
    {
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(DateSelector));

        public bool ShowYear
        {
            get { return (bool)GetValue(ShowYearProperty); }
            set { SetValue(ShowYearProperty, value); }
        }
        public static readonly DependencyProperty ShowYearProperty =
            DependencyProperty.Register("ShowYear", typeof(bool), typeof(DateSelector), new PropertyMetadata(true));

        public int Year
        {
            get { return (int)GetValue(YearProperty); }
            set { SetValue(YearProperty, value); }
        }
        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof(int), typeof(DateSelector),
                new FrameworkPropertyMetadata(2000, new PropertyChangedCallback(OnDateChanged), new CoerceValueCallback(CoerceYearValue)));

        private static object CoerceYearValue(DependencyObject d, object baseValue)
        {
            return Math.Clamp((int)baseValue, 1919, 2222);
        }

        public int Month
        {
            get { return (int)GetValue(MonthProperty); }
            set { SetValue(MonthProperty, value); }
        }

        public static readonly DependencyProperty MonthProperty =
            DependencyProperty.Register("Month", typeof(int), typeof(DateSelector),
                 new FrameworkPropertyMetadata(1, new PropertyChangedCallback(OnDateChanged), new CoerceValueCallback(CoerceMonthValue)));

        private static object CoerceMonthValue(DependencyObject d, object baseValue)
        {
            return Math.Clamp((int)baseValue, 1, 12);
        }

        public int Day
        {
            get { return (int)GetValue(DayProperty); }
            set { SetValue(DayProperty, value); }
        }
        public static readonly DependencyProperty DayProperty =
            DependencyProperty.Register("Day", typeof(int), typeof(DateSelector),
                 new FrameworkPropertyMetadata(1, new PropertyChangedCallback(OnDateChanged), new CoerceValueCallback(CoerceDayValue)));

        private static object CoerceDayValue(DependencyObject d, object baseValue)
        {
            var ds = (DateSelector)d;
            var v = (int)baseValue;
            var max = DateTime.DaysInMonth(ds.Year, ds.Month);
            return Math.Clamp(v, 1, max);
        }

        public DateOnly Date
        {
            get { return (DateOnly)GetValue(DateProperty); }
            set { SetValue(DateProperty, value); }
        }
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateOnly), typeof(DateSelector),
                new FrameworkPropertyMetadata(new DateOnly(2000, 1, 1), new PropertyChangedCallback(OnDateChanged)));

        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tp = (DateSelector)d;
            if (!tp.BeginEdit())
            {
                tp.EndEdit();
                return;
            }
            if (e.Property == YearProperty)
            {
                tp.Date = new DateOnly((int)e.NewValue, tp.Month, tp.Day);
            }
            else if (e.Property == MonthProperty)
            {
                tp.Date = new DateOnly(tp.Year, (int)e.NewValue, tp.Day);
            }
            else if (e.Property == DayProperty)
            {
                tp.Date = new DateOnly(tp.Year, tp.Month, (int)e.NewValue);
            }
            else if (e.Property == DateProperty)
            {
                var date = (DateOnly)e.NewValue;
                tp.Year = date.Year;
                tp.Month = date.Month;
                tp.Day = date.Day;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Date = DateOnly.FromDateTime(DateTime.Now);
        }


        private int _editCount;
        private bool BeginEdit()
        {
            _editCount++;
            return _editCount == 1;
        }
        private void EndEdit()
        {
            _editCount--;
        }
    }
}
