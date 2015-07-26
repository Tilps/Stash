using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PixelSolver
{
    public partial class PixelDisplay : Control
    {
        public PixelDisplay()
        {
            InitializeComponent();
        }
        public PixelPuzzle Puzzle;

        int deltaX;
        int deltaY;

        protected override void OnPaint(PaintEventArgs pe)
        {
            Brush fontBrush = new SolidBrush(this.ForeColor);
            Pen emptyLink = Pens.LightGray;
            Pen redLink = Pens.Red;
            Pen fullLink = Pens.Black;
            Brush dot = new SolidBrush(this.ForeColor);
            Rectangle border = this.ClientRectangle;
            border.Width = border.Width - 1;
            border.Height = border.Height - 1;
            if (Puzzle != null)
            {
                int longestRow;
                int longestColumn;
                int margin;
                int spacing;
                int xOffset;
                int yOffset;
                DetermineParams(out longestRow, out longestColumn, out margin, out spacing, out xOffset, out yOffset);
                int y = yOffset;
                for (int i = 0; i < Puzzle.RowClues.Count+1; i++)
                {
                    if (i < Puzzle.RowClues.Count)
                    {
                        List<int> clues = Puzzle.RowClues[i];
                        int x = margin + (longestRow - clues.Count) * spacing + deltaX;
                        for (int j = 0; j < clues.Count; j++)
                        {
                            int clue = clues[j];
                            if (Puzzle.RowColors[i][j] == 1)
                            {
                                pe.Graphics.DrawString(clue.ToString(), this.Font, fontBrush, x, y + this.Font.Height / 4);
                            }
                            else
                            {
                                pe.Graphics.DrawString(clue.ToString(), this.Font, new SolidBrush(Color.FromKnownColor(
                                   (KnownColor)(Puzzle.RowColors[i][j] + 4))), x, y + this.Font.Height / 4);
                            }
                            x += spacing;
                        }
                        if (clues.Count == 0)
                            pe.Graphics.DrawString("0", this.Font, fontBrush, x - spacing, y + this.Font.Height / 4);
                    }
                    pe.Graphics.DrawLine(i%5==0 ? redLink : fullLink, xOffset, y, xOffset + Puzzle.ColumnClues.Count * spacing, y);
                    y += spacing;
                }
                int x2 = xOffset;
                for (int i = 0; i < Puzzle.ColumnClues.Count+1; i++)
                {
                    if (i < Puzzle.ColumnClues.Count)
                    {
                        List<int> clues = Puzzle.ColumnClues[i];
                        int y2 = margin + (longestColumn - clues.Count) * spacing + deltaY;
                        for (int j = 0; j < clues.Count; j++)
                        {
                            int clue = clues[j];
                            if (Puzzle.ColumnColors[i][j] == 1)
                            {
                                pe.Graphics.DrawString(clue.ToString(), this.Font, fontBrush, x2 + this.Font.Height / 4, y2);
                            }
                            else
                            {
                                pe.Graphics.DrawString(clue.ToString(), this.Font, new SolidBrush(Color.FromKnownColor(
                                   (KnownColor)(Puzzle.ColumnColors[i][j] + 4))), x2 + this.Font.Height / 4, y2);
                            }
                            y2 += spacing;
                        }
                        if (clues.Count == 0)
                            pe.Graphics.DrawString("0", this.Font, fontBrush, x2 + this.Font.Height / 4, y2 - spacing);
                    }
                    pe.Graphics.DrawLine(i%5==0 ? redLink : fullLink, x2, yOffset, x2, yOffset + Puzzle.RowClues.Count * spacing);
                    x2 += spacing;
                }
                int y3 = yOffset;
                foreach (List<int> row in Puzzle.Board)
                {
                    int x3 = xOffset;
                    foreach (int cell in row)
                    {
                        if (cell == 0)
                        {
                            pe.Graphics.FillEllipse(fontBrush, x3 + spacing / 2-2, y3 + spacing / 2-2, 5, 5);
                        }
                        else if (cell == 1)
                        {
                            pe.Graphics.FillRectangle(fontBrush, x3+1, y3+1, spacing-1, spacing-1);
                        }
                        else if (cell > 1)
                        {
                            pe.Graphics.FillRectangle(new SolidBrush(Color.FromKnownColor(
                               (KnownColor)(cell + 4))), x3+1, y3+1, spacing-1, spacing-1);
                        }
                        x3 += spacing;
                    }
                    y3 += spacing;
                }
            }
            ControlPaint.DrawVisualStyleBorder(pe.Graphics, border);
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        private void DetermineParams(out int longestRow, out int longestColumn, out int margin, out int spacing, out int xOffset, out int yOffset)
        {
            longestRow = 1;
            foreach (List<int> clues in Puzzle.RowClues)
            {
                if (clues.Count > longestRow)
                    longestRow = clues.Count;
            }
            longestColumn = 1;
            foreach (List<int> clues in Puzzle.ColumnClues)
            {
                if (clues.Count > longestColumn)
                    longestColumn = clues.Count;
            }
            margin = 5;
            spacing = this.Font.Height * 3 / 2;
            xOffset = longestRow * spacing + margin + deltaX;
            yOffset = longestColumn * spacing + margin + deltaY;
        }

        int lastX;
        int lastY;
        int lastDeltaX;
        int lastDeltaY;

        protected override void OnDoubleClick(EventArgs e)
        {
            if (Control.MouseButtons == MouseButtons.Right)
            {
                deltaX = 0;
                deltaY = 0;
                Refresh();
            }
            base.OnDoubleClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                lastX = e.X;
                lastY = e.Y;
                lastDeltaX = deltaX;
                lastDeltaY = deltaY;
            }
            if (e.Button == MouseButtons.Left)
            {
                int longestRow;
                int longestColumn;
                int margin;
                int spacing;
                int xOffset;
                int yOffset;
                DetermineParams(out longestRow, out longestColumn, out margin, out spacing, out xOffset, out yOffset);

                int xLoc = (e.X - xOffset) / spacing;
                int yLoc = (e.Y - yOffset) / spacing;
                try
                {
                    int curVal = Puzzle.Board[yLoc][xLoc];
                    int newVal = curVal - 1;
                    if (newVal < -1)
                        newVal = Puzzle.colorMax;
                    Puzzle.Board[yLoc][xLoc] = newVal;
                    lastCol = newVal;
                    lastXSet = xLoc;
                    lastYSet = yLoc;

                    Refresh();
                }
                catch
                {
                }
            }
            base.OnMouseDown(e);
        }
        int lastCol;
        int lastXSet;
        int lastYSet;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                deltaX = lastDeltaX +  e.X - lastX;
                deltaY = lastDeltaY + e.Y - lastY;
                Refresh();
            }
            if (e.Button == MouseButtons.Left)
            {
                int longestRow;
                int longestColumn;
                int margin;
                int spacing;
                int xOffset;
                int yOffset;
                DetermineParams(out longestRow, out longestColumn, out margin, out spacing, out xOffset, out yOffset);

                int xLoc = (e.X - xOffset) / spacing;
                int yLoc = (e.Y - yOffset) / spacing;
                if (xLoc != lastXSet || yLoc != lastYSet)
                {
                    try
                    {
                        Puzzle.Board[yLoc][xLoc] = lastCol;

                        Refresh();
                    }
                    catch
                    {
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }
    }
}
