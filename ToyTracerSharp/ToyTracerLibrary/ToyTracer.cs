using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using System.IO;
using System.Threading;

namespace ToyTracerLibrary
{
    public delegate void ProgressEventHandler(PixelMap current, int heightToDraw, int completed); 

    public class ToyTracer
    {
        public const double EPSILON = 0.00000001;

        public ToyTracer()
        {
            maxrecurse = 12;
            minratio = 0.002;
            antialias = false;
            AAn = 9;
            AAco = 0.1;
            AAcomr = 2;
        }

        public event ProgressEventHandler OnProgress;

        public bool allLightsArePhotons = false;

        public bool realLighting = true;

        public PixelMap Trace()
        {
            return Trace(ThreadCount);
        }
        public int ThreadCount = 1;
        public PhotonMap Map = new PhotonMap();
        public string PhotonFile;
        public int PhotonCount;
        public HitInfoListPool[] Pools;
#if DEBUG
        public Photon[] PhotonParents;
#endif

        private PixelMap Trace(int threadCount)
        {
            if (scene == null)
            {
                throw new NullReferenceException("Empty Scene");
            }
            scene.SetTracer(this, threadCount);
            raycount = new int[threadCount];
            Pools = new HitInfoListPool[threadCount];
#if DEBUG
            PhotonParents = new Photon[threadCount];
#endif
            for (int i = 0; i < threadCount; i++)
                Pools[i] = new HitInfoListPool();
            if (photons)
            {
                if (PhotonFile == null || !File.Exists(PhotonFile))
                {
                    Map.InitForThreads(threadCount, true);
                    Thread[] pthreads = new Thread[threadCount];
                    for (int i = 0; i < threadCount; i++)
                    {
                        pthreads[i] = new Thread(GeneratePhotonsWrap);
                        pthreads[i].IsBackground = true;
                        PhotonThreadArgs args = new PhotonThreadArgs();
                        args.threadCount = threadCount;
                        args.threadId = i;
                        pthreads[i].Start(args);
                    }
                    for (int i = 0; i < threadCount; i++)
                    {
                        pthreads[i].Join();
                    }
                    Map.BalanceMap();
                    if (PhotonFile != null)
                        Map.Save(PhotonFile);
                    Map.OptimiseRadi();
                }
                else
                {
                    Map.InitForThreads(threadCount, false);
                    Map.Load(PhotonFile);
                    Map.ValidateBalance();
                    Map.OptimiseRadi();
                }
            }
            PixelMap scrpix = new PixelMap(viewport.ScreenX, viewport.ScreenY);
            Color[] screen = scrpix.Pix;

            List<ColorIntensity>[] shadings = new List<ColorIntensity>[screen.Length];
            for (int i = 0; i < shadings.Length; i++)
                shadings[i] = new List<ColorIntensity>();
            Counter completed = new Counter();
            int[] successive = new int[screen.Length];
            if (OnProgress != null)
            {
                OnProgress(scrpix, 0 , 0);
            }
            Thread[] threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                threads[i] = new Thread(OneRenderThreadWrap);
                threads[i].IsBackground = true;
                RenderThreadArgs args = new RenderThreadArgs();
                args.threadCount = threadCount;
                args.threadId = i;
                args.successive = successive;
                args.shadings = shadings;
                args.scrpix = scrpix;
                args.screen = screen;
                args.completed = completed;
                threads[i].Start(args);
            }
            for (int i = 0; i < threadCount; i++)
            {
                threads[i].Join();
            }
            return scrpix;
        }

        private class PhotonThreadArgs
        {
            public int threadCount;
            public int threadId;
        }

        private class RenderThreadArgs
        {
            public int threadCount;
            public int threadId;
            public PixelMap scrpix;
            public Color[] screen;
            public List<ColorIntensity>[] shadings;
            public int[] successive;
            public Counter completed;
        }

        private class Counter
        {
            public int Incriment()
            {
                return Interlocked.Increment(ref counter);
            }
            private int counter;
        }

        private void OneRenderThreadWrap(object input)
        {
            RenderThreadArgs args = (RenderThreadArgs)input;
            OneRenderThread(args.threadCount, args.threadId, args.scrpix, args.screen, args.shadings, args.successive, args.completed);
        }

