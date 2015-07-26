#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace FiniteCalculator {



    /// <summary>
    /// Implements a poorly designed choose iterator.
    /// </summary>
    public class ChooseIterator {



        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="count">
        /// Number of elements to choose from.
        /// </param>
        /// <param name="size">
        /// Size of the choice.
        /// </param>
        public ChooseIterator(int count, int size) {
            this.count = count;
            this.size = size;
            values = new int[count];
            for (int i = 0; i < count; i++) {
                values[i] = i + 1;
            }
        }



        /// <summary>
        /// Moves to the next choice.
        /// </summary>
        public void Next() {
            int a = count - 1;
            while (a >= 0 && values[a] == size + a - count + 1)
                a--;
            if (a < 0) {
                size = 0;
                return;
            }
            values[a]++;
            a++;
            while (a < count) {
                values[a] = values[a - 1] + 1;
                a++;
            }
        }



        /// <summary>
        /// Gets the current choice.
        /// </summary>
        public int[] values;



        /// <summary>
        /// Stores the size of the basis.
        /// </summary>
        public int count;



        /// <summary>
        /// Stores the size of the choice.  This is set to 0 when next moves past end of choices.
        /// </summary>
        public int size;
    }
}
