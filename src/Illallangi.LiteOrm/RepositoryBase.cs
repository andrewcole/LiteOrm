using System.Collections.Generic;
using System.Data.SQLite;
using Common.Logging;
    
namespace Illallangi.LiteOrm
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        #region Fields

        private readonly IConnectionSource currentConnectionSource;

        private readonly ILog currentLog;

        #endregion

        #region Constructors

        protected RepositoryBase(IConnectionSource connectionSource, ILog log)
        {
            this.currentConnectionSource = connectionSource;
            this.currentLog = log;
            this.Log.DebugFormat(
                @"RepositoryBase(connectionSource=""{0}"", log=""{1}"")",
                this.ConnectionSource,
                this.Log);
        }

        #endregion

        #region Properties

        #region Protected Properties

        protected ILog Log
        {
            get { return this.currentLog; }
        }

        #endregion

        #region Private Properties

        private IConnectionSource ConnectionSource
        {
            get { return this.currentConnectionSource; }
        }

        #endregion

        #endregion

        #region Methods

        public abstract T Create(T obj);

        public abstract IEnumerable<T> Retrieve(T obj = null);

        public abstract T Update(T obj);

        public abstract void Delete(T obj);

        protected SQLiteConnection GetConnection()
        {
            return this.ConnectionSource.GetConnection();
        }

        #endregion
    }
}