        private void OneRenderThread(int threadCount, int threadId, PixelMap scrpix, Color[] screen, List<ColorIntensity>[] shadings, int[] successive, Counter completed)
        {

            int localAAn = AAn;
            int localAAcomr = AAcomr;
            if (!antialias)
            {
                localAAn = 0;
                localAAcomr = 1;
            }
            int tries = 0;
            int localCompleted = 0;
            Random localRnd = new Random();
            while (tries <= localAAn)
            {
                int scrIdx = 0;
                for (int y = 0; y < viewport.ScreenY; y++)
                {
                    if (y % threadCount != threadId)
                    {
                        scrIdx += viewport.ScreenX;
                        continue;
                    }
                    for (int x = 0; x < viewport.ScreenX; x++)
                    {
                        if (successive[scrIdx] < localAAcomr)
                        {
                            ColorIntensity newshading;
                            double rndx = 0.0;
                            double rndy = 0.0;
                            if (antialias)
                            {
                                rndx = localRnd.NextDouble() - 0.5;
                                rndy = localRnd.NextDouble() - 0.5;
                            }
                            Line ray = viewport.RayThrough(x + rndx, y + rndy);
                            SetIn(ref ray.Start, threadId);
                            HitInfo found = scene.Intersect(ray, threadId);
                            List<ColorIntensity> shades = shadings[scrIdx];
                            if (!(found.HitDist == -1))
                            {
                                newshading = Shade(found.GetReal(ray), ray, threadId);
                                shades.Add(newshading);
                            }
                            else
                            {
                                newshading.R = 0.0;
                                newshading.G = 0.0;
                                newshading.B = 0.0;
                                shades.Add(newshading);
                            }
                            if (tries > 0)
                            {
                                ColorIntensity before = Colorav(shades, shades.Count - 1);
                                ColorIntensity after = Colorav(shades, shades.Count);
                                double total = 0.0;
                                total += Math.Abs(before.R - after.R);
                                total += Math.Abs(before.G - after.G);
                                total += Math.Abs(before.B - after.B);
                                if (total < AAco)
                                {
                                    successive[scrIdx]++;
                                }
                                else
                                    successive[scrIdx] = 0;
                                if (successive[scrIdx] >= AAcomr)
                                    localCompleted = completed.Incriment();
                            }
                            if (!antialias)
                                localCompleted = completed.Incriment();
                            Color final = Stochav(shades);
                            screen[scrIdx] = final;
                        }
                        scrIdx++;
                    }
                    if (OnProgress != null)
                    {
                        OnProgress(scrpix, y + 1, localCompleted);
                    }
                }
                tries++;
            }
        }
#if DEBUG
        public bool Gathering = false;

        public GatherInfo CurrentGather;

        public GatherInfo Gather(int x, int y)
        {
            Gathering = true;
            CurrentGather = new GatherInfo();
            Line ray = viewport.RayThrough(x , y);
            SetIn(ref ray.Start, 0);
            HitInfo found = scene.Intersect(ray, 0);
            if (!(found.HitDist == -1))
            {
                Shade(found.GetReal(ray), ray, 0);
            }
            Gathering = false;
            return CurrentGather;
        }
        public void Project(Point pos, out int x, out int y)
        {
            viewport.Project(pos, out x, out y);
        }
#endif
        private void GeneratePhotonsWrap(object input)
        {
            PhotonThreadArgs args = (PhotonThreadArgs)input;
            GeneratePhotons(args.threadId, args.threadCount);
        }

        private void GeneratePhotons(int threadId, int threadCount)
        {
            Random rnd = new Random();
            if (!photons)
                return;
            foreach (Light light in Lights)
            {
                if (light.LightType == LightType.Point)
                {
                    for (int i = 0; i < PhotonCount/threadCount; i++)
                    {   
                        double z = rnd.NextDouble()*2-1;
                        double theta = rnd.NextDouble()*2.0*Math.PI;
                        double r = Math.Sqrt(1-z*z);
                        double x = Math.Cos(theta)*r;
                        double y = Math.Sin(theta)*r;
                        Line ray = new Line();
                        ray.Direct = new Vector(x, y, z);
                        ray.Start = light.Direction.Start;
                        SetIn(ref ray.Start, threadId);
                        HitInfo hit = scene.Intersect(ray, threadId);
                        ColorIntensity lightCol = new ColorIntensity();
                        lightCol.R = light.Color.R;
                        lightCol.G = light.Color.G;
                        lightCol.B = light.Color.B;
                        if (hit.HitDist != -1)
                        {
                            PhotonShade(hit, ray, threadId, lightCol, rnd, 0.0, Math.PI*4);
                        }
                        if (i % 100000 == 0)
                            GC.Collect();
                    }
                }
                else if (light.LightType == LightType.Directional)
                {
                    Point centre;
                    double radius;
                    scene.Bounds.GetSphere(out centre, out radius);
                    Point eye = centre.MoveBy(light.Direction.Direct.Scale(-radius * 2));
                    Vector a;
                    Vector b;
                    GetPerp(light.Direction.Direct.Scale(1.0/light.Direction.Direct.Length), out a, out b);
                    for (int i = 0; i < PhotonCount/threadCount; i++)
                    {
                        double x = 0.0;
                        double y= 0.0;
                        do
                        {
                            x = rnd.NextDouble() * 2 - 1;
                            y = rnd.NextDouble() * 2 - 1;
                        } while (x * x + y * y > 1.0);
                        x = x * radius;
                        y = y * radius;
                        Line ray = new Line();
                        ray.Direct = light.Direction.Direct;
                        ray.Start = eye.MoveBy(a.Scale(x/a.Length)).MoveBy(b.Scale(y/b.Length));
                        SetIn(ref ray.Start, threadId);
                        HitInfo hit = scene.Intersect(ray, threadId);
                        ColorIntensity lightCol = new ColorIntensity();
                        lightCol.R = light.Color.R;
                        lightCol.G = light.Color.G;
                        lightCol.B = light.Color.B;
                        if (hit.HitDist != -1)
                        {
                            PhotonShade(hit, ray, threadId, lightCol, rnd, double.PositiveInfinity, Math.PI*radius*radius);
                        }
                        if (i % 100000 == 0)
                            GC.Collect();
                    }
                }
            }
        }

