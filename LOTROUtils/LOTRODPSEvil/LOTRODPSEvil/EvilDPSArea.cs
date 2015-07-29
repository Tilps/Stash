using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace LOTRODPSEvil
{
    public partial class EvilDPSArea : UserControl
    {
        public EvilDPSArea()
        {
            InitializeComponent();
            evilDPSBarDOut.MouseHover += new EventHandler(evilDPSBarDIn_MouseHover);
            evilDPSBarDIn.MouseHover += new EventHandler(evilDPSBarDIn_MouseHover);
            evilDPSBarHOut.MouseHover += new EventHandler(evilDPSBarDIn_MouseHover);
            evilDPSBarHIn.MouseHover += new EventHandler(evilDPSBarDIn_MouseHover);
            evilDPSBarPDOut.MouseHover += new EventHandler(evilDPSBarDIn_MouseHover);
            evilDPSBarPDIn.MouseHover += new EventHandler(evilDPSBarDIn_MouseHover);
        }

        void evilDPSBarDIn_MouseHover(object sender, EventArgs e)
        {
            ShowTooltip();
        }

        internal void GetCurrent(out float dout, out float din, out float hout, out float hin, out float dpout, out float dpin)
        {
            dout = evilDPSBarDOut.Cur;
            din = evilDPSBarDIn.Cur;
            hout = evilDPSBarHOut.Cur;
            hin = evilDPSBarHIn.Cur;
            dpout = evilDPSBarPDOut.Cur;
            dpin = evilDPSBarPDIn.Cur;
        }

        internal void UpdateAsync(float dout, float din, float hout, float hin, float dpout, float dpin)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate() { UpdateAsync(dout, din, hout, hin, dpout, dpin); }));
                return;
            }
            evilDPSBarDOut.Cur = (int)dout;
            evilDPSBarDIn.Cur = (int)din;
            evilDPSBarHOut.Cur = (int)hout;
            evilDPSBarHIn.Cur = (int)hin;
            evilDPSBarPDOut.Cur = (int)dpout;
            evilDPSBarPDIn.Cur = (int)dpin;
            douts.Add((int)dout);
            Trim(douts);
            dins.Add((int)din);
            Trim(dins);
            houts.Add((int)hout);
            Trim(houts);
            hins.Add((int)hin);
            Trim(hins);
            dpouts.Add((int)dpout);
            Trim(dpouts);
            dpins.Add((int)dpin);
            Trim(dpins);
            if (currentGraph != null)
                currentGraph.Invalidate();
            if (Updated != null)
                Updated(this, EventArgs.Empty);
        }
        Random rnd = new Random();

        public event EventHandler Updated;

        private void Trim(List<int> list)
        {
            // Little bit of random to ensure not all lists trim in the same update.
            if (list.Count > 20000 + rnd.Next(0, 30))
            {
                list.RemoveRange(0, 5000);
            }
        }

        List<int> douts = new List<int>();
        List<int> dins = new List<int>();
        List<int> houts = new List<int>();
        List<int> hins = new List<int>();
        List<int> dpouts = new List<int>();
        List<int> dpins = new List<int>();


        private void ShowTooltip()
        {
            Point loc = Cursor.Position;
            Control child = this.GetChildAtPoint(this.PointToClient(loc));
            if (child is EvilDPSBar)
            {
                EvilDPSGraph g = GetGraphForBar(child);
                if (g.Samples.Count == 0)
                    return;
                ToolStripDropDown f = new ToolStripDropDown();
                f.Margin = Padding.Empty;
                f.Padding = Padding.Empty;
                ToolStripControlHost host = new ToolStripControlHost(g);
                host.Padding = Padding.Empty;
                host.Margin = Padding.Empty;
                f.Items.Add(host);
                f.Width = 300;
                f.Height = 80;
                g.Dock = DockStyle.Fill;
                host.Dock = DockStyle.Fill;
                g.Width = 300;
                g.Height = 80;
                f.Show(Cursor.Position);

                currentGraph = g;
            }
        }

        private EvilDPSGraph GetGraphForBar(Control child)
        {
            EvilDPSBar[] bars = new EvilDPSBar[] { evilDPSBarDOut, evilDPSBarDIn, evilDPSBarHOut, evilDPSBarHIn, evilDPSBarPDOut, evilDPSBarPDIn };
            List<int>[] samples = new List<int>[] { douts, dins, houts, hins, dpouts, dpins };
            EvilDPSGraph g = new EvilDPSGraph();
            int pos = Array.IndexOf(bars, child);
            if (pos >= 0)
                g.Samples = samples[pos];
            return g;
        }
        EvilDPSGraph currentGraph = null;


        private void checkBox_CheckedChanged(object sender, EventArgs e)
        {
            int curTop = 19;
            int increment = 20;
            Label[] labels = new Label[] { labelDOut, labelDIn, labelHOut, labelHIn, labelPDOut, labelPDIn };
            EvilDPSBar[] bars = new EvilDPSBar[] { evilDPSBarDOut, evilDPSBarDIn, evilDPSBarHOut, evilDPSBarHIn, evilDPSBarPDOut, evilDPSBarPDIn};
            CheckBox[] checkBoxes = new CheckBox[] { checkBox1, checkBox2, checkBox3, checkBox4, checkBox5, checkBox6 };

            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].Visible = checkBoxes[i].Checked;
                bars[i].Visible = checkBoxes[i].Checked;
                if (checkBoxes[i].Checked)
                {
                    labels[i].Top = curTop + 1;
                    bars[i].Top = curTop;
                    curTop += increment;
                }
            }
            
            this.Height = curTop + 5;
        }

        public string DisplayName { get { return label1.Text; } set { label1.Text = value; } }


        internal List<KeyValuePair<string, EvilDPSGraph>> GetShownAsGraph()
        {
            List<KeyValuePair<string, EvilDPSGraph>> result = new List<KeyValuePair<string, EvilDPSGraph>>();
            Label[] labels = new Label[] { labelDOut, labelDIn, labelHOut, labelHIn, labelPDOut, labelPDIn };
            EvilDPSBar[] bars = new EvilDPSBar[] { evilDPSBarDOut, evilDPSBarDIn, evilDPSBarHOut, evilDPSBarHIn, evilDPSBarPDOut, evilDPSBarPDIn };
            for (int i=0; i < labels.Length; i++)
            {
                if (bars[i].Visible)
                {
                    result.Add(new KeyValuePair<string, EvilDPSGraph>(labels[i].Text, GetGraphForBar(bars[i])));
                }
            }
            return result;
        }
    }
}
