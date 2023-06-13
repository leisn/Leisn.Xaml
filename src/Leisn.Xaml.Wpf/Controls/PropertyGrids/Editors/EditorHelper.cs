// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

using Leisn.Common.Data;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class EditorHelper
    {
        public static ComboBox CreateComboBox(IEnumerable<IDataDeclaration<object>> datas)
        {
            var contaniner = new FrameworkElementFactory(typeof(Border));
            contaniner.SetValue(Border.BackgroundProperty, Brushes.Transparent);
            contaniner.SetValue(Border.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            contaniner.SetBinding(Border.ToolTipProperty, new Binding("Description"));
            contaniner.SetValue(ToolTip.PlacementProperty, PlacementMode.Top);
            var textBlock = new FrameworkElementFactory(typeof(TextBlock));
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("DisplayName"));
            contaniner.AppendChild(textBlock);
            var dataTemplate = new DataTemplate { VisualTree = contaniner };

            var box = new ComboBox
            {
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                ItemsSource = datas,
                SelectedValuePath = "Value",
                ItemTemplate = dataTemplate,
            };
            return box;
        }
    }
}
