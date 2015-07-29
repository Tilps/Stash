namespace LOTROChatNarrator
{
    partial class ChatNarratorMainForm
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
            this.buttonSpeak = new System.Windows.Forms.Button();
            this.checkBoxFilterSelf = new System.Windows.Forms.CheckBox();
            this.checkBoxFilterChannels = new System.Windows.Forms.CheckBox();
            this.textBoxSubstitutions = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBoxIncludeOnly = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBoxRegex = new System.Windows.Forms.CheckBox();
            this.checkBoxCombat = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonSpeak
            // 
            this.buttonSpeak.Location = new System.Drawing.Point(13, 13);
            this.buttonSpeak.Name = "buttonSpeak";
            this.buttonSpeak.Size = new System.Drawing.Size(92, 23);
            this.buttonSpeak.TabIndex = 0;
            this.buttonSpeak.Text = "Start Speaking";
            this.buttonSpeak.UseVisualStyleBackColor = true;
            this.buttonSpeak.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBoxFilterSelf
            // 
            this.checkBoxFilterSelf.AutoSize = true;
            this.checkBoxFilterSelf.Location = new System.Drawing.Point(13, 43);
            this.checkBoxFilterSelf.Name = "checkBoxFilterSelf";
            this.checkBoxFilterSelf.Size = new System.Drawing.Size(158, 17);
            this.checkBoxFilterSelf.TabIndex = 2;
            this.checkBoxFilterSelf.Text = "Filter out messages from self";
            this.checkBoxFilterSelf.UseVisualStyleBackColor = true;
            this.checkBoxFilterSelf.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBoxFilterChannels
            // 
            this.checkBoxFilterChannels.AutoSize = true;
            this.checkBoxFilterChannels.Location = new System.Drawing.Point(13, 67);
            this.checkBoxFilterChannels.Name = "checkBoxFilterChannels";
            this.checkBoxFilterChannels.Size = new System.Drawing.Size(145, 17);
            this.checkBoxFilterChannels.TabIndex = 3;
            this.checkBoxFilterChannels.Text = "Don\'t say channel names";
            this.checkBoxFilterChannels.UseVisualStyleBackColor = true;
            this.checkBoxFilterChannels.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // textBoxSubstitutions
            // 
            this.textBoxSubstitutions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSubstitutions.Location = new System.Drawing.Point(11, 129);
            this.textBoxSubstitutions.Multiline = true;
            this.textBoxSubstitutions.Name = "textBoxSubstitutions";
            this.textBoxSubstitutions.Size = new System.Drawing.Size(270, 132);
            this.textBoxSubstitutions.TabIndex = 4;
            this.textBoxSubstitutions.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Replacements (word:message):";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(200, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Change Voice";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // checkBoxIncludeOnly
            // 
            this.checkBoxIncludeOnly.AutoSize = true;
            this.checkBoxIncludeOnly.Location = new System.Drawing.Point(11, 434);
            this.checkBoxIncludeOnly.Name = "checkBoxIncludeOnly";
            this.checkBoxIncludeOnly.Size = new System.Drawing.Size(172, 17);
            this.checkBoxIncludeOnly.TabIndex = 6;
            this.checkBoxIncludeOnly.Text = "Use above as white list instead";
            this.checkBoxIncludeOnly.UseVisualStyleBackColor = true;
            this.checkBoxIncludeOnly.CheckedChanged += new System.EventHandler(this.checkBoxIncludeOnly_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 264);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Exclusions:";
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 280);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(268, 148);
            this.textBox1.TabIndex = 5;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // checkBoxRegex
            // 
            this.checkBoxRegex.AutoSize = true;
            this.checkBoxRegex.Location = new System.Drawing.Point(11, 458);
            this.checkBoxRegex.Name = "checkBoxRegex";
            this.checkBoxRegex.Size = new System.Drawing.Size(264, 17);
            this.checkBoxRegex.TabIndex = 7;
            this.checkBoxRegex.Text = "Use advanced matching for exclusions (inclusions)";
            this.checkBoxRegex.UseVisualStyleBackColor = true;
            this.checkBoxRegex.CheckedChanged += new System.EventHandler(this.checkBoxRegex_CheckedChanged);
            // 
            // checkBoxCombat
            // 
            this.checkBoxCombat.AutoSize = true;
            this.checkBoxCombat.Location = new System.Drawing.Point(13, 90);
            this.checkBoxCombat.Name = "checkBoxCombat";
            this.checkBoxCombat.Size = new System.Drawing.Size(135, 17);
            this.checkBoxCombat.TabIndex = 8;
            this.checkBoxCombat.Text = "Enable combat support";
            this.checkBoxCombat.UseVisualStyleBackColor = true;
            this.checkBoxCombat.CheckedChanged += new System.EventHandler(this.checkBoxCombat_CheckedChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(154, 86);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Advanced";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ChatNarratorMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 481);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.checkBoxCombat);
            this.Controls.Add(this.checkBoxRegex);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkBoxIncludeOnly);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSubstitutions);
            this.Controls.Add(this.checkBoxFilterChannels);
            this.Controls.Add(this.checkBoxFilterSelf);
            this.Controls.Add(this.buttonSpeak);
            this.MaximumSize = new System.Drawing.Size(20000, 519);
            this.MinimumSize = new System.Drawing.Size(310, 519);
            this.Name = "ChatNarratorMainForm";
            this.Text = "LOTRO Chat Narrator";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LogSpeakMainForm_FormClosed);
            this.Load += new System.EventHandler(this.LogSpeakMainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonSpeak;
        private System.Windows.Forms.CheckBox checkBoxFilterSelf;
        private System.Windows.Forms.CheckBox checkBoxFilterChannels;
        private System.Windows.Forms.TextBox textBoxSubstitutions;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxIncludeOnly;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBoxRegex;
        private System.Windows.Forms.CheckBox checkBoxCombat;
        private System.Windows.Forms.Button button2;
    }
}

