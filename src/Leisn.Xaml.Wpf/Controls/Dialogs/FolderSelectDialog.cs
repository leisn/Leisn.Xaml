// By Leisn (https://leisn.com , https://github.com/leisn)

using Microsoft.Win32;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;

namespace Leisn.Xaml.Wpf.Controls.Dialogs
{
    public sealed class FolderSelectDialog : CommonDialog
    {
        private Environment.SpecialFolder _rootFolder;
        private string _selectedFolder;
        private string _descriptionText;
        private Win32Apis.BrowseCallbackProc? _callback;

        public FolderSelectDialog()
        {
            Reset();
        }

        public bool ShowNewFolderButton { get; set; }
        public string SelectedFolder
        {
            get => _selectedFolder;
            set => _selectedFolder = value is null ? string.Empty : value;
        }
        public Environment.SpecialFolder RootFolder
        {
            get => _rootFolder;
            set
            {
                if (!Enum.IsDefined(typeof(Environment.SpecialFolder), value))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Environment.SpecialFolder));
                }
                _rootFolder = value;
            }
        }
        public string Description
        {
            get => _descriptionText;
            set => _descriptionText = value is null ? string.Empty : value;
        }

        [MemberNotNull(nameof(_selectedFolder), nameof(_descriptionText))]
        public override void Reset()
        {
            _rootFolder = Environment.SpecialFolder.Desktop;
            _selectedFolder = string.Empty;
            _descriptionText = string.Empty;
            ShowNewFolderButton = true;
        }

        protected override bool RunDialog(IntPtr hWndOwner)
        {
            IntPtr pidlRoot = IntPtr.Zero;
            bool returnValue = false;

            _ = Win32Apis.SHGetSpecialFolderLocation(hWndOwner, (int)RootFolder, ref pidlRoot);
            if (pidlRoot == IntPtr.Zero)
            {
                _ = Win32Apis.SHGetSpecialFolderLocation(hWndOwner, Win32Apis.CSIDL_DESKTOP, ref pidlRoot);
                if (pidlRoot == IntPtr.Zero)
                {
                    throw new InvalidOperationException($"{RootFolder}");
                }
            }

            int mergedOptions = unchecked((int)(long)Win32Apis.BrowseInfos.NewDialogStyle);
            if (!ShowNewFolderButton)
            {
                mergedOptions += unchecked((int)(long)Win32Apis.BrowseInfos.HideNewFolderButton);
            }


            IntPtr pidlRet = IntPtr.Zero;
            IntPtr pszDisplayName = IntPtr.Zero;
            IntPtr pszSelectedPath = IntPtr.Zero;

            try
            {
                // Construct a BROWSEINFO
                Win32Apis.BROWSEINFO bi = new();
                pszDisplayName = Marshal.AllocHGlobal(Win32Apis.MAX_PATH * Marshal.SystemDefaultCharSize);
                pszSelectedPath = Marshal.AllocHGlobal((Win32Apis.MAX_PATH + 1) * Marshal.SystemDefaultCharSize);
                _callback = new Win32Apis.BrowseCallbackProc(OnBrowseCallbackProc);

                bi.pidlRoot = pidlRoot;
                bi.hwndOwner = hWndOwner;
                bi.pszDisplayName = pszDisplayName;
                bi.lpszTitle = Description;
                bi.ulFlags = mergedOptions;
                bi.lpfn = _callback;
                bi.lParam = IntPtr.Zero;
                bi.iImage = 0;

                // And show the dialog
                pidlRet = Win32Apis.SHBrowseForFolder(bi);

                if (pidlRet != IntPtr.Zero)
                {
                    // Then retrieve the path from the IDList
                    _ = Win32Apis.SHGetPathFromIDListLongPath(pidlRet, ref pszSelectedPath);

                    // set the flag to True before selectedPath is set to
                    // assure security check and avoid bogus race condition

                    // Convert to a string
                    SelectedFolder = Marshal.PtrToStringAuto(pszSelectedPath)!;

                    returnValue = true;
                }
            }
            finally
            {
                Win32Apis.CoTaskMemFree(pidlRoot);
                if (pidlRet != IntPtr.Zero)
                {
                    Win32Apis.CoTaskMemFree(pidlRet);
                }

                // Then free all the stuff we've allocated or the SH API gave us
                if (pszSelectedPath != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pszSelectedPath);
                }
                if (pszDisplayName != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pszDisplayName);
                }

                _callback = null;
            }
            return returnValue;
        }

        private int OnBrowseCallbackProc(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData)
        {
            switch (msg)
            {
                case Win32Apis.BFFM_INITIALIZED:
                    // Indicates the browse dialog box has finished initializing. The lpData value is zero. 
                    if (!string.IsNullOrEmpty(SelectedFolder))
                    {
                        // Try to select the folder specified by selectedPath
                        _ = Win32Apis.SendMessage(new HandleRef(null, hwnd), Win32Apis.BFFM_SETSELECTION, 1, SelectedFolder);
                    }
                    break;
                case Win32Apis.BFFM_SELCHANGED:
                    // Indicates the selection has changed. The lpData parameter points to the item identifier list for the newly selected item. 
                    IntPtr selectedPidl = lParam;
                    if (selectedPidl != IntPtr.Zero)
                    {
                        IntPtr pszSelectedPath = Marshal.AllocHGlobal((Win32Apis.MAX_PATH + 1) * Marshal.SystemDefaultCharSize);
                        // Try to retrieve the path from the IDList
                        bool isFileSystemFolder = Win32Apis.SHGetPathFromIDListLongPath(selectedPidl, ref pszSelectedPath);
                        Marshal.FreeHGlobal(pszSelectedPath);
                        _ = Win32Apis.SendMessage(new HandleRef(null, hwnd), Win32Apis.BFFM_ENABLEOK, 0, isFileSystemFolder ? 1 : 0);
                    }
                    break;
            }
            return 0;
        }
    }

    [SuppressUnmanagedCodeSecurity()]
    internal static class Win32Apis
    {
        public const string SHELL32 = "shell32.dll";
        public const string USER32 = "user32.dll";
        public const string OLE32 = "Ole32.dll";

        public const int S_OK = 0x00000000;
        public const int CSIDL_DESKTOP = 0x0000;
        public const int MAX_PATH = 260;
        public const int BFFM_INITIALIZED = 1;
        public const int BFFM_SELCHANGED = 2;
        public const int MAX_UNICODESTRING_LEN = short.MaxValue;
        public const int BFFM_ENABLEOK = 0x400 + 101;
        public const int BFFM_SETSELECTIONA = 0x400 + 102;
        public const int BFFM_SETSELECTIONW = 0x400 + 103;
        public static readonly int BFFM_SETSELECTION;

        static Win32Apis()
        {
            BFFM_SETSELECTION = Marshal.SystemDefaultCharSize == 1 ? BFFM_SETSELECTIONA : BFFM_SETSELECTIONW;
        }
        [Flags]
        public enum BrowseInfos
        {
            NewDialogStyle = 0x0040,   // Use the new dialog layout with the ability to resize
            HideNewFolderButton = 0x0200    // Don't display the 'New Folder' button
        }
        public delegate int BrowseCallbackProc(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData);

        [DllImport(OLE32, SetLastError = true, CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern void CoTaskMemFree(IntPtr pv);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class BROWSEINFO
        {
            public IntPtr hwndOwner; //HWND hwndOwner; // HWND of the owner for the dialog
            public IntPtr pidlRoot; //LPCITEMIDLIST pidlRoot; // Root ITEMIDLIST

            // For interop purposes, send over a buffer of MAX_PATH size. 
            public IntPtr pszDisplayName; //LPWSTR pszDisplayName; // Return display name of item selected.

            public string lpszTitle = null!; //LPCWSTR lpszTitle; // text to go in the banner over the tree.
            public int ulFlags; //UINT ulFlags; // Flags that control the return stuff
            public BrowseCallbackProc lpfn = null!; //BFFCALLBACK lpfn; // Call back pointer
            public IntPtr lParam; //LPARAM lParam; // extra info that's passed back in callbacks
            public int iImage; //int iImage; // output var: where to return the Image index.
        }

        [DllImport(USER32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);
        [DllImport(USER32, CharSet = CharSet.Unicode)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, string lParam);

        [DllImport(SHELL32)]
        [ResourceExposure(ResourceScope.None)]
        public static extern int SHGetSpecialFolderLocation(IntPtr hwnd, int csidl, ref IntPtr ppidl);
        //SHSTDAPI SHGetSpecialFolderLocation(HWND hwnd, int csidl, LPITEMIDLIST *ppidl);

        [DllImport(SHELL32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        private static extern bool SHGetPathFromIDListEx(IntPtr pidl, IntPtr pszPath, int cchPath, int flags);
        //SHSTDAPI_(BOOL) SHGetPathFromIDListW(LPCITEMIDLIST pidl, LPWSTR pszPath);

        public static bool SHGetPathFromIDListLongPath(IntPtr pidl, ref IntPtr pszPath)
        {
            int noOfTimes = 1;
            // This is how size was allocated in the calling method.
            _ = MAX_PATH * Marshal.SystemDefaultCharSize;
            int length = MAX_PATH;
            bool result;

            // SHGetPathFromIDListEx returns false in case of insufficient buffer.
            // This method does not distinguish between insufficient memory and an error. Until we get a proper solution,
            // this logic would work. In the worst case scenario, loop exits when length reaches unicode string length.
            while ((result = SHGetPathFromIDListEx(pidl, pszPath, length, 0)) == false
                    && length < MAX_UNICODESTRING_LEN)
            {
                string path = Marshal.PtrToStringAuto(pszPath)!;

                if (path.Length != 0 && path.Length < length)
                {
                    break;
                }

                noOfTimes += 2; //520 chars capacity increase in each iteration.
                length = noOfTimes * length >= MAX_UNICODESTRING_LEN
                    ? MAX_UNICODESTRING_LEN : noOfTimes * length;
                pszPath = Marshal.ReAllocHGlobal(pszPath, (IntPtr)((length + 1) * Marshal.SystemDefaultCharSize));
            }

            return result;
        }


        [DllImport(SHELL32, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.None)]
        public static extern IntPtr SHBrowseForFolder([In] BROWSEINFO lpbi);
    }
}
