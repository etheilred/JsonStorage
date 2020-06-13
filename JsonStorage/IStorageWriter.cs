using System;
using System.Collections;
using System.Collections.Generic;

namespace JsonStorage
{
    public interface IStorageWriter
    {
        bool Write<T>(T item, string tableName = null);
        bool Write<T>(IEnumerable<T> items, string tableName = null);
        void Delete<T>(Predicate<T> toDelete, string tableName = null);
        void DeleteAll<T>(string tableName = null);
    }
}