using System.Collections.Generic;
using System.Data.SQLite;

namespace Illallangi.LiteOrm
{
    public sealed class SQLiteInsertCommand
    {
        private readonly SQLiteConnection currentConnection;
        private readonly string currentTable;
        private readonly IDictionary<string, object> currentValues;
 
        public SQLiteInsertCommand(SQLiteConnection connection, string table)
        {
            this.currentConnection = connection;
            this.currentTable = table;
            this.currentValues = new Dictionary<string, object>();
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

        public IDictionary<string, object> Values
        {
            get
            {
                return this.currentValues;
            }
        }
    }
}