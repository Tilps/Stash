using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{
    public enum LightType
    {
        Ambient,
        Point,
        Directional,
    }

    public class Light
    {
        public ColorIntensity Color = new ColorIntensity();

  /*      public Line Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }
    */    public Line Direction = new Line();

        public LightType LightType
        {
            get
            {
                return lightType;
            }
            set
            {
                lightType = value;
            }
        }
        LightType lightType;

    }
}
