// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Input
{
    public class AsyncCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Func<Task> _task;
        private readonly Func<bool>? _canExecuteFunc;
        private bool _canExecute = true;
        public AsyncCommand(Func<Task> task, Func<bool> canExecute)
        {
            _task = task;
            _canExecuteFunc = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute && (_canExecuteFunc == null || _canExecuteFunc());

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public async void Execute(object? parameter)
        {
            if (!_canExecute)
                return;
            _canExecute = false;
            RaiseCanExecuteChanged();
            await _task();
            _canExecute = true;
            RaiseCanExecuteChanged();
        }
    }
}
