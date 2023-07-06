using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leisn.Xaml.Wpf.Internals
{
    internal static class EditorUtil
    {

        private static readonly Dictionary<object, int> _editCounts = new();

        public static bool BeginEdit(object editor)
        {
            lock (_editCounts)
            {
                if (!_editCounts.TryGetValue(editor, out var count))
                {
                    _editCounts.Add(editor, 1);
                }
                else
                {
                    _editCounts[editor] = count + 1;
                }
                return _editCounts[editor] == 1;
            }
        }

        public static void EndEdit(object editor)
        {
            lock (_editCounts)
            {
                if (_editCounts.TryGetValue(editor, out var count))
                {
                    if (count == 1)
                        _editCounts.Remove(editor);
                    else
                        _editCounts[editor] = count - 1;
                }
            }
        }
    }
}
