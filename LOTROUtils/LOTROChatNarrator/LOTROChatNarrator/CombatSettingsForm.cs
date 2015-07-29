using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LOTROChatNarrator.Properties;

namespace LOTROChatNarrator
{
    public partial class CombatSettingsForm : Form
    {
        public CombatSettingsForm()
        {
            InitializeComponent();
        }

        private void CombatSettingsForm_Load(object sender, EventArgs e)
        {
            checkBoxIncomingDPS.Checked = Settings.Default.ReportDPSIn;
            checkBoxOutgoingDPS.Checked = Settings.Default.ReportDPS;
            checkBoxIncomingPetDPS.Checked = Settings.Default.ReportPetDPSIn;
            checkBoxOutgoingPetDPS.Checked = Settings.Default.ReportPetDPS;
            checkBoxIncomingHeals.Checked = Settings.Default.ReportHealingIn;
            checkBoxOutgoingHeals.Checked = Settings.Default.ReportHealingOut;
            textBoxPetNames.Text = Settings.Default.PetNames;
            textBoxReportPeriod.Text = Settings.Default.CombatReportPeriod.ToString();
            textBoxDistributed.Text = Settings.Default.DistributedAnnounce;
            checkBoxChatOnly.Checked = Settings.Default.AssumeNonChatIsCombat;
            textBox1.Text = Settings.Default.TimeDelayMessages;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Settings.Default.ReportPetDPSIn = checkBoxIncomingDPS.Checked;
            Settings.Default.ReportDPS = checkBoxOutgoingDPS.Checked;
            Settings.Default.ReportPetDPSIn = checkBoxIncomingPetDPS.Checked;
            Settings.Default.ReportPetDPS = checkBoxOutgoingPetDPS.Checked;
            Settings.Default.ReportHealingIn = checkBoxIncomingHeals.Checked;
            Settings.Default.ReportHealingOut = checkBoxOutgoingHeals.Checked;
            Settings.Default.PetNames = textBoxPetNames.Text;
            int period;
            if (!int.TryParse(textBoxReportPeriod.Text, out period) || period < 1)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.None;
                MessageBox.Show("Report Period must be a positive number.");
                return;
            }

            Settings.Default.CombatReportPeriod = period;
            Settings.Default.DistributedAnnounce = textBoxDistributed.Text;
            Settings.Default.AssumeNonChatIsCombat = checkBoxChatOnly.Checked;
            Settings.Default.TimeDelayMessages = textBox1.Text;
            Settings.Default.Save();
        }
    }
}
