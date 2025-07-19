namespace CTableConverter
{
    partial class Frm_Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Txt_Filepath = new System.Windows.Forms.TextBox();
            this.Btn_Browse = new System.Windows.Forms.Button();
            this.Ofd_File = new System.Windows.Forms.OpenFileDialog();
            this.Btn_Load = new System.Windows.Forms.Button();
            this.Grp_LoadedTable = new System.Windows.Forms.GroupBox();
            this.Lsv_Data = new System.Windows.Forms.ListView();
            this.Lbl_Rows = new System.Windows.Forms.Label();
            this.Lbl_Version = new System.Windows.Forms.Label();
            this.Grp_LoadedTable.SuspendLayout();
            this.SuspendLayout();
            // 
            // Txt_Filepath
            // 
            this.Txt_Filepath.Location = new System.Drawing.Point(39, 38);
            this.Txt_Filepath.Name = "Txt_Filepath";
            this.Txt_Filepath.Size = new System.Drawing.Size(297, 23);
            this.Txt_Filepath.TabIndex = 0;
            // 
            // Btn_Browse
            // 
            this.Btn_Browse.Location = new System.Drawing.Point(342, 37);
            this.Btn_Browse.Name = "Btn_Browse";
            this.Btn_Browse.Size = new System.Drawing.Size(75, 23);
            this.Btn_Browse.TabIndex = 1;
            this.Btn_Browse.Text = "&Browse";
            this.Btn_Browse.UseVisualStyleBackColor = true;
            // 
            // Ofd_File
            // 
            this.Ofd_File.FileName = "MyCheatTable.ct";
            this.Ofd_File.Filter = "Cheattable|*.ct";
            this.Ofd_File.ShowReadOnly = true;
            // 
            // Btn_Load
            // 
            this.Btn_Load.Location = new System.Drawing.Point(505, 38);
            this.Btn_Load.Name = "Btn_Load";
            this.Btn_Load.Size = new System.Drawing.Size(103, 23);
            this.Btn_Load.TabIndex = 2;
            this.Btn_Load.Text = "&Load";
            this.Btn_Load.UseVisualStyleBackColor = true;
            // 
            // Grp_LoadedTable
            // 
            this.Grp_LoadedTable.Controls.Add(this.Lsv_Data);
            this.Grp_LoadedTable.Controls.Add(this.Lbl_Rows);
            this.Grp_LoadedTable.Controls.Add(this.Lbl_Version);
            this.Grp_LoadedTable.Location = new System.Drawing.Point(12, 85);
            this.Grp_LoadedTable.Name = "Grp_LoadedTable";
            this.Grp_LoadedTable.Size = new System.Drawing.Size(776, 371);
            this.Grp_LoadedTable.TabIndex = 3;
            this.Grp_LoadedTable.TabStop = false;
            this.Grp_LoadedTable.Text = "Loaded table";
            // 
            // Lsv_Data
            // 
            this.Lsv_Data.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.Lsv_Data.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Lsv_Data.FullRowSelect = true;
            this.Lsv_Data.GridLines = true;
            this.Lsv_Data.Location = new System.Drawing.Point(6, 22);
            this.Lsv_Data.Name = "Lsv_Data";
            this.Lsv_Data.Size = new System.Drawing.Size(764, 323);
            this.Lsv_Data.TabIndex = 3;
            this.Lsv_Data.UseCompatibleStateImageBehavior = false;
            this.Lsv_Data.View = System.Windows.Forms.View.Details;
            // 
            // Lbl_Rows
            // 
            this.Lbl_Rows.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.Lbl_Rows.AutoSize = true;
            this.Lbl_Rows.Location = new System.Drawing.Point(701, 348);
            this.Lbl_Rows.Name = "Lbl_Rows";
            this.Lbl_Rows.Size = new System.Drawing.Size(56, 15);
            this.Lbl_Rows.TabIndex = 2;
            this.Lbl_Rows.Text = "Lbl_Rows";
            // 
            // Lbl_Version
            // 
            this.Lbl_Version.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.Lbl_Version.AutoSize = true;
            this.Lbl_Version.Location = new System.Drawing.Point(6, 348);
            this.Lbl_Version.Name = "Lbl_Version";
            this.Lbl_Version.Size = new System.Drawing.Size(66, 15);
            this.Lbl_Version.TabIndex = 0;
            this.Lbl_Version.Text = "Lbl_Version";
            // 
            // Frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 518);
            this.Controls.Add(this.Grp_LoadedTable);
            this.Controls.Add(this.Btn_Load);
            this.Controls.Add(this.Btn_Browse);
            this.Controls.Add(this.Txt_Filepath);
            this.Name = "Frm_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Main";
            this.Grp_LoadedTable.ResumeLayout(false);
            this.Grp_LoadedTable.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button Btn_Browse;
        internal System.Windows.Forms.OpenFileDialog Ofd_File;
        internal System.Windows.Forms.TextBox Txt_Filepath;
        private System.Windows.Forms.Button Btn_Load;
        private System.Windows.Forms.GroupBox Grp_LoadedTable;
        private System.Windows.Forms.Label Lbl_Version;
        private System.Windows.Forms.Label Lbl_Rows;
        internal System.Windows.Forms.ListView Lsv_Data;
    }
}
