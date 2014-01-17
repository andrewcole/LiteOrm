using System.Collections.Generic;

namespace Illallangi.LiteOrm
{
    public interface ILiteOrmConfig
    {
        string DbPath { get; }

        string ConnectionString { get; }

        IEnumerable<string> Extensions { get; }

        IEnumerable<string> Pragmas { get; }

        IEnumerable<string> SqlSchema { get; }
    }
}