        private void PhotonShade(HitInfo hit, Line ray, int threadId, ColorIntensity lightCol, Random rnd, double distance, double area)
        {
            PhotonShade(hit, ray, threadId, 0, false, lightCol, rnd, distance, area);
        }

        private void PhotonShade(HitInfo hit, Line ray, int threadId, int depth, bool inside, ColorIntensity lightCol, Random rnd, double distance, double area)
        {
            if (depth > maxrecurse)
                return;
            if (lightCol.GreyScale() < minratio)
                return;
            RealHitInfo realHit = hit.GetReal(ray);
            Material surf = realHit.HitStuff;
            ColorIntensity pigment = realHit.Pigment.GetTexture(realHit.Normal, 0);
            double dist = realHit.Normal.Start.LineTo(ray.Start).Length;
            distance += dist;
            if (surf.Attenutive && inside)
            {
                lightCol.R *= Math.Exp(-(1.0 - surf.Attenuation[0]) * dist / surf.AttenuationDistance);
                lightCol.G *= Math.Exp(-(1.0 - surf.Attenuation[1]) * dist / surf.AttenuationDistance);
                lightCol.B *= Math.Exp(-(1.0 - surf.Attenuation[2]) * dist / surf.AttenuationDistance);
            }
#if DEBUG
            Photon currentParent = PhotonParents[threadId];
#endif
            if ((depth > 0 || allLightsArePhotons))
            {
                Photon photon = new Photon();
                photon.HitPos.X = (float)realHit.Normal.Start.X;
                photon.HitPos.Y = (float)realHit.Normal.Start.Y;
                photon.HitPos.Z = (float)realHit.Normal.Start.Z;
                photon.TravelDir.Dx = (float)ray.Direct.Dx;
                photon.TravelDir.Dy = (float)ray.Direct.Dy;
                photon.TravelDir.Dz = (float)ray.Direct.Dz;
                ColorIntensity photonColor = lightCol;
                if (realLighting || distance == double.PositiveInfinity)
                {
                    photonColor.R *= area / PhotonCount;
                    photonColor.G *= area / PhotonCount;
                    photonColor.B *= area / PhotonCount;
                }
                else
                {
                    photonColor.R *= distance * distance * area / PhotonCount;
                    photonColor.G *= distance * distance * area / PhotonCount;
                    photonColor.B *= distance * distance * area / PhotonCount;
                }
                photon.PhotonColorPower.R = (float)photonColor.R;
                photon.PhotonColorPower.G = (float)photonColor.G;
                photon.PhotonColorPower.B = (float)photonColor.B;
#if DEBUG
                photon.parent = currentParent;
                PhotonParents[threadId] = photon;
#endif
                Map.AddPhoton(photon, threadId);
            }
            ColorIntensity reflectance = new ColorIntensity();
            ColorIntensity transmitance = new ColorIntensity();
            Line refractRay = new Line();
            Line reflectRay = new Line();
            if (surf.Refractive || surf.Reflective) 
            {
                ray.Direct = ray.Direct.Scale(1 / ray.Direct.Length);
                reflectance.R = surf.Reflective ? surf.Reflectance[0] : 0.0;
                reflectance.G = surf.Reflective ? surf.Reflectance[1] : 0.0;
                reflectance.B = surf.Reflective ? surf.Reflectance[2] : 0.0;
                if (surf.Refractive)
                {
                    double ni = inside ? surf.RefractIndex : 1.0;
                    double nt = (!inside) ? surf.RefractIndex : 1.0;
                    double cratio = ni / nt;
                    double ct1 = -ray.Direct.Dot(realHit.Normal.Direct);
                    double ct2sqrd = 1 - cratio * cratio * (1 - ct1 * ct1);
                    if (ct2sqrd <= 0)
                    {
                        reflectance.R = 1;
                        reflectance.G = 1;
                        reflectance.B = 1;
                    }
                    else
                    {
                        double ct2 = Math.Sqrt(ct2sqrd);
                        // fresnel equations for reflectance perp and parallel.
                        double rperp = (ni * ct1 - nt * ct2) / (ni * ct1 + nt * ct2);
                        double rpll = (nt * ct1 - ni * ct2) / (ni * ct2 + nt + ct1);
                        // assume unpolarised light always - better then tracing 2 
                        // rays for both sides of every interface.
                        double reflectanceval = (rperp * rperp + rpll * rpll) / 2;
                        reflectance.R = Math.Min(1.0, reflectance.R + reflectanceval);
                        reflectance.G = Math.Min(1.0, reflectance.G + reflectanceval);
                        reflectance.B = Math.Min(1.0, reflectance.B + reflectanceval);
                        transmitance.R = 1 - reflectance.R;
                        transmitance.G = 1 - reflectance.G;
                        transmitance.B = 1 - reflectance.B;
                        refractRay.Direct = ray.Direct.Scale(cratio);
                        refractRay.Direct.Add(realHit.Normal.Direct.Scale(cratio * (ct1) - ct2));
                        refractRay.Start = realHit.Normal.Start.MoveBy(refractRay.Direct.Scale(EPSILON * 10));
                    }
                }
                reflectRay.Direct = new Vector(ray.Direct.Dx, ray.Direct.Dy, ray.Direct.Dz);
                reflectRay.Direct.Add(realHit.Normal.Direct.Scale(-2 * ray.Direct.Dot(realHit.Normal.Direct)));
                reflectRay.Start = realHit.Normal.Start.MoveBy(reflectRay.Direct.Scale(EPSILON * 10));
            }
            bool doReflect = false;
            bool doRefract = false;
            bool doDifuse = false;
            double avr = reflectance.R + reflectance.G + reflectance.B;
            avr /= 3;
            double avt = transmitance.R + transmitance.G + transmitance.B;
            avt /= 3;
            double reflectWeight = Math.Max(avr, 0.0);
            double refractWeight = Math.Max(avt, 0.0);
            double diffuseWeight = Math.Max(pigment.GreyScale() * surf.Diffuse, 0.0);
            double specularity = 1.0;
            double diffusivity = 1.0;
            if (surf.Refractive)
            {
                specularity = surf.Specularity;
                diffusivity = 1.0 - specularity;
                reflectWeight *= surf.Specularity;
                refractWeight *= surf.Specularity;
                diffuseWeight *= 1.0-surf.Specularity;
            }
            double choice = rnd.NextDouble();
            if (choice < diffuseWeight)
                doDifuse = true;
            else if (choice < diffuseWeight+reflectWeight)
                doReflect = true;
            else if (choice < diffuseWeight+reflectWeight+refractWeight)
                doRefract = true;
            if (doRefract && avt > minratio)
            {
                SetIn(ref refractRay.Start, threadId);
                HitInfo hitnew = scene.Intersect(refractRay, threadId);
                if (!(hitnew.HitDist == -1))
                {
                    ColorIntensity lightCol2 = new ColorIntensity();
                    lightCol2.R = lightCol.R * transmitance.R *specularity/ refractWeight;
                    lightCol2.G = lightCol.G * transmitance.G * specularity / refractWeight;
                    lightCol2.B = lightCol.B * transmitance.B * specularity / refractWeight;
                    PhotonShade(hitnew, refractRay, threadId, depth + 1, !inside, lightCol2, rnd, distance, area);
                }
            }
            if (doReflect && avr > minratio)
            {
                SetIn(ref reflectRay.Start, threadId);
                HitInfo hitnew2 = scene.Intersect(reflectRay, threadId);
                if (!(hitnew2.HitDist == -1))
                {
                    ColorIntensity lightCol2 = new ColorIntensity();
                    lightCol2.R = lightCol.R * reflectance.R * specularity / reflectWeight;
                    lightCol2.G = lightCol.G * reflectance.G * specularity / reflectWeight;
                    lightCol2.B = lightCol.B * reflectance.B * specularity / reflectWeight;
                    PhotonShade(hitnew2, reflectRay, threadId, depth + 1, inside, lightCol2, rnd, distance, area);
                }
            }
            if (doDifuse)
            {
                double z = rnd.NextDouble();
                double theta = rnd.NextDouble() * 2.0 * Math.PI;
                double r = Math.Sqrt(1 - z * z);
                double x = Math.Cos(theta) * r;
                double y = Math.Sin(theta) * r;

                Line diffuseRay = new Line();
                Vector a;
                Vector b;
                Vector basis = realHit.Normal.Direct.Scale(1.0/realHit.Normal.Direct.Length);
                GetPerp(basis, out a, out b);
                diffuseRay.Direct = basis.Scale(z);
                diffuseRay.Direct.Add(a.Scale(x/a.Length));
                diffuseRay.Direct.Add(b.Scale(y/b.Length));
                diffuseRay.Start = realHit.Normal.Start.MoveBy(diffuseRay.Direct.Scale(EPSILON * 10));
                SetIn(ref diffuseRay.Start, threadId);
                HitInfo hitnew2 = scene.Intersect(diffuseRay, threadId);
                if (!(hitnew2.HitDist == -1))
                {
                    ColorIntensity lightCol2 = new ColorIntensity();
                    lightCol2.R = lightCol.R * surf.Diffuse * pigment.R *diffusivity/ diffuseWeight;
                    lightCol2.G = lightCol.G * surf.Diffuse * pigment.G * diffusivity / diffuseWeight;
                    lightCol2.B = lightCol.B * surf.Diffuse * pigment.B * diffusivity / diffuseWeight;
                    PhotonShade(hitnew2, diffuseRay, threadId, depth + 1, inside, lightCol2, rnd, distance, area);
                }
            }
#if DEBUG
            PhotonParents[threadId] = currentParent;
#endif
        }

