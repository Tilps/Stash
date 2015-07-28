using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace ToyTracerLibrary
{
    public class PixelMap
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

        public PixelMap(int width, int height) {
            this.width = width;
            this.height = height;
            pix = new Color[width * height];
        }

        public Color[] Pix {
            get {
                return pix;
            }
        }
        private Color[] pix;
    }
}
