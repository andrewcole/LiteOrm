using System.Collections.Generic;
using System.Data.SQLite;
using Common.Logging;
    
namespace Illallangi.LiteOrm
{
    public abstract class RepositoryBase<T> : SQLiteContextBase, IRepository<T> where T : class
    {
        #region Methods

        protected RepositoryBase(IEnumerable<string> pragmas, IEnumerable<string> extensions, IEnumerable<string> sqlSchema, string databasePath, string connectionString, ILog log = null)
            : base(pragmas, extensions, sqlSchema, databasePath, connectionString, log)
        {
        }

        public abstract T Create(T obj);

        public abstract IEnumerable<T> Retrieve(T obj = null);

        public abstract T Update(T obj);

        public abstract void Delete(T obj);

        #endregion
    }
}