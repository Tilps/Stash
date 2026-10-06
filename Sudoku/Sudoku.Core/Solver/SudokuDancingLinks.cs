namespace Sudoku.Core.Solver;

public class SudokuDancingLinks : DancingLinks
{
    private readonly int width;
    private readonly int sizex;
    private readonly int sizey;
    private int[,] grid;
    private int[,] solution;
    private int count;

    public SudokuDancingLinks(int sizex, int sizey)
    {
        this.sizex = sizex;
        this.sizey = sizey;
        this.width = sizex * sizey;
        this.grid = new int[width, width];
        this.solution = new int[width, width];

        // 1. Each cell contains 1 and only 1 digit
        for (int R = 0; R < width; R++)
        {
            for (int C = 0; C < width; C++)
            {
                AddColumn($"R{R + 1}C{C + 1}");
            }
        }

        // 2. Each digit appears once in each row, column, and block
        for (int d = 1; d <= width; d++)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 1; j <= width; j++)
                {
                    AddColumn($"{d}in{"RCB"[i]}{j}");
                }
            }
        }

        // 3. Create matrix rows: potential placements
        for (int R = 0; R < width; R++)
        {
            for (int C = 0; C < width; C++)
            {
                for (int d = 1; d <= width; d++)
                {
                    NewRow();
                    SetColumn($"R{R + 1}C{C + 1}");
                    SetColumn($"{d}inR{R + 1}");
                    SetColumn($"{d}inC{C + 1}");
                    int block = (R / sizey) * sizey + (C / sizex) + 1;
                    SetColumn($"{d}inB{block}");
                }
            }
        }
    }

    public int[,] Grid => grid;
    public int[,] Solution => solution;
    public int Count => count;

    public void Clear()
    {
        grid = new int[width, width];
        solution = new int[width, width];
    }

    public void SetGrid(int[,] newGrid)
    {
        for (int r = 0; r < width; r++)
        {
            for (int c = 0; c < width; c++)
            {
                grid[r, c] = newGrid[r, c];
            }
        }
    }

    public new void Solve()
    {
        // disable rows not satisfying fixed cells
        for (int R = 0; R < width; R++)
        {
            for (int C = 0; C < width; C++)
            {
                if (grid[R, C] <= 0) continue;
                for (int d = 1; d <= width; d++)
                {
                    if (grid[R, C] != d)
                    {
                        DisableRow(R * width * width + C * width + d - 1);
                    }
                }
            }
        }

        count = 0;
        base.Solve();

        // restore rows
        for (int R = 0; R < width; R++)
        {
            for (int C = 0; C < width; C++)
            {
                if (grid[R, C] <= 0) continue;
                for (int d = 1; d <= width; d++)
                {
                    if (grid[R, C] != d)
                    {
                        EnableRow(R * width * width + C * width + d - 1);
                    }
                }
            }
        }
    }

    protected override bool Record()
    {
        if (++count > 1) return false;
        for (string? s = GetSolution(); s != null; s = GetSolution())
        {
            int R = -1, C = -1;
            for (; s != null; s = GetSolution())
            {
                string[] splits = s.Split('i', 'n');
                if (splits.Length != 3) continue;
                if (splits[2][0] == 'R')
                    R = int.Parse(splits[2].Substring(1)) - 1;
                if (splits[2][0] == 'C')
                    C = int.Parse(splits[2].Substring(1)) - 1;
                if (R >= 0 && C >= 0)
                {
                    solution[R, C] = int.Parse(splits[0]);
                }
            }
        }
        return true;
    }
}
