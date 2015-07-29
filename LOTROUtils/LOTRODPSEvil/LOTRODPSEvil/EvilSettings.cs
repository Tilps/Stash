using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LOTRODPSEvil
{
    public partial class EvilSettings : Form
    {
        public EvilSettings()
        {
            InitializeComponent();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox4.ReadOnly = !radioButton2.Checked;
            checkBox1.Enabled = radioButton2.Checked;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            textBox5.ReadOnly = !radioButton3.Checked;
            textBox6.ReadOnly = !radioButton3.Checked;
            textBox7.ReadOnly = !radioButton3.Checked;
        }

        public string PetNames
        {
            get
            {
                return textBox3.Text;
            }
            set { textBox3.Text = value; }
        }

        public bool DedicatedServerMode
        {
            get
            {
                return checkBox1.Checked;
            }
            set
            {
                checkBox1.Checked = value;
            }
        }

        public int UpdatePeriod
        {
            get
            {
                double value;
                if (!double.TryParse(textBox1.Text, out value) || value <= 1.0)
                    value = 1.0;
                return (int)(1000 * value);
            }
            set
            {
                textBox1.Text = (value / 1000.0f).ToString();
            }
        }

        public int AveragePeriod
        {
            get
            {
                double value;
                if (!double.TryParse(textBox2.Text, out value) || value <= 1.0)
                    value = 15.0;
                return (int)(1000 * value);
            }
            set
            {
                textBox2.Text = (value / 1000.0f).ToString();
            }
        }

        public int ServerPort
        {
            get
            {
                if (radioButton2.Checked)
                {
                    ushort value;
                    if (!ushort.TryParse(textBox4.Text, out value))
                        return -1;
                    return value;
                }
                return -1;
            }
            set
            {
                textBox4.Text = value == -1 ? string.Empty : value.ToString();
                radioButton2.Checked = value != -1;
            }
        }

        public string RemoteHost
        {
            get
            {
                if (radioButton3.Checked)
                {
                    return textBox5.Text;
                }
                else
                    return null;
            }
            set
            {
                if (value != null)
                {
                    textBox5.Text = value;
                }
                else
                {
                    textBox5.Text = string.Empty;
                }
                radioButton3.Checked = value != null;
            }
        }

        public int RemotePort
        {
            get
            {
                if (radioButton3.Checked)
                {
                    ushort value;
                    if (!ushort.TryParse(textBox6.Text, out value))
                        return -1;
                    return value;
                }
                return -1;
            }
            set
            {
                textBox6.Text = value == -1 ? string.Empty : value.ToString();
                radioButton3.Checked = value != -1;
            }
        }

        public string RemoteName
        {
            get
            {
                if (radioButton3.Checked)
                {
                    return textBox7.Text;
                }
                else
                    return null;
            }
            set
            {
                if (value != null)
                {
                    textBox7.Text = value;
                }
                else
                {
                    textBox7.Text = string.Empty;
                }
                radioButton3.Checked = value != null;
            }
        }

        public string CharacterName
        {
            get
            {
                return textBox8.Text;
            }
            set
            {
                textBox8.Text = value;
            }
        }
    }
}