        private void GetPerp(Vector vector, out Vector a, out Vector b)
        {
            Vector basis;
            if (Math.Abs(vector.Dz) < 0.9)
            {
                basis = new Vector(0, 0, 1);
            }
            else
            {
                basis = new Vector(1, 0, 0);
            }
            a = vector.Cross(basis);
            b = a.Cross(vector);
        }


        public Primative Scene
        {
            get
            {
                return scene;
            }
            set
            {
                scene = value;
            }
        }
        private Primative scene;


        public List<Light> Lights
        {
            get
            {
                return lights;
            }
        }
        private List<Light> lights = new List<Light>();


        public View Viewport
        {
            get
            {
                return viewport;
            }
            set
            {
                viewport = value;
            }
        }
        private View viewport;


        public int MaxRecurse
        {
            get
            {
                return maxrecurse;
            }
            set
            {
                maxrecurse = value;
            }
        }
        private int maxrecurse;


        public double RecurseMinRatio
        {
            get
            {
                return minratio;
            }
            set
            {
                minratio = value;
            }
        }
        private double minratio;

        
        public int AntiAliasLevel
        {
            get
            {
                return AAn;
            }
            set
            {
                AAn = value;
            }
        }
        private int AAn;


        public double AntiAliasCutOut
        {
            get
            {
                return AAco;
            }
            set
            {
                AAco = value;
            }
        }
        private double AAco;
        public int AntiAliasCutOutMinRepetition
        {
            get
            {
                return AAcomr;
            }
            set
            {
                AAcomr = value;
            }
        }
        private int AAcomr;


