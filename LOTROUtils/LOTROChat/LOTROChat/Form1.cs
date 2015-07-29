using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace LOTROChat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        LOTROGateway gateway;
        private void button1_Click(object sender, EventArgs e)
        {
            gateway = new LOTROGateway("talk.google.com", "gmail.com", textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, true);
        }


    }
}
