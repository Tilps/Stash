namespace LOTRODPSEvil
{
    partial class DPSEvilForm
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
            this.evilDPSArea1 = new LOTRODPSEvil.EvilDPSArea();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Edit Settings";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // evilDPSArea1
            // 
            this.evilDPSArea1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.evilDPSArea1.DisplayName = "";
            this.evilDPSArea1.Location = new System.Drawing.Point(12, 41);
            this.evilDPSArea1.Name = "evilDPSArea1";
            this.evilDPSArea1.Size = new System.Drawing.Size(175, 144);
            this.evilDPSArea1.TabIndex = 7;
            this.evilDPSArea1.SizeChanged += new System.EventHandler(this.evilDPSArea1_SizeChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(93, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(81, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Show Graphs";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // DPSEvilForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(199, 185);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.evilDPSArea1);
            this.Controls.Add(this.button1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(2000, 223);
            this.MinimumSize = new System.Drawing.Size(215, 223);
            this.Name = "DPSEvilForm";
            this.Text = "DPS Evil";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private LOTRODPSEvil.EvilDPSArea evilDPSArea1;
        private System.Windows.Forms.Button button2;
    }
}

