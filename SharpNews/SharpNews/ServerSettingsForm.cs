using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SharpNews
{
    /// <summary>
    /// Provides a form for setting settings pertaining to NNTP servers.
    /// </summary>
    public partial class ServerSettingsForm : Form
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public ServerSettingsForm()
        {
            InitializeComponent();
        }

        private void authCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            usernameTextBox.ReadOnly = !authCheckBox.Checked;
            passwordTextBox.ReadOnly = !authCheckBox.Checked;
        }

        private ServerSettings settings = new ServerSettings();

        private void ServerSettingsForm_Load(object sender, EventArgs e)
        {
            settings.Load();
            authCheckBox.Checked = settings.Username != null;
            if (settings.Username != null)
            {
                usernameTextBox.Text = settings.Username;
                passwordTextBox.Text = settings.Password;
            }
            hostnameTextBox.Text = settings.Hostname;
            if (settings.Port != -1)
            {
                portTextBox.Text = settings.Port.ToString();
            }
            else
                portTextBox.Text = string.Empty;    
            nameTextBox.Text = settings.Name;
            maxConnectTextBox.Text = settings.MaxConnections.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (authCheckBox.Checked)
            {
                settings.Username = usernameTextBox.Text;
                settings.Password = passwordTextBox.Text;
            }
            else
            {
                settings.Username = null;
                settings.Password = null;
            }
            settings.Hostname = hostnameTextBox.Text;
            settings.Name = nameTextBox.Text;
            int temp = -1;
            int.TryParse(portTextBox.Text, out temp);
            settings.Port = temp;
            temp = 10;
            int.TryParse(maxConnectTextBox.Text, out temp);
            settings.MaxConnections = temp;
            settings.Save();
            this.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}