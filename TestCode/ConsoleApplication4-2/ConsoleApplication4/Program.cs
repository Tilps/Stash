using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication4
{
    class Program
    {
        private struct TestObject
        {
            public TestObject(int a)
            {
                longValue = 3;
                longVolValue = 3;
                decimalValue = 3;
                decimalOptValue = 3;
                padding1 = 0;
                padding2 = 0;
                padding3 = 0;
            }

            private long padding1;
            private long padding2;
            private long padding3;
            private long longValue;
            private decimal decimalValue;
            private decimal? decimalOptValue;
            private long longVolValue;

            public long GetStub()
            {
                return 3;
            }

            public long GetLong()
            {
                return longValue;
            }

            public decimal GetDecimal()
            {
                return decimalValue;
            }

            public decimal? GetDecimalOpt()
            {
                return decimalOptValue;
            }

            public long GetLongVol()
            {
                return Interlocked.Read(ref longVolValue);
            }
        }

        static void Main(string[] args)
        {
            Test1();
            Test2();
            Test3();
            Test4();
            Test5();
            Test1();
            Test2();
            Test3();
            Test4();
            Test5();
            Test1();
            Test2();
            Test3();
            Test4();
            Test5();
            Test1();
            Test2();
            Test3();
            Test4();
            Test5();
        }

        private static void Test1()
        {
            GC.Collect();
            Stopwatch watch = new Stopwatch();
            TestObject[] array = new TestObject[arrayLength];
            for (int i=0; i < array.Length; i++)
                array[i] = new TestObject(1);
            long total = 0;
            uint idx = 123425;
            watch.Start();
            for (int i = 0; i < 10000000; i++)
            {
                idx *= 397;
                idx ^= (uint)i;
                //total += (int)(idx % arrayLength); 
                total += array[idx % arrayLength].GetStub();
            }
            watch.Stop();
            Console.Out.WriteLine(total);
            Console.Out.Write("Test1");
            Console.Out.WriteLine(watch.Elapsed);
        }

        private static void Test2()
        {
            GC.Collect();
            Stopwatch watch = new Stopwatch();
            TestObject[] array = new TestObject[arrayLength];
            for (int i = 0; i < array.Length; i++)
                array[i] = new TestObject(1);
            long total = 0;
            uint idx = 123425;
            watch.Start();
            for (int i = 0; i < 10000000; i++)
            {
                idx *= 397;
                idx ^= (uint)i;
                total += array[idx % arrayLength].GetLong();
            }
            watch.Stop();
            Console.Out.WriteLine(total);
            Console.Out.Write("Test2");
            Console.Out.WriteLine(watch.Elapsed);
        }
        const int arrayLength = 65536;

        private static void Test3()
        {
            GC.Collect();
            Stopwatch watch = new Stopwatch();
            TestObject[] array = new TestObject[arrayLength];
            for (int i = 0; i < array.Length; i++)
                array[i] = new TestObject(1);
            decimal total = 0;
            uint idx = 123425;
            watch.Start();
            for (int i = 0; i < 10000000; i++)
            {
                idx *= 397;
                idx ^= (uint)i;
                total = array[idx % arrayLength].GetDecimal();
            }
            watch.Stop();
            Debugger.Launch();
            Debugger.Break();
            Console.Out.WriteLine(total);
            Console.Out.Write("Test3");
            Console.Out.WriteLine(watch.Elapsed);
        }

        private static void Test4()
        {
            GC.Collect();
            Stopwatch watch = new Stopwatch();
            TestObject[] array = new TestObject[arrayLength];
            for (int i = 0; i < array.Length; i++)
                array[i] = new TestObject(1);
            decimal? total = 0;
            uint idx = 123425;
            watch.Start();
            for (int i = 0; i < 10000000; i++)
            {
                idx *= 397;
                idx ^= (uint)i;
                total = array[idx % arrayLength].GetDecimalOpt();
            }
            watch.Stop();
            Console.Out.WriteLine(total);
            Console.Out.Write("Test4");
            Console.Out.WriteLine(watch.Elapsed);
        }

        private static void Test5()
        {
            GC.Collect();
            Stopwatch watch = new Stopwatch();
            TestObject[] array = new TestObject[arrayLength];
            for (int i = 0; i < array.Length; i++)
                array[i] = new TestObject(1);
            long total = 0;
            uint idx = 123425;
            watch.Start();
            for (int i = 0; i < 10000000; i++)
            {
                idx *= 397;
                idx ^= (uint)i;
                total = array[idx % arrayLength].GetLongVol();
            }
            watch.Stop();
            Console.Out.WriteLine(total);
            Console.Out.Write("Test5");
            Console.Out.WriteLine(watch.Elapsed);
        }
    }
}
