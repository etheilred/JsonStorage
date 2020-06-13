using JsonStorage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tester
{
    class Program
    {
        private static Random random = new Random();

        static void Main(string[] args)
        {
            using (var sp = new StorageProvider(@"..\..\..\..\storage2"))
            {
                // sp.Write<A>(new A[] {new A(), new A(), new A(),});
                sp.Delete<A>(x => x.Id == 1);
            }
        }

        static void TestStorage()
        {
            using (var provider = new StorageProvider(@"..\..\..\..\storage"))
            {
                provider.Write(new A {Name = "A", Surname = "B"});
                provider.Write(new A {Name = "A", Surname = "B"});
                provider.Write(new A {Name = "A", Surname = "B"});
                provider.Write(new A {Name = "A", Surname = "B"});
            }
        }

        static string GenerateRandomStr(int length)
        {
            char[] symbols = new[]
            {
                Enumerable.Range(0, 'z' - 'a' + 1).Select(x => (char) ('a' + x)),
                Enumerable.Range(0, 'Z' - 'A' + 1).Select(x => (char) ('A' + x)),
                Enumerable.Range(0, '9' - '0' + 1).Select(x => (char) ('0' + x)),
            }.SelectMany(x => x.Select(y => (char) y)).ToArray();
            return new string(Enumerable
                .Range(0, length)
                .Select(x => symbols[random.Next(symbols.Length)])
                .ToArray());
        }

        class A
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }

            public override string ToString()
            {
                return $"{Id}. {Name} {Surname}";
            }
        }
    }
}