        public bool AntiAlias
        {
            get
            {
                return antialias;
            }
            set
            {
                antialias = value;
            }
        }
        bool antialias;

        public bool Photons
        {
            get
            {
                return photons;
            }
            set
            {
                photons = value;
            }
        }
        private bool photons;

        private double lambert2(Vector light, Vector normal, double brilliance)
        {
            return Math.Pow(Math.Abs(light.Dot(normal) / light.Length), brilliance);
        }

        private ColorIntensity Shade(RealHitInfo hit, Line by, int recurse, double ratio, bool inside, int threadId)
        {
#if DEBUG
            if (Gathering)
            {
                CurrentGather.Weight = ratio;
                CurrentGather.RealInfo = hit;
            }
#endif
            ColorIntensity colors, shadowcolors;
            Material surf = hit.HitStuff;
            ColorIntensity pigment = hit.Pigment.GetTexture(hit.Normal, 0);
            ColorIntensity atten = new ColorIntensity();
            if (surf.Attenutive && inside)
            {
                double dist = hit.Normal.Start.LineTo(by.Start).Length;
                atten.R = Math.Exp(-(1.0 - surf.Attenuation[0]) * dist / surf.AttenuationDistance);
                atten.G = Math.Exp(-(1.0 - surf.Attenuation[1]) * dist / surf.AttenuationDistance);
                atten.B = Math.Exp(-(1.0 - surf.Attenuation[2]) * dist / surf.AttenuationDistance);
                ratio = ratio * (atten.R + atten.G + atten.B) / 3;
            }
            double rI = 0;
            double gI = 0;
            double bI = 0;
            Vector lightdir;
            Line newray = new Line();
            HitInfo hitnewray = new HitInfo();
            hit.Normal.Direct = hit.Normal.Direct.Scale(1 / hit.Normal.Direct.Length);
            Vector eye = by.Direct.Scale(-1 / by.Direct.Length);

            int lightCount = (allLightsArePhotons && photons) ? 0 : lights.Count;
            //lightCount = 0;
            for (int i = 0; i < lightCount; i++)
            {
                double lightdist = double.PositiveInfinity;
                switch (lights[i].LightType)
                {
                    case LightType.Ambient:
                        rI += lights[i].Color.R * surf.Ambient[0];
                        gI += lights[i].Color.G * surf.Ambient[1];
                        bI += lights[i].Color.B * surf.Ambient[2];
                        continue;
                    case LightType.Directional:
                        lightdir = lights[i].Direction.Direct.Scale(-1);
                        break;
                    case LightType.Point:
                        lightdir = hit.Normal.Start.LineTo(lights[i].Direction.Start);
                        lightdist = lightdir.Length;
                        break;
                    default:
                        continue;
                }
                // Add double illuminate flag or something to turn this off.
                if (true)
                {
                    if (lightdir.Dot(hit.Normal.Direct) <= 0.0)
                        continue;
                }
                newray.Direct = lightdir.Scale(1.0/lightdir.Length);
                if (newray.Direct.Dot(hit.Normal.Direct) >= 0)
                    newray.Start = hit.Normal.Start.MoveBy(newray.Direct.Scale(EPSILON * 10));
                else
                    newray.Start = hit.Normal.Start.MoveBy(newray.Direct.Scale(-EPSILON * 10));
                SetIn(ref newray.Start, threadId);
                HitInfo info = scene.Intersect(newray, false, threadId);
                if (!(info.HitDist == -1 || info.HitDist > lightdist))
                    continue;
                shadowcolors.R = 1.0;
                shadowcolors.G = 1.0;
                shadowcolors.B = 1.0;
                // Don't need to setIn again, seperate intersect cache for shadow vs non-shadow rays.
                hitnewray = scene.Intersect(newray, threadId);
                if (!photons)
                {
                    if (!(hitnewray.HitDist == -1 || hitnewray.HitDist > lightdist))
                    {
#if DEBUG
                        GatherInfo last = null;
                        if (Gathering)
                        {
                            last = CurrentGather;
                            CurrentGather = new GatherInfo();
                        }
#endif
                        shadowcolors = ShadowShade(hitnewray.GetReal(newray), newray, recurse + 1, ratio, inside, threadId, lightdist);
#if DEBUG
                        if (Gathering)
                        {
                            last.LightChildren.Add(CurrentGather);
                            CurrentGather = last;
                        }
#endif
                    }
                }
                else
                {
                    if (!(hitnewray.HitDist == -1 || hitnewray.HitDist > lightdist))
                        continue;
                }
                shadowcolors.R *= lights[i].Color.R;
                shadowcolors.G *= lights[i].Color.G;
                shadowcolors.B *= lights[i].Color.B;
                if (realLighting && lightdist < double.PositiveInfinity)
                {
                    double lightdistSq = lightdist * lightdist;
                    shadowcolors.R /= lightdistSq;
                    shadowcolors.G /= lightdistSq;
                    shadowcolors.B /= lightdistSq;
                }
                CalculateLightContrib(hit.Normal.Direct, shadowcolors, surf, pigment, ref rI, ref gI, ref bI, lightdir, eye);
            }
            if (photons)
            {
                double scaleRadSq;
                List<Photon> closePhotons = Map.GetClosest(hit.Normal.Start, hit.Normal.Direct, out scaleRadSq, threadId);
                double photoR = 0.0;
                double photoG = 0.0;
                double photoB = 0.0;
                foreach (Photon photon in closePhotons)
                {
                    lightdir.Dx = -photon.TravelDir.Dx;
                    lightdir.Dy = -photon.TravelDir.Dy;
                    lightdir.Dz = -photon.TravelDir.Dz;
                    if (true)
                    {
                        if (lightdir.Dot(hit.Normal.Direct) <= 0.0)
                            continue;
                    }
                    ColorIntensity photonColor;
                    photonColor.R = photon.PhotonColorPower.R;
                    photonColor.G = photon.PhotonColorPower.G;
                    photonColor.B = photon.PhotonColorPower.B;
                    CalculateLightContrib(hit.Normal.Direct, photonColor, surf, pigment, ref photoR, ref photoG, ref photoB, lightdir, eye);
#if DEBUG
                    if (Gathering)
                    {
                        CurrentGather.GatheredPhotons.Add(photon);
                    }
#endif
                }
                if (closePhotons.Count > 0)
                {
                    rI += photoR / Math.PI / scaleRadSq;
                    if (rI > 3)
                    {
                        int breakpoint = 3;
                        breakpoint *= 34;
                    }
                    gI += photoG / Math.PI / scaleRadSq;
                    if (gI > 3)
                    {
                        int breakpoint = 3;
                        breakpoint *= 34;
                    }
                    bI += photoB / Math.PI / scaleRadSq;
                    if (bI > 3)
                    {
                        int breakpoint = 3;
                        breakpoint *= 34;
                    }
                }
            }
            if ((surf.Refractive || surf.Reflective) && recurse < maxrecurse)
            {
                by.Direct = by.Direct.Scale(1 / by.Direct.Length);
                ColorIntensity reflectance = new ColorIntensity();
                reflectance.R = surf.Reflective ? surf.Reflectance[0] : 0.0;
                reflectance.G = surf.Reflective ? surf.Reflectance[1] : 0.0;
                reflectance.B = surf.Reflective ? surf.Reflectance[2] : 0.0;
                if (surf.Refractive)
                {
                    double ni = inside ? surf.RefractIndex : 1.0;
                    double nt = (!inside) ? surf.RefractIndex : 1.0;
                    double cratio = ni / nt;
                    double ct1 = -by.Direct.Dot(hit.Normal.Direct);
                    double ct2sqrd = 1 - cratio * cratio * (1 - ct1 * ct1);
                    if (ct2sqrd <= 0)
                    {
                        reflectance.R = 1;
                        reflectance.G = 1;
                        reflectance.B = 1;
                    }
                    else
                    {
                        double ct2 = Math.Sqrt(ct2sqrd);
                        // fresnel equations for reflectance perp and parallel.
                        double rperp = (ni * ct1 - nt * ct2) / (ni * ct1 + nt * ct2);
                        double rpll = (nt * ct1 - ni * ct2) / (ni * ct2 + nt + ct1);
                        // assume unpolarised light always - better then tracing 2 
                        // rays for both sides of every interface.
                        double reflectanceval = (rperp * rperp + rpll * rpll) / 2;
                        reflectance.R = Math.Min(1.0, reflectance.R + reflectanceval);
                        reflectance.G = Math.Min(1.0, reflectance.G + reflectanceval);
                        reflectance.B = Math.Min(1.0, reflectance.B + reflectanceval);
                        ColorIntensity transmitance = new ColorIntensity();
                        transmitance.R = 1 - reflectance.R;
                        transmitance.G = 1 - reflectance.G;
                        transmitance.B = 1 - reflectance.B;
                        double avt = transmitance.R + transmitance.G + transmitance.B;
                        avt /= 3;
                        if (avt * ratio*surf.Specularity > minratio)
                        {
                            Line newray2 = new Line();
                            newray2.Direct = by.Direct.Scale(cratio);
                            newray2.Direct.Add(hit.Normal.Direct.Scale(cratio * (ct1) - ct2));
                            newray2.Start = hit.Normal.Start.MoveBy(newray2.Direct.Scale(EPSILON * 10));
                            SetIn(ref newray2.Start, threadId);
                            HitInfo hitnew = scene.Intersect(newray2, threadId);
                            if (!(hitnew.HitDist == -1))
                            {
#if DEBUG
                                GatherInfo last2 = null;
                                if (Gathering)
                                {
                                    last2 = CurrentGather;
                                    CurrentGather = new GatherInfo();
                                }
#endif
                                ColorIntensity cl = Shade(hitnew.GetReal(newray2), newray2, recurse + 1, avt * ratio * surf.Specularity, !inside, threadId);
#if DEBUG
                                if (Gathering)
                                {
                                    last2.Children.Add(CurrentGather);
                                    CurrentGather = last2;
                                }
#endif
                                rI = rI + ((double)transmitance.R * cl.R) * surf.Specularity;
                                gI = gI + ((double)transmitance.G * cl.G) * surf.Specularity;
                                bI = bI + ((double)transmitance.B * cl.B) * surf.Specularity;
                            }
                        }
                    }
                }
                double avr = reflectance.R + reflectance.G + reflectance.B;
                avr /= 3;
                double specularity = surf.Refractive ? surf.Specularity : 1.0;
                if (avr * ratio*specularity > minratio)
                {
                    Line newray2 = new Line();
                    newray2.Direct = new Vector(by.Direct.Dx, by.Direct.Dy, by.Direct.Dz);
                    newray2.Direct.Add(hit.Normal.Direct.Scale(-2 * by.Direct.Dot(hit.Normal.Direct)));
                    newray2.Start = hit.Normal.Start.MoveBy(newray2.Direct.Scale(EPSILON * 10));
                    SetIn(ref newray2.Start, threadId);
                    HitInfo hitnew2 = scene.Intersect(newray2, threadId);
                    if (!(hitnew2.HitDist == -1))
                    {
#if DEBUG
                        GatherInfo last3 = null;
                        if (Gathering)
                        {
                            last3 = CurrentGather;
                            CurrentGather = new GatherInfo();
                        }
#endif
                        ColorIntensity cl2 = Shade(hitnew2.GetReal(newray2), newray2, recurse + 1, avr * ratio * specularity, inside, threadId);
#if DEBUG
                        if (Gathering)
                        {
                            last3.Children.Add(CurrentGather);
                            CurrentGather = last3;
                        }
#endif
                        rI = rI + ((double)reflectance.R * cl2.R) * specularity;
                        gI = gI + ((double)reflectance.G * cl2.G) * specularity;
                        bI = bI + ((double)reflectance.B * cl2.B) * specularity;
                    }
                }
            }
            else
            {
                if (recurse >= maxrecurse)
                {
                    // Recurse limit reached.
                    int i = 0;
                    i = i * 3;
                }
            }
            if (surf.Attenutive && inside)
            {
                rI *= atten.R;
                gI *= atten.G;
                bI *= atten.B;
            }
            colors.R = rI;
            colors.G = gI;
            colors.B = bI;
            return colors;
        }

