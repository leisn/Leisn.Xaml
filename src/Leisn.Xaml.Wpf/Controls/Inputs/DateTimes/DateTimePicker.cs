// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

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
    public class DateTimePicker : Control
    {


        const string PART_HeaderButtonName = "PART_HeaderButton";
        const string PART_PreviousButtonName = "PART_PreviousButton";
        const string PART_NextButtonName = "PART_NextButton";
        const string PART_ItemHostName = "PART_ItemHost";
        const string PART_TimeItemHostName = "PART_TimeItemHost";

        private ButtonBase _headerButton = null!;
        private ButtonBase _previousButton = null!;
        private ButtonBase _nextButton = null!;
        private UniformGrid _itemHostGrid = null!;
        private UniformGrid _timeItemHostGrid = null!;
        private CalendarMode _dateDisplayMode = CalendarMode.Decade;
        private CalendarMode _timeDisplayMode = CalendarMode.Year;

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
        }

        public static readonly RoutedEvent SelectedDateTimeChangedEvent =
          EventManager.RegisterRoutedEvent(nameof(SelectedDateTimeChanged), RoutingStrategy.Bubble,
              typeof(RoutedPropertyChangedEventHandler<Hsv>), typeof(DateTimePicker));
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<Hsv> SelectedDateTimeChanged
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
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedDateTimeChanged)));

        private static void OnSelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (DateTimePicker)d;
            ctrl.OnSelectedDateTimeChanged(e.OldValue as DateTime?, e.NewValue as DateTime?);
        }

        public DateTime DisplayDateTime
        {
            get { return (DateTime)GetValue(DisplayDateTimeProperty); }
            set { SetValue(DisplayDateTimeProperty, value); }
        }
        internal static readonly DependencyPropertyKey DisplayDateTimePropertyKey =
            DependencyProperty.RegisterReadOnly("DisplayDateTime", typeof(DateTime), typeof(DateTimePicker), new PropertyMetadata(DateTime.Now));
        public static readonly DependencyProperty DisplayDateTimeProperty = DisplayDateTimePropertyKey.DependencyProperty;

        internal bool IsNowTimeView
        {
            get { return (bool)GetValue(IsNowTimeViewProperty); }
            private set { SetValue(IsNowTimeViewProperty, value); }
        }
        public static readonly DependencyProperty IsNowTimeViewProperty =
            DependencyProperty.Register("IsNowTimeView", typeof(bool), typeof(DateTimePicker),
                new PropertyMetadata(false, new PropertyChangedCallback(OnIsNowTimeViewChanged)));

        private static void OnIsNowTimeViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (DateTimePicker)d;
            ctrl.OnIsNowTimeViewChanged();
        }

        private void OnIsNowTimeViewChanged()
        {
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
            UpdateDateView();
        }

        private void SetTimeDisplayMode(CalendarMode mode)
        {
            if (mode == _timeDisplayMode)
                return;
            _timeDisplayMode = mode;
            UpdateTimeView();
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

        protected virtual void OnSelectedDateTimeChanged(DateTime? oldTime, DateTime? newTime)
        {
            SetDisplayDateTime(newTime ?? (DisplayDateTime.Equals(DateTime.MinValue) ? DateTime.Now : DisplayDateTime));
            UpdateDateView();
            RaiseEvent(new RoutedPropertyChangedEventArgs<DateTime?>(oldTime, newTime, SelectedDateTimeChangedEvent));
        }

        private void OnNextButtonClicked(object sender, RoutedEventArgs e)
        {
            SetDisplayDateTime(DisplayDateTime.AddMonths(1));
            UpdateDateView();
        }

        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            SetDisplayDateTime(DisplayDateTime.AddMonths(-1));
            UpdateDateView();
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
            SetDisplayDateTime(SelectedDateTime ?? DateTime.Now);
            for (int i = 0; i < 7; i++)
            {
                _itemHostGrid.Children.Add(new TextBlock()
                {
                    Style = (Style)FindResource("DateTimePickerWeekStyle")
                });
            }
            for (int i = 0; i < DISPLAY_DAYS; i++)
            {
                var item = new DateTimePickerItem();
                _itemHostGrid.Children.Add(item);
                item.Click += OnDateItemClicked;
            }
            OnLangChanged();
            Lang.LangChanged += OnLangChanged;

            for (int i = 0; i < DISPLAY_SECONEDS; i++)
            {
                var item = new DateTimePickerItem();
                _itemHostGrid.Children.Add(item);
                item.Click += OnTimeItemClicked;
            }
        }

        private DateTimePickerItem? _lastSelectedItem;
        private void OnDateItemClicked(object sender, RoutedEventArgs e)
        {
            if (_lastSelectedItem != null)
                _lastSelectedItem.IsSelected = false;
            var selected = (DateTimePickerItem)sender;
            selected.IsSelected = true;
            _lastSelectedItem = selected;

            var dateTime = SelectedDateTime ?? DisplayDateTime;
            switch (_dateDisplayMode)
            {
                case CalendarMode.Year:
                    SetDisplayDateTime(dateTime.WithDate(selected.Value.Year, dateTime.Month, dateTime.Day));
                    SetDateDisplayMode(CalendarMode.Month);
                    break;
                case CalendarMode.Month:
                    SetDisplayDateTime(dateTime.WithDate(selected.Value.Year, selected.Value.Month, dateTime.Day));
                    SetDateDisplayMode(CalendarMode.Decade);
                    break;
                case CalendarMode.Decade:
                    SelectedDateTime = dateTime.WithDate(selected.Value);
                    SetDisplayDateTime(SelectedDateTime.Value);
                    break;
                default:
                    break;
            }
        }

        private DateTimePickerItem? _lastSelectedTimeItem;
        private void OnTimeItemClicked(object sender, RoutedEventArgs e)
        {
            if (_lastSelectedTimeItem != null)
                _lastSelectedTimeItem.IsSelected = false;
            var selected = (DateTimePickerItem)sender;
            selected.IsSelected = true;
            _lastSelectedTimeItem = selected;

            var dateTime = SelectedDateTime ?? DisplayDateTime;
            switch (_dateDisplayMode)
            {
                case CalendarMode.Year:
                    SetTimeDisplayMode(CalendarMode.Month);
                    break;
                case CalendarMode.Month:
                    SetTimeDisplayMode(CalendarMode.Decade);
                    break;
                case CalendarMode.Decade:
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
            UpdateDateView();
        }

        private void UpdateDateView()
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
            _headerButton.IsEnabled = _dateDisplayMode is not CalendarMode.Year;
        }

        private void UpdateDateHeader()
        {

        }

        private void UpdateDaysView()
        {
            var (start, startIndex, length) = GetDisplayDaysFromMonth(DisplayDateTime);
            var now = DateTime.Now.Date;
            var endIndex = startIndex + length;
            DateTime end = start;
            for (int i = 0; i < DISPLAY_DAYS; i++)
            {
                end = start.AddDays(i);
                var item = (DateTimePickerItem)_itemHostGrid.Children[i + 7];
                item.Value = end;
                item.Title = end.Day.ToString();
                item.Subtitle = end.GetLunisolarDay(true);
                item.IsFirstDayOfLunisolarMonth = end.IsFirstDayOfLunisolarMonth();
                item.IsCurrent = Equals(now, end.Date);
                item.IsActive = i >= startIndex && i < endIndex;
                item.IsDisplaying = Equals(DisplayDateTime.Date, end.Date);
                item.IsSelected = Equals(SelectedDateTime?.Date, end.Date);
            }
            var yearString = start.GetLunisolarYear();
            var endYearString = end.GetLunisolarYear();
            if (!Equals(yearString, endYearString))
            {
                yearString = string.Concat(yearString, " - ", endYearString);
            }
            _headerButton.Content = $"{DisplayDateTime.ToString(Lang.CurrentCulture.DateTimeFormat.YearMonthPattern)} ({yearString}年)";
            _itemHostGrid.Rows = 7;
            _itemHostGrid.Columns = 7;
            for (int i = 0; i < 49; i++)
            {
                var control = _itemHostGrid.Children[i];
                control.Visibility = Visibility.Visible;
            }
        }
        private static (DateTime Start, int StartIndex, int Length) GetDisplayDaysFromMonth(DateTime date)
        {
            var firstDay = date.FirstDayOfMonth();
            var dayInMonth = date.GetDaysOfMonth();

            var firstWeek = firstDay.DayOfWeek;
            DateTime start = firstDay;
            if (firstWeek > 0)
            {
                start = firstDay.AddDays(-(int)firstWeek);
            }
            return (start, (int)firstWeek, dayInMonth);
        }

        private void UpdateMonthsView()
        {
            var start = DisplayDateTime.WithDate(DisplayDateTime.Year, 1, 1).AddMonths(-2);
            var end = DisplayDateTime.WithDate(DisplayDateTime.Year + 1, 2, DateTime.DaysInMonth(DisplayDateTime.Year + 1, 2));
            var yearString = start.GetLunisolarYear();
            var endYearString = end.GetLunisolarYear();
            if (!Equals(yearString, endYearString))
            {
                yearString = string.Concat(yearString, " - ", endYearString);
            }
            var format = Lang.CurrentCulture.DateTimeFormat;
            var monthNames = format.AbbreviatedMonthNames;
            _headerButton.Content = $"{DisplayDateTime.Year.ToString(format)} ({yearString}年)";
            var now = DateTime.Now.Date;
            for (int i = 0; i < 16; i++)
            {
                end = start.AddMonths(i);
                var mouthString = end.GetLunisolarMonth();
                var lastString = end.AddMonths(1).AddDays(-1).GetLunisolarMonth();
                if (!Equals(mouthString, lastString))
                {
                    mouthString = string.Concat(mouthString, "月 - ", lastString, "月");
                }

                var item = (DateTimePickerItem)_itemHostGrid.Children[i + 7];
                item.Value = end;
                item.Title = monthNames != null && monthNames.Length > 0 ?
                    monthNames[(end.Month - 1) % monthNames.Length] : end.Month.ToString(format);
                item.Subtitle = mouthString;
                item.IsFirstDayOfLunisolarMonth = false;
                item.IsCurrent = now.Year == end.Year && now.Month == end.Month;
                item.IsActive = DisplayDateTime.Year == end.Year;
                item.IsDisplaying = DisplayDateTime.Year == end.Year && DisplayDateTime.Month == end.Month;
                item.IsSelected = SelectedDateTime.HasValue && SelectedDateTime.Value.Year == end.Year
                    && SelectedDateTime.Value.Month == end.Month;
            }

            _itemHostGrid.Rows = 4;
            _itemHostGrid.Columns = 4;
            for (int i = 0; i < 49; i++)
            {
                var control = _itemHostGrid.Children[i];
                control.Visibility = i is < 7 or > 22 ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void UpdateYearsView()
        {
            var start = new DateTime(DisplayDateTime.Year - 6, 1, 1);
            var end = new DateTime(DisplayDateTime.Year + 9, 12, 31);
            var format = Lang.CurrentCulture.DateTimeFormat;
            var yearString = start.GetLunisolarYear();
            var endYearString = end.GetLunisolarYear();
            if (!Equals(yearString, endYearString))
            {
                yearString = string.Concat(yearString, " - ", endYearString);
            }
            _headerButton.Content = $"{start.Year.ToString(format)} - {end.Year.ToString(format)} ({yearString}年)";

            var now = DateTime.Now.Date;
            for (int i = 0; i < 16; i++)
            {
                end = start.AddYears(i);
                yearString = end.GetLunisolarYear();
                endYearString = end.WithDate(end.Year, 12, 31).GetLunisolarYear();
                if (!Equals(yearString, endYearString))
                {
                    yearString = string.Concat(yearString, " - ", endYearString);
                }
                var item = (DateTimePickerItem)_itemHostGrid.Children[i + 7];
                item.Value = end;
                item.Title = end.Year.ToString(format);
                item.Subtitle = string.Concat(yearString, "年");
                item.IsFirstDayOfLunisolarMonth = false;
                item.IsCurrent = now.Year == end.Year;
                item.IsActive = true;
                item.IsDisplaying = DisplayDateTime.Year == end.Year;
                item.IsSelected = SelectedDateTime.HasValue && SelectedDateTime.Value.Year == end.Year;
            }

            _itemHostGrid.Rows = 4;
            _itemHostGrid.Columns = 4;
            for (int i = 0; i < 49; i++)
            {
                var control = _itemHostGrid.Children[i];
                control.Visibility = i is < 7 or > 22 ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void UpdateTimeView()
        {

        }

        private void UpdateTimeHeader()
        {

        }
    }
}
