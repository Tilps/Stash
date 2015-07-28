using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary.Primatives
{
    public class Sphere : Primative
    {
        Matrix trans = new Matrix();
        Matrix inv = new Matrix();

        public Sphere()
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
            bool temp = test.X * test.X + test.Y * test.Y +test.Z*test.Z <= 1 + ToyTracer.EPSILON;
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
            double vz = transray.Direct.Dz;
            double sx = transray.Start.X;
            double sy = transray.Start.Y;
            double sz = transray.Start.Z;
            double a = vx * vx + vy * vy + vz * vz;
            double b = 2 * (sx * vx + sy * vy + sz * vz);
            double c = sx * sx + sy * sy + sz * sz - 1;
            double descr = b * b - 4 * a * c;
            double t;
            if (descr < 0)
                return ret;
            if (descr == 0)
            {
                t = -b / 2 / a;
                if (t >= 0)
                {
                    ret.HitDist = t;
                    ret.SurfaceIndex = 0;
                }
                return ret;
            }
            double closest = -1;
            double dessqrt = Math.Sqrt(descr);
            t = (-b + dessqrt) / 2 / a;
            if (t >= 0)
            {
                closest = t;
                ret.SurfaceIndex = 0;
            }
            t = (-b - dessqrt) / 2 / a;
            if (t >= 0 && (closest == -1 || t < closest))
            {
                closest = t;
                ret.SurfaceIndex = 0;
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
            double vz = transray.Direct.Dz;
            double sx = transray.Start.X;
            double sy = transray.Start.Y;
            double sz = transray.Start.Z;
            double a = vx * vx + vy * vy + vz * vz;
            double b = 2 * (sx * vx + sy * vy + sz * vz);
            double c = sx * sx + sy * sy + sz * sz - 1;
            double descr = b * b - 4 * a * c;
            double t;
            if (descr < 0)
                return ret;
            if (descr == 0)
            {
                t = -b / 2 / a;
                if (t >= 0)
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
            if (t >= 0)
            {
                HitInfo retv;
                retv.Hit = this;
                retv.SurfaceIndex = 0;
                retv.HitDist = t;
                ret.Add(retv);
            }
            t = (-b - dessqrt) / 2 / a;
            if (t >= 0)
            {
                HitInfo retv;
                retv.Hit = this;
                retv.SurfaceIndex = 0;
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
            switch (hit.SurfaceIndex)
            {
                case 0:
                    Point hitLoc2 = inv.Apply(realHit.Normal.Start);
                    realHit.Normal.Direct.Dx = hitLoc2.X;
                    realHit.Normal.Direct.Dy = hitLoc2.Y;
                    realHit.Normal.Direct.Dz = hitLoc2.Z;
                    break;
                default:
                    throw new InvalidOperationException("Invalid surface index in hitdata");
            }
            Vector before = realHit.Normal.Direct;
            realHit.Normal.Direct = trans.Apply(realHit.Normal.Direct);
            if (realHit.Normal.Direct.Dot(by.Direct) > 0)
            {
                realHit.Normal.Direct.ScaleSelf(-1.0);
            }
            return realHit;
        }

        public override void ApplyMatrix(Matrix transIn, Matrix invTrans)
        {
            trans = transIn.MulBy(trans);
            inv = inv.MulBy(invTrans);
            Bounds.ApplyMatrix(transIn, invTrans);
        }

    }
}
