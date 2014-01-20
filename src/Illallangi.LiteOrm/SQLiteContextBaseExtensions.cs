using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Illallangi.LiteOrm
{
    public static class SQLiteContextBaseExtensions
    {
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
            }
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetDllDirectory(string lpPathName);
    }
}