#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace FiniteCalculator {



    /// <summary>
    /// Encapsulates a finite set of integers which is most likely a member of a powerset.
    /// </summary>
    public class Set : IComparable<Set> {



        /// <summary>
        /// Constructor.
        /// </summary>
        public Set() {
            num = -1;
            index = -1;
        }



        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="size">
        /// Initial array size.
        /// </param>
        public Set(int size) {
            elements = new int[size];
            num = -1;
            index = -1;
        }



        /// <summary>
        /// Elements of the set.
        /// </summary>
        public int[] elements;


        /// <summary>
        /// Gets the size of the set, relic of c days.
        /// </summary>
        /// <value></value>
        public int Size {
            get {
                return elements.Length;
            }
        }


        /// <summary>
        /// Sets this set equal to the union of two other sets.
        /// </summary>
        /// <param name="a">
        /// Set a.
        /// </param>
        /// <param name="b">
        /// Set b.
        /// </param>
        public void TakeRealUnion(Set a, Set b) {
            List<int> results = new List<int>();
            int i = 0;
            int j = 0;
            while (i < a.Size || j < b.Size) {
                if (i == a.Size) {
                    results.Add(b.elements[j]);
                    j++;
                }
                else if (j == b.Size) {
                    results.Add(a.elements[i]);
                    i++;
                }
                else if (a.elements[i] < b.elements[j]) {
                    results.Add(a.elements[i]);
                    i++;
                }
                else if (a.elements[i] > b.elements[j]) {
                    results.Add(b.elements[j]);
                    j++;
                }
                else if (a.elements[i] == b.elements[j]) {
                    results.Add(a.elements[i]);
                    i++;
                    j++;
                }
            }
            elements = results.ToArray();
            num = -1;
            index = -1;
        }



        /// <summary>
        /// Sets this set equal to the intersection of two other sets.
        /// </summary>
        /// <param name="a">
        /// Set a.
        /// </param>
        /// <param name="b">
        /// Set b.
        /// </param>
        public void TakeRealIntersection(Set a, Set b) {
            List<int> results = new List<int>();
            int i = 0;
            int j = 0;
            while (i < a.Size && j < b.Size) {
                if (a.elements[i] < b.elements[j]) {
                    i++;
                }
                else if (a.elements[i] > b.elements[j]) {
                    j++;
                }
                else if (a.elements[i] == b.elements[j]) {
                    results.Add(a.elements[i]);
                    i++;
                    j++;
                }
            }
            elements = results.ToArray();
            num = -1;
            index = -1;
        }



        /// <summary>
        /// The number of this set in the powerset.
        /// -1 indicates no powerset.
        /// </summary>
        public int num;



        /// <summary>
        /// The index of this set in the powerset section with same size as this set.
        /// </summary>
        public int index;



        #region IComparable<Set> Members



        /// <summary>
        /// Compares this set to another.  Uses poerset numbers if available.
        /// </summary>
        /// <param name="other">
        /// Other set to compare to.
        /// </param>
        /// <returns>
        /// A number indicating the relative order of the two sets.
        /// </returns>
        public int CompareTo(Set other) {
            int sizeDiff = Math.Sign(Size - other.Size);
            if (sizeDiff != 0)
                return sizeDiff;
            if (num >= 0 && other.num >= 0)
                return Math.Sign(num - other.num);
            for (int i = 0; i < Size; i++) {
                if (elements[i] < other.elements[i])
                    return -1;
                if (elements[i] > other.elements[i])
                    return 1;
            }
            return 0;
        }



        /// <summary>
        /// Determines set equality.  Uses CompareTo.
        /// </summary>
        /// <param name="other">
        /// The other set to compare to.
        /// </param>
        /// <returns>
        /// True if the sets are equivelent.
        /// False otherwise.
        /// </returns>
        public bool Equals(Set other) {
            return this.CompareTo(other) == 0;
        }



        #endregion



        /// <summary>
        /// Overrides tostring to produce a formated set view.
        /// </summary>
        /// <returns>
        /// A string representation of the set.
        /// </returns>
        public override string ToString() {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            for (int i = 0; i < Size; i++) {
                if (builder.Length > 1) {
                    builder.Append(", ");
                }
                builder.Append(elements[i]);
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
