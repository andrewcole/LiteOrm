using System;
using System.Data.SQLite;
using System.Linq;

namespace Illallangi.LiteOrm
{
    public static class SQLiteInsertCommandExtensions
    {
        public static SQLiteInsertCommand Values(this SQLiteInsertCommand insert, string column, DateTime value)
        {
            insert.Values.Add(column, value.ToString("yyyy-MM-ddTHH:mm:ss"));
            return insert;
        }

        public static SQLiteInsertCommand Values(this SQLiteInsertCommand insert, string column, object value)
        {
            insert.Values.Add(column, value);
            return insert;
        }

        public static string GetColumnNames(this SQLiteInsertCommand insert)
        {
            return string.Join(", ", insert.Values.Keys);
        }

        public static string GetNamedArguments(this SQLiteInsertCommand insert)
        {
            return string.Join(", ", insert.Values.Keys.Select(k => string.Concat("@", k)));
        }

        public static string GetSql(this SQLiteInsertCommand insert)
        {
            return string.Format(
                "INSERT INTO {0}({1}) VALUES ({2});",
                insert.Table,
                insert.GetColumnNames(),
                insert.GetNamedArguments());
        }

        public static SQLiteCommand CreateCommand(this SQLiteInsertCommand insert)
        {
            var cm = insert.Connection.CreateCommand();
            cm.CommandText = insert.GetSql();
            foreach (var value in insert.Values)
            {
                cm.Parameters.Add(new SQLiteParameter(string.Concat("@", value.Key), value.Value));
            }

            return cm;
        }

        public static int Go(this SQLiteInsertCommand insert)
        {
            insert.CreateCommand().ExecuteNonQuery();
            using (var command = insert.Connection.CreateCommand())
            {
                command.CommandText = "SELECT last_insert_rowid() AS id";
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }
}