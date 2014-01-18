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

        private readonly IEnumerable<string> currentSqlSchemaLines;

        private readonly IEnumerable<string> currentSqlSchemaFiles;

        private readonly IEnumerable<string> currentPragmas;

        private readonly IEnumerable<string> currentExtensions;

        private readonly ILog currentLog;

        private IEnumerable<string> currentSqlSchema; 
        #endregion

        #region Constructor

        protected SQLiteContextBase(
                string databasePath,
                string connectionString = null,
                IEnumerable<string> sqlSchemaLines = null,
                IEnumerable<string> sqlSchemaFiles = null,
                IEnumerable<string> pragmas = null,
                IEnumerable<string> extensions = null,
                ILog log = null)
        {
            this.currentDatabasePath = databasePath;
            this.currentConnectionString = connectionString ?? @"data source=""{0}""";
            this.currentSqlSchemaLines = sqlSchemaLines ?? new List<string>();
            this.currentSqlSchemaFiles = sqlSchemaFiles ?? new List<string>();
            this.currentPragmas = pragmas ?? new List<string>();
            this.currentExtensions = extensions ?? new List<string>();
            this.currentLog = log ?? new NoOpLogger();
            
            this.Log.DebugFormat(
                    @"SQLiteContextBase(databasePath=""{0}"", connectionString=""{1}"", sqlSchemaLines=""{2}"", sqlSchemaFiles=""{3}"", pragmas=""{4}"", extensions=""{5}"", log = ""{6}"")",
                    this.DatabasePath,
                    this.ConnectionString,
                    this.SqlSchemaLines,
                    this.SqlSchemaFiles,
                    this.Pragmas,
                    this.Extensions,
                    this.Log);

            this.SetupSQLiteInterop();
        }

        #endregion

        #region Properties

        public ILog Log
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

        private IEnumerable<string> SqlSchemaLines
        {
            get
            {
                return this.currentSqlSchemaLines;
            }
        }

        private IEnumerable<string> SqlSchemaFiles
        {
            get
            {
                return this.currentSqlSchemaFiles;
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

        private IEnumerable<string> SqlSchema
        {
            get
            {
                return this.currentSqlSchema ?? (this.currentSqlSchema = this.GetSqlSchema());
            }
        }

        #endregion

        #region Methods

        #region Protected Methods

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
                    foreach (var line in this.SqlSchema)
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
            Debug.Assert(null != path, "null != Path.GetFullPath(SQLiteContextBase.DatabasePath)");

            var dir = Path.GetDirectoryName(path);
            Debug.Assert(null != dir, "null != Path.GetDirectoryName(Path.GetFullPath(SQLiteContextBase.DatabasePath))");

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

        private IEnumerable<string> GetSqlSchema()
        {
            return
                this.SqlSchemaFiles.Select(f => Path.GetFullPath(Environment.ExpandEnvironmentVariables(f)))
                    .Where(File.Exists)
                    .SelectMany(file => File.ReadAllText(file).Split(';'))
                    .Concat(this.SqlSchemaLines);
        }

        #endregion

        #endregion
    }
}