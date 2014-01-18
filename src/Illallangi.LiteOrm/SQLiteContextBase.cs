﻿using System;
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

        private readonly ILog currentLog;

        private readonly IEnumerable<string> currentPragmas;

        private readonly IEnumerable<string> currentExtensions;

        private readonly IEnumerable<string> currentSqlSchema;

        private readonly string currentDatabasePath;

        private readonly string currentConnectionString;

        #endregion

        #region Constructors

        protected SQLiteContextBase(IEnumerable<string> pragmas, IEnumerable<string> extensions, IEnumerable<string> sqlSchema, string databasePath, string connectionString, ILog log = null)
        {
            this.currentPragmas = pragmas;
            this.currentExtensions = extensions;
            this.currentSqlSchema = sqlSchema;
            this.currentDatabasePath = databasePath;
            this.currentConnectionString = connectionString;
            this.currentLog = log ?? new NoOpLogger();

            this.Log.DebugFormat(
                    @"SQLiteConnectionSource(pragmas=""{0}"", extensions=""{1}"", sqlSchema=""{2}"", databasePath=""{3}"", connectionString=""{4}"", log = ""{5}"")",
                    this.Pragmas,
                    this.Extensions,
                    this.SqlSchema,
                    this.DatabasePath,
                    this.ConnectionString,
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