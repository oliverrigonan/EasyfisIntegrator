namespace EasyfisIntegrator
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnMainIntegrate = new System.Windows.Forms.Button();
            this.pgbMainLoading = new System.Windows.Forms.ProgressBar();
            this.lblLoading = new System.Windows.Forms.Label();
            this.btnIntegrateFiles = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(544, 64);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(39, 39);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(57, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(281, 39);
            this.label1.TabIndex = 0;
            this.label1.Text = "Easyfis Integrator";
            // 
            // btnMainIntegrate
            // 
            this.btnMainIntegrate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMainIntegrate.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnMainIntegrate.FlatAppearance.BorderSize = 0;
            this.btnMainIntegrate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMainIntegrate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnMainIntegrate.ForeColor = System.Drawing.Color.White;
            this.btnMainIntegrate.Location = new System.Drawing.Point(12, 236);
            this.btnMainIntegrate.Name = "btnMainIntegrate";
            this.btnMainIntegrate.Size = new System.Drawing.Size(254, 58);
            this.btnMainIntegrate.TabIndex = 1;
            this.btnMainIntegrate.Text = "Innosoft POS Integration";
            this.btnMainIntegrate.UseVisualStyleBackColor = false;
            this.btnMainIntegrate.Click += new System.EventHandler(this.btnMainIntegrate_Click);
            // 
            // pgbMainLoading
            // 
            this.pgbMainLoading.BackColor = System.Drawing.Color.Black;
            this.pgbMainLoading.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pgbMainLoading.Location = new System.Drawing.Point(0, 363);
            this.pgbMainLoading.Name = "pgbMainLoading";
            this.pgbMainLoading.Size = new System.Drawing.Size(544, 39);
            this.pgbMainLoading.TabIndex = 2;
            this.pgbMainLoading.Visible = false;
            // 
            // lblLoading
            // 
            this.lblLoading.AutoSize = true;
            this.lblLoading.BackColor = System.Drawing.Color.Transparent;
            this.lblLoading.ForeColor = System.Drawing.Color.White;
            this.lblLoading.Location = new System.Drawing.Point(5, 343);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(158, 17);
            this.lblLoading.TabIndex = 3;
            this.lblLoading.Text = "Loading... Please wait...";
            this.lblLoading.Visible = false;
            // 
            // btnIntegrateFiles
            // 
            this.btnIntegrateFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIntegrateFiles.BackColor = System.Drawing.Color.SeaGreen;
            this.btnIntegrateFiles.FlatAppearance.BorderSize = 0;
            this.btnIntegrateFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIntegrateFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F);
            this.btnIntegrateFiles.ForeColor = System.Drawing.Color.White;
            this.btnIntegrateFiles.Location = new System.Drawing.Point(278, 236);
            this.btnIntegrateFiles.Name = "btnIntegrateFiles";
            this.btnIntegrateFiles.Size = new System.Drawing.Size(254, 58);
            this.btnIntegrateFiles.TabIndex = 4;
            this.btnIntegrateFiles.Text = "Folder Monitoring";
            this.btnIntegrateFiles.UseVisualStyleBackColor = false;
            this.btnIntegrateFiles.Click += new System.EventHandler(this.btnIntegrateFiles_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.pictureBox2);
            this.panel2.Location = new System.Drawing.Point(12, 139);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(254, 100);
            this.panel2.TabIndex = 5;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.pictureBox3);
            this.panel3.Location = new System.Drawing.Point(278, 139);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(254, 100);
            this.panel3.TabIndex = 6;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(248, 94);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox3.Image")));
            this.pictureBox3.Location = new System.Drawing.Point(3, 10);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(248, 78);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 1;
            this.pictureBox3.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(544, 402);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnIntegrateFiles);
            this.Controls.Add(this.lblLoading);
            this.Controls.Add(this.pgbMainLoading);
            this.Controls.Add(this.btnMainIntegrate);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Easyfis Integrator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMainIntegrate;
        private System.Windows.Forms.ProgressBar pgbMainLoading;
        private System.Windows.Forms.Label lblLoading;
        private System.Windows.Forms.Button btnIntegrateFiles;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
    }
}