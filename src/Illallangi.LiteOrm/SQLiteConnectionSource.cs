using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common.Logging;

namespace Illallangi.LiteOrm
{
    public sealed class SQLiteConnectionSource : IConnectionSource
    {
        #region Fields

        private readonly ILiteOrmConfig currentLiteOrmConfig;
        private readonly ILog currentLog;

        #endregion

        #region Constructors

        public SQLiteConnectionSource(ILiteOrmConfig liteOrmConfig, ILog log)
        {
            this.currentLiteOrmConfig = liteOrmConfig;
            this.currentLog = log;
        }

        #endregion

        #region Properties

        private ILiteOrmConfig LiteOrmConfig
        {
            get
            {
                return this.currentLiteOrmConfig;
            }
        }

        private ILog Log
        {
            get { return this.currentLog; }
        }

        #endregion

        #region Methods
        
        #region Public Methods

        public SQLiteConnection GetConnection()
        {
            if (!File.Exists(this.GetDbPath()))
            {
                using (var conn = new SQLiteConnection(this.GetConnectionString())
                    .OpenAndReturn()
                    .LoadAllExtensions(this.LiteOrmConfig.Extensions)
                    .SetAllPragmas(this.LiteOrmConfig.Pragmas)
                    .WithLogger(this.Log))
                {
                    foreach (
                        var line in
                            this.LiteOrmConfig
                                .SqlSchema
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
                .LoadAllExtensions(this.LiteOrmConfig.Extensions)
                .SetAllPragmas(this.LiteOrmConfig.Pragmas)
                .WithLogger(this.Log);
        }

        #endregion
        
        #region Private Methods

        private string GetDbPath()
        {
            var path = Path.GetFullPath(Environment.ExpandEnvironmentVariables(this.LiteOrmConfig.DbPath));
            Debug.Assert(path != null, "dbPath != null");

            var dir = Path.GetDirectoryName(path);
            Debug.Assert(dir != null, "dbDirectory != null");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return path;
        }

        private string GetConnectionString()
        {
            return string.Format(this.LiteOrmConfig.ConnectionString, this.GetDbPath());
        }

        #endregion

        #endregion
    }
}