﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Leisn.Xaml.Wpf.Locales
{
    public class Lang : INotifyPropertyChanged
    {
        private Lang() { }

        #region static

        private static Lang? _current;
        public static Lang Current => _current ??= new Lang();
        public static event Action? LangChanged;
        public static string Get(string key)
        {
            return Current.Values[key];
        }
        public static void ChangeLang(string lang)
        {
            Current.SetLang(lang);
            LangChanged?.Invoke();
        }
        public static void Initialize(string currentLang, string folder = "./locales", string fileFilter = "*.lang", string? defalutLang = null)
        {
            Current.LoadLocales(currentLang, folder, fileFilter, defalutLang);
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        public IReadOnlyList<string> Langs => _langDict.Keys.ToList();
        public IReadOnlyDictionary<string, string> Values => _values;
        public string CurrentLang => _currentLang;

        private string _currentLang = null!;
        private string _defaultLang = null!;

        private readonly LangDict _values = new();

        private readonly Dictionary<string, string> _langDict = new();

        private void SetLang(string lang)
        {
            lang = lang.ToLowerInvariant();
            if (!_langDict.ContainsKey(lang))
            {
                throw new ArgumentOutOfRangeException(nameof(lang));
            }

            _currentLang = lang;
            LoadValues(_langDict[lang]);
            NotifyLocalesChagned();
        }

        private void LoadLocales(string currentLang, string folder = "./locales", string fileFilter = "*.lang", string? defalutLang = null)
        {
            _defaultLang = string.IsNullOrEmpty(defalutLang) ? CultureInfo.CurrentCulture.Name.ToLower() : defalutLang;
            _currentLang = string.IsNullOrEmpty(currentLang) ? _defaultLang : currentLang;
            _values.Clear();
            _langDict.Clear();
            DirectoryInfo dir = new(folder);
            if (!dir.Exists)
            {
                return;
            }
            FileInfo[] files = dir.GetFiles(fileFilter);
            foreach (FileInfo item in files)
            {
                string langName = Path.GetFileNameWithoutExtension(item.Name);
                _langDict.Add(langName, item.FullName);
                if (langName.Equals(_currentLang))
                {
                    LoadValues(item.FullName);
                }
            }
        }

        private void LoadValues(string path)
        {
            string[] lines = File.ReadAllLines(path);
            int index;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#') || line.StartsWith("//"))
                {
                    continue;
                }

                try
                {
                    index = line.IndexOf('=');
                    _values[line[..index].Trim()] = line[(index + 1)..];
                }
                catch
                {
                    Debug.WriteLine($"Wrong format at line {i + 1} in {path} ");
                    throw;
                }
            }
        }

        private void NotifyLocalesChagned()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Values)));
        }

        private class LangDict : IReadOnlyDictionary<string, string>
        {
            private readonly Dictionary<string, string> _values = new();

            public string this[string key]
            {
                get
                {
                    if (_values.TryGetValue(key, out string? value))
                    {
                        return value;
                    }

                    return key;
                }
                set => _values[key] = value;
            }

            public IEnumerable<string> Keys => _values.Keys;

            public IEnumerable<string> Values => _values.Values;

            public int Count => _values.Count;

            public bool ContainsKey(string key)
            {
                return _values.ContainsKey(key);
            }

            public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            {
                return _values.GetEnumerator();
            }

            public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
            {
                return _values.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _values.GetEnumerator();
            }

            public void Clear()
            {
                _values.Clear();
            }
        }
    }
}
