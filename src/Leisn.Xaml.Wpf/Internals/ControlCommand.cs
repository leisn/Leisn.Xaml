// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Internals
{
    internal class ControlCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action _action;
        private readonly Func<bool>? _canExecute;
        public ControlCommand(Action action, Func<bool>? canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public virtual void Execute(object? parameter)
        {
            _action?.Invoke();
        }
    }
    internal class ControlCommand<T> : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action<T?> _action;
        private readonly Func<T?, bool>? _canExecuteFunc;
        public ControlCommand(Action<T?> action, Func<T?, bool>? canExecuteFunc = null)
        {
            _action = action;
            _canExecuteFunc = canExecuteFunc;
        }

        bool ICommand.CanExecute(object? parameter)
        {
            return CanExecute((T?)parameter);
        }

        void ICommand.Execute(object? parameter)
        {
            Execute((T?)parameter);
        }

        public bool CanExecute(T? parameter)
        {
            return _canExecuteFunc == null || _canExecuteFunc(parameter);
        }

        public void Execute(T? parameter)
        {
            _action?.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
