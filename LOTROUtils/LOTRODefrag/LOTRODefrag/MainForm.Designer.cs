namespace LOTRODefrag
{
    partial class MainForm
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
            this.dataGridViewFiles = new System.Windows.Forms.DataGridView();
            this.Selected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.NameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IntFrag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IntFiles = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RecomDatDefrag = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ExtFrags = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExtFragRecommended = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonAnalyze = new System.Windows.Forms.Button();
            this.buttonDefrag = new System.Windows.Forms.Button();
            this.backgroundWorkerProcessing = new System.ComponentModel.BackgroundWorker();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.checkBoxAdvanced = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // dataGridViewFiles
            // 
            this.dataGridViewFiles.AllowUserToAddRows = false;
            this.dataGridViewFiles.AllowUserToDeleteRows = false;
            this.dataGridViewFiles.AllowUserToResizeRows = false;
            this.dataGridViewFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Selected,
            this.NameCol,
            this.IntFrag,
            this.IntFiles,
            this.RecomDatDefrag,
            this.ExtFrags,
            this.ExtFragRecommended,
            this.Status});
            this.dataGridViewFiles.Location = new System.Drawing.Point(12, 44);
            this.dataGridViewFiles.Name = "dataGridViewFiles";
            this.dataGridViewFiles.RowHeadersVisible = false;
            this.dataGridViewFiles.RowHeadersWidth = 4;
            this.dataGridViewFiles.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridViewFiles.Size = new System.Drawing.Size(757, 360);
            this.dataGridViewFiles.TabIndex = 0;
            this.dataGridViewFiles.SelectionChanged += new System.EventHandler(this.dataGridViewFiles_SelectionChanged);
            // 
            // Selected
            // 
            this.Selected.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.Selected.HeaderText = "Selected";
            this.Selected.Name = "Selected";
            this.Selected.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Selected.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Selected.Width = 74;
            // 
            // NameCol
            // 
            this.NameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.NameCol.FillWeight = 150F;
            this.NameCol.HeaderText = "Name";
            this.NameCol.Name = "NameCol";
            this.NameCol.ReadOnly = true;
            // 
            // IntFrag
            // 
            this.IntFrag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.IntFrag.HeaderText = "Internal Fragments";
            this.IntFrag.Name = "IntFrag";
            this.IntFrag.ReadOnly = true;
            this.IntFrag.Width = 109;
            // 
            // IntFiles
            // 
            this.IntFiles.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.IntFiles.HeaderText = "Internal Files";
            this.IntFiles.Name = "IntFiles";
            this.IntFiles.ReadOnly = true;
            this.IntFiles.Width = 84;
            // 
            // RecomDatDefrag
            // 
            this.RecomDatDefrag.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.RecomDatDefrag.HeaderText = "Dat Defrag Recommended";
            this.RecomDatDefrag.Name = "RecomDatDefrag";
            this.RecomDatDefrag.ReadOnly = true;
            this.RecomDatDefrag.Width = 126;
            // 
            // ExtFrags
            // 
            this.ExtFrags.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ExtFrags.HeaderText = "External Fragments";
            this.ExtFrags.Name = "ExtFrags";
            this.ExtFrags.ReadOnly = true;
            this.ExtFrags.Width = 112;
            // 
            // ExtFragRecommended
            // 
            this.ExtFragRecommended.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.ExtFragRecommended.HeaderText = "External Defrag Recommended";
            this.ExtFragRecommended.Name = "ExtFragRecommended";
            this.ExtFragRecommended.ReadOnly = true;
            this.ExtFragRecommended.Width = 145;
            // 
            // Status
            // 
            this.Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Status.FillWeight = 200F;
            this.Status.HeaderText = "Status";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // buttonAnalyze
            // 
            this.buttonAnalyze.Location = new System.Drawing.Point(139, 12);
            this.buttonAnalyze.Name = "buttonAnalyze";
            this.buttonAnalyze.Size = new System.Drawing.Size(75, 23);
            this.buttonAnalyze.TabIndex = 1;
            this.buttonAnalyze.Text = "Analyze";
            this.buttonAnalyze.UseVisualStyleBackColor = true;
            this.buttonAnalyze.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonDefrag
            // 
            this.buttonDefrag.Location = new System.Drawing.Point(220, 12);
            this.buttonDefrag.Name = "buttonDefrag";
            this.buttonDefrag.Size = new System.Drawing.Size(108, 23);
            this.buttonDefrag.TabIndex = 2;
            this.buttonDefrag.Text = "Full Defrag";
            this.buttonDefrag.UseVisualStyleBackColor = true;
            this.buttonDefrag.Click += new System.EventHandler(this.buttonDefrag_Click);
            // 
            // backgroundWorkerProcessing
            // 
            this.backgroundWorkerProcessing.WorkerReportsProgress = true;
            this.backgroundWorkerProcessing.WorkerSupportsCancellation = true;
            this.backgroundWorkerProcessing.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorkerProcessing.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorkerProcessing_ProgressChanged);
            this.backgroundWorkerProcessing.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorkerProcessing_RunWorkerCompleted);
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Dat Internal",
            "Filesystem"});
            this.comboBox1.Location = new System.Drawing.Point(12, 13);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // checkBoxAdvanced
            // 
            this.checkBoxAdvanced.AutoSize = true;
            this.checkBoxAdvanced.Location = new System.Drawing.Point(334, 16);
            this.checkBoxAdvanced.Name = "checkBoxAdvanced";
            this.checkBoxAdvanced.Size = new System.Drawing.Size(75, 17);
            this.checkBoxAdvanced.TabIndex = 4;
            this.checkBoxAdvanced.Text = "Advanced";
            this.checkBoxAdvanced.UseVisualStyleBackColor = true;
            this.checkBoxAdvanced.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(781, 416);
            this.Controls.Add(this.checkBoxAdvanced);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.buttonDefrag);
            this.Controls.Add(this.buttonAnalyze);
            this.Controls.Add(this.dataGridViewFiles);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MainForm";
            this.Text = "LOTRO Defrag";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewFiles;
        private System.Windows.Forms.Button buttonAnalyze;
        private System.Windows.Forms.Button buttonDefrag;
        private System.ComponentModel.BackgroundWorker backgroundWorkerProcessing;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.CheckBox checkBoxAdvanced;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Selected;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn IntFrag;
        private System.Windows.Forms.DataGridViewTextBoxColumn IntFiles;
        private System.Windows.Forms.DataGridViewCheckBoxColumn RecomDatDefrag;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExtFrags;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ExtFragRecommended;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
    }
}

