// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls
{
    public class TimePicker : Control
    {
        public int Hour
        {
            get { return (int)GetValue(HourProperty); }
            set { SetValue(HourProperty, value); }
        }
        public static readonly DependencyProperty HourProperty =
            DependencyProperty.Register("Hour", typeof(int), typeof(TimePicker),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged), new CoerceValueCallback(CoerceHourValue)));

        private static object CoerceHourValue(DependencyObject d, object baseValue)
        {
            var v = (int)baseValue;
            v = Math.Clamp(v, 0, 23);
            return v;
        }

        public int Minute
        {
            get { return (int)GetValue(MinuteProperty); }
            set { SetValue(MinuteProperty, value); }
        }

        public static readonly DependencyProperty MinuteProperty =
            DependencyProperty.Register("Minute", typeof(int), typeof(TimePicker),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged), new CoerceValueCallback(CoerceMinuteAndSecondValue)));

        private static object CoerceMinuteAndSecondValue(DependencyObject d, object baseValue)
        {
            var v = (int)baseValue;
            v = Math.Clamp(v, 0, 59);
            return v;
        }

        public int Second
        {
            get { return (int)GetValue(SecondProperty); }
            set { SetValue(SecondProperty, value); }
        }
        public static readonly DependencyProperty SecondProperty =
            DependencyProperty.Register("Second", typeof(int), typeof(TimePicker),
                 new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged), new CoerceValueCallback(CoerceMinuteAndSecondValue)));

        public bool ShowSecond
        {
            get { return (bool)GetValue(ShowSecondProperty); }
            set { SetValue(ShowSecondProperty, value); }
        }
        public static readonly DependencyProperty ShowSecondProperty =
            DependencyProperty.Register("ShowSecond", typeof(bool), typeof(TimePicker), new PropertyMetadata(true));

        public TimeOnly Time
        {
            get { return (TimeOnly)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(TimeOnly), typeof(TimePicker),
                new FrameworkPropertyMetadata(TimeOnly.MinValue, new PropertyChangedCallback(OnTimeChanged)));

        private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tp = (TimePicker)d;
            if (!tp.BeginEdit())
            {
                tp.EndEdit();
                return;
            }
            if (e.Property == HourProperty)
            {
                tp.Time = new TimeOnly((int)e.NewValue, tp.Time.Minute, tp.Time.Second);
            }
            else if (e.Property == MinuteProperty)
            {
                tp.Time = new TimeOnly(tp.Time.Hour, (int)e.NewValue, tp.Time.Second);
            }
            else if (e.Property == SecondProperty)
            {
                tp.Time = new TimeOnly(tp.Time.Hour, tp.Time.Minute, (int)e.NewValue);
            }
            else if (e.Property == TimeProperty)
            {
                var time = (TimeOnly)e.NewValue;
                tp.Hour = time.Hour;
                tp.Minute = time.Minute;
                tp.Second = time.Second;
            }
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
