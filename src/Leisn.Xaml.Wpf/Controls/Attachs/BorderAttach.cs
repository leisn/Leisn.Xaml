// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using Leisn.Xaml.Wpf.Converters;

namespace Leisn.Xaml.Wpf.Controls
{
    public class BorderAttach
    {
        public static Thickness GetBorderThickness(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(BorderThicknessProperty);
        }
        public static void SetBorderThickness(DependencyObject obj, Thickness value)
        {
            obj.SetValue(BorderThicknessProperty, value);
        }
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.RegisterAttached("BorderThickness", typeof(Thickness), typeof(BorderAttach),
                new FrameworkPropertyMetadata(default(Thickness), FrameworkPropertyMetadataOptions.Inherits));

        public static Brush GetBorderBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(BorderBrushProperty);
        }

        public static void SetBorderBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(BorderBrushProperty, value);
        }
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.RegisterAttached("BorderBrush", typeof(Brush), typeof(BorderAttach),
                new FrameworkPropertyMetadata(default, FrameworkPropertyMetadataOptions.Inherits));

        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(BorderAttach),
                  new FrameworkPropertyMetadata(default(CornerRadius), FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetIsCircle(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCircleProperty);
        }

        public static void SetIsCircle(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCircleProperty, value);
        }

        public static readonly DependencyProperty IsCircleProperty =
            DependencyProperty.RegisterAttached("IsCircle", typeof(bool), typeof(BorderAttach),
                 new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIsCircleChanged)));

        private static void OnIsCircleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Border border)
                return;
            if ((bool)e.NewValue)
            {
                MultiBinding binding = new() { Converter = new BorderCircleConverter(), Mode = BindingMode.OneWay };
                binding.Bindings.Add(new Binding(FrameworkElement.ActualWidthProperty.Name) { Source = border });
                binding.Bindings.Add(new Binding(FrameworkElement.ActualHeightProperty.Name) { Source = border });
                BindingOperations.SetBinding(border, Border.CornerRadiusProperty, binding);
            }
            else
            {
                BindingOperations.ClearBinding(border, Border.CornerRadiusProperty);
            }
        }
    }
}
