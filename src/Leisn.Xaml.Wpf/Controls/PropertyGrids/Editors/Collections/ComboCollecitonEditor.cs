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
    internal class ComboCollecitonEditor : CollectionEditorBase
    {
        public IEnumerable<IDataDeclaration<object>> _dataSource = null!;
        public ComboCollecitonEditor()
        {
        }

        public override FrameworkElement CreateElement(PropertyItem item)
        {
            var type = item.PropertyDescriptor.Attr<DataProviderAttribute>()!.ProviderType;
            _dataSource = EditorHelper.ResolveDataProvider(type);
            return base.CreateElement(item);
        }

        protected override UIElement CreateItemElement(object? item)
        {
            var comboBox = EditorHelper.CreateComboBox(_dataSource);
            comboBox.SelectedValue = item;
            return comboBox;
        }

        protected override bool CreateNewItem(out object? newItem)
        {
            newItem = null;
            return true;
        }

        protected override bool DeleteItemAt(int index)
        {
            return true;
        }
    }
}
