using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class RobotSimulation
{
    public int cellsVisited(string program, int times)
    {
        Dictionary<int, bool> visited = new Dictionary<int, bool>();
        List<int> counts = new List<int>();
        visited[0] = true;
        counts.Add(1);
        int position = 0;
        for (int i = 0; i < times && i < 20; i++)
        {
            int nextCount=0;
            foreach (char c in program)
            {
                switch (c)
                {
                    case 'U':
                        position += 32768;
                        break;
                    case 'D':
                        position -= 32768;
                        break;
                    case 'L':
                        position -= 1;
                        break;
                    case 'R':
                        position += 1;
                        break;
                    default:
                        return -1;
                }
                if (!visited.ContainsKey(position))
                {
                    nextCount++;
                    visited[position] = true;
                }
            }
            counts.Add(nextCount);
        }
        int sum = 0;
        for (int i = 0; i < counts.Count; i++)
        {
            sum += counts[i];
        }
        if (times > counts.Count-1)
        {
            sum += (times - counts.Count + 1) * counts[counts.Count - 1];
        }
        return sum;
    }
}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            RobotSimulation c = new RobotSimulation();
            object o = c.cellsVisited("UUDUDDLLDR", 1);
            PrintObj(o);
            System.Console.In.ReadLine();
        }

        private static void PrintObj(object o)
        {
            if (o is IEnumerable)
            {
                foreach (object b in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(b);
                }
            }
            else
                System.Console.Out.WriteLine(o);
        }
    }
}
