// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Leisn.Common.Attributes;
using Leisn.Xaml.Wpf.Extensions;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = PART_ButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_ApplyButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_CloseButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_TextBlockName, Type = typeof(TextBlock))]
    [TemplatePart(Name = PART_CalendarName, Type = typeof(DateTimeCalendar))]
    [TemplatePart(Name = PART_PopupName, Type = typeof(Popup))]
    public class DateTimePicker : Control
    {
        const string PART_ButtonName = "PART_Button";
        const string PART_TextBlockName = "PART_TextBlock";
        const string PART_CalendarName = "PART_Calendar";
        const string PART_PopupName = "PART_Popup";
        const string PART_ApplyButtonName = "PART_ApplyButton";
        const string PART_CloseButtonName = "PART_CloseButton";
        const string DefaultFormat = "yyyy/MM/dd HH:mm:ss";
        const string DefaultDateFormat = "yyyy/MM/dd";
        const string DefaultTimeFormat = "HH:mm:ss";

        private ButtonBase _button = null!;
        private ButtonBase _applyButton = null!;
        private ButtonBase _closeButton = null!;
        private TextBlock? _textBlock;
        private DateTimeCalendar _calendar = null!;
        private Popup _popup = null!;

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
        }

        public static readonly RoutedEvent SelectedDateTimeChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(SelectedDateTimeChanged), RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<DateTime?>), typeof(DateTimePicker));

        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<DateTime?> SelectedDateTimeChanged
        {
            add => AddHandler(SelectedDateTimeChangedEvent, value);
            remove => RemoveHandler(SelectedDateTimeChangedEvent, value);
        }

        public DateTime? SelectedDateTime
        {
            get { return (DateTime?)GetValue(SelectedDateTimeProperty); }
            set { SetValue(SelectedDateTimeProperty, value); }
        }
        public static readonly DependencyProperty SelectedDateTimeProperty =
               DependencyProperty.Register("SelectedDateTime", typeof(DateTime?), typeof(DateTimePicker),
                  new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnSelectedDateTimeChanged),
                    new CoerceValueCallback(CoerceSelectedDateValue)));
        private static object CoerceSelectedDateValue(DependencyObject d, object baseValue)
        {
            if (baseValue is null)
                return baseValue!;
            var time = (DateTime)baseValue;
            if (time < DateTimeCalendar.MinDateTime)
            {
                return DateTimeCalendar.MinDateTime.WithTime(time);
            }
            if (time > DateTimeCalendar.MaxDateTime)
            {
                return DateTimeCalendar.MaxDateTime.WithTime(time);
            }
            return time;
        }

        private static void OnSelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DateTimePicker)d;
            control.OnSelectedDateTimeChanged(e.OldValue as DateTime?, e.NewValue as DateTime?);
        }


        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DateTimePicker), new PropertyMetadata(false));

        public string DateTimeFormatString
        {
            get { return (string)GetValue(DateTimeFormatStringProperty); }
            set { SetValue(DateTimeFormatStringProperty, value); }
        }
        public static readonly DependencyProperty DateTimeFormatStringProperty =
            DependencyProperty.Register("DateTimeFormatString", typeof(string), typeof(DateTimePicker), new PropertyMetadata(null));

        public DateTimeSelectionMode SelectionMode
        {
            get { return (DateTimeSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        public static readonly DependencyProperty SelectionModeProperty = DateTimeCalendar.SelectionModeProperty.AddOwner(typeof(DateTimePicker));


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _button = (ButtonBase)GetTemplateChild(PART_ButtonName);
            _popup = (Popup)GetTemplateChild(PART_PopupName);
            _calendar = (DateTimeCalendar)GetTemplateChild(PART_CalendarName);
            _textBlock = GetTemplateChild(PART_TextBlockName) as TextBlock;
            _applyButton = (ButtonBase)GetTemplateChild(PART_ApplyButtonName);
            _closeButton = (ButtonBase)GetTemplateChild(PART_CloseButtonName);

            _button.Click += OnButtonClicked;
            _applyButton.Click += OnApplyButtonClicked;
            _closeButton.Click += OnCloseButtonClicked;

            if (SelectedDateTime.HasValue)
                _calendar.SelectedDateTime = SelectedDateTime.Value;
            UpdateText();
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = false;
        }

        private void OnApplyButtonClicked(object sender, RoutedEventArgs e)
        {
            SelectedDateTime = _calendar.SelectedDateTime;
            IsDropDownOpen = false;
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            IsDropDownOpen = true;
        }

        protected virtual void OnSelectedDateTimeChanged(DateTime? oldTime, DateTime? newTime)
        {
            UpdateText();
            RaiseEvent(new RoutedPropertyChangedEventArgs<DateTime?>(oldTime, newTime, SelectedDateTimeChangedEvent));
        }

        private void UpdateText()
        {
            if (_button == null)
            {
                return;
            }
            var formart = DateTimeFormatString ??
                (SelectionMode == DateTimeSelectionMode.TimeOnly ?
                    DefaultTimeFormat : SelectionMode == DateTimeSelectionMode.DateOnly ?
                    DefaultDateFormat : DefaultFormat);
            var text = SelectedDateTime.HasValue ? SelectedDateTime.Value.ToString(formart) : string.Empty;
            if (_textBlock is not null)
                _textBlock.Text = text;
            else
                _button.Content = text;
        }
    }
}
