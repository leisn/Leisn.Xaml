// @Leisn (https://leisn.com , https://github.com/leisn)

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

using Leisn.Common.Attributes;
using WpfDemo.Providers;
using static WpfDemo.MainWindow;
using System.Collections.ObjectModel;

namespace WpfDemo.Pages
{
    /// <summary>
    /// PropertyGridPage.xaml 的交互逻辑
    /// </summary>
    public partial class PropertyGridPage : UserControl
    {
        public PropertyGridPage()
        {
            InitializeComponent();
            perpertyGrid1.Source = new PropertyClass1();
            perpertyGrid2.Source = new PropertyClass2();
            perpertyGrid3.Source = new PropertyClass3();
            perpertyGrid4.Source = new PropertyClass4();
        }

        enum EnumSample
        {
            [Category("V1")]
            [Description("The v1 value")]
            Value1,
            [Category("V2")]
            [Description("The v2 value")]
            Value2,
            [Category("V3")]
            [Description("The v3 value")]
            Value3,
        }
        class PropertyClass1
        {
            [Category("Number")]
            [DisplayName("int value")]
            [Description("Range 4-10, Increment=2 \n{ValueRange}：[4,10]")]
            [Range(4, 10)]
            [Increment(2)]
            public int IntValue { get; set; }
            [Category("Number")]
            [DisplayName("Double Value")]
            [Description("Decimals=3, Suffix=$, Increment=5\n{ValueRange}：[-25,25]")]
            [NumericFormat(Decimals = 3, Suffix = "$")]
            [NumericUpDown(Increment = 5, Maximum = 25, Minimum = -25)]
            public double DoubleValue { get; set; }
            [Category("Number")]
            [DisplayName("Sbyte Value")]
            public sbyte SbyteVale { get; set; }

            [Category("PathSelect")]
            [PathSelect(Mode = PathSelectMode.OpenFile, FileFilter = "json文件|*.json")]
            public string? FilePath { get; set; }

            [Category("PathSelect")]
            [PathSelect(Mode = PathSelectMode.Folder, DialogTitle = "选择一个文件夹{Ok}")]
            public string? FolderPath { get; set; }

            [Category("PathSelect")]
            [PathSelect(Mode = PathSelectMode.SaveFile)]
            public string? SaveFilePath { get; set; }

            [Category("String")]
            [Placeholder("输入字符{Ok}")]
            [StringLength(10)]
            [Description("最多输入10个字符")]
            public string? Text { get; set; }
            [Category("String")]
            [ReadOnly(true)]
            public string ReadOnlyText { get; set; } = "ReadOnlyString";

            [Category("Enum")]
            public EnumSample EnumValue { get; set; }
        }

        class PropertyClass2
        {
            [Category("Bool")]
            public bool? BoolNullable { get; set; }
            [Category("Bool")]
            public bool Bool { get; set; }

            [Category("DateTime")]
            public DateTime DateTime { get; set; } = DateTime.Now;
            [Category("DateTime")]
            [DateTimePick(DateTimeSelectionMode.DateOnly)]
            public DateTime OnlyDate { get; set; } = DateTime.Now;
            [Category("DateTime")]
            [DateTimePick(DateTimeSelectionMode.TimeOnly)]
            public DateTime OnlyTime { get; set; } = DateTime.Now;
            [Category("DateTime")]
            public DateTime? DateTimeNullable { get; set; }
            [Category("DateTime")]
            public DateOnly DateOnly { get; set; } = new DateOnly(2000, 02, 20);
            [Category("DateTime")]
            public TimeOnly TimeOnly { get; set; } = new TimeOnly(5, 12, 9);

            [Category("Color")]
            public Color Color { get; set; } = ColorEx.FromHex("#7805112c");
        }
        class SubClass
        {
            public int ClassValue1 { get; set; }
            public string ClassValue2 { get; set; } = "";
            public override string ToString()
            {
                return $"1={ClassValue1},2={ClassValue2}";
            }
        }

        class SubClass1 : SubClass
        {
            public string? StringValue { get; set; }
            public override string ToString()
            {
                return $"1={ClassValue1},2={ClassValue2},3={StringValue}";
            }
        }
        class SubClass2 : SubClass
        {
            public EnumSample EnumValue { get; }
            public override string ToString()
            {
                return $"1={ClassValue1},2={ClassValue2},3={EnumValue}";
            }
        }

        class PropertyClass3
        {
            [Category("Class")]
            public SubClass SubClass { get; set; } = new();
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
            public List<int> ArrayProviderList { get; set; } = new() { 1, 2, 3 };
        }
        class PropertyClass4
        {
            [Category("Collection")]
            public IEnumerable<string> ReadOnlyStrings { get; set; } = new ReadOnlyCollection<string>(new List<string> { "stri 1", "str 2" });
            [Category("Collection")]
            public List<string> ListStrings { get; set; } = new List<string>() { "stri 1", "str 2" };
            [PathSelect(Mode = PathSelectMode.Folder)]
            [Category("Collection")]
            public string[] FolderArray { get; set; } = new string[] { "stri 1", "str 2", "stri 3", "str 4" };
            [Category("Collection")]
            [InstanceTypes(typeof(SubClass1), typeof(SubClass2))]
            public List<SubClass> SubClasses { get; set; } = new List<SubClass>();
            [StringLength(10)]
            [Category("Collection")]
            [DataProvider(typeof(SampleTextProvider), DictionaryTarget = DictionaryTarget.Key)]
            public Dictionary<string, List<SubClass>> DictionaryStringClass { get; set; } = new Dictionary<string, List<SubClass>>();

            [Range(1, 10)]
            [Increment(1)]
            [DefaultValue(9)]
            [Category("Collection")]
            public IEnumerable<int> IntValues { get; set; } = new List<int>() { 1, 2, 3, 4, 5 };
            [Category("Collection")]
            public List<EnumSample> Enumvs { get; set; } = new List<EnumSample>();
        }
    }
}
