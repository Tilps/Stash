namespace LOTROChatNarrator
{
    partial class CombatSettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxReportPeriod = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPetNames = new System.Windows.Forms.TextBox();
            this.checkBoxOutgoingDPS = new System.Windows.Forms.CheckBox();
            this.checkBoxIncomingDPS = new System.Windows.Forms.CheckBox();
            this.checkBoxOutgoingPetDPS = new System.Windows.Forms.CheckBox();
            this.checkBoxIncomingPetDPS = new System.Windows.Forms.CheckBox();
            this.checkBoxIncomingHeals = new System.Windows.Forms.CheckBox();
            this.checkBoxOutgoingHeals = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDistributed = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBoxChatOnly = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Report Period:";
            // 
            // textBoxReportPeriod
            // 
            this.textBoxReportPeriod.Location = new System.Drawing.Point(90, 6);
            this.textBoxReportPeriod.Name = "textBoxReportPeriod";
            this.textBoxReportPeriod.Size = new System.Drawing.Size(100, 20);
            this.textBoxReportPeriod.TabIndex = 1;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(220, 441);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(301, 441);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Pet Names:";
            // 
            // textBoxPetNames
            // 
            this.textBoxPetNames.Location = new System.Drawing.Point(90, 32);
            this.textBoxPetNames.Name = "textBoxPetNames";
            this.textBoxPetNames.Size = new System.Drawing.Size(286, 20);
            this.textBoxPetNames.TabIndex = 5;
            // 
            // checkBoxOutgoingDPS
            // 
            this.checkBoxOutgoingDPS.AutoSize = true;
            this.checkBoxOutgoingDPS.Location = new System.Drawing.Point(15, 58);
            this.checkBoxOutgoingDPS.Name = "checkBoxOutgoingDPS";
            this.checkBoxOutgoingDPS.Size = new System.Drawing.Size(129, 17);
            this.checkBoxOutgoingDPS.TabIndex = 6;
            this.checkBoxOutgoingDPS.Text = "Report Outgoing DPS";
            this.checkBoxOutgoingDPS.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncomingDPS
            // 
            this.checkBoxIncomingDPS.AutoSize = true;
            this.checkBoxIncomingDPS.Location = new System.Drawing.Point(15, 81);
            this.checkBoxIncomingDPS.Name = "checkBoxIncomingDPS";
            this.checkBoxIncomingDPS.Size = new System.Drawing.Size(129, 17);
            this.checkBoxIncomingDPS.TabIndex = 7;
            this.checkBoxIncomingDPS.Text = "Report Incoming DPS";
            this.checkBoxIncomingDPS.UseVisualStyleBackColor = true;
            // 
            // checkBoxOutgoingPetDPS
            // 
            this.checkBoxOutgoingPetDPS.AutoSize = true;
            this.checkBoxOutgoingPetDPS.Location = new System.Drawing.Point(15, 104);
            this.checkBoxOutgoingPetDPS.Name = "checkBoxOutgoingPetDPS";
            this.checkBoxOutgoingPetDPS.Size = new System.Drawing.Size(148, 17);
            this.checkBoxOutgoingPetDPS.TabIndex = 8;
            this.checkBoxOutgoingPetDPS.Text = "Report Outgoing Pet DPS";
            this.checkBoxOutgoingPetDPS.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncomingPetDPS
            // 
            this.checkBoxIncomingPetDPS.AutoSize = true;
            this.checkBoxIncomingPetDPS.Location = new System.Drawing.Point(15, 127);
            this.checkBoxIncomingPetDPS.Name = "checkBoxIncomingPetDPS";
            this.checkBoxIncomingPetDPS.Size = new System.Drawing.Size(148, 17);
            this.checkBoxIncomingPetDPS.TabIndex = 9;
            this.checkBoxIncomingPetDPS.Text = "Report Incoming Pet DPS";
            this.checkBoxIncomingPetDPS.UseVisualStyleBackColor = true;
            // 
            // checkBoxIncomingHeals
            // 
            this.checkBoxIncomingHeals.AutoSize = true;
            this.checkBoxIncomingHeals.Location = new System.Drawing.Point(15, 151);
            this.checkBoxIncomingHeals.Name = "checkBoxIncomingHeals";
            this.checkBoxIncomingHeals.Size = new System.Drawing.Size(134, 17);
            this.checkBoxIncomingHeals.TabIndex = 10;
            this.checkBoxIncomingHeals.Text = "Report Incoming Heals";
            this.checkBoxIncomingHeals.UseVisualStyleBackColor = true;
            // 
            // checkBoxOutgoingHeals
            // 
            this.checkBoxOutgoingHeals.AutoSize = true;
            this.checkBoxOutgoingHeals.Location = new System.Drawing.Point(15, 174);
            this.checkBoxOutgoingHeals.Name = "checkBoxOutgoingHeals";
            this.checkBoxOutgoingHeals.Size = new System.Drawing.Size(134, 17);
            this.checkBoxOutgoingHeals.TabIndex = 11;
            this.checkBoxOutgoingHeals.Text = "Report Outgoing Heals";
            this.checkBoxOutgoingHeals.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 198);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(172, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Distributed Damage Annoucement:";
            // 
            // textBoxDistributed
            // 
            this.textBoxDistributed.Location = new System.Drawing.Point(190, 195);
            this.textBoxDistributed.Name = "textBoxDistributed";
            this.textBoxDistributed.Size = new System.Drawing.Size(145, 20);
            this.textBoxDistributed.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 259);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(219, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Time delay messages (match:delay:message)";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(10, 276);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(366, 160);
            this.textBox1.TabIndex = 15;
            // 
            // checkBoxChatOnly
            // 
            this.checkBoxChatOnly.AutoSize = true;
            this.checkBoxChatOnly.Location = new System.Drawing.Point(15, 226);
            this.checkBoxChatOnly.Name = "checkBoxChatOnly";
            this.checkBoxChatOnly.Size = new System.Drawing.Size(365, 17);
            this.checkBoxChatOnly.TabIndex = 16;
            this.checkBoxChatOnly.Text = "Only read out chat messages (assume everything else is combat related)";
            this.checkBoxChatOnly.UseVisualStyleBackColor = true;
            // 
            // CombatSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(388, 476);
            this.Controls.Add(this.checkBoxChatOnly);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxDistributed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBoxOutgoingHeals);
            this.Controls.Add(this.checkBoxIncomingHeals);
            this.Controls.Add(this.checkBoxIncomingPetDPS);
            this.Controls.Add(this.checkBoxOutgoingPetDPS);
            this.Controls.Add(this.checkBoxIncomingDPS);
            this.Controls.Add(this.checkBoxOutgoingDPS);
            this.Controls.Add(this.textBoxPetNames);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxReportPeriod);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "CombatSettingsForm";
            this.Text = "Advanced Combat Settings";
            this.Load += new System.EventHandler(this.CombatSettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxReportPeriod;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPetNames;
        private System.Windows.Forms.CheckBox checkBoxOutgoingDPS;
        private System.Windows.Forms.CheckBox checkBoxIncomingDPS;
        private System.Windows.Forms.CheckBox checkBoxOutgoingPetDPS;
        private System.Windows.Forms.CheckBox checkBoxIncomingPetDPS;
        private System.Windows.Forms.CheckBox checkBoxIncomingHeals;
        private System.Windows.Forms.CheckBox checkBoxOutgoingHeals;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDistributed;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBoxChatOnly;
    }
}