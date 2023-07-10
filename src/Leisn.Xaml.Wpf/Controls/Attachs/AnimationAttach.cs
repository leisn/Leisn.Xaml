// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Leisn.Xaml.Wpf.Controls
{
    public class AnimationAttach
    {
        private static DoubleAnimationUsingKeyFrames BreatheAnimation { get; } = new()
        {
            RepeatBehavior = RepeatBehavior.Forever,
            Duration = new Duration(TimeSpan.FromSeconds(1.2)),
            KeyFrames = new DoubleKeyFrameCollection
            {
                new LinearDoubleKeyFrame(.3,KeyTime.FromPercent(.4)),
                new LinearDoubleKeyFrame(1,KeyTime.FromPercent(.6))
            },
        };

        public static bool GetBreathe(DependencyObject obj)
        {
            return (bool)obj.GetValue(BreatheProperty);
        }
        public static void SetBreathe(DependencyObject obj, bool value)
        {
            obj.SetValue(BreatheProperty, value);
        }

        public static readonly DependencyProperty BreatheProperty =
            DependencyProperty.RegisterAttached("Breathe", typeof(bool), typeof(AnimationAttach),
                new PropertyMetadata(false, new PropertyChangedCallback(OnBreatheChanged)));

        private static void OnBreatheChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control control = (Control)d;
            if ((bool)e.NewValue)
            {
                control.BeginAnimation(UIElement.OpacityProperty, BreatheAnimation);
            }
            else
            {
                control.BeginAnimation(UIElement.OpacityProperty, null);
            }
        }
    }
}
