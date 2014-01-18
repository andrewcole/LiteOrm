using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common.Logging;
using Common.Logging.Simple;

namespace Illallangi.LiteOrm
{
    public abstract class SQLiteContextBase
    {
        #region Fields

        private readonly string currentDatabasePath;

        private readonly string currentConnectionString;

        private readonly IEnumerable<string> currentSqlSchema;
        
        private readonly IEnumerable<string> currentPragmas;

        private readonly IEnumerable<string> currentExtensions;

        private readonly ILog currentLog;

        #endregion

        #region Constructor

        protected SQLiteContextBase(
                string databasePath,
                string connectionString,
                IEnumerable<string> sqlSchema,
                IEnumerable<string> pragmas = null,
                IEnumerable<string> extensions = null,
                ILog log = null)
        {
            this.currentDatabasePath = databasePath;
            this.currentConnectionString = connectionString;
            this.currentSqlSchema = sqlSchema;
            this.currentPragmas = pragmas;
            this.currentExtensions = extensions;
            this.currentLog = log ?? new NoOpLogger();

            this.Log.DebugFormat(
                    @"SQLiteContextBase(databasePath=""{0}"", connectionString=""{1}"", sqlSchema=""{2}"", pragmas=""{3}"", extensions=""{4}"", log = ""{5}"")",
                    this.DatabasePath,
                    this.ConnectionString,
                    this.SqlSchema,
                    this.Pragmas,
                    this.Extensions,
                    this.Log);
        }

        #endregion

        #region Properties

        protected ILog Log
        {
            get
            {
                return this.currentLog;
            }
        }

        private IEnumerable<string> Pragmas
        {
            get
            {
                return this.currentPragmas;
            }
        }

        private IEnumerable<string> Extensions
        {
            get
            {
                return this.currentExtensions;
            }
        }

        private IEnumerable<string> SqlSchema
        {
            get
            {
                return this.currentSqlSchema;
            }
        }

        private string DatabasePath
        {
            get
            {
                return this.currentDatabasePath;
            }
        }

        private string ConnectionString
        {
            get
            {
                return this.currentConnectionString;
            }
        }

        #endregion

        #region Methods

        #region Public Methods

        protected SQLiteConnection GetConnection()
        {
            if (!File.Exists(this.GetDbPath()))
            {
                using (var conn = new SQLiteConnection(this.GetConnectionString())
                    .OpenAndReturn()
                    .LoadAllExtensions(this.Extensions)
                    .SetAllPragmas(this.Pragmas)
                    .WithLogger(this.Log))
                {
                    foreach (
                        var line in
                            this.SqlSchema
                                .Select(f => Path.GetFullPath(Environment.ExpandEnvironmentVariables(f)))
                                .Where(File.Exists)
                                .SelectMany(file => File.ReadAllText(file).Split(';')))
                    {
                        new SQLiteCommand(line, conn).ExecuteNonQuery();
                    }
                }
            }

            return new SQLiteConnection(this.GetConnectionString())
                .OpenAndReturn()
                .LoadAllExtensions(this.Extensions)
                .SetAllPragmas(this.Pragmas)
                .WithLogger(this.Log);
        }

        #endregion

        #region Private Methods

        private string GetDbPath()
        {
            var path = Path.GetFullPath(Environment.ExpandEnvironmentVariables(this.DatabasePath));
            Debug.Assert(null != path, "null == Path.GetFullPath(SQLiteConnectionSource.DatabasePath)");

            var dir = Path.GetDirectoryName(path);
            Debug.Assert(null != dir, "null == Path.GetDirectoryName(Path.GetFullPath(SQLiteConnectionSource.DatabasePath))");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return path;
        }

        private string GetConnectionString()
        {
            return string.Format(this.ConnectionString, this.GetDbPath());
        }

        #endregion

        #endregion
    }
}