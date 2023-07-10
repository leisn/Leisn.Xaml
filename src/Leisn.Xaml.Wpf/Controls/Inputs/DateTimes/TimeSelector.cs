// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Leisn.Xaml.Wpf.Internals;

namespace Leisn.Xaml.Wpf.Controls
{
    public class TimeSelector : Control
    {
        public static readonly RoutedEvent TimeChangedEvent =
           EventManager.RegisterRoutedEvent(nameof(TimeChanged), RoutingStrategy.Bubble,
               typeof(RoutedPropertyChangedEventHandler<TimeOnly>), typeof(TimeSelector));
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<TimeOnly> TimeChanged
        {
            add => AddHandler(TimeChangedEvent, value);
            remove => RemoveHandler(TimeChangedEvent, value);
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(TimeSelector));

        public int Hour
        {
            get => (int)GetValue(HourProperty);
            set => SetValue(HourProperty, value);
        }
        public static readonly DependencyProperty HourProperty =
            DependencyProperty.Register("Hour", typeof(int), typeof(TimeSelector),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged), new CoerceValueCallback(CoerceHourValue)));

        private static object CoerceHourValue(DependencyObject d, object baseValue)
        {
            return Math.Clamp((int)baseValue, 0, 23);
        }

        public int Minute
        {
            get => (int)GetValue(MinuteProperty);
            set => SetValue(MinuteProperty, value);
        }

        public static readonly DependencyProperty MinuteProperty =
            DependencyProperty.Register("Minute", typeof(int), typeof(TimeSelector),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged), new CoerceValueCallback(CoerceMinuteAndSecondValue)));

        private static object CoerceMinuteAndSecondValue(DependencyObject d, object baseValue)
        {
            return Math.Clamp((int)baseValue, 0, 59);
        }

        public int Second
        {
            get => (int)GetValue(SecondProperty);
            set => SetValue(SecondProperty, value);
        }
        public static readonly DependencyProperty SecondProperty =
            DependencyProperty.Register("Second", typeof(int), typeof(TimeSelector),
                 new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged), new CoerceValueCallback(CoerceMinuteAndSecondValue)));

        public bool ShowSecond
        {
            get => (bool)GetValue(ShowSecondProperty);
            set => SetValue(ShowSecondProperty, value);
        }
        public static readonly DependencyProperty ShowSecondProperty =
            DependencyProperty.Register("ShowSecond", typeof(bool), typeof(TimeSelector), new PropertyMetadata(true));

        public TimeOnly Time
        {
            get => (TimeOnly)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register("Time", typeof(TimeOnly), typeof(TimeSelector),
                new FrameworkPropertyMetadata(TimeOnly.MinValue, new PropertyChangedCallback(OnTimeChanged)));

        private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TimeSelector tp = (TimeSelector)d;
            if (!EditorUtil.BeginEdit(tp))
            {
                EditorUtil.EndEdit(tp);
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
                TimeOnly time = (TimeOnly)e.NewValue;
                tp.Hour = time.Hour;
                tp.Minute = time.Minute;
                tp.Second = time.Second;
                tp.RaiseEvent(new RoutedPropertyChangedEventArgs<TimeOnly>((TimeOnly)e.OldValue, time, TimeChangedEvent));
            }
        }

    }
}
