// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Leisn.Common.Attributes;

using Microsoft.Win32;

using WindowsAPICodePack.Dialogs;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = PART_TextBoxName, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_ButtonName, Type = typeof(ButtonBase))]
    public class PathSelector : Control
    {
        private const string PART_TextBoxName = "PART_TextBox";
        private const string PART_ButtonName = "PART_Button";

        private TextBox _textBox = null!;
        private ButtonBase _button = null!;
        public PathSelectMode Mode
        {
            get { return (PathSelectMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("Mode", typeof(PathSelectMode), typeof(PathSelector), new PropertyMetadata(PathSelectMode.Folder));

        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }
        public static readonly DependencyProperty PathProperty =
            DependencyProperty.Register("Path", typeof(string), typeof(PathSelector), new PropertyMetadata(default));

        public bool IsTextReadOnly
        {
            get => (bool)GetValue(IsTextReadOnlyProperty);
            set => SetValue(IsTextReadOnlyProperty, value);
        }
        public static readonly DependencyProperty IsTextReadOnlyProperty =
            DependencyProperty.Register("IsTextReadOnly", typeof(bool), typeof(PathSelector), new PropertyMetadata(true));

        public string DialogTitle
        {
            get => (string)GetValue(DialogTitleProperty);
            set => SetValue(DialogTitleProperty, value);
        }
        public static readonly DependencyProperty DialogTitleProperty =
            DependencyProperty.Register("DialogTitle", typeof(string), typeof(PathSelector), new PropertyMetadata(string.Empty));

        public string FileFilter
        {
            get => (string)GetValue(FileFilterProperty);
            set => SetValue(FileFilterProperty, value);
        }
        public static readonly DependencyProperty FileFilterProperty =
            DependencyProperty.Register("FileFilter", typeof(string), typeof(PathSelector), new PropertyMetadata(default));

        public override void OnApplyTemplate()
        {
            if (_button != null)
            {
                _button.Click -= OnButtonClicked;
            }

            base.OnApplyTemplate();
            _textBox = (TextBox)GetTemplateChild(PART_TextBoxName);
            _button = (ButtonBase)GetTemplateChild(PART_ButtonName);
            _button.Click += OnButtonClicked;
        }

        private void OnButtonClicked(object sender, RoutedEventArgs e)
        {
            if (Mode == PathSelectMode.OpenFile || Mode == PathSelectMode.SaveFile)
            {
                FileDialog fileDialog = Mode == PathSelectMode.SaveFile ?
                    new Microsoft.Win32.SaveFileDialog() : new Microsoft.Win32.OpenFileDialog();
                if (!string.IsNullOrEmpty(FileFilter))
                    fileDialog.Filter = FileFilter;
                if (!string.IsNullOrEmpty(DialogTitle))
                    fileDialog.Title = DialogTitle;
                if (fileDialog.ShowDialog(Application.Current.MainWindow) != true)
                    return;
                Path = fileDialog.FileName;
            }
            else if (Mode == PathSelectMode.Folder)
            {
                CommonOpenFileDialog dialog = new()
                {
                    IsFolderPicker = true,
                    Title = DialogTitle,
                };
                if (dialog.ShowDialog(Application.Current.MainWindow) != CommonFileDialogResult.Ok)
                    return;
                Path = dialog.FileName!;
            }
        }
    }
}
