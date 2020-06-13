using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace JsonStorage
{
    [JsonObject]
    internal class Table<T> : IEnumerable<T>, IJsonSerializable
    {
        [JsonProperty]
        private int _id = 0;
        bool _setId = false;

        [JsonProperty] private List<T> Collection { get; set; } = new List<T>();

        public void Insert(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Insert(item);
            }
        }

        public void Insert(T item)
        {
            if (_setId)
            { 
                typeof(T).GetProperty("Id").SetValue(item, ++_id);
            }
            Collection.Add(item);
        }

        public void Clear()
        {
            Collection.Clear();
            _id = 0;
        }

        public void Remove(Predicate<T> toDelete)
            => Collection.RemoveAt(Collection.FindIndex(toDelete));

        public Table()
        {
            if (typeof(T).GetProperties().Count(x => x.Name == "Id"
                                                     && x.PropertyType == typeof(int)) != 0)
            {
                _setId = true;
                // _id = Collection.Any() ? Collection.Max(x => (int)typeof(T).GetProperty("Id").GetValue(x)) + 1 : 0;
            }
        }

        public string Name { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

internal interface IJsonSerializable
{
    string Serialize();
}