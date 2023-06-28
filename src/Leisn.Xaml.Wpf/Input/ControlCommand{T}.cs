// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Input
{
    public class ControlCommand<T> : ICommand<T>
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action<T?> _action;
        private readonly Func<T?, bool>? _canExecuteFunc;
        public ControlCommand(Action<T?> action, Func<T?, bool>? canExecuteFunc = null)
        {
            _action = action;
            _canExecuteFunc = canExecuteFunc;
        }

        bool ICommand.CanExecute(object? parameter) => CanExecute((T?)parameter);

        void ICommand.Execute(object? parameter) => Execute((T?)parameter);

        public bool CanExecute(T? parameter)
            => _canExecuteFunc == null || _canExecuteFunc(parameter);

        public void Execute(T? parameter) => _action?.Invoke(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
