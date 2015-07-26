using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace ConsoleApplication5
{
    class Program
    {
        static void Main(string[] args)
        {

            object a = new object();
            Func<object> res = Expression.Lambda<Func<object>>(Expression.Constant(a)).Compile();
            object b = res();
            object c = a;
            Func<object> res2 = () => c;
            if (ReferenceEquals(a, b))
                Console.WriteLine("WIN");
            Console.ReadKey();
        }
    }
}
