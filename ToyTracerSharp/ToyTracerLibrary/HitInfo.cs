using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{
    public struct HitInfo
    {

        public Primative Hit;

        /// <summary>
        /// This is a optional index to accelerate the creation of RealHitInfo from HitInfo
        /// </summary>
        public int SurfaceIndex;

        public double HitDist;

        public RealHitInfo GetReal(Line by)
        {
            if (Hit != null)
                return Hit.DelayedHitCalc(by, this);
            throw new InvalidOperationException("This should never happen!");
        }

        public bool EarlierThan(HitInfo other)
        {
            return HitDist < other.HitDist;
        }

    }

    public class HitInfoListPool
    {
        public List<HitInfo> Get()
        {
            if (pool.Count == 0)
                return new List<HitInfo>();
            else
            {
                return pool.Pop();
            }
        }
        public void Add(List<HitInfo> free)
        {
            free.Clear();
            pool.Push(free);
        }
        Stack<List<HitInfo>> pool = new Stack<List<HitInfo>>();
    }

    public class HitInfoComparer : IComparer<HitInfo>
    {
        public static HitInfoComparer Instance = new HitInfoComparer();
        #region IComparer<Line> Members

        public int Compare(HitInfo x, HitInfo y)
        {
            return x.HitDist.CompareTo(y.HitDist);
        }

        #endregion
    }

    public struct RealHitInfo
    {
        public Material HitStuff;

        public Line Normal;

        public Texture Pigment;
    }
}
