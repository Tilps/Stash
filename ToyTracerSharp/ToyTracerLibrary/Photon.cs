using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ToyTracerLibrary
{
    public class Photon
    {
        public PointF HitPos;
        public VectorF TravelDir;
        public ColorIntensityF PhotonColorPower;
        internal int SplitAxis;
#if DEBUG
        public Photon parent;
#endif
    }

    public class PhotonMap
    {
        public PhotonMap()
        {
            photonComparer[0] = new PhotonComparer(0);
            photonComparer[1] = new PhotonComparer(1);
            photonComparer[2] = new PhotonComparer(2);
        }

        public void InitForThreads(int threadCount, bool generating)
        {
            if (generating)
            {
                tlPhotons = new List<Photon>[threadCount];
                for (int i = 0; i < tlPhotons.Length; i++)
                    tlPhotons[i] = new List<Photon>();
            }
            heaps = new PhotonHeap[threadCount];
            for (int i = 0; i < heaps.Length; i++)
            {
                heaps[i] = new PhotonHeap(TargetCount);
                heaps[i].Comparer = new DistComparer();
            }
        }
        public void AddPhoton(Photon p, int threadId)
        {
            tlPhotons[threadId].Add(p);
        }
        public void BalanceMap()
        {
            if (tlPhotons != null)
            {
                photons.Clear();
                int length = 0;
                for (int i = 0; i < tlPhotons.Length; i++)
                {
                    length += tlPhotons[i].Count;
                }
                photons = new List<Photon>(length);
                for (int i = 0; i < tlPhotons.Length; i++)
                {
                    photons.AddRange(tlPhotons[i]);
                    tlPhotons[i] = null;
                }
                tlPhotons = null;
            }
            if (photons.Count == 0)
                return;
            BalanceSome(0, photons.Count - 1);
        }
        public void OptimiseRadi()
        {
            if (photons.Count == 0)
                return;
            Random rnd = new Random();
            double mind = 1.0E19;
            double avgd, sum, sum2;
            double maxd = avgd = sum = sum2 = 0.0;

            int numToSample = photons.Count / 20;
            if (numToSample > 1000) numToSample = 1000;
            if (numToSample < 100) numToSample = 100;

            PhotonHeap some = heaps[0];
            for (int i = 0; i < numToSample; i++)
            {
                int j = rnd.Next(photons.Count);

                Point to = new Point(photons[j].HitPos.X, photons[j].HitPos.Y, photons[j].HitPos.Z);
                some.Clear();
                some.Comparer.Loc = to;
                some.Comparer.Norm = new Vector();
                some.CurRadiusSq = 1.0E+19;
                GatherSome(0, photons.Count - 1, some);
                int n = some.Heap.Count;
                double density = n / (Math.PI * some.CurRadiusSq);


                if (density > maxd) maxd = density;
                if (density < mind) mind = density;
                sum += density;
                sum2 += density * density;
            }
            avgd = sum / numToSample;
            double saveDensity = avgd;

            this.MinSearchRadiusSq = TargetCount / (avgd * Math.PI);

            int lessThan = 0;
            for (int i = 0; i < numToSample; i++)
            {
                int j = rnd.Next(photons.Count);
                Point to = new Point(photons[j].HitPos.X, photons[j].HitPos.Y, photons[j].HitPos.Z);
                some.Clear();
                some.Comparer.Loc = to;
                some.Comparer.Norm = new Vector();
                some.CurRadiusSq = MinSearchRadiusSq;
                GatherSome(0, photons.Count - 1, some);
                int n = some.Heap.Count;
                double density = n / (Math.PI * some.CurRadiusSq);

                // count as "lessThan" if the density is below 70% of the average,
                // and if it is at least above 5% of the average.
                if (density < (saveDensity * 0.7) && density > (saveDensity * 0.05))
                    lessThan++;
            }

            // the 30.0 is a bit of a fudge-factor.
            MinSearchRadiusSq *= Math.Pow((1.0 + 20.0 * ((double)lessThan / (double)(numToSample))), 2.0);
            this.RadiusSqExpand = MinSearchRadiusSq * 8;
        }

        private void BalanceSome(int first, int last)
        {
            Point minExt;
            Point maxExt;
            DetermineExtents(first, last, out minExt, out maxExt);
            int splitAxis = 0;
            double curDif = maxExt.X - minExt.X;
            double yDif = maxExt.Y - minExt.Y;
            double zDif = maxExt.Z - minExt.Z;
            if (yDif > curDif)
            {
                splitAxis = 1;
                curDif = yDif;
            }
            if (zDif > curDif)
            {
                splitAxis = 2;
                curDif = zDif;
            }
            int medPos = PositionMedian(splitAxis, first, last);
            photons[medPos].SplitAxis = splitAxis;
            if (medPos - first > 1)
                BalanceSome(first, medPos - 1);
            else
            {
                photons[first].SplitAxis = -1;
            }
            if (last - medPos > 1)
            {
                BalanceSome(medPos + 1, last);
            }
            else if (last - medPos > 0)
            {
                photons[last].SplitAxis = -1;
            }
        }

        private void DetermineExtents(int first, int last, out Point minExt, out Point maxExt)
        {
            minExt = new Point(double.MaxValue, double.MaxValue, double.MaxValue);
            maxExt = new Point(double.MinValue, double.MinValue, double.MinValue);
            for (int i = first; i <= last; i++)
            {
                if (photons[i].HitPos.X < minExt.X)
                    minExt.X = photons[i].HitPos.X;
                if (photons[i].HitPos.X > maxExt.X)
                    maxExt.X = photons[i].HitPos.X;
                if (photons[i].HitPos.Y < minExt.Y)
                    minExt.Y = photons[i].HitPos.Y;
                if (photons[i].HitPos.Y > maxExt.Y)
                    maxExt.Y = photons[i].HitPos.Y;
                if (photons[i].HitPos.Z < minExt.Z)
                    minExt.Z = photons[i].HitPos.Z;
                if (photons[i].HitPos.Z > maxExt.Z)
                    maxExt.Z = photons[i].HitPos.Z;
            }
        }

        private int PositionMedian(int splitAxis, int first, int last)
        {
            int mid = (first + last + 1) >> 1;
            CutSort(first, last, mid, photons, this.photonComparer[splitAxis]);
            return mid;
        }

        private void CutSort(int first, int last, int cut, List<Photon> some, IComparer<Photon> comparer)
        {
            int mid = first + ((last - first) >> 1);
            int newMid = Pivot(first, last, mid, some, comparer);
            if (cut < newMid)
                CutSort(first, newMid - 1, cut, some, comparer);
            else if (cut > newMid)
                CutSort(newMid + 1, last, cut, some, comparer);
        }
        private int Pivot(int left, int right, int pivot, List<Photon> some, IComparer<Photon> comparer)
        {
            Photon val = some[pivot];
            while (comparer.Compare(some[left], val) < 0)
                left++;
            while (comparer.Compare(val, some[right]) < 0)
                right--;
            if (left >= right)
                return pivot;
            Photon temp = some[pivot];
            some[pivot] = some[right];
            some[right] = temp;
            int storedIndex = left;
            for (int i = left; i < right; i++)
            {
                int compareValue = comparer.Compare(some[i], val);
                if (compareValue < 0)
                {
                    temp = some[storedIndex];
                    some[storedIndex] = some[i];
                    some[i] = temp;
                    storedIndex++;
                }
            }
            temp = some[right];
            some[right] = some[storedIndex];
            some[storedIndex] = temp;
            while (storedIndex > pivot && comparer.Compare(some[storedIndex - 1], val) == 0)
                storedIndex--;
            while (storedIndex < pivot && comparer.Compare(some[storedIndex + 1], val) == 0)
                storedIndex++;
            return storedIndex;
        }

        private PhotonComparer[] photonComparer = new PhotonComparer[3];
        private List<Photon> photons = new List<Photon>();
        private List<Photon>[] tlPhotons;
        private PhotonHeap[] heaps;

        public double MinSearchRadiusSq;
        public double RadiusSqExpand;
        public int MaxExpandSteps;
        public int TargetMinCount;
        public int TargetCount = 100;


        public List<Photon> GetClosest(Point to, Vector normal, out double finalRadiusSq, int threadId)
        {
            double curRadiusSq = MinSearchRadiusSq;
            int curCount = 0;
            int step = 0;
            PhotonHeap some = heaps[threadId];
            some.Comparer.Loc = to;
            some.Comparer.Norm = normal;
            while (true)
            {
                some.Clear();
                finalRadiusSq = curRadiusSq;
                some.CurRadiusSq = curRadiusSq;
                GatherSome(0, photons.Count - 1, some);
                curCount = some.Heap.Count;
                if (curCount >= TargetMinCount || step >= MaxExpandSteps)
                    break;
                curRadiusSq += RadiusSqExpand;
                step++;
            }
            if (some.Heap.Count < 2)
                some.Clear();
            finalRadiusSq = some.CurRadiusSq;
            return some.Heap;
        }

        private void GatherSome(int first, int last, PhotonHeap some)
        {
            int mid = (first + last + 1) >> 1;
            Photon current = photons[mid];
            double daxis = 0.0;
            if (current.SplitAxis == 0)
            {
                daxis = current.HitPos.X - some.Comparer.Loc.X;
            }
            else if (current.SplitAxis == 1)
            {
                daxis = current.HitPos.Y - some.Comparer.Loc.Y;
            }
            else if (current.SplitAxis == 2)
            {
                daxis = current.HitPos.Z - some.Comparer.Loc.Z;
            }
            some.TryAdd(current);
            if (current.SplitAxis != -1)
            {
                if (daxis >= 0)
                {
                    GatherSome(first, mid - 1,some);
                    if (last != mid && daxis * daxis < some.CurRadiusSq)
                    {
                        GatherSome(mid + 1, last, some);
                    }
                }
                else
                {
                    if (last != mid)
                    {
                        GatherSome(mid + 1, last, some);
                    }
                    if (daxis * daxis < some.CurRadiusSq)
                    {
                        GatherSome(first, mid - 1, some);
                    }
                }
            }
        }

        private static Guid magic = new Guid("{EDBCFD1E-D05C-48f6-A2C5-96873A30BA3C}");
        private static int version = 1;

        internal void Save(string photonFile)
        {
            using (FileStream stream = new FileStream(photonFile, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(magic.ToByteArray());
                    writer.Write(version);
                    writer.Write(photons.Count);
                    foreach (Photon p in photons)
                    {
                        writer.Write(p.HitPos.X);
                        writer.Write(p.HitPos.Y);
                        writer.Write(p.HitPos.Z);
                        writer.Write(p.PhotonColorPower.R);
                        writer.Write(p.PhotonColorPower.G);
                        writer.Write(p.PhotonColorPower.B);
                        writer.Write(p.TravelDir.Dx);
                        writer.Write(p.TravelDir.Dy);
                        writer.Write(p.TravelDir.Dz);
                        writer.Write(p.SplitAxis);
                    }
                }
            }
        }

        internal void Load(string photonFile)
        {
            using (FileStream stream = new FileStream(photonFile, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    Guid magicCheck = new Guid(reader.ReadBytes(16));
                    if (!magic.Equals(magicCheck))
                        throw new ApplicationException("Photon file has wrong magic id.");
                    int versionCheck = reader.ReadInt32();
                    if (versionCheck != version)
                        throw new ApplicationException("Unsupported photon file version.");
                    int count = reader.ReadInt32();
                    photons = new List<Photon>(count);
                    for (int i = 0; i < count; i++)
                    {
                        Photon p = new Photon();
                        p.HitPos.X = reader.ReadSingle();
                        p.HitPos.Y = reader.ReadSingle();
                        p.HitPos.Z = reader.ReadSingle();
                        p.PhotonColorPower.R = reader.ReadSingle();
                        p.PhotonColorPower.G = reader.ReadSingle();
                        p.PhotonColorPower.B = reader.ReadSingle();
                        p.TravelDir.Dx = reader.ReadSingle();
                        p.TravelDir.Dy = reader.ReadSingle();
                        p.TravelDir.Dz = reader.ReadSingle();
                        p.SplitAxis = reader.ReadInt32();
                        photons.Add(p);
                    }
                }
            }
        }

        internal void ValidateBalance()
        {
            ValidateSome(0, photons.Count - 1);
        }

        private void ValidateSome(int first, int last)
        {
            int mid = (first + last + 1) >> 1;
            Photon current = photons[mid];
            if (current.SplitAxis == -1 && first != last)
                throw new ApplicationException("Invalid photon tree.");
            if (current.SplitAxis != -1)
            {
                PhotonComparer comparer = this.photonComparer[current.SplitAxis];
                for (int i = first; i <= last; i++)
                {
                    if (i == mid)
                        continue;
                    if (i < mid)
                    {
                        if (comparer.Compare(photons[i], current) > 0)
                            throw new ApplicationException("Incorrectly balanced photon tree.");
                    }
                    else
                    {
                        if (comparer.Compare(photons[i], current) < 0)
                            throw new ApplicationException("Incorrectly balanced photon tree.");
                    }
                }
                ValidateSome(first, mid - 1);
                if (mid != last)
                    ValidateSome(mid + 1, last);
            }
        }
    }
    public class PhotonHeap
    {
        public PhotonHeap(int size)
        {
            heap = new List<Photon>(size+1);
            dists = new List<double>(size+1);
            target = size;
        }

        public double CurRadiusSq;
        private int target;

        public DistComparer Comparer
        {
            get
            {
                return comparer;
            }
            set
            {
                comparer = value;
            }
        }
        DistComparer comparer;
        /// <summary>
        /// Adds a photon to the priority queue.
        /// </summary>
        /// <param name="evt">
        /// Photon to add to the queue.
        /// </param>
        /// <param name="distance">
        /// Distance value for this photon.
        /// </param>
        public void Add(Photon evt, double distance)
        {
            heap.Add(evt);
            dists.Add(distance);
            PushHeap(evt, distance, heap.Count - 1);
        }

        /// <summary>
        /// Pushes a value up the heap until it is correct.
        /// </summary>
        /// <param name="evt">
        /// Photon which is to be pushed up the heap.
        /// </param>
        /// <param name="distance">
        /// Distance value for the current photon.
        /// </param>
        /// <param name="startIndex">
        /// Current index of photon to be pushed up.
        /// </param>
        private void PushHeap(Photon evt, double distance, int startIndex)
        {
            int testNode = startIndex;
            int parentNode = (testNode - 1) / 2;
            while (testNode > 0 && dists[parentNode].CompareTo(distance) < 0)
            {
                heap[testNode] = heap[parentNode];
                dists[testNode] = dists[parentNode];
                testNode = parentNode;
                parentNode = (testNode - 1) / 2;
            }
            heap[testNode] = evt;
            dists[testNode] = distance;
        }

        /// <summary>
        /// Pops heap, rearangeing to be a new heap.
        /// </summary>
        /// <param name="length">
        /// Current length of heap.
        /// </param>
        private void PopHeap(int length, int topIndex)
        {
            if (length > 1)
            {
                // Now push empty down heap of one less size.
                length--;
                int secondChild = 2 * topIndex + 2;
                while (secondChild < length)
                {
                    if (dists[secondChild].CompareTo(dists[secondChild - 1]) < 0)
                    {
                        secondChild--;
                    }
                    dists[topIndex] = dists[secondChild];
                    heap[topIndex] = heap[secondChild];
                    topIndex = secondChild;
                    secondChild = 2 * topIndex + 2;
                }
                if (secondChild == length)
                {
                    secondChild--;
                    heap[topIndex] = heap[secondChild];
                    dists[topIndex] = dists[secondChild];
                    topIndex = secondChild;
                }
                // Now put the last element back into the heap of one less size, in the empty spot.
                PushHeap(heap[length], dists[length], topIndex);
                heap.RemoveAt(length);
                dists.RemoveAt(length);
            }
            else
            {
                heap.Clear();
                dists.Clear();
            }
        }
        public void PopFront()
        {
            PopHeap(heap.Count, 0);
        }
        public void Clear()
        {
            heap.Clear();
            dists.Clear();
        }


        /// <summary>
        /// Heap of photons to use as priority queue.
        /// </summary>
        public List<Photon> Heap
        {
            get
            {
                return heap;
            }
        }
        private List<Photon> heap;
        private List<double> dists;


        internal void TryAdd(Photon current)
        {
            double sqDist;
            if (comparer.CheckDist(current.HitPos, CurRadiusSq, out sqDist))
            {
                Add(current, sqDist);
                if (heap.Count > target)
                {
                    PopFront();
                }
                if (heap.Count == target)
                {
                    CurRadiusSq = dists[0];
                }
            }
        }
    }
    public class PhotonComparer : IComparer<Photon>
    {
        public PhotonComparer(int type)
        {
            this.type = type;
        }
        private int type;
        #region IComparer<Photon> Members

        public int Compare(Photon x, Photon y)
        {
            if (type == 0)
                return x.HitPos.X.CompareTo(y.HitPos.X);
            else if (type == 1)
                return x.HitPos.Y.CompareTo(y.HitPos.Y);
            else if (type == 2)
                return x.HitPos.Z.CompareTo(y.HitPos.Z);
            throw new ApplicationException("Invalid photon comparer type.");
        }

        #endregion
    }
    public class DistComparer
    {
        public DistComparer()
        {
            
        }
        public Point Loc;
        public Vector Norm;

        public bool CheckDist(PointF other, double cutoff, out double sqDist)
        {
            double dx = Loc.X - other.X;
            double dy = Loc.Y - other.Y;
            double dz = Loc.Z - other.Z;
            sqDist = dx * dx + dy * dy + dz * dz;
            if (sqDist < cutoff)
            {
                double offSurface = Math.Abs(dx * Norm.Dx + dy * Norm.Dy + dz * Norm.Dz);
                sqDist += 16 * sqDist * offSurface;
                if (sqDist < cutoff)
                    return true;
            }
            return false;
        }
    }
}
