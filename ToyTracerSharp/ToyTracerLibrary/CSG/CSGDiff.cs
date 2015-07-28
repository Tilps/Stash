using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary.CSG
{
    public class CSGDiff : Primative
    {
        Primative primA;
        Primative primB;

        public CSGDiff(Primative inA, Primative inB)
        {
            Render = (inA.Render || inB.Render);
            Shadow = (inA.Shadow || inB.Shadow);
            primA = inA;
            primB = inB;
            Bounds = new AllignedBox(primA.Bounds);
        }

        protected override bool GetIn(Point inP, int threadId)
        {
            if (!Bounds.InTest(inP, threadId))
                return false;
            return primA.In(inP, threadId) && !primB.In(inP, threadId);
        }

        public override Material Surface
        {
            get
            {
                return base.Surface;
            }
            set
            {
                primA.Surface = value;
                primB.Surface = value;
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
                primA.Pigment = value;
                primB.Pigment = value;
                base.Pigment = value;
            }
        }

        protected override HitInfo GetIntersect(Line lineIn, bool renderray, int threadId)
        {
            HitInfo ret = new HitInfo();
            ret.HitDist = -1;
            if (renderray && !Render || !renderray && !Shadow)
                return ret;
            bool r1 = primA.Bounds.HitTest(lineIn, renderray, threadId);
            if (!r1)
                return ret;
            bool r2 = primB.Bounds.HitTest(lineIn, renderray, threadId);
            if (!r2)
                return primA.Intersect(lineIn, renderray, threadId);
            List<HitInfo> list = primA.AllIntersect(lineIn, renderray, threadId);
            if (list.Count == 0)
                return ret;
            List<HitInfo> other = primB.AllIntersect(lineIn, renderray, threadId);
            if (other.Count == 0)
            {
                return list[0];
            }
            int i = 0;
            int j = 0;
            bool insideflag1;
            bool insideflag2;
            insideflag1 = primA.In(lineIn.Start, threadId);
            insideflag2 = primB.In(lineIn.Start, threadId);
            bool done1 = list.Count == 0;
            bool done2 = other.Count == 0;
            int imax = list.Count;
            int jmax = other.Count;
            while (!done1 || !done2)
            {
                if (!done1 && !done2)
                {
                    if (list[i].EarlierThan(other[j]))
                    {
                        if (!insideflag2)
                            return list[i];
                        i++;
                        insideflag1 = !insideflag1;
                        if (i >= imax) done1 = true;
                    }
                    else
                    {
                        if (insideflag1)
                        {
                            return other[j];
                        }
                        j++;
                        insideflag2 = !insideflag2;
                        if (j >= jmax) done2 = true;
                    }
                }
                else if (!done1)
                {
                    if (!insideflag2)
                        return list[i];
                    break;
                }
                else
                {
                    if (insideflag1)
                        return other[j];
                    break;
                }
            }
            return ret;
        }

        protected override List<HitInfo> GetAllIntersect(Line lineIn, bool renderray, int threadId)
        {
            List<HitInfo> list = null;
            if (!Bounds.HitTest(lineIn, renderray, threadId))
            {
                list = Owner.Pools[threadId].Get();
                return list;
            }
            list = primA.AllIntersect(lineIn, renderray, threadId);
            List<HitInfo> res = Owner.Pools[threadId].Get();
            if (list.Count == 0)
                return res;
            List<HitInfo> other;
            other = primB.AllIntersect(lineIn, renderray, threadId);
            if (other.Count == 0)
            {
                res.AddRange(list);
                return res;
            }
            int i = 0;
            int j = 0;
            bool insideflag1;
            bool insideflag2;
            insideflag1 = primA.In(lineIn.Start, threadId);
            insideflag2 = primB.In(lineIn.Start, threadId);
            bool done1 = false;
            bool done2 = false;
            int imax = list.Count;
            int jmax = other.Count;
            while (!done1 || !done2)
            {
                if (!done1 && !done2)
                {
                    if (list[i].EarlierThan(other[j]))
                    {
                        if (!insideflag2)
                            res.Add(list[i]);
                        i++;
                        insideflag1 = !insideflag1;
                        if (i >= imax) done1 = true;
                    }
                    else
                    {
                        if (insideflag1)
                        {
                            res.Add(other[j]);
                        }
                        j++;
                        insideflag2 = !insideflag2;
                        if (j >= jmax) done2 = true;
                    }
                }
                else if (!done1)
                {
                    if (!insideflag2)
                        for (; i < imax; i++)
                            res.Add(list[i]);
                    break;
                }
                else
                {
                    if (insideflag1)
                    {
                        for (; j < jmax; j++)
                            res.Add(other[j]);
                    }
                    break;
                }
            }
            return res;
        }
        protected override void SetTracerChildren(ToyTracer tt, int threadCount)
        {
            primA.SetTracer(tt, threadCount);
            primB.SetTracer(tt, threadCount);
        }

        public override void ApplyMatrix(Matrix trans, Matrix invTrans)
        {
            primA.ApplyMatrix(trans, invTrans);
            primB.ApplyMatrix(trans, invTrans);
            Bounds.ApplyMatrix(trans, invTrans);
        }
    }
}
