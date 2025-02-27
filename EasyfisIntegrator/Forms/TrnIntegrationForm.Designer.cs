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
            this.tabPagePOSSalesIntegration = new System.Windows.Forms.TabPage();
            this.buttonUpdateMasterFileInventory = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.checkBoxSalesIntegrationUseItemPrice = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxSalesIntegrationUserCode = new System.Windows.Forms.TextBox();
            this.dateTimePickerSalesIntegrationDate = new System.Windows.Forms.DateTimePicker();
            this.textBoxSalesIntegrationDomain = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxSalesIntegrationBranchCode = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.buttonSalesIntegrationStart = new System.Windows.Forms.Button();
            this.buttonSalesIntegrationStop = new System.Windows.Forms.Button();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.tabPagePOSManualSalesIntegration = new System.Windows.Forms.TabPage();
            this.buttonUpdateManualMasterFileInventory = new System.Windows.Forms.Button();
            this.buttonManualSalesIntegrationStop = new System.Windows.Forms.Button();
            this.textBoxPOSManualSalesIntegrationLogs = new System.Windows.Forms.TextBox();
            this.buttonManualSalesIntegrationStart = new System.Windows.Forms.Button();
            this.panel7 = new System.Windows.Forms.Panel();
            this.textBoxManualSalesIntegrationDomain = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.comboBoxManualSalesIntegrationTerminal = new System.Windows.Forms.ComboBox();
            this.dateTimePickerManualSalesIntegrationDate = new System.Windows.Forms.DateTimePicker();
            this.label17 = new System.Windows.Forms.Label();
            this.tabPageFolderMonitoringIntegration = new System.Windows.Forms.TabPage();
            this.dateTimePickerDeleteAllTransactions = new System.Windows.Forms.DateTimePicker();
            this.buttonFolderMonitoringDeleteAllTransactions = new System.Windows.Forms.Button();
            this.buttonFolderMonitoringIntegrationStop = new System.Windows.Forms.Button();
            this.btnGetCSVTemplate = new System.Windows.Forms.Button();
            this.comboBoxTransactionList = new System.Windows.Forms.ComboBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxFolderToMonitor = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxFolderMonitoringUserCode = new System.Windows.Forms.TextBox();
            this.textBoxFolderMonitoringDomain = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.buttonFolderMonitoringIntegrationStart = new System.Windows.Forms.Button();
            this.txtFolderMonitoringLogs = new System.Windows.Forms.TextBox();
            this.backgroundWorkerFolderMonitoringIntegration = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerManualSalesIntegration = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerSalesIntegration = new System.ComponentModel.BackgroundWorker();
            this.backgroundWorkerDeleteAllTransactions = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.tabIntegration.SuspendLayout();
            this.tabPagePOSSalesIntegration.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPagePOSManualSalesIntegration.SuspendLayout();
            this.panel7.SuspendLayout();
            this.tabPageFolderMonitoringIntegration.SuspendLayout();
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
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
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
            this.btnLogout.Margin = new System.Windows.Forms.Padding(2);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(92, 29);
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
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(292, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "EasyFIS Sales Integrator";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(11, 10);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
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
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 17);
            this.label4.TabIndex = 1;
            this.label4.Text = "Easyfis Integrator";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(88, 36);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(207, 17);
            this.label5.TabIndex = 2;
            this.label5.Text = "Version: 2.202502271336.NOR";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(88, 52);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
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
            this.panel2.Location = new System.Drawing.Point(0, 669);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
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
            this.btnSettings.Location = new System.Drawing.Point(752, 0);
            this.btnSettings.Margin = new System.Windows.Forms.Padding(2);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(108, 40);
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
            this.btnClearLogs.Location = new System.Drawing.Point(138, 0);
            this.btnClearLogs.Margin = new System.Windows.Forms.Padding(2);
            this.btnClearLogs.Name = "btnClearLogs";
            this.btnClearLogs.Size = new System.Drawing.Size(108, 40);
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
            this.btnSaveLogs.Margin = new System.Windows.Forms.Padding(2);
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
            this.panel5.Margin = new System.Windows.Forms.Padding(2);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(872, 618);
            this.panel5.TabIndex = 26;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.Controls.Add(this.btnSettings);
            this.panel6.Controls.Add(this.btnSaveLogs);
            this.panel6.Controls.Add(this.btnClearLogs);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 572);
            this.panel6.Margin = new System.Windows.Forms.Padding(2);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(872, 46);
            this.panel6.TabIndex = 26;
            // 
            // tabIntegration
            // 
            this.tabIntegration.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabIntegration.Controls.Add(this.tabPagePOSSalesIntegration);
            this.tabIntegration.Controls.Add(this.tabPagePOSManualSalesIntegration);
            this.tabIntegration.Controls.Add(this.tabPageFolderMonitoringIntegration);
            this.tabIntegration.Location = new System.Drawing.Point(0, 0);
            this.tabIntegration.Margin = new System.Windows.Forms.Padding(2);
            this.tabIntegration.Name = "tabIntegration";
            this.tabIntegration.SelectedIndex = 0;
            this.tabIntegration.Size = new System.Drawing.Size(872, 566);
            this.tabIntegration.TabIndex = 0;
            // 
            // tabPagePOSSalesIntegration
            // 
            this.tabPagePOSSalesIntegration.Controls.Add(this.buttonUpdateMasterFileInventory);
            this.tabPagePOSSalesIntegration.Controls.Add(this.panel3);
            this.tabPagePOSSalesIntegration.Controls.Add(this.buttonSalesIntegrationStart);
            this.tabPagePOSSalesIntegration.Controls.Add(this.buttonSalesIntegrationStop);
            this.tabPagePOSSalesIntegration.Controls.Add(this.txtLogs);
            this.tabPagePOSSalesIntegration.Location = new System.Drawing.Point(4, 25);
            this.tabPagePOSSalesIntegration.Margin = new System.Windows.Forms.Padding(2);
            this.tabPagePOSSalesIntegration.Name = "tabPagePOSSalesIntegration";
            this.tabPagePOSSalesIntegration.Padding = new System.Windows.Forms.Padding(2);
            this.tabPagePOSSalesIntegration.Size = new System.Drawing.Size(864, 537);
            this.tabPagePOSSalesIntegration.TabIndex = 0;
            this.tabPagePOSSalesIntegration.Text = "POS - Sales Integration";
            this.tabPagePOSSalesIntegration.UseVisualStyleBackColor = true;
            // 
            // buttonUpdateMasterFileInventory
            // 
            this.buttonUpdateMasterFileInventory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpdateMasterFileInventory.BackColor = System.Drawing.Color.SeaGreen;
            this.buttonUpdateMasterFileInventory.Enabled = false;
            this.buttonUpdateMasterFileInventory.FlatAppearance.BorderSize = 0;
            this.buttonUpdateMasterFileInventory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUpdateMasterFileInventory.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonUpdateMasterFileInventory.ForeColor = System.Drawing.Color.White;
            this.buttonUpdateMasterFileInventory.Location = new System.Drawing.Point(8, 191);
            this.buttonUpdateMasterFileInventory.Margin = new System.Windows.Forms.Padding(2);
            this.buttonUpdateMasterFileInventory.Name = "buttonUpdateMasterFileInventory";
            this.buttonUpdateMasterFileInventory.Size = new System.Drawing.Size(305, 38);
            this.buttonUpdateMasterFileInventory.TabIndex = 22;
            this.buttonUpdateMasterFileInventory.Text = "Update Master File and Inventory";
            this.buttonUpdateMasterFileInventory.UseVisualStyleBackColor = false;
            this.buttonUpdateMasterFileInventory.Click += new System.EventHandler(this.buttonUpdateMasterFileInventory_Click);
            // 
            // panel3
            // 
            this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel3.BackColor = System.Drawing.SystemColors.Control;
            this.panel3.Controls.Add(this.label11);
            this.panel3.Controls.Add(this.checkBoxSalesIntegrationUseItemPrice);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Controls.Add(this.textBoxSalesIntegrationUserCode);
            this.panel3.Controls.Add(this.dateTimePickerSalesIntegrationDate);
            this.panel3.Controls.Add(this.textBoxSalesIntegrationDomain);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.textBoxSalesIntegrationBranchCode);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Location = new System.Drawing.Point(8, 6);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(848, 180);
            this.panel3.TabIndex = 21;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label11.Location = new System.Drawing.Point(14, 144);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(125, 20);
            this.label11.TabIndex = 25;
            this.label11.Text = "Use Item Price:";
            // 
            // checkBoxSalesIntegrationUseItemPrice
            // 
            this.checkBoxSalesIntegrationUseItemPrice.AutoSize = true;
            this.checkBoxSalesIntegrationUseItemPrice.Enabled = false;
            this.checkBoxSalesIntegrationUseItemPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.checkBoxSalesIntegrationUseItemPrice.Location = new System.Drawing.Point(148, 146);
            this.checkBoxSalesIntegrationUseItemPrice.Margin = new System.Windows.Forms.Padding(2);
            this.checkBoxSalesIntegrationUseItemPrice.Name = "checkBoxSalesIntegrationUseItemPrice";
            this.checkBoxSalesIntegrationUseItemPrice.Size = new System.Drawing.Size(18, 17);
            this.checkBoxSalesIntegrationUseItemPrice.TabIndex = 24;
            this.checkBoxSalesIntegrationUseItemPrice.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label10.Location = new System.Drawing.Point(45, 114);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 20);
            this.label10.TabIndex = 22;
            this.label10.Text = "User Code:";
            // 
            // textBoxSalesIntegrationUserCode
            // 
            this.textBoxSalesIntegrationUserCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxSalesIntegrationUserCode.Location = new System.Drawing.Point(148, 110);
            this.textBoxSalesIntegrationUserCode.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSalesIntegrationUserCode.Name = "textBoxSalesIntegrationUserCode";
            this.textBoxSalesIntegrationUserCode.ReadOnly = true;
            this.textBoxSalesIntegrationUserCode.Size = new System.Drawing.Size(266, 26);
            this.textBoxSalesIntegrationUserCode.TabIndex = 23;
            // 
            // dateTimePickerSalesIntegrationDate
            // 
            this.dateTimePickerSalesIntegrationDate.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dateTimePickerSalesIntegrationDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dateTimePickerSalesIntegrationDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerSalesIntegrationDate.Location = new System.Drawing.Point(148, 15);
            this.dateTimePickerSalesIntegrationDate.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerSalesIntegrationDate.Name = "dateTimePickerSalesIntegrationDate";
            this.dateTimePickerSalesIntegrationDate.Size = new System.Drawing.Size(152, 26);
            this.dateTimePickerSalesIntegrationDate.TabIndex = 18;
            // 
            // textBoxSalesIntegrationDomain
            // 
            this.textBoxSalesIntegrationDomain.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxSalesIntegrationDomain.Location = new System.Drawing.Point(148, 46);
            this.textBoxSalesIntegrationDomain.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSalesIntegrationDomain.Name = "textBoxSalesIntegrationDomain";
            this.textBoxSalesIntegrationDomain.ReadOnly = true;
            this.textBoxSalesIntegrationDomain.Size = new System.Drawing.Size(432, 26);
            this.textBoxSalesIntegrationDomain.TabIndex = 20;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label7.Location = new System.Drawing.Point(28, 81);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(112, 20);
            this.label7.TabIndex = 15;
            this.label7.Text = "Branch Code:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label9.Location = new System.Drawing.Point(68, 50);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 20);
            this.label9.TabIndex = 19;
            this.label9.Text = "Domain:";
            // 
            // textBoxSalesIntegrationBranchCode
            // 
            this.textBoxSalesIntegrationBranchCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxSalesIntegrationBranchCode.Location = new System.Drawing.Point(148, 79);
            this.textBoxSalesIntegrationBranchCode.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxSalesIntegrationBranchCode.Name = "textBoxSalesIntegrationBranchCode";
            this.textBoxSalesIntegrationBranchCode.ReadOnly = true;
            this.textBoxSalesIntegrationBranchCode.Size = new System.Drawing.Size(266, 26);
            this.textBoxSalesIntegrationBranchCode.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label8.Location = new System.Drawing.Point(90, 20);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 20);
            this.label8.TabIndex = 17;
            this.label8.Text = "Date:";
            // 
            // buttonSalesIntegrationStart
            // 
            this.buttonSalesIntegrationStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSalesIntegrationStart.BackColor = System.Drawing.Color.SeaGreen;
            this.buttonSalesIntegrationStart.FlatAppearance.BorderSize = 0;
            this.buttonSalesIntegrationStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSalesIntegrationStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonSalesIntegrationStart.ForeColor = System.Drawing.Color.White;
            this.buttonSalesIntegrationStart.Location = new System.Drawing.Point(664, 191);
            this.buttonSalesIntegrationStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSalesIntegrationStart.Name = "buttonSalesIntegrationStart";
            this.buttonSalesIntegrationStart.Size = new System.Drawing.Size(92, 38);
            this.buttonSalesIntegrationStart.TabIndex = 11;
            this.buttonSalesIntegrationStart.Text = "Start";
            this.buttonSalesIntegrationStart.UseVisualStyleBackColor = false;
            this.buttonSalesIntegrationStart.Click += new System.EventHandler(this.btnStartIntegration_Click);
            // 
            // buttonSalesIntegrationStop
            // 
            this.buttonSalesIntegrationStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSalesIntegrationStop.BackColor = System.Drawing.Color.IndianRed;
            this.buttonSalesIntegrationStop.Enabled = false;
            this.buttonSalesIntegrationStop.FlatAppearance.BorderSize = 0;
            this.buttonSalesIntegrationStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSalesIntegrationStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonSalesIntegrationStop.ForeColor = System.Drawing.Color.White;
            this.buttonSalesIntegrationStop.Location = new System.Drawing.Point(762, 191);
            this.buttonSalesIntegrationStop.Margin = new System.Windows.Forms.Padding(2);
            this.buttonSalesIntegrationStop.Name = "buttonSalesIntegrationStop";
            this.buttonSalesIntegrationStop.Size = new System.Drawing.Size(92, 38);
            this.buttonSalesIntegrationStop.TabIndex = 10;
            this.buttonSalesIntegrationStop.Text = "Stop";
            this.buttonSalesIntegrationStop.UseVisualStyleBackColor = false;
            this.buttonSalesIntegrationStop.Click += new System.EventHandler(this.btnStopIntegration_Click);
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
            this.txtLogs.Location = new System.Drawing.Point(8, 234);
            this.txtLogs.Margin = new System.Windows.Forms.Padding(2);
            this.txtLogs.MaxLength = 0;
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.Size = new System.Drawing.Size(848, 294);
            this.txtLogs.TabIndex = 14;
            // 
            // tabPagePOSManualSalesIntegration
            // 
            this.tabPagePOSManualSalesIntegration.Controls.Add(this.buttonUpdateManualMasterFileInventory);
            this.tabPagePOSManualSalesIntegration.Controls.Add(this.buttonManualSalesIntegrationStop);
            this.tabPagePOSManualSalesIntegration.Controls.Add(this.textBoxPOSManualSalesIntegrationLogs);
            this.tabPagePOSManualSalesIntegration.Controls.Add(this.buttonManualSalesIntegrationStart);
            this.tabPagePOSManualSalesIntegration.Controls.Add(this.panel7);
            this.tabPagePOSManualSalesIntegration.Location = new System.Drawing.Point(4, 25);
            this.tabPagePOSManualSalesIntegration.Margin = new System.Windows.Forms.Padding(2);
            this.tabPagePOSManualSalesIntegration.Name = "tabPagePOSManualSalesIntegration";
            this.tabPagePOSManualSalesIntegration.Padding = new System.Windows.Forms.Padding(2);
            this.tabPagePOSManualSalesIntegration.Size = new System.Drawing.Size(864, 537);
            this.tabPagePOSManualSalesIntegration.TabIndex = 2;
            this.tabPagePOSManualSalesIntegration.Text = "POS - Manual Sales Integration";
            this.tabPagePOSManualSalesIntegration.UseVisualStyleBackColor = true;
            // 
            // buttonUpdateManualMasterFileInventory
            // 
            this.buttonUpdateManualMasterFileInventory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpdateManualMasterFileInventory.BackColor = System.Drawing.Color.SeaGreen;
            this.buttonUpdateManualMasterFileInventory.Enabled = false;
            this.buttonUpdateManualMasterFileInventory.FlatAppearance.BorderSize = 0;
            this.buttonUpdateManualMasterFileInventory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUpdateManualMasterFileInventory.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonUpdateManualMasterFileInventory.ForeColor = System.Drawing.Color.White;
            this.buttonUpdateManualMasterFileInventory.Location = new System.Drawing.Point(8, 139);
            this.buttonUpdateManualMasterFileInventory.Margin = new System.Windows.Forms.Padding(2);
            this.buttonUpdateManualMasterFileInventory.Name = "buttonUpdateManualMasterFileInventory";
            this.buttonUpdateManualMasterFileInventory.Size = new System.Drawing.Size(305, 38);
            this.buttonUpdateManualMasterFileInventory.TabIndex = 27;
            this.buttonUpdateManualMasterFileInventory.Text = "Update Master File and Inventory";
            this.buttonUpdateManualMasterFileInventory.UseVisualStyleBackColor = false;
            this.buttonUpdateManualMasterFileInventory.Click += new System.EventHandler(this.buttonUpdateManualMasterFileInventory_Click);
            // 
            // buttonManualSalesIntegrationStop
            // 
            this.buttonManualSalesIntegrationStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonManualSalesIntegrationStop.BackColor = System.Drawing.Color.IndianRed;
            this.buttonManualSalesIntegrationStop.Enabled = false;
            this.buttonManualSalesIntegrationStop.FlatAppearance.BorderSize = 0;
            this.buttonManualSalesIntegrationStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonManualSalesIntegrationStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonManualSalesIntegrationStop.ForeColor = System.Drawing.Color.White;
            this.buttonManualSalesIntegrationStop.Location = new System.Drawing.Point(761, 139);
            this.buttonManualSalesIntegrationStop.Margin = new System.Windows.Forms.Padding(2);
            this.buttonManualSalesIntegrationStop.Name = "buttonManualSalesIntegrationStop";
            this.buttonManualSalesIntegrationStop.Size = new System.Drawing.Size(92, 38);
            this.buttonManualSalesIntegrationStop.TabIndex = 26;
            this.buttonManualSalesIntegrationStop.Text = "Stop";
            this.buttonManualSalesIntegrationStop.UseVisualStyleBackColor = false;
            this.buttonManualSalesIntegrationStop.Click += new System.EventHandler(this.buttonManualSalesIntegrationStop_Click);
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
            this.textBoxPOSManualSalesIntegrationLogs.Location = new System.Drawing.Point(8, 181);
            this.textBoxPOSManualSalesIntegrationLogs.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxPOSManualSalesIntegrationLogs.MaxLength = 0;
            this.textBoxPOSManualSalesIntegrationLogs.Multiline = true;
            this.textBoxPOSManualSalesIntegrationLogs.Name = "textBoxPOSManualSalesIntegrationLogs";
            this.textBoxPOSManualSalesIntegrationLogs.ReadOnly = true;
            this.textBoxPOSManualSalesIntegrationLogs.Size = new System.Drawing.Size(848, 347);
            this.textBoxPOSManualSalesIntegrationLogs.TabIndex = 25;
            // 
            // buttonManualSalesIntegrationStart
            // 
            this.buttonManualSalesIntegrationStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonManualSalesIntegrationStart.BackColor = System.Drawing.Color.SeaGreen;
            this.buttonManualSalesIntegrationStart.FlatAppearance.BorderSize = 0;
            this.buttonManualSalesIntegrationStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonManualSalesIntegrationStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonManualSalesIntegrationStart.ForeColor = System.Drawing.Color.White;
            this.buttonManualSalesIntegrationStart.Location = new System.Drawing.Point(664, 139);
            this.buttonManualSalesIntegrationStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonManualSalesIntegrationStart.Name = "buttonManualSalesIntegrationStart";
            this.buttonManualSalesIntegrationStart.Size = new System.Drawing.Size(92, 38);
            this.buttonManualSalesIntegrationStart.TabIndex = 24;
            this.buttonManualSalesIntegrationStart.Text = "Start";
            this.buttonManualSalesIntegrationStart.UseVisualStyleBackColor = false;
            this.buttonManualSalesIntegrationStart.Click += new System.EventHandler(this.buttonPOSManualSalesIntegrationStart_Click);
            // 
            // panel7
            // 
            this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel7.BackColor = System.Drawing.SystemColors.Control;
            this.panel7.Controls.Add(this.textBoxManualSalesIntegrationDomain);
            this.panel7.Controls.Add(this.label15);
            this.panel7.Controls.Add(this.label13);
            this.panel7.Controls.Add(this.comboBoxManualSalesIntegrationTerminal);
            this.panel7.Controls.Add(this.dateTimePickerManualSalesIntegrationDate);
            this.panel7.Controls.Add(this.label17);
            this.panel7.Location = new System.Drawing.Point(8, 6);
            this.panel7.Margin = new System.Windows.Forms.Padding(2);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(848, 128);
            this.panel7.TabIndex = 22;
            // 
            // textBoxManualSalesIntegrationDomain
            // 
            this.textBoxManualSalesIntegrationDomain.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxManualSalesIntegrationDomain.Location = new System.Drawing.Point(101, 46);
            this.textBoxManualSalesIntegrationDomain.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxManualSalesIntegrationDomain.Name = "textBoxManualSalesIntegrationDomain";
            this.textBoxManualSalesIntegrationDomain.ReadOnly = true;
            this.textBoxManualSalesIntegrationDomain.Size = new System.Drawing.Size(432, 26);
            this.textBoxManualSalesIntegrationDomain.TabIndex = 30;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label15.Location = new System.Drawing.Point(21, 50);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(72, 20);
            this.label15.TabIndex = 29;
            this.label15.Text = "Domain:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label13.Location = new System.Drawing.Point(12, 82);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(79, 20);
            this.label13.TabIndex = 28;
            this.label13.Text = "Terminal:";
            // 
            // comboBoxManualSalesIntegrationTerminal
            // 
            this.comboBoxManualSalesIntegrationTerminal.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.comboBoxManualSalesIntegrationTerminal.FormattingEnabled = true;
            this.comboBoxManualSalesIntegrationTerminal.Location = new System.Drawing.Point(101, 79);
            this.comboBoxManualSalesIntegrationTerminal.Margin = new System.Windows.Forms.Padding(2);
            this.comboBoxManualSalesIntegrationTerminal.Name = "comboBoxManualSalesIntegrationTerminal";
            this.comboBoxManualSalesIntegrationTerminal.Size = new System.Drawing.Size(152, 28);
            this.comboBoxManualSalesIntegrationTerminal.TabIndex = 27;
            // 
            // dateTimePickerManualSalesIntegrationDate
            // 
            this.dateTimePickerManualSalesIntegrationDate.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dateTimePickerManualSalesIntegrationDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.dateTimePickerManualSalesIntegrationDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerManualSalesIntegrationDate.Location = new System.Drawing.Point(101, 15);
            this.dateTimePickerManualSalesIntegrationDate.Margin = new System.Windows.Forms.Padding(2);
            this.dateTimePickerManualSalesIntegrationDate.Name = "dateTimePickerManualSalesIntegrationDate";
            this.dateTimePickerManualSalesIntegrationDate.Size = new System.Drawing.Size(152, 26);
            this.dateTimePickerManualSalesIntegrationDate.TabIndex = 18;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label17.Location = new System.Drawing.Point(44, 20);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(50, 20);
            this.label17.TabIndex = 17;
            this.label17.Text = "Date:";
            // 
            // tabPageFolderMonitoringIntegration
            // 
            this.tabPageFolderMonitoringIntegration.Controls.Add(this.dateTimePickerDeleteAllTransactions);
            this.tabPageFolderMonitoringIntegration.Controls.Add(this.buttonFolderMonitoringDeleteAllTransactions);
            this.tabPageFolderMonitoringIntegration.Controls.Add(this.buttonFolderMonitoringIntegrationStop);
            this.tabPageFolderMonitoringIntegration.Controls.Add(this.btnGetCSVTemplate);
            this.tabPageFolderMonitoringIntegration.Controls.Add(this.comboBoxTransactionList);
            this.tabPageFolderMonitoringIntegration.Controls.Add(this.panel4);
            this.tabPageFolderMonitoringIntegration.Controls.Add(this.buttonFolderMonitoringIntegrationStart);
            this.tabPageFolderMonitoringIntegration.Controls.Add(this.txtFolderMonitoringLogs);
            this.tabPageFolderMonitoringIntegration.Location = new System.Drawing.Point(4, 25);
            this.tabPageFolderMonitoringIntegration.Margin = new System.Windows.Forms.Padding(2);
            this.tabPageFolderMonitoringIntegration.Name = "tabPageFolderMonitoringIntegration";
            this.tabPageFolderMonitoringIntegration.Padding = new System.Windows.Forms.Padding(2);
            this.tabPageFolderMonitoringIntegration.Size = new System.Drawing.Size(864, 537);
            this.tabPageFolderMonitoringIntegration.TabIndex = 1;
            this.tabPageFolderMonitoringIntegration.Text = "Folder Monitoring";
            this.tabPageFolderMonitoringIntegration.UseVisualStyleBackColor = true;
            // 
            // dateTimePickerDeleteAllTransactions
            // 
            this.dateTimePickerDeleteAllTransactions.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerDeleteAllTransactions.Location = new System.Drawing.Point(382, 146);
            this.dateTimePickerDeleteAllTransactions.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dateTimePickerDeleteAllTransactions.Name = "dateTimePickerDeleteAllTransactions";
            this.dateTimePickerDeleteAllTransactions.Size = new System.Drawing.Size(102, 22);
            this.dateTimePickerDeleteAllTransactions.TabIndex = 28;
            // 
            // buttonFolderMonitoringDeleteAllTransactions
            // 
            this.buttonFolderMonitoringDeleteAllTransactions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFolderMonitoringDeleteAllTransactions.BackColor = System.Drawing.Color.IndianRed;
            this.buttonFolderMonitoringDeleteAllTransactions.FlatAppearance.BorderSize = 0;
            this.buttonFolderMonitoringDeleteAllTransactions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFolderMonitoringDeleteAllTransactions.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonFolderMonitoringDeleteAllTransactions.ForeColor = System.Drawing.Color.White;
            this.buttonFolderMonitoringDeleteAllTransactions.Location = new System.Drawing.Point(491, 139);
            this.buttonFolderMonitoringDeleteAllTransactions.Margin = new System.Windows.Forms.Padding(2);
            this.buttonFolderMonitoringDeleteAllTransactions.Name = "buttonFolderMonitoringDeleteAllTransactions";
            this.buttonFolderMonitoringDeleteAllTransactions.Size = new System.Drawing.Size(92, 38);
            this.buttonFolderMonitoringDeleteAllTransactions.TabIndex = 24;
            this.buttonFolderMonitoringDeleteAllTransactions.Text = "Delete";
            this.buttonFolderMonitoringDeleteAllTransactions.UseVisualStyleBackColor = false;
            this.buttonFolderMonitoringDeleteAllTransactions.Click += new System.EventHandler(this.buttonFolderMonitoringDeleteAllTransactions_Click);
            // 
            // buttonFolderMonitoringIntegrationStop
            // 
            this.buttonFolderMonitoringIntegrationStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFolderMonitoringIntegrationStop.BackColor = System.Drawing.Color.IndianRed;
            this.buttonFolderMonitoringIntegrationStop.Enabled = false;
            this.buttonFolderMonitoringIntegrationStop.FlatAppearance.BorderSize = 0;
            this.buttonFolderMonitoringIntegrationStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFolderMonitoringIntegrationStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonFolderMonitoringIntegrationStop.ForeColor = System.Drawing.Color.White;
            this.buttonFolderMonitoringIntegrationStop.Location = new System.Drawing.Point(761, 139);
            this.buttonFolderMonitoringIntegrationStop.Margin = new System.Windows.Forms.Padding(2);
            this.buttonFolderMonitoringIntegrationStop.Name = "buttonFolderMonitoringIntegrationStop";
            this.buttonFolderMonitoringIntegrationStop.Size = new System.Drawing.Size(92, 38);
            this.buttonFolderMonitoringIntegrationStop.TabIndex = 27;
            this.buttonFolderMonitoringIntegrationStop.Text = "Stop";
            this.buttonFolderMonitoringIntegrationStop.UseVisualStyleBackColor = false;
            this.buttonFolderMonitoringIntegrationStop.Click += new System.EventHandler(this.buttonFolderMonitoringIntegrationStop_Click);
            // 
            // btnGetCSVTemplate
            // 
            this.btnGetCSVTemplate.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnGetCSVTemplate.FlatAppearance.BorderSize = 0;
            this.btnGetCSVTemplate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGetCSVTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.btnGetCSVTemplate.ForeColor = System.Drawing.Color.White;
            this.btnGetCSVTemplate.Location = new System.Drawing.Point(8, 139);
            this.btnGetCSVTemplate.Margin = new System.Windows.Forms.Padding(2);
            this.btnGetCSVTemplate.Name = "btnGetCSVTemplate";
            this.btnGetCSVTemplate.Size = new System.Drawing.Size(220, 38);
            this.btnGetCSVTemplate.TabIndex = 23;
            this.btnGetCSVTemplate.Text = "Get CSV Template";
            this.btnGetCSVTemplate.UseVisualStyleBackColor = false;
            this.btnGetCSVTemplate.Click += new System.EventHandler(this.btnGetCSVTemplate_Click);
            // 
            // comboBoxTransactionList
            // 
            this.comboBoxTransactionList.FormattingEnabled = true;
            this.comboBoxTransactionList.Items.AddRange(new object[] {
            "OR",
            "CV",
            "JV",
            "RR",
            "SI",
            "IN",
            "OT",
            "ST"});
            this.comboBoxTransactionList.Location = new System.Drawing.Point(234, 146);
            this.comboBoxTransactionList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxTransactionList.Name = "comboBoxTransactionList";
            this.comboBoxTransactionList.Size = new System.Drawing.Size(139, 24);
            this.comboBoxTransactionList.TabIndex = 23;
            this.comboBoxTransactionList.Text = "OR";
            // 
            // panel4
            // 
            this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Controls.Add(this.label12);
            this.panel4.Controls.Add(this.textBoxFolderToMonitor);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.textBoxFolderMonitoringUserCode);
            this.panel4.Controls.Add(this.textBoxFolderMonitoringDomain);
            this.panel4.Controls.Add(this.label14);
            this.panel4.Location = new System.Drawing.Point(8, 6);
            this.panel4.Margin = new System.Windows.Forms.Padding(2);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(848, 128);
            this.panel4.TabIndex = 22;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label12.Location = new System.Drawing.Point(14, 80);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(141, 20);
            this.label12.TabIndex = 26;
            this.label12.Text = "Folder to Monitor:";
            // 
            // textBoxFolderToMonitor
            // 
            this.textBoxFolderToMonitor.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxFolderToMonitor.Location = new System.Drawing.Point(168, 76);
            this.textBoxFolderToMonitor.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxFolderToMonitor.Name = "textBoxFolderToMonitor";
            this.textBoxFolderToMonitor.ReadOnly = true;
            this.textBoxFolderToMonitor.Size = new System.Drawing.Size(543, 26);
            this.textBoxFolderToMonitor.TabIndex = 27;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label3.Location = new System.Drawing.Point(64, 49);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 20);
            this.label3.TabIndex = 24;
            this.label3.Text = "User Code:";
            // 
            // textBoxFolderMonitoringUserCode
            // 
            this.textBoxFolderMonitoringUserCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxFolderMonitoringUserCode.Location = new System.Drawing.Point(168, 45);
            this.textBoxFolderMonitoringUserCode.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxFolderMonitoringUserCode.Name = "textBoxFolderMonitoringUserCode";
            this.textBoxFolderMonitoringUserCode.ReadOnly = true;
            this.textBoxFolderMonitoringUserCode.Size = new System.Drawing.Size(266, 26);
            this.textBoxFolderMonitoringUserCode.TabIndex = 25;
            // 
            // textBoxFolderMonitoringDomain
            // 
            this.textBoxFolderMonitoringDomain.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.textBoxFolderMonitoringDomain.Location = new System.Drawing.Point(168, 14);
            this.textBoxFolderMonitoringDomain.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxFolderMonitoringDomain.Name = "textBoxFolderMonitoringDomain";
            this.textBoxFolderMonitoringDomain.ReadOnly = true;
            this.textBoxFolderMonitoringDomain.Size = new System.Drawing.Size(432, 26);
            this.textBoxFolderMonitoringDomain.TabIndex = 20;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.label14.Location = new System.Drawing.Point(88, 18);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(72, 20);
            this.label14.TabIndex = 19;
            this.label14.Text = "Domain:";
            // 
            // buttonFolderMonitoringIntegrationStart
            // 
            this.buttonFolderMonitoringIntegrationStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonFolderMonitoringIntegrationStart.BackColor = System.Drawing.Color.SeaGreen;
            this.buttonFolderMonitoringIntegrationStart.FlatAppearance.BorderSize = 0;
            this.buttonFolderMonitoringIntegrationStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFolderMonitoringIntegrationStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonFolderMonitoringIntegrationStart.ForeColor = System.Drawing.Color.White;
            this.buttonFolderMonitoringIntegrationStart.Location = new System.Drawing.Point(664, 139);
            this.buttonFolderMonitoringIntegrationStart.Margin = new System.Windows.Forms.Padding(2);
            this.buttonFolderMonitoringIntegrationStart.Name = "buttonFolderMonitoringIntegrationStart";
            this.buttonFolderMonitoringIntegrationStart.Size = new System.Drawing.Size(92, 38);
            this.buttonFolderMonitoringIntegrationStart.TabIndex = 16;
            this.buttonFolderMonitoringIntegrationStart.Text = "Start";
            this.buttonFolderMonitoringIntegrationStart.UseVisualStyleBackColor = false;
            this.buttonFolderMonitoringIntegrationStart.Click += new System.EventHandler(this.btnStartFolderMonitoringIntegration_Click);
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
            this.txtFolderMonitoringLogs.Location = new System.Drawing.Point(8, 181);
            this.txtFolderMonitoringLogs.Margin = new System.Windows.Forms.Padding(2);
            this.txtFolderMonitoringLogs.MaxLength = 0;
            this.txtFolderMonitoringLogs.Multiline = true;
            this.txtFolderMonitoringLogs.Name = "txtFolderMonitoringLogs";
            this.txtFolderMonitoringLogs.ReadOnly = true;
            this.txtFolderMonitoringLogs.Size = new System.Drawing.Size(848, 347);
            this.txtFolderMonitoringLogs.TabIndex = 17;
            // 
            // backgroundWorkerFolderMonitoringIntegration
            // 
            this.backgroundWorkerFolderMonitoringIntegration.WorkerReportsProgress = true;
            this.backgroundWorkerFolderMonitoringIntegration.WorkerSupportsCancellation = true;
            this.backgroundWorkerFolderMonitoringIntegration.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerFolderMonitoringIntegration_DoWork);
            // 
            // backgroundWorkerManualSalesIntegration
            // 
            this.backgroundWorkerManualSalesIntegration.WorkerReportsProgress = true;
            this.backgroundWorkerManualSalesIntegration.WorkerSupportsCancellation = true;
            this.backgroundWorkerManualSalesIntegration.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerManualSalesIntegration_DoWork);
            // 
            // backgroundWorkerSalesIntegration
            // 
            this.backgroundWorkerSalesIntegration.WorkerReportsProgress = true;
            this.backgroundWorkerSalesIntegration.WorkerSupportsCancellation = true;
            this.backgroundWorkerSalesIntegration.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerSalesIntegration_DoWork);
            // 
            // backgroundWorkerDeleteAllTransactions
            // 
            this.backgroundWorkerDeleteAllTransactions.WorkerReportsProgress = true;
            this.backgroundWorkerDeleteAllTransactions.WorkerSupportsCancellation = true;
            this.backgroundWorkerDeleteAllTransactions.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerDeleteAllTransactions_DoWork);
            // 
            // TrnIntegrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(872, 749);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "TrnIntegrationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EasyFIS Sales Integrator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TrnInnosoftPOSIntegrationForm_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.tabIntegration.ResumeLayout(false);
            this.tabPagePOSSalesIntegration.ResumeLayout(false);
            this.tabPagePOSSalesIntegration.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabPagePOSManualSalesIntegration.ResumeLayout(false);
            this.tabPagePOSManualSalesIntegration.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.tabPageFolderMonitoringIntegration.ResumeLayout(false);
            this.tabPageFolderMonitoringIntegration.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.FolderBrowserDialog fbdGetCSVTemplate;
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
        private System.Windows.Forms.TabPage tabPageFolderMonitoringIntegration;
        private System.Windows.Forms.Button btnGetCSVTemplate;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxFolderMonitoringUserCode;
        private System.Windows.Forms.TextBox textBoxFolderMonitoringDomain;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button buttonFolderMonitoringIntegrationStart;
        private System.Windows.Forms.TextBox txtFolderMonitoringLogs;
        private System.Windows.Forms.TabPage tabPagePOSSalesIntegration;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox checkBoxSalesIntegrationUseItemPrice;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxSalesIntegrationUserCode;
        private System.Windows.Forms.DateTimePicker dateTimePickerSalesIntegrationDate;
        private System.Windows.Forms.TextBox textBoxSalesIntegrationDomain;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxSalesIntegrationBranchCode;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button buttonSalesIntegrationStart;
        private System.Windows.Forms.Button buttonSalesIntegrationStop;
        private System.Windows.Forms.TextBox txtLogs;
        private System.Windows.Forms.Panel panel6;
        private System.ComponentModel.BackgroundWorker backgroundWorkerFolderMonitoringIntegration;
        private System.Windows.Forms.TabPage tabPagePOSManualSalesIntegration;
        private System.Windows.Forms.Button buttonManualSalesIntegrationStart;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.DateTimePicker dateTimePickerManualSalesIntegrationDate;
        private System.Windows.Forms.Label label17;
        private System.ComponentModel.BackgroundWorker backgroundWorkerManualSalesIntegration;
        private System.Windows.Forms.TextBox textBoxPOSManualSalesIntegrationLogs;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox comboBoxManualSalesIntegrationTerminal;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSalesIntegration;
        private System.Windows.Forms.TextBox textBoxManualSalesIntegrationDomain;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxFolderToMonitor;
        private System.Windows.Forms.Button buttonManualSalesIntegrationStop;
        private System.Windows.Forms.Button buttonFolderMonitoringIntegrationStop;
        private System.Windows.Forms.Button buttonUpdateMasterFileInventory;
        private System.Windows.Forms.Button buttonUpdateManualMasterFileInventory;
        private System.Windows.Forms.Button buttonFolderMonitoringDeleteAllTransactions;
        private System.Windows.Forms.ComboBox comboBoxTransactionList;
        private System.Windows.Forms.DateTimePicker dateTimePickerDeleteAllTransactions;
        private System.ComponentModel.BackgroundWorker backgroundWorkerDeleteAllTransactions;
    }
}