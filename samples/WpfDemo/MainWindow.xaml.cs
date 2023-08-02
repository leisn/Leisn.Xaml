using Leisn.Common.Attributes;
using Leisn.Xaml.Wpf.Locales;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfDemo.Providers;
using System.Diagnostics;
using Leisn.Xaml.Wpf;
using System.Globalization;
using Leisn.Common.Data;
using System.Collections.ObjectModel;
using System.Collections;

namespace WpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void LangsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Lang.SetLanguage(((CultureInfo)langsBox.SelectedItem).Name);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var list = Lang.Languages.Select(x => new CultureInfo(x)).ToList();
            langsBox.DisplayMemberPath = nameof(CultureInfo.NativeName);
            langsBox.ItemsSource = list;
            langsBox.SelectedItem = new CultureInfo(Lang.CurrentLanguage);
            langsBox.SelectionChanged += LangsBox_SelectionChanged;
            dataGrid.LoadingRow += DataGrid_LoadingRow;
            dataGrid.ItemsSource = new List<DataGridModel> {
                new DataGridModel{ Name="Name1"},new DataGridModel{ Name="Name2"},
                new DataGridModel{ Name="Name3"},new DataGridModel{ Name="Name4"},
                new DataGridModel{ Name="Name5"},
            };

        }

        private void DataGrid_LoadingRow(object? sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            //AppTheme.ChangeTheme(new Uri("/Leisn.Xaml.Wpf;component/Assets/ColorsLight.xaml", UriKind.Relative));
        }
    }
    class DataGridModel
    {
        public bool Check { get; set; }
        public string Name { get; set; } = "Name";
        public string Url { get; set; } = @"https://bing.com";
        public Dock Dock { get; set; }
    }
}
