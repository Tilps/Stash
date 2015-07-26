using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void chart1_Click(object sender, EventArgs e)
        {
            int maxTimes = 100;
            CumulativeRandom rnd = new CumulativeRandom(maxTimes);
            int[] runLengthCounts = new int[maxTimes*100];
            int runCount = 0;
            int totalPoints = 100000*maxTimes;
            int totalRuns = 0;
            for (int i = 0; i < totalPoints; i++)
            {
                if (rnd.Try())
                {
                    totalRuns++;
                    runLengthCounts[runCount]++;
                    runCount = 0;
                }
                else
                {
                    runCount++;
                }
            }
            chart1.Series[0].Points.Clear();
            int max = Array.FindLastIndex(runLengthCounts, a => a > 0);
            for (int i = 0; i <= max; i++)
                chart1.Series[0].Points.AddXY(i, runLengthCounts[i]);
            chart1.Refresh();
            listBox1.Items.Clear();
            double[] thresholds = new double[] { 0.01, 0.05, 0.20, 0.50, 0.80, 0.95, 0.99};
            for (int j = 0; j < thresholds.Length; j++)
            {
                int total = 0;
                int targetTotal = (int)(thresholds[j]*totalRuns);
                for (int i = 0; i <= max; i++)
                {
                    total += runLengthCounts[i];
                    if (total >= targetTotal)
                    {
                        listBox1.Items.Add((thresholds[j]*100) + "%: " + (i+1));
                        break;
                    }
                }
            }

        }
    }
}
