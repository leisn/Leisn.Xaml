// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Input
{
    public interface ICommand<T> : ICommand
    {
        bool CanExecute(T? parameter);
        void Execute(T? parameter);
    }
}
