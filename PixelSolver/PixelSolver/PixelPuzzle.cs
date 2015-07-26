using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PixelSolver
{
    public class PixelPuzzle
    {
        public List<List<int>> RowClues;
        public List<List<int>> RowColors;
        public List<List<int>> ColumnClues;
        public List<List<int>> ColumnColors;
        public List<List<int>> Board;
        public int colorMax = 1;

        public PixelPuzzle(string[] rows, string[] columns)
            : this(GetLength(rows), GetLength(columns))
        {
            Parse(rows, RowClues, RowColors);
            Parse(columns, ColumnClues, ColumnColors);
        }

        private static int GetLength(string[] columns)
        {
            if (columns.Length == 1 && columns[0].StartsWith("Count="))
            {
                return int.Parse(columns[0].Substring("Count=".Length));
            }
            else
                return columns.Length;
        }

        private void Parse(string[] rows, List<List<int>> target, List<List<int>> target2)
        {
            if (rows.Length == 1 && rows[0].StartsWith("Count="))
            {
                return;
            }
            for (int i = 0; i < rows.Length; i++)
            {
                string[] bits = rows[i].Trim().Split(' ');
                foreach (string bit in bits)
                {
                    if (string.IsNullOrEmpty(bit))
                        continue;
                    string[] subs = bit.Split(',');

                    int val = int.Parse(subs[0]);
                    if (val == 0)
                        break;
                    target[i].Add(val);
                    int col = 1;
                    if (subs.Length > 1)
                        col = int.Parse(subs[1]);
                    if (col > colorMax)
                        colorMax = col;
                    target2[i].Add(col);
                }
            }
        }

        public PixelPuzzle(int rows, int columns)
        {
            Board = new List<List<int>>();
            RowClues = new List<List<int>>();
            RowColors = new List<List<int>>();
            ColumnClues = new List<List<int>>();
            ColumnColors = new List<List<int>>();
            for (int i = 0; i < rows; i++)
            {
                Board.Add(new List<int>());
                for (int j = 0; j < columns; j++)
                {
                    Board[i].Add(-1);
                }
                RowClues.Add(new List<int>());
                RowColors.Add(new List<int>());
            }
            for (int i = 0; i < columns; i++)
            {
                ColumnClues.Add(new List<int>());
                ColumnColors.Add(new List<int>());
            }
        }

        public void Solve()
        {
            bool progressing = true;
            while (progressing)
            {
                progressing = false;
                if (RowPass())
                    progressing = true;
                if (ColumnPass())
                    progressing = true;
            }
        }

        public bool RowPass()
        {
            bool progressing = false;
            for (int i = 0; i < RowClues.Count; i++)
            {
                List<KeyValuePair<int, int>> found = SimpleSolve(RowClues[i], RowColors[i], Board[i]);

                if (found.Count > 0)
                {
                    progressing = true;
                    foreach (KeyValuePair<int, int> proved in found)
                    {
                        Board[i][proved.Key] = proved.Value;
                    }
                }
            }
            return progressing;
        }

        private List<KeyValuePair<int, int>> SimpleSolve(List<int> clues, List<int> colors, List<int> existing)
        {
            List<KeyValuePair<int, int>> res = new List<KeyValuePair<int, int>>();
            for (int i = 0; i < existing.Count; i++)
            {
                if (existing[i] == -1)
                {
                    int color = -1;
                    for (int j = 0; j <= colorMax; j++)
                    {
                        existing[i] = j;
                        int possibles = CountSols(clues, colors, existing);
                        if (possibles > 0)
                        {
                            if (color == -1)
                                color = j;
                            else
                            {
                                color = -2;
                                break;
                            }
                        }
                    }
                    if (color >= 0)
                        res.Add(new KeyValuePair<int, int>(i, color));
                    // if color == -1 here we have an impossibility, -2 is simply uncertainty.
                    existing[i] = -1;
                }
            }
            return res;
        }

        private int CountSols(List<int> clues, List<int> colors, List<int> existing)
        {
            int[,] counts = new int[existing.Count + 1, clues.Count + 1]; // number of ways to solve first n spots using the first k clues, with the kth clue at the edge.
            counts[0, 0] = 1;
            for (int i = 1; i <= existing.Count; i++)
            {
                for (int j = 1; j <= clues.Count; j++)
                {
                    int prevTotal = 0;
                    int currentColor = colors[j - 1];
                    int currentCount = clues[j - 1];
                    bool gap = false;
                    if (j > 1 && currentColor == colors[j - 2])
                        gap = true;
                    bool fail = i-currentCount < 0;
                    if (!fail)
                    {
                        for (int k = 0; k < currentCount; k++)
                        {
                            if (existing[i - k - 1] != -1 && existing[i - k - 1] != currentColor)
                            {
                                fail = true;
                                break;
                            }
                        }
                    }
                    if (!fail && gap)
                    {
                        fail = i - currentCount - 1 < 0;
                        if (!fail)
                        {
                            if (existing[i - currentCount - 1] > 0)
                            {
                                fail = true;
                            }
                        }
                    }
                    if (!fail)
                    {
                        int start = i - currentCount - 1;
                        if (gap)
                            start--;
                        while (start >= -1)
                        {
                            prevTotal += counts[start + 1, j - 1];
                            if (start >= 0 && existing[start] > 0)
                                break;
                            start--;
                        }
                    }
                    counts[i, j] = prevTotal;
                }
            }
            int sum = 0;
            for (int i = existing.Count; i >= 0; i--)
            {
                sum += counts[i, clues.Count];
                if (i > 0 && existing[i - 1] > 0)
                    break;
            }
            return sum;
        }

        public bool ColumnPass()
        {
            bool progressing = false;
            for (int i = 0; i < ColumnClues.Count; i++)
            {
                List<KeyValuePair<int, int>> found = SimpleSolve(ColumnClues[i], ColumnColors[i], ConstructColumnView(i));

                if (found.Count > 0)
                {
                    progressing = true;
                    foreach (KeyValuePair<int, int> proved in found)
                    {
                        Board[proved.Key][i] = proved.Value;
                    }
                }
            }
            return progressing;
        }

        private List<int> ConstructColumnView(int i)
        {
            List<int> res = new List<int>();
            for (int j = 0; j < Board.Count; j++)
            {
                res.Add(Board[j][i]);
            }
            return res;
        }
    }
}
