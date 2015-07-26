using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication4
{
    internal static class Extensions
    {
        private static void Add<K, V>(this IList<KeyValuePair<K, V>> inList, K first, V second)
        {
            inList.Add(new KeyValuePair<K, V>(first, second));
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            var list = new List<KeyValuePair<int, int>>() { { 1, 1 }, { 1, 2 }, { 2, 3 } };
        }
    }
}
