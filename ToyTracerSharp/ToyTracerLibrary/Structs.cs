using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{

    public struct ColorIntensity
    {
        public double R, G, B;

        internal double GreyScale()
        {
            // TODO: do green promotion as per human eye reactivity.
            return (R + G + B) / 3;
        }
    }

    public struct ColorIntensityF
    {
        public float R, G, B;

    }
}
