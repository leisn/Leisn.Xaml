// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Input
{
    public class ControlCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        private readonly Action _action;
        private readonly Func<bool>? _canExecute;
        public ControlCommand(Action action, Func<bool>? canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public virtual bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public virtual void Execute(object? parameter)
        {
            _action?.Invoke();
        }
    }


}
