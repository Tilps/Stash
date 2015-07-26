#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace FiniteCalculator {



    /// <summary>
    /// Implements a powerset as a descendent of a topology.
    /// </summary>
    public class PowerSet : Topology {



        /// <summary>
        /// Constuct a powerset from a finite basis of size 'size'.
        /// </summary>
        /// <param name="size">
        /// The size of the basis.
        /// </param>
        public PowerSet(int size) : base(size) {
            elements[0].Add(new Set(0));
            elements[0][0].num = 0;
            elements[0][0].index = 0;
            this.size = 1;

            // Enumerate over all selections of i from size for each i.
            for (int i = 1; i <= size; i++) {
                int powernum = 0;
                ChooseIterator ci = new ChooseIterator(i, size);
                for (; ci.size > 0; ci.Next()) {
                    elements[i].Add(new Set(i));
                    for (int j = 0; j < i; j++) {
                        elements[i][powernum].elements[j] = ci.values[j];
                    }
                    elements[i][powernum].num = this.size;
                    elements[i][powernum].index = powernum;
                    powernum++;
                    this.size++;
                }
            }
            powerset = this;
            factorialSize = Factorial(size);
        }



        /// <summary>
        /// Stores n! for n=size to save computing it in multiple places.
        /// </summary>
        /// <value></value>
        public int FactorialSize {
            get {
                return factorialSize;
            }
        }
        private int factorialSize;



        /// <summary>
        /// Calculates num!
        /// </summary>
        /// <param name="num">
        /// Number to calculate the factorial of.
        /// </param>
        /// <returns>
        /// num!
        /// </returns>
        private int Factorial(int num) {
            int result = 1;
            for (int i = 2; i <= num; i++) {
                result *= i;
            }
            return result;
        }



        /// <summary>
        /// Generates all of the permutations of an array of integers.
        /// </summary>
        /// <param name="basSet">
        /// The array of integers to permute.
        /// </param>
        /// <returns>
        /// The list of all permutations.
        /// </returns>
        private int[,] Permute(int[] basSet) {
            return Permute(basSet, 0);
        }



        /// <summary>
        /// Generates all the permutations of an array of integers, ignoring the first few.
        /// </summary>
        /// <param name="baseSet">
        /// The array of integers to permute.
        /// </param>
        /// <param name="start">
        /// The starting point of the permutation.
        /// </param>
        /// <returns>
        /// The list of all permutations.
        /// </returns>
        /// <remarks>
        /// Recursive implementation.
        /// </remarks>
        private int[,] Permute(int[] baseSet, int start) {
            int size = baseSet.Length - start;
            if (size == 1) {
                int[,] result = new int[1, 1];
                result[0, 0] = baseSet[start];
                return result;
            }
            else {
                // get one less.
                int[,] recurse = Permute(baseSet, start + 1);
                int fact = Factorial(size - 1);
                // Inser the current member into all places in the permutations of one less.
                int[,] result = new int[fact * size, size];
                for (int i = 0; i < fact; i++) {
                    for (int j = 0; j < size; j++) {
                        int cur = i * size + j;
                        for (int k = 0; k < size; k++) {
                            if (j == k)
                                result[cur, k] = baseSet[start];
                            else if (k < j)
                                result[cur, k] = recurse[i, k];
                            else
                                result[cur, k] = recurse[i, k - 1];
                        }
                    }
                }
                return result;
            }
        }



        /// <summary>
        /// Applys a permutation to an array into a second array.
        /// </summary>
        /// <param name="orig">
        /// Original array.
        /// </param>
        /// <param name="permutated">
        /// Array to receive the permuted values.
        /// </param>
        /// <param name="permutations">
        /// Permutations list.
        /// </param>
        /// <param name="permIndex">
        /// Permutation to use from that list.
        /// </param>
        private void ApplyPermutation(int[] orig, int[] permutated, int[,] permutations, int permIndex) {
            for (int i = 0; i < orig.Length; i++) {
                permutated[i] = permutations[permIndex, orig[i] - 1];
            }
        }



        /// <summary>
        /// Precalculate all permutations of every element of the powerset.
        /// </summary>
        private void PrecalcPerms() {
            int size = elements.Length - 1;
            int[] tempset = new int[size];
            for (int i = 0; i < size; i++)
                tempset[i] = i + 1;
            int[,] perms = Permute(tempset);
            Set[] temp = new Set[elements.Length];
            for (int i = 0; i < elements.Length; i++) {
                temp[i] = new Set(i);
            }
            permutationsIndex = new byte[factorialSize, this.size];
            for (int j = 0; j < factorialSize; j++) {
                for (int i = 1; i < size; i++) {
                    for (int k = 0; k < elements[i].Count; k++) {
                        ApplyPermutation(elements[i][k].elements, temp[i].elements, perms, j);
                        Array.Sort(temp[i].elements);
                        int index = elements[i].BinarySearch(temp[i]);
                        if (index < 0)
                            Console.Out.WriteLine("Impossible Happend");
                        permutationsIndex[j, elements[i][k].num] = (byte)index;
                    }
                }
            }
        }



        /// <summary>
        /// Precalculate a union/intersection.
        /// </summary>
        /// <param name="unionTemp">
        /// Set to recieve the union/intersection result.
        /// </param>
        /// <param name="i">
        /// Size of first set.
        /// </param>
        /// <param name="j">
        /// Index of first set.
        /// </param>
        /// <param name="k">
        /// Size of second set.
        /// </param>
        /// <param name="l">
        /// Index of second set.
        /// </param>
        private void PrecalcAUnionIntersect(Set unionTemp, int i, int j, int k, int l) {
            unionTemp.TakeRealUnion(elements[i][j], elements[k][l]);
            int index = elements[unionTemp.Size].BinarySearch(unionTemp);
            unionsSize[elements[i][j].num, elements[k][l].num] = elements[unionTemp.Size][index].Size;
            unionsIndex[elements[i][j].num, elements[k][l].num] = index;
            unionTemp.TakeRealIntersection(elements[i][j], elements[k][l]);
            index = elements[unionTemp.Size].BinarySearch(unionTemp);
            intersectionsSize[elements[i][j].num, elements[k][l].num] = elements[unionTemp.Size][index].Size;
            intersectionsIndex[elements[i][j].num, elements[k][l].num] = index;
        }



        /// <summary>
        /// Precalculates all unions and intersections for every combination of sets in the powerset.
        /// </summary>
        private void PrecalcUnionsIntersects() {
            unionsSize = new int[size, size];
            intersectionsSize = new int[size, size];
            unionsIndex = new int[size, size];
            intersectionsIndex = new int[size, size];
            Set unionTemp = new Set();
            // Do half here.
            for (int i = 0; i < elements.Length; i++) {
                for (int j = 0; j < elements[i].Count; j++) {
                    for (int k = 0; k < i; k++) {
                        for (int l = 0; l < elements[k].Count; l++) {
                            PrecalcAUnionIntersect(unionTemp, i, j, k, l);
                        }
                    }
                    for (int l = 0; l <= j; l++) {
                        PrecalcAUnionIntersect(unionTemp, i, j, i, l);
                    }
                }
            }
            // And copy the other half of the results here.
            for (int i = 0; i < size - 1; i++) {
                for (int j = i + 1; j < size; j++) {
                    unionsSize[i, j] = unionsSize[j, i];
                    intersectionsSize[i, j] = intersectionsSize[j, i];
                    unionsIndex[i, j] = unionsIndex[j, i];
                    intersectionsIndex[i, j] = intersectionsIndex[j, i];
                }
            }
        }



        /// <summary>
        /// Precalculate the set of permutations which map within each number of minimal size 1 sets.
        /// </summary>
        private void PrecalcStartPoint() {
            startPoint = new int[elements.Length, factorialSize + 1];
            Set[] tempset2 = new Set[elements.Length];
            for (int i = 0; i < elements.Length; i++) {
                int j = 0;
                for (int k = 0; k < factorialSize; k++) {
                    bool testvar = true;
                    if (i > 1)
                        Array.Sort<Set>(tempset2, 0, i, new SetPermComparer(k, this));
                    for (int l = 0; l < i; l++) {
                        if (permutationsIndex[k, tempset2[l].num] != l)
                            testvar = false;
                    }
                    if (testvar) {
                        startPoint[i, j] = k;
                        j++;
                    }
                }
                startPoint[i, j] = -1;
                if (i < elements.Length - 1)
                    tempset2[i] = elements[1][i];
            }
        }



        /// <summary>
        /// Do all supported precalculations.
        /// </summary>
        public void InitPrecalc() {
            PrecalcPerms();
            PrecalcUnionsIntersects();
            PrecalcStartPoint();
            // Some potential code for if pair swaps become useful.
            /*
            Console.Out.WriteLine("Pair swaps init\n");
            tempset = new int[size];
            for (int i3 = 0; i3 < size; i3++)
                tempset[i3] = i3 + 1;
            int[] tempset3 = new int[size];
            List<int> pairSwaps = new List<int>();
            for (int i = 0; i < facsize; i++) {
                ApplyPermutation(tempset, tempset3, perms, i);
                int differences = 0;
                for (int j = 0; j < size; j++) {
                    if (tempset[j] != tempset3[j])
                        differences++;
                }
                if (differences == 2)
                    pairSwaps.Add(i);
            }
            Console.Out.WriteLine("PairSwapCount {0}", pairSwaps.Count);
*/
        }



        /// <summary>
        /// Get the indexes of precalculated unions.
        /// </summary>
        /// <value></value>
        public int[,] UnionsIndex {
            get {
                return unionsIndex;
            }
        }
        private int[,] unionsIndex;



        /// <summary>
        /// Get the sizes of precalculated unions.
        /// </summary>
        /// <value></value>
        public int[,] UnionsSize {
            get {
                return unionsSize;
            }
        }
        private int[,] unionsSize;



        /// <summary>
        /// Gets the indexes of precalculated intersections.
        /// </summary>
        /// <value></value>
        public int[,] IntersectionsIndex {
            get {
                return intersectionsIndex;
            }
        }
        private int[,] intersectionsIndex;



        /// <summary>
        /// Gets the sizes of precalculated intersections.
        /// </summary>
        /// <value></value>
        public int[,] IntersectionsSize {
            get {
                return intersectionsSize;
            }
        }
        private int[,] intersectionsSize;



        /// <summary>
        /// Looks up a permutation of a set.
        /// </summary>
        /// <param name="toPermute">
        /// The set to permute.
        /// </param>
        /// <param name="permutation">
        /// The permutation to apply.
        /// </param>
        /// <returns>
        /// The matching permuted set.
        /// </returns>
        public Set PermuteSet(Set toPermute, int permutation) {
            return elements[toPermute.Size][permutationsIndex[permutation, toPermute.num]];
        }



        /// <summary>
        /// Looks up the set which is the intersection of two sets.
        /// </summary>
        /// <param name="a">
        /// Set a.
        /// </param>
        /// <param name="b">
        /// Set b.
        /// </param>
        /// <returns>
        /// The set corresponding to the intersection of a and b.
        /// </returns>
        public Set IntersectSets(Set a, Set b) {
            return elements[intersectionsSize[a.num, b.num]][intersectionsIndex[a.num, b.num]];
        }



        /// <summary>
        /// Looks up the set which is the union of two sets.
        /// </summary>
        /// <param name="a">
        /// Set a.
        /// </param>
        /// <param name="b">
        /// Set b.
        /// </param>
        /// <returns>
        /// The set corresponding to the union of a and b.
        /// </returns>
        public Set UnionSets(Set a, Set b) {
            return elements[unionsSize[a.num, b.num]][unionsIndex[a.num, b.num]];
        }



        /// <summary>
        /// Gets precalculated permutation applications for each set.
        /// </summary>
        /// <value>
        /// Gets precalculated permutation applications for each set.
        /// </value>
        /// <remarks>
        /// Stored as byte as it is a Huge memory waste otherwise.
        /// Size not stored since its already know at location of invocation.
        /// </remarks>
        public byte[,] PermutationsIndex {
            get {
                return permutationsIndex;
            }
        }
        private byte[,] permutationsIndex;



        /// <summary>
        /// Gets the starting points for the minimisation method.
        /// </summary>
        /// <value></value>
        public int[,] StartPoint {
            get {
                return startPoint;
            }
        }
        private int[,] startPoint;
    }
}
