using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JsonStorage
{
    public class StorageProvider : IStorageReader, IStorageWriter, IDisposable
    {
        private const string ConfigPath = @"__storageConfig.json";

        private readonly string _content;
        private readonly string _storageDir;

        private Dictionary<string, object> _tablesCache = new Dictionary<string, object>();

        private StorageConfig Config { get; set; } = new StorageConfig();

        public StorageProvider(string storageDir = "", string content = "Content")
        {
            _content = content;
            _storageDir = storageDir;
            InitializeDirHierarchy();
            InitializeConfig();
        }

        private void InitializeDirHierarchy()
        {
            if (!Directory.Exists(Path.Combine(_storageDir)))
            {
                Directory.CreateDirectory(_storageDir);
            }

            string cPath = Path.Combine(_storageDir, _content);
            if (!Directory.Exists(cPath))
            {
                Directory.CreateDirectory(cPath);
            }
        }

        private void InitializeConfig()
        {
            // var path = $@"{_storageDir}\{ConfigPath}";
            var path = Path.Combine(_storageDir, ConfigPath);
            if (!File.Exists(path))
            {
                using var fs = File.Create(path);
                using var sw = new StreamWriter(fs);
                using var jw = new JsonTextWriter(sw);
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jw, Config);
            }
            else
            {
                using var fs = File.Open(path, FileMode.Open);
                using var sr = new StreamReader(fs);
                using var jr = new JsonTextReader(sr);
                JsonSerializer serializer = new JsonSerializer();
                Config = serializer.Deserialize<StorageConfig>(jr);
                if (Config == null) Config = new StorageConfig();
            }
        }

        public T Get<T>(Predicate<T> toGet, string tableName = null)
        {
            return GetInternalTable<T>(tableName).First(x => toGet(x));
        }

        public IEnumerable<T> GetTable<T>(string tableName = null)
        {
            return GetInternalTable<T>(tableName);
        }

        public bool Write<T>(T item, string tableName = null)
        {
            try
            {
                Table<T> table = GetInternalTable<T>(tableName);

                table.Insert(item);
                // string json = JsonConvert.SerializeObject(table);
                // File.WriteAllText($@"{_storageDir}\{_content}\{table.Name}.json", json);
                _tablesCache[table.Name] = table;
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public bool Write<T>(IEnumerable<T> items, string tableName = null)
        {
            try
            {
                Table<T> table = GetInternalTable<T>(tableName);
                table.Insert(items);
                _tablesCache[table.Name] = table;
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Table<T> GetInternalTable<T>(string tableName = null)
        {
            Table<T> table;
            if (_tablesCache.ContainsKey($"{typeof(T)}{tableName}"))
            {
                table = _tablesCache[$"{typeof(T)}{tableName}"] as Table<T>;
            }
            else if (Config.Tables.ContainsKey(typeof(T)))
            {
                if (!Config.Tables[typeof(T)].Contains(tableName))
                {
                    Config.Tables[typeof(T)].Add(tableName);
                    table = new Table<T>() { Name = $"{typeof(T)}{tableName}" };
                }
                else
                {
                    // string path = $@"{_storageDir}\{_content}\{typeof(T)}{tableName}.json";
                    string path = Path.Combine(_storageDir, _content, $"{typeof(T)}{tableName}.json");
                    if (!File.Exists(path))
                    {
                        table = new Table<T>()
                        {
                            Name = $"{typeof(T)}{tableName}",
                        };
                    }
                    else table = JsonConvert.DeserializeObject<Table<T>>(
                        File.ReadAllText(path));
                }
            }
            else
            {
                Config.Tables.Add(typeof(T), new List<string> { tableName });
                table = new Table<T>() { Name = $"{typeof(T)}{tableName}" };
            }

            var internalTable = table;
            if (internalTable != null)
            {
                return internalTable;
            }

            return new Table<T>() { Name = $"{typeof(T)}{tableName}" };
        }

        public void Delete<T>(Predicate<T> toDelete, string tableName = null)
        {
            var table = GetInternalTable<T>(tableName);
            table.Remove(toDelete);
            _tablesCache[table.Name] = table;
        }

        public void DeleteAll<T>(string tableName = null)
        {
            var table = GetInternalTable<T>(tableName);
            table.Clear();
            _tablesCache[table.Name] = table;
        }

        public void Close()
        {
            JsonSerializer serializer = new JsonSerializer();
            string pathToSw = Path.Combine(_storageDir, ConfigPath);
            using var sw = new StreamWriter(pathToSw);
            using var jw = new JsonTextWriter(sw);
            serializer.Serialize(jw, Config);
            foreach (var kv in _tablesCache)
            {
                if (kv.Value is IJsonSerializable serializable)
                {
                    string json = serializable.Serialize();
                    string serializedPath = Path.Combine(_storageDir, _content, $"{kv.Key}.json");
                    File.WriteAllText(serializedPath, json);
                }
            }
        }

        public void Dispose()
        {
            Close();
        }

        ~StorageProvider()
        {
            Dispose();
        }
    }

    internal class StorageConfig
    {
        private Dictionary<Type, List<string>> _tables = new Dictionary<Type, List<string>>();

        public Dictionary<Type, List<string>> Tables
        {
            get
            {
                if (_tables == null) _tables = new Dictionary<Type, List<string>>();
                return _tables;
            }
            set => _tables = value;
        }
    }
}