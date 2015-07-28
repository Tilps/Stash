using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary.Primatives
{
    public class Box : Primative
    {
        Matrix trans = new Matrix();
        Matrix inv = new Matrix();

        public Box()
        {
            Shadow = true;
            Render = true;
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
            bool temp = Math.Abs(test.X) <= 1 + ToyTracer.EPSILON && Math.Abs(test.Y) <= 1 + ToyTracer.EPSILON && Math.Abs(test.Z) <= 1 + ToyTracer.EPSILON;
            return temp;
        }

        /// <summary>
        /// Gets whether a point is in, for transformed points only.
        /// </summary>
        /// <param name="inP">
        /// Point to test.
        /// </param>
        /// <returns>
        /// True if the transformed point is inside the box.
        /// </returns>
        protected bool In2(Point inP)
        {
            return Math.Abs(inP.X) <= 1 + ToyTracer.EPSILON && Math.Abs(inP.Y) <= 1 + ToyTracer.EPSILON && Math.Abs(inP.Z) <= 1 + ToyTracer.EPSILON;
        }

        protected override HitInfo GetIntersect(Line lineIn, bool renderray, int threadId)
        {
            HitInfo ret = new HitInfo();
            ret.HitDist = -1;
            if (renderray && !Render || !renderray && !Shadow)
                return ret;
            Line transray = inv.Apply(lineIn);
            ret.Hit = this;
            double closest = -1;
            double t = (1 - transray.Start.X) / transray.Direct.Dx;
            if
          ((closest == -1 || t < closest) && t >= 0 && In2(transray.Project(t)))
            {
                closest = t;
                ret.SurfaceIndex = 0;
            }
            t = (-1 - transray.Start.X) / transray.Direct.Dx;
            if
          ((closest == -1 || t < closest) && t >= 0 && In2(transray.Project(t)))
            {
                closest = t;
                ret.SurfaceIndex = 1;
            }
            t = (1 - transray.Start.Y) / transray.Direct.Dy;
            if
          ((closest == -1 || t < closest) && t >= 0 && In2(transray.Project(t)))
            {
                closest = t;
                ret.SurfaceIndex = 2;
            }
            t = (-1 - transray.Start.Y) / transray.Direct.Dy;
            if
          ((closest == -1 || t < closest) && t >= 0 && In2(transray.Project(t)))
            {
                closest = t;
                ret.SurfaceIndex = 3;
            }
            t = (1 - transray.Start.Z) / transray.Direct.Dz;
            if
          ((closest == -1 || t < closest) && t >= 0 && In2(transray.Project(t)))
            {
                closest = t;
                ret.SurfaceIndex = 4;

            }
            t = (-1 - transray.Start.Z) / transray.Direct.Dz;
            if
          ((closest == -1 || t < closest) && t >= 0 && In2(transray.Project(t)))
            {
                closest = t;
                ret.SurfaceIndex = 5;
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
            double t = (1 - transray.Start.X) / transray.Direct.Dx;
            if (t >= 0 && In2(transray.Project(t)))
            {
                HitInfo inr;
                inr.Hit = this;
                inr.SurfaceIndex = 0;
                inr.HitDist = t;
                ret.Add(inr);
            }
            t = (-1 - transray.Start.X) / transray.Direct.Dx;
            if (t >= 0 && In2(transray.Project(t)))
            {
                HitInfo inr;
                inr.Hit = this;
                inr.SurfaceIndex = 1;
                inr.HitDist = t;
                ret.Add(inr);
            }
            t = (1 - transray.Start.Y) / transray.Direct.Dy;
            if (t >= 0 && In2(transray.Project(t)))
            {
                HitInfo inr;
                inr.Hit = this;
                inr.SurfaceIndex = 2;
                inr.HitDist = t;
                ret.Add(inr);
            }
            t = (-1 - transray.Start.Y) / transray.Direct.Dy;
            if (t >= 0 && In2(transray.Project(t)))
            {
                HitInfo inr;
                inr.Hit = this;
                inr.SurfaceIndex = 3;
                inr.HitDist = t;
                ret.Add(inr);
            }
            t = (1 - transray.Start.Z) / transray.Direct.Dz;
            if (t >= 0 && In2(transray.Project(t)))
            {
                HitInfo inr;
                inr.Hit = this;
                inr.SurfaceIndex = 4;
                inr.HitDist = t;
                ret.Add(inr);
            }
            t = (-1 - transray.Start.Z) / transray.Direct.Dz;
            if (t >= 0 && In2(transray.Project(t)))
            {
                HitInfo inr;
                inr.Hit = this;
                inr.SurfaceIndex = 5;
                inr.HitDist = t;
                ret.Add(inr);
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
            realHit.Normal = new Line();
            realHit.HitStuff = Surface;
            realHit.Pigment = Pigment;
            realHit.Normal.Start = by.Project(hit.HitDist);
            realHit.Normal.Direct.Dx = 0;
            realHit.Normal.Direct.Dy = 0;
            realHit.Normal.Direct.Dz = 0;
            Point hitLoc;
            Vector deviance = new Vector();
            if (rounded)
            {
                hitLoc = inv.Apply(realHit.Normal.Start);
                if (hit.SurfaceIndex != 5 || !RoundedTopOnly)
                {
                    if (hitLoc.X < -1 + XRounding)
                    {
                        switch (hit.SurfaceIndex)
                        {
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                deviance.Dx = -1 + (hitLoc.X + 1) / XRounding;
                                break;
                        }
                    }
                    if (hitLoc.X > 1 - XRounding)
                    {
                        switch (hit.SurfaceIndex)
                        {
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                                deviance.Dx = 1 - (1 - hitLoc.X) / XRounding;
                                break;
                        }
                    }
                    if (hitLoc.Y < -1 + YRounding)
                    {
                        switch (hit.SurfaceIndex)
                        {
                            case 0:
                            case 1:
                            case 4:
                            case 5:
                                deviance.Dy = -1 + (hitLoc.Y + 1) / YRounding;
                                break;
                        }
                    }
                    if (hitLoc.Y > 1 - YRounding)
                    {
                        switch (hit.SurfaceIndex)
                        {
                            case 0:
                            case 1:
                            case 4:
                            case 5:
                                deviance.Dy = 1 - (1 - hitLoc.Y) / YRounding;
                                break;
                        }
                    }
                }
                if (!RoundedTopOnly && hitLoc.Z < -1 + ZRounding)
                {
                    switch (hit.SurfaceIndex)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            deviance.Dz = -1 +(hitLoc.Z + 1) / ZRounding;
                            break;
                    }
                }
                if (hitLoc.Z > 1 - ZRounding)
                {
                    switch (hit.SurfaceIndex)
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            deviance.Dz = 1 - (1 - hitLoc.Z) / ZRounding;
                            break;
                    }
                }
            }
            switch (hit.SurfaceIndex)
            {
                case 0:
                    realHit.Normal.Direct.Dx = 1;
                    break;
                case 1:
                    realHit.Normal.Direct.Dx = -1;
                    break;
                case 2:
                    realHit.Normal.Direct.Dy = 1;
                    break;
                case 3:
                    realHit.Normal.Direct.Dy = -1;
                    break;
                case 4:
                    realHit.Normal.Direct.Dz = 1;
                    break;
                case 5:
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

        public double XRounding = 0.0;
        public double YRounding = 0.0;
        public double ZRounding = 0.0;
    }
}
