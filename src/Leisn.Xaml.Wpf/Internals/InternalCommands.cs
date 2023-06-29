// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Leisn.Xaml.Wpf.Input;

namespace Leisn.Xaml.Wpf.Internals
{
    internal class InternalCommands
    {
        public static ICommand ClearDatePickerCommand { get; } = new ControlCommand<DatePicker>((picker) => picker!.SelectedDate = null);
        public static ICommand ClearTextboxCommand { get; } = new ControlCommand<TextBox>((box) => box!.Text = null);
    }
}
