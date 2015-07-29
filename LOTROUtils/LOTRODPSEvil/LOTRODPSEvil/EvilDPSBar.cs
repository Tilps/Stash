using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LOTRODPSEvil
{
    public partial class EvilDPSBar : Control
    {
        public EvilDPSBar()
        {
            InitializeComponent();
        }

        public int Max { get { return max; } set { max = value; this.Invalidate(); } }
        private int max = 200;
        public int Cur { get { return cur; } set { if (value == cur) return;  cur = value; if (cur > max) max = cur; this.Invalidate(); } }
        private int cur = 140;

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            pe.Graphics.FillRectangle(Brushes.Azure, 0, 0, (int)(this.Width * Percent(Cur, Max)), this.Height);
            string toShow = string.Format("{0}/{1}", Cur, Max);
            SizeF stringSize = pe.Graphics.MeasureString(toShow, this.Font);
            pe.Graphics.DrawString(toShow, this.Font, Brushes.Black, (this.Width-stringSize.Width)/2.0f, (this.Height-stringSize.Height)/2.0f);
        }

        private float Percent(int Cur, int Max)
        {
            if (Max == 0)
                return 1.0f;
            else
                return (float)Cur / (float)Max;
        }
    }
}
