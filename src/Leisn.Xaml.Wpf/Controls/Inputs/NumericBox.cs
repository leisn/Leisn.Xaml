// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Controls
{
    public enum NumericType
    {
        Int,
        Float,
        UInt,
        UFloat
    }

    public class NumericBox : TextBox
    {
        static NumericBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericBox), new FrameworkPropertyMetadata(typeof(NumericBox)));
        }

        public NumericType NumericType
        {
            get => (NumericType)GetValue(NumericTypeProperty);
            set => SetValue(NumericTypeProperty, value);
        }
        public static readonly DependencyProperty NumericTypeProperty =
            DependencyProperty.Register(nameof(NumericType), typeof(NumericType), typeof(NumericBox), new FrameworkPropertyMetadata(NumericType.Float, FrameworkPropertyMetadataOptions.Inherits));


        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            if (!IsKeyboardFocusWithin)
            {
                e.Handled = true;
                _ = Focus();
            }
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            SelectAll();
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (!AcceptInput(e.Text, Text, SelectionStart, SelectionLength, SelectedText))
            {
                e.Handled = true;
                FrameworkTextComposition? composition = e.TextComposition as FrameworkTextComposition;
                if (composition?.ResultLength > 0)//触发事件时，已完成输入
                {
                    Text = Text.Remove(composition.ResultOffset, composition.ResultLength);
                }
            }
        }

        protected virtual bool AcceptInput(string input, string originalText, int selectedIndex, int selectedLength, string seletedText)
        {
            bool allowSub = NumericType is NumericType.Int or NumericType.Float;
            bool allowDot = NumericType is NumericType.Float or NumericType.UFloat;

            string regex = string.Format(@"^{0}\d?{1}\d?$", allowSub ? "-?" : "", allowDot ? @"\.?" : "");
            if (!Regex.IsMatch(input, regex))
            {
                return false;
            }

            if (originalText != seletedText)
            {
                if (input.Contains('.'))
                {
                    if (!allowDot)
                    {
                        return false;
                    }

                    if (originalText.Contains('.'))//重复.
                    {
                        return false;
                    }
                }

                if (input.Contains('-'))
                {
                    if (!allowSub)
                    {
                        return false;
                    }

                    if (!string.IsNullOrEmpty(originalText) && selectedIndex > 0)//-号不在首位
                    {
                        return false;
                    }

                    if (originalText.Contains('-'))//重复-
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
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
