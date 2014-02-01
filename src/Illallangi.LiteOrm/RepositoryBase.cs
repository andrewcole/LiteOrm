using System.Collections.Generic;
using Common.Logging;
    
namespace Illallangi.LiteOrm
{
    public abstract class RepositoryBase<T> : SQLiteContextBase, IRepository<T> where T : class
    {
        #region Constructor

        protected RepositoryBase(
                string databasePath,
                string connectionString = null,
                IEnumerable<string> sqlSchemaLines = null,
                IEnumerable<string> sqlSchemaFiles = null,
                IEnumerable<string> pragmas = null,
                IEnumerable<string> extensions = null,
                ILog log = null)
            : base(
                databasePath, 
                connectionString,
                sqlSchemaLines,
                sqlSchemaFiles,
                pragmas, 
                extensions, 
                log)
        {
        }

        #endregion

        #region Methods

        public abstract int Import(params T[] objs);

        public abstract IEnumerable<T> Create(params T[] objs);

        public abstract IEnumerable<T> Retrieve(T obj = null);

        public abstract T Update(T obj);

        public abstract void Delete(params T[] objs);

        #endregion
    }
}