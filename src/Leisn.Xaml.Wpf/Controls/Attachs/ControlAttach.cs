// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

using Leisn.Xaml.Wpf.Converters;

namespace Leisn.Xaml.Wpf.Controls
{
    public class ControlAttach
    {
        #region about text
        public static bool GetShowClear(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowClearProperty);
        }
        public static void SetShowClear(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowClearProperty, value);
        }
        public static readonly DependencyProperty ShowClearProperty =
            DependencyProperty.RegisterAttached("ShowClear", typeof(bool), typeof(ControlAttach), new PropertyMetadata(true));

        public static string GetPlaceholder(DependencyObject obj)
        {
            return (string)obj.GetValue(PlaceholderProperty);
        }
        public static void SetPlaceholder(DependencyObject obj, string value)
        {
            obj.SetValue(PlaceholderProperty, value);
        }
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached("Placeholder", typeof(string), typeof(ControlAttach), new PropertyMetadata(null));
        #endregion

        #region layout
        public static Thickness GetPadding(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(PaddingProperty);
        }

        public static void SetPadding(DependencyObject obj, Thickness value)
        {
            obj.SetValue(PaddingProperty, value);
        }
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.RegisterAttached("Padding", typeof(Thickness), typeof(ControlAttach), new PropertyMetadata(new Thickness()));
        #endregion

        #region border
        public static Thickness GetBorderThickness(DependencyObject obj)
        {
            return (Thickness)obj.GetValue(BorderThicknessProperty);
        }
        public static void SetBorderThickness(DependencyObject obj, Thickness value)
        {
            obj.SetValue(BorderThicknessProperty, value);
        }
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.RegisterAttached("BorderThickness", typeof(Thickness), typeof(ControlAttach),
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
            DependencyProperty.RegisterAttached("BorderBrush", typeof(Brush), typeof(ControlAttach),
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
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlAttach),
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
            DependencyProperty.RegisterAttached("IsCircle", typeof(bool), typeof(ControlAttach),
                 new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits, new PropertyChangedCallback(OnIsCircleChanged)));

        private static void OnIsCircleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not Border border)
            {
                return;
            }

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
        #endregion

        public static bool GetEnterMoveFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnterMoveFocusProperty);
        }
        public static void SetEnterMoveFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(EnterMoveFocusProperty, value);
        }
        public static readonly DependencyProperty EnterMoveFocusProperty =
            DependencyProperty.RegisterAttached("EnterMoveFocus", typeof(bool), typeof(ControlAttach),
                new PropertyMetadata(false, new PropertyChangedCallback(OnEnterMoveFocusChanged)));
        private static void OnEnterMoveFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var fe = (FrameworkElement)d;
            if ((bool)e.NewValue)
            {
                fe.KeyDown += OnEnterDown;
            }
            else
            {
                fe.KeyDown -= OnEnterDown;
            }
        }

        private static void OnEnterDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.FocusedElement is UIElement element)
                {
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
        }
    }
}
