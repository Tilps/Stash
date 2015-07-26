using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Treemap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog choose = new FolderBrowserDialog();
            if (choose.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                textBox1.Text = choose.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            root = new TreeNode { Name = textBox1.Text, Shade = RandomColor(rnd) };
            BuildTree(root, rnd, 1);
            LayoutRoot(root);
            this.Invalidate();
        }

        private void LayoutRoot(TreeNode root)
        {
            root.X = 5;
            root.Y = 45;
            root.Width = this.Width - 30;
            root.Height = this.Height - 100;
            LayoutTree(root);
        }

        private void LayoutTree(TreeNode root)
        {
            if (root.Children != null)
            {
                LayoutSegment(root.X, root.Y, root.Width, root.Height, root.Children, 0, root.Children.Count);
                foreach (TreeNode child in root.Children)
                    LayoutTree(child);
            }
        }

        private void LayoutSegment(int x, int y, int width, int height, List<TreeNode> list, int start, int length)
        {
            if (length == 0)
                return;
            if (length == 1)
            {
                list[start].X = x;
                list[start].Y = y;
                list[start].Width = width;
                list[start].Height = height;
                return;
            }
            bool switched = false;
            if (width > height)
            {
                switched = true;
                int temp = x;
                x = y;
                y = temp;
                temp = width;
                width = height;
                height = temp;
            }

            int totalArea = width * height;
            if (totalArea == 0)
            {
                // After zooming these values may not be zero and we need at least one layer of zeros to stop drawing.
                for (int i = start; i < start + length; i++)
                {
                    list[i].Width = width;
                    list[i].Height = height;
                }
                return;
            }

            int mid = length / 2 + start;
            long sum1 = 0;
            for (int i = start; i < mid; i++)
                sum1 += GetComponent(list[i]);
            long sum2 = GetComponent(list[mid]);
            long sum3 = 0;
            for (int i = mid + 1; i < start + length; i++)
                sum3 += GetComponent(list[i]);
            long sum4 = 0;
            long total = sum1+sum2+sum3;
            double ratio = CalcRatio(width, height, totalArea, sum1, sum2, sum3, sum4, total);
            double bestRatio = ratio;
            int bestIndex = start+length;
            for (int i = start+length-1; i > mid; i--)
            {
                sum4 += GetComponent(list[i]);
                sum3 -= GetComponent(list[i]);
                ratio = CalcRatio(width, height, totalArea, sum1, sum2, sum3, sum4, total);
                if (Math.Abs(1.0-ratio) < Math.Abs(1.0-bestRatio))
                {
                    bestRatio = ratio;
                    bestIndex = i;
                }
                else
                {
                    sum4 -= GetComponent(list[i]);
                    sum3 += GetComponent(list[i]);
                    break;
                }
            }


            double a1 = (double)sum1 / (double)total * (double)totalArea;
            double a2 = (double)sum2 / (double)total * (double)totalArea;
            double a3 = (double)sum3 / (double)total * (double)totalArea;
            double a4 = (double)sum4 / (double)total * (double)totalArea;
            double dy1 = a1 / (double)width;
            int y1 = y+(int)Math.Round(dy1);
            double dy2 = (double)height - a4 / (double)width;
            int y2 = y+(int)Math.Round(dy2);
            int x1 = x+(int)Math.Round(a2 / (dy2 - dy1));

            if (!switched)
            {
                LayoutSegment(x, y, width, y1 - y, list, start, mid - start);
                LayoutSegment(x, y1, x1 - x, y2 - y1, list, mid, 1);
                LayoutSegment(x1, y1, width - (x1 - x), y2 - y1, list, mid + 1, bestIndex - mid - 1);
                LayoutSegment(x, y2, width, height - (y2-y), list, bestIndex, length - (bestIndex - start));
            }
            else
            {
                LayoutSegmentSwitched(x, y, width, y1 - y, list, start, mid - start);
                LayoutSegmentSwitched(x, y1, x1 - x, y2 - y1, list, mid, 1);
                LayoutSegmentSwitched(x1, y1, width - (x1 - x), y2 - y1, list, mid + 1, bestIndex - mid - 1);
                LayoutSegmentSwitched(x, y2, width, height - (y2 - y), list, bestIndex, length - (bestIndex - start));
            }


        }

        private void LayoutSegmentSwitched(int x, int y, int width, int height, List<TreeNode> list, int start, int length)
        {
            LayoutSegment(y, x, height, width, list, start, length);
        }

        private static double CalcRatio(int width, int height, int totalArea, long sum1, long sum2, long sum3, long sum4, long total)
        {
            double a1 = (double)sum1 / (double)total * (double)totalArea;
            double a2 = (double)sum2 / (double)total * (double)totalArea;
            double a3 = (double)sum3 / (double)total * (double)totalArea;
            double a4 = (double)sum4 / (double)total * (double)totalArea;
            double y1 = a1 / (double)width;
            double y2 = (double)height - a4 / (double)width;
            double x1 = a2 / (y2 - y1);
            double ratio = x1 / (y2 - y1);
            return ratio;
        }

        private long GetComponent(TreeNode treeNode)
        {
            if (checkBox1.Checked)
                return treeNode.ChildCount;
            return treeNode.ChildTotalSize;
        }

        private static Color RandomColor(Random rnd)
        {
            return Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
        }

        private bool BuildTree(TreeNode root, Random rnd, int depth)
        {
            if (Directory.Exists(root.Name))
            {                
                try
                {
                    DirectoryInfo info = new DirectoryInfo(root.Name);
                    if ((info.Attributes & FileAttributes.ReparsePoint) != 0)
                        return false;
                    string[] children = Directory.GetFileSystemEntries(root.Name);
                    root.Children = new List<TreeNode>(children.Length);
                    foreach (string child in children)
                    {
                        TreeNode newChild = new TreeNode { Name = child, Shade = Blend(RandomColor(rnd), root.Shade, depth) };
                        newChild.Parent = root;
                        if (BuildTree(newChild, rnd, depth + 1))
                            root.Children.Add(newChild);
                    }
                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    root.ChildCount = 1;
                    root.ChildTotalSize = new FileInfo(root.Name).Length;
                    TreeNode parentWalk = root.Parent;
                    while (parentWalk != null)
                    {
                        parentWalk.ChildTotalSize += root.ChildTotalSize;
                        parentWalk.ChildCount += root.ChildCount;
                        parentWalk = parentWalk.Parent;
                    }
                }
                catch
                {
                }
            }
            return true;
        }

        private Color Blend(Color color, Color color_2, int weight2)
        {
            int total = weight2 + 1;
            return Color.FromArgb((color.R + color_2.R * weight2) / total, (color.G + color_2.G * weight2) / total, (color.B + color_2.B * weight2) / total);
        }

        private TreeNode root;

        class TreeNode
        {
            public TreeNode Parent;
            public string Name;
            public List<TreeNode> Children;
            public long ChildCount;
            public long ChildTotalSize;
            public int X;
            public int Y;
            public int Width;
            public int Height;
            public Color Shade;
            public Brush Brush;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (root != null)
            {
                PaintTree(root, e);
                if (lastFirstLevel != null)
                {
                    e.Graphics.DrawRectangle(Pens.Black, lastFirstLevel.X, lastFirstLevel.Y, lastFirstLevel.Width, lastFirstLevel.Height);
                }
            }
        }

        private void PaintTree(TreeNode root, PaintEventArgs e)
        {
            if (root.Brush == null)
                root.Brush = new SolidBrush(root.Shade);
            bool noDraw = true;
            if (root.Children != null)
            {
                foreach (var child in root.Children)
                {
                    if (child.Width == 0 || child.Height == 0)
                        continue;
                    PaintTree(child, e);
                    noDraw = false;
                }
            }
            if (noDraw)
            {
                e.Graphics.FillRectangle(root.Brush, root.X, root.Y, root.Width, root.Height);
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            TreeNode prevChild = lastFirstLevel;
            lastFirstLevel = null;
            if (root != null)
            {
                FindFile(root, e);
                if (root.Children != null)
                {
                    foreach (var child in root.Children)
                    {
                        if (e.X >= child.X && e.X < child.X + child.Width && e.Y >= child.Y && e.Y < child.Y + child.Height)
                        {
                            lastFirstLevel = child;
                            break;
                        }
                    }
                }
            }
            if (lastFirstLevel != prevChild)
            {
                this.Invalidate();
            }
        }
        private TreeNode lastFirstLevel;

        private void FindFile(TreeNode root, MouseEventArgs e)
        {
            if (root.Children == null)
                label1.Text = root.Name;
            else if (root.Width == 0 || root.Height == 0)
                label1.Text = root.Parent.Name;
            else
            {
                bool found = false;
                foreach (var child in root.Children)
                {
                    if (e.X >= child.X && e.X < child.X + child.Width && e.Y >= child.Y && e.Y < child.Y + child.Height)
                    {
                        FindFile(child, e);
                        found = true;
                    }
                }
                if (!found)
                    label1.Text = string.Empty;
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (root.Parent != null)
                {
                    root = root.Parent;
                    LayoutRoot(root);
                    this.Invalidate();
                    return;
                }
            }
            else
            {
                if (lastFirstLevel != null)
                {
                    root = lastFirstLevel;
                    LayoutRoot(root);
                    this.Invalidate();
                    return;
                }
            }
        }

    }
}
