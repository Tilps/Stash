using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    public abstract class Generator
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sizex">
        /// X width of blocks.
        /// </param>
        /// <param name="sizey">
        /// Y width of blocks.
        /// </param>
        /// <param name="rnd">
        /// Random number generator to use.
        /// </param>
        /// <param name="maxLookahead">
        /// Maximum lookahead to use.
        /// </param>
        public Generator(int sizex, int sizey, Random rnd, int maxLookahead)
        {
            this.sizex = sizex;
            this.sizey = sizey;
            this.rnd = rnd;
            this.maxLookahead = maxLookahead;
        }

        protected int sizex;
        protected int sizey;
        protected Random rnd;
        protected int maxLookahead;

        /// <summary>
        /// Gets or sets whether to ensure that the difficulty matches the maximum lookahead.
        /// </summary>
        public bool EnsureDiff
        {
            get
            {
                return ensureDiff;
            }
            set
            {
                ensureDiff = value;
            }
        }
        protected bool ensureDiff = true;

        /// <summary>
        /// Gets the generated x coordinates.
        /// </summary>
        public List<int> Xs
        {
            get
            {
                return xs;
            }
        }
        protected List<int> xs = new List<int>();

        /// <summary>
        /// Gets the generated y coordinates.
        /// </summary>
        public List<int> Ys
        {
            get
            {
                return ys;
            }
        }
        protected List<int> ys = new List<int>();

        /// <summary>
        /// Gets the generated values.
        /// </summary>
        public List<int> Values
        {
            get
            {
                return values;
            }
        }
        protected List<int> values = new List<int>();

        public abstract void Generate();
        
    }
}
