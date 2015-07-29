using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LOTRODPSEvil
{
    public partial class EvilDPSGraph : Control
    {
        public EvilDPSGraph()
        {
            InitializeComponent();
        }

        public List<int> Samples { get { return samples; } set { samples = value; Invalidate(); } }
        private List<int> samples = new List<int>();

        private int samplesPerPixel = 1;

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            HandleClick();
        }

        private void HandleClick()
        {
            samplesPerPixel = samplesPerPixel * 2;
            if (samplesPerPixel > 32)
                samplesPerPixel = 1;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            // right margin = 50
            int chartArea = this.Width - 50;
            int totalSamples = chartArea * samplesPerPixel;
            if (totalSamples > samples.Count)
                totalSamples = samples.Count;
            int maxOverSamples = 0;
            for (int i = 0; i < totalSamples; i++)
            {
                if (samples[samples.Count - 1 - i] > maxOverSamples)
                    maxOverSamples = samples[samples.Count - 1 - i];
            }
            pe.Graphics.DrawString(maxOverSamples.ToString(), this.Font, Brushes.Black, this.Width-49, 0);
            int sumNonZero = 0;
            int countNonZero = 0;
            for (int i = 0; i < this.Width-50 ; i++)
            {
                if (i < totalSamples / samplesPerPixel)
                {
                    int average = 0;
                    for (int j = 0; j < samplesPerPixel; j++)
                    {
                        average += samples[samples.Count - 1 - i * samplesPerPixel - j];
                        if (samples[samples.Count - 1 - i * samplesPerPixel - j] > 0)
                        {
                            sumNonZero += samples[samples.Count - 1 - i * samplesPerPixel - j];
                            countNonZero++;
                        }
                    }
                    average /= samplesPerPixel;
                    pe.Graphics.DrawLine(Pens.SkyBlue, this.Width - 50 - i, (this.Height * (maxOverSamples - average) / Math.Max(1, maxOverSamples)), this.Width - 50 - i, this.Height);
                }
                int mod = samplesPerPixel < 16 ? (60 / samplesPerPixel) : (64 / samplesPerPixel);
                if (i % mod == 0)
                    pe.Graphics.DrawLine(Pens.Black, this.Width - 50 - i, (this.Height - 4), this.Width - 50 - i, this.Height);
            }
            if (countNonZero > 0)
            {
                int averageNonZero = sumNonZero / countNonZero;
                if (averageNonZero > 0 && averageNonZero < maxOverSamples)
                {
                    int y = this.Height * (maxOverSamples-averageNonZero) / (Math.Max(1, maxOverSamples));
                    pe.Graphics.DrawLine(Pens.Red, 0, y, this.Width - 50, y);
                    int stringHeight =(int)pe.Graphics.MeasureString(averageNonZero.ToString(), this.Font).Height;
                    pe.Graphics.DrawString(averageNonZero.ToString(), this.Font, Brushes.Red, this.Width-49, this.Height/2-stringHeight/2);
                }
            }
        }

        internal void SimulClick()
        {
            HandleClick();
        }
    }
}
