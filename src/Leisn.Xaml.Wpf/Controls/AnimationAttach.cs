using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;

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
            var control = (Control)d;
            if (e.NewValue is bool b)
            {
                if (b)
                    control.BeginAnimation(UIElement.OpacityProperty, BreatheAnimation);
                else
                    control.BeginAnimation(UIElement.OpacityProperty, null);
            }
        }
    }
}
