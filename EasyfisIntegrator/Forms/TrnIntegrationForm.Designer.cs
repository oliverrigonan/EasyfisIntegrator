namespace EasyfisIntegrator.Forms
{
    partial class TrnIntegrationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrnIntegrationForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.fbdGetCSVTemplate = new System.Windows.Forms.FolderBrowserDialog();
            this.fileSystemWatcherCSVFiles = new System.IO.FileSystemWatcher();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnClearLogs = new System.Windows.Forms.Button();
            this.btnSaveLogs = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.tabIntegration = new System.Windows.Forms.TabControl();
            this.tabPOSIntegration = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.cbxUseItemPrice = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtUserCode = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.dtpIntegrationDate = new System.Windows.Forms.DateTimePicker();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtBranchCode = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnStartIntegration = new System.Windows.Forms.Button();
            this.btnStopIntegration = new System.Windows.Forms.Button();
            this.lblCurrentUser = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.tabPageManualSalesIntegration = new System.Windows.Forms.TabPage();
            this.textBoxPOSManualSalesIntegrationLogs = new System.Windows.Forms.TextBox();
            this.buttonPOSManualSalesIntegrationStart = new System.Windows.Forms.Button();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.dateTimePickerPOSManualSalesIntegrationDate = new System.Windows.Forms.DateTimePicker();
            this.label17 = new System.Windows.Forms.Label();
            this.tabFolderMonitoring = new System.Windows.Forms.TabPage();
            this.btnGetCSVTemplate = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.txtFolderMonitoringUserCode = new System.Windows.Forms.TextBox();
            this.txtFolderMonitoringDomain = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.btnStartFolderMonitoringIntegration = new System.Windows.Forms.Button();
            this.txtFolderMonitoringLogs = new System.Windows.Forms.TextBox();
            this.bgwFolderMonitoringIntegration = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerManualSalesIntegration = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcherCSVFiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.tabIntegration.SuspendLayout();
            this.tabPOSIntegration.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tabPageManualSalesIntegration.SuspendLayout();
            this.panel7.SuspendLayout();
            this.tabFolderMonitoring.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.panel1.Controls.Add(this.btnLogout);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(872, 51);
            this.panel1.TabIndex = 8;
            // 
            // btnLogout
            // 
            this.btnLogout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLogout.BackColor = System.Drawing.Color.IndianRed;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(769, 11);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(93, 29);
            this.btnLogout.TabIndex = 22;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Integration";
            // 
            // fileSystemWatcherCSVFiles
            // 
            this.fileSystemWatcherCSVFiles.EnableRaisingEvents = true;
            this.fileSystemWatcherCSVFiles.Filter = "*.csv";
            this.fileSystemWatcherCSVFiles.IncludeSubdirectories = true;
            this.fileSystemWatcherCSVFiles.NotifyFilter = ((System.IO.NotifyFilters)((((System.IO.NotifyFilters.FileName | System.IO.NotifyFilters.DirectoryName) 
            | System.IO.NotifyFilters.LastWrite) 
            | System.IO.NotifyFilters.LastAccess)));
            this.fileSystemWatcherCSVFiles.SynchronizingObject = this;
            this.fileSystemWatcherCSVFiles.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcherCSVFiles_Created);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(11, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(66, 60);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(88, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Easyfis Integrator";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(88, 36);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(207, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Version: 2.201911191510.NOR";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(88, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(204, 17);
            this.label6.TabIndex = 3;
            this.label6.Text = "Developer: Easyfis Corporation";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 760);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(872, 80);
            this.panel2.TabIndex = 7;
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.BackColor = System.Drawing.SystemColors.Control;
            this.btnSettings.FlatAppearance.BorderSize = 0;
            this.btnSettings.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnSettings.ForeColor = System.Drawing.Color.Black;
            this.btnSettings.Location = new System.Drawing.Point(753, 0);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(107, 40);
            this.btnSettings.TabIndex = 24;
            this.btnSettings.Text = "Settings";
            this.btnSettings.UseVisualStyleBackColor = false;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // btnClearLogs
            // 
            this.btnClearLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearLogs.BackColor = System.Drawing.SystemColors.Control;
            this.btnClearLogs.FlatAppearance.BorderSize = 0;
            this.btnClearLogs.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClearLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnClearLogs.ForeColor = System.Drawing.Color.Black;
            this.btnClearLogs.Location = new System.Drawing.Point(137, 0);
            this.btnClearLogs.Name = "btnClearLogs";
            this.btnClearLogs.Size = new System.Drawing.Size(107, 40);
            this.btnClearLogs.TabIndex = 23;
            this.btnClearLogs.Text = "Clear Logs";
            this.btnClearLogs.UseVisualStyleBackColor = false;
            this.btnClearLogs.Click += new System.EventHandler(this.btnClearLogs_Click);
            // 
            // btnSaveLogs
            // 
            this.btnSaveLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSaveLogs.BackColor = System.Drawing.SystemColors.Control;
            this.btnSaveLogs.FlatAppearance.BorderSize = 0;
            this.btnSaveLogs.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSaveLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.btnSaveLogs.ForeColor = System.Drawing.Color.Black;
            this.btnSaveLogs.Location = new System.Drawing.Point(12, 0);
            this.btnSaveLogs.Name = "btnSaveLogs";
            this.btnSaveLogs.Size = new System.Drawing.Size(119, 40);
            this.btnSaveLogs.TabIndex = 22;
            this.btnSaveLogs.Text = "Save Logs";
            this.btnSaveLogs.UseVisualStyleBackColor = false;
            this.btnSaveLogs.Click += new System.EventHandler(this.btnSaveLogs_Click);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.White;
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Controls.Add(this.tabIntegration);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 51);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(872, 709);
            this.panel5.TabIndex = 26;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.Controls.Add(this.btnSettings);
            this.panel6.Controls.Add(this.btnSaveLogs);
            this.panel6.Controls.Add(this.btnClearLogs);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 663);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(872, 46);
            this.panel6.TabIndex = 26;
            // 
            // tabIntegration
            // 
            this.tabIntegration.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabIntegration.Controls.Add(this.tabPOSIntegration);
            this.tabIntegration.Controls.Add(this.tabPageManualSalesIntegration);
            this.tabIntegration.Controls.Add(this.tabFolderMonitoring);
            this.tabIntegration.Location = new System.Drawing.Point(0, 0);
            this.tabIntegration.Name = "tabIntegration";
            this.tabIntegration.SelectedIndex = 0;
            this.tabIntegration.Size = new System.Drawing.Size(872, 657);
            this.tabIntegration.TabIndex = 0;
            // 
            // tabPOSIntegration
            // 
            this.tabPOSIntegration.Controls.Add(this.panel3);
            this.tabPOSIntegration.Controls.Add(this.btnStartIntegration);
            this.tabPOSIntegration.Controls.Add(this.btnStopIntegration);
            this.tabPOSIntegration.Controls.Add(this.lblCurrentUser);
            this.tabPOSIntegration.Controls.Add(this.label2);
            this.tabPOSIntegration.Controls.Add(this.txtLogs);
            this.tabPOSIntegration.Location = new System.Drawing.Point(4, 25);
            this.tabPOSIntegration.Name = "tabPOSIntegration";
            this.tabPOSIntegration.Padding = new System.Windows.Forms.Padding(3);
            this.tabPOSIntegration.Size = new System.Drawing.Size(864, 628);
            this.tabPOSIntegration.TabIndex = 0;
            this.tabPOSIntegration.Text = "POS Integration";
            this.tabPOSIntegration.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.cbxUseItemPrice);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.txtUserCode);
            this.panel3.Controls.Add(this.pictureBox2);
            this.panel3.Controls.Add(this.dtpIntegrationDate);
            this.panel3.Controls.Add(this.txtDomain);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.txtBranchCode);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Location = new System.Drawing.Point(8, 6);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(848, 173);
            this.panel3.TabIndex = 21;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label11.Location = new System.Drawing.Point(18, 141);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(125, 20);
            this.label11.TabIndex = 25;
            this.label11.Text = "Use Item Price:";
            // 
            // cbxUseItemPrice
            // 
            this.cbxUseItemPrice.AutoSize = true;
            this.cbxUseItemPrice.Enabled = false;
            this.cbxUseItemPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.cbxUseItemPrice.Location = new System.Drawing.Point(167, 144);
            this.cbxUseItemPrice.Name = "cbxUseItemPrice";
            this.cbxUseItemPrice.Size = new System.Drawing.Size(18, 17);
            this.cbxUseItemPrice.TabIndex = 24;
            this.cbxUseItemPrice.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label10.Location = new System.Drawing.Point(49, 111);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 20);
            this.label10.TabIndex = 22;
            this.label10.Text = "User Code:";
            // 
            // txtUserCode
            // 
            this.txtUserCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtUserCode.Location = new System.Drawing.Point(167, 108);
            this.txtUserCode.Name = "txtUserCode";
            this.txtUserCode.ReadOnly = true;
            this.txtUserCode.Size = new System.Drawing.Size(266, 26);
            this.txtUserCode.TabIndex = 23;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(694, 27);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(123, 104);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 21;
            this.pictureBox2.TabStop = false;
            // 
            // dtpIntegrationDate
            // 
            this.dtpIntegrationDate.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dtpIntegrationDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dtpIntegrationDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpIntegrationDate.Location = new System.Drawing.Point(167, 12);
            this.dtpIntegrationDate.Name = "dtpIntegrationDate";
            this.dtpIntegrationDate.Size = new System.Drawing.Size(152, 26);
            this.dtpIntegrationDate.TabIndex = 18;
            // 
            // txtDomain
            // 
            this.txtDomain.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtDomain.Location = new System.Drawing.Point(167, 44);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.ReadOnly = true;
            this.txtDomain.Size = new System.Drawing.Size(432, 26);
            this.txtDomain.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label7.Location = new System.Drawing.Point(31, 79);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 20);
            this.label7.TabIndex = 15;
            this.label7.Text = "Branch Code:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label9.Location = new System.Drawing.Point(71, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 20);
            this.label9.TabIndex = 19;
            this.label9.Text = "Domain:";
            // 
            // txtBranchCode
            // 
            this.txtBranchCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtBranchCode.Location = new System.Drawing.Point(167, 76);
            this.txtBranchCode.Name = "txtBranchCode";
            this.txtBranchCode.ReadOnly = true;
            this.txtBranchCode.Size = new System.Drawing.Size(266, 26);
            this.txtBranchCode.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label8.Location = new System.Drawing.Point(93, 17);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 20);
            this.label8.TabIndex = 17;
            this.label8.Text = "Date:";
            // 
            // btnStartIntegration
            // 
            this.btnStartIntegration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartIntegration.BackColor = System.Drawing.Color.SeaGreen;
            this.btnStartIntegration.FlatAppearance.BorderSize = 0;
            this.btnStartIntegration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartIntegration.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnStartIntegration.ForeColor = System.Drawing.Color.White;
            this.btnStartIntegration.Location = new System.Drawing.Point(664, 185);
            this.btnStartIntegration.Name = "btnStartIntegration";
            this.btnStartIntegration.Size = new System.Drawing.Size(93, 37);
            this.btnStartIntegration.TabIndex = 11;
            this.btnStartIntegration.Text = "Start";
            this.btnStartIntegration.UseVisualStyleBackColor = false;
            this.btnStartIntegration.Click += new System.EventHandler(this.btnStartIntegration_Click);
            // 
            // btnStopIntegration
            // 
            this.btnStopIntegration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopIntegration.BackColor = System.Drawing.Color.IndianRed;
            this.btnStopIntegration.FlatAppearance.BorderSize = 0;
            this.btnStopIntegration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopIntegration.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnStopIntegration.ForeColor = System.Drawing.Color.White;
            this.btnStopIntegration.Location = new System.Drawing.Point(763, 185);
            this.btnStopIntegration.Name = "btnStopIntegration";
            this.btnStopIntegration.Size = new System.Drawing.Size(93, 37);
            this.btnStopIntegration.TabIndex = 10;
            this.btnStopIntegration.Text = "Stop";
            this.btnStopIntegration.UseVisualStyleBackColor = false;
            this.btnStopIntegration.Click += new System.EventHandler(this.btnStopIntegration_Click);
            // 
            // lblCurrentUser
            // 
            this.lblCurrentUser.AutoSize = true;
            this.lblCurrentUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.lblCurrentUser.Location = new System.Drawing.Point(93, 193);
            this.lblCurrentUser.Name = "lblCurrentUser";
            this.lblCurrentUser.Size = new System.Drawing.Size(110, 20);
            this.lblCurrentUser.TabIndex = 13;
            this.lblCurrentUser.Text = "#CurrentUser";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(9, 193);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 20);
            this.label2.TabIndex = 12;
            this.label2.Text = "Current:";
            // 
            // txtLogs
            // 
            this.txtLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLogs.BackColor = System.Drawing.Color.Black;
            this.txtLogs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtLogs.ForeColor = System.Drawing.Color.White;
            this.txtLogs.Location = new System.Drawing.Point(8, 228);
            this.txtLogs.MaxLength = 0;
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.Size = new System.Drawing.Size(848, 394);
            this.txtLogs.TabIndex = 14;
            // 
            // tabPageManualSalesIntegration
            // 
            this.tabPageManualSalesIntegration.Controls.Add(this.textBoxPOSManualSalesIntegrationLogs);
            this.tabPageManualSalesIntegration.Controls.Add(this.buttonPOSManualSalesIntegrationStart);
            this.tabPageManualSalesIntegration.Controls.Add(this.panel7);
            this.tabPageManualSalesIntegration.Location = new System.Drawing.Point(4, 25);
            this.tabPageManualSalesIntegration.Name = "tabPageManualSalesIntegration";
            this.tabPageManualSalesIntegration.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageManualSalesIntegration.Size = new System.Drawing.Size(864, 628);
            this.tabPageManualSalesIntegration.TabIndex = 2;
            this.tabPageManualSalesIntegration.Text = "POS - Manual Sales Integration";
            this.tabPageManualSalesIntegration.UseVisualStyleBackColor = true;
            // 
            // textBoxPOSManualSalesIntegrationLogs
            // 
            this.textBoxPOSManualSalesIntegrationLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxPOSManualSalesIntegrationLogs.BackColor = System.Drawing.Color.Black;
            this.textBoxPOSManualSalesIntegrationLogs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxPOSManualSalesIntegrationLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxPOSManualSalesIntegrationLogs.ForeColor = System.Drawing.Color.White;
            this.textBoxPOSManualSalesIntegrationLogs.Location = new System.Drawing.Point(8, 112);
            this.textBoxPOSManualSalesIntegrationLogs.MaxLength = 0;
            this.textBoxPOSManualSalesIntegrationLogs.Multiline = true;
            this.textBoxPOSManualSalesIntegrationLogs.Name = "textBoxPOSManualSalesIntegrationLogs";
            this.textBoxPOSManualSalesIntegrationLogs.ReadOnly = true;
            this.textBoxPOSManualSalesIntegrationLogs.Size = new System.Drawing.Size(848, 510);
            this.textBoxPOSManualSalesIntegrationLogs.TabIndex = 25;
            // 
            // buttonPOSManualSalesIntegrationStart
            // 
            this.buttonPOSManualSalesIntegrationStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPOSManualSalesIntegrationStart.BackColor = System.Drawing.Color.SeaGreen;
            this.buttonPOSManualSalesIntegrationStart.FlatAppearance.BorderSize = 0;
            this.buttonPOSManualSalesIntegrationStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPOSManualSalesIntegrationStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonPOSManualSalesIntegrationStart.ForeColor = System.Drawing.Color.White;
            this.buttonPOSManualSalesIntegrationStart.Location = new System.Drawing.Point(680, 69);
            this.buttonPOSManualSalesIntegrationStart.Name = "buttonPOSManualSalesIntegrationStart";
            this.buttonPOSManualSalesIntegrationStart.Size = new System.Drawing.Size(176, 37);
            this.buttonPOSManualSalesIntegrationStart.TabIndex = 24;
            this.buttonPOSManualSalesIntegrationStart.Text = "Integrate";
            this.buttonPOSManualSalesIntegrationStart.UseVisualStyleBackColor = false;
            this.buttonPOSManualSalesIntegrationStart.Click += new System.EventHandler(this.buttonPOSManualSalesIntegrationStart_Click);
            // 
            // panel7
            // 
            this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel7.BackColor = System.Drawing.SystemColors.Control;
            this.panel7.Controls.Add(this.label12);
            this.panel7.Controls.Add(this.dateTimePickerPOSManualSalesIntegrationDate);
            this.panel7.Controls.Add(this.label17);
            this.panel7.Location = new System.Drawing.Point(8, 6);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(848, 57);
            this.panel7.TabIndex = 22;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label12.Location = new System.Drawing.Point(347, 17);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(447, 17);
            this.label12.TabIndex = 26;
            this.label12.Text = "This will disabled the real-time sales integration from the previous tab.";
            // 
            // dateTimePickerPOSManualSalesIntegrationDate
            // 
            this.dateTimePickerPOSManualSalesIntegrationDate.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dateTimePickerPOSManualSalesIntegrationDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dateTimePickerPOSManualSalesIntegrationDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerPOSManualSalesIntegrationDate.Location = new System.Drawing.Point(167, 12);
            this.dateTimePickerPOSManualSalesIntegrationDate.Name = "dateTimePickerPOSManualSalesIntegrationDate";
            this.dateTimePickerPOSManualSalesIntegrationDate.Size = new System.Drawing.Size(152, 26);
            this.dateTimePickerPOSManualSalesIntegrationDate.TabIndex = 18;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label17.Location = new System.Drawing.Point(93, 17);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(50, 20);
            this.label17.TabIndex = 17;
            this.label17.Text = "Date:";
            // 
            // tabFolderMonitoring
            // 
            this.tabFolderMonitoring.Controls.Add(this.btnGetCSVTemplate);
            this.tabFolderMonitoring.Controls.Add(this.panel4);
            this.tabFolderMonitoring.Controls.Add(this.btnStartFolderMonitoringIntegration);
            this.tabFolderMonitoring.Controls.Add(this.txtFolderMonitoringLogs);
            this.tabFolderMonitoring.Location = new System.Drawing.Point(4, 25);
            this.tabFolderMonitoring.Name = "tabFolderMonitoring";
            this.tabFolderMonitoring.Padding = new System.Windows.Forms.Padding(3);
            this.tabFolderMonitoring.Size = new System.Drawing.Size(864, 628);
            this.tabFolderMonitoring.TabIndex = 1;
            this.tabFolderMonitoring.Text = "Folder Monitoring";
            this.tabFolderMonitoring.UseVisualStyleBackColor = true;
            // 
            // btnGetCSVTemplate
            // 
            this.btnGetCSVTemplate.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnGetCSVTemplate.FlatAppearance.BorderSize = 0;
            this.btnGetCSVTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetCSVTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnGetCSVTemplate.ForeColor = System.Drawing.Color.White;
            this.btnGetCSVTemplate.Location = new System.Drawing.Point(8, 100);
            this.btnGetCSVTemplate.Name = "btnGetCSVTemplate";
            this.btnGetCSVTemplate.Size = new System.Drawing.Size(220, 37);
            this.btnGetCSVTemplate.TabIndex = 23;
            this.btnGetCSVTemplate.Text = "Get CSV Template";
            this.btnGetCSVTemplate.UseVisualStyleBackColor = false;
            this.btnGetCSVTemplate.Click += new System.EventHandler(this.btnGetCSVTemplate_Click);
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.txtFolderMonitoringUserCode);
            this.panel4.Controls.Add(this.txtFolderMonitoringDomain);
            this.panel4.Controls.Add(this.label14);
            this.panel4.Location = new System.Drawing.Point(8, 6);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(848, 88);
            this.panel4.TabIndex = 22;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label3.Location = new System.Drawing.Point(49, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 20);
            this.label3.TabIndex = 24;
            this.label3.Text = "User Code:";
            // 
            // txtFolderMonitoringUserCode
            // 
            this.txtFolderMonitoringUserCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtFolderMonitoringUserCode.Location = new System.Drawing.Point(167, 44);
            this.txtFolderMonitoringUserCode.Name = "txtFolderMonitoringUserCode";
            this.txtFolderMonitoringUserCode.ReadOnly = true;
            this.txtFolderMonitoringUserCode.Size = new System.Drawing.Size(266, 26);
            this.txtFolderMonitoringUserCode.TabIndex = 25;
            // 
            // txtFolderMonitoringDomain
            // 
            this.txtFolderMonitoringDomain.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtFolderMonitoringDomain.Location = new System.Drawing.Point(167, 12);
            this.txtFolderMonitoringDomain.Name = "txtFolderMonitoringDomain";
            this.txtFolderMonitoringDomain.ReadOnly = true;
            this.txtFolderMonitoringDomain.Size = new System.Drawing.Size(432, 26);
            this.txtFolderMonitoringDomain.TabIndex = 20;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label14.Location = new System.Drawing.Point(71, 16);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(72, 20);
            this.label14.TabIndex = 19;
            this.label14.Text = "Domain:";
            // 
            // btnStartFolderMonitoringIntegration
            // 
            this.btnStartFolderMonitoringIntegration.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartFolderMonitoringIntegration.BackColor = System.Drawing.Color.SeaGreen;
            this.btnStartFolderMonitoringIntegration.FlatAppearance.BorderSize = 0;
            this.btnStartFolderMonitoringIntegration.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartFolderMonitoringIntegration.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnStartFolderMonitoringIntegration.ForeColor = System.Drawing.Color.White;
            this.btnStartFolderMonitoringIntegration.Location = new System.Drawing.Point(763, 100);
            this.btnStartFolderMonitoringIntegration.Name = "btnStartFolderMonitoringIntegration";
            this.btnStartFolderMonitoringIntegration.Size = new System.Drawing.Size(93, 37);
            this.btnStartFolderMonitoringIntegration.TabIndex = 16;
            this.btnStartFolderMonitoringIntegration.Text = "Start";
            this.btnStartFolderMonitoringIntegration.UseVisualStyleBackColor = false;
            this.btnStartFolderMonitoringIntegration.Click += new System.EventHandler(this.btnStartFolderMonitoringIntegration_Click);
            // 
            // txtFolderMonitoringLogs
            // 
            this.txtFolderMonitoringLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFolderMonitoringLogs.BackColor = System.Drawing.Color.Black;
            this.txtFolderMonitoringLogs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFolderMonitoringLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtFolderMonitoringLogs.ForeColor = System.Drawing.Color.White;
            this.txtFolderMonitoringLogs.Location = new System.Drawing.Point(8, 143);
            this.txtFolderMonitoringLogs.MaxLength = 0;
            this.txtFolderMonitoringLogs.Multiline = true;
            this.txtFolderMonitoringLogs.Name = "txtFolderMonitoringLogs";
            this.txtFolderMonitoringLogs.ReadOnly = true;
            this.txtFolderMonitoringLogs.Size = new System.Drawing.Size(848, 479);
            this.txtFolderMonitoringLogs.TabIndex = 17;
            // 
            // bgwFolderMonitoringIntegration
            // 
            this.bgwFolderMonitoringIntegration.WorkerReportsProgress = true;
            this.bgwFolderMonitoringIntegration.WorkerSupportsCancellation = true;
            this.bgwFolderMonitoringIntegration.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgwFolderMonitoringIntegration_DoWork);
            // 
            // backgroundWorkerManualSalesIntegration
            // 
            this.backgroundWorkerManualSalesIntegration.WorkerReportsProgress = true;
            this.backgroundWorkerManualSalesIntegration.WorkerSupportsCancellation = true;
            this.backgroundWorkerManualSalesIntegration.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerManualSalesIntegration_DoWork);
            // 
            // TrnIntegrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(872, 840);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "TrnIntegrationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Integration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrnInnosoftPOSIntegrationForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcherCSVFiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.tabIntegration.ResumeLayout(false);
            this.tabPOSIntegration.ResumeLayout(false);
            this.tabPOSIntegration.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tabPageManualSalesIntegration.ResumeLayout(false);
            this.tabPageManualSalesIntegration.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.tabFolderMonitoring.ResumeLayout(false);
            this.tabFolderMonitoring.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.FolderBrowserDialog fbdGetCSVTemplate;
        private System.IO.FileSystemWatcher fileSystemWatcherCSVFiles;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnSaveLogs;
        private System.Windows.Forms.Button btnClearLogs;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TabControl tabIntegration;
        private System.Windows.Forms.TabPage tabFolderMonitoring;
        private System.Windows.Forms.Button btnGetCSVTemplate;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtFolderMonitoringUserCode;
        private System.Windows.Forms.TextBox txtFolderMonitoringDomain;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button btnStartFolderMonitoringIntegration;
        private System.Windows.Forms.TextBox txtFolderMonitoringLogs;
        private System.Windows.Forms.TabPage tabPOSIntegration;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox cbxUseItemPrice;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtUserCode;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.DateTimePicker dtpIntegrationDate;
        private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtBranchCode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnStartIntegration;
        private System.Windows.Forms.Button btnStopIntegration;
        private System.Windows.Forms.Label lblCurrentUser;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLogs;
        private System.Windows.Forms.Panel panel6;
        private System.ComponentModel.BackgroundWorker bgwFolderMonitoringIntegration;
        private System.Windows.Forms.TabPage tabPageManualSalesIntegration;
        private System.Windows.Forms.Button buttonPOSManualSalesIntegrationStart;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.DateTimePicker dateTimePickerPOSManualSalesIntegrationDate;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label12;
        private System.ComponentModel.BackgroundWorker backgroundWorkerManualSalesIntegration;
        private System.Windows.Forms.TextBox textBoxPOSManualSalesIntegrationLogs;
    }
}