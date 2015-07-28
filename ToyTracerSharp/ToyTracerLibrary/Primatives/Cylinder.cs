using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary.Primatives
{
    public class Cylinder : Primative
    {
        Matrix trans = new Matrix();
        Matrix inv = new Matrix();

        public Cylinder()
        {
            Render = true;
            Shadow = true;
            trans.MakeTrans(0, 0, 0);
            inv.MakeTrans(0, 0, 0);
            Point origin = new Point(0, 0, 0);
            Bounds = new AllignedBox(origin, 2, 2, 2);
        }

        protected override bool GetIn(Point inP, int threadId)
        {
            if (!Bounds.InTest(inP, threadId))
                return false;
            Point test = inv.Apply(inP);
            bool temp = test.X * test.X + test.Y * test.Y <= 1 + ToyTracer.EPSILON && Math.Abs(test.Z) <= 1 + ToyTracer.EPSILON;
            return temp;
        }

        protected override HitInfo GetIntersect(Line lineIn, bool renderray, int threadId)
        {
            HitInfo ret = new HitInfo();
            ret.Hit = this;
            ret.HitDist = -1;
            if (renderray && !Render || !renderray && !Shadow)
                return ret;
            Line transray = inv.Apply(lineIn);
            double vx = transray.Direct.Dx;
            double vy = transray.Direct.Dy;
            double sx = transray.Start.X;
            double sy = transray.Start.Y;
            double a = vx * vx + vy * vy;
            double b = 2 * (sx * vx + sy * vy);
            double c = sx * sx + sy * sy - 1;
            double descr = b * b - 4 * a * c;
            double t;
            if (descr < 0)
                return ret;
            if (descr == 0)
            {
                t = -b / 2 / a;
                if (t >= 0 && Math.Abs(transray.Start.Z + t * transray.Direct.Dz) <= 1 + ToyTracer.EPSILON)
                {
                    ret.HitDist = t;
                    ret.SurfaceIndex = 0;
                }
                return ret;
            }
            double closest = -1;
            double dessqrt = Math.Sqrt(descr);
            t = (-b + dessqrt) / 2 / a;
            double sz = transray.Start.Z;
            double vz = transray.Direct.Dz;
            if (t >= 0 && Math.Abs(sz + t * vz) <= 1 + ToyTracer.EPSILON)
            {
                closest = t;
                ret.SurfaceIndex = 0;
            }
            t = (-b - dessqrt) / 2 / a;
            if ((closest == -1 || t < closest) && t >= 0 && Math.Abs(sz + t * vz) <= 1 + ToyTracer.EPSILON)
            {
                closest = t;
                ret.SurfaceIndex = 0;
            }
            t = (1 - sz) / vz;
            double ix = sx + t * vx;
            double iy = sy + t * vy;
            if ((closest == -1 || t < closest) && t >= 0 && ix * ix + iy * iy <= 1 + ToyTracer.EPSILON)
            {
                closest = t;
                ret.SurfaceIndex = 1;
            }
            t = (-1 - sz) / vz;
            ix = sx + t * vx;
            iy = sy + t * vy;
            if ((closest == -1 || t < closest) && t >= 0 && ix * ix + iy * iy <= 1 + ToyTracer.EPSILON)
            {
                closest = t;
                ret.SurfaceIndex = 2;
            }
            ret.HitDist = closest;
            return ret;
        }

        protected override List<HitInfo> GetAllIntersect(Line lineIn, bool renderray, int threadId)
        {
            List<HitInfo> ret = Owner.Pools[threadId].Get();
            if (renderray && !Render || !renderray && !Shadow)
                return ret;
            Line transray = inv.Apply(lineIn);
            double vx = transray.Direct.Dx;
            double vy = transray.Direct.Dy;
            double sx = transray.Start.X;
            double sy = transray.Start.Y;
            double a = vx * vx + vy * vy;
            double b = 2 * (sx * vx + sy * vy);
            double c = sx * sx + sy * sy - 1;
            double descr = b * b - 4 * a * c;
            double t;
            if (descr < 0)
                return ret;
            if (descr == 0)
            {
                t = -b / 2 / a;
                if (t >= 0 && Math.Abs(transray.Start.Z + t * transray.Direct.Dz) <= 1 + ToyTracer.EPSILON)
                {
                    HitInfo retp;
                    retp.Hit = this;
                    retp.SurfaceIndex = 0;
                    retp.HitDist = t;
                    ret.Add(retp);
                }
                return ret;
            }
            double dessqrt = Math.Sqrt(descr);
            t = (-b + dessqrt) / 2 / a;
            double sz = transray.Start.Z;
            double vz = transray.Direct.Dz;
            if (t >= 0 && Math.Abs(sz + t * vz) <= 1 + ToyTracer.EPSILON)
            {
                HitInfo retv;
                retv.Hit = this;
                retv.SurfaceIndex = 0;
                retv.HitDist = t;
                ret.Add(retv);
            }
            t = (-b - dessqrt) / 2 / a;
            if (t >= 0 && Math.Abs(sz + t * vz) <= 1 + ToyTracer.EPSILON)
            {
                HitInfo retv;
                retv.Hit = this;
                retv.SurfaceIndex = 0;
                retv.HitDist = t;
                ret.Add(retv);
            }
            t = (1 - sz) / vz;
            double ix = sx + t * vx;
            double iy = sy + t * vy;
            if (t >= 0 && ix * ix + iy * iy <= 1 + ToyTracer.EPSILON)
            {
                HitInfo retv;
                retv.Hit = this;
                retv.SurfaceIndex = 1;
                retv.HitDist = t;
                ret.Add(retv);
            }
            t = (-1 - sz) / vz;
            ix = sx + t * vx;
            iy = sy + t * vy;
            if (t >= 0 && ix * ix + iy * iy <= 1 + ToyTracer.EPSILON)
            {
                HitInfo retv;
                retv.Hit = this;
                retv.SurfaceIndex = 2;
                retv.HitDist = t;
                ret.Add(retv);
            }
            ret.Sort(HitInfoComparer.Instance);
            return ret;
        }

        protected override void SetTracerChildren(ToyTracer tt, int threadCount)
        {
        }

        public override RealHitInfo DelayedHitCalc(Line by, HitInfo hit)
        {
            RealHitInfo realHit = new RealHitInfo();
            realHit.HitStuff = Surface;
            realHit.Pigment = Pigment;
            realHit.Normal = new Line();
            realHit.Normal.Start = by.Project(hit.HitDist);
            realHit.Normal.Direct.Dx = 0;
            realHit.Normal.Direct.Dy = 0;
            realHit.Normal.Direct.Dz = 0;
            Point hitLoc;
            Vector deviance = new Vector();
            if (rounded)
            {
                hitLoc = inv.Apply(realHit.Normal.Start);
                if (hit.SurfaceIndex == 1 || (!RoundedTopOnly && hit.SurfaceIndex == 2))
                {
                    double r = Math.Sqrt(hitLoc.X * hitLoc.X + hitLoc.Y * hitLoc.Y);
                    if (r > 1 - RRounding)
                    {
                        double dr = (1 - (1 - r) / RRounding) / (r);
                        deviance.Dx = hitLoc.X * dr;
                        deviance.Dy = hitLoc.Y * dr;
                    }
                }
                if (!RoundedTopOnly && hitLoc.Z < -1 + ZRounding)
                {
                    switch (hit.SurfaceIndex)
                    {
                        case 0:
                            deviance.Dz = -1 + (hitLoc.Z + 1) / ZRounding;
                            break;
                    }
                }
                if (hitLoc.Z > 1 - ZRounding)
                {
                    switch (hit.SurfaceIndex)
                    {
                        case 0:
                            deviance.Dz = 1 - (1 - hitLoc.Z) / ZRounding;
                            break;
                    }
                }
            } 
            switch (hit.SurfaceIndex)
            {
                case 0:
                    Point hitLoc2 = inv.Apply(realHit.Normal.Start);
                    realHit.Normal.Direct.Dx = hitLoc2.X;
                    realHit.Normal.Direct.Dy = hitLoc2.Y;
                    break;
                case 1:
                    realHit.Normal.Direct.Dz = 1;
                    break;
                case 2:
                    realHit.Normal.Direct.Dz = -1;
                    break;
                default:
                    throw new InvalidOperationException("Invalid surface index in hitdata");
            }
            Vector before = realHit.Normal.Direct;
            realHit.Normal.Direct = trans.Apply(realHit.Normal.Direct);
            if (realHit.Normal.Direct.Dot(by.Direct) > 0)
            {
                if (rounded)
                {
                    before.ScaleSelf(-1.0);
                    deviance.ScaleSelf(-1.0);
                }
                else
                    realHit.Normal.Direct.ScaleSelf(-1.0);
            }
            if (rounded)
            {
                realHit.Normal.Direct = before;
                realHit.Normal.Direct.Add(deviance);
                realHit.Normal.Direct = trans.Apply(realHit.Normal.Direct);
                if (realHit.Normal.Direct.Dot(by.Direct) > 0)
                {
                    Vector perp1 = realHit.Normal.Direct.Cross(by.Direct);
                    realHit.Normal.Direct = by.Direct.Cross(perp1);
                }
           }
            return realHit;
        }

        public override void ApplyMatrix(Matrix transIn, Matrix invTrans)
        {
            trans = transIn.MulBy(trans);
            inv = inv.MulBy(invTrans);
            Bounds.ApplyMatrix(transIn, invTrans);
        }

        public bool Rounded
        {
            get
            {
                return rounded;
            }
            set
            {
                rounded = value;
            }
        }
        private bool rounded = false;

        public bool RoundedTopOnly = false;

        public double RRounding = 0.0;
        public double ZRounding = 0.0;

    }
}
