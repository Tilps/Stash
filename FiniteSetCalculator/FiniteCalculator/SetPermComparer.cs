#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace FiniteCalculator {



    /// <summary>
    /// Compares two sets under the same permutation.
    /// </summary>
    public class SetPermComparer : IComparer<Set> {



        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="index">
        /// Permutation to compare under.
        /// </param>
        /// <param name="powerset">
        /// Powerset to use to lookup the permutation precalc.
        /// </param>
        public SetPermComparer(int index, PowerSet powerset) {
            by = index;
            this.powerset = powerset;
        }



        /// <summary>
        /// Permutation to use.
        /// </summary>
        private int by;



        /// <summary>
        /// The powerset to use for looking up precalc.
        /// </summary>
        private PowerSet powerset;



        #region IComparer<Set> Members



        /// <summary>
        /// Compares two sets.
        /// </summary>
        /// <param name="x">
        /// Set x.
        /// </param>
        /// <param name="y">
        /// Set y.
        /// </param>
        /// <returns>
        /// A number representing the comparative order of the two sets.
        /// </returns>
        public int Compare(Set x, Set y) {
            int sizeDiff = Math.Sign(x.Size - y.Size);
            if (sizeDiff != 0)
                return sizeDiff;
            int index1 = powerset.PermutationsIndex[by, x.num];
            int index2 = powerset.PermutationsIndex[by, y.num];
            return Math.Sign(index1 - index2);
        }



        /// <summary>
        /// Determines if two sets are equal.
        /// </summary>
        /// <param name="x">
        /// Set x.
        /// </param>
        /// <param name="y">
        /// Set y.
        /// </param>
        /// <returns>
        /// True if the two sets are equal.
        /// False otherwise.
        /// </returns>
        public bool Equals(Set x, Set y) {
            if (x.Size != y.Size)
                return false;
            int index1 = powerset.PermutationsIndex[by, x.num];
            int index2 = powerset.PermutationsIndex[by, y.num];
            return index1 == index2;
        }



        /// <summary>
        /// Returns a number which is the same if two sets are equal.
        /// </summary>
        /// <param name="obj">
        /// The set to get the hash code for.
        /// </param>
        /// <returns>
        /// The hashcode.
        /// </returns>
        public int GetHashCode(Set obj) {
            return powerset.PermuteSet(obj, by).num;
        }



        #endregion
    }
}
