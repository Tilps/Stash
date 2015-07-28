using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary.CSG
{
    public class CSGUnion : Primative
    {
        Primative primA;
        Primative primB;
        bool disjoint;

        public CSGUnion(Primative inA, Primative inB)
            : this(inA, inB, false)
        {
        }

        public CSGUnion(Primative inA, Primative inB, bool disjoint)
        {
            Render = (inA.Render || inB.Render);
            Shadow = (inA.Shadow || inB.Shadow);
            primA = inA;
            primB = inB;
            Bounds = primA.Bounds.UnionWith(primB.Bounds);
            this.disjoint = disjoint;
        }

        protected override bool GetIn(Point inP, int threadId)
        {
            if (!Bounds.InTest(inP, threadId))
                return false;
            if (disjoint)
            {
                return primA.In(inP, threadId) ^ primB.In(inP, threadId);
            }
            return primA.In(inP, threadId) || primB.In(inP, threadId);
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
            if (renderray && !Render || !renderray && !Shadow)
            {
                HitInfo ret = new HitInfo();
                ret.HitDist = -1;
                return ret;
            }

            bool inA = primA.Bounds.In(lineIn.Start, threadId);
            bool inB = primB.Bounds.In(lineIn.Start, threadId);
            bool in_check = inA || inB;
            if (in_check)
            {
                bool r1 = !inA && !primA.Bounds.HitTest(lineIn, renderray, threadId);
                bool r2 = !inB && !primB.Bounds.HitTest(lineIn, renderray, threadId);
                if (r1)
                    return primB.Intersect(lineIn, renderray, threadId);
                if (r2)
                    return primA.Intersect(lineIn, renderray, threadId);

                if (disjoint)
                {
                    HitInfo realInters1 = primA.Intersect(lineIn, renderray, threadId);
                    if (realInters1.HitDist == -1)
                        return primB.Intersect(lineIn, renderray, threadId);
                    HitInfo realInters2 = primB.Intersect(lineIn, renderray, threadId);
                    if (realInters2.HitDist == -1)
                        return realInters1;
                    return realInters1.HitDist < realInters2.HitDist ? realInters1 : realInters2;
                }
            }
            else
            {
                HitInfo inters1 = primA.Bounds.Intersect(lineIn, renderray, threadId);
                HitInfo inters2 = primB.Bounds.Intersect(lineIn, renderray, threadId);
                bool r1 = inters1.HitDist == -1;
                bool r2 = inters2.HitDist == -1;
                if (r1 && r2)
                {
                    HitInfo ret = new HitInfo();
                    ret.HitDist = -1;
                    return ret;
                }
                if (r1)
                    return primB.Intersect(lineIn, renderray, threadId);
                if (r2)
                    return primA.Intersect(lineIn, renderray, threadId);

                double t1 = inters1.HitDist;
                double t2 = inters2.HitDist;
                if (t1 < t2)
                {
                    HitInfo realInters1 = primA.Intersect(lineIn, renderray, threadId);
                    if (realInters1.HitDist == -1)
                        return primB.Intersect(lineIn, renderray, threadId);
                    else if (realInters1.HitDist < t2)
                        return realInters1;
                    HitInfo realInters2 = primB.Intersect(lineIn, renderray, threadId);
                    if (realInters2.HitDist == -1)
                        return realInters1;
                    return realInters1.HitDist < realInters2.HitDist ? realInters1 : realInters2;
                }
                else
                {
                    HitInfo realInters1 = primB.Intersect(lineIn, renderray, threadId);
                    if (realInters1.HitDist == -1)
                        return primA.Intersect(lineIn, renderray, threadId);
                    else if (realInters1.HitDist < t1)
                        return realInters1;
                    HitInfo realInters2 = primA.Intersect(lineIn, renderray, threadId);
                    if (realInters2.HitDist == -1)
                        return realInters1;
                    return realInters1.HitDist < realInters2.HitDist ? realInters1 : realInters2;
                }
            }
            // Since we are starting on risky optimisations, lets try the real in test now, this should avoid allintersect call for a very large number of rays.
            // All intersect will probably call this anyway.
            inA = inA && primA.In(lineIn.Start, threadId);
            inB = inB && primB.In(lineIn.Start, threadId);
            if (!inA && !inB)
            {
                HitInfo realInters1 = primA.Intersect(lineIn, renderray, threadId);
                if (realInters1.HitDist == -1)
                    return primB.Intersect(lineIn, renderray, threadId);
                HitInfo realInters2 = primB.Intersect(lineIn, renderray, threadId);
                if (realInters2.HitDist == -1)
                    return realInters1;
                return realInters1.HitDist < realInters2.HitDist ? realInters1 : realInters2;
            }
            // 'risky' optimisations here 
            // there is potential for work duplication in this process.
            // since intersect will be called on both - and then call
            // all intersect if it failed to shortcut
            if (inA && !inB)
            {
                HitInfo realInters1 = primB.Intersect(lineIn, renderray, threadId);
                if (realInters1.HitDist == -1)
                    return primA.Intersect(lineIn, renderray, threadId);
                HitInfo realInters2 = primA.Intersect(lineIn, renderray, threadId);
                // this next condition should never be true now.
                if (realInters2.HitDist == -1)
                    return realInters1;
                if (realInters1.HitDist > realInters2.HitDist)
                    return realInters2;
            }
            else if (inB && !inA)
            {
                HitInfo realInters1 = primA.Intersect(lineIn, renderray, threadId);
                if (realInters1.HitDist == -1)
                    return primB.Intersect(lineIn, renderray, threadId);
                HitInfo realInters2 = primB.Intersect(lineIn, renderray, threadId);
                // this next condition should never be true now.
                if (realInters2.HitDist == -1)
                    return realInters1;
                if (realInters1.HitDist > realInters2.HitDist)
                    return realInters2;
            }
            /* oops - out of optimisations ... doh!  */
            List<HitInfo> temp = AllIntersect(lineIn, renderray, threadId);
            if (temp.Count == 0)
            {
                HitInfo ret = new HitInfo();
                ret.HitDist = -1;
                return ret;
            }
            else
            {
                HitInfo res = temp[0];
                return res;
            }
        }

        protected override List<HitInfo> GetAllIntersect(Line lineIn, bool renderray, int threadId)
        {
            List<HitInfo> res = Owner.Pools[threadId].Get();
            if (!Bounds.HitTest(lineIn, renderray, threadId))
            {
                return res;
            }
            List<HitInfo> list = primA.AllIntersect(lineIn, renderray, threadId);
            if (list.Count == 0)
            {
                List<HitInfo> other2 = primB.AllIntersect(lineIn, renderray, threadId);
                res.AddRange(other2);
                return res;
            }
            List<HitInfo> other = primB.AllIntersect(lineIn, renderray, threadId);
            if (other.Count == 0)
            {
                res.AddRange(list);
                return res;
            }

            if (disjoint)
            {
                res.AddRange(list);
                res.AddRange(other);
                res.Sort(HitInfoComparer.Instance);
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
                        insideflag1 = !insideflag1;
                        if (!insideflag2)
                        {
                            res.Add(list[i]);
                        }
                        i++;
                        if (i >= imax) done1 = true;
                    }
                    else
                    {
                        insideflag2 = !insideflag2;
                        if (!insideflag1)
                        {
                            res.Add(other[j]);
                        }
                        j++;
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
                    if (!insideflag1)
                        for (; j < jmax; j++)
                            res.Add(other[j]);
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
