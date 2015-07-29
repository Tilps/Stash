using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DatHash
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
            dialog.Description = "Select LOTRO installation location.";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                HashForm f = new HashForm();
                f.TargetDirectory = dialog.SelectedPath;
                f.ShowDialog();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select LOTRO installation location.";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveFileDialog dialog2 = new SaveFileDialog();
                dialog2.Title = "Select where to save template";
                dialog2.Filter = "Template|*.dattemplate";
                if (dialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ProgressFrom f = new ProgressFrom();
                    f.TargetDirectory = dialog.SelectedPath;
                    f.TargetFile = dialog2.FileName;
                    f.ShowDialog();
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select LOTRO installation location.";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OpenFileDialog dialog2 = new OpenFileDialog();
                dialog2.Title = "Select where to save template";
                dialog2.Filter = "Template|*.dattemplate";
                if (dialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CompareForm f = new CompareForm();
                    f.TargetDirectory = dialog.SelectedPath;
                    f.TargetFile = dialog2.FileName;
                    f.ShowDialog();
                }
            }

        }
    }
}
