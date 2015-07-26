using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AppTimeTracker
{
    public partial class DetailedStats : Form
    {
        public DetailedStats()
        {
            InitializeComponent();
        }

        public List<ProcessStats> Stats
        {
            get
            {
                return stats;
            }
            set
            {
                stats = value;
            }
        }
        private List<ProcessStats> stats;

        private TimeSpan mergeInterval = TimeSpan.FromMinutes(1.5);

        private void DetailedStats_Load(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            listView1.Items.Clear();
            foreach (ProcessStats stat in stats)
            {
                for (int i = 0; i < stat.ForegroundTitles.Count; i++)
                {
                    if (i > 0 && stat.ForegroundTitles[i] == stat.ForegroundTitles[i - 1] && stat.ForegroundWhens[i].Subtract(stat.ForegroundWhens[i - 1]) < mergeInterval)
                    {
                        ListViewItem lastItem = listView1.Items[listView1.Items.Count - 1];
                        lastItem.SubItems[2].Text = (int.Parse(lastItem.SubItems[2].Text) + 1).ToString();
                    }
                    else
                    {
                        listView1.Items.Add(new ListViewItem(new string[] { stat.ForegroundTitles[i], stat.ForegroundWhens[i].ToString(), 1.ToString() }));
                    }
                }
            }
            listView1.EndUpdate();
        }
    }
}