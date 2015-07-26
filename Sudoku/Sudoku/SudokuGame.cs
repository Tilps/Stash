using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Sudoku
{
    public partial class SudokuGame : Form
    {
        public SudokuGame()
        {
            InitializeComponent();
        }

        List<List<TextBox>> boxes;
        private int sizex;
        private int sizey;

        private void ClearControls()
        {
            if (boxes != null)
            {
                foreach (List<TextBox> row in boxes)
                {
                    foreach (TextBox cell in row)
                    {
                        splitContainer2.Panel1.Controls.Remove(cell);
                        cell.Dispose();
                    }
                }
                boxes.Clear();
            }
        }

        private const int cellsize = 20;

        private void AddControls(int baseSizex, int baseSizey)
        {
            ClearControls();
            int width = baseSizex * baseSizey;
            sizex = baseSizex;
            sizey = baseSizey;
            boxes = new List<List<TextBox>>();
            for (int i = 0; i < width; i++)
            {
                boxes.Add(new List<TextBox>());
                for (int j = 0; j < width; j++)
                {
                    TextBox cell = new TextBox();
                    cell.Width = cellsize;
                    cell.Height = cellsize;
                    cell.Top = i * cellsize + i/baseSizey*2;
                    cell.Left = j * cellsize + j/baseSizex*2;
                    boxes[i].Add(cell);
                    splitContainer2.Panel1.Controls.Add(cell);
                }
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string[] splits = textBox1.Text.Split('x');
            if (splits.Length == 1) 
                AddControls(int.Parse(splits[0]), int.Parse(splits[0]));
            else
                AddControls(int.Parse(splits[0]), int.Parse(splits[1]));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (boxes == null)
            {
                return;
            }
            Board board = new Board(sizey, sizex);
            for (int i = 0; i < boxes.Count; i++)
            {
                for (int j = 0; j < boxes[i].Count; j++)
                {
                    try
                    {
                        if (boxes[i][j].Text.Length != 0)
                        {
                            int value = int.Parse(boxes[i][j].Text);
                            if (value > 0 && value <= boxes.Count)
                                board.Set(i, j, value);
                        }
                    }
                    catch
                    {
                        try
                        {
                            if (boxes[i][j].Text.Length != 0)
                            {
                                int value = char.ToUpper(boxes[i][j].Text[0])-'A'+10;
                                if (value > 0 && value <= boxes.Count)
                                    board.Set(i, j, value);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            try
            {
                if (textBox2.Text.Length != 0)
                {
                    string[] splits = textBox2.Text.Split('.');
                    board.MaxLookahead = int.Parse(splits[0]);
                }
            }
            catch
            {
            }
            board.UseLogging = true;
            SolveState result = board.SolveWithRating();
            if (board.LastLookaheadUsed != 1)
                textBox2.Text = board.LastLookaheadUsed.ToString();
            else
                textBox2.Text = board.LastLookaheadUsed.ToString() + "." + board.Score.ToString() + "." + board.HighTuples.ToString();
            for (int i = 0; i < boxes.Count; i++)
            {
                for (int j = 0; j < boxes[i].Count; j++)
                {
                    int value = board.Get(i, j);
                    if (value != 0)
                    {
                        boxes[i][j].Text = value.ToString();
                    }
                }
            }
            textBox3.Text = "";
            if (result != SolveState.Solved)
                MessageBox.Show("Solving produced the following result: " + result.ToString());
            else
                textBox3.Lines = board.Log.Split('\n');
        }

        /// <summary>
        /// Variables to hold return results from Internal generation functions
        /// </summary>
        List<int> xs = new List<int>();
        List<int> ys = new List<int>();
        List<int> values = new List<int>();

        private void Generate(int maxLookahead, Random rnd)
        {
            Generator g = new DancingGenerator(sizex, sizey, rnd, maxLookahead);
            g.Generate();
            xs = g.Xs;
            ys = g.Ys;
            values = g.Values;

            /* reduction phase, takes a long time right now and for no real gain.
            int width = boxes.Count;
            for (int i = 0; i < xs.Count; i++)
            {
                List<int> xs2 = new List<int>(xs);
                List<int> ys2 = new List<int>(ys);
                List<int> values2 = new List<int>(values);
                if (width % 2 == 0 || (xs2[i] != width / 2 || ys2[i] != width / 2))
                {
                    if (i > 0)
                    {
                        if (xs2[i - 1] == width - 1 - xs2[i] && ys2[i - 1] == width - 1 - ys2[i])
                        {
                            continue;
                        }
                    }
                    if (i < xs.Count - 1)
                    {
                        if (xs2[i + 1] == width - 1 - xs2[i] && ys2[i + 1] == width - 1 - ys2[i])
                        {
                            xs2.RemoveAt(i);
                            xs2.RemoveAt(i);
                            ys2.RemoveAt(i);
                            ys2.RemoveAt(i);
                            values2.RemoveAt(i);
                            values2.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    xs2.RemoveAt(i);
                    ys2.RemoveAt(i);
                    values2.RemoveAt(i);
                }
                Board board = new Board(sizey, sizex);
                board.MaxLookahead = maxLookahead;
                for (int j = 0; j < xs2.Count; j++)
                {
                    board.Set(xs2[j], ys2[j], values2[j]);
                }
                SolveState result = board.Solve();
                if (result == SolveState.Solved)
                {
                    xs = xs2;
                    ys = ys2;
                    values = values2;
                    i--;
                }
            }
            */
            // We have what we are looking for, output it and rate it.

            for (int i = 0; i < boxes.Count; i++)
            {
                for (int j = 0; j < boxes[i].Count; j++)
                {
                    boxes[i][j].Text = string.Empty;
                }
            }
            for (int i = 0; i < xs.Count; i++)
            {
                boxes[xs[i]][ys[i]].Text = values[i].ToString();
            }
            Board board2 = new Board(sizey, sizex);
            board2.MaxLookahead = maxLookahead;
            board2.Apply(xs, ys, values);
            textBox4.Lines = DrawBoard(board2); ;

            SolveState result2 = board2.SolveWithRating();
            if (board2.LastLookaheadUsed != 1)
                textBox2.Text = board2.LastLookaheadUsed.ToString();
            else
                textBox2.Text = board2.LastLookaheadUsed.ToString() + "." + board2.Score.ToString() + "." + board2.HighTuples.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Random rnd = new Random((int)DateTime.UtcNow.Ticks);
            if (boxes == null)
                return;
            int bigSize = sizex > sizey ? sizex : sizey;
            int smallSize = sizex < sizey ? sizex : sizey;
            int maxLookahead = 0;
            try
            {
                if (textBox5.Text.Length != 0)
                {
                    string[] splits = textBox5.Text.Split('.');
                    maxLookahead = int.Parse(splits[0]);
                }
            }
            catch
            {
            }
            if (maxLookahead > smallSize - 2 || (bigSize > 3 && maxLookahead != 0))
            {
                DialogResult res = MessageBox.Show("Your requested lookahead is not known to be generateable, selecting yes will most likely hang the software forever.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (res == DialogResult.No)
                    if (bigSize <= 3)
                        maxLookahead = smallSize - 2;
                    else
                        maxLookahead = 0;
                if (maxLookahead < 0)
                    maxLookahead = 0;
                textBox5.Text = maxLookahead.ToString();
            }
            bool done = false;
            while (!done)
            {
                try
                {
                    Generate(maxLookahead, rnd);
                    done = true;
                }
                catch (StackOverflowException)
                {
                    // New random for good luck.
                    rnd = new Random((int)DateTime.UtcNow.Ticks);
                    // Push the system harder!
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (boxes == null)
                return;
            string[] strs = textBox4.Lines;
            List<List<char>> lines = new List<List<char>>();
            for (int i = 0; i < strs.Length; i++)
            {
                lines.Add(new List<char>());
                for (int j = 0; j < strs[i].Length; j++)
                {
                    if (char.IsLetterOrDigit(strs[i][j]))
                    {
                        lines[i].Add(strs[i][j]);
                    }
                    else if (strs[i][j] == '.' || strs[i][j] == ' ')
                    {
                        lines[i].Add('0');
                    }
                }
            }
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (lines[i].Count == 0)
                    lines.RemoveAt(i);
            }
            int length = 0;
            if (lines.Count != boxes.Count)
            {
                MessageBox.Show("Number of lines does not match current layout.");
                return;
            }
            for (int i=0; i < lines.Count ;i++) {
                if (length == 0)
                    length = lines[i].Count;
                else if (length != lines[i].Count)
                {
                    MessageBox.Show("Text area does not contain consistant line lengths.");
                    return;
                }
            }
            if (length % lines.Count != 0)
            {
                MessageBox.Show("Line lengths dont match with number of lines.");
                return;
            }
            for (int i = 0; i < lines.Count; i++)
            {
                int offset = 0;
                for (int j = 0; j < lines.Count; j++)
                {
                    string temp ="";
                    for (int k = 0; k < length / lines.Count; k++)
                    {
                        char c = lines[i][offset];
                        c = char.ToUpper(c);
                        if (char.IsDigit(c) && c != '0')
                        {
                            temp += c;
                        }
                        else if (c != '0')
                        {
                            temp += (c - 'A' + 10).ToString();
                        }
                        offset++;
                    }
                    boxes[i][j].Text = temp;
                }
            }

        }

        private Dictionary<string, int> frequency = new Dictionary<string, int>();
        private Dictionary<string, int> frequency2 = new Dictionary<string, int>();
        private Dictionary<string, List<Board>> sample = new Dictionary<string, List<Board>>();
        private Dictionary<string, List<Board>> sample2 = new Dictionary<string, List<Board>>();

        private void Search()
        {
            Generator g = new DancingGenerator(sizex, sizey, new Random(), 1);
            g.EnsureDiff = false;
            int loop = 0;
            while (true)
            {
                loop++;
                g.Generate();
                List<int> xs = g.Xs;
                List<int> ys = g.Ys;
                List<int> values = g.Values;

                Board board2 = new Board(sizey, sizex);
                board2.MaxLookahead = 2;
                board2.Apply(xs, ys, values);

                SolveState result2 = board2.SolveWithRating();
                string name = "";
                if (board2.LastLookaheadUsed != 1)
                    name = board2.LastLookaheadUsed.ToString();
                else
                    name = board2.LastLookaheadUsed.ToString() + "." + board2.Score.ToString() + "." + board2.HighTuples.ToString();
                if (result2 != SolveState.Solved)
                {
                    name = "Failed to solve.";
                }
                else
                {

                    bool highest = true;
                    string[] nameSplits = name.Split('.');
                    foreach (string key in frequency.Keys)
                    {
                        string[] splits = key.Split('.');
                        if (int.Parse(splits[0]) > int.Parse(nameSplits[0]))
                        {
                            highest = false;
                            break;
                        }
                        else if (int.Parse(splits[0]) < int.Parse(nameSplits[0]))
                        {
                            continue;
                        }
                        else
                        {
                            if (splits.Length == 1)
                            {
                                highest = false;
                                break;
                            }
                            if (int.Parse(splits[1]) > int.Parse(nameSplits[1]))
                            {
                                highest = false;
                                break;
                            }
                            else if (int.Parse(splits[1]) < int.Parse(nameSplits[1]))
                            {
                                continue;
                            }
                            else
                            {
                                if (int.Parse(splits[2]) > int.Parse(nameSplits[2]))
                                {
                                    highest = false;
                                    break;
                                }
                                else if (int.Parse(splits[2]) < int.Parse(nameSplits[2]))
                                {
                                    continue;
                                }
                                else
                                {
                                    highest = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (highest)
                    {
                        this.Invoke(new UpdateBestDelegate(UpdateBest), xs, ys, values);
                    }
                }
                Board board = new Board(sizey, sizex);
                board.Apply(xs, ys, values);

                if (frequency.ContainsKey(name))
                {
                    frequency[name] = frequency[name] + 1;
                    if (sample[name].Count < 100)
                        sample[name].Add(board);
                }
                else
                {
                    frequency[name] = 1;
                    sample[name] = new List<Board>();
                    sample[name].Add(board);
                }
                name = "Count: " + xs.Count;
                if (frequency2.ContainsKey(name))
                {
                    frequency2[name] = frequency2[name] + 1;
                    if (sample2[name].Count < 100)
                        sample2[name].Add(board);
                }
                else
                {
                    frequency2[name] = 1;
                    sample2[name] = new List<Board>();
                    sample2[name].Add(board);
                }
                if ((loop & 0xF) == 0)
                {
                    string result = "";
                    foreach (KeyValuePair<string, int> de in frequency)
                    {
                        result += de.Key + ":" + de.Value.ToString() + "\n";
                    }
                    foreach (KeyValuePair<string, int> de in frequency2)
                    {
                        result += de.Key + ":" + de.Value.ToString() + "\n";
                    }
                    this.Invoke(new UpdateStringDelegate(UpdateString), result);
                }
            }
        }

        delegate void UpdateBestDelegate(List<int> xs, List<int> ys, List<int> values);
        private void UpdateBest(List<int> xs, List<int> ys, List<int> values)
        {
            Board board2 = new Board(sizey, sizex);
            board2.Apply(xs, ys, values);
            textBox4.Lines = DrawBoard(board2);
        }

        private string[] DrawBoard(Board board2)
        {
            List<string> strs = new List<string>();
            for (int i = 0; i < boxes.Count; i++)
            {
                if (i % sizey == 0 && i != 0)
                {
                    string str2 = "";
                    for (int j = 0; j < boxes.Count ; j++)
                    {
                        if (j % sizex == 0 && j != 0)
                            str2 += '+';
                        str2 += '-';
                    }
                    strs.Add(str2);
                }
                string str = "";
                for (int j = 0; j < boxes.Count; j++)
                {
                    if (j % sizex == 0 && j != 0)
                        str += '|';
                    if (board2.Get(i, j) == 0)
                        str += ".";
                    else if (board2.Get(i, j) < 10)
                        str += board2.Get(i, j).ToString();
                    else
                        str += ((char)('A' + board2.Get(i, j) - 10)).ToString();
                }
                strs.Add(str);
            }
            return strs.ToArray();
        }

        delegate void UpdateStringDelegate(string res);

        private void UpdateString(string result)
        {
            textBox3.Lines = result.Split('\n');
        }

        private Thread searchThread = null;

        private void button5_Click(object sender, EventArgs e)
        {
            if (searchThread != null)
            {
                searchThread.Abort();
                searchThread = null;
                button5.Text = "Search";
                return;
            }
            ThreadStart ts = new ThreadStart(Search);
            searchThread = new Thread(ts);
            searchThread.Start();
            button5.Text = "Stop Search";

        }

        private void button7_Click(object sender, EventArgs e)
        {
            frequency = new Dictionary<string, int>();
            frequency2 = new Dictionary<string, int>();
            sample = new Dictionary<string, List<Board>>();
            sample2 = new Dictionary<string, List<Board>>();

        }

        private void SaveBoards(string filename, List<Board> boards)
        {
            StreamWriter writer = File.CreateText(filename.Replace(":","") + ".txt");
            for (int i = 0; i < boards.Count; i++)
            {
                string[] lines = DrawBoard(boards[i]);
                for (int j=0; j < lines.Length; j++) 
                    writer.WriteLine(lines[j]);
                if (i < boards.Count - 1) 
                    writer.WriteLine();
            }
            writer.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, List<Board>> de in sample)
            {
                SaveBoards(de.Key, de.Value);
            }
            foreach (KeyValuePair<string, List<Board>> de in sample2)
            {
                SaveBoards(de.Key, de.Value);
            }
        }
    }
}