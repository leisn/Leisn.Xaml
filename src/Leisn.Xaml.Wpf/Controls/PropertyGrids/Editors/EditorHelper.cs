// By Leisn (https://leisn.com , https://github.com/leisn)

using Leisn.Common.Data;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Controls.PropertyGrids.Editors
{
    internal class EditorHelper
    {
        public static ComboBox CreateComboBox(IEnumerable<IDataDeclaration<object>> datas)
        {
            FrameworkElementFactory contaniner = new(typeof(Border));
            contaniner.SetValue(Border.BackgroundProperty, Brushes.Transparent);
            contaniner.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            contaniner.SetBinding(FrameworkElement.ToolTipProperty, new Binding("Description"));
            contaniner.SetValue(ToolTip.PlacementProperty, PlacementMode.Top);
            FrameworkElementFactory textBlock = new(typeof(TextBlock));
            textBlock.SetBinding(TextBlock.TextProperty, new Binding("DisplayName"));
            contaniner.AppendChild(textBlock);
            DataTemplate dataTemplate = new() { VisualTree = contaniner };

            ComboBox box = new()
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