        private void CalculateLightContrib(Vector normal, ColorIntensity lightColor, Material surf, ColorIntensity pigment, ref double rI, ref double gI, ref double bI, Vector lightdir, Vector eye)
        {
            double diffusivity = 1.0-surf.Specularity;
            if (surf.Phong > 0.0)
            {
                Vector reflection = eye;
                reflection.Add(normal.Scale(-2.0 * eye.Dot(normal)));
                double phong = -lightdir.Scale(1.0 / lightdir.Length).Dot(reflection);
                if (phong > 0.0)
                {
                    double phongpowd = Math.Pow(phong, surf.Exponent);
                    rI += surf.Phong * phongpowd * lightColor.R*diffusivity;
                    gI += surf.Phong * phongpowd * lightColor.G * diffusivity;
                    bI += surf.Phong * phongpowd * lightColor.B * diffusivity;
                }
            }
            if (surf.Specular > 0.0)
            {
                Vector half = lightdir.Scale(1.0 / lightdir.Length);
                half.Add(eye);
                half.ScaleSelf(0.5);
                double spec = half.Dot(normal) / half.Length;

                if (spec > 0.0)
                {
                    double specpowd = Math.Pow(spec, 1.0 / surf.Roughness);
                    rI += surf.Specular * specpowd * lightColor.R * diffusivity;
                    gI += surf.Specular * specpowd * lightColor.G * diffusivity;
                    bI += surf.Specular * specpowd * lightColor.B * diffusivity;
                }
            }
            if (surf.Iridescence > 0.0)
            {
                double cosaoi = lightdir.Dot(normal)/lightdir.Length;
                double interference = 4.0 * Math.PI * surf.FilmThickness * cosaoi;

                double intensity = cosaoi * surf.Iridescence;
                double rwl = 0.25;
                double gwl = 0.18;
                double bwl = 0.14;
                /* Modify color by phase offset for each wavelength. */

                rI += surf.Iridescence * (intensity * (1.0 - 0.5 * Math.Cos(interference / rwl))) * diffusivity;
                gI += surf.Iridescence * (intensity * (1.0 - 0.5 * Math.Cos(interference / gwl))) * diffusivity;
                bI += surf.Iridescence * (intensity * (1.0 - 0.5 * Math.Cos(interference / bwl))) * diffusivity;

            }
            double lam = lambert2(lightdir, normal, surf.Brilliance);
            rI += surf.Diffuse * pigment.R * lam * lightColor.R * diffusivity;
            gI += surf.Diffuse * pigment.G * lam * lightColor.G * diffusivity;
            bI += surf.Diffuse * pigment.B * lam * lightColor.B * diffusivity;
        }


