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
using System.Windows.Input;
using Leisn.Xaml.Wpf.Input;
using System.Windows.Controls.Primitives;
using System.Collections;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        class A : List<string> { }
        public App()
        {

            var type0 = typeof(IList<>);
            var type1 = typeof(IList<object>);
            var type2 = typeof(IList<int>);
            var type3 = typeof(List<int>);
            
            var type4 = typeof(Dictionary<int, string>);
            var type5 = typeof(IDictionary<int, string>);
            var type6 = typeof(IDictionary<,>);
            var re2 = type4.IsTypeOf(type5);
            // Process process = new();
            // process.StartInfo.UseShellExecute = false;
            // process.StartInfo.FileName = "cmd";
            // process.StartInfo.Arguments = @"/C ""more""";
            // process.StartInfo.RedirectStandardInput = true;
            // process.Start();
            // Console.SetOut(process.StandardInput);
            // Console.SetError(process.StandardInput);

            Console.WriteLine("Lang init...");
            Lang.Initialize(Thread.CurrentThread.CurrentCulture.Name);

            Console.WriteLine("Sevices init...");
            var sevices = new ServiceCollection();
            sevices.AddSingleton<SampleTextProvider>()
                   .AddSingleton<SampleArrayProvider>()
                   .AddSingleton<SampleObjectProvider>();

            Console.WriteLine("UIContext init...");
            AppIoc.Initialize(sevices.BuildServiceProvider());
            Console.WriteLine("App started.");
        }
    }

}
