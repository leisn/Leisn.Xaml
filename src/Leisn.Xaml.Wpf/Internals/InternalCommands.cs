// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Leisn.Xaml.Wpf.Controls;

namespace Leisn.Xaml.Wpf.Internals
{
    internal class InternalCommands
    {
        public static ICommand ClearDateTimeCommand { get; } = new ControlCommand<UIElement>(picker =>
        {
            if (picker is DatePicker dp)
            {
                dp.SelectedDate = null;
            }
            else if (picker is DateTimePicker dtp)
            {
                dtp.SelectedDateTime = null;
            }
        });
        public static ICommand ClearTextCommand { get; } = new ControlCommand<UIElement>(box =>
        {
            if (box is TextBox textbox)
            {
                textbox.Text = null;
            }
            else if (box is TextBlock block)
            {
                block.Text = null;
            }
            else if (box is PasswordBox passwordbox)
            {
                passwordbox.Password = null;
            }
        });
    }
}
