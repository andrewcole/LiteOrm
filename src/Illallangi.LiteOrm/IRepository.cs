namespace Illallangi.LiteOrm
{
    using System.Collections.Generic;

    public interface IRepository<T> where T : class
    {
        T Create(T obj);

        IEnumerable<T> Retrieve(T obj = null);

        T Update(T obj);

        void Delete(T obj);
    }
}