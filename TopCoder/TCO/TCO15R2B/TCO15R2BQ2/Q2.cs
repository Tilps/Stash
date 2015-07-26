using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

public class DisjointTracker<T>
{
    public DisjointTracker() : this(null)
    {
    }

    public DisjointTracker(IEqualityComparer<T> comparer)
    {
        if (comparer == null)
            comparer = EqualityComparer<T>.Default;
        this.comparer = comparer;
        tracker = new Dictionary<T, T>(comparer);
        ranker = new Dictionary<T, int>(comparer);
    }


    private readonly Dictionary<T, T> tracker;
    private readonly Dictionary<T, int> ranker;
    private readonly IEqualityComparer<T> comparer;


    public void Add(T value)
    {
        tracker[value] = value;
        ranker[value] = 0;
    }

    public void Union(T first, T second)
    {
        Link(GetRepresentative(first), GetRepresentative(second));
    }

    public T GetRepresentative(T value)
    {
        T parent = tracker[value];
        if (comparer.Equals(parent, value))
            return parent;
        T realParent = GetRepresentative(parent);
        tracker[value] = realParent;
        return realParent;
    }

    private void Link(T first, T second)
    {
        if (comparer.Equals(first, second))
            return;
        int firstRank = ranker[first];
        int secondRank = ranker[second];
        if (firstRank > secondRank)
        {
            tracker[second] = first;
        }
        else
        {
            if (firstRank == secondRank)
                ranker[second] = secondRank + 1;
            tracker[first] = second;
        }
    }
}

public class Balance
{
    private int Rep(int i, int j, string[] map)
    {
        return i*map[0].Length + j;
    }
    public string canTransform(string[] initial, string[] target)
    {
        Dictionary<int, HashSet<int>> children;
        Dictionary<int, int> parents;
        int root;
        MakeTree(initial, out children, out parents, out root);
        Dictionary<int, HashSet<int>> tchildren;
        Dictionary<int, int> tparents;
        int troot;
        MakeTree(target, out tchildren, out tparents, out troot);
        Dictionary<long, bool> equalSubs = new Dictionary<long, bool>();
        return Compare(children, tchildren, root, troot, equalSubs) ? "Possible" : "Impossible";
    }

    private bool Compare(Dictionary<int, HashSet<int>> children, Dictionary<int, HashSet<int>> tchildren, int root, int troot, Dictionary<long, bool> equalSubs)
    {
        long key = ((long)root << 32) + (long)troot;
        bool result;
        if (equalSubs.TryGetValue(key, out result)) return result;

        if (children[root].Count == tchildren[troot].Count)
        {
            // Sort each set of children and compare?   Presort using separate versions of compare which return ints.
        }
        else
        {
            result = false;
        }

        
        equalSubs[key] = result;
        return result;
    }

    private void MakeTree(string[] initial, out Dictionary<int, HashSet<int>> children, out Dictionary<int, int> parents, out int root)
    {
        DisjointTracker<int> initReps = new DisjointTracker<int>();
        initReps.Add(-1);
        for (int i = 0; i < initial.Length; i++)
        {
            for (int j = 0; j < initial[i].Length; j++)
            {
                initReps.Add(Rep(i, j, initial));
            }
        }
        for (int i = 0; i < initial.Length; i++)
        {
            for (int j = 0; j < initial[i].Length; j++)
            {
                if (initial[i][j] == '#' && (i == 0 || j == 0 || i == initial.Length - 1 || j == initial[0].Length - 1))
                    initReps.Union(-1, Rep(i, j, initial));
                if (i > 0 && initial[i][j] == initial[i - 1][j])
                {
                    initReps.Union(Rep(i, j, initial), Rep(i - 1, j, initial));
                }
                if (j > 0 && initial[i][j] == initial[i][j - 1])
                {
                    initReps.Union(Rep(i, j, initial), Rep(i, j - 1, initial));
                }
                if (i < initial.Length - 1 && initial[i][j] == initial[i + 1][j])
                {
                    initReps.Union(Rep(i, j, initial), Rep(i + 1, j, initial));
                }
                if (j < initial[0].Length - 1 && initial[i][j] == initial[i][j + 1])
                {
                    initReps.Union(Rep(i, j, initial), Rep(i, j + 1, initial));
                }
            }
        }
        children = new Dictionary<int, HashSet<int>>();
        parents = new Dictionary<int, int>();
        Stack<int> toDo = new Stack<int>();
        toDo.Push(initReps.GetRepresentative(-1));
        root = initReps.GetRepresentative(-1);
        while (toDo.Count > 0)
        {
            int next = toDo.Pop();
            int parent = -2;
            if (!parents.TryGetValue(next, out parent))
                parent = -2;
            HashSet<int> childs = new HashSet<int>();
            children[next] = childs;
            for (int i = 0; i < initial.Length; i++)
            {
                for (int j = 0; j < initial[i].Length; j++)
                {
                    if (initReps.GetRepresentative(Rep(i, j, initial)) == next)
                    {
                        if (i > 0)
                        {
                            int neighbour = initReps.GetRepresentative(Rep(i - 1, j, initial));
                            if (neighbour != parent)
                            {
                                if (childs.Add(neighbour))
                                {
                                    toDo.Push(neighbour);
                                    parents[neighbour] = next;
                                }
                            }
                        }
                        if (j > 0)
                        {
                            int neighbour = initReps.GetRepresentative(Rep(i, j - 1, initial));
                            if (neighbour != parent)
                            {
                                if (childs.Add(neighbour))
                                {
                                    toDo.Push(neighbour);
                                    parents[neighbour] = next;
                                }
                            }
                        }
                        if (i < initial.Length - 1)
                        {
                            int neighbour = initReps.GetRepresentative(Rep(i + 1, j, initial));
                            if (neighbour != parent)
                            {
                                if (childs.Add(neighbour))
                                {
                                    toDo.Push(neighbour);
                                    parents[neighbour] = next;
                                }
                            }
                        }
                        if (j < initial[0].Length - 1)
                        {
                            int neighbour = initReps.GetRepresentative(Rep(i, j + 1, initial));
                            if (neighbour != parent)
                            {
                                if (childs.Add(neighbour))
                                {
                                    toDo.Push(neighbour);
                                    parents[neighbour] = next;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

namespace Q2
{
    class Q2
    {
        static void Main(string[] args)
        {
            Balance c = new Balance();
            object o = c.canTransform(new string[] {}, new string[] {});
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

