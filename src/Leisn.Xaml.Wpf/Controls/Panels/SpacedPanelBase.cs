// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls
{
    public abstract class SpacedPanelBase : Panel
    {
        [Bindable(true), Category("Layout")]
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }
        public static readonly DependencyProperty OrientationProperty =
           System.Windows.Controls.StackPanel.OrientationProperty.AddOwner(typeof(SpacedPanelBase),
                new FrameworkPropertyMetadata(Orientation.Vertical,
                    FrameworkPropertyMetadataOptions.AffectsMeasure,
                    new PropertyChangedCallback(OnOrientationChanged)));

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel= (SpacedPanelBase)d;
            panel.OnOrientationChanged();
        }

        [Bindable(true), Category("Layout")]
        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }
        public static readonly DependencyProperty PaddingProperty =
            Control.PaddingProperty.AddOwner(typeof(SpacedPanelBase),
                new FrameworkPropertyMetadata(new Thickness(),
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        [Bindable(true), Category("Layout")]
        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(SpacedPanelBase),
                new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));

        [Bindable(true), Category("Layout")]
        public double HorizontalSpacing
        {
            get => (double)GetValue(HorizontalSpacingProperty);
            set => SetValue(HorizontalSpacingProperty, value);
        }
        public static readonly DependencyProperty HorizontalSpacingProperty =
            DependencyProperty.Register("HorizontalSpacing", typeof(double), typeof(SpacedPanelBase),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        [Bindable(true), Category("Layout")]
        public double VerticalSpacing
        {
            get => (double)GetValue(VerticalSpacingProperty);
            set => SetValue(VerticalSpacingProperty, value);
        }
        public static readonly DependencyProperty VerticalSpacingProperty =
            DependencyProperty.Register("VerticalSpacing", typeof(double), typeof(SpacedPanelBase),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));

        internal double FinalHorizontalSpacing => double.IsNaN(Spacing) || Spacing == 0 ? HorizontalSpacing : Spacing;
        internal double FinalVerticalSpacing => double.IsNaN(Spacing) || Spacing == 0 ? VerticalSpacing : Spacing;

        protected virtual void OnOrientationChanged()
        {
        }
    }
}
