using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary.Bricks
{
    // A primative which is itself just containing a single primative which was built during the constructor for the descendent types.
    public abstract class Construct : Primative
    {
        protected Primative Parts
        {
            get
            {
                return parts;
            }
            set
            {
                parts = value;
            }
        }
        Primative parts;

        public Construct()
        {
        }

        protected override bool GetIn(Point inP, int threadId)
        {
            if (!Bounds.InTest(inP, threadId))
                return false;
            bool temp = parts.In(inP, threadId);
            return temp;
        }

        public override Material Surface
        {
            get
            {
                return base.Surface;
            }
            set
            {
                parts.Surface = value;
                base.Surface = value;
            }
        }

        public override Texture Pigment
        {
            get
            {
                return base.Pigment;
            }
            set
            {
                parts.Pigment = value;
                base.Pigment = value;
            }
        }

        protected override HitInfo GetIntersect(Line lineIn, bool renderray, int threadId)
        {
            HitInfo ret = new HitInfo();
            ret.HitDist = -1;
            if (renderray && !Render || !renderray && !Shadow)
                return ret;
            return parts.Intersect(lineIn, renderray, threadId);
        }

        protected override List<HitInfo> GetAllIntersect(Line lineIn, bool renderray, int threadId)
        {
            List<HitInfo> ret = Owner.Pools[threadId].Get();
            if (renderray && !Render || !renderray && !Shadow)
            {
                return ret;
            }
            ret.AddRange(parts.AllIntersect(lineIn, renderray, threadId));
            return ret;
        }

        protected override void SetTracerChildren(ToyTracer tt, int threadCount)
        {
            parts.SetTracer(tt, threadCount);
        }

        public override void ApplyMatrix(Matrix trans, Matrix invTrans)
        {
            parts.ApplyMatrix(trans, invTrans);
        }
    }
}
