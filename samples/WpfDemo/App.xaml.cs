using Leisn.Xaml.Wpf.Locales;

using Leisn.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using Leisn.Xaml.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Lang.Initialize("en-us");

            var sevices = new ServiceCollection();

            UIContext.Initialize(sevices.BuildServiceProvider());
        }
    }

}
