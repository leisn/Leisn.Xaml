// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Reflection;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Input
{
    public static class CommandEx
    {
        public static void RaiseCanExecuteChanged(this ICommand command)
        {
            Type type = command.GetType();
            MethodInfo? method = type.GetMethod("RaiseCanExecuteChanged");
            if (method != null)
            {
                method.Invoke(command, Array.Empty<object>());
                return;
            }
            FieldInfo? field = type.GetField("CanExecuteChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            EventHandler? instache = field?.GetValue(command) as EventHandler;
            instache?.Invoke(command, EventArgs.Empty);
        }
    }
}
