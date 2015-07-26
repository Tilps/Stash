using System;
using System.Collections;
using System.Text;

public class VariableAddition
{
    public int add(string eq, string[] vars)
    {

        string[] terms = eq.Split('+');
        Hashtable vartab = new Hashtable();
        for (int i = 0; i < vars.Length; i++)
        {
            string[] splits = vars[i].Split(' ');
            vartab.Add(splits[0], int.Parse(splits[1]));
        }
        int total = 0;
        for (int i = 0; i < terms.Length; i++)
        {
            try
            {
                total += int.Parse(terms[i]);
            }
            catch
            {
                total += (int)vartab[terms[i]];
            }
        }

        return total;
    }
}

namespace TCO05QRQ1
{
    class Program
    {
        static void Main(string[] args)
        {
            VariableAddition c = new VariableAddition();
            object o = c.add("", new string[] { });

            if (o is IEnumerable)
            {
                foreach (object oi in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(oi);
          
                }
            }
            else
                System.Console.Out.WriteLine(o);
            System.Console.In.ReadLine();
        }
    }
}