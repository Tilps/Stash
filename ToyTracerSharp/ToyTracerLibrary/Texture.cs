using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{
    public abstract class Texture
    {
        public abstract ColorIntensity GetTexture(Line location, int layer);
    }

    public class SolidTexture : Texture
    {
        public SolidTexture(ColorIntensity color)
        {
            this.color = color;
        }
        private ColorIntensity color;

        public override ColorIntensity GetTexture(Line location, int layer)
        {
            return color;
        }
    }
}
