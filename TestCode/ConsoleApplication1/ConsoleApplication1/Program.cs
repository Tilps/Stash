using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int j = 0; j < 10; j++)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < 100000; i++)
                {
                    GenType<int> a = new GenType<int>();
                    a.Prop = 1;
                    a.Prop = 2;
                    dynamic b = (object)a;
                    var result = (int)Method(b);
                }
                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
                watch.Reset();
                watch.Start();
                for (int i = 0; i < 100000; i++)
                {
                    GenType2 a = new GenType2();
                    a.Prop = 1;
                    a.Prop = 2;
                    var result = Comparer<double>.Default.Compare(Convert.ToDouble(a.Prop), Convert.ToDouble(a.Prop2));
                }
                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
            }
            Console.ReadKey();
        }

        public static int Method<T>(GenType<T> a)
        {
            return Comparer<T>.Default.Compare(a.Prop, a.Prop2);
        }

        public class GenType<T>
        {
            public T Prop;
            public T Prop2;
        }

        public class GenType2
        {
            public object Prop;
            public object Prop2;
        }
    }
}
