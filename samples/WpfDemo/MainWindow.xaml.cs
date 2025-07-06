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
using Leisn.NodeEditor;

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
            nodeView.Inputs =
            [
               new NodeSlot { Header = "Input 1" },
                new NodeSlot { Header = "Input 2" },
                new NodeSlot { Header = "Input 3" }
            ];
            nodeView.Outputs =
            [
                new NodeSlot { Header = "Output 1" },
                new NodeSlot { Header = "Output 2" },
            ];
        }

        private void DataGrid_LoadingRow(object? sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            //AppTheme.ChangeTheme(new Uri("/Leisn.Xaml.Wpf;component/Assets/ColorsLight.xaml", UriKind.Relative));
        }

        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Application.Current.Resources.MergedDictionaries.RemoveAt(0);
            var color = "/Leisn.Xaml.Wpf;component/Assets/ColorsLight.xaml";
            if ((e.Source as ComboBox)?.SelectedIndex == 0)
                color = "/Leisn.Xaml.Wpf;component/Assets/Colors.xaml";
            var uri = new Uri($"pack://application:,,,{color}", UriKind.RelativeOrAbsolute);
            var res = new ResourceDictionary();
            res.BeginInit();
            res.Source = uri;
            res.EndInit();
            Application.Current.Resources.MergedDictionaries.Insert(0, res);
        }

        private void RouletteTabItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F && Keyboard.IsKeyDown(Key.LeftShift))
            {
                roulette.Visibility = Visibility.Visible;
                roulette.SelectionChanged += Roulette_SelectionChanged;
            }
            else if (e.Key == Key.Escape)
            {
                roulette.SelectionChanged -= Roulette_SelectionChanged;
                roulette.Visibility = Visibility.Hidden;
            }
        }

        private void Roulette_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            roulette.SelectionChanged -= Roulette_SelectionChanged;
            roulette.Visibility = Visibility.Collapsed;
            rouletterText.Text = $"Selected: {roulette.SelectedItem}";
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
