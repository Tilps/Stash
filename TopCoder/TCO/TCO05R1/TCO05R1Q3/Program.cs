using System;
using System.Collections;
using System.Text;

public class VariableSolve
{
    public string[] getSolutions(string equation)
    {
        ArrayList parts = new ArrayList();
        ArrayList counts = new ArrayList();
        bool rhs = false;
        bool curPos = true;
        StringBuilder cur = new StringBuilder();
        for (int i = 0; i < equation.Length; i++)
        {
            if (equation[i] == '+')
            {
                parts.Add(cur.ToString());
                if (curPos)
                    counts.Add(1);
                else
                    counts.Add(-1);
                cur.Length = 0;
                curPos = !rhs;
            }
            else if (equation[i] == '-')
            {
                parts.Add(cur.ToString());
                if (curPos)
                    counts.Add(1);
                else
                    counts.Add(-1);
                cur.Length = 0;
                curPos = rhs;
            }
            else if (equation[i] == '=')
            {
                parts.Add(cur.ToString());
                if (curPos)
                    counts.Add(1);
                else
                    counts.Add(-1);
                cur.Length = 0;
                curPos = false;
                rhs = true;
            }
            else
                cur.Append(equation[i]);
        }
        parts.Add(cur.ToString());
        if (curPos)
            counts.Add(1);
        else
            counts.Add(-1);
        for (int i = 0; i < parts.Count; i++)
        {
            string a = (string)parts[i];
            char[] chs = a.ToCharArray();
            Array.Sort(chs);
            parts[i] = new string(chs);
        }
        Hashtable rels = new Hashtable();
        for (int i = 0; i < parts.Count; i++)
        {
            if (rels.Contains(parts[i]))
            {
                rels[parts[i]] = (int)rels[parts[i]] + (int)counts[i];
                if ((int)rels[parts[i]] == 0)
                    rels.Remove(parts[i]);
            }
            else
            {
                rels.Add(parts[i], counts[i]);
            }
        }
        ArrayList vars = new ArrayList();
        for (int i = 0; i < parts.Count; i++)
        {
            for (int j = 0; j < ((string)parts[i]).Length; j++)
            {
                if (!vars.Contains(((string)parts[i])[j]))
                {
                    vars.Add(((string)parts[i])[j]);
                }
            }
        }
        ArrayList res = new ArrayList();
        Hashtable removable = new Hashtable();
        if (rels.Count > 0)
        {
            foreach (char var in vars)
            {
                bool all = true;
                foreach (string part in rels.Keys)
                {
                    if (part.IndexOf(var) == -1)
                    {
                        all = false;
                        break;
                    }
                }
                if (all)
                    res.Add("" + var);
                removable[var] = all;
            }
        }
        return (string[])res.ToArray(typeof(string));
    }
}

namespace TCO05R1Q3
{
    class Program
    {
        static void Main(string[] args)
        {
            VariableSolve c = new VariableSolve();
            object o = c.getSolutions("baa+ba+b=P-P");

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