        private ColorIntensity Shade(RealHitInfo hit, Line by, int threadId)
        {
            return Shade(hit, by, 0, 1.0, false, threadId);
        }


        private ColorIntensity ShadowShade(RealHitInfo hit, Line by, int recurse, double ratio, bool inside, int threadId, double lightdist)
        {
#if DEBUG
            if (Gathering)
            {
                CurrentGather.Weight = ratio;
                CurrentGather.RealInfo = hit;
            }
#endif
            ColorIntensity colors;
            Material surf = hit.HitStuff;
            double[] atten = new double[3];
            double rI, gI, bI;
            double dist = hit.Normal.Start.LineTo(by.Start).Length;
            lightdist -= dist;
            bool stop = lightdist < 0;
            if (lightdist < 0)
                dist += lightdist;
            if (surf.Attenutive && inside)
            {
                atten[0] = Math.Exp(-(1.0 - surf.Attenuation[0]) * dist / surf.AttenuationDistance);
                atten[1] = Math.Exp(-(1.0 - surf.Attenuation[1]) * dist / surf.AttenuationDistance);
                atten[2] = Math.Exp(-(1.0 - surf.Attenuation[2]) * dist / surf.AttenuationDistance);
                ratio = ratio * (atten[0] + atten[1] + atten[2]) / 3;
            }

            if (recurse < maxrecurse && ratio > minratio)
            {
                if (!stop)
                { 
                    Line newray = new Line();
                    newray.Direct = new Vector(by.Direct.Dx, by.Direct.Dy, by.Direct.Dz);
                    newray.Start = hit.Normal.Start.MoveBy(newray.Direct.Scale(EPSILON * 10));
                    SetIn(ref newray.Start, threadId);
                    HitInfo hitnew = scene.Intersect(newray, threadId);
                    if (!(hitnew.HitDist == -1))
                    {
#if DEBUG
                        GatherInfo last = null;
                        if (Gathering)
                        {
                            last = CurrentGather;
                            CurrentGather = new GatherInfo();
                        }
#endif
                        ColorIntensity cl = ShadowShade(hitnew.GetReal(newray), newray, recurse + 1, ratio, !inside, threadId, lightdist);
#if DEBUG
                        if (Gathering)
                        {
                            last.LightChildren.Add(CurrentGather);
                            CurrentGather = last;
                        }
#endif
                        rI = cl.R;
                        gI = cl.G;
                        bI = cl.B;
                    }
                    else
                    {
                        rI = 1.0;
                        gI = 1.0;
                        bI = 1.0;
                    }
                }
                else
                {
                    rI = 1.0;
                    gI = 1.0;
                    bI = 1.0;
                }
            }
            else
            {
                if (recurse >= maxrecurse)
                {
                    // Recurse limit reached.
                    int i = 0;
                    i = i * 3;
                }
                rI = 0;
                bI = 0;
                gI = 0;
            }
            if (surf.Attenutive && inside)
            {
                rI *= atten[0];
                gI *= atten[1];
                bI *= atten[2];
            }
            if (!stop)
            {
                // Correct for how much light would be diffused at each interface.
                rI *= surf.Specularity;
                gI *= surf.Specularity;
                bI *= surf.Specularity;
                // Correct for reflection.
                double minFresnelReflectence = (surf.RefractIndex - 1) / (surf.RefractIndex + 1);
                minFresnelReflectence *= minFresnelReflectence;
                rI *= 1.0 - surf.Reflectance[0] - minFresnelReflectence;
                gI *= 1.0 - surf.Reflectance[1] - minFresnelReflectence;
                bI *= 1.0 - surf.Reflectance[2] - minFresnelReflectence;
            }
            colors.R = rI;
            colors.G = gI;
            colors.B = bI;
            return colors;

        }


