using Microsoft.WindowsAPICodePack.Dialogs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = PART_TextBoxName, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_ButtonName, Type = typeof(ButtonBase))]
    public class PathSelector : Control
    {
        const string PART_TextBoxName = "PART_TextBox";
        const string PART_ButtonName = "PART_Button";

        private TextBox _textBox = null!;
        private ButtonBase _button = null!;

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(PathSelector), new PropertyMetadata(default));

        public bool IsTextReadOnly
        {
            get { return (bool)GetValue(IsTextReadOnlyProperty); }
            set { SetValue(IsTextReadOnlyProperty, value); }
        }
        public static readonly DependencyProperty IsTextReadOnlyProperty =
            DependencyProperty.Register("IsTextReadOnly", typeof(bool), typeof(PathSelector), new PropertyMetadata(true));

        public bool IsSelectFolder
        {
            get { return (bool)GetValue(IsSelectFolderProperty); }
            set { SetValue(IsSelectFolderProperty, value); }
        }

        public static readonly DependencyProperty IsSelectFolderProperty =
            DependencyProperty.Register("IsSelectFolder", typeof(bool), typeof(PathSelector), new PropertyMetadata(false));

        public string DialogTitle
        {
            get { return (string)GetValue(DialogTitleProperty); }
            set { SetValue(DialogTitleProperty, value); }
        }
        public static readonly DependencyProperty DialogTitleProperty =
            DependencyProperty.Register("DialogTitle", typeof(string), typeof(PathSelector), new PropertyMetadata(string.Empty));

        public string FileFilter
        {
            get { return (string)GetValue(FileFilterProperty); }
            set { SetValue(FileFilterProperty, value); }
        }
        public static readonly DependencyProperty FileFilterProperty =
            DependencyProperty.Register("FileFilter", typeof(string), typeof(PathSelector), new PropertyMetadata(default));

        public override void OnApplyTemplate()
        {
            if (_button != null)
                _button.Click -= OnButtonClicked;
            base.OnApplyTemplate();
            _textBox = (TextBox)GetTemplateChild(PART_TextBoxName);
            _button = (ButtonBase)GetTemplateChild(PART_ButtonName);
            _button.Click += OnButtonClicked;
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = IsSelectFolder,
                Title = DialogTitle ?? "选择",
            };
            if (!IsSelectFolder && !string.IsNullOrEmpty(FileFilter))
            {
                var ts = FileFilter.Split('|');
                for (int i = 1; i <= ts.Length; i += 2)
                {
                    dialog.Filters.Add(new CommonFileDialogFilter(ts[i - 1], ts[i]));
                }
            }
            if (dialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
                return;
            Path = dialog.FileName!;
        }
    }
}
