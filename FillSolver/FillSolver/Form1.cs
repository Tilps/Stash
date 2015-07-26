using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FillSolver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void checkBoxEdit_CheckedChanged(object sender, EventArgs e)
        {
            fillDisplay1.EditMode = checkBoxEdit.Checked;
            fillDisplay1.Refresh();
        }

        private void buttonSolve_Click(object sender, EventArgs e)
        {
            try
            {
                FillPuzzle duplicate = (FillPuzzle)fillDisplay1.Puzzle.Clone();
                duplicate.Solve();
                fillDisplay1.Puzzle = duplicate;
                fillDisplay1.Refresh();
                MessageBox.Show(string.Format("{0}:{1}:{2}", duplicate.SolveTime, duplicate.DeriveTime, duplicate.FillTime));
            }
            catch
            {
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            fillDisplay1.Puzzle = new FillPuzzle(10, 10);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void buttonRecreate_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    fillDisplay1.Puzzle = new FillPuzzle(10, 10);
                }
                else
                {
                    string[] parts = textBox1.Text.Split('x');
                    int xsize;
                    int ysize;
                    if (parts.Length == 1)
                        xsize = ysize = int.Parse(parts[0]);
                    else
                    {
                        xsize = int.Parse(parts[0]);
                        ysize = int.Parse(parts[1]);
                    }
                    fillDisplay1.Puzzle = new FillPuzzle(xsize, ysize);
                }
                fillDisplay1.Refresh();
            }
            catch
            {
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fillDisplay1.Puzzle.Save(dialog.FileName);
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                fillDisplay1.Puzzle = FillPuzzle.Load(dialog.FileName);
                fillDisplay1.Refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FillPuzzle duplicate = (FillPuzzle)fillDisplay1.Puzzle.Clone();
            duplicate.Init();
            for (int x = 0; x < fillDisplay1.Puzzle.width; x++)
            {
                for (int y = 0; y < fillDisplay1.Puzzle.height; y++)
                {
                    switch (fillDisplay1.Puzzle.Board[x, y])
                    {
                        case Status.Empty:
                            duplicate.Set(x, y, false);
                            break;
                        case Status.Filled:
                            duplicate.Set(x, y, true);
                            break;
                    }
                }
            }
            duplicate.SolveWithoutInit();
            fillDisplay1.Puzzle = duplicate;
            fillDisplay1.Refresh();
            MessageBox.Show(string.Format("{0}:{1}:{2}", duplicate.SolveTime, duplicate.DeriveTime, duplicate.FillTime));

        }

    }
}
