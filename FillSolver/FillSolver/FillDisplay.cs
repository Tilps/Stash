using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FillSolver
{
    public partial class FillDisplay : Control
    {
        public FillDisplay()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }
        public FillPuzzle Puzzle;

        protected override bool IsInputKey(Keys keyData)
        {
            return true;
//            return base.IsInputKey(keyData);
        }

        protected override bool IsInputChar(char charCode)
        {
            return true;
//            return base.IsInputChar(charCode);
        }

        int deltaX;
        int deltaY;

        public bool EditMode = false;

        protected override void OnPaint(PaintEventArgs pe)
        {
            Brush fontBrush = new SolidBrush(this.ForeColor);
            Brush otherColorBrush = new SolidBrush(Color.WhiteSmoke);
            Pen emptyLink = Pens.LightGray;
            Pen redLink = Pens.Red;
            Pen fullLink = Pens.Black;
            Brush dot = new SolidBrush(this.ForeColor);
            Brush fill = new SolidBrush(Color.Black);
            Brush selected = new SolidBrush(Color.Orange);
            Rectangle border = this.ClientRectangle;
            border.Width = border.Width - 1;
            border.Height = border.Height - 1;
            if (Puzzle != null)
            {
                int spacing;
                int xOffset;
                int yOffset;
                DetermineParams(out spacing, out xOffset, out yOffset);
                for (int i = 0; i <= Puzzle.width; i++)
                {
                    pe.Graphics.DrawLine(fullLink, xOffset + i * spacing, yOffset, xOffset + i * spacing, yOffset + spacing * Puzzle.height); 
                }
                for (int j = 0; j <= Puzzle.height; j++)
                {
                    pe.Graphics.DrawLine(fullLink, xOffset, yOffset + j * spacing, xOffset + spacing * Puzzle.width, yOffset + j * spacing);
                }
                for (int i = 0; i < Puzzle.width; i++)
                {
                    for (int j = 0; j < Puzzle.height; j++)
                    {
                        switch (Puzzle.Board[i, j])
                        {
                            case Status.Empty:
                                pe.Graphics.DrawLine(fullLink, xOffset + i * spacing + spacing / 4, yOffset + j * spacing + spacing / 4, xOffset + i * spacing + 3 * spacing / 4, yOffset + j * spacing + 3 * spacing / 4);
                                pe.Graphics.DrawLine(fullLink, xOffset + i * spacing + 3*spacing / 4, yOffset + j * spacing + spacing / 4, xOffset + i * spacing + spacing / 4, yOffset + j * spacing + 3 * spacing / 4);
                                break;
                            case Status.Filled:
                                pe.Graphics.FillRectangle(fill, xOffset + i * spacing + 2, yOffset + j * spacing + 2, spacing - 3, spacing - 3);
                                break;
                        }
                        if (EditMode && i==lastXSet && j==lastYSet)
                        {
                            pe.Graphics.FillRectangle(selected, xOffset + i * spacing + 2, yOffset + j * spacing + 2, spacing - 3, spacing - 3);
                        }
                        if (Puzzle.Clues[i, j] != -1)
                        {
                            if (Puzzle.Board[i, j] != Status.Filled)
                            {
                                pe.Graphics.DrawString(Puzzle.Clues[i, j].ToString(), this.Font, fontBrush, xOffset + i * spacing + spacing / 4, yOffset + j * spacing + spacing / 4);
                            }
                            else
                            {
                                pe.Graphics.DrawString(Puzzle.Clues[i, j].ToString(), this.Font, otherColorBrush, xOffset + i * spacing + spacing / 4, yOffset + j * spacing + spacing / 4);
                            }
                        }
                    }
                }
            }
            ControlPaint.DrawVisualStyleBorder(pe.Graphics, border);
            // Calling the base class OnPaint
            base.OnPaint(pe);
        }

        private void DetermineParams(out int spacing, out int xOffset, out int yOffset)
        {
            int margin = 5;
            spacing = this.Font.Height * 3 / 2;
            xOffset = margin + deltaX;
            yOffset = margin + deltaY;
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
            this.Select();
            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = true;
            this.Select();
            base.OnKeyPress(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (EditMode)
            {
                try
                {
                    if (!MoveCode(e.KeyCode))
                    {
                        Puzzle.Clues[lastXSet, lastYSet] = ConvertCode(e.KeyCode);
                    }
                    this.Select();
                    Refresh();
                }
                catch
                {
                }
            }
            base.OnKeyUp(e);
        }

        private bool MoveCode(Keys keys)
        {
            switch (keys)
            {
                case Keys.Left:
                    if (lastXSet > 0)
                        lastXSet--;
                    return true;
                case Keys.Right:
                    if (lastXSet < Puzzle.width - 1)
                        lastXSet++;
                    return true;
                case Keys.Down:
                    if (lastYSet < Puzzle.height - 1)
                        lastYSet++;
                    return true;
                case Keys.Up:
                    if (lastYSet > 0)
                        lastYSet--;
                    return true;
            }
            return false;
        }

        private int ConvertCode(Keys keys)
        {
            switch (keys)
            {
                case Keys.D0:
                case Keys.NumPad0:
                    return 0;
                case Keys.D1:
                case Keys.NumPad1:
                    return 1;
                case Keys.D2:
                case Keys.NumPad2:
                    return 2;
                case Keys.D3:
                case Keys.NumPad3:
                    return 3;
                case Keys.D4:
                case Keys.NumPad4:
                    return 4;
                case Keys.D5:
                case Keys.NumPad5:
                    return 5;
                case Keys.D6:
                case Keys.NumPad6:
                    return 6;
                case Keys.D7:
                case Keys.NumPad7:
                    return 7;
                case Keys.D8:
                case Keys.NumPad8:
                    return 8;
                case Keys.D9:
                case Keys.NumPad9:
                    return 9;
            }
            return -1;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Select();
            if (e.Button == MouseButtons.Right)
            {
                lastX = e.X;
                lastY = e.Y;
                lastDeltaX = deltaX;
                lastDeltaY = deltaY;
            }
            if (e.Button == MouseButtons.Left)
            {
                int spacing;
                int xOffset;
                int yOffset;
                DetermineParams(out spacing, out xOffset, out yOffset);

                int xLoc = (e.X - xOffset) / spacing;
                int yLoc = (e.Y - yOffset) / spacing;
                try
                {
                    if (!EditMode)
                    {
                        Status curVal = Puzzle.Board[xLoc, yLoc];
                        int newVal = (int)curVal + 1;
                        if (newVal > (int)Status.Empty)
                            newVal = (int)Status.Unknown;
                        Puzzle.Board[xLoc, yLoc] = (Status)newVal;
                        lastCol = (Status)newVal;
                    }
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
        Status lastCol;
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
                if (!EditMode)
                {
                    int spacing;
                    int xOffset;
                    int yOffset;
                    DetermineParams(out spacing, out xOffset, out yOffset);

                    int xLoc = (e.X - xOffset) / spacing;
                    int yLoc = (e.Y - yOffset) / spacing;
                    if (xLoc != lastXSet || yLoc != lastYSet)
                    {
                        try
                        {
                            Puzzle.Board[xLoc, yLoc] = lastCol;

                            Refresh();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }

        internal void RaiseKeyUp(KeyEventArgs e)
        {
            OnKeyUp(e);
        }
    }
}
