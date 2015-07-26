#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#endregion

namespace FiniteCalculator {



    /// <summary>
    /// Generates finite topologies which are unique under isomorphisms.
    /// </summary>
    class FiniteTopologyGenerator {



        /// <summary>
        /// Attempts to add a topology to a list of existing topologies.
        /// </summary>
        /// <param name="topologies">
        /// The existing topologies.
        /// </param>
        /// <param name="newTopol">
        /// The topologies to add.
        /// </param>
        /// <returns>
        /// True if the topology wasnt already in the list.
        /// </returns>
        /// <remarks>
        /// Ensures that the stored topology has had its minimal form applied.
        /// </remarks>
        static bool CheckAdd(List<Topology>[] topologies, Topology newTopol) {
            int index = topologies[newTopol.Size].BinarySearch(newTopol);
            if (index >= 0) {
                topologies[newTopol.Size][index].duplicates++;
                return false;
            }
            index = ~index;
            topologies[newTopol.Size].Insert(index, newTopol);
            newTopol.ReMin();
            return true;
        }



        /// <summary>
        /// Checks if a topology can be added to a list of topologies.
        /// </summary>
        /// <param name="topologies">
        /// The existing topologies.
        /// </param>
        /// <param name="newTopol">
        /// The topology to check for.
        /// </param>
        /// <returns>
        /// True if the topology could be added.
        /// False otherwise.
        /// </returns>
        public static bool Check(List<Topology>[] topologies, Topology newTopol) {
            int index = topologies[newTopol.Size].BinarySearch(newTopol);
            if (index >= 0) {
                return false;
            }
            return true;
        }



        /// <summary>
        /// Checks if a topology can be added to the 'storedTopologies' list.
        /// </summary>
        /// <param name="newTopol">
        /// Topology to check.
        /// </param>
        /// <returns>
        /// True if the topology could be added.
        /// False otherwise.
        /// </returns>
        public static bool Check(Topology newTopol) {
            return Check(storedTopologies, newTopol);
        }



        /// <summary>
        /// Checks and adds if a topology can be added to the 'seenTopologies' list.
        /// </summary>
        /// <param name="newTopol">
        /// Topology to check/add.
        /// </param>
        /// <returns>
        /// True if the topology could be added.
        /// False otherwise.
        /// </returns>
        public static bool CheckAdd(Topology newTopol) {
            return CheckAdd(seenTopologies, newTopol);
        }



        /// <summary>
        /// The stored topologies list.
        /// </summary>
        private static List<Topology>[] storedTopologies;



        /// <summary>
        /// The seen topologies list.
        /// </summary>
        private static List<Topology>[] seenTopologies;



        /// <summary>
        /// Main, runs the topology generation.
        /// </summary>
        /// <param name="args">
        /// Args, which arent used.
        /// </param>
        static void Main(string[] args) {
            Console.Out.Write("Please enter size of base set: ");
            string answer = Console.In.ReadLine();
            int size = int.Parse(answer);
            Console.Out.WriteLine("Memory init 1");
            PowerSet powerset = new PowerSet(size);
            Console.Out.WriteLine("Precalc init\n");
            powerset.InitPrecalc();

            Console.Out.WriteLine("Starting");

            List<Topology>[] topologies = new List<Topology>[powerset.Size + 1];
            seenTopologies = new List<Topology>[powerset.Size + 1];
            storedTopologies = topologies;
            for (int i = 0; i < powerset.Size + 1; i++) {
                topologies[i] = new List<Topology>();
                seenTopologies[i] = new List<Topology>();
            }
            Topology first = new Topology(powerset);
            topologies[first.Size].Add(first);
            int total = 1;
            int failure = 0;
            int outerFailure = 0;
            int midFailure = 0;
            int midMinFailure = 0;
            //    int useless = 0;
            // Iterate over the sizes of the topologies.
            for (int i = 2; i < powerset.Size + 1; i++) {
                // For each size iterate over its topologies.
                for (int j = 0; j < topologies[i].Count; j++) {
                    Topology next = topologies[i][j];
                    int nextIndex2 = 0;

                    // Attempt to add each set from the powerset which is not already a member of the topology.
                    for (int nextSize = 1; nextSize < powerset.Elements.Length - 1; nextSize++) {
                        nextIndex2 = 0;
                        for (int current = 0; current < powerset.Elements[nextSize].Count; current++) {
                            Set currentSet = powerset.Elements[nextSize][current];
                            bool exit = false;
                            // verify the set isnt already in the topology.
                            while (nextIndex2 < next.Elements[nextSize].Count) {
                                int thisNum = next.Elements[nextSize][nextIndex2].num;
                                if (currentSet.num < thisNum)
                                    break;
                                if (currentSet.num == thisNum) {
                                    exit = true;
                                    break;
                                }
                                nextIndex2++;
                            }
                            if (exit)
                                continue;

                            Topology newt = next.Copy();
                            // Attempt to add the set to the topology to generate a new topology.
                            if (newt.AddProper(currentSet) == 0) {

                                // Check it isnt already minimal and we've already found it.
                                // Supprisingly this check is Very effective.
                                if (!Check(topologies, newt)) {
                                    midFailure++;
                                    continue;
                                }
                                
                                // damn premin actually makes things slower!
                                /*
                                newt.PreMin(pairSwaps.ToArray());
                                if (!Check(topologies, newt)) {
                                    midFailure++;
                                    continue;
                                }
                                */

                                // Minimise.
                                if (!newt.Minimize()) {
                                    midMinFailure++;
                                    continue;
                                }
                                // Attempt to add it to the collection of minimal topologies.
                                if (!CheckAdd(topologies, newt)) {
                                    failure++;
                                }
                                else {
                                    total++;
                                }
                            }
                            else {
                                outerFailure++;
                            }
                        }
                    }
                    Console.Out.Write(next);
                    // This is a Lot of output.
                    Console.Out.WriteLine("Summary:");
                    Console.Out.WriteLine("Subtotal: {0}", total);
                    Console.Out.WriteLine("Dups Subtotal: {0}", failure);
                    Console.Out.WriteLine("PreDups: {0}", outerFailure);
                    Console.Out.WriteLine("MidDups: {0}", midFailure);
                    Console.Out.WriteLine("MidMinDups: {0}", midMinFailure);
                }
                // get rid of data we can not need anymore.
                topologies[i].Clear();
                seenTopologies[i].Clear();
            }
            Console.Out.WriteLine("Final total: {0}", total);
            Console.In.ReadLine();
        }
    }
}
