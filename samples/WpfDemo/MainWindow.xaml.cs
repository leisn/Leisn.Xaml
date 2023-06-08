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
            Lang.ChangeLang(langsBox.SelectedItem.ToString()!);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            langsBox.ItemsSource = Lang.Current.Langs;
            langsBox.SelectedItem = Lang.Current.CurrentLang;
            langsBox.SelectionChanged += LangsBox_SelectionChanged;

            perpertyGrid.Source = new PgTest();
        }

        private class PgTest
        {
            [PathSelect(IsSelectFolder = true)]
            [DisplayName("{Ask} him {Ok}")]
            public string Folder
            {
                get;
                set;
            }
            [PathSelect(IsSelectFolder = false)]
            public string File { get; set; }

            [NumericUpDown(Increment = 0.1, Maximum = 1, Minimum = -1)]
            [NumericFormat(Decimals = 3, Unit = "m")]
            public double Formart { get; set; }

            [Category("sbyte value")]
            [DisplayName("sbyte value 1")]
            [Description("Range not limited")]
            public sbyte sbValue1 { get; set; }
            [Category("sbyte value")]
            [DisplayName("sbyte value 2")]
            [Description("Range 4-10, Increment=2 \n 数值范围：[4,10]")]
            [Range(4, 10)]
            [Increment(2)]
            public sbyte sbValue2 { get; set; }


            [Category("byte value")]
            [DisplayName("byte value 1")]
            [Description("Range not limited")]
            public byte bValue1 { get; set; }
            [Category("byte value")]
            [DisplayName("byte value 2")]
            [Description("Range 4-10, Increment=2")]
            [Range(4, 10)]
            [Increment(2)]
            public byte bValue2 { get; set; }

            [Category("short value")]
            [DisplayName("short value 1")]
            [Description("Range not limited")]
            public short sValue1 { get; set; }
            [Category("short value")]
            [DisplayName("short value 2")]
            [Description("Range 4-10, Increment=2")]
            [Range(4, 10)]
            [Increment(2)]
            public short sValue2 { get; set; }

            [Category("ushort value")]
            [DisplayName("ushort value 1")]
            [Description("Range not limited")]
            public ushort usValue1 { get; set; }
            [Category("ushort value")]
            [DisplayName("ushort value 2")]
            [Description("Range 4-10, Increment=2")]
            [Range(4, 10)]
            [Increment(2)]
            public ushort usValue2 { get; set; }

            [Category("int value")]
            [DisplayName("int value 1")]
            [Description("int not limited")]
            public int Value1 { get; set; }
            [Category("int value")]
            [DisplayName("int value 2")]
            [Description("Range 4-10, Increment=2")]
            [Range(4, 10)]
            [Increment(2)]
            public int Value2 { get; set; }

            [Category("uint value")]
            [DisplayName("uint value 1")]
            [Description("uint not limited")]
            public uint uValue1 { get; set; }
            [Category("uint value")]
            [DisplayName("uint value 2")]
            [Description("Range 4-10, Increment=2")]
            [Range(4, 10)]
            [Increment(2)]
            public uint uValue2 { get; set; }

            [Category("long value")]
            [DisplayName("long value 1")]
            [Description("Range not limited")]
            public long lValue1 { get; set; }
            [Category("long value")]
            [DisplayName("long value 2")]
            [Description("Range 4-10, Increment=2")]
            [Range(4, 10)]
            [Increment(2)]
            public long lValue2 { get; set; }

            [Category("ulong value")]
            [DisplayName("ulong value 1")]
            [Description("Range not limited")]
            public ulong ulValue1 { get; set; }
            [Category("ulong value")]
            [DisplayName("ulong value 2")]
            [Description("Range 4-10, Increment=2")]
            [Range(4, 10)]
            [Increment(2)]
            public ulong ulValue2 { get; set; }

            [Category("double value")]
            [DisplayName("double value 1")]
            [Description("Range not limited")]
            public double dValue1 { get; set; }
            [Category("double value")]
            [DisplayName("double value 2")]
            [Description("Range 4-10, Increment=2")]
            [Range(4, 10)]
            [Increment(2)]
            public double dValue2 { get; set; }

            [Category("decimal value")]
            [DisplayName("decimal value 1")]
            [Description("Range not limited")]
            public decimal mValue1 { get; set; }
            [Category("decimal value")]
            [DisplayName("decimal value 2")]
            [Description("Range 4-10, Increment=2")]
            [Range(4, 10)]
            [Increment(2)]
            public decimal mValue2 { get; set; }


            public int Value3 { get; set; }
            public int Value4 { get; set; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
