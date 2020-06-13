using System.Collections.Generic;
using System.Linq;
using JsonStorage;
using NUnit.Framework;

namespace Tests
{
    public class Tests
    {
        private StorageProvider _provider;
        private const string PathToStorageDir = "storage";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestWrite()
        {
            using (_provider = new StorageProvider(PathToStorageDir))
            {
                _provider.Write(new C {A = 1, B = 2});
                _provider.Write(new C {A = 2, B = 3});
                _provider.Write(new C {A = 3, B = 4});
                Assert.AreEqual(3, _provider.Get<C>(x => x.Id == 3).A);
                _provider.DeleteAll<C>();
            }
        }

        [Test]
        public void TestWriteRange()
        {
            using (_provider = new StorageProvider(PathToStorageDir))
            {
                _provider.Write<int>(new[]
                {
                    1, 2, 3, 4, 5
                });
                Assert.AreEqual(new[] {1,2,3,4,5}, _provider.GetTable<int>().ToArray());
                _provider.DeleteAll<int>();
            }
        }

        private class C
        {
            public int A { get; set; }
            public int B { get; set; }
            public int Id { get; set; }
        }
    }
}