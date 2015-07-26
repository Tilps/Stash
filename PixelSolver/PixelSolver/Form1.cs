using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PixelSolver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                puzzle = new PixelPuzzle(textBox1.Lines, textBox2.Lines);
                pixelDisplay1.Puzzle = puzzle;
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failure to update puzzle, due to input causing the following error. " + ex.ToString());
            }
        }

        PixelPuzzle puzzle;

        private void button2_Click(object sender, EventArgs e)
        {
            if (puzzle == null)
                return;
            try
            {
                puzzle.Solve();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error during solve. " + ex.ToString());
            }
            UpdateUI();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (puzzle == null)
                return;
            try
            {
                puzzle.RowPass();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error during solve. " + ex.ToString());
            }
            UpdateUI();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (puzzle == null)
                return;
            try
            {
                puzzle.ColumnPass();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error during solve. " + ex.ToString());
            }
            UpdateUI();
        }

        private void UpdateUI()
        {
            pixelDisplay1.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string[] lines = File.ReadAllLines(dialog.FileName);

                    string[] bits = lines[0].Split(' ');
                    int rows = int.Parse(bits[0]);
                    int columns = int.Parse(bits[1]);
                    string[] rowStrings = new string[rows];
                    string[] columnStrings = new string[columns];
                    Array.Copy(lines, 1, rowStrings, 0, rows);
                    Array.Copy(lines, 1 + rows, columnStrings, 0, columns);
                    textBox1.Lines = rowStrings;
                    textBox2.Lines = columnStrings;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error during load. " + ex.ToString());
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    List<string> lines = new List<string>();
                    lines.Add(textBox1.Lines.Length.ToString() + " " + textBox2.Lines.Length.ToString());
                    lines.AddRange(textBox1.Lines);
                    lines.AddRange(textBox2.Lines);
                    File.WriteAllLines(dialog.FileName, lines.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error during save. " + ex.ToString());
            }
        }
    }
}
