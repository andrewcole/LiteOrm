using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using Ninject.Extensions.Logging;

namespace Illallangi.LiteOrm
{
    public static class SQLiteConnectionExtensions
    {
        public static SQLiteInsertCommand InsertInto(this SQLiteConnection cx, string table)
        {
            return new SQLiteInsertCommand(cx, table);
        }

        public static SQLiteSelectCommand<T> Select<T>(this SQLiteConnection cx, string table) where T : new()
        {
            return new SQLiteSelectCommand<T>(cx, table);
        }

        public static SQLiteDeleteCommand DeleteFrom(this SQLiteConnection cx, string table)
        {
            return new SQLiteDeleteCommand(cx, table);
        }

        public static SQLiteConnection LoadAllExtensions(this SQLiteConnection cx, IEnumerable<string> extensions)
        {
            foreach (var extension in extensions)
            {
                cx.LoadExtension(Path.GetFullPath(extension));
            }

            return cx;
        }

        public static SQLiteConnection SetAllPragmas(this SQLiteConnection cx, IEnumerable<string> pragmas)
        {
            foreach (var pragma in pragmas)
            {
                new SQLiteCommand(string.Format("pragma {0};", pragma), cx).ExecuteNonQuery();
            }

            return cx;
        }

        public static SQLiteConnection WithLogger(this SQLiteConnection cx, ILogger logger)
        {
            cx.Trace += (o, a) => logger.Debug(@"Executing ""{0}""", a.Statement);
            return cx;
        }
    }
}