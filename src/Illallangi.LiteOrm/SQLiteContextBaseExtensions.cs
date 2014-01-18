using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Illallangi.LiteOrm
{
    using System;

    public static class SQLiteContextBaseExtensions
    {
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode, SetLastError = true)]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        private const string FileName = @"SQLite.Interop.dll";

        // http://stackoverflow.com/questions/13028069/unable-to-load-dll-sqlite-interop-dll
        public static void SetupSQLiteInterop(this SQLiteContextBase sx)
        {
            sx.Log.DebugFormat(@"Searching for SQLite.Interop.dll");
            
            var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Debug.Assert(directoryName != null, "directoryName != null");
            
            var current = Path.Combine(directoryName, SQLiteContextBaseExtensions.FileName);

            if (File.Exists(current))
            {
                sx.Log.DebugFormat(@"Found at ""{0}"", no action required", current);
                return;
            }

            var architecture = Path.Combine(directoryName, (IntPtr.Size == 4) ? "x86" : "x64", SQLiteContextBaseExtensions.FileName);

            if (File.Exists(architecture))
            {
                sx.Log.DebugFormat(@"Found at ""{0}"", calling SetDllDirectory", architecture);
                SetDllDirectory(Path.Combine(directoryName, (IntPtr.Size == 4) ? "x86" : "x64"));
                return;
            }
        }
    }
}