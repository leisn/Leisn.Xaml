// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Leisn.Xaml.Wpf.Extensions
{
    public class RectClips : ISet<Rect>, IList<Rect>
    {
        private readonly List<Rect> _list = new();

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public Rect this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public void Insert(int index, Rect item)
        {
            int i = _list.IndexOf(item);
            if (i != -1)
            {
                _list.RemoveAt(i);
                index = Math.Clamp(index, 0, Count - 1);
            }
            _list.Insert(index, item);
        }

        public bool Add(Rect item)
        {
            int index = _list.IndexOf(item);
            if (index != -1)
            {
                return false;
            }

            _list.Add(item);
            return true;
        }

        public void Add(params Rect[] items)
        {
            AddAll(items, false);
        }

        public void AddIfNotEmpty(params Rect[] items)
        {
            AddAll(items, true);
        }

        public void AddAll(IEnumerable<Rect> items, bool ignoreEmpty = true)
        {
            foreach (Rect item in items)
            {
                if (!(ignoreEmpty && item.IsEmpty()))
                {
                    Add(item);
                }
            }
        }

        public bool MergeItem(params Rect[] items)
        {
            return MergeItems(items);
        }

        /// <summary>
        /// 添加对象，后对集合中每一项尝试合并为更大的矩形，
        /// 直到当前集合内容不能再合并
        /// </summary>
        /// <returns> 只添加返回true，合并false</returns>
        public bool MergeItems(IEnumerable<Rect> items)
        {
            AddAll(items, true);
            int oldCount = Count;
            int newCount = MergeEachOthers();
            return newCount == oldCount;
        }

        /// <summary>
        /// 整理合并集合中所有项,使每一项不能再互相合并
        /// </summary>
        /// <returns>当前集合的项总数</returns>
        public int MergeEachOthers()
        {
            for (int i = Count - 1; i > 0; i--)
            {
                bool merged = false;
                for (int j = i - 1; j >= 0; j--)
                {
                    if (_list[j].TryMerge(_list[i], out Rect target))
                    {
                        _list[j] = target;
                        merged = true;
                    }
                }
                if (merged)//被合并就没有必要存在了
                {
                    _list.RemoveAt(i);
                }
            }
            return Count;
        }

        /// <summary>
        /// 对集合中所有项进行剪切，排除剪切区域，合并其他区域到集合
        /// </summary>
        public void CutThenMergeOthers(Rect target, params Rect[] attachedItems)
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                (bool Clipped, RectClip ClipResult) x = _list[i].Clip(target);
                if (x.Clipped)
                {
                    _list.RemoveAt(i);
                    AddIfNotEmpty(x.ClipResult.Clips);
                }
            }
            AddIfNotEmpty(attachedItems);
            MergeEachOthers();
        }


        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public void Sort(Comparison<Rect> comparison)
        {
            _list.Sort(comparison);
        }
        public void Sort(IComparer<Rect> comparer)
        {
            _list.Sort(comparer);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(Rect item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(Rect[] array, int arrayIndex)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                array[i + arrayIndex] = _list[i];
            }
        }

        public void ExceptWith(IEnumerable<Rect> other)
        {
            foreach (Rect item in other)
            {
                _list.Remove(item);
            }
        }

        public int IndexOf(Rect item)
        {
            return _list.IndexOf(item);
        }

        public bool GetIfExists(Rect item, out Rect temp)
        {
            int index = IndexOf(item);
            if (index != -1)
            {
                temp = _list[index];
                return true;
            }
            temp = default;
            return false;
        }

        public IEnumerator<Rect> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void IntersectWith(IEnumerable<Rect> other)
        {
            List<Rect> temp = new(other);
            foreach (Rect item in temp)
            {
                if (!_list.Contains(item))
                {
                    _list.Remove(item);
                }
            }
        }

        public bool IsProperSubsetOf(IEnumerable<Rect> other)
        {
            List<Rect> temp = new(other);
            foreach (Rect item in _list)
            {
                if (!temp.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsProperSupersetOf(IEnumerable<Rect> other)
        {
            foreach (Rect item in other)
            {
                if (!_list.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsSubsetOf(IEnumerable<Rect> other)
        {
            return IsProperSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<Rect> other)
        {
            return IsProperSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<Rect> other)
        {
            foreach (Rect item in other)
            {
                if (_list.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Remove(Rect item)
        {
            return _list.Remove(item);
        }

        public bool SetEquals(IEnumerable<Rect> other)
        {
            List<Rect> temp = new(other);
            if (temp.Count != _list.Count)
            {
                return false;
            }

            foreach (Rect item in temp)
            {
                if (!_list.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        public void SymmetricExceptWith(IEnumerable<Rect> other)
        {
            List<Rect> temp = new(other);
            foreach (Rect item in temp)
            {
                if (_list.Contains(item))
                {
                    _list.Remove(item);
                }
                else
                {
                    _list.Add(item);
                }
            }
        }

        public void UnionWith(IEnumerable<Rect> other)
        {
            if (other == null)
            {
                return;
            }

            foreach (Rect item in other)
            {
                int index = _list.IndexOf(item);
                if (index != -1)
                {
                    _list[index] = item;
                }
                else
                {
                    _list.Add(item);
                }
            }
        }

        void ICollection<Rect>.Add(Rect item)
        {
            Add(item);
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }
}
