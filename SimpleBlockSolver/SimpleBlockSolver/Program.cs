using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBlockSolver
{
    class Level
    {
        public Level(char[][] data)
        {
            Data = data;
            for (int i = 0; i < Data.Length; i++)
            {
                for (int j = 0; j < Data[i].Length; j++)
                {
                    if (Data[i][j] == 'Z')
                    {
                        CurY = i;
                        CurX = j;
                        goto Found;
                    }
                }
            }            
            Found: ;
        }

        public readonly char[][] Data;
        public readonly int CurX;
        public readonly int CurY;
        public override bool Equals(object obj)
        {
            Level otherLevel = (Level) obj;

            for (int i = 0; i < Data.Length; i++)
            {
                for (int j = 0; j < Data[i].Length; j++)
                {
                    if (Data[i][j] != otherLevel.Data[i][j]) return false;
                }
            }
            return true;
        }

        private int _hashCode = -1;
        public override int GetHashCode()
        {
            if (_hashCode == -1)
            {
                int result = 51039875;
                for (int i = 0; i < Data.Length; i++)
                {
                    for (int j = 0; j < Data[i].Length; j++)
                    {
                        result *= 33;
                        result ^= Data[i][j];
                    }
                }
                _hashCode = result;
            }
            return _hashCode;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < Data.Length; i++)
            {
                for (int j = 0; j < Data[i].Length; j++)
                {
                    builder.Append(Data[i][j]);
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string[] rawLevel = new[]
            {
                "WWWWWWWWW",
                "WWWWW   W",
                "W    B  W",
                "W B  B WW",
                "W  B BBWW",
                "WW WWWZWW",
                "WWWWWWWWW"
            };
            string[] rawTargets = new[]
            {
                "WWWWWWWWW",
                "WWWWW  TW",
                "W       W",
                "W TT  TWW",
                "WT     WW",
                "WWTWWW WW",
                "WWWWWWWWW"
            };
            char[][] levelData = new char[rawLevel.Length][];
            for (int i = 0; i < levelData.Length; i++)
            {
                levelData[i] = rawLevel[i].ToCharArray();
            }
            Level firstLevel = new Level(levelData);
            Dictionary<Level, Level> foundFrom = new Dictionary<Level, Level>();
            foundFrom.Add(firstLevel, firstLevel);
            Queue<Level> toConsider = new Queue<Level>();
            toConsider.Enqueue(firstLevel);
            while (toConsider.Count != 0)
            {
                Level curLevel = toConsider.Dequeue();
                if (Done(curLevel, rawTargets))
                {
                    while (!curLevel.Equals(firstLevel))
                    {
                        Console.Out.WriteLine(curLevel.ToString());
                        Console.Out.WriteLine();
                        curLevel = foundFrom[curLevel];
                    }
                    Console.ReadKey();
                    break;
                }
                int x = curLevel.CurX;
                int y = curLevel.CurY;
                if (curLevel.Data[y - 1][x] == ' ' || curLevel.Data[y - 1][x] == 'B' && curLevel.Data[y - 2][x] == ' ')
                {
                    char[][] rawData = new char[curLevel.Data.Length][];
                    for (int i = 0; i < curLevel.Data.Length; i++)
                    {
                        rawData[i] = (char[])curLevel.Data[i].Clone();
                    }
                    if (curLevel.Data[y - 1][x] == 'B') rawData[y - 2][x] = 'B';
                    rawData[y][x] = ' ';
                    rawData[y - 1][x] = 'Z';
                    Level newLevel = new Level(rawData);
                    if (!foundFrom.ContainsKey(newLevel))
                    {
                        foundFrom.Add(newLevel, curLevel);
                        toConsider.Enqueue(newLevel);
                    }
                }
                if (curLevel.Data[y + 1][x] == ' ' || curLevel.Data[y + 1][x] == 'B' && curLevel.Data[y + 2][x] == ' ')
                {
                    char[][] rawData = new char[curLevel.Data.Length][];
                    for (int i = 0; i < curLevel.Data.Length; i++)
                    {
                        rawData[i] = (char[])curLevel.Data[i].Clone();
                    }
                    if (curLevel.Data[y + 1][x] == 'B') rawData[y + 2][x] = 'B';
                    rawData[y][x] = ' ';
                    rawData[y + 1][x] = 'Z';
                    Level newLevel = new Level(rawData);
                    if (!foundFrom.ContainsKey(newLevel))
                    {
                        foundFrom.Add(newLevel, curLevel);
                        toConsider.Enqueue(newLevel);
                    }

                }
                if (curLevel.Data[y][x - 1] == ' ' || curLevel.Data[y][x - 1] == 'B' && curLevel.Data[y][x - 2] == ' ')
                {
                    char[][] rawData = new char[curLevel.Data.Length][];
                    for (int i = 0; i < curLevel.Data.Length; i++)
                    {
                        rawData[i] = (char[])curLevel.Data[i].Clone();
                    }
                    if (curLevel.Data[y][x - 1] == 'B') rawData[y][x - 2] = 'B';
                    rawData[y][x] = ' ';
                    rawData[y][x - 1] = 'Z';
                    Level newLevel = new Level(rawData);
                    if (!foundFrom.ContainsKey(newLevel))
                    {
                        foundFrom.Add(newLevel, curLevel);
                        toConsider.Enqueue(newLevel);
                    }

                }
                if (curLevel.Data[y][x + 1] == ' ' || curLevel.Data[y][x + 1] == 'B' && curLevel.Data[y][x + 2] == ' ')
                {
                    char[][] rawData = new char[curLevel.Data.Length][];
                    for (int i = 0; i < curLevel.Data.Length; i++)
                    {
                        rawData[i] = (char[])curLevel.Data[i].Clone();
                    }
                    if (curLevel.Data[y][x + 1] == 'B') rawData[y][x + 2] = 'B';
                    rawData[y][x] = ' ';
                    rawData[y][x + 1] = 'Z';
                    Level newLevel = new Level(rawData);
                    if (!foundFrom.ContainsKey(newLevel))
                    {
                        foundFrom.Add(newLevel, curLevel);
                        toConsider.Enqueue(newLevel);
                    }
                }
            }
        }

        private static bool Done(Level curLevel, string[] rawTargets)
        {
            for (int i = 0; i < curLevel.Data.Length; i++)
            {
                for (int j = 0; j < curLevel.Data[i].Length; j++)
                {
                    if (rawTargets[i][j] == 'T' && curLevel.Data[i][j] != 'B')
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
