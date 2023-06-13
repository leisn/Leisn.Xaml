// By Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Leisn.Xaml.Wpf.Controls
{
    /// <summary>
    /// LoadingRing.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressRing : UserControl
    {

        public ProgressRing()
        {
            InitializeComponent();
            _animation = (Storyboard)Resources["ProgressRingStoryboard"];
        }

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(ProgressRing),
                new PropertyMetadata(false, new PropertyChangedCallback(IsActiveChanged)));
        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        private static void IsActiveChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((ProgressRing)sender).onIsActiveChanged(Convert.ToBoolean(e.NewValue));
        }

        private void onIsActiveChanged(bool newValue)
        {
            if (newValue)
            {
                _animation.Begin();
            }
            else
            {
                _animation.Stop();
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            // force the ring to the largest square which is fully visible in the control
            Ring.Width = Math.Min(ActualWidth, ActualHeight);
            Ring.Height = Math.Min(ActualWidth, ActualHeight);
            base.OnRenderSizeChanged(sizeInfo);
        }

        private readonly Storyboard _animation;
    }
}
