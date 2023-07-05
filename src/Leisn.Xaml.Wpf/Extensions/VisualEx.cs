// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Reflection;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Leisn.Xaml.Wpf.Extensions
{
    public static class VisualEx
    {
        public static bool HasDescendant(this DependencyObject reference, DependencyObject? node)
        {
            bool result = false;
            DependencyObject? curr = node;
            while (curr != null)
            {
                if (curr == reference)
                {
                    result = true;
                    break;
                }
                if (curr is Popup popup)
                {
                    curr = popup.Parent;
                    if (curr == null)
                    {
                        curr = popup.PlacementTarget;
                    }
                }
                else
                {
                    curr = GetParent(curr);
                }
            }
            return result;
        }

        public static DependencyObject? GetParent(this DependencyObject o)
        {
            // see if o is a Visual or a Visual3D
            DependencyObject? v = o as Visual;
            if (v == null)
            {
                v = o as Visual3D;
            }

            ContentElement? ce = (v == null) ? o as ContentElement : null;

            if (ce != null)
            {
                o = ContentOperations.GetParent(ce);
                if (o != null)
                {
                    return o;
                }
                else
                {
                    if (ce is FrameworkContentElement fce)
                    {
                        return fce.Parent;
                    }
                }
            }
            else if (v != null)
            {
                if (v is FrameworkElement fe && fe.Parent is not null)
                {
                    return fe.Parent;
                }

                return VisualTreeHelper.GetParent(v);
            }

            return null;
        }

        public static DpiScale GetDpiScale()
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Static;
            var type = typeof(SystemParameters);
            var dpiXProp = type.GetProperty("DpiX", flags);
            var dpiYProp = type.GetProperty("Dpi", flags);
            if (dpiXProp != null && dpiYProp != null)
            {
                var x = (int)dpiXProp.GetValue(null, null)!;
                var y = (int)dpiYProp.GetValue(null, null)!;
                return new DpiScale(x / 96d, y / 96d);
            }
            return new DpiScale(1, 1);
        }
    }
}
