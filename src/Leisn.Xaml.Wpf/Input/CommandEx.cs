// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Input
{
    public static class CommandEx
    {
        public static void RaiseCanExecuteChanged(this ICommand command)
        {
            var type = command.GetType();
            var method = type.GetMethod("RaiseCanExecuteChanged");
            if (method != null)
            {
                method.Invoke(command, Array.Empty<object>());
                return;
            }
            var field = type.GetField("CanExecuteChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            var instache = field?.GetValue(command) as EventHandler;
            instache?.Invoke(command, EventArgs.Empty);
        }
    }
}
