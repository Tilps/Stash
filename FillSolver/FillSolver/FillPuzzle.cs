using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FillSolver
{
    public class FillPuzzle : ICloneable
    {

        public FillPuzzle(int x, int y)
        {
            width = x;
            height = y;
            Board = new Status[x, y];
            Clues = new int[x, y];
            areaSpotLookups = new List<int>[x, y];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    Clues[i, j] = -1;
                    areaSpotLookups[i, j] = new List<int>();
                }
            }
        }

        public readonly int width;
        public readonly int height;

        public Status[,] Board;
        public int[,] Clues;

        private List<int>[,] areaSpotLookups;
        private int lastId;
        private Dictionary<AreaKey, Area> areaLookup = new Dictionary<AreaKey, Area>();
        private Dictionary<int, Area> areaIdLookup = new Dictionary<int, Area>();


        public object Clone()
        {
            FillPuzzle newPuzzle = new FillPuzzle(width, height);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    newPuzzle.Clues[i, j] = this.Clues[i, j];
                }
            }
            return newPuzzle;
        }

        internal void Init()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (Clues[i, j] > -1)
                        CreateAreaWithCenter(i, j, Clues[i, j]);
                }
            }
        }

        internal void Solve()
        {
            Init();
            SolveWithoutInit();
        }

        internal void SolveWithoutInit()
        {
            Stopwatch watch = new Stopwatch();
            Stopwatch dWatch = new Stopwatch();
            Stopwatch fWatch = new Stopwatch();
            watch.Start();
            fWatch.Start();
            while (FillPass())
            {
            }
            fWatch.Stop();
            bool progress;
            do
            {
                dWatch.Start();
                progress = DerivePass();
                dWatch.Stop();
                fWatch.Start();
                while (FillPass())
                {
                    progress = true;
                }
                fWatch.Stop();
            } while (progress);
            watch.Stop();
            SolveTime = watch.ElapsedTicks;
            DeriveTime = dWatch.ElapsedTicks;
            FillTime = fWatch.ElapsedTicks;
        }

        public long SolveTime;
        public long DeriveTime;
        public long FillTime;

        private void CreateAreaWithCenter(int x, int y, int count)
        {
            Area newArea = new Area();
            for (int j = -1; j <= 1; j++)
            {
                if (y + j < 0)
                    continue;
                if (y + j >= height)
                    continue;
                for (int i = -1; i <= 1; i++)
                {
                    if (x + i < 0)
                        continue;
                    if (x + i >= width)
                        continue;
                    newArea.Location.spots.Add(x + i + (y + j) * width);
                }
            }
            newArea.maxCount = count;
            newArea.minCount = count;
            AddArea(newArea);
        }

        private void AddArea(Area newArea)
        {
            if (!areaLookup.ContainsKey(newArea.Location))
            {
                int id = lastId;
                lastId++;
                newArea.Id = id;
                areaIdLookup.Add(id, newArea);
                areaLookup.Add(newArea.Location, newArea);
                foreach (int spot in newArea.Location.spots)
                {
                    areaSpotLookups[spot % width, spot / width].Add(id);
                }
            }
        }

        private bool FillPass()
        {
            List<Area> complete = new List<Area>();
            foreach (Area area in areaIdLookup.Values)
            {
                if (area.maxCount == area.minCount)
                {
                    if (area.maxCount == area.Location.spots.Count)
                        complete.Add(area);
                    else if (area.maxCount == 0)
                        complete.Add(area);
                }
            }
            bool progress = false;
            // These areas may be dynamically affected due to overlap, but ... hopefully not in a way which breaks this code!
            foreach (Area area in complete)
            {
                bool fill = area.maxCount != 0;
                if (area.Location.spots.Count > 0)
                {
                    int[] spots = new int[area.Location.spots.Count];
                    area.Location.spots.CopyTo(spots);
                    foreach (int loc in spots)
                    {
                        int x = loc % width;
                        int y = loc / width;
                        progress |= Set(x, y, fill);
                    }
                }
            }
            return progress;
        }

        internal bool Set(int x, int y, bool fill)
        {
            if (fill && Board[x, y] == Status.Empty || !fill && Board[x,y] == Status.Filled)
                throw new Exception("UHOH");
            if (Board[x, y] == Status.Unknown)
            {
                if (fill)
                    Board[x, y] = Status.Filled;
                else
                    Board[x, y] = Status.Empty;
                foreach (int areaId in areaSpotLookups[x, y])
                {
                    Area a = areaIdLookup[areaId];
                    areaLookup.Remove(a.Location);
                    a.Shrink(x + y * width, fill);
                    if (a.Location.spots.Count > 0)
                    {
                        Area otherMatch;
                        if (areaLookup.TryGetValue(a.Location, out otherMatch))
                        {
                            areaIdLookup.Remove(areaId);
                            foreach (int spot in a.Location.spots)
                            {
                                int x2 = spot % width;
                                int y2 = spot / width;
                                // Should be irrelivent since a has been shrunk...
                                if (x != x2 || y != y2)
                                {
                                    areaSpotLookups[x2, y2].Remove(areaId);
                                }
                            }
                            if (a.minCount > otherMatch.minCount)
                                otherMatch.minCount = a.minCount;
                            if (a.maxCount < otherMatch.maxCount)
                                otherMatch.maxCount = a.maxCount;
                        }
                        else
                        {
                            areaLookup.Add(a.Location, a);
                        }
                    }
                    else
                    {
                        areaIdLookup.Remove(areaId);
                    }
                }
                areaSpotLookups[x, y].Clear();
                return true;
            }
            return false;
        }

        private bool DerivePass()
        {
            List<Area> derived = new List<Area>();
            for (int spot = 0; spot < height * width; spot++)
            {
                int x = spot % width;
                int y = spot / width;
                List<int> areas = areaSpotLookups[x,y];
                for (int i = 0; i < areas.Count; i++)
                {
                    for (int j = i + 1; j < areas.Count; j++)
                    {
                        Area left;
                        Area right;
                        Area inters;
                        areaIdLookup[areas[i]].MarkParts(areaIdLookup[areas[j]], out left, out right, out inters);
                        if (left.minCount != 0 || left.maxCount != left.Location.spots.Count)
                            derived.Add(left);
                        if (right.minCount != 0 || right.maxCount != right.Location.spots.Count)
                            derived.Add(right);
                        if (inters.minCount != 0 || inters.maxCount != inters.Location.spots.Count)
                            derived.Add(inters);
                    }
                }
            }
            bool progress = false;
            foreach (Area d in derived)
            {
                Area other;
                if (areaLookup.TryGetValue(d.Location, out other))
                {
                    if (d.maxCount < other.maxCount)
                    {
                        other.maxCount = d.maxCount;
                        progress = true;
                    }
                    if (d.minCount > other.minCount)
                    {
                        other.minCount = d.minCount;
                        progress = true;
                    }
                }
                else
                {
                    AddArea(d);
                    progress = true;
                }
            }
            return progress;
        }

        internal void Save(string fileName)
        {
            using (StreamWriter output = File.CreateText(fileName))
            {
                output.WriteLine("{0}x{1}", width, height);
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (Clues[x, y] >= 0)
                            output.Write(Clues[x, y].ToString());
                        else
                            output.Write(' ');
                    }
                    output.WriteLine();
                }
            }
        }

        internal static FillPuzzle Load(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            string[] bits = lines[0].Split('x');
            int width = int.Parse(bits[0]);
            int height = int.Parse(bits[1]);
            FillPuzzle res = new FillPuzzle(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (lines[i + 1][j] != ' ')
                    {
                        res.Clues[j, i] = int.Parse(""+lines[i + 1][j]);
                    }
                }
            }
            return res;
        }
    }

    public class AreaKey
    {
        public List<int> spots = new List<int>(9);
        public override int GetHashCode()
        {
            int basis = 124315136;
            foreach (int i in spots)
                basis ^= i;
            return basis;
        }

        public override bool Equals(object obj)
        {
            AreaKey other = obj as AreaKey;
            if (other.spots.Count != spots.Count)
                return false;
            for (int i = 0; i < other.spots.Count; i++)
            {
                if (spots[i] != other.spots[i])
                    return false;
            }
            return true;
        }
    }

    public class Area
    {
        public AreaKey Location = new AreaKey();
        public int minCount;
        public int maxCount;
        public int Id;

        internal void Shrink(int spot, bool filled)
        {
            Location.spots.Remove(spot);
            if (filled)
            {
                maxCount--;
                minCount--;
            }
            if (maxCount > Location.spots.Count)
                maxCount = Location.spots.Count;
            if (minCount < 0)
                minCount = 0;
        }

        internal void MarkParts(Area other, out Area left, out Area right, out Area inters)
        {
            left = new Area();
            inters = new Area();
            right = new Area();
            foreach (int spot in Location.spots)
            {
                if (!other.Location.spots.Contains(spot))
                    left.Location.spots.Add(spot);
                else
                    inters.Location.spots.Add(spot);
            }
            foreach (int spot in other.Location.spots)
            {
                if (!Location.spots.Contains(spot))
                    right.Location.spots.Add(spot);
            }
            left.maxCount = left.Location.spots.Count;
            inters.maxCount = inters.Location.spots.Count;
            right.maxCount = right.Location.spots.Count;

            bool progress;
            do
            {
                progress = false;
                int nlMax = Math.Min(this.maxCount - inters.minCount, this.maxCount - other.minCount + right.maxCount);
                if (nlMax < left.maxCount)
                {
                    left.maxCount = nlMax;
                    progress = true;
                }
                int nlMin = Math.Max(this.minCount - inters.maxCount, this.minCount - other.maxCount + right.minCount);
                if (nlMin > left.minCount)
                {
                    left.minCount = nlMin;
                    progress = true;
                }
                int nrMax = Math.Min(other.maxCount - inters.minCount, other.maxCount - this.minCount + left.maxCount);
                if (nrMax < right.maxCount)
                {
                    right.maxCount = nrMax;
                    progress = true;
                }
                int nrMin = Math.Max(other.minCount - inters.maxCount, other.minCount - this.maxCount + left.minCount);
                if (nrMin > right.minCount)
                {
                    right.minCount = nrMin;
                    progress = true;
                }
                int niMax = Math.Min(this.maxCount - left.minCount, other.maxCount - right.minCount);
                if (niMax < inters.maxCount)
                {
                    inters.maxCount = niMax;
                    progress = true;
                }
                int niMin = Math.Max(this.minCount - left.maxCount, other.minCount - right.maxCount);
                if (niMin > inters.minCount)
                {
                    inters.minCount = niMin;
                    progress = true;
                }
                /*
                i=A-j
                i=A-B-k
                j=A-i
                j=B-k
                k=B-j
                k=B-A-i
                 * */
            }
            while (progress);
        }

     }

    public enum Status
    {
        Unknown,
        Filled,
        Empty,
    }
}
