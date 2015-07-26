using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FFClocks
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[] values = new int[textBoxes.Count];
            try
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = int.Parse(textBoxes[i].Text);
            }
            catch
            {
                MessageBox.Show("Clock text boxes need numbers in them!");
                return;
            }
            for (int i=0; i < values.Length; i++)
                if (values[i] > textBoxes.Count/2+1)
                    return;
            int[] order = Solve(values);
            for (int i = 0; i < values.Length; i++)
                textBoxes[i].Text = order[i].ToString();
        }

        private int[] Solve(int[] values)
        {
            List<int> order = new List<int>();
            for (int i = 0; i < values.Length; i++)
            {
                order.Add(i);
                if (Go(values, order))
                    break;
                order.Remove(i);
            }
            int[] output = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
                output[order[i]] = i;
            return output;
        }

        private bool Go(int[] values, List<int> order)
        {
            if (order.Count == values.Length)
                return true;
            int current = order.Last();
            int next1 = (current + values[current]) % values.Length;
            int next2 = (values.Length + current - values[current]) % values.Length;
            if (!order.Contains(next1))
            {
                order.Add(next1);
                if (Go(values, order))
                    return true;
                order.Remove(next1);
            }
            if (next1 != next2 && !order.Contains(next2))
            {
                order.Add(next2);
                if (Go(values, order))
                    return true;
                order.Remove(next2);
            }
            return false;
        }

        List<TextBox> textBoxes = new List<TextBox>();

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var tb in textBoxes)
                this.Controls.Remove(tb);
            textBoxes.Clear();
            try
            {
                int count = int.Parse(textBox1.Text);
                for (int i = 0; i < count; i++)
                {
                    var tb = new TextBox();
                    this.Controls.Add(tb);
                    double rad = 5 * count;
                    tb.Height = tb.Font.Height + 4;
                    tb.Width = tb.Height;
                    tb.Top = (int)(50 + rad - rad * Math.Cos(Math.PI * 2.0 / count * i));
                    tb.Left = (int)(50 + rad + rad * Math.Sin(Math.PI * 2.0 / count * i));
                    textBoxes.Add(tb);
                }
            }
            catch
            {
                MessageBox.Show("Need a number of clock face entries to layout.");
            }
        }
    }
}
