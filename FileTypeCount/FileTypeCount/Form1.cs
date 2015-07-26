using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FileTypeCount
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Dictionary<string, int> counts = new Dictionary<string, int>();
                Gather(dialog.SelectedPath, counts);
                listBox1.BeginUpdate();
                listBox1.Items.Clear();
                foreach (var kvp in counts.OrderBy(a=>a.Value))
                {
                    listBox1.Items.Add(kvp.Key + ":" + kvp.Value);
                }
                listBox1.EndUpdate();
            }
        }

        private void Gather(string p, Dictionary<string, int> counts)
        {
            if (Directory.Exists(p))
            {
                try
                {
                    foreach (var entry in Directory.GetFileSystemEntries(p))
                        Gather(entry, counts);
                }
                catch
                {
                }
            }
            else
            {
                string key = Path.GetExtension(p);
                int existing = 0;
                counts.TryGetValue(key, out existing);
                counts[key] = existing + 1;
            }
        }
    }
}
