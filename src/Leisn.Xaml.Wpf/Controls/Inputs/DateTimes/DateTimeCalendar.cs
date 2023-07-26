// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using Leisn.Common.Attributes;
using Leisn.Common.Media;
using Leisn.Xaml.Wpf.Extensions;
using Leisn.Xaml.Wpf.Locales;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = PART_HeaderButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_PreviousButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_NextButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_ItemHostName, Type = typeof(UniformGrid))]
    [TemplatePart(Name = PART_TimeItemHostName, Type = typeof(UniformGrid))]
    public class DateTimeCalendar : Control
    {
        const string PART_HeaderButtonName = "PART_HeaderButton";
        const string PART_PreviousButtonName = "PART_PreviousButton";
        const string PART_NextButtonName = "PART_NextButton";
        const string PART_ItemHostName = "PART_ItemHost";
        const string PART_TimeItemHostName = "PART_TimeItemHost";

        public static readonly DateTime MaxDateTime = new(2095, 12, 31);
        public static readonly DateTime MinDateTime = new(1905, 1, 1);

        private ButtonBase _headerButton = null!;
        private ButtonBase _previousButton = null!;
        private ButtonBase _nextButton = null!;
        private UniformGrid _itemHostGrid = null!;
        private UniformGrid _timeItemHostGrid = null!;
        private CalendarMode _dateDisplayMode = CalendarMode.Decade;
        private CalendarMode _timeDisplayMode = CalendarMode.Year;

        static DateTimeCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimeCalendar), new FrameworkPropertyMetadata(typeof(DateTimeCalendar)));
        }

        public static readonly RoutedEvent SelectedDateTimeChangedEvent =
          EventManager.RegisterRoutedEvent(nameof(SelectedDateTimeChanged), RoutingStrategy.Bubble,
              typeof(RoutedPropertyChangedEventHandler<DateTime>), typeof(DateTimeCalendar));
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<DateTime> SelectedDateTimeChanged
        {
            add => AddHandler(SelectedDateTimeChangedEvent, value);
            remove => RemoveHandler(SelectedDateTimeChangedEvent, value);
        }

        public DateTime SelectedDateTime
        {
            get { return (DateTime)GetValue(SelectedDateTimeProperty); }
            set { SetValue(SelectedDateTimeProperty, value); }
        }
        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime", typeof(DateTime), typeof(DateTimeCalendar),
                new FrameworkPropertyMetadata(DateTime.Now,
                    new PropertyChangedCallback(OnSelectedDateTimeChanged),
                    new CoerceValueCallback(CoerceSelectedDateValue)));

        private static object CoerceSelectedDateValue(DependencyObject d, object baseValue)
        {
            var time = (DateTime)baseValue;
            if (time < MinDateTime)
            {
                return MinDateTime.WithTime(time);
            }
            if (time > MaxDateTime)
            {
                return MaxDateTime.WithTime(time);
            }
            return time;
        }

        private static void OnSelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (DateTimeCalendar)d;
            ctrl.OnSelectedDateTimeChanged((DateTime)e.OldValue, (DateTime)e.NewValue);
        }

        internal DateTime DisplayDateTime
        {
            get { return (DateTime)GetValue(DisplayDateTimeProperty); }
            set { SetValue(DisplayDateTimeProperty, value); }
        }
        internal static readonly DependencyPropertyKey DisplayDateTimePropertyKey =
            DependencyProperty.RegisterReadOnly("DisplayDateTime", typeof(DateTime), typeof(DateTimeCalendar), new PropertyMetadata(DateTime.Now));
        internal static readonly DependencyProperty DisplayDateTimeProperty = DisplayDateTimePropertyKey.DependencyProperty;

        internal bool IsNowTimeView
        {
            get { return (bool)GetValue(IsNowTimeViewProperty); }
            set { SetValue(IsNowTimeViewProperty, value); }
        }
        internal static readonly DependencyProperty IsNowTimeViewProperty =
            DependencyProperty.Register("IsNowTimeView", typeof(bool), typeof(DateTimeCalendar),
                new PropertyMetadata(false, new PropertyChangedCallback(OnIsNowTimeViewChanged)));
        private static void OnIsNowTimeViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (DateTimeCalendar)d;
            ctrl.UpdateHeader();
        }

        public DateTimeSelectionMode SelectionMode
        {
            get { return (DateTimeSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(DateTimeSelectionMode), typeof(DateTimeCalendar),
                new FrameworkPropertyMetadata(DateTimeSelectionMode.DateTime, new PropertyChangedCallback(OnSelectionModeChanged)));

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (DateTimeCalendar)d;
            ctrl.OnSelectionModeChanged();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _headerButton = (ButtonBase)GetTemplateChild(PART_HeaderButtonName);
            _previousButton = (ButtonBase)GetTemplateChild(PART_PreviousButtonName);
            _nextButton = (ButtonBase)GetTemplateChild(PART_NextButtonName);
            _itemHostGrid = (UniformGrid)GetTemplateChild(PART_ItemHostName);
            _timeItemHostGrid = (UniformGrid)GetTemplateChild(PART_TimeItemHostName);

            _headerButton.Click += OnHeaderButtonClicked;
            _previousButton.Click += OnPreviousButtonClicked;
            _nextButton.Click += OnNextButtonClicked;

            InitView();
        }

        protected virtual void OnSelectionModeChanged()
        {
        }


        private void UpdateHeader()
        {
            if (_headerButton == null)
            {
                return;
            }

            if (IsNowTimeView)
            {
                UpdateTimeHeader();
            }
            else
            {
                UpdateDateHeader();
            }
        }

        internal void SetDisplayDateTime(DateTime display)
        {
            SetValue(DisplayDateTimePropertyKey, display);
        }

        private void SetDateDisplayMode(CalendarMode mode)
        {
            if (mode == _dateDisplayMode)
                return;
            _dateDisplayMode = mode;
            UpdateDateView(true);
        }

        private void SetTimeDisplayMode(CalendarMode mode)
        {
            if (mode == _timeDisplayMode)
                return;
            _timeDisplayMode = mode;
            UpdateTimeView(true);
        }

        protected virtual void OnSelectedDateTimeChanged(DateTime oldTime, DateTime newTime)
        {
            SetDisplayDateTime(newTime);
            UpdateDateView(false);
            UpdateTimeView(false);
            UpdateHeader();
            RaiseEvent(new RoutedPropertyChangedEventArgs<DateTime>(oldTime, newTime, SelectedDateTimeChangedEvent));
        }

        private void OnNextButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (_dateDisplayMode)
            {
                case CalendarMode.Year:
                    SetDisplayDateTime(DisplayDateTime.AddYears(10));
                    break;
                case CalendarMode.Month:
                    SetDisplayDateTime(DisplayDateTime.AddMonths(12));
                    break;
                case CalendarMode.Decade:
                    SetDisplayDateTime(DisplayDateTime.AddMonths(1));
                    break;
                default:
                    break;
            }
            UpdateDateView(true);
        }

        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            switch (_dateDisplayMode)
            {
                case CalendarMode.Year:
                    SetDisplayDateTime(DisplayDateTime.AddYears(-7));
                    break;
                case CalendarMode.Month:
                    SetDisplayDateTime(DisplayDateTime.AddMonths(-12));
                    break;
                case CalendarMode.Decade:
                    SetDisplayDateTime(DisplayDateTime.AddMonths(-1));
                    break;
                default:
                    break;
            }

            UpdateDateView(true);
        }

        private void OnHeaderButtonClicked(object sender, RoutedEventArgs e)
        {
            if (IsNowTimeView)
            {
                switch (_timeDisplayMode)
                {
                    case CalendarMode.Month:
                        SetTimeDisplayMode(CalendarMode.Decade);
                        break;
                    case CalendarMode.Year:
                        SetTimeDisplayMode(CalendarMode.Month);
                        break;
                    case CalendarMode.Decade:
                        SetTimeDisplayMode(CalendarMode.Year);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (_dateDisplayMode)
                {
                    case CalendarMode.Month:
                        SetDateDisplayMode(CalendarMode.Year);
                        break;
                    case CalendarMode.Year:
                        break;
                    case CalendarMode.Decade:
                        SetDateDisplayMode(CalendarMode.Month);
                        break;
                    default:
                        break;
                }
            }
        }

        const int DISPLAY_DAYS = 42;
        const int DISPLAY_SECONEDS = 60;
        private void InitView()
        {
            Lang.LangChanged -= OnLangChanged;
            SetDisplayDateTime(SelectedDateTime);
            for (int i = 0; i < 7; i++)
            {
                _itemHostGrid.Children.Add(new TextBlock()
                {
                    Style = (Style)FindResource("DateTimePickerWeekStyle")
                });
            }
            for (int i = 0; i < DISPLAY_DAYS; i++)
            {
                var item = new DateTimeCalendarItem();
                _itemHostGrid.Children.Add(item);
                item.Click += OnDateItemClicked;
            }
            for (int i = 0; i < DISPLAY_SECONEDS; i++)
            {
                var item = new DateTimeCalendarItem();
                _timeItemHostGrid.Children.Add(item);
                item.Click += OnTimeItemClicked;
            }
            OnLangChanged();
            Lang.LangChanged += OnLangChanged;
        }

        private DateTimeCalendarItem? _lastSelectedItem;
        private void OnDateItemClicked(object sender, RoutedEventArgs e)
        {
            if (_lastSelectedItem != null)
                _lastSelectedItem.IsSelected = false;
            var selected = (DateTimeCalendarItem)sender;
            selected.IsSelected = true;
            _lastSelectedItem = selected;

            var dateTime = DisplayDateTime;
            switch (_dateDisplayMode)
            {
                case CalendarMode.Year:
                    var newTime = dateTime.WithDate(selected.Value.Year, dateTime.Month, 1);
                    var days = newTime.GetDaysOfMonth();
                    SetDisplayDateTime(dateTime.WithDate(selected.Value.Year, dateTime.Month, Math.Min(dateTime.Day, days)));
                    SetDateDisplayMode(CalendarMode.Month);
                    break;
                case CalendarMode.Month:
                    newTime = dateTime.WithDate(selected.Value.Year, selected.Value.Month, 1);
                    days = newTime.GetDaysOfMonth();
                    SetDisplayDateTime(dateTime.WithDate(selected.Value.Year, selected.Value.Month, Math.Min(dateTime.Day, days)));
                    SetDateDisplayMode(CalendarMode.Decade);
                    break;
                case CalendarMode.Decade:
                    SetDisplayDateTime(dateTime.WithDate(selected.Value));
                    SelectedDateTime = DisplayDateTime;
                    break;
                default:
                    break;
            }
        }

        private DateTimeCalendarItem? _lastSelectedTimeItem;
        private void OnTimeItemClicked(object sender, RoutedEventArgs e)
        {
            if (_lastSelectedTimeItem != null)
                _lastSelectedTimeItem.IsSelected = false;
            var selected = (DateTimeCalendarItem)sender;
            selected.IsSelected = true;
            _lastSelectedTimeItem = selected;

            switch (_timeDisplayMode)
            {
                case CalendarMode.Year:
                    SetDisplayDateTime(DisplayDateTime.With(Hour: selected.Value.Hour));
                    SetTimeDisplayMode(CalendarMode.Month);
                    break;
                case CalendarMode.Month:
                    SetDisplayDateTime(DisplayDateTime.With(Minute: selected.Value.Minute));
                    SetTimeDisplayMode(CalendarMode.Decade);
                    break;
                case CalendarMode.Decade:
                    SetDisplayDateTime(DisplayDateTime.With(Second: selected.Value.Second));
                    SelectedDateTime = DisplayDateTime;
                    break;
                default:
                    break;
            }
        }

        private void OnLangChanged()
        {
            var dateFormat = Lang.CurrentCulture.DateTimeFormat;
            var names = dateFormat.ShortestDayNames;
            for (int i = 0; i < 7; i++)
            {
                var textBlock = (TextBlock)_itemHostGrid.Children[i];
                textBlock.Text = names[i];
            }
            UpdateDateView(false);
            UpdateTimeView(false);
            UpdateHeader();
        }

        private void UpdateDateView(bool updateHeader)
        {
            if (_itemHostGrid == null)
                return;
            switch (_dateDisplayMode)
            {
                case CalendarMode.Month:
                    UpdateMonthsView();
                    break;
                case CalendarMode.Year:
                    UpdateYearsView();
                    break;
                case CalendarMode.Decade:
                    UpdateDaysView();
                    break;
                default:
                    break;
            }
            if (updateHeader)
                UpdateHeader();
        }

        private void UpdateDateHeader()
        {
            if (_headerButton == null)
                return;
            _headerButton.IsEnabled = _dateDisplayMode is not CalendarMode.Year;

            DateTime start, end;
            var format = Lang.CurrentCulture.DateTimeFormat;
            switch (_dateDisplayMode)
            {
                case CalendarMode.Year:
                    start = ((DateTimeCalendarItem)_itemHostGrid.Children[7]).Value;
                    end = start.AddYears(15);
                    end = end.WithDate(end.Year, 12, 31);
                    _headerButton.Content = $"{start.Year.ToString(format)} - {end.Year.ToString(format)} ({GetYearString()}年)";
                    break;
                case CalendarMode.Month:
                    start = DisplayDateTime.With(Month: 1, Day: 1);
                    end = DisplayDateTime.With(Month: 12, Day: 31);
                    _headerButton.Content = $"{DisplayDateTime.Year.ToString(format)} ({GetYearString()}年)";
                    break;

                case CalendarMode.Decade:
                    var pickerItem = (DateTimeCalendarItem)_itemHostGrid.Children[7];
                    start = pickerItem.Value;
                    if (pickerItem.Visibility != Visibility.Visible)
                    {
                        for (int i = 8; i < _itemHostGrid.Children.Count; i++)
                        {
                            pickerItem = (DateTimeCalendarItem)_itemHostGrid.Children[i];
                            if (pickerItem.Visibility == Visibility.Visible)
                            {
                                start = pickerItem.Value;
                                break;
                            }
                        }
                    }
                    end = ((DateTimeCalendarItem)_itemHostGrid.Children[48]).Value;
                    _headerButton.Content = $"{DisplayDateTime.ToString(format.YearMonthPattern)} ({GetYearString()}年)";
                    break;
            }
            string GetYearString()
            {
                var yearString = start.GetLunisolarYear();
                var endYearString = end.GetLunisolarYear();
                if (!Equals(yearString, endYearString))
                {
                    yearString = string.Concat(yearString, " - ", endYearString);
                }
                return yearString;
            }
        }
        private void UpdateYearsView()
        {
            var year = Math.Max(DisplayDateTime.Year - 6, MinDateTime.Year);
            _previousButton.IsEnabled = DisplayDateTime.Year - 6 > MinDateTime.Year;
            var endYear = year + 15;
            if (endYear > MaxDateTime.Year)
            {
                year = MaxDateTime.Year - 15;
                _nextButton.IsEnabled = false;
            }
            else
            {
                _nextButton.IsEnabled = true;
            }
            var start = new DateTime(year, 1, 1);
            var format = Lang.CurrentCulture.DateTimeFormat;
            DateTime end, now = DateTime.Now.Date;
            for (int i = 0; i < 16; i++)
            {
                end = start.AddYears(i);
                var yearString = end.GetLunisolarYear();
                var endYearString = end.WithDate(end.Year, 12, 31).GetLunisolarYear();
                if (!Equals(yearString, endYearString))
                {
                    yearString = string.Concat(yearString, " - ", endYearString);
                }
                var item = (DateTimeCalendarItem)_itemHostGrid.Children[i + 7];
                item.Value = end;
                item.Title = end.Year.ToString(format);
                item.Subtitle = string.Concat(yearString, "年");
                item.IsFirstDayOfLunisolarMonth = false;
                item.IsCurrent = now.Year == end.Year;
                item.IsActive = true;
                item.IsDisplaying = DisplayDateTime.Year == end.Year;
                item.IsSelected = SelectedDateTime.Year == end.Year;
            }

            _itemHostGrid.Rows = 4;
            _itemHostGrid.Columns = 4;
            for (int i = 0; i < 49; i++)
            {
                var control = _itemHostGrid.Children[i];
                control.Visibility = i is < 7 or > 22 ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        private void UpdateMonthsView()
        {
            var start = DisplayDateTime.WithDate(DisplayDateTime.Year, 1, 1); ;
            if (DisplayDateTime.Year == MinDateTime.Year)
            {
                _previousButton.IsEnabled = false;
            }
            else
            {
                start = start.AddMonths(-2);
                _previousButton.IsEnabled = true;
            }
            if (DisplayDateTime.Year == MaxDateTime.Year)
            {
                _nextButton.IsEnabled = false;
                start = DisplayDateTime.WithDate(DisplayDateTime.Year, 12, 1).AddMonths(-15);
            }
            else
            {
                _nextButton.IsEnabled = true;
            }
            var format = Lang.CurrentCulture.DateTimeFormat;
            var monthNames = format.AbbreviatedMonthNames;
            DateTime end, now = DateTime.Now.Date;
            for (int i = 0; i < 16; i++)
            {
                end = start.AddMonths(i);
                var mouthString = end.GetLunisolarMonth();
                var lastString = end.AddMonths(1).AddDays(-1).GetLunisolarMonth();
                if (!Equals(mouthString, lastString))
                {
                    mouthString = string.Concat(mouthString, "月 - ", lastString, "月");
                }

                var item = (DateTimeCalendarItem)_itemHostGrid.Children[i + 7];
                item.Value = end;
                item.Title = monthNames != null && monthNames.Length > 0 ?
                    monthNames[(end.Month - 1) % monthNames.Length] : end.Month.ToString(format);
                item.Subtitle = mouthString;
                item.IsFirstDayOfLunisolarMonth = false;
                item.IsCurrent = now.Year == end.Year && now.Month == end.Month;
                item.IsActive = DisplayDateTime.Year == end.Year;
                item.IsDisplaying = DisplayDateTime.Year == end.Year && DisplayDateTime.Month == end.Month;
                item.IsSelected = SelectedDateTime.Year == end.Year && SelectedDateTime.Month == end.Month;
            }

            _itemHostGrid.Rows = 4;
            _itemHostGrid.Columns = 4;
            for (int i = 0; i < 49; i++)
            {
                var control = _itemHostGrid.Children[i];
                control.Visibility = i is < 7 or > 22 ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        private void UpdateDaysView()
        {
            var (start, startIndex, length, hiddenStart) = GetDisplayDaysFromMonth(DisplayDateTime);
            DateTime end, now = DateTime.Now.Date;
            var endIndex = startIndex + length;
            for (int i = 0; i < DISPLAY_DAYS; i++)
            {
                var item = (DateTimeCalendarItem)_itemHostGrid.Children[i + 7];
                item.Visibility = Visibility.Visible;
                if (hiddenStart && i < startIndex)
                {
                    item.Visibility = Visibility.Hidden;
                    continue;
                }

                end = start.AddDays(i);
                item.Value = end;
                item.Title = end.Day.ToString();
                item.Subtitle = end.GetLunisolarDay(true);
                item.IsFirstDayOfLunisolarMonth = end.IsFirstDayOfLunisolarMonth();
                item.IsCurrent = Equals(now, end.Date);
                item.IsActive = i >= startIndex && i < endIndex;
                item.IsDisplaying = Equals(DisplayDateTime.Date, end.Date);
                item.IsSelected = Equals(SelectedDateTime.Date, end.Date);
            }
            _itemHostGrid.Rows = 7;
            _itemHostGrid.Columns = 7;
            for (int i = 0; i < 7; i++)
            {
                _itemHostGrid.Children[i].Visibility = Visibility.Visible;
            }
        }
        private (DateTime Start, int StartIndex, int Length, bool HiddenStart) GetDisplayDaysFromMonth(DateTime date)
        {
            var firstDay = date.FirstDayOfMonth();
            var lastDay = date.LastDayOfMonth();
            var dayInMonth = date.GetDaysOfMonth();
            if (lastDay.Date.AddDays(7) > MaxDateTime)
            {
                var first = MaxDateTime.AddDays(-41).WithTime(firstDay);
                _nextButton.IsEnabled = false;
                return (first, 11, dayInMonth, false);
            }
            _nextButton.IsEnabled = true;
            var firstWeek = (int)firstDay.DayOfWeek;
            DateTime start = firstDay;
            bool hiddenStart = false;
            if (firstWeek > 0)
            {
                var min = MinDateTime.WithTime(start);
                if (min.AddDays(firstWeek) > start)
                {
                    start = min;
                    hiddenStart = true;
                }
                else
                {
                    start = firstDay.AddDays(-firstWeek);
                }
            }
            _previousButton.IsEnabled = firstDay.AddMonths(-1) >= MinDateTime;
            return (start, firstWeek, dayInMonth, hiddenStart);
        }

        #region time view
        private void UpdateTimeView(bool updateHeader)
        {
            if (_timeItemHostGrid == null)
                return;
            switch (_timeDisplayMode)
            {
                case CalendarMode.Year:
                    UpdateHoursView();
                    break;
                case CalendarMode.Month:
                    UpdateMinutesView();
                    break;
                case CalendarMode.Decade:
                    UpdateSecondsView();
                    break;
                default:
                    break;
            }
            if (updateHeader)
                UpdateHeader();
        }

        private void UpdateTimeHeader()
        {
            if (_headerButton == null)
                return;
            _headerButton.IsEnabled = true;
            var format = Lang.CurrentCulture.DateTimeFormat;
            var dateString = string.Concat(DisplayDateTime.ToString(format.YearMonthPattern), DisplayDateTime.Day, Equals("zh-cn", Lang.CurrentLanguage) ? "日" : "");
            switch (_timeDisplayMode)
            {
                case CalendarMode.Year:
                    _headerButton.Content = $"{dateString}  [{DisplayDateTime.Hour}] :{DisplayDateTime.Minute}:{DisplayDateTime.Second}";
                    break;
                case CalendarMode.Month:
                    _headerButton.Content = $"{dateString}  {DisplayDateTime.Hour}: [{DisplayDateTime.Minute}] :{DisplayDateTime.Second}";
                    break;
                case CalendarMode.Decade:
                    _headerButton.Content = $"{dateString}  {DisplayDateTime.Hour}:{DisplayDateTime.Minute}: [{DisplayDateTime.Second}]";
                    break;
                default:
                    break;
            }
        }
        private void UpdateHoursView()
        {
            var start = DisplayDateTime.With(Hour: 0);
            var now = DateTime.Now;
            var format = Lang.CurrentCulture.DateTimeFormat;
            DateTime end;
            for (int i = 0; i < 24; i++)
            {
                end = start.AddHours(i);
                var item = (DateTimeCalendarItem)_timeItemHostGrid.Children[i];
                item.Value = end;
                item.Title = end.Hour.ToString(format);
                item.Subtitle = null;
                item.IsFirstDayOfLunisolarMonth = false;
                item.IsCurrent = now.Hour == end.Hour;
                item.IsActive = true;
                item.IsDisplaying = DisplayDateTime.Hour == end.Hour;
                item.IsSelected = SelectedDateTime.Hour == end.Hour;
            }
            _timeItemHostGrid.Rows = 5;
            _timeItemHostGrid.Columns = 5;
            for (int i = 24; i < _timeItemHostGrid.Children.Count; i++)
            {
                var control = _timeItemHostGrid.Children[i];
                control.Visibility = Visibility.Collapsed;
            }
        }
        private void UpdateMinutesView()
        {
            var start = DisplayDateTime.With(Minute: 0);
            var now = DateTime.Now;
            var format = Lang.CurrentCulture.DateTimeFormat;
            DateTime end;
            for (int i = 0; i < DISPLAY_SECONEDS; i++)
            {
                end = start.AddMinutes(i);
                var item = (DateTimeCalendarItem)_timeItemHostGrid.Children[i];
                item.Visibility = Visibility.Visible;
                item.Value = end;
                item.Title = end.Minute.ToString(format);
                item.Subtitle = null;
                item.IsFirstDayOfLunisolarMonth = false;
                item.IsCurrent = now.Minute == end.Minute;
                item.IsActive = true;
                item.IsDisplaying = DisplayDateTime.Minute == end.Minute;
                item.IsSelected = SelectedDateTime.Minute == end.Minute;
            }
            _timeItemHostGrid.Rows = 8;
            _timeItemHostGrid.Columns = 8;
        }
        private void UpdateSecondsView()
        {
            var start = DisplayDateTime.With(Second: 0);
            var now = DateTime.Now;
            var format = Lang.CurrentCulture.DateTimeFormat;
            DateTime end;
            for (int i = 0; i < DISPLAY_SECONEDS; i++)
            {
                end = start.AddSeconds(i);
                var item = (DateTimeCalendarItem)_timeItemHostGrid.Children[i];
                item.Visibility = Visibility.Visible;
                item.Value = end;
                item.Title = end.Second.ToString(format);
                item.Subtitle = null;
                item.IsFirstDayOfLunisolarMonth = false;
                item.IsCurrent = now.Second == end.Second;
                item.IsActive = true;
                item.IsDisplaying = DisplayDateTime.Second == end.Second;
                item.IsSelected = SelectedDateTime.Second == end.Second;
            }
            _timeItemHostGrid.Rows = 8;
            _timeItemHostGrid.Columns = 8;
        }
        #endregion
    }
}
