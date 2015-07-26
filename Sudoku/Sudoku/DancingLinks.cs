using System;
using System.Collections.Generic;
using System.Text;

namespace Sudoku
{
    public class DancingLinks
    {
        class Node
        {
            public Node left, right, up, down;
            public Header head;
        }
        class Header : Node
        {
            public int size;
            public string name;
        }

        protected int dequeRemovals;
        protected int numSolutions;

        private Header root;
        private List<Header> columns = new List<Header>();
        private Dictionary<string, int> names = new Dictionary<string,int>();
        private List<Node> rows = new List<Node>();
        private List<Node> stack = new List<Node>();
        private int iterStack;
        private Node iterRow;

        private Header column;
        private Node row;
        bool more;

        public void AddColumn(string name)
        {
            AddColumn(name, true);
        }

        public void AddColumn(string name, bool mandatory)
        {
            Header col = new Header();
            col.name = name;
            col.size = 0;
            col.up = col.down = col.left = col.right = col.head = col;
            if (mandatory)
            {
                col.right = root;
                col.left = root.left;
                root.left.right = col;
                root.left = col;
            }
            names[name] = columns.Count;
            columns.Add(col);
        }

        public void NewRow()
        {
            row = null;
        }

        public void DeleteRows()
        {
            foreach (Header it in columns)
                it.down = it.up = it;
            rows.Clear();
            row = null;
        }

        public void DisableRow(int number)
        {
            if (number >= rows.Count) return;
            Node i = rows[number];
            if (i.up == i) return; // already disabled
            Node j = i;
            do
            {
                j.up.down = j.down;
                j.down.up = j.up;
                j.down = j.up = j;
                j.head.size--;
                j = j.right;
            } while (i != j);
        }

        public void EnableRow(int number)
        {
            if (number >= rows.Count) return;
            Node i = rows[number];
            if (i.up != i) return; // already enabled
            Node j = i;
            do
            {
                j.up = j.head;
                j.down = j.head.down;
                j.up.down = j;
                j.down.up = j;
                j.head.size++;
                j = j.right;
            } while (i != j);
        }

        public void SetColumn(string name)
        {
            if (!names.ContainsKey(name)) return;
            SetColumn(names[name]);
        }

        public void SetColumn(int number)
        {
            if (number >= columns.Count) return;
            Header header = columns[number];
            Node node = new Node();
            if (row == null)
            {
                row = node;
                rows.Add(node);
                node.left = node;
                node.right = node;
            }
            else
            {
                node.left = row;
                node.right = row.right;
                row.right.left = node;
                row.right = node;
            }
            node.head = header;
            node.up = header;
            node.down = header.down;
            header.down.up = node;
            header.down = node;
            header.size++;
        }

        public string GetSolution()
        {
            if (iterRow != null)
            {
                string ret = iterRow.head.name;
                iterRow = iterRow.right;
                if (iterRow == stack[iterStack]) iterRow = null;
                return ret;
            }
            if (iterStack < stack.Count && ++iterStack < stack.Count)
            {
                iterRow = stack[iterStack];
            }
            return null;
        }

        protected virtual bool Record()
        {
            /*
    cout << "Solution (found after " << dequeRemovals
                     << " deque removals):" << endl;
                for (string* s = GetSolution(); s != NULL; s = GetSolution()) {
                    for (; s != NULL; s = GetSolution()) {
                        cout << *s << " ";
                    }
                    cout << endl;
                }
                cout << endl;
                return true;
             * */
            return true;
        }

        public void Solve()
        {
            more = true;
            stack.Clear();
            dequeRemovals = 0;
            numSolutions = 0;
            Search();
        }

        private void Search()
        {
            int depth = 0;
        Enter:
            depth++;
            if (root.right == root)
            {
                numSolutions++;
                iterStack = 0;
                iterRow = (iterStack < stack.Count ? stack[iterStack] : null);
                more = Record();
                goto Exit;
            }
            Choose();
            Cover(column);
            stack.Add(null);
            row = column.down;
        LoopStart:
            if (row == column)
            {
                goto LoopDone;
            }
            for (Node i = row.right; i != row; i = i.right)
            {
                Cover(i.head);
            }
            stack[stack.Count - 1] = row;
            goto Enter;
        Exit:
            depth--;
            if (depth == 0)
                return;
            row = stack[stack.Count - 1]; column = row.head;
            for (Node i = row.left; i != row; i = i.left)
            {
                Uncover(i.head);
            }
            if (!more) goto LoopDone;
            row = row.down;
            goto LoopStart;
        LoopDone:
            stack.RemoveAt(stack.Count - 1);
            Uncover(column);
            goto Exit;
        }

