namespace KwonTomCollector
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.labelNames = new System.Windows.Forms.Label();
            this.labelTimes = new System.Windows.Forms.Label();
            this.labelAction = new System.Windows.Forms.Label();
            this.labelWarning = new System.Windows.Forms.Label();
            this.labelWrongNames = new System.Windows.Forms.Label();
            this.labelWrongScores = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // labelNames
            // 
            this.labelNames.AutoSize = true;
            this.labelNames.Location = new System.Drawing.Point(12, 38);
            this.labelNames.Name = "labelNames";
            this.labelNames.Size = new System.Drawing.Size(40, 13);
            this.labelNames.TabIndex = 1;
            this.labelNames.Text = "Names";
            // 
            // labelTimes
            // 
            this.labelTimes.AutoSize = true;
            this.labelTimes.Location = new System.Drawing.Point(12, 61);
            this.labelTimes.Name = "labelTimes";
            this.labelTimes.Size = new System.Drawing.Size(38, 13);
            this.labelTimes.TabIndex = 2;
            this.labelTimes.Text = "Times:";
            // 
            // labelAction
            // 
            this.labelAction.AutoSize = true;
            this.labelAction.Location = new System.Drawing.Point(12, 88);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(40, 13);
            this.labelAction.TabIndex = 3;
            this.labelAction.Text = "Action:";
            // 
            // labelWarning
            // 
            this.labelWarning.AutoSize = true;
            this.labelWarning.Location = new System.Drawing.Point(12, 112);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(35, 13);
            this.labelWarning.TabIndex = 4;
            this.labelWarning.Text = "label1";
            // 
            // labelWrongNames
            // 
            this.labelWrongNames.AutoSize = true;
            this.labelWrongNames.Location = new System.Drawing.Point(139, 38);
            this.labelWrongNames.Name = "labelWrongNames";
            this.labelWrongNames.Size = new System.Drawing.Size(40, 13);
            this.labelWrongNames.TabIndex = 5;
            this.labelWrongNames.Text = "Names";
            // 
            // labelWrongScores
            // 
            this.labelWrongScores.AutoSize = true;
            this.labelWrongScores.Location = new System.Drawing.Point(139, 61);
            this.labelWrongScores.Name = "labelWrongScores";
            this.labelWrongScores.Size = new System.Drawing.Size(40, 13);
            this.labelWrongScores.TabIndex = 6;
            this.labelWrongScores.Text = "Names";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.labelWrongScores);
            this.Controls.Add(this.labelWrongNames);
            this.Controls.Add(this.labelWarning);
            this.Controls.Add(this.labelAction);
            this.Controls.Add(this.labelTimes);
            this.Controls.Add(this.labelNames);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label labelNames;
        private System.Windows.Forms.Label labelTimes;
        private System.Windows.Forms.Label labelAction;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.Label labelWrongNames;
        private System.Windows.Forms.Label labelWrongScores;
    }
}

