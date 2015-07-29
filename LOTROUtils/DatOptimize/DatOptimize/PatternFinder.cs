using System;
using System.Collections.Generic;
using System.Text;

namespace DatOptimize
{
    class PatternFinder<T>
    {

        public PatternFinder(IEnumerable<T> data, IEqualityComparer<T> comparer)
        {
            this.data = data;
            this.comparer = comparer;
        }

        IEnumerable<T> data;
        IEqualityComparer<T> comparer;

        public PatternTree FindPatterns()
        {
            int cutoff = 2;
            Dictionary<KeyValuePair<T, T>, int> freqs = new Dictionary<KeyValuePair<T, T>, int>(new KeyValuePairComparer(comparer));
            bool started = false;
            // Step 1, find pair frequencies.
            T last = default(T);
            foreach (T item in data)
            {
                if (started)
                {
                    var key = new KeyValuePair<T, T>(last, item);
                    int count = 0;
                    freqs.TryGetValue(key, out count);
                    freqs[key] = count + 1;
                }
                last = item;
                started = true;
            }
            PatternTree tree = new PatternTree(comparer);
            List<PatternTreeNode> currentPositions = new List<PatternTreeNode>();
            // Step 2, find chain frequencies where pair frequencies indicate plausibility of >cutoff pattern frequency. (aka every pair has > cutoff frequency.)
            started = false;
            last = default(T);
            foreach (T item in data)
            {
                if (started)
                {
                    var key = new KeyValuePair<T, T>(last, item);
                    int count = 0;
                    freqs.TryGetValue(key, out count);
                    if (count >= cutoff)
                    {
                        // start/extend pattern.
                        PatternTreeNode node = tree.Start(last);
                        currentPositions.Add(node);
                        List<PatternTreeNode> nextPositions = new List<PatternTreeNode>(currentPositions.Count);
                        foreach (PatternTreeNode position in currentPositions)
                        {
                            nextPositions.Add(position.Extend(item));
                        }
                        currentPositions = nextPositions;
                    }
                    else
                    {
                        currentPositions.Clear();
                    }
                }
                last = item;
                started = true;
            }
            tree.Trim(cutoff);
            // TODO: subpattern subtraction.
            // Determine for each node its 'true count'.  Its count minus each of its child counts.
            // For each node starting with the deepest first, find every sub pattern and subtract the node's true count from the found location. O (n*m^2) where m is max depth if we have backpointers. 
            // Then do DFS trim of nodes with no children and subcutoff count.
            // Need backpointers to do this efficiently?  Still not going to be cheap :P


            return tree;

        }

        public class PatternTree
        {

            internal PatternTree(IEqualityComparer<T> comparer)
            {
                this.comparer = comparer;
                roots = new Dictionary<T, PatternTreeNode>(comparer);
            }
            IEqualityComparer<T> comparer;

            public PatternTreeNode Start(T node)
            {
                PatternTreeNode next;
                if (roots.TryGetValue(node, out next))
                {
                    next.count++;
                }
                else
                {
                    next = new PatternTreeNode(comparer);
                    next.count = 1;
                    next.self = node;
                    roots[node] = next;
                }
                return next;
            }

            public Dictionary<T, PatternTreeNode> roots;

            internal void Trim(int cutoff)
            {
                foreach (PatternTreeNode node in roots.Values)
                {
                    node.Trim(cutoff);
                }
            }
        }

        public class PatternTreeNode
        {
            internal PatternTreeNode(IEqualityComparer<T> comparer)
            {
                this.comparer = comparer;
            }
            IEqualityComparer<T> comparer;

            public PatternTreeNode Extend(T node)
            {
                PatternTreeNode next;
                if (children.TryGetValue(node, out next))
                {
                    next.count++;
                }
                else
                {
                    next = new PatternTreeNode(comparer);
                    next.count = 1;
                    next.self = node;
                    children[node] = next;
                }
                return next;
            }

            public T self;
            public int count;
            public Dictionary<T, PatternTreeNode> children = new Dictionary<T, PatternTreeNode>();

            internal void Trim(int cutoff)
            {
                List<T> toRemove = new List<T>();
                foreach (PatternTreeNode node in children.Values)
                {
                    if (node.count < cutoff)
                        toRemove.Add(node.self);
                }
                foreach (T nodeId in toRemove)
                    children.Remove(nodeId);
            }
        }

        private class KeyValuePairComparer : IEqualityComparer<KeyValuePair<T, T>>
        {
            public KeyValuePairComparer(IEqualityComparer<T> comparer)
            {
                this.comparer = comparer;
            }
            IEqualityComparer<T> comparer;

            #region IEqualityComparer<KeyValuePair<T,T>> Members

            public bool Equals(KeyValuePair<T, T> x, KeyValuePair<T, T> y)
            {
                return comparer.Equals(x.Key, y.Key) && comparer.Equals(x.Value, y.Value);
            }

            public int GetHashCode(KeyValuePair<T, T> obj)
            {
                return (comparer.GetHashCode(obj.Key) * 33) ^ comparer.GetHashCode(obj.Value);
            }

            #endregion
        }
    }

}
