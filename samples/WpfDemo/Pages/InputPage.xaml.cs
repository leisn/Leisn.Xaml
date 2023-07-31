// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.IO;
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

using Leisn.Common.Collections;

namespace WpfDemo.Pages
{
    /// <summary>
    /// ButtonPage.xaml 的交互逻辑
    /// </summary>
    public partial class InputPage : UserControl
    {
        public InputPage()
        {
            InitializeComponent();

            Loaded += InputPage_Loaded;
        }

        private void InputPage_Loaded(object sender, RoutedEventArgs e)
        {
            var path = new DirectoryInfo(Directory.GetCurrentDirectory());
            var items = new List<SystemItem>();
            path.GetDirectories().ForEach(dir =>
            {
                var dirItem = new DirItem(dir.Name);
                InitDir(dirItem, dir);
                items.Add(dirItem);
            });
            path.GetFiles().ForEach(file => items.Add(new FileItem(file.Name)));
            treeView.ItemsSource = items;
        }

        private static void InitDir(DirItem item, DirectoryInfo info)
        {
            var dirs = info.GetDirectories();
            foreach (var dir in dirs)
            {
                var newDir = new DirItem(dir.Name);
                InitDir(newDir, dir);
                item.Items.Add(newDir);
            }
            info.GetFiles().ForEach(file =>
            {
                item.Items.Add(new FileItem(file.Name));
            });
        }
    }

    class SystemItem
    {
        public List<SystemItem> Items { get; } = new List<SystemItem>();
        public string Name { get; set; }
        public SystemItem(string name)
        {
            Name = name;
        }
    }
    class DirItem : SystemItem
    {
        public DirItem(string name) : base(name)
        {
        }
        public override string ToString()
        {
            return $"(D){Name}";
        }
    }
    class FileItem : SystemItem
    {
        public FileItem(string name) : base(name)
        {
        }
        public override string ToString()
        {
            return $"(F){Name}";
        }
    }
}
