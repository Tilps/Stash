using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            int[][] transitions = new int[][] { 
                new int[] {1},
            new int[] {0,2,3}, 
            new int[] {1,4},
            new int[] {1,5},
            new int[] {2,5,6},
            new int[] {3,4, 7},
            new int[] {4, 7, 8},
            new int[] {5, 6, 9},
            new int[] {6, 10},
            new int[] {7, 10},
            new int[] {8, 9, 11},
            new int[] {10}
            };
            int[] startState = new int[] {
                2,
                0,
                1,
                1,
                1,
                1,
                1,
                0,
                1,
                1,
                0,
                0,
            };

            int start = 0;
            int end = startState.Length - 1;
            Queue<State> toCheck = new Queue<State>();
            toCheck.Enqueue(new State { Depth = 0, States = startState });

            HashSet<State> seen = new HashSet<State>();
            int minDepth = -1;
            while (toCheck.Count > 0)
            {
                State current = toCheck.Dequeue();
                if (current.States[end] == 2)
                {
                    if (minDepth == -1) 
                        minDepth = current.Depth;
                    if (minDepth == current.Depth)
                        Print(current);
                    continue;
                }
                for (int i = 0; i < current.States.Length; i++)
                {
                    if (current.States[i] == 0)
                        continue;
                    for (int j = 0; j < transitions[i].Length; j++)
                    {
                        int dest = transitions[i][j];
                        if (current.States[dest] != 0)
                            continue;
                        if (dest == end && current.States[i] != 2)
                            continue;
                        int[] newState = new int[current.States.Length];
                        current.States.CopyTo(newState, 0);
                        newState[dest] = current.States[i];
                        newState[i] = 0;
                        State newStateState = new State { From = current, Depth = current.Depth + 1, States = newState };
                        if (seen.Contains(newStateState))
                            continue;
                        seen.Add(newStateState);
                        toCheck.Enqueue(newStateState);
                    }
                }
            }
            Console.ReadKey();
        }

        private static void Print(State current)
        {
            Console.Out.WriteLine(current.Depth);
            while (current != null)
            {
                Console.Out.WriteLine(string.Concat(current.States.Select(a=>a.ToString())));
                current = current.From;
            }
        }

        private class State
        {
            public int[] States;
            public int Depth;
            public State From;

            public override int GetHashCode()
            {
                int res = 0;
                for (int i = 0; i < States.Length; i++)
                {
                    res = (res * 397) ^ States[i];
                }
                return res;
            }
            public override bool Equals(object obj)
            {
                State other = (State)obj;
                if (other.States.Length != this.States.Length)
                    return false;
                for (int i = 0; i < other.States.Length; i++)
                {
                    if (other.States[i] != States[i])
                        return false;
                }
                return true;
            }
        }
    }
}
