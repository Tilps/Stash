namespace WTTMW
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelAttemptsToday = new System.Windows.Forms.Label();
            this.labelAttemptSetting = new System.Windows.Forms.Label();
            this.labelAttemptsEver = new System.Windows.Forms.Label();
            this.labelCorrectToday = new System.Windows.Forms.Label();
            this.labelCorrectSettings = new System.Windows.Forms.Label();
            this.labelCorrectEver = new System.Windows.Forms.Label();
            this.labelPercentToday = new System.Windows.Forms.Label();
            this.labelPercentSettings = new System.Windows.Forms.Label();
            this.labelPercentEver = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(197, 92);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(65, 9);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Min";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Max";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(65, 35);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Spacing";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(64, 61);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 20);
            this.textBox3.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Attempts";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(84, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Today";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(142, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Latest Settings";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(226, 123);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(29, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Ever";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 185);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "Correct";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 227);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Percent";
            // 
            // labelAttemptsToday
            // 
            this.labelAttemptsToday.AutoSize = true;
            this.labelAttemptsToday.Location = new System.Drawing.Point(84, 149);
            this.labelAttemptsToday.Name = "labelAttemptsToday";
            this.labelAttemptsToday.Size = new System.Drawing.Size(41, 13);
            this.labelAttemptsToday.TabIndex = 13;
            this.labelAttemptsToday.Text = "label10";
            // 
            // labelAttemptSetting
            // 
            this.labelAttemptSetting.AutoSize = true;
            this.labelAttemptSetting.Location = new System.Drawing.Point(159, 149);
            this.labelAttemptSetting.Name = "labelAttemptSetting";
            this.labelAttemptSetting.Size = new System.Drawing.Size(41, 13);
            this.labelAttemptSetting.TabIndex = 14;
            this.labelAttemptSetting.Text = "label11";
            // 
            // labelAttemptsEver
            // 
            this.labelAttemptsEver.AutoSize = true;
            this.labelAttemptsEver.Location = new System.Drawing.Point(226, 149);
            this.labelAttemptsEver.Name = "labelAttemptsEver";
            this.labelAttemptsEver.Size = new System.Drawing.Size(41, 13);
            this.labelAttemptsEver.TabIndex = 15;
            this.labelAttemptsEver.Text = "label12";
            // 
            // labelCorrectToday
            // 
            this.labelCorrectToday.AutoSize = true;
            this.labelCorrectToday.Location = new System.Drawing.Point(84, 185);
            this.labelCorrectToday.Name = "labelCorrectToday";
            this.labelCorrectToday.Size = new System.Drawing.Size(41, 13);
            this.labelCorrectToday.TabIndex = 16;
            this.labelCorrectToday.Text = "label13";
            // 
            // labelCorrectSettings
            // 
            this.labelCorrectSettings.AutoSize = true;
            this.labelCorrectSettings.Location = new System.Drawing.Point(159, 185);
            this.labelCorrectSettings.Name = "labelCorrectSettings";
            this.labelCorrectSettings.Size = new System.Drawing.Size(41, 13);
            this.labelCorrectSettings.TabIndex = 17;
            this.labelCorrectSettings.Text = "label14";
            // 
            // labelCorrectEver
            // 
            this.labelCorrectEver.AutoSize = true;
            this.labelCorrectEver.Location = new System.Drawing.Point(226, 185);
            this.labelCorrectEver.Name = "labelCorrectEver";
            this.labelCorrectEver.Size = new System.Drawing.Size(41, 13);
            this.labelCorrectEver.TabIndex = 18;
            this.labelCorrectEver.Text = "label15";
            // 
            // labelPercentToday
            // 
            this.labelPercentToday.AutoSize = true;
            this.labelPercentToday.Location = new System.Drawing.Point(84, 227);
            this.labelPercentToday.Name = "labelPercentToday";
            this.labelPercentToday.Size = new System.Drawing.Size(41, 13);
            this.labelPercentToday.TabIndex = 19;
            this.labelPercentToday.Text = "label16";
            // 
            // labelPercentSettings
            // 
            this.labelPercentSettings.AutoSize = true;
            this.labelPercentSettings.Location = new System.Drawing.Point(159, 227);
            this.labelPercentSettings.Name = "labelPercentSettings";
            this.labelPercentSettings.Size = new System.Drawing.Size(41, 13);
            this.labelPercentSettings.TabIndex = 20;
            this.labelPercentSettings.Text = "label17";
            // 
            // labelPercentEver
            // 
            this.labelPercentEver.AutoSize = true;
            this.labelPercentEver.Location = new System.Drawing.Point(226, 227);
            this.labelPercentEver.Name = "labelPercentEver";
            this.labelPercentEver.Size = new System.Drawing.Size(41, 13);
            this.labelPercentEver.TabIndex = 21;
            this.labelPercentEver.Text = "label18";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 321);
            this.Controls.Add(this.labelPercentEver);
            this.Controls.Add(this.labelPercentSettings);
            this.Controls.Add(this.labelPercentToday);
            this.Controls.Add(this.labelCorrectEver);
            this.Controls.Add(this.labelCorrectSettings);
            this.Controls.Add(this.labelCorrectToday);
            this.Controls.Add(this.labelAttemptsEver);
            this.Controls.Add(this.labelAttemptSetting);
            this.Controls.Add(this.labelAttemptsToday);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "What\'s the time mr wolf?";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelAttemptsToday;
        private System.Windows.Forms.Label labelAttemptSetting;
        private System.Windows.Forms.Label labelAttemptsEver;
        private System.Windows.Forms.Label labelCorrectToday;
        private System.Windows.Forms.Label labelCorrectSettings;
        private System.Windows.Forms.Label labelCorrectEver;
        private System.Windows.Forms.Label labelPercentToday;
        private System.Windows.Forms.Label labelPercentSettings;
        private System.Windows.Forms.Label labelPercentEver;
    }
}

