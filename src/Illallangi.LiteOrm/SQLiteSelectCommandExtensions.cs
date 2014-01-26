using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace Illallangi.LiteOrm
{
    using System.Diagnostics.CodeAnalysis;

    public static class SQLiteSelectCommandExtensions
    {
        private static readonly Dictionary<SelectType, string> SelectTypes = 
            new Dictionary<SelectType, string>
                        {
                            { SelectType.Equals, @"=" },
                            { SelectType.Like, @" LIKE " },
                            { SelectType.Glob, @" GLOB " },
                        };

        #region Methods

        #region Public Methods

        public static SQLiteSelectCommand<T> FloatColumn<T>(this SQLiteSelectCommand<T> select, string column, Action<T, float> func, float? value = null) where T : new()
        {
            select.FloatMap.Add(column, func);
            return select.Column(column, value);
        }

        public static SQLiteSelectCommand<T> Column<T>(this SQLiteSelectCommand<T> select, string column, Action<T, int> func, int? value = null) where T : new()
        {
            select.IntMap.Add(column, func);
            return select.Column(column, value);
        }

        public static SQLiteSelectCommand<T> Column<T>(this SQLiteSelectCommand<T> select, string column, Action<T, DateTime> func, string value = null) where T : new()
        {
            select.DateMap.Add(column, func);
            return select.Column(column, value);
        }

        public static SQLiteSelectCommand<T> Column<T>(this SQLiteSelectCommand<T> select, string column, Action<T, string> func, string value = null, SelectType selectType = SelectType.Equals) where T : new()
        {
            select.StringMap.Add(column, func);
            return select.Column(column, value, selectType);
        }
        
        public static IEnumerable<T> Go<T>(this SQLiteSelectCommand<T> select) where T : new()
        {
            using (var command = select.CreateCommand())
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.HasRows && reader.Read())
                    {
                        var result = new T();
                        foreach (var kvm in select.IntMap)
                        {
                            var int32 = reader.GetInt32(reader.GetOrdinal(kvm.Key));
                            kvm.Value(result, int32);
                        }

                        foreach (var kvm in select.FloatMap)
                        {
                            kvm.Value(result, reader.GetFloat(reader.GetOrdinal(kvm.Key)));
                        }

                        foreach (var kvm in select.StringMap)
                        {
                            var ordinal = reader.GetOrdinal(kvm.Key);
                            if (!reader.IsDBNull(ordinal))
                            {
                                kvm.Value(result, reader.GetString(ordinal));
                            }
                        }

                        foreach (var kvm in select.DateMap)
                        {
                            kvm.Value(result, reader.GetDateTime(reader.GetOrdinal(kvm.Key)));
                        }

                        yield return result;
                    }
                }
            }
        }

        public static string GetColumnNames<T>(this SQLiteSelectCommand<T> select) where T : new()
        {
            return string.Join(", ", select.Columns.Keys.Select(k => string.Format("[{0}].[{1}] as {1}", select.Table, k)));
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines", Justification = "Reviewed. Suppression is OK here.")]
        public static string GetWhereClause<T>(this SQLiteSelectCommand<T> select) where T : new()
        {
            return select.Columns.Any(kvp => null != kvp.Value.Item1)
                ? string.Concat(
                    " WHERE ",
                    string.Join(
                        " AND ",
                        select.Columns
                              .Where(kvp => null != kvp.Value.Item1)
                              .Select(kvp => string.Format(
                                  "[{0}].[{1}]{2}@{1}", 
                                  select.Table, 
                                  kvp.Key, 
                                  SQLiteSelectCommandExtensions.SelectTypes[kvp.Value.Item2]))))
                : string.Empty;
        }

        public static string GetSql<T>(this SQLiteSelectCommand<T> select) where T : new()
        {
            return string.Format(
                "SELECT {0} FROM {1}{2};",
                select.GetColumnNames(),
                select.Table,
                select.GetWhereClause());
        }

        public static SQLiteCommand CreateCommand<T>(this SQLiteSelectCommand<T> select) where T : new()
        {
            var cm = select.Connection.CreateCommand();
            cm.CommandText = select.GetSql();
            foreach (var column in select.Columns.Where(kvp => null != kvp.Value.Item1))
            {
                cm.Parameters.Add(new SQLiteParameter(string.Concat("@", column.Key), column.Value.Item1));
            }

            return cm;
        }

        #endregion

        #region Private Methods

        private static SQLiteSelectCommand<T> Column<T>(this SQLiteSelectCommand<T> select, string column, string value = null, SelectType selectType = SelectType.Equals) where T : new()
        {
            select.Columns.Add(column, new Tuple<string, SelectType>(string.IsNullOrWhiteSpace(value) ? null : value, selectType));
            return select;
        }

        private static SQLiteSelectCommand<T> Column<T>(this SQLiteSelectCommand<T> select, string column, object value = null, SelectType selectType = SelectType.Equals) where T : new()
        {
            select.Columns.Add(column, new Tuple<string, SelectType>(null == value ? null : value.ToString(), selectType));
            return select;
        }

        #endregion

        #endregion
    }
}