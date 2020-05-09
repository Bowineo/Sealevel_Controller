namespace CHOV
{
    partial class FrmHardware
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
            this.Pnl_Botoes = new System.Windows.Forms.Panel();
            this.BtSearch = new System.Windows.Forms.Button();
            this.BtOpen = new System.Windows.Forms.Button();
            this.BtWrite = new System.Windows.Forms.Button();
            this.BtClear = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Pnl_Botoes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Pnl_Botoes
            // 
            this.Pnl_Botoes.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.Pnl_Botoes.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Pnl_Botoes.Controls.Add(this.BtSearch);
            this.Pnl_Botoes.Controls.Add(this.BtOpen);
            this.Pnl_Botoes.Controls.Add(this.BtWrite);
            this.Pnl_Botoes.Controls.Add(this.BtClear);
            this.Pnl_Botoes.Location = new System.Drawing.Point(286, 284);
            this.Pnl_Botoes.Name = "Pnl_Botoes";
            this.Pnl_Botoes.Size = new System.Drawing.Size(431, 66);
            this.Pnl_Botoes.TabIndex = 76;
            // 
            // BtSearch
            // 
            this.BtSearch.BackColor = System.Drawing.Color.LightGray;
            this.BtSearch.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.BtSearch.FlatAppearance.BorderSize = 0;
            this.BtSearch.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightGray;
            this.BtSearch.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Snow;
            this.BtSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtSearch.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtSearch.Location = new System.Drawing.Point(11, 9);
            this.BtSearch.Name = "BtSearch";
            this.BtSearch.Size = new System.Drawing.Size(95, 43);
            this.BtSearch.TabIndex = 0;
            this.BtSearch.Text = "SEARCH";
            this.BtSearch.UseVisualStyleBackColor = false;
            // 
            // BtOpen
            // 
            this.BtOpen.BackColor = System.Drawing.Color.LightGray;
            this.BtOpen.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.BtOpen.FlatAppearance.BorderSize = 0;
            this.BtOpen.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightGray;
            this.BtOpen.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Snow;
            this.BtOpen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtOpen.Location = new System.Drawing.Point(116, 9);
            this.BtOpen.Name = "BtOpen";
            this.BtOpen.Size = new System.Drawing.Size(95, 43);
            this.BtOpen.TabIndex = 50;
            this.BtOpen.Text = "OPEN";
            this.BtOpen.UseVisualStyleBackColor = false;
            this.BtOpen.Click += new System.EventHandler(this.BtOpen_Click);
            // 
            // BtWrite
            // 
            this.BtWrite.BackColor = System.Drawing.Color.LightGray;
            this.BtWrite.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.BtWrite.FlatAppearance.BorderSize = 0;
            this.BtWrite.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightGray;
            this.BtWrite.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Snow;
            this.BtWrite.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtWrite.Location = new System.Drawing.Point(217, 9);
            this.BtWrite.Name = "BtWrite";
            this.BtWrite.Size = new System.Drawing.Size(95, 43);
            this.BtWrite.TabIndex = 52;
            this.BtWrite.Text = "WRITE";
            this.BtWrite.UseVisualStyleBackColor = false;
            // 
            // BtClear
            // 
            this.BtClear.BackColor = System.Drawing.Color.LightGray;
            this.BtClear.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.BtClear.FlatAppearance.BorderSize = 0;
            this.BtClear.FlatAppearance.MouseDownBackColor = System.Drawing.Color.LightGray;
            this.BtClear.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Snow;
            this.BtClear.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BtClear.Location = new System.Drawing.Point(318, 9);
            this.BtClear.Name = "BtClear";
            this.BtClear.Size = new System.Drawing.Size(95, 43);
            this.BtClear.TabIndex = 54;
            this.BtClear.Text = "CLEAR";
            this.BtClear.UseVisualStyleBackColor = false;
            this.BtClear.Click += new System.EventHandler(this.BtClear_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 16;
            this.listBox1.Location = new System.Drawing.Point(2, 369);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(961, 228);
            this.listBox1.TabIndex = 77;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::CHOV.Properties.Resources._410E;
            this.pictureBox1.Location = new System.Drawing.Point(163, 36);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(628, 218);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 78;
            this.pictureBox1.TabStop = false;
            // 
            // FrmHardware
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.ClientSize = new System.Drawing.Size(998, 700);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.Pnl_Botoes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmHardware";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hardware";
            this.TopMost = true;
            this.Pnl_Botoes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel Pnl_Botoes;
        private System.Windows.Forms.Button BtSearch;
        private System.Windows.Forms.Button BtOpen;
        private System.Windows.Forms.Button BtWrite;
        private System.Windows.Forms.Button BtClear;
        public System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}