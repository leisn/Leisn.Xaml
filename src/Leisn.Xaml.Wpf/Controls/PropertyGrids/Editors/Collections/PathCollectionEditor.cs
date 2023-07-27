// @Leisn (https://leisn.com , https://github.com/leisn)

using Leisn.Common.Attributes;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class PathCollectionEditor : CollectionEditorBase<PathSelector>
    {
        private readonly PathSelectAttribute _selectAttr;
        public PathCollectionEditor(PathSelectAttribute attribute)
        {
            _selectAttr = attribute;
        }

        protected override PathSelector CreateItemElement(int index, object? item)
        {
            PathSelector control = new()
            {
                Mode = _selectAttr.Mode,
                IsTextReadOnly = _selectAttr.IsTextReadOnly,
                DialogTitle = _selectAttr.DialogTitle,
                FileFilter = _selectAttr.FileFilter,
                Path = item?.ToString()!,
                IsEnabled = !IsCoerceReadOnly,
                Tag = index
            };
            control.PathChanged += OnItemPathChanged;
            return control;
        }

        private void OnItemPathChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<string> e)
        {
            UpdateItemValue((PathSelector)sender);
        }

        protected override object CreateNewItem()
        {
            return string.Empty;
        }

        protected override object GetElementValue(PathSelector element)
        {
            return element.Path;
        }

        protected override void OnRemoveElement(PathSelector element)
        {
            element.PathChanged -= OnItemPathChanged;
        }
    }
}
