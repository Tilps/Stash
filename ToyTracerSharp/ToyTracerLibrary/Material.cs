using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{
    public class Material
    {
        public void SetByName(string matname)
        {
            specularity = 0.0;
            attenuationPower = 1001;
            attenuationDistance = 0.4;
            if (matname == "gold")
            {
                refractive = false;
                attenutive = false;
                reflective = true;
                reflectance[0] = 0;
                reflectance[1] = 0;
                reflectance[2] = 0;
                ambient[0] = 0.247;
                ambient[1] = 0.199;
                ambient[2] = 0.074;
                diffuse = 0.3;
                phong = 0.7;
                exponent = 70;
                brilliance = 3;
            }
            else if (matname == "plastic")
            {
                refractive = false;
                refractIndex = 1.5;
                attenutive = false;
                reflective = true;
                reflectance[0] = 0.1;
                reflectance[1] = 0.1;
                reflectance[2] = 0.1;
                ambient[0] = 0.1;
                ambient[1] = 0.1;
                ambient[2] = 0.1;
                diffuse = 0.6;
                phong = 0.4;
                exponent = 60;
                //specular = 0.3;
                //roughness = 0.003;
                brilliance = 1;
                attenuationDistance = 0.8;
            }
            else if (matname == "glass")
            {
                refractIndex = 1.5;
                refractive = false;
                attenutive = false;
                reflective = true;
                reflectance[0] = 0.1;
                reflectance[1] = 0.1;
                reflectance[2] = 0.1;
                ambient[0] = 0.1;
                ambient[1] = 0.1;
                ambient[2] = 0.1;
                diffuse = 0.1;
                specular = 1.8;
                roughness = 0.003;
                brilliance = 1;
            }

        }

        public double Specularity
        {
            get
            {
                return specularity;
            }
        }
        private double specularity;

        public string MatnameIn
        {
            get
            {
                return matnameIn;
            }
        }
        string matnameIn;

        public double FilmThickness
        {
            get
            {
                return filmThickness;
            }
        }
        private double filmThickness;

        public double Iridescence
        {
            get
            {
                return iridescence;
            }
        }
        private double iridescence;

        public double[] Ambient
        {
            get
            {
                return ambient;
            }
        }
        double[] ambient = new double[3];

        public double Diffuse
        {
            get
            {
                return diffuse;
            }
        }
        double diffuse;

        public double Brilliance
        {
            get
            {
                return brilliance;
            }
        }
        double brilliance;

        public double Phong
        {
            get
            {
                return phong;
            }
            set
            {
                phong = value;
            }
        }
        double phong;

        public double Exponent
        {
            get
            {
                return exponent;
            }
            set
            {
                exponent = value;
            }
        }
        double exponent;

        public double Specular
        {
            get
            {
                return specular;
            }
        }
        double specular;

        public double Roughness
        {
            get
            {
                return roughness;
            }
            set
            {
                roughness = value;
            }
        }
        double roughness;

        public bool Refractive
        {
            get
            {
                return refractive;
            }
            set
            {
                refractive = value;
            }
        }
        bool refractive;

        public double RefractIndex
        {
            get
            {
                return refractIndex;
            }
            set
            {
                refractIndex = value;
            }
        }
        double refractIndex;

        public bool Reflective
        {
            get
            {
                return reflective;
            }
            set
            {
                reflective = value;
            }
        }
        bool reflective;

        public double[] Reflectance
        {
            get
            {
                return reflectance;
            }
        }
        double[] reflectance = new double[3];

        public bool Attenutive
        {
            get
            {
                return attenutive;
            }
            set
            {
                attenutive = value;
            }
        }
        bool attenutive;

        public double[] Attenuation
        {
            get
            {
                return attenuation;
            }
        }
        double[] attenuation = new double[3];

        public double AttenuationPower
        {
            get
            {
                return attenuationPower;
            }
        }
        double attenuationPower;

        public double AttenuationDistance
        {
            get
            {
                return attenuationDistance;
            }
        }
        double attenuationDistance;

        public Material(string matname)
        {
            SetByName(matname);
            matnameIn = matname;
        }

        public Material(string matname, double red, double green, double blue, bool transparent)
        {
            SetByName(matname);
            matnameIn = matname;
            double rred = red;
            double rgreen = green;
            double rblue = blue;
            if (transparent)
            {
                specularity = 0.90;
                reflectance[0] = 0.0;
                reflectance[1] = 0.0;
                reflectance[2] = 0.0;
                diffuse = 1.0;
            }
            else
                specularity = 0.0;
            double ratio = 1.0/(1.0-specularity);
            ambient[0] = ambient[0] * rred / ratio;
            ambient[1] = ambient[1] * rgreen / ratio;
            ambient[2] = ambient[2] * rblue / ratio;
            attenuation[0] = rred/1.6;
            attenuation[1] = rgreen/1.6;
            attenuation[2] = rblue/1.6;
            refractive = transparent;
            attenutive = transparent;
            reflective = !transparent;
        }

        public void ScaleBy(int r, int g, int b)
        {
            double rred = ((double)r) / 255.0;
            double rgreen = ((double)g) / 255.0;
            double rblue = ((double)b) / 255.0;
            ambient[0] = ambient[0] * rred;
            ambient[1] = ambient[1] * rgreen;
            ambient[2] = ambient[2] * rblue;
            attenuation[0] = rred/1.6;
            attenuation[1] = rgreen/1.6;
            attenuation[2] = rblue/1.6;
        }
    }
}
