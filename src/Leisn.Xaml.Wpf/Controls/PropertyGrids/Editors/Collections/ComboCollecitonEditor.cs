// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using Leisn.Common.Attributes;
using Leisn.Common.Collections;
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

        protected override ComboBox CreateItemElement(object? item)
        {
            var comboBox = EditorHelper.CreateComboBox(_dataSource);
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
