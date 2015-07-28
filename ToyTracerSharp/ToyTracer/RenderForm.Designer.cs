namespace ToyTracer
{
    partial class RenderForm
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
            this.filenameTextBox = new System.Windows.Forms.TextBox();
            this.fileLabel = new System.Windows.Forms.Label();
            this.fileButton = new System.Windows.Forms.Button();
            this.renderButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.drawPanel = new System.Windows.Forms.Panel();
            this.outputPictureBox = new System.Windows.Forms.PictureBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.drawPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.outputPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // filenameTextBox
            // 
            this.filenameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filenameTextBox.Location = new System.Drawing.Point(62, 6);
            this.filenameTextBox.Name = "filenameTextBox";
            this.filenameTextBox.Size = new System.Drawing.Size(349, 20);
            this.filenameTextBox.TabIndex = 0;
            // 
            // fileLabel
            // 
            this.fileLabel.AutoSize = true;
            this.fileLabel.Location = new System.Drawing.Point(12, 9);
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(44, 13);
            this.fileLabel.TabIndex = 1;
            this.fileLabel.Text = "Tal File:";
            // 
            // fileButton
            // 
            this.fileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fileButton.Location = new System.Drawing.Point(417, 4);
            this.fileButton.Name = "fileButton";
            this.fileButton.Size = new System.Drawing.Size(25, 23);
            this.fileButton.TabIndex = 2;
            this.fileButton.Text = "...";
            this.fileButton.UseVisualStyleBackColor = true;
            this.fileButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // renderButton
            // 
            this.renderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.renderButton.Location = new System.Drawing.Point(448, 4);
            this.renderButton.Name = "renderButton";
            this.renderButton.Size = new System.Drawing.Size(75, 23);
            this.renderButton.TabIndex = 3;
            this.renderButton.Text = "Render";
            this.renderButton.UseVisualStyleBackColor = true;
            this.renderButton.Click += new System.EventHandler(this.renderButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 553);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(593, 23);
            this.progressBar.TabIndex = 4;
            // 
            // drawPanel
            // 
            this.drawPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.drawPanel.Controls.Add(this.outputPictureBox);
            this.drawPanel.Location = new System.Drawing.Point(12, 32);
            this.drawPanel.Name = "drawPanel";
            this.drawPanel.Size = new System.Drawing.Size(593, 515);
            this.drawPanel.TabIndex = 5;
            // 
            // outputPictureBox
            // 
            this.outputPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputPictureBox.Location = new System.Drawing.Point(0, 0);
            this.outputPictureBox.Name = "outputPictureBox";
            this.outputPictureBox.Size = new System.Drawing.Size(593, 515);
            this.outputPictureBox.TabIndex = 0;
            this.outputPictureBox.TabStop = false;
            this.outputPictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.outputPictureBox_MouseClick);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(530, 4);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 6;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(12, 582);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(593, 23);
            this.progressBar1.TabIndex = 7;
            // 
            // RenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 611);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.drawPanel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.renderButton);
            this.Controls.Add(this.fileButton);
            this.Controls.Add(this.fileLabel);
            this.Controls.Add(this.filenameTextBox);
            this.Name = "RenderForm";
            this.Text = "ToyTracer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.drawPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.outputPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox filenameTextBox;
        private System.Windows.Forms.Label fileLabel;
        private System.Windows.Forms.Button fileButton;
        private System.Windows.Forms.Button renderButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel drawPanel;
        private System.Windows.Forms.PictureBox outputPictureBox;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}

