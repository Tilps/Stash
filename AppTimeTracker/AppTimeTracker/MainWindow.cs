using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AppTimeTracker
{
    public partial class MainWindow : Form
    {
        /// <summary>
        /// The GetForegroundWindow function returns a handle to the foreground window.
        /// </summary>
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public MainWindow()
        {
            InitializeComponent();
        }
        Dictionary<int, ProcessStats> stats = new Dictionary<int, ProcessStats>();
        Dictionary<string, List<int>> statsByName = new Dictionary<string, List<int>>();
        Dictionary<string, bool> ignore = new Dictionary<string, bool>();
        int lastFocused = -1;
        int lastFocusedCount = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcesses();
            IntPtr foregroundWindow = GetForegroundWindow();
            uint focusedProcess = 0;
            GetWindowThreadProcessId(foregroundWindow, out focusedProcess);
            foreach (Process p in processes)
            {
                if (ignore.ContainsKey(p.ProcessName))
                    continue;
                try
                {
                    if (p.MainWindowHandle == IntPtr.Zero)
                        continue;
                    if (!stats.ContainsKey(p.Id))
                    {
                        stats[p.Id] = new ProcessStats();
                        if (!statsByName.ContainsKey(p.ProcessName))
                            statsByName[p.ProcessName] = new List<int>();
                        statsByName[p.ProcessName].Add(p.Id);
                    }
                    stats[p.Id].Count = stats[p.Id].Count + 1;
                    if (p.Id == focusedProcess)
                    {
                        stats[p.Id].ForegroundCount = stats[p.Id].ForegroundCount + 1;
                        stats[p.Id].ForegroundWhens.Add(DateTime.UtcNow);
                        stats[p.Id].ForegroundTitles.Add(GetTitle(foregroundWindow));
                        if (lastFocused == p.Id)
                        {
                            lastFocusedCount++;
                            if (lastFocusedCount > GetThreshold(p.ProcessName))
                            {
                                notifyIcon1.BalloonTipTitle = "Excessive App usage detected.";
                                notifyIcon1.BalloonTipText = string.Format("The same app has been focused on for the last {0} minutes.", lastFocusedCount);
                                notifyIcon1.BalloonTipIcon = ToolTipIcon.Error;
                                notifyIcon1.ShowBalloonTip(10000);
                            }
                        }
                        else
                        {
                            lastFocusedCount = 0;
                            lastFocused = p.Id;
                        }
                    }
                }
                catch
                {
                }
            }
            if (this.Visible)
                UpdateUI();
        }

        private string GetTitle(IntPtr foregroundWindow)
        {
            int length = GetWindowTextLength(foregroundWindow);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(foregroundWindow, sb, sb.Capacity);
            return sb.ToString();
        }

        private int GetThreshold(string p)
        {
            return 30;
        }

        private void UpdateUI()
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            foreach (KeyValuePair<string, List<int>> statSet in statsByName)
            {
                ProcessStats accumulated = Accumulate(statSet.Value);
                listView1.Items.Add(new ListViewItem(new string[] {statSet.Key, accumulated.Count.ToString(), accumulated.ForegroundCount.ToString() }));
            }
            listView1.EndUpdate();
        }

        private ProcessStats Accumulate(List<int> list)
        {
            ProcessStats accumulated = new ProcessStats();
            foreach (int pid in list)
            {
                ProcessStats stat = stats[pid];
                accumulated.Count += stat.Count;
                accumulated.ForegroundCount += stat.ForegroundCount;
            }
            return accumulated;
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.Focus();
            this.ShowInTaskbar = true;
            UpdateUI();
        }

        private bool realExit = false;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (realExit)
                return;
            this.Hide();
            e.Cancel = true;
            this.ShowInTaskbar = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1_Tick(this, EventArgs.Empty);
            timer1.Enabled = false;
            timer1.Enabled = true;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            realExit = true;
            Application.Exit();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
                return;
            DetailedStats statsDisplay = new DetailedStats();
            ListViewItem item = listView1.SelectedItems[0];
            List<int> stats = statsByName[item.Text];
            List<ProcessStats> resolvedStats = new List<ProcessStats>();
            foreach (int stat in stats)
                resolvedStats.Add(this.stats[stat]);
            statsDisplay.Stats = resolvedStats;
            statsDisplay.ShowDialog(this);
        }
    }

    public class ProcessStats
    {
        public int ForegroundCount;
        public int Count;
        public List<DateTime> ForegroundWhens = new List<DateTime>();
        public List<string> ForegroundTitles = new List<string>();
    }
}