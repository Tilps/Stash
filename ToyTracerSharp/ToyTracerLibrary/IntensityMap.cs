using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{
    public class IntensityMap
    {
         public int Width
        {
            get
            {
                return width;
            }
        }
        private int width;

        public int Height
        {
            get
            {
                return height;
            }
        }
        private int height;

        public IntensityMap(int width, int height) {
            this.width = width;
            this.height = height;
            pix = new ColorIntensity[width * height];
        }

        public ColorIntensity[] Pix {
            get {
                return pix;
            }
        }
        private ColorIntensity[] pix;
   }
}
