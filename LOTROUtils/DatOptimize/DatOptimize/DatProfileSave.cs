using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace DatOptimize
{
    public partial class DatProfileSave : Form
    {
        public DatProfileSave()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        public Dictionary<string, List<int>> Mapping { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.OverwritePrompt = true;
            dialog.CheckPathExists = true;
            dialog.Filter = "Dat Profiles|*.datprofile";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = dialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("You must specify a save file name.");
                DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(textBox2.Text)))
                {
                    MessageBox.Show("The selected save folder does not exist.");
                    DialogResult = System.Windows.Forms.DialogResult.None;
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Save file name is invalid.");
                DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }
            try
            {
                using (StreamWriter writer = File.CreateText(textBox2.Text))
                {
                    writer.WriteLine("1");
                    writer.WriteLine(textBox1.Text);
                    writer.WriteLine((string)comboBox1.SelectedItem);
                    foreach (var entry in Mapping)
                    {
                        writer.WriteLine(entry.Key);
                        foreach (int id in entry.Value)
                            writer.WriteLine(id);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Save failed. Error details: {0}", ex.ToString()));
                DialogResult = System.Windows.Forms.DialogResult.None;
                return;
            }
        }
    }
}
