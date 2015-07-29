#if DEBUG
#region License
/*
Copyright (c) 2008, Gareth Pearce (www.themissingdocs.net)
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the www.themissingdocs.net nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace DatOptimize
{
    /// <summary>
    /// Implements a dictionary using a tree, with useful extra methods.
    /// </summary>
    /// <remarks>
    /// Performs similar to SortedDictionary but provides O(log n) Get Enumerator for equal or prev.
    /// </remarks>
    [SuppressMessage("Microsoft.Naming", "CA1710")]
    public class SortedDictionary2<K, V> : IDictionary<K, V>
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public SortedDictionary2()
        {
            treeRoot = sentinal;
            comparer = Comparer<K>.Default;
        }
        IComparer<K> comparer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initialList">
        /// Initial values to populate the list with.
        /// </param>
        public SortedDictionary2(IEnumerable<KeyValuePair<K, V>> initialList)
            : this()
        {
            foreach (KeyValuePair<K, V> val in initialList)
            {
                this.Add(val.Key, val.Value);
            }
        }

        #region internal tree

        internal class TreeNode
        {
            public TreeNode Parent;
            public TreeNode Left;
            public TreeNode Right;
            public K Key;
            public V Value;
            public bool Red;
        }


        private TreeNode sentinal = new TreeNode();
        private TreeNode treeRoot;

        private TreeNode TreeFind(K key)
        {
            TreeNode result = treeRoot;
            while (result != sentinal)
            {
                int order = comparer.Compare(key, result.Key);
                if (order < 0)
                {
                    result = result.Left;
                }
                else if (order > 0)
                {
                    result = result.Right;
                }
                else
                    break;
            }
            return result;
        }
        private TreeNode TreeFindApprox(K key)
        {
            TreeNode result = treeRoot;
            TreeNode lastResult = sentinal;
            bool lastLeft = false;
            while (result != sentinal)
            {
                int order = comparer.Compare(key, result.Key);
                if (order < 0)
                {
                    lastResult = result;
                    result = result.Left;
                    lastLeft = true;
                }
                else if (order > 0)
                {
                    lastResult = result;
                    result = result.Right;
                    lastLeft = false;
                }
                else
                    break;
            }
            if (result == sentinal)
            {
                if (lastResult == sentinal)
                    return lastResult;
                if (!lastLeft)
                    return lastResult;
                return FirstLeft(lastResult.Parent, lastResult);
            }
            return result;
        }

        private TreeNode FirstLeft(TreeNode parent, TreeNode node)
        {
            if (parent == sentinal)
                return parent;
            if (parent.Left == node)
                return FirstLeft(parent.Parent, parent);
            else
                return parent;
        }
        private void TreeInsert(TreeNode node, TreeNode newNode)
        {
            TreeNode other = null;
            bool lastLeft = false;
            while (node != sentinal)
            {
                other = node;
                int order = comparer.Compare(newNode.Key, node.Key);
                if (order < 0)
                {
                    node = node.Left;
                    lastLeft = true;
                }
                else if (order > 0)
                {
                    node = node.Right;
                    lastLeft = false;
                }
                else
                    throw new Exception("Duplicate key.");
            }
            newNode.Parent = other;
            if (lastLeft)
                newNode.Parent.Left = newNode;
            else
                newNode.Parent.Right = newNode;

            FixRedBlackInsert(newNode);
            count++;
        }

        private void FixRedBlackInsert(TreeNode newNode)
        {
            while (newNode != treeRoot && newNode.Parent.Red)
            {
                if (newNode.Parent == newNode.Parent.Parent.Left)
                {
                    TreeNode other = newNode.Parent.Parent.Right;
                    if (other != sentinal && other.Red)
                    {
                        newNode.Parent.Red = false;
                        other.Red = false;
                        newNode.Parent.Parent.Red = true;
                        newNode = newNode.Parent.Parent;
                    }
                    else
                    {
                        if (newNode == newNode.Parent.Right)
                        {
                            newNode = newNode.Parent;
                            LeftRotate(newNode);
                        }
                        newNode.Parent.Red = false;
                        newNode.Parent.Parent.Red = true;
                        RightRotate(newNode.Parent.Parent);
                    }
                }
                else
                {
                    TreeNode other = newNode.Parent.Parent.Left;
                    if (other != sentinal && other.Red)
                    {
                        newNode.Parent.Red = false;
                        other.Red = false;
                        newNode.Parent.Parent.Red = true;
                        newNode = newNode.Parent.Parent;
                    }
                    else
                    {
                        if (newNode == newNode.Parent.Left)
                        {
                            newNode = newNode.Parent;
                            RightRotate(newNode);
                        }
                        newNode.Parent.Red = false;
                        newNode.Parent.Parent.Red = true;
                        LeftRotate(newNode.Parent.Parent);
                    }
                }
            }
        }

        private void LeftRotate(TreeNode newNode)
        {
            TreeNode other = newNode.Right;
            if (other == sentinal)
                throw new InvalidOperationException("This should not happen.");
            newNode.Right = other.Left;
            if (other.Left != sentinal)
                other.Left.Parent = newNode;
            other.Parent = newNode.Parent;
            if (newNode.Parent == sentinal)
                treeRoot = other;
            else
            {
                if (newNode == newNode.Parent.Left)
                    newNode.Parent.Left = other;
                else
                    newNode.Parent.Right = other;
            }
            other.Left = newNode;
            newNode.Parent = other;
        }

        private void RightRotate(TreeNode newNode)
        {
            TreeNode other = newNode.Left;
            if (other == sentinal)
                throw new InvalidOperationException("This should not happen.");
            newNode.Left = other.Right;
            if (other.Right != sentinal)
                other.Right.Parent = newNode;
            other.Parent = newNode.Parent;
            if (newNode.Parent == sentinal)
                treeRoot = other;
            else
            {
                if (newNode == newNode.Parent.Right)
                    newNode.Parent.Right = other;
                else
                    newNode.Parent.Left = other;
            }
            other.Right = newNode;
            newNode.Parent = other;
        }


        private void RemoveNode(TreeNode node)
        {
            TreeNode other;
            if (node.Left == sentinal || node.Right == sentinal)
                other = node;
            else
                other = FullLeft(node.Right);
            TreeNode other2;
            if (other.Left != sentinal)
                other2 = other.Left;
            else
                other2 = other.Right;
            other2.Parent = other.Parent;
            if (other.Parent == sentinal)
                treeRoot = other2;
            else
            {
                if (other == other.Parent.Left)
                    other.Parent.Left = other2;
                else
                    other.Parent.Right = other2;
            }
            if (other != node)
            {
                node.Key = other.Key;
                node.Value = other.Value;
            }
            TreeNode backWalk = other.Parent;
            if (!other.Red)
                FixRedBlackDelete(other2);
            count--;
        }

        private void FixRedBlackDelete(TreeNode node)
        {
            while (node != treeRoot && !node.Red)
            {
                if (node == node.Parent.Left)
                {
                    TreeNode other = node.Parent.Right;
                    if (other.Red)
                    {
                        other.Red = false;
                        node.Parent.Red = true;
                        LeftRotate(node.Parent);
                        other = node.Parent.Right;
                    }
                    if (!other.Left.Red && !other.Right.Red)
                    {
                        other.Red = true;
                        node = node.Parent;
                    }
                    else
                    {
                        if (!other.Right.Red)
                        {
                            other.Left.Red = false;
                            other.Red = true;
                            RightRotate(other);
                            other = node.Parent.Right;
                        }
                        other.Red = node.Parent.Red;
                        node.Parent.Red = false;
                        other.Right.Red = false;
                        LeftRotate(node.Parent);
                        node = treeRoot;
                    }
                }
                else
                {
                    TreeNode other = node.Parent.Left;
                    if (other.Red)
                    {
                        other.Red = false;
                        node.Parent.Red = true;
                        RightRotate(node.Parent);
                        other = node.Parent.Left;
                    }
                    if (!other.Right.Red && !other.Left.Red)
                    {
                        other.Red = true;
                        node = node.Parent;
                    }
                    else
                    {
                        if (!other.Left.Red)
                        {
                            other.Right.Red = false;
                            other.Red = true;
                            LeftRotate(other);
                            other = node.Parent.Left;
                        }
                        other.Red = node.Parent.Red;
                        node.Parent.Red = false;
                        other.Left.Red = false;
                        RightRotate(node.Parent);
                        node = treeRoot;
                    }
                }
            }
            node.Red = false;
        }

        private TreeNode FullLeft(TreeNode treeNode)
        {
            while (treeNode.Left != sentinal)
            {
                treeNode = treeNode.Left;
            }
            return treeNode;
        }


        #endregion

        private int version;
        private int count;


        #region ICollection<T> Members

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void Clear()
        {
            treeRoot = sentinal;
            count = 0;
            version++;
        }



        /// <summary>
        /// Gets the count of the number of items in the list.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Returns false.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }


        #endregion


        #region IEnumerable Members

        /// <summary>
        /// Gets the untyped enumerator for this list.
        /// </summary>
        /// <returns>
        /// An enumerator to enumerate the objects stored in this list.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new SortedDictionary2Enumerator(this);
        }

        #endregion

        /// <summary>
        /// Enumerator for tree lists.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1815")]
        public struct SortedDictionary2Enumerator : IEnumerator<KeyValuePair<K, V>>
        {

            /// <summary>
            /// Internal constructor.
            /// </summary>
            /// <param name="parent">
            /// Parent tree list to enumerate.
            /// </param>
            internal SortedDictionary2Enumerator(SortedDictionary2<K, V> parent)
            {
                this.parent = parent;
                origVersion = parent.version;
                currentNode = null;
                offEnd = false;
            }

            /// <summary>
            /// Internal constructor.
            /// </summary>
            /// <param name="parent">
            /// Parent tree list to enumerate.
            /// </param>
            /// <param name="start">
            /// Node to start with.
            /// </param>
            internal SortedDictionary2Enumerator(SortedDictionary2<K, V> parent, TreeNode start)
            {
                this.parent = parent;
                origVersion = parent.version;
                currentNode = start;
                if (start == parent.sentinal)
                    currentNode = null;
                offEnd = false;
            }

            private SortedDictionary2<K, V> parent;
            private int origVersion;
            private TreeNode currentNode;
            private bool offEnd;

            #region IEnumerator<T> Members

            /// <summary>
            /// Gets the current item this enumerator points to.
            /// </summary>
            public KeyValuePair<K, V> Current
            {
                get
                {
                    if (currentNode == null)
                        return new KeyValuePair<K, V>();
                    else
                        return new KeyValuePair<K, V>(currentNode.Key, currentNode.Value);
                }
            }

            #endregion

            #region IDisposable Members

            /// <summary>
            /// Does nothing.
            /// </summary>
            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Gets the current object the enumerator points to.
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (currentNode == null)
                        throw new InvalidOperationException("Current is not pointing to a valid node.");
                    return this.Current;
                }
            }

            /// <summary>
            /// Moves to the next node if possible.
            /// </summary>
            /// <returns>
            /// True if moved to the next node, false otherwise.
            /// </returns>
            public bool MoveNext()
            {
                if (origVersion != parent.version)
                    throw new InvalidOperationException("The collection being enumerated has been modified since enumerator was acquired.");
                if (!offEnd && currentNode == null)
                {
                    if (parent.treeRoot == parent.sentinal)
                    {
                        offEnd = true;
                        return false;
                    }
                    else
                    {
                        currentNode = parent.FullLeft(parent.treeRoot);
                        return true;
                    }
                }
                else
                {
                    if (currentNode.Right != parent.sentinal)
                    {
                        currentNode = parent.FullLeft(currentNode.Right);
                        return true;
                    }
                    else
                    {
                        TreeNode other = currentNode.Parent;
                        while (other != parent.sentinal && currentNode == other.Right)
                        {
                            currentNode = other;
                            other = other.Parent;
                        }
                        if (other != parent.sentinal)
                        {
                            currentNode = other;
                            return true;
                        }
                        else
                        {
                            currentNode = null;
                            offEnd = true;
                            return false;
                        }
                    }
                }
            }

            /// <summary>
            /// Resets the enumerator.
            /// </summary>
            void System.Collections.IEnumerator.Reset()
            {
                if (origVersion != parent.version)
                    throw new InvalidOperationException("The collection being enumerated has been modified since enumerator was acquired.");
                currentNode = null;
                offEnd = false;
            }

            #endregion
        }


        /// <summary>
        /// Adds the specifiedkey and value to the tree.
        /// </summary>
        /// <param name="key">
        /// Key to add the value under.
        /// </param>
        /// <param name="value">
        /// Value to add.
        /// </param>
        public void Add(K key, V value)
        {
            TreeNode newNode = new TreeNode();
            newNode.Key = key;
            newNode.Value = value;
            newNode.Red = true;
            newNode.Parent = sentinal;
            newNode.Left = sentinal;
            newNode.Right = sentinal;
            if (treeRoot == sentinal)
            {
                treeRoot = newNode;
                count++;
            }
            else
                TreeInsert(treeRoot, newNode);
            treeRoot.Red = false;
            version++;
        }

        /// <summary>
        /// Returns whether the specfied key is stored in this dictionary.
        /// </summary>
        /// <param name="key">
        /// Key to check for.
        /// </param>
        /// <returns>
        /// True if the specified key is stored, otherwise false.
        /// </returns>
        public bool ContainsKey(K key)
        {
            return TreeFind(key) != sentinal;
        }

        /// <summary>
        /// Gets the collection of keys in the dictionary.
        /// </summary>
        public ICollection<K> Keys
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Removes the specified key from the dictionary if it is present.
        /// </summary>
        /// <param name="key">
        /// Key to remove.
        /// </param>
        /// <returns>
        /// True if the key was present and removed.
        /// </returns>
        public bool Remove(K key)
        {
            TreeNode node = TreeFind(key);
            if (node == sentinal)
                return false;
            RemoveNode(node);
            version++;
            return true;
        }

        /// <summary>
        /// Attempts to obtain the value for the specified key, if it is present.
        /// </summary>
        /// <param name="key">
        /// Key to check for.
        /// </param>
        /// <param name="value">
        /// Receives value if found.
        /// </param>
        /// <returns>
        /// True if key is found and value returned, false otherwise.
        /// </returns>
        public bool TryGetValue(K key, out V value)
        {
            TreeNode node = TreeFind(key);
            if (node == sentinal)
            {
                value = default(V);
                return false;
            }
            value = node.Value;
            return true;
        }

        /// <summary>
        /// Gets the collection of values.
        /// </summary>
        public ICollection<V> Values
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Indexer.  Gets or sets values by keys.
        /// </summary>
        /// <param name="key">
        /// Key to access or update.
        /// </param>
        /// <returns>
        /// The value stored at the key if the key is present in the dictionary.
        /// </returns>
        public V this[K key]
        {
            get
            {
                V result;
                if (TryGetValue(key, out result))
                    return result;
                throw new KeyNotFoundException();
            }
            set
            {
                Remove(key);
                Add(key, value);
            }
        }

        /// <summary>
        /// Adds the specified key value pair into the dictionary.
        /// </summary>
        /// <param name="item">
        /// The key value pair to add.
        /// </param>
        public void Add(KeyValuePair<K, V> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Checks if the specified key value pair is already in the dictionary.
        /// </summary>
        /// <param name="item">
        /// Key value pair to check the dictionary for.
        /// </param>
        /// <returns>
        /// True if the key value pair is in the dictionary, false otherwise.
        /// </returns>
        public bool Contains(KeyValuePair<K, V> item)
        {
            V otherValue;
            if (!TryGetValue(item.Key, out otherValue))
                return false;
            return Comparer<V>.Default.Compare(item.Value, otherValue) == 0;
        }

        /// <summary>
        /// Copies the contents of the dictionary into the array.
        /// </summary>
        /// <param name="array">
        /// Array to receive the key value pairs stored in the dictionary.
        /// </param>
        /// <param name="arrayIndex">
        /// Starting index to copy to.
        /// </param>
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            if (arrayIndex < 0 || arrayIndex + count > array.Length)
                throw new ArgumentOutOfRangeException("arrayIndex");
            TreeCopy(treeRoot, array, ref arrayIndex);
        }

        private void TreeCopy(TreeNode node, KeyValuePair<K, V>[] array, ref int arrayIndex)
        {
            if (node == sentinal)
                return;
            TreeCopy(node.Left, array, ref arrayIndex);
            array[arrayIndex] = new KeyValuePair<K, V>(node.Key, node.Value);
            arrayIndex++;
            TreeCopy(node.Right, array, ref arrayIndex);
        }

        /// <summary>
        /// Removes the specified key value pair if it is in the dictionary.
        /// </summary>
        /// <param name="item">
        /// Key value pair to remove.
        /// </param>
        /// <returns>
        /// True if the key value pair was in the dictionary and removed.
        /// </returns>
        public bool Remove(KeyValuePair<K, V> item)
        {
            TreeNode node = TreeFind(item.Key);
            if (node == sentinal)
            {
                return false;
            }
            if (Comparer<V>.Default.Compare(item.Value, node.Value) != 0)
                return false;
            RemoveNode(node);
            return true;
        }

        /// <summary>
        /// Gets an enumerator for the specified key or just before it.
        /// </summary>
        /// <param name="key">
        /// Key to get the enumerator for.
        /// </param>
        /// <returns>
        /// An enumerator either pointing at the specified key, or immediately before it.
        /// </returns>
        public IEnumerator<KeyValuePair<K, V>> GetEnumeratorForKeyOrPrev(K key)
        {
            TreeNode node = TreeFindApprox(key);
            return new SortedDictionary2Enumerator(this, node);
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            return new SortedDictionary2Enumerator(this);
        }
    }
}
#endif