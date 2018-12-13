namespace EasyfisIntegrator.Forms
{
    partial class SysSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SysSettings));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.tabSettings = new System.Windows.Forms.TabControl();
            this.tabPagePOSSettings = new System.Windows.Forms.TabPage();
            this.cboPostSupplier = new System.Windows.Forms.ComboBox();
            this.cboPostUser = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.cbxUseItemPrice = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtUserCode = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtBranchCode = new System.Windows.Forms.TextBox();
            this.tabPageSystem = new System.Windows.Forms.TabPage();
            this.btnLocateLogFileLocation = new System.Windows.Forms.Button();
            this.btnLocateFolderToMonitor = new System.Windows.Forms.Button();
            this.cbxIsAutoStartIntegration = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFolderToMonitor = new System.Windows.Forms.TextBox();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtLogFileLocation = new System.Windows.Forms.TextBox();
            this.tabPageConnection = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.btnCloseSettings = new System.Windows.Forms.Button();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.fbdLogFileLocation = new System.Windows.Forms.FolderBrowserDialog();
            this.fbdFolderToMonitor = new System.Windows.Forms.FolderBrowserDialog();
            this.cbxIsFolderMonitoring = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.tabPagePOSSettings.SuspendLayout();
            this.tabPageSystem.SuspendLayout();
            this.tabPageConnection.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(634, 51);
            this.panel1.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Settings";
            // 
            // tabSettings
            // 
            this.tabSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabSettings.Controls.Add(this.tabPagePOSSettings);
            this.tabSettings.Controls.Add(this.tabPageSystem);
            this.tabSettings.Controls.Add(this.tabPageConnection);
            this.tabSettings.Location = new System.Drawing.Point(12, 57);
            this.tabSettings.Name = "tabSettings";
            this.tabSettings.SelectedIndex = 0;
            this.tabSettings.Size = new System.Drawing.Size(610, 282);
            this.tabSettings.TabIndex = 10;
            // 
            // tabPagePOSSettings
            // 
            this.tabPagePOSSettings.Controls.Add(this.cboPostSupplier);
            this.tabPagePOSSettings.Controls.Add(this.cboPostUser);
            this.tabPagePOSSettings.Controls.Add(this.label2);
            this.tabPagePOSSettings.Controls.Add(this.label3);
            this.tabPagePOSSettings.Controls.Add(this.label11);
            this.tabPagePOSSettings.Controls.Add(this.cbxUseItemPrice);
            this.tabPagePOSSettings.Controls.Add(this.label10);
            this.tabPagePOSSettings.Controls.Add(this.txtUserCode);
            this.tabPagePOSSettings.Controls.Add(this.label7);
            this.tabPagePOSSettings.Controls.Add(this.txtBranchCode);
            this.tabPagePOSSettings.Location = new System.Drawing.Point(4, 25);
            this.tabPagePOSSettings.Name = "tabPagePOSSettings";
            this.tabPagePOSSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePOSSettings.Size = new System.Drawing.Size(602, 253);
            this.tabPagePOSSettings.TabIndex = 0;
            this.tabPagePOSSettings.Text = "POS Settings";
            this.tabPagePOSSettings.UseVisualStyleBackColor = true;
            // 
            // cboPostSupplier
            // 
            this.cboPostSupplier.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cboPostSupplier.FormattingEnabled = true;
            this.cboPostSupplier.Location = new System.Drawing.Point(199, 135);
            this.cboPostSupplier.Name = "cboPostSupplier";
            this.cboPostSupplier.Size = new System.Drawing.Size(370, 28);
            this.cboPostSupplier.TabIndex = 34;
            // 
            // cboPostUser
            // 
            this.cboPostUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cboPostUser.FormattingEnabled = true;
            this.cboPostUser.Location = new System.Drawing.Point(199, 103);
            this.cboPostUser.Name = "cboPostUser";
            this.cboPostUser.Size = new System.Drawing.Size(370, 28);
            this.cboPostUser.TabIndex = 33;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label2.Location = new System.Drawing.Point(62, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(114, 20);
            this.label2.TabIndex = 32;
            this.label2.Text = "Post Supplier:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label3.Location = new System.Drawing.Point(87, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 20);
            this.label3.TabIndex = 30;
            this.label3.Text = "Post User:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label11.Location = new System.Drawing.Point(51, 77);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(125, 20);
            this.label11.TabIndex = 29;
            this.label11.Text = "Use Item Price:";
            // 
            // cbxUseItemPrice
            // 
            this.cbxUseItemPrice.AutoSize = true;
            this.cbxUseItemPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cbxUseItemPrice.Location = new System.Drawing.Point(199, 80);
            this.cbxUseItemPrice.Name = "cbxUseItemPrice";
            this.cbxUseItemPrice.Size = new System.Drawing.Size(18, 17);
            this.cbxUseItemPrice.TabIndex = 28;
            this.cbxUseItemPrice.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label10.Location = new System.Drawing.Point(82, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 20);
            this.label10.TabIndex = 26;
            this.label10.Text = "User Code:";
            // 
            // txtUserCode
            // 
            this.txtUserCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtUserCode.Location = new System.Drawing.Point(199, 48);
            this.txtUserCode.Name = "txtUserCode";
            this.txtUserCode.Size = new System.Drawing.Size(370, 26);
            this.txtUserCode.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label7.Location = new System.Drawing.Point(64, 19);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 20);
            this.label7.TabIndex = 24;
            this.label7.Text = "Branch Code:";
            // 
            // txtBranchCode
            // 
            this.txtBranchCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtBranchCode.Location = new System.Drawing.Point(199, 16);
            this.txtBranchCode.Name = "txtBranchCode";
            this.txtBranchCode.Size = new System.Drawing.Size(370, 26);
            this.txtBranchCode.TabIndex = 25;
            // 
            // tabPageSystem
            // 
            this.tabPageSystem.Controls.Add(this.cbxIsFolderMonitoring);
            this.tabPageSystem.Controls.Add(this.label12);
            this.tabPageSystem.Controls.Add(this.btnLocateLogFileLocation);
            this.tabPageSystem.Controls.Add(this.btnLocateFolderToMonitor);
            this.tabPageSystem.Controls.Add(this.cbxIsAutoStartIntegration);
            this.tabPageSystem.Controls.Add(this.label8);
            this.tabPageSystem.Controls.Add(this.label6);
            this.tabPageSystem.Controls.Add(this.label4);
            this.tabPageSystem.Controls.Add(this.txtFolderToMonitor);
            this.tabPageSystem.Controls.Add(this.txtDomain);
            this.tabPageSystem.Controls.Add(this.label9);
            this.tabPageSystem.Controls.Add(this.txtLogFileLocation);
            this.tabPageSystem.Location = new System.Drawing.Point(4, 25);
            this.tabPageSystem.Name = "tabPageSystem";
            this.tabPageSystem.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSystem.Size = new System.Drawing.Size(602, 253);
            this.tabPageSystem.TabIndex = 1;
            this.tabPageSystem.Text = "System";
            this.tabPageSystem.UseVisualStyleBackColor = true;
            // 
            // btnLocateLogFileLocation
            // 
            this.btnLocateLogFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLocateLogFileLocation.BackColor = System.Drawing.SystemColors.Control;
            this.btnLocateLogFileLocation.FlatAppearance.BorderSize = 0;
            this.btnLocateLogFileLocation.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnLocateLogFileLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnLocateLogFileLocation.ForeColor = System.Drawing.Color.Black;
            this.btnLocateLogFileLocation.Location = new System.Drawing.Point(515, 19);
            this.btnLocateLogFileLocation.Name = "btnLocateLogFileLocation";
            this.btnLocateLogFileLocation.Size = new System.Drawing.Size(54, 30);
            this.btnLocateLogFileLocation.TabIndex = 47;
            this.btnLocateLogFileLocation.Text = "...";
            this.btnLocateLogFileLocation.UseVisualStyleBackColor = false;
            this.btnLocateLogFileLocation.Click += new System.EventHandler(this.btnLocateLogFileLocation_Click);
            // 
            // btnLocateFolderToMonitor
            // 
            this.btnLocateFolderToMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLocateFolderToMonitor.BackColor = System.Drawing.SystemColors.Control;
            this.btnLocateFolderToMonitor.FlatAppearance.BorderSize = 0;
            this.btnLocateFolderToMonitor.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnLocateFolderToMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnLocateFolderToMonitor.ForeColor = System.Drawing.Color.Black;
            this.btnLocateFolderToMonitor.Location = new System.Drawing.Point(515, 83);
            this.btnLocateFolderToMonitor.Name = "btnLocateFolderToMonitor";
            this.btnLocateFolderToMonitor.Size = new System.Drawing.Size(54, 30);
            this.btnLocateFolderToMonitor.TabIndex = 27;
            this.btnLocateFolderToMonitor.Text = "...";
            this.btnLocateFolderToMonitor.UseVisualStyleBackColor = false;
            this.btnLocateFolderToMonitor.Click += new System.EventHandler(this.btnLocateFolderToMonitor_Click);
            // 
            // cbxIsAutoStartIntegration
            // 
            this.cbxIsAutoStartIntegration.AutoSize = true;
            this.cbxIsAutoStartIntegration.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cbxIsAutoStartIntegration.Location = new System.Drawing.Point(199, 112);
            this.cbxIsAutoStartIntegration.Name = "cbxIsAutoStartIntegration";
            this.cbxIsAutoStartIntegration.Size = new System.Drawing.Size(18, 17);
            this.cbxIsAutoStartIntegration.TabIndex = 46;
            this.cbxIsAutoStartIntegration.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label8.Location = new System.Drawing.Point(69, 109);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(107, 20);
            this.label8.TabIndex = 45;
            this.label8.Text = "Is Auto Start:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label6.Location = new System.Drawing.Point(104, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 20);
            this.label6.TabIndex = 44;
            this.label6.Text = "Domain:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label4.Location = new System.Drawing.Point(35, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 20);
            this.label4.TabIndex = 42;
            this.label4.Text = "Folder to Monitor:";
            // 
            // txtFolderToMonitor
            // 
            this.txtFolderToMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtFolderToMonitor.Location = new System.Drawing.Point(199, 80);
            this.txtFolderToMonitor.Name = "txtFolderToMonitor";
            this.txtFolderToMonitor.ReadOnly = true;
            this.txtFolderToMonitor.Size = new System.Drawing.Size(310, 26);
            this.txtFolderToMonitor.TabIndex = 43;
            // 
            // txtDomain
            // 
            this.txtDomain.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtDomain.Location = new System.Drawing.Point(199, 48);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(370, 26);
            this.txtDomain.TabIndex = 37;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label9.Location = new System.Drawing.Point(33, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(143, 20);
            this.label9.TabIndex = 34;
            this.label9.Text = "Log File Location:";
            // 
            // txtLogFileLocation
            // 
            this.txtLogFileLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtLogFileLocation.Location = new System.Drawing.Point(199, 16);
            this.txtLogFileLocation.Name = "txtLogFileLocation";
            this.txtLogFileLocation.ReadOnly = true;
            this.txtLogFileLocation.Size = new System.Drawing.Size(310, 26);
            this.txtLogFileLocation.TabIndex = 35;
            // 
            // tabPageConnection
            // 
            this.tabPageConnection.Controls.Add(this.label5);
            this.tabPageConnection.Controls.Add(this.txtConnectionString);
            this.tabPageConnection.Location = new System.Drawing.Point(4, 25);
            this.tabPageConnection.Name = "tabPageConnection";
            this.tabPageConnection.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConnection.Size = new System.Drawing.Size(602, 253);
            this.tabPageConnection.TabIndex = 2;
            this.tabPageConnection.Text = "Connection";
            this.tabPageConnection.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label5.Location = new System.Drawing.Point(29, 19);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 20);
            this.label5.TabIndex = 36;
            this.label5.Text = "Connection String:";
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtConnectionString.Location = new System.Drawing.Point(199, 16);
            this.txtConnectionString.Multiline = true;
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.ReadOnly = true;
            this.txtConnectionString.Size = new System.Drawing.Size(370, 138);
            this.txtConnectionString.TabIndex = 37;
            // 
            // btnCloseSettings
            // 
            this.btnCloseSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnCloseSettings.FlatAppearance.BorderSize = 0;
            this.btnCloseSettings.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCloseSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnCloseSettings.ForeColor = System.Drawing.Color.Black;
            this.btnCloseSettings.Location = new System.Drawing.Point(511, 345);
            this.btnCloseSettings.Name = "btnCloseSettings";
            this.btnCloseSettings.Size = new System.Drawing.Size(107, 32);
            this.btnCloseSettings.TabIndex = 25;
            this.btnCloseSettings.Text = "Close";
            this.btnCloseSettings.UseVisualStyleBackColor = false;
            this.btnCloseSettings.Click += new System.EventHandler(this.btnCloseSettings_Click);
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveSettings.FlatAppearance.BorderSize = 0;
            this.btnSaveSettings.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSaveSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnSaveSettings.ForeColor = System.Drawing.Color.Black;
            this.btnSaveSettings.Location = new System.Drawing.Point(398, 345);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(107, 32);
            this.btnSaveSettings.TabIndex = 26;
            this.btnSaveSettings.Text = "Save";
            this.btnSaveSettings.UseVisualStyleBackColor = false;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // cbxIsFolderMonitoring
            // 
            this.cbxIsFolderMonitoring.AutoSize = true;
            this.cbxIsFolderMonitoring.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cbxIsFolderMonitoring.Location = new System.Drawing.Point(199, 135);
            this.cbxIsFolderMonitoring.Name = "cbxIsFolderMonitoring";
            this.cbxIsFolderMonitoring.Size = new System.Drawing.Size(18, 17);
            this.cbxIsFolderMonitoring.TabIndex = 49;
            this.cbxIsFolderMonitoring.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label12.Location = new System.Drawing.Point(14, 132);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(162, 20);
            this.label12.TabIndex = 48;
            this.label12.Text = "Is Folder Monitoring:";
            // 
            // SysSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 389);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.btnCloseSettings);
            this.Controls.Add(this.tabSettings);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SysSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabSettings.ResumeLayout(false);
            this.tabPagePOSSettings.ResumeLayout(false);
            this.tabPagePOSSettings.PerformLayout();
            this.tabPageSystem.ResumeLayout(false);
            this.tabPageSystem.PerformLayout();
            this.tabPageConnection.ResumeLayout(false);
            this.tabPageConnection.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabSettings;
        private System.Windows.Forms.TabPage tabPagePOSSettings;
        private System.Windows.Forms.TabPage tabPageSystem;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtUserCode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtBranchCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox cbxUseItemPrice;
        private System.Windows.Forms.Button btnCloseSettings;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtFolderToMonitor;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtLogFileLocation;
        private System.Windows.Forms.TabPage tabPageConnection;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cboPostSupplier;
        private System.Windows.Forms.ComboBox cboPostUser;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox cbxIsAutoStartIntegration;
        private System.Windows.Forms.Button btnLocateFolderToMonitor;
        private System.Windows.Forms.Button btnLocateLogFileLocation;
        private System.Windows.Forms.FolderBrowserDialog fbdLogFileLocation;
        private System.Windows.Forms.FolderBrowserDialog fbdFolderToMonitor;
        private System.Windows.Forms.CheckBox cbxIsFolderMonitoring;
        private System.Windows.Forms.Label label12;
    }
}