        private void Cover(Header col)
        {
            col.right.left = col.left;
            col.left.right = col.right;
            dequeRemovals++;
            for (Node i = col.down; i != col; i = i.down)
            {
                for (Node j = i.right; j != i; j = j.right)
                {
                    j.down.up = j.up;
                    j.up.down = j.down;
                    j.head.size--;
                    dequeRemovals++;
                }
            }
        }

        private void Uncover(Header col)
        {
            for (Node i = col.up; i != col; i = i.up)
            {
                for (Node j = i.left; j != i; j = j.left)
                {
                    j.head.size++;
                    j.down.up = j;
                    j.up.down = j;
                }
            }
            col.right.left = col;
            col.left.right = col;
        }

        private void Choose()
        {
            int best = int.MaxValue;
            for (Header i = (Header)root.right; i != root;
                          i = (Header)i.right)
            {
                if (i.size < best)
                {
                    column = i;
                    best = i.size;
                }
            }
        }

        public DancingLinks()
        {
            root = new Header();
            root.left = root.right = root;
            row = null;
        }
    }

    public class SudokuDancingLinks : DancingLinks
    {
        public SudokuDancingLinks(int sizex, int sizey)
        {
            this.width = sizex * sizey;
            this.grid = new int[width, width];
            this.solution = new int[width, width];
            this.sizex = sizex;
            this.sizey = sizey;

            // define columns for individual cells
            // ("a cell contains 1 and only 1 digit")
            for (int R = 0; R < width; R++)
            {
                for (int C = 0; C < width; C++)
                {
                    string ss = "R" + (R + 1).ToString() + "C" + (C + 1).ToString();
                    AddColumn(ss);
                }
            }

            // define columns for each group containing each digit
            // ("each digit appears once and only once in each group")
            for (int d = 1; d <= width; d++)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 1; j <= width; j++)
                    {
                        string ss = d.ToString() + "in" + "RCB"[i].ToString() + j.ToString();
                        AddColumn(ss);
                    }
                }
            }

            // create rows ("there could be any digit in any empty cell")
            for (int R = 0; R < width; R++)
            {
                for (int C = 0; C < width; C++)
                {
                    for (int d = 1; d <= width; d++)
                    {
                        NewRow();
                        string ss;
                        ss = "R" + (R + 1).ToString() + "C" + (C + 1).ToString();
                        SetColumn(ss);
                        ss = d.ToString() + "inR" + (R + 1).ToString();
                        SetColumn(ss);
                        ss = d.ToString() + "inC" + (C + 1).ToString();
                        SetColumn(ss);
                        ss = d.ToString() + "inB" + ((R / sizey) * sizey + (C / sizex) + 1).ToString();
                        SetColumn(ss);
                    }
                }
            }
        }
        
        private int width;
        private int sizex;
        private int sizey;

        public void Clear()
        {
            this.grid = new int[width, width];
            this.solution = new int[width, width];
        }

        public int[,] Grid
        {
            get
            {
                return grid;
            }
        }
        private int[,] grid;

        public int[,] Solution
        {
            get
            {
                return solution;
            }
        }
        private int[,] solution;

        public int Count
        {
            get
            {
                return count;
            }
        }
        private int count;

        public new void Solve()
        {
            // disable rows not satisfying fixed cells
            for (int R = 0; R < width; R++)
            {
                for (int C = 0; C < width; C++)
                {
                    if (grid[R,C] <= 0) continue;
                    for (int d = 1; d <= width; d++)
                    {
                        if (grid[R,C] != d) DisableRow(R * width * width + C * width + d - 1);
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
                    if (grid[R,C] <= 0) continue;
                    for (int d = 1; d <= width; d++)
                    {
                        if (grid[R,C] != d) EnableRow(R * width *width+ C * width + d - 1);
                    }
                }
            }
        }

        protected override bool Record()
        {
            if (++count > 1) return false;
            for (string s = GetSolution(); s != null; s = GetSolution())
            {
                int R = -1, C = -1;
                for (; s != null; s = GetSolution())
                {
                    string[] splits = s.Split('i', 'n');
                    if (splits.Length != 3)
                        continue;
                    if (splits[2][0] == 'R')
                        R = int.Parse(splits[2].Substring(1))-1;
                    if (splits[2][0] == 'C')
                        C = int.Parse(splits[2].Substring(1))-1;
                    if (R >= 0 && C >= 0) 
                        solution[R,C] = int.Parse(splits[0]);
                }
            }
            return true;
        }

    }
}
