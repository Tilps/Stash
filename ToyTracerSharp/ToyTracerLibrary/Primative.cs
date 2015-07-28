using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{
    public abstract class Primative
    {
        virtual public Material Surface
        {
            get
            {
                return surface;
            }
            set
            {
                surface = value;
            }
        }
        Material surface;

        virtual public Texture Pigment
        {
            get
            {
                return pigment;
            }
            set
            {
                pigment = value;
            }
        }
        private Texture pigment;

        public bool Render;

        public bool Shadow;

        public AllignedBox Bounds;

        public bool In(Point other, int threadId)
        {
            if (other.Tag != inCacheTag[threadId])
            {
                inCacheTag[threadId] = other.Tag;
                inCache[threadId] = GetIn(other, threadId);
            }
            return inCache[threadId];
        }
        protected abstract bool GetIn(Point other, int threadId);

        public HitInfo Intersect(Line lineIn, int threadId)
        {
            return Intersect(lineIn, true, threadId);
        }
        public HitInfo Intersect(Line lineIn, bool renderray, int threadId)
        {
            if (renderray)
            {
                if (lineIn.Start.Tag != intersectCacheTag[threadId])
                {
                    intersectCacheTag[threadId] = lineIn.Start.Tag;
                    intersectCache[threadId] = GetIntersect(lineIn, renderray, threadId);
                }
                return intersectCache[threadId];
            }
            else
            {
                if (lineIn.Start.Tag != intersectCacheTagShadow[threadId])
                {
                    intersectCacheTagShadow[threadId] = lineIn.Start.Tag;
                    intersectCacheShadow[threadId] = GetIntersect(lineIn, renderray, threadId);
                }
                return intersectCacheShadow[threadId];
            }
        }
        protected abstract HitInfo GetIntersect(Line lineIn, bool renderray, int threadId);

        public List<HitInfo> AllIntersect(Line lineIn, int threadId)
        {
            return AllIntersect(lineIn, true, threadId);
        }
        public List<HitInfo> AllIntersect(Line lineIn, bool renderray, int threadId)
        {
            if (renderray)
            {
                if (lineIn.Start.Tag != allIntersectCacheTag[threadId])
                {
                    allIntersectCacheTag[threadId] = lineIn.Start.Tag;
                    if (allIntersectCache[threadId] != null)
                        Owner.Pools[threadId].Add(allIntersectCache[threadId]);
                    allIntersectCache[threadId] = GetAllIntersect(lineIn, renderray, threadId);
                }
                return allIntersectCache[threadId];
            }
            else
            {
                if (lineIn.Start.Tag != allIntersectCacheTagShadow[threadId])
                {
                    allIntersectCacheTagShadow[threadId] = lineIn.Start.Tag;
                    if (allIntersectCacheShadow[threadId] != null)
                        Owner.Pools[threadId].Add(allIntersectCacheShadow[threadId]);
                    allIntersectCacheShadow[threadId] = GetAllIntersect(lineIn, renderray, threadId);
                }
                return allIntersectCacheShadow[threadId];
            }
        }
        protected abstract List<HitInfo> GetAllIntersect(Line lineIn, bool renderray, int threadId);

        public abstract void ApplyMatrix(Matrix trans, Matrix invTrans);

        protected ToyTracer Owner
        {
            get
            {
                return owner;
            }
        }
        private ToyTracer owner;

        private int[] inCacheTag;

        private bool[] inCache;

        private int[] intersectCacheTag;

        private HitInfo[] intersectCache;

        private int[] allIntersectCacheTag;

        private List<HitInfo>[] allIntersectCache;

        private int[] intersectCacheTagShadow;

        private HitInfo[] intersectCacheShadow;

        private int[] allIntersectCacheTagShadow;

        private List<HitInfo>[] allIntersectCacheShadow;

        public void SetTracer(ToyTracer tt, int threadCount)
        {
            SetTracerChildren(tt, threadCount);
            owner = tt;
            inCacheTag = new int[threadCount];
            inCache = new bool[threadCount];
            intersectCacheTag = new int[threadCount];
            intersectCache = new HitInfo[threadCount];
            allIntersectCacheTag = new int[threadCount];
            allIntersectCache = new List<HitInfo>[threadCount];
            intersectCacheTagShadow = new int[threadCount];
            intersectCacheShadow = new HitInfo[threadCount];
            allIntersectCacheTagShadow = new int[threadCount];
            allIntersectCacheShadow = new List<HitInfo>[threadCount];
            if (Bounds != null && Bounds != this)
                Bounds.SetTracer(tt, threadCount);

        }

        protected abstract void SetTracerChildren(ToyTracer tt, int threadCount);

        public virtual RealHitInfo DelayedHitCalc(Line by, HitInfo hit)
        {
            throw new InvalidOperationException("Shouldn't have called DelayedHitCalc on this since it doesnt think its a base primative.");
        }

    }

    public class AllignedBox : Primative
    {
        public Point Centre
        {
            get
            {
                return centre;
            }
        }
        Point centre;

        public double SurfaceArea
        {
            get
            {
                return 2.0 * (width * height + height * depth + width * depth);
            }
        }
        double width, height, depth;
        double width2e, height2e, depth2e;
        double xmin, xmax, ymin, ymax, zmin, zmax;

        public AllignedBox(AllignedBox other)
            : this(new Point(other.centre.X, other.centre.Y, other.centre.Z), other.width, other.height, other.depth)
        {
        }

        public AllignedBox(Point centreIn, double widthIn, double heightIn, double depthIn)
        {
            centre = centreIn;
            width = widthIn;
            height = heightIn;
            depth = depthIn;
            width2e = width / 2 + ToyTracer.EPSILON;
            height2e = height / 2 + ToyTracer.EPSILON;
            depth2e = depth / 2 + ToyTracer.EPSILON;
            xmin = centre.X - width / 2;
            ymin = centre.Y - height / 2;
            zmin = centre.Z - depth / 2;
            xmax = centre.X + width / 2;
            ymax = centre.Y + height / 2;
            zmax = centre.Z + depth / 2;
            Render = true;
            Shadow = true;
        }

        public void Increase(AllignedBox stretchby)
        {
            double newx1 = Math.Min(centre.X - width / 2, stretchby.centre.X - stretchby.width / 2);
            double newx2 = Math.Max(centre.X + width / 2, stretchby.centre.X + stretchby.width / 2);
            double newy1 = Math.Min(centre.Y - height / 2, stretchby.centre.Y - stretchby.height / 2);
            double newy2 = Math.Max(centre.Y + height / 2, stretchby.centre.Y + stretchby.height / 2);
            double newz1 = Math.Min(centre.Z - depth / 2, stretchby.centre.Z - stretchby.depth / 2);
            double newz2 = Math.Max(centre.Z + depth / 2, stretchby.centre.Z + stretchby.depth / 2);
            centre.X = (newx2 + newx1) / 2;
            centre.Y = (newy2 + newy1) / 2;
            centre.Z = (newz2 + newz1) / 2;
            width = newx2 - newx1;
            height = newy2 - newy1;
            depth = newz2 - newz1;
            width2e = width / 2 + ToyTracer.EPSILON;
            height2e = height / 2 + ToyTracer.EPSILON;
            depth2e = depth / 2 + ToyTracer.EPSILON;
            xmin = centre.X - width / 2;
            ymin = centre.Y - height / 2;
            zmin = centre.Z - depth / 2;
            xmax = centre.X + width / 2;
            ymax = centre.Y + height / 2;
            zmax = centre.Z + depth / 2;
        }

        public AllignedBox Intersect(AllignedBox other)
        {
            double newx1 = Math.Max(centre.X - width / 2, other.centre.X - other.width / 2);
            double newx2 = Math.Min(centre.X + width / 2, other.centre.X + other.width / 2);
            double newy1 = Math.Max(centre.Y - height / 2, other.centre.Y - other.height / 2);
            double newy2 = Math.Min(centre.Y + height / 2, other.centre.Y + other.height / 2);
            double newz1 = Math.Max(centre.Z - depth / 2, other.centre.Z - other.depth / 2);
            double newz2 = Math.Min(centre.Z + depth / 2, other.centre.Z + other.depth / 2);
            Point newcentre = new Point();
            newcentre.X = (newx2 + newx1) / 2;
            newcentre.Y = (newy2 + newy1) / 2;
            newcentre.Z = (newz2 + newz1) / 2;
            double newwidth = newx2 - newx1;
            double newheight = newy2 - newy1;
            double newdepth = newz2 - newz1;
            AllignedBox boxret = new AllignedBox(newcentre, newwidth, newheight, newdepth);
            return boxret;
        }

        public AllignedBox UnionWith(AllignedBox other)
        {
            double newx1 = Math.Min(centre.X - width / 2, other.centre.X - other.width / 2);
            double newx2 = Math.Max(centre.X + width / 2, other.centre.X + other.width / 2);
            double newy1 = Math.Min(centre.Y - height / 2, other.centre.Y - other.height / 2);
            double newy2 = Math.Max(centre.Y + height / 2, other.centre.Y + other.height / 2);
            double newz1 = Math.Min(centre.Z - depth / 2, other.centre.Z - other.depth / 2);
            double newz2 = Math.Max(centre.Z + depth / 2, other.centre.Z + other.depth / 2);
            Point newcentre = new Point();
            newcentre.X = (newx2 + newx1) / 2;
            newcentre.Y = (newy2 + newy1) / 2;
            newcentre.Z = (newz2 + newz1) / 2;
            double newwidth = newx2 - newx1;
            double newheight = newy2 - newy1;
            double newdepth = newz2 - newz1;
            AllignedBox boxret = new AllignedBox(newcentre, newwidth, newheight, newdepth);
            return boxret;
        }

        public bool Disjoint(AllignedBox other)
        {
            double widthav = (width + other.width) / 2;
            if (Math.Abs(centre.X - other.centre.X) > widthav)
                return true;
            double heightav = (height + other.height) / 2;
            if (Math.Abs(centre.Y - other.centre.Y) > heightav)
                return true;
            double depthav = (depth + other.depth) / 2;
            if (Math.Abs(centre.Z - other.centre.Z) > widthav)
                return true;
            return false;
        }

        public bool InTest(Point other, int threadId)
        {
            bool temp = (Math.Abs(centre.X - other.X) <= width2e) && (Math.Abs(centre.Y - other.Y) <= height2e) && (Math.Abs(centre.Z - other.Z) <= depth2e);
            return temp;
        }

        protected override bool GetIn(Point other, int threadId)
        {
            bool temp = (Math.Abs(centre.X - other.X) <= width2e) && (Math.Abs(centre.Y - other.Y) <= height2e) && (Math.Abs(centre.Z - other.Z) <= depth2e);
            return temp;
        }

        protected override HitInfo GetIntersect(Line lineIn, bool renderray, int threadId)
        {
            HitInfo ret = new HitInfo();
            ret.HitDist = -1;
            ret.Hit = this;
            if (renderray && !Render || !renderray && !Shadow)
                return ret;
            double xd = lineIn.Direct.Dx;
            double yd = lineIn.Direct.Dy;
            double zd = lineIn.Direct.Dz;
            if (InTest(lineIn.Start, threadId))
            {
                xd = -xd;
                yd = -yd;
                zd = -zd;
            }
            if (xd < 0)
                if (yd < 0)
                    if (zd < 0)
                    {
                        double t = (xmax - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 0;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (ymax - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 2;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (zmax - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                ret.SurfaceIndex = 4;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                    }
                    else
                    { //pos z
                        double t = (xmax - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 0;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (ymax - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 2;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (zmin - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                ret.SurfaceIndex = 5;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                    }
                else
                    if (zd < 0)
                    {
                        double t = (xmax - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 0;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (ymin - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 3;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (zmax - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                ret.SurfaceIndex = 4;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                    }
                    else
                    {
                        double t = (xmax - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 0;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (ymin - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 3;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (zmin - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                ret.SurfaceIndex = 5;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                    }
            else
                if (yd < 0)
                    if (zd < 0)
                    {
                        double t = (xmin - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 1;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (ymax - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 2;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (zmax - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                ret.SurfaceIndex = 4;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                    }
                    else
                    {
                        double t = (xmin - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 1;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (ymax - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 2;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (zmin - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                ret.SurfaceIndex = 5;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                    }
                else
                    if (zd < 0)
                    {
                        double t = (xmin - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 1;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (ymin - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 3;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (zmax - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                ret.SurfaceIndex = 4;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                    }
                    else
                    {
                        double t = (xmin - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 1;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (ymin - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                ret.SurfaceIndex = 3;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                        t = (zmin - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                ret.SurfaceIndex = 5;
                                ret.HitDist = t;
                                return ret;
                            }
                        }
                    }
            return ret;

        }

        public bool HitTest(Line lineIn, int threadId)
        {
            return HitTest(lineIn, true, threadId);
        }

        public bool HitTest(Line lineIn, bool renderray, int threadId)
        {
            if (renderray && !Render || !renderray && !Shadow)
                return false;
            if (InTest(lineIn.Start, threadId))
            {
                return true;
            }
            double xd = lineIn.Direct.Dx;
            double yd = lineIn.Direct.Dy;
            double zd = lineIn.Direct.Dz;
            if (xd < 0)
            {
                if (yd < 0)
                {
                    if (zd < 0)
                    {
                        double t = (xmax - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (ymax - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (zmax - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    { //pos z
                        double t = (xmax - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (ymax - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (zmin - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (zd < 0)
                    {
                        double t = (xmax - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (ymin - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (zmax - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        double t = (xmax - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (ymin - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (zmin - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (yd < 0)
                {
                    if (zd < 0)
                    {
                        double t = (xmin - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (ymax - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (zmax - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        double t = (xmin - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (ymax - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (zmin - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    if (zd < 0)
                    {
                        double t = (xmin - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (ymin - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (zmax - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        double t = (xmin - lineIn.Start.X) / lineIn.Direct.Dx;
                        Point temp = new Point();
                        if (t >= 0)
                        {
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (ymin - lineIn.Start.Y) / lineIn.Direct.Dy;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Z = lineIn.Start.Z + lineIn.Direct.Dz * t;
                            if (Math.Abs(centre.X - temp.X) <= width2e && Math.Abs(centre.Z - temp.Z) <= depth2e)
                            {
                                return true;
                            }
                        }
                        t = (zmin - lineIn.Start.Z) / lineIn.Direct.Dz;
                        if (t >= 0)
                        {
                            temp.X = lineIn.Start.X + lineIn.Direct.Dx * t;
                            temp.Y = lineIn.Start.Y + lineIn.Direct.Dy * t;
                            if (Math.Abs(centre.Y - temp.Y) <= height2e && Math.Abs(centre.X - temp.X) <= width2e)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        protected override List<HitInfo> GetAllIntersect(Line lineIn, bool renderray, int threadId)
        {
            throw new NotImplementedException("This should not have been called so we did not implement it.");
        }

        protected override void SetTracerChildren(ToyTracer tt, int threadCount)
        {
        }

        public override void ApplyMatrix(Matrix trans, Matrix invTrans)
        {
            Point[] c = new Point[8];
            c[0] = new Point(centre.X - width / 2, centre.Y - height / 2, centre.Z - depth / 2);
            c[1] = new Point(centre.X + width / 2, centre.Y - height / 2, centre.Z - depth / 2);
            c[2] = new Point(centre.X - width / 2, centre.Y + height / 2, centre.Z - depth / 2);
            c[3] = new Point(centre.X + width / 2, centre.Y + height / 2, centre.Z - depth / 2);
            c[4] = new Point(centre.X - width / 2, centre.Y - height / 2, centre.Z + depth / 2);
            c[5] = new Point(centre.X + width / 2, centre.Y - height / 2, centre.Z + depth / 2);
            c[6] = new Point(centre.X - width / 2, centre.Y + height / 2, centre.Z + depth / 2);
            c[7] = new Point(centre.X + width / 2, centre.Y + height / 2, centre.Z + depth / 2);
            for (int i = 0; i < 8; i++)
                c[i] = trans.Apply(c[i]);
            double xmin = c[0].X;
            double xmax = c[0].X;
            double ymin = c[0].Y;
            double ymax = c[0].Y;
            double zmin = c[0].Z;
            double zmax = c[0].Z;
            for (int i = 1; i < 8; i++)
            {
                if (c[i].X < xmin)
                    xmin = c[i].X;
                if (c[i].X > xmax)
                    xmax = c[i].X;
                if (c[i].Y < ymin)
                    ymin = c[i].Y;
                if (c[i].Y > ymax)
                    ymax = c[i].Y;
                if (c[i].Z < zmin)
                    zmin = c[i].Z;
                if (c[i].Z > zmax)
                    zmax = c[i].Z;
            }
            centre.X = (xmax + xmin) / 2;
            centre.Y = (ymax + ymin) / 2;
            centre.Z = (zmax + zmin) / 2;
            width = (xmax - xmin);
            height = (ymax - ymin);
            depth = (zmax - zmin);
            width2e = width / 2 + ToyTracer.EPSILON;
            height2e = height / 2 + ToyTracer.EPSILON;
            depth2e = depth / 2 + ToyTracer.EPSILON;
            this.xmin = centre.X - width / 2;
            this.ymin = centre.Y - height / 2;
            this.zmin = centre.Z - depth / 2;
            this.xmax = centre.X + width / 2;
            this.ymax = centre.Y + height / 2;
            this.zmax = centre.Z + depth / 2;
        }


        internal void GetSphere(out Point centre, out double radius)
        {
            centre = this.centre;
            radius = Math.Sqrt(this.width2e * this.width2e + this.height2e * this.height2e + this.depth2e * this.depth2e);
        }
    }
}
