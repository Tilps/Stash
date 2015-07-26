using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace HashCalc
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                SHA1 sha = SHA1.Create();
                byte[] hash = sha.ComputeHash(File.ReadAllBytes(dialog.FileName));
                MessageBox.Show(ToString(hash));
            }
        }

        private string ToString(byte[] hash)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in hash)
                builder.Append(b.ToString("X2"));
            return builder.ToString();
        }
    }
}
