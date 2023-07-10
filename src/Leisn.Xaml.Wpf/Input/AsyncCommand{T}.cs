// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Input
{
    public class AsyncCommand<T> : ICommand<T>
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Func<T?, Task> _task;
        private readonly Func<T?, bool>? _canExecuteFunc;
        private bool _canExecute = true;

        public AsyncCommand(Func<T?, Task> task, Func<T?, bool>? canExecuteFunc = null)
        {
            _task = task;
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
            return _canExecute && (_canExecuteFunc == null || _canExecuteFunc(parameter));
        }

        public async void Execute(T? parameter)
        {
            if (!_canExecute)
            {
                return;
            }

            _canExecute = false;
            RaiseCanExecuteChanged();
            await _task(parameter);
            _canExecute = true;
            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
