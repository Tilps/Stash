using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Stamper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
            }
            else
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    button1.Text = "Stop Stamping";
                    backgroundWorker1.RunWorkerAsync(dialog.FileName);
                }
            }

        }

        DateTime start = DateTime.MinValue;

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                start = DateTime.Now;
                string filename = (string)e.Argument;
                long lastLoc = -1;
                int spins = 1;
                while (true)
                {
                    bool fastSpin = true;
                    using (FileStream stream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            fastSpin = ProcessFile(reader, lastLoc);
                            if (backgroundWorker1.CancellationPending)
                                break;
                            lastLoc = reader.BaseStream.Position;
                        }
                    }
                    if (!fastSpin)
                    {
                        System.Threading.Thread.Sleep(spins);
                        if (spins < 100)
                        {
                            spins++;
                        }
                    }
                    else
                    {
                        spins = 1;
                    }
                }
            }
            catch (Exception ex)
            {
            }

        }

        private bool ProcessFile(StreamReader reader, long lastLoc)
        {
            bool initialRead = true;
            // Skip whatever is in the file when it is selected.
            if (lastLoc == -1)
                reader.BaseStream.Seek(0, SeekOrigin.End);
            else
                reader.BaseStream.Seek(lastLoc, SeekOrigin.Begin);
            while (true)
            {
                long beforePos = reader.BaseStream.Position;
                string line = reader.ReadLine();
                long afterPos = reader.BaseStream.Position;
                if (string.IsNullOrEmpty(line))
                {
                    if (initialRead)
                    {
                        return false;
                    }
                    return true;
                }
                else if (beforePos + line.Length == afterPos)
                {
                }
                if (initialRead)
                {
                    initialRead = false;
                }

                if (checkBox1.Checked)
                {                    
                    lines.Add(DateTime.Now.Subtract(start).TotalSeconds.ToString("00000.00") + " " + line);
                }
                else
                {
                    lines.Add(DateTime.Now.ToString("hh:mm:ss.fff") + " " + line);
                }

                if (backgroundWorker1.CancellationPending)
                    break;
            }
            return true;

        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            button1.Text = "Start Stamping";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (lastLength == lines.Count)
                return;
            int oldLength = lastLength;
            lastLength = lines.Count;
            listBox1.BeginUpdate();
            for (int i = oldLength; i < lastLength; i++)
            {
                listBox1.Items.Add(lines[i]);
            }
            listBox1.EndUpdate();
        }

        int lastLength = 0;

        private List<string> lines = new List<string>();

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllLines(dialog.FileName, lines.ToArray());
            }
        }
    }
}
