using System;
using System.Collections;
using System.Text;

public class OddDigitable
{
    public string findMultiple(int N, int M)
    {
        bool[] done = new bool[N];
        ArrayList strs = new ArrayList();
        ArrayList vals = new ArrayList();
        strs.Add("");
        vals.Add(0);
        for (int index = 0; index < strs.Count; index++)
        {
            string ba = (string)strs[index];
            int va = (int)vals[index];
            for (int i = 1; i < 10; i += 2)
            {
                int cur = va * 10 + i;
                string cub = ba + i.ToString();
                int mod = cur % N;
                if (mod == M)
                    return cub;
                if (done[mod])
                    continue;
                strs.Add(cub);
                vals.Add(mod);
                done[mod] = true;
            }
        }
        return "-1";

    }
}

namespace SRM255Q3
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
