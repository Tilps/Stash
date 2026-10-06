namespace Sudoku.Core.Solver;

public class DancingLinks
{
    protected class Node
    {
        public Node left = null!;
        public Node right = null!;
        public Node up = null!;
        public Node down = null!;
        public Header head = null!;
    }

    protected class Header : Node
    {
        public int size;
        public string name = "";
    }

    protected int dequeRemovals;
    protected int numSolutions;

    private Header root;
    private List<Header> columns = new();
    private Dictionary<string, int> names = new();
    private List<Node> rows = new();
    private List<Node?> stack = new();
    private int iterStack;
    private Node? iterRow;

    private Header column = null!;
    private Node? row;
    private bool more;

    public DancingLinks()
    {
        root = new Header();
        root.left = root.right = root;
        row = null;
    }

    public void AddColumn(string name, bool mandatory = true)
    {
        Header col = new()
        {
            name = name,
            size = 0
        };
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
        {
            it.down = it.up = it;
        }
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
        if (names.TryGetValue(name, out int index))
        {
            SetColumn(index);
        }
    }

    public void SetColumn(int number)
    {
        if (number >= columns.Count) return;
        Header header = columns[number];
        Node node = new();
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

    public string? GetSolution()
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
        row = stack[stack.Count - 1];
        column = row!.head;
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
        for (Header i = (Header)root.right; i != root; i = (Header)i.right)
        {
            if (i.size < best)
            {
                column = i;
                best = i.size;
            }
        }
    }
}
