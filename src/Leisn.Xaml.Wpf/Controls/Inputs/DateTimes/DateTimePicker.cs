// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

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
        private const string PART_ButtonName = "PART_Button";
        private const string PART_TextBlockName = "PART_TextBlock";
        private const string PART_CalendarName = "PART_Calendar";
        private const string PART_PopupName = "PART_Popup";
        private const string PART_ApplyButtonName = "PART_ApplyButton";
        private const string PART_CloseButtonName = "PART_CloseButton";
        private const string DefaultFormat = "yyyy/MM/dd HH:mm:ss";
        private const string DefaultDateFormat = "yyyy/MM/dd";
        private const string DefaultTimeFormat = "HH:mm:ss";

        private ButtonBase _button = null!;
        private ButtonBase _applyButton = null!;
        private ButtonBase _closeButton = null!;
        private TextBlock? _textBlock;
        private DateTimeCalendar _calendar = null!;
        private Popup _popup = null!;

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
            EventManager.RegisterClassHandler(typeof(DateTimePicker), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
            EventManager.RegisterClassHandler(typeof(DateTimePicker), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true); // call us even if the transparent button in the style gets the click.
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
            get => (DateTime?)GetValue(SelectedDateTimeProperty);
            set => SetValue(SelectedDateTimeProperty, value);
        }
        public static readonly DependencyProperty SelectedDateTimeProperty =
               DependencyProperty.Register("SelectedDateTime", typeof(DateTime?), typeof(DateTimePicker),
                  new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback(OnSelectedDateTimeChanged),
                    new CoerceValueCallback(CoerceSelectedDateValue)));
        private static object CoerceSelectedDateValue(DependencyObject d, object baseValue)
        {
            if (baseValue is null)
            {
                return baseValue!;
            }

            DateTime time = (DateTime)baseValue;
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
            DateTimePicker control = (DateTimePicker)d;
            control.OnSelectedDateTimeChanged(e.OldValue as DateTime?, e.NewValue as DateTime?);
        }


        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DateTimePicker),
                 new PropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged)));
        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DateTimePicker picker = (DateTimePicker)d;
            picker.OnIsDropDownChanged();
        }

        public string DateTimeFormatString
        {
            get => (string)GetValue(DateTimeFormatStringProperty);
            set => SetValue(DateTimeFormatStringProperty, value);
        }
        public static readonly DependencyProperty DateTimeFormatStringProperty =
            DependencyProperty.Register("DateTimeFormatString", typeof(string), typeof(DateTimePicker), new PropertyMetadata(null));

        public DateTimeSelectionMode SelectionMode
        {
            get => (DateTimeSelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }
        public static readonly DependencyProperty SelectionModeProperty = DateTimeCalendar.SelectionModeProperty.AddOwner(typeof(DateTimePicker));


        public override void OnApplyTemplate()
        {
            if (_button != null)
            {
                _button.Click -= OnButtonClicked;
            }
            if (_applyButton != null)
            {
                _applyButton.Click -= OnApplyButtonClicked;
            }
            if (_closeButton != null)
            {
                _closeButton.Click -= OnCloseButtonClicked;
            }

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
            {
                _calendar.SelectedDateTime = SelectedDateTime.Value;
            }

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
            IsDropDownOpen = !IsDropDownOpen;
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
            string formart = DateTimeFormatString ??
                (SelectionMode == DateTimeSelectionMode.TimeOnly ?
                    DefaultTimeFormat : SelectionMode == DateTimeSelectionMode.DateOnly ?
                    DefaultDateFormat : DefaultFormat);
            string text = SelectedDateTime.HasValue ? SelectedDateTime.Value.ToString(formart) : string.Empty;
            if (_textBlock is not null)
            {
                _textBlock.Text = text;
            }
            else
            {
                _button.Content = text;
            }
        }

        #region IsDropDownOpen Popup
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                IsDropDownOpen = false;
            }
            else if (e.Key == Key.Enter)
            {
                IsDropDownOpen = true;
            }
        }

        private void OnIsDropDownChanged()
        {
            if (IsDropDownOpen)
            {
                Mouse.Capture(this, CaptureMode.SubTree);
            }
            else if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
            }
        }


        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
            if (_popup.IsOpen && !IsKeyboardFocusWithin)
            {
                IsDropDownOpen = false;
            }
        }

        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            DateTimePicker editor = (DateTimePicker)sender;

            if (!editor.IsKeyboardFocusWithin)
            {
                editor.Focus();
            }
            e.Handled = true;

            if (Mouse.Captured == editor && e.OriginalSource == editor)
            {
                editor.IsDropDownOpen = false;
            }
        }

        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            DateTimePicker editor = (DateTimePicker)sender;
            IInputElement captured = Mouse.Captured;
            if (captured != editor)
            {
                if (e.OriginalSource == editor)
                {
                    if (captured == null || !editor.HasDescendant(captured as DependencyObject))
                    {
                        editor.IsDropDownOpen = false;
                    }
                }
                else
                {
                    if (editor.HasDescendant(e.OriginalSource as DependencyObject))
                    {
                        if (editor.IsDropDownOpen && captured == null)
                        {
                            Mouse.Capture(editor, CaptureMode.SubTree);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        editor.IsDropDownOpen = false;
                    }
                }
            }
        }
        #endregion
    }
}
