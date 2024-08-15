using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Microsoft.Windows.MixedReality.DotNetWinRT
{
    [InitializeOnLoad]
    internal class Init
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern int AddDllDirectory(string lpPathName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibraryExW([MarshalAs(UnmanagedType.LPWStr)] string fileName, IntPtr fileHandle, uint flags);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeLibrary(IntPtr moduleHandle);

        const uint LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;
        const string MONO_SUPPORT = "MonoSupport.dll";

        static Init()
        {
            IntPtr modulePtr = LoadLibraryExW(MONO_SUPPORT, IntPtr.Zero, LOAD_LIBRARY_SEARCH_DEFAULT_DIRS);
            if (modulePtr != IntPtr.Zero)
            {
                // DLL search paths already configured in this process; nothing more to do.
                FreeLibrary(modulePtr);
                return;
            }

            // Find the path to the MonoSupport DLL
            var assets = AssetDatabase.FindAssets(Path.GetFileNameWithoutExtension(MONO_SUPPORT));
            if (assets.Length != 1)
            {
                Debug.LogError($"Failed to find single asset for {MONO_SUPPORT}; found {assets.Length} instead!");
                return;
            }

            string dllDirectory = Path.GetDirectoryName(Path.GetFullPath(AssetDatabase.GUIDToAssetPath(assets[0])));

            if (AddDllDirectory(dllDirectory) == 0)
            {
                Debug.LogError($"Failed to set DLL directory {dllDirectory}: Win32 error {Marshal.GetLastWin32Error()}");
                return;
            }

            Debug.Log(string.Format("Added DLL directory {0} to the user search path.", dllDirectory));
        }
    }
}