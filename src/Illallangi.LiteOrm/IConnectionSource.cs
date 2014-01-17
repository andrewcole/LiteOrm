using System.Data.SQLite;

namespace Illallangi.LiteOrm
{
    public interface IConnectionSource
    {
        SQLiteConnection GetConnection();
    }
}