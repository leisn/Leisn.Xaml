using Leisn.Xaml.Wpf.Locales;

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
using WpfDemo.Providers;
using Leisn.Common.Media;
using System.Threading;
using Leisn.Xaml.Wpf.Controls;
using System.Windows.Controls;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Process process = new();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "cmd";
            process.StartInfo.Arguments = @"/C ""more""";
            process.StartInfo.RedirectStandardInput = true;
            process.Start();
            Console.SetOut(process.StandardInput);
            Console.SetError(process.StandardInput);

            Console.WriteLine("Lang init...");
            Lang.Initialize(Thread.CurrentThread.CurrentCulture.Name);

            Console.WriteLine("Sevices init...");
            var sevices = new ServiceCollection();
            sevices.AddSingleton<SampleTextProvider>()
                   .AddSingleton<SampleTextWidthDescProvider>();

            Console.WriteLine("UIContext init...");
            UIContext.Initialize(sevices.BuildServiceProvider());
            Console.WriteLine("App started.");
        }
    }

}
