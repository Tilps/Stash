using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
    class A : IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("A");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            using (A a = new A(), b = new A())
            {
            }
            Console.ReadKey();
        }
    }
}
