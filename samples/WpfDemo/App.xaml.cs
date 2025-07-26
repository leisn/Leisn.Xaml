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
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.VisualBasic;
using System.Runtime.CompilerServices;
using Leisn.Xaml.Wpf.Extensions;
using System.Timers;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly struct Source
        {
            public readonly string Name;
            public int Value { get; }
            public Source(string name, int value)
            {
                Name = name;
                Value = value;
            }
        }

        class Target
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        public App()
        {
            var source = new Source("123", 456);
            var target = new Target();
            var sourceType = source.GetType();
            var targetType = target.GetType();
            var sourceFields = sourceType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            var sourceProperties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanRead);
            foreach (var sourceField in sourceFields)
            {
                var value = sourceField.GetValue(source);
                targetType.GetField(sourceField.Name)?
                    .SetValue(target, value);
                var property = targetType.GetProperty(sourceField.Name);
                if (property?.CanWrite == true)
                    property.SetValue(target, value);
            }

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
