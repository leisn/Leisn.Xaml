// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Internals
{
    internal class InternalCommands
    {
        public static ICommand ClearCommand { get; } 
        public static ICommandSource GetCommandSource(DependencyObject obj)
        {
            return (ICommandSource)obj.GetValue(CommandSourceProperty);
        }

        public static void SetCommandSource(DependencyObject obj, ICommandSource value)
        {
            obj.SetValue(CommandSourceProperty, value);
        }

        public static readonly DependencyProperty CommandSourceProperty =
            DependencyProperty.RegisterAttached("CommandSource", typeof(ICommandSource), typeof(InternalCommands), new PropertyMetadata(0,new PropertyChangedCallback(OnCommandSourceChanged)));

        private static void OnCommandSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
          
        }
    }
}
