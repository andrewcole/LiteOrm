using System.Collections.Generic;
using System.Data.SQLite;

namespace Illallangi.LiteOrm
{
    public sealed class SQLiteDeleteCommand
    {
        private readonly SQLiteConnection currentConnection;
        private readonly string currentTable;
        private readonly IDictionary<string, object> currentColumns;

        public SQLiteDeleteCommand(SQLiteConnection connection, string table)
        {
            this.currentConnection = connection;
            this.currentTable = table;
            this.currentColumns = new Dictionary<string, object>();
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

        public IDictionary<string, object> Columns
        {
            get
            {
                return this.currentColumns;
            }
        }
    }
}