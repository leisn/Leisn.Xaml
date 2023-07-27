// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using Leisn.Common.Data;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ComboCollecitonEditor : CollectionEditorBase<ComboBox>
    {
        public IEnumerable<IDataDeclaration<object>> _dataSource = null!;
        public ComboCollecitonEditor(Type dateProviderType)
        {
            _dataSource = EditorHelper.ResolveDataProvider(dateProviderType);
        }

        protected override ComboBox CreateItemElement(int index, object? item)
        {
            ComboBox comboBox = EditorHelper.CreateComboBox(_dataSource);
            comboBox.SelectedValue = item;
            comboBox.SelectionChanged += ComboBox_SelectionChanged;
            return comboBox;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateItemValue((ComboBox)sender);
        }

        protected override object CreateNewItem()
        {
            return _dataSource.First().Value;
        }

        protected override object GetElementValue(ComboBox element)
        {
            return element.SelectedValue;
        }

        protected override void OnRemoveElement(ComboBox element)
        {
            element.SelectionChanged -= ComboBox_SelectionChanged;
        }
    }
}
