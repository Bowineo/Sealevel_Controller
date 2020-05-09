namespace CHOV
{
    partial class FrmLogs
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
            this.components = new System.ComponentModel.Container();
            this.Historico1 = new System.Windows.Forms.ListBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.LblNitensHistorico = new System.Windows.Forms.Label();
            this.label1LblNitensLog = new System.Windows.Forms.Label();
            this.pnlStatLog = new System.Windows.Forms.Panel();
            this.TimerLogs = new System.Windows.Forms.Timer(this.components);
            this.LblRelogioLog = new System.Windows.Forms.Label();
            this.BtnExport = new System.Windows.Forms.Button();
            this.pnlStatLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // Historico1
            // 
            this.Historico1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Historico1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Historico1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Historico1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Historico1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Historico1.FormattingEnabled = true;
            this.Historico1.HorizontalScrollbar = true;
            this.Historico1.ItemHeight = 16;
            this.Historico1.Location = new System.Drawing.Point(33, 41);
            this.Historico1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Historico1.Name = "Historico1";
            this.Historico1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.Historico1.Size = new System.Drawing.Size(1329, 402);
            this.Historico1.TabIndex = 48;
            this.Historico1.TabStop = false;
            // 
            // LblNitensHistorico
            // 
            this.LblNitensHistorico.AutoSize = true;
            this.LblNitensHistorico.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.LblNitensHistorico.Location = new System.Drawing.Point(4, 7);
            this.LblNitensHistorico.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblNitensHistorico.Name = "LblNitensHistorico";
            this.LblNitensHistorico.Size = new System.Drawing.Size(180, 17);
            this.LblNitensHistorico.TabIndex = 51;
            this.LblNitensHistorico.Text = "Diferential historical - Logs:";
            // 
            // label1LblNitensLog
            // 
            this.label1LblNitensLog.AutoSize = true;
            this.label1LblNitensLog.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.label1LblNitensLog.Location = new System.Drawing.Point(433, 7);
            this.label1LblNitensLog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1LblNitensLog.Name = "label1LblNitensLog";
            this.label1LblNitensLog.Size = new System.Drawing.Size(197, 17);
            this.label1LblNitensLog.TabIndex = 52;
            this.label1LblNitensLog.Text = "Number of historical readings:";
            // 
            // pnlStatLog
            // 
            this.pnlStatLog.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.pnlStatLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlStatLog.Controls.Add(this.LblNitensHistorico);
            this.pnlStatLog.Controls.Add(this.label1LblNitensLog);
            this.pnlStatLog.Location = new System.Drawing.Point(289, 464);
            this.pnlStatLog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlStatLog.Name = "pnlStatLog";
            this.pnlStatLog.Size = new System.Drawing.Size(815, 29);
            this.pnlStatLog.TabIndex = 53;
            // 
            // TimerLogs
            // 
            this.TimerLogs.Interval = 500;
            this.TimerLogs.Tick += new System.EventHandler(this.TimerLogs_Tick);
            // 
            // LblRelogioLog
            // 
            this.LblRelogioLog.BackColor = System.Drawing.SystemColors.ControlText;
            this.LblRelogioLog.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.LblRelogioLog.Location = new System.Drawing.Point(966, 11);
            this.LblRelogioLog.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.LblRelogioLog.Name = "LblRelogioLog";
            this.LblRelogioLog.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.LblRelogioLog.Size = new System.Drawing.Size(397, 22);
            this.LblRelogioLog.TabIndex = 108;
            this.LblRelogioLog.Text = "00/00/0000 00:00:00";
            this.LblRelogioLog.Visible = false;
            // 
            // BtnExport
            // 
            this.BtnExport.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.BtnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(103)))), ((int)(((byte)(128)))), ((int)(((byte)(159)))));
            this.BtnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BtnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.BtnExport.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnExport.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.BtnExport.FlatAppearance.CheckedBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.BtnExport.FlatAppearance.MouseDownBackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.BtnExport.FlatAppearance.MouseOverBackColor = System.Drawing.SystemColors.AppWorkspace;
            this.BtnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BtnExport.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.BtnExport.Location = new System.Drawing.Point(1280, 463);
            this.BtnExport.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BtnExport.Name = "BtnExport";
            this.BtnExport.Size = new System.Drawing.Size(83, 33);
            this.BtnExport.TabIndex = 7;
            this.BtnExport.Text = "Export";
            this.BtnExport.UseVisualStyleBackColor = false;
            this.BtnExport.Click += new System.EventHandler(this.BtnExport_Click);
            // 
            // FrmLogs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.MenuText;
            this.ClientSize = new System.Drawing.Size(1396, 516);
            this.Controls.Add(this.BtnExport);
            this.Controls.Add(this.LblRelogioLog);
            this.Controls.Add(this.pnlStatLog);
            this.Controls.Add(this.Historico1);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.Name = "FrmLogs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Logs";
            this.TopMost = true;
            this.Shown += new System.EventHandler(this.FrmLogs_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmLogs_KeyDown);
            this.pnlStatLog.ResumeLayout(false);
            this.pnlStatLog.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.ListBox Historico1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label LblNitensHistorico;
        private System.Windows.Forms.Label label1LblNitensLog;
        private System.Windows.Forms.Panel pnlStatLog;
        private System.Windows.Forms.Timer TimerLogs;
        private System.Windows.Forms.Label LblRelogioLog;
        private System.Windows.Forms.Button BtnExport;
    }
}