// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Input
{
    public interface ICommand<T> : ICommand
    {
        bool CanExecute(T? parameter);
        void Execute(T? parameter);
    }
}
