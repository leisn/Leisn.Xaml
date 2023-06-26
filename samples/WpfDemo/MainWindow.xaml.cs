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
            langsBox.DisplayMemberPath = "NativeName";
            langsBox.ItemsSource = list;
            langsBox.SelectedItem = new CultureInfo(Lang.CurrentLanguage);
            langsBox.SelectionChanged += LangsBox_SelectionChanged;

            perpertyGrid.Source = new PgTest();
        }

        public enum Enumv : byte
        {
            [Category("V1")]
            [Description("V1 desc")]
            Value1,
            [Category("V2")]
            [Description("V2 desc")]
            Value2,
        }

        public class SubClass
        {
            [Category("sbyte value")]
            [DisplayName("sbyte value 2")]
            [Description("Range 4-10, Increment=2 \n{ValueRange}：[4,10]")]
            [Range(4, 10)]
            [Increment(2)]
            public sbyte sbValue2 { get; set; }
            [Category("int value")]
            [DisplayName("int value 1")]
            [Description("int not limited")]
            public int Value1 { get; set; }
        }
        private class PgTest
        {
            public SubClass SubClass { get; set; } = new();
            //public List<int> ListInts { get; set; } = new();
            //public IEnumerable<int> EnumInts { get; set; } = new Collection<int>();
            public DateTime DateTime { get; set; }
            public bool? BoolNullable { get; set; }
            public bool Bool { get; set; }
            public Color Color { get; set; } = ColorEx.FromHex("#7805112c");
            public Enumv EunmV { get; set; }

            [PathSelect(Mode = PathSelectMode.Folder, DialogTitle = "选择一个文件夹{Ok}")]
            [DisplayName("{Ask} him {Ok}")]
            public string Folder { get; set; } = "";
            [PathSelect(Mode = PathSelectMode.OpenFile)]
            public string File { get; set; } = "D://";

            [NumericUpDown(Increment = 0.1, Maximum = 1, Minimum = -1)]
            [NumericFormat(Decimals = 3, Suffix = "m")]
            public double Formart { get; set; }

            [ReadOnly(true)]
            public string ReadOnlyText { get; set; } = "ReadOnlyText 1";

            [DisplayName("最多可以输入几个字呢")]
            public string Text { get; set; } = "input text";

            [Category("DataProvider")]
            [DataProvider(typeof(SampleTextProvider))]
            public string TextProvider { get; set; } = "Text 1";
            [Category("DataProvider")]
            [DataProvider(typeof(SampleTextWidthDescProvider))]
            public string ArrayTextProvider { get; set; } = "Text 1";
            [Category("DataProvider")]
            [DataProvider(typeof(SampleObjectProvider))]
            public SampleObject ObjectProvider { get; set; } = null!;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

    }
}
