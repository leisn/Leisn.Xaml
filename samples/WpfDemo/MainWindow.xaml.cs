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

        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            //AppTheme.ChangeTheme(new Uri("/Leisn.Xaml.Wpf;component/Assets/ColorsLight.xaml", UriKind.Relative));
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
            public IEnumerable<string> ReadOnlyStrings { get; set; } = new ReadOnlyCollection<string>(new List<string> { "stri 1", "str 2" });
            public string[] StringArray { get; set; } = new string[] { "stri 1", "str 2", "stri 3", "str 4" };
            [StringLength(10)]
            public List<string> ListStrings { get; set; } = new List<string>() { "stri 1", "str 2" };
            [Range(4, 10)]
            [Increment(1)]
            public List<int> IntValues { get; set; } = new List<int>();
            public List<Enumv> Enumvs { get; set; } = new List<Enumv>();
            public List<SubClass> SubClasses { get; set; } = new List<SubClass>();
            public DateTime DateTime { get; set; } = DateTime.Now;
            public DateTime? DateTime2 { get; set; }
            public DateOnly DateOnly { get; set; } = new DateOnly(2000, 02, 20);
            public TimeOnly TimeOnly { get; set; } = new TimeOnly(5, 12, 9);
            public SubClass SubClass { get; set; } = new();
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
            [DisplayName("最多可以输入几个字呢")]
            [Placeholder("输入字符{Ok}")]
            public string Text { get; set; } = "input text";
            [ReadOnly(true)]
            public string ReadOnlyText { get; set; } = "ReadOnlyText 1";
            [Category("DataProvider")]
            [DataProvider(typeof(SampleTextProvider))]
            public string TextProvider { get; set; } = "Sample Text5";
            [Category("DataProvider")]
            [DataProvider(typeof(SampleArrayProvider))]
            public int? ArrayTextProvider { get; set; } = 2;
            [Category("DataProvider")]
            [DataProvider(typeof(SampleObjectProvider))]
            public SampleObject? ObjectProvider { get; set; }

            [Category("DataProvider")]
            [DataProvider(typeof(SampleArrayProvider))]
            public List<int> ArrayProviderTextList { get; set; } = new() { 1, 2, 3 };
        }

    }
}
