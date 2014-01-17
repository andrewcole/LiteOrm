using System.Data.SQLite;
using System.Linq;

namespace Illallangi.LiteOrm
{
    using System.Diagnostics.CodeAnalysis;

    public static class SQLiteDeleteCommandExtensions
    {
        public static SQLiteDeleteCommand Where(this SQLiteDeleteCommand delete, string column, object value = null)
        {
            delete.Columns.Add(column, value);
            return delete;
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        public static string GetWhereClause(this SQLiteDeleteCommand delete)
        {
            return delete.Columns.Any(kvp => null != kvp.Value)
                ? string.Concat(
                    " WHERE ",
                    string.Join(
                        " AND ", 
                        delete.Columns
                              .Where(kvp => null != kvp.Value)
                              .Select(kvp => string.Format("[{0}].[{1}]=@{1}", delete.Table, kvp.Key))))
                : string.Empty;
        }

        public static string GetSql(this SQLiteDeleteCommand delete)
        {
            return string.Format(
                "DELETE FROM {0}{1};",
                delete.Table,
                delete.GetWhereClause());
        }

        public static SQLiteCommand CreateCommand(this SQLiteDeleteCommand delete)
        {
            var cm = delete.Connection.CreateCommand();
            cm.CommandText = delete.GetSql();
            foreach (var column in delete.Columns.Where(kvp => null != kvp.Value))
            {
                cm.Parameters.Add(new SQLiteParameter(string.Concat("@", column.Key), column.Value));
            }

            return cm;
        }
    }
}