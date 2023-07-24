// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Leisn.Xaml.Wpf.Controls
{
    public enum DateTimeDisplayMode
    {
        Day, Month, Year, Hour, Minute, Second
    }

    [TemplatePart(Name = PART_HeaderButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_PreviousButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_NextButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_ItemHostName, Type = typeof(UniformGrid))]
    public class DateTimePicker : Control
    {
        const string PART_HeaderButtonName = "PART_HeaderButton";
        const string PART_PreviousButtonName = "PART_PreviousButton";
        const string PART_NextButtonName = "PART_NextButton";
        const string PART_ItemHostName = "PART_ItemHost";

        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
        }

        private ButtonBase _headerButton = null!;
        private ButtonBase _previousButton = null!;
        private ButtonBase _nextButton = null!;
        private UniformGrid _itemHostGrid = null!;

        public DateTime? SelectedDateTime
        {
            get { return (DateTime?)GetValue(SelectedDateTimeProperty); }
            set { SetValue(SelectedDateTimeProperty, value); }
        }
        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime", typeof(DateTime?), typeof(DateTimePicker), new PropertyMetadata(null));

        public DateTime DisplayDateTime
        {
            get { return (DateTime)GetValue(DisplayDateTimeProperty); }
            set { SetValue(DisplayDateTimeProperty, value); }
        }
        public static readonly DependencyProperty DisplayDateTimeProperty =
            DependencyProperty.Register("DisplayDateTime", typeof(DateTime), typeof(DateTimePicker), new PropertyMetadata(DateTime.Now));

        public DateTimeDisplayMode DisplayMode
        {
            get { return (DateTimeDisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register("DisplayMode", typeof(DateTimeDisplayMode), typeof(DateTimePicker),
                new FrameworkPropertyMetadata(DateTimeDisplayMode.Day, new PropertyChangedCallback(OnDisplayModeChanged)));

        private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (DateTimePicker)d;
            ctrl.UpdateView();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _headerButton = (ButtonBase)GetTemplateChild(PART_HeaderButtonName);
            _previousButton = (ButtonBase)GetTemplateChild(PART_PreviousButtonName);
            _nextButton = (ButtonBase)GetTemplateChild(PART_NextButtonName);
            _itemHostGrid = (UniformGrid)GetTemplateChild(PART_ItemHostName);

            _headerButton.Click += OnHeaderButtonClicked;
            _previousButton.Click += OnPreviousButtonClicked;
            _nextButton.Click += OnNextButtonClicked;

            _itemHostGrid.Rows = 7;
            _itemHostGrid.Columns = 7;

            var dateTime = SelectedDateTime ?? DateTime.Now;
            var week = new DateTime(dateTime.Year, dateTime.Month, 1);
            for (int i = 0; i < 60; i++)
            {
                _itemHostGrid.Children.Add(new DateTimePickerItem() { Value = i, Subtitle = i > 20 ? $"V{i + 1}" : null });
            }

        }


        private void OnNextButtonClicked(object sender, RoutedEventArgs e)
        {
        }

        private void OnPreviousButtonClicked(object sender, RoutedEventArgs e)
        {
        }

        private void OnHeaderButtonClicked(object sender, RoutedEventArgs e)
        {
        }

        private void UpdateView()
        {
            if (DisplayMode > DateTimeDisplayMode.Year)
            {
                _previousButton.Visibility = Visibility.Hidden;
                _nextButton.Visibility = Visibility.Hidden;
            }
            else
            {
                _previousButton.Visibility = Visibility.Visible;
                _nextButton.Visibility = Visibility.Visible;
            }
        }

    }
}