        private ColorIntensity ShadowShade(RealHitInfo hit, Line by, int threadId, double lightdist)
        {
            return ShadowShade(hit, by, 0, 1.0, false, threadId, lightdist);
        }


        private Color Stochav(List<ColorIntensity> colors)
        {
            ColorIntensity sumer;
            sumer = Colorav(colors, colors.Count);
            Color res = new Color();
            res.ScR = (float)sumer.R;
            res.ScG = (float)sumer.G;
            res.ScB = (float)sumer.B;
            return res;
        }

        private ColorIntensity Colorav(List<ColorIntensity> colors, int limit)
        {
            ColorIntensity sumer;
            sumer.R = 0.0;
            sumer.G = 0.0;
            sumer.B = 0.0;
            for (int i = 0; i < limit; i++)
            {
                sumer.R += colors[i].R;
                sumer.G += colors[i].G;
                sumer.B += colors[i].B;
            }
            sumer.R /= limit;
            sumer.G /= limit;
            sumer.B /= limit;
            return sumer;
        }


        private bool Decide(byte[] screen, int screenwidth, ColorIntensity current)
        {
            // Todo: new decide function.
            return false;
        }


        private void SetIn(ref Point inPoint, int threadId)
        {
            raycount[threadId]++;
            inPoint.Tag = raycount[threadId];
        }
        private int[] raycount;

    }
#if DEBUG
    public class GatherInfo
    {
        public double Weight;
        public RealHitInfo RealInfo;
        public List<Photon> GatheredPhotons = new List<Photon>();
        public List<GatherInfo> LightChildren = new List<GatherInfo>();
        public List<GatherInfo> Children = new List<GatherInfo>();
    }
#endif
}
