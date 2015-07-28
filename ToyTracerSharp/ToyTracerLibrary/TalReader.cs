using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ToyTracerLibrary.CSG;
using ToyTracerLibrary.Bricks;
using System.Windows.Media;

namespace ToyTracerLibrary
{
    public class TalReader
    {

        public TalReader()
        {
            BoundsComparers[0] = new BoundsComparer(0);
            BoundsComparers[1] = new BoundsComparer(1);
            BoundsComparers[2] = new BoundsComparer(2);
        }

        private struct Settings
        {
            public int sx, sy;
            public double ex, ey, ez;
            public double vax, vay, vaz;
            public double vux, vuy, vuz;
            public double f;
            public double vsx, vsy;
            public bool recSpec;
            public int recDepth;
            public double recCutoff;
            public bool aaSpec;
            public int aaDepth;
            public int aaCutoffMinRep;
            public double aaCutoff;
            public bool photonSpec;
            public int photonMaxStep;
            public int photonTargetCount;
            public int photonTargetMaxCount;
            public int photonCount;
            public string photonFile;
            public int threadCount;
        }
        Settings input;

        public void Read(string fileName)
        {
            worldlist = new List<Primative>();
            worldlistUnion = false;
            viewport = new View();
            lights = new List<Light>();
            using (StreamReader reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    line = line.Trim();
                    string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0)
                        continue;
                    string first = parts[0].ToLower();
                    switch (first)
                    {
                        case "imagesize":
                            if (parts.Length != 3)
                                continue;
                            else
                            {
                                input.sx = int.Parse(parts[1]);
                                input.sy = int.Parse(parts[2]);
                            }
                            break;
                        case "worldlistunion":
                            if (parts.Length != 1)
                                continue;
                            else
                                worldlistUnion = true;
                            break;
                        case "eye":
                            if (parts.Length != 4)
                                continue;
                            else
                            {
                                input.ex = double.Parse(parts[1]);
                                input.ey = double.Parse(parts[2]);
                                input.ez = double.Parse(parts[3]);
                            }
                            break;
                        case "viewat":
                            if (parts.Length != 4)
                                continue;
                            else
                            {
                                input.vax = double.Parse(parts[1]);
                                input.vay = double.Parse(parts[2]);
                                input.vaz = double.Parse(parts[3]);
                            }
                            break;
                        case "viewup":
                            if (parts.Length != 4)
                                continue;
                            else
                            {
                                input.vux = double.Parse(parts[1]);
                                input.vuy = double.Parse(parts[2]);
                                input.vuz = double.Parse(parts[3]);
                            }
                            break;
                        case "focal":
                            if (parts.Length != 2)
                                continue;
                            else
                            {
                                input.f = double.Parse(parts[1]);
                            }
                            break;
                        case "viewsize":
                            if (parts.Length != 3)
                                continue;
                            else
                            {
                                input.vsx = double.Parse(parts[1]);
                                input.vsy = double.Parse(parts[2]);
                            }
                            break;
                        case "recurselimits":
                            if (parts.Length != 3)
                                continue;
                            else
                            {
                                input.recSpec = true;
                                input.recDepth = int.Parse(parts[1]);
                                input.recCutoff = double.Parse(parts[2]);
                            }
                            break;
                        case "processing":
                            if (parts.Length != 2)
                                continue;
                            else
                            {
                                input.threadCount = int.Parse(parts[1]);
                            }
                            break;
                        case "photonsave":
                            if (parts.Length == 1)
                                continue;
                            else
                            {
                                input.photonFile = line.Substring("photonsave".Length + 1);
                            }
                            break;
                        case "photonlimits":
                            if (parts.Length != 5)
                                continue;
                            else
                            {
                                input.photonSpec = true;
                                input.photonMaxStep = int.Parse(parts[1]);
                                input.photonTargetCount = int.Parse(parts[2]);
                                input.photonTargetMaxCount = int.Parse(parts[3]);
                                input.photonCount = int.Parse(parts[4]);
                            }
                            break;
                        case "antialiasing":
                            if (parts.Length != 3 && parts.Length != 4)
                                continue;
                            else
                            {
                                input.aaSpec = true;
                                input.aaDepth = int.Parse(parts[1]);
                                input.aaCutoff = double.Parse(parts[2]);
                                if (parts.Length > 3)
                                    input.aaCutoffMinRep = int.Parse(parts[3]);
                            }
                            break;
                        case "pointsource":
                        case "directional":
                            if (parts.Length != 5 && parts.Length != 6)
                                continue;
                            else
                            {
                                double power = 1.0;
                                if (parts.Length == 6)
                                {
                                    power = double.Parse(parts[5]);
                                }
                                ColorIntensity color = ParseColor(parts[4]);
                                double x = double.Parse(parts[1]);
                                double y = double.Parse(parts[2]);
                                double z = double.Parse(parts[3]);
                                Light l = new Light();
                                if (first == "pointsource")
                                {
                                    l.LightType = LightType.Point;
                                    l.Direction.Start.X = x;
                                    l.Direction.Start.Y = y;
                                    l.Direction.Start.Z = z;
                                }
                                else
                                {
                                    l.LightType = LightType.Directional;
                                    l.Direction.Direct.Dx = x;
                                    l.Direction.Direct.Dy = y;
                                    l.Direction.Direct.Dz = z;
                                }
                                l.Color.R = color.R * power;
                                l.Color.G = color.G * power;
                                l.Color.B = color.B * power;
                                lights.Add(l);
                            }
                            break;
                        case "ambient":
                            if (parts.Length != 2 && parts.Length != 3)
                                continue;
                            else
                            {
                                double power = 1.0;
                                if (parts.Length == 3)
                                {
                                    power = double.Parse(parts[2]);
                                }
                                ColorIntensity color = ParseColor(parts[1]);
                                Light l = new Light();
                                l.LightType = LightType.Ambient;
                                l.Color.R = color.R * power;
                                l.Color.G = color.G * power;
                                l.Color.B = color.B * power;
                                lights.Add(l);
                            }
                            break;
                        case "#":
                            continue;
                        default:
                            if (parts.Length < 9 || parts[parts.Length - 1].ToLower() != "end")
                                continue;
                            else
                            {
                                ColorIntensity color = ParseColor(parts[1]);
                                double x = double.Parse(parts[2]);
                                double y = double.Parse(parts[3]);
                                double z = double.Parse(parts[4]);
                                double rotx = double.Parse(parts[5]) / 360 * 2.0 * Math.PI;
                                double roty = double.Parse(parts[6]) / 360 * 2.0 * Math.PI;
                                double rotz = double.Parse(parts[7]) / 360 * 2.0 * Math.PI;
                                string[] rest = new string[parts.Length - 9];
                                Array.Copy(parts, 8, rest, 0, parts.Length - 9);
                                bool transparent;
                                Primative bit = ParsePrimative(first, rest, out transparent);
                                bit.Surface = new Material("plastic", color.R, color.G, color.B, transparent);
                                ColorIntensity colorIntens;
                                colorIntens.R = color.R;
                                colorIntens.G = color.G;
                                colorIntens.B = color.B;
                                bit.Pigment = new SolidTexture(colorIntens);
                                bit.Shadow = !transparent;
                                Matrix mat1 = new Matrix();
                                Matrix invmat1 = new Matrix();
                                mat1.MakeRot(rotx, 0, 0);
                                invmat1.MakeRot(-rotx, 0, 0);
                                bit.ApplyMatrix(mat1, invmat1);
                                mat1.MakeRot(0, roty, 0);
                                invmat1.MakeRot(0, -roty, 0);
                                bit.ApplyMatrix(mat1, invmat1);
                                mat1.MakeRot(0, 0, -rotz);
                                invmat1.MakeRot(0, 0, rotz);
                                bit.ApplyMatrix(mat1, invmat1);
                                mat1.MakeTrans(x, y, z);
                                invmat1.MakeTrans(-x, -y, -z);
                                bit.ApplyMatrix(mat1, invmat1);

                                worldlist.Add(bit);
                            }
                            break;
                    }
                }
            }
            viewport.ScreenX = input.sx;
            viewport.ScreenY = input.sy;
            viewport.ViewWidth = input.vsx;
            viewport.ViewHeight = input.vsy;
            viewport.Focal = input.f;
            Point eye = new Point(input.ex, input.ey, input.ez);
            Point eyelook = new Point(input.vax, input.vay, input.vaz);
            Point eyeup = new Point(input.vux, input.vuy, input.vuz);
            viewport.Pos(eye);
            viewport.LookAt(eyelook);
            viewport.Upward(eyeup);

        }

        private void GetBrickSettings(string[] rest, out int width, out int length, out bool trans)
        {
            width = 0;
            length = 0;
            trans = false;
            for (int i = 0; i < rest.Length; i++)
            {
                string cur = rest[i].ToLower();
                switch (cur)
                {
                    case "width":
                        width = int.Parse(rest[i + 1]);
                        i++;
                        break;
                    case "length":
                    case "height":
                        length = int.Parse(rest[i + 1]);
                        i++;
                        break;
                    case "transparent":
                    case "translucent":
                    case "trans":
                        trans = true;
                        break;
                }
            }
        }

        private void GetSphereSettings(string[] rest, out double width, out bool trans)
        {
            width = 0;
            trans = false;
            for (int i = 0; i < rest.Length; i++)
            {
                string cur = rest[i].ToLower();
                switch (cur)
                {
                    case "width":
                        width = double.Parse(rest[i + 1]);
                        i++;
                        break;
                    case "transparent":
                    case "translucent":
                    case "trans":
                        trans = true;
                        break;
                }
            }
        }

        private Primative ParsePrimative(string first, string[] rest, out bool transparent)
        {
            Primative res = null;
            transparent = false;
            int width;
            int length;
            bool trans;
            switch (first)
            {
                case "brick":
                    GetBrickSettings(rest, out width, out length, out trans);
                    res = new Brick(width, length);
                    transparent = trans;
                    break;
                case "tile":
                    GetBrickSettings(rest, out width, out length, out trans);
                    res = new Tile(width, length);
                    transparent = trans;
                    break;
                case "plate":
                    GetBrickSettings(rest, out width, out length, out trans);
                    res = new Plate(width, length);
                    transparent = trans;
                    break;
                case "baseplate":
                    GetBrickSettings(rest, out width, out length, out trans);
                    res = new BasePlate(width, length);
                    transparent = trans;
                    break;
                case "sphere":
                    double radius = 0.0;
                    GetSphereSettings(rest, out radius,out trans);
                    res = new HollowSphere(radius);
                    transparent = trans;
                    break;
            }
            return res;
        }


        private ColorIntensity ParseColor(string p)
        {
            ColorIntensity res;
            if (p.StartsWith("rgb:"))
            {
                Color inter = new Color();
                inter.R = (byte)int.Parse(p.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                inter.G = (byte)int.Parse(p.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                inter.B = (byte)int.Parse(p.Substring(8, 2), System.Globalization.NumberStyles.HexNumber);
                res.R = inter.ScR;
                res.G = inter.ScG;
                res.B = inter.ScB;
            }
            else
            {
                System.Drawing.Color c = System.Drawing.Color.FromName(p);
                Color inter = new Color();
                inter.R = c.R;
                inter.G = c.G;
                inter.B = c.B;
                res.R = inter.ScR;
                res.G = inter.ScG;
                res.B = inter.ScB;
            }
            return res;
        }

        List<Primative> worldlist = new List<Primative>();
        bool worldlistUnion = false;
        View viewport = new View();
        List<Light> lights = new List<Light>();

        public ToyTracer CreateSceneRenderer()
        {
            ToyTracer tt = new ToyTracer();
            tt.Scene = MakeWorld(worldlist, worldlistUnion);
            tt.Viewport = viewport;
            tt.Lights.AddRange(lights);
            if (input.aaSpec)
            {
                tt.AntiAlias = true;
                tt.AntiAliasCutOut = input.aaCutoff;
                tt.AntiAliasLevel = input.aaDepth;
                if (input.aaCutoffMinRep != 0)
                    tt.AntiAliasCutOutMinRepetition = input.aaCutoffMinRep;
            }
            if (input.recSpec)
            {
                tt.MaxRecurse = input.recDepth;
                tt.RecurseMinRatio = input.recCutoff;
            }
            if (input.photonSpec)
            {
                tt.Photons = true;
                tt.Map.MaxExpandSteps = input.photonMaxStep;
                tt.Map.TargetMinCount = input.photonTargetCount;
                tt.Map.TargetCount = input.photonTargetMaxCount;
                tt.PhotonCount = input.photonCount;
                tt.PhotonFile = input.photonFile;
            }
            if (input.threadCount > 0)
                tt.ThreadCount = input.threadCount;
            /*
            List<Primative> worldlist = new List<Primative>();
            Brick brick = new Brick(3, 4);
            brick.Surface = new Material("plastic", 255, 10, 10, true);
            brick.Shadow = false;
            worldlist.Add(brick);
            brick = new Brick(3, 2);
            brick.Surface = new Material("plastic", 100, 255, 20, true);
            brick.Shadow = false;
            Matrix trans = new Matrix();
            trans.MakeTrans(1, 1, 1.2);
            Matrix invTrans = new Matrix();
            invTrans.MakeTrans(-1, -1, -1.2);
            brick.ApplyMatrix(trans, invTrans);
            worldlist.Add(brick);
            tt.Scene = MakeWorld(worldlist);
            Light light = new Light();
            light.LightType = LightType.Directional;
            light.Color[0] = 1.0;
            light.Color[1] = 1.0;
            light.Color[2] = 1.0;
            light.Direction.Direct.Dx = 0.8;
            light.Direction.Direct.Dy = -0.9;
            light.Direction.Direct.Dz = -0.3;
            tt.Lights.Add(light);
            Light light2 = new Light();
            light2.LightType = LightType.Ambient;
            light2.Color[0] = 3.0;
            light2.Color[1] = 3.0;
            light2.Color[2] = 3.0;
            tt.Lights.Add(light2);

            View view = new View();
            view.Focal = 1.0;
            view.ScreenX = 640;
            view.ScreenY = 400;
            view.ViewHeight = 1.0;
            view.ViewWidth = 1.6;
            view.Pos(new Point(5, 5, 5));
            view.LookAt(new Point(0, 0, 0));
            view.Upward(new Point(0, 0, 10));
            tt.Viewport = view;
             * */
            return tt;
        }

        BoundsComparer[] BoundsComparers = new BoundsComparer[3];

        private Primative MakeWorld(List<Primative> worldlist, bool worldlistUnion)
        {
            return MakeWorld(worldlist, 0, worldlist.Count - 1, !worldlistUnion);
        }
        private Primative MakeWorld(List<Primative> worldlist, int first, int last, bool disjoint)
        {
            if (first > last)
                return null;
            if (first == last)
                return worldlist[first];
            if (last - first == 1)
                return new CSGUnion(worldlist[first], worldlist[last], disjoint);
            Point mins = new Point(double.MaxValue, double.MaxValue, double.MaxValue);
            Point maxs = new Point(double.MinValue, double.MinValue, double.MinValue);
            for (int i = first; i <= last; i++)
            {
                double x = worldlist[first].Bounds.Centre.X;
                if (x > maxs.X)
                    maxs.X = x;
                if (x < mins.X)
                    mins.X = x;
                double y = worldlist[first].Bounds.Centre.Y;
                if (y > maxs.Y)
                    maxs.Y = y;
                if (y < mins.Y)
                    mins.Y = y;
                double z = worldlist[first].Bounds.Centre.Z;
                if (z > maxs.Z)
                    maxs.Z = z;
                if (z < mins.Z)
                    mins.Z = z;
            }
            int longest = 0;
            double longLength = maxs.X - mins.X;
            if (maxs.Y - mins.Y > longLength)
            {
                longest = 1;
                longLength = maxs.Y - mins.Y;
            }
            if (maxs.Z - mins.Z > longLength)
                longest = 2;
            int total = last - first + 1;
            worldlist.Sort(first, total, BoundsComparers[longest]);
            // Now we choose where to split.
            AllignedBox[] leftBounds = new AllignedBox[total];
            AllignedBox[] rightBounds = new AllignedBox[total];
            leftBounds[0] = worldlist[first].Bounds;
            rightBounds[0] = worldlist[last].Bounds;
            for (int i = 1; i < total; i++)
            {
                leftBounds[i] = leftBounds[i - 1].UnionWith(worldlist[first + i].Bounds);
                rightBounds[i] = rightBounds[i - 1].UnionWith(worldlist[last - i].Bounds);
            }
            double bestScore = double.MaxValue;
            int bestIndex = -1;
            for (int i = 0; i < total-1; i++)
            {
                double score = leftBounds[i].SurfaceArea * (i + 1) + rightBounds[total - 1 - 1 - i].SurfaceArea * (total - 1 - i);
                if (score < bestScore)
                {
                    bestScore = score;
                    bestIndex = i;
                }
            }
            return new CSGUnion(MakeWorld(worldlist, first, first + bestIndex, disjoint), MakeWorld(worldlist, first + bestIndex + 1, last, disjoint), disjoint);
        }

    }

    public class BoundsComparer : IComparer<Primative>
    {
        public BoundsComparer(int type)
        {
            this.type = type;
        }
        private int type;
        #region IComparer<Primative> Members

        public int Compare(Primative x, Primative y)
        {
            if (type == 0)
                return x.Bounds.Centre.X.CompareTo(y.Bounds.Centre.X);
            if (type == 1)
                return x.Bounds.Centre.Y.CompareTo(y.Bounds.Centre.Y);
            if (type == 2)
                return x.Bounds.Centre.Z.CompareTo(y.Bounds.Centre.Z);
            throw new ApplicationException("Invalid dimension.");
        }

        #endregion
    }
}
