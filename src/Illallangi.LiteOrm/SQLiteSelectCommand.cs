using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Illallangi.LiteOrm
{
    public sealed class SQLiteSelectCommand<T> where T : new()
    {
        private readonly SQLiteConnection currentConnection;
        private readonly string currentTable;
        private readonly IDictionary<string, string> currentColumns;
        private readonly IDictionary<string, Action<T, float>> currentFloatMap;
        private readonly IDictionary<string, Action<T, int>> currentIntMap;
        private readonly IDictionary<string, Action<T, string>> currentStringMap;
        private readonly IDictionary<string, Action<T, DateTime>> currentDateMap;

        public SQLiteSelectCommand(SQLiteConnection connection, string table)
        {
            this.currentConnection = connection;
            this.currentTable = table;
            this.currentDateMap = new Dictionary<string, Action<T, DateTime>>();
            this.currentColumns = new Dictionary<string, string>();
            this.currentFloatMap = new Dictionary<string, Action<T, float>>();
            this.currentIntMap = new Dictionary<string, Action<T, int>>();
            this.currentStringMap = new Dictionary<string, Action<T, string>>();
        }

        public SQLiteConnection Connection
        {
            get
            {
                return this.currentConnection;
            }
        }

        public string Table
        {
            get
            {
                return this.currentTable;
            }
        }

        public IDictionary<string, string> Columns
        {
            get
            {
                return this.currentColumns;
            }
        }

        public IDictionary<string, Action<T, float>> FloatMap
        {
            get { return this.currentFloatMap; }
        }

        public IDictionary<string, Action<T, int>> IntMap
        {
            get { return this.currentIntMap; }
        }

        public IDictionary<string, Action<T, string>> StringMap
        {
            get { return this.currentStringMap; }
        }

        public IDictionary<string, Action<T, DateTime>> DateMap
        {
            get { return this.currentDateMap; }
        }
    }
}