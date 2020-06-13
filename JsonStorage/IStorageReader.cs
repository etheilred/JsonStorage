using System;
using System.Collections.Generic;

namespace JsonStorage
{
    public interface IStorageReader
    {
        T Get<T>(Predicate<T> toGet, string tableName = null);
        IEnumerable<T> GetTable<T>(string tableName = null);
    }
}