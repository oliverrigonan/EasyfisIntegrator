using EasyfisIntegrator.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace EasyfisIntegrator.Forms
{
    public partial class TrnIntegrationForm : Form
    {
        private InnosoftPOSData.InnosoftPOSDataDataContext posdb = new InnosoftPOSData.InnosoftPOSDataDataContext(Controllers.SysGlobalSettings.getConnectionString());

        public SysLoginForm sysLoginForm;
        public Boolean isIntegrationStarted = false;
        public Boolean isSettingsClicked = false;
        private Timer integrationTimer = new Timer();

        public Boolean isIntegrating = false;
        public Boolean isIntegratingCustomer = false;
        public Boolean isIntegratingItem = false;
        public Boolean isIntegratingSupplier = false;
        public Boolean isIntegratingCollection = false;
        public Boolean isIntegratingItemPrice = false;
        public Boolean isIntegratingReceivingReceipt = false;
        public Boolean isIntegratingSalesReturn = false;
        public Boolean isIntegratingStockIn = false;
        public Boolean isIntegratingStockOut = false;
        public Boolean isIntegratingTransferIn = false;
        public Boolean isIntegratingTransferOut = false;
        public Int32 logMessageCount = 0;

        public Boolean isFolderMonitoringOnly = false;
        public String folderToMonitor = "";
        public String domain = "";
        public Boolean isFolderMonitoringIntegrationStarted = false;

        public Boolean ManualSalesIntegration = false;

        public TrnIntegrationForm()
        {
            InitializeComponent();

            //bgwFolderMonitoringIntegration.DoWork += new DoWorkEventHandler(bgwSalesInvoice_DoWork);

            isSettingsClicked = false;

            logMessages("Press start button to integrate. \r\n\n" + "\r\n\n");
            getSettings();

            logFolderMonitoringMessage("Press start button to integrate. \r\n\n" + "\r\n\n");

            fileSystemWatcherCSVFiles.Path = folderToMonitor;
            GetTerminalList();
        }

        public void getLoginDetails(SysLoginForm form)
        {
            sysLoginForm = form;
            lblCurrentUser.Text = sysLoginForm.currentUser;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (isIntegrationStarted)
            {
                MessageBox.Show("Please stop the integration first.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                isFolderMonitoringIntegrationStarted = false;

                Hide();

                SysLoginForm sysLoginForm = new SysLoginForm();
                sysLoginForm.Show();
            }
        }

        public void getSettings()
        {
            stopIntegration();

            String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

            String json;
            using (StreamReader trmRead = new StreamReader(settingsPath)) { json = trmRead.ReadToEnd(); }

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(json);

            txtDomain.Text = sysSettings.Domain;
            txtFolderMonitoringDomain.Text = sysSettings.Domain;
            txtFolderMonitoringUserCode.Text = sysSettings.FolderMonitoringUserCode;
            isFolderMonitoringOnly = sysSettings.IsFolderMonitoringOnly;
            folderToMonitor = sysSettings.FolderToMonitor;
            fileSystemWatcherCSVFiles.Path = folderToMonitor;
            domain = sysSettings.Domain;
            ManualSalesIntegration = sysSettings.ManualSalesIntegration;

            if (isFolderMonitoringOnly)
            {
                tabPOSIntegration.Enabled = false;
                tabFolderMonitoring.Enabled = true;
                tabIntegration.SelectedTab = tabFolderMonitoring;
                tabIntegration.TabPages.Remove(tabPOSIntegration);
                tabIntegration.TabPages.Remove(tabPageManualSalesIntegration);

                btnLogout.Visible = false;
            }
            else
            {
                tabPOSIntegration.Enabled = true;
                tabFolderMonitoring.Enabled = true;
                tabIntegration.SelectedTab = tabPOSIntegration;

                btnLogout.Visible = true;

                var settings = from d in posdb.SysSettings select d;
                if (settings.Any())
                {
                    posdb.Refresh(RefreshMode.OverwriteCurrentValues, settings);

                    txtBranchCode.Text = settings.FirstOrDefault().BranchCode;
                    txtUserCode.Text = settings.FirstOrDefault().UserCode;
                    cbxUseItemPrice.Checked = settings.FirstOrDefault().UseItemPrice;
                }
            }

            btnStartFolderMonitoringIntegration.Enabled = true;

            if (ManualSalesIntegration == false)
            {
                buttonPOSManualSalesIntegrationStart.Enabled = false;
            }
            else
            {
                buttonPOSManualSalesIntegrationStart.Enabled = true;
            }
        }

        private void TrnInnosoftPOSIntegrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Activate();

            btnStartFolderMonitoringIntegration.Enabled = true;

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to close this application?", "Close Integrator", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                if (isFolderMonitoringOnly)
                {
                    isFolderMonitoringIntegrationStarted = false;

                    Hide();

                    MainForm mainForm = new MainForm();
                    mainForm.Show();
                }
                else
                {
                    if (isIntegrationStarted)
                    {
                        MessageBox.Show("Please stop the integration first.", "Close Integrator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        Hide();

                        MainForm mainForm = new MainForm();
                        mainForm.Show();
                    }
                }
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            isSettingsClicked = true;

            SysSettings sysSettings = new SysSettings(this);
            sysSettings.ShowDialog();
        }

        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to clear all logs? You can't undo changes anymore.", "Clear Logs", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                txtLogs.Text = "Press start button to integrate. \r\n\n" + "\r\n\n";
                txtFolderMonitoringLogs.Text = "Press start button to integrate. \r\n\n" + "\r\n\n";
            }
        }

        private void btnSaveLogs_Click(object sender, EventArgs e)
        {
            try
            {
                String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

                String json;
                using (StreamReader trmRead = new StreamReader(settingsPath)) { json = trmRead.ReadToEnd(); }

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(json);

                File.WriteAllText(sysSettings.LogFileLocation + "\\ISPOS_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt", txtLogs.Text);
                File.WriteAllText(sysSettings.LogFileLocation + "\\FM_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt", txtFolderMonitoringLogs.Text);

                MessageBox.Show("Save Log Successful!", "Save Logs", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (DirectoryNotFoundException drex)
            {
                MessageBox.Show(drex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStartIntegration_Click(object sender, EventArgs e)
        {
            startIntegration();
        }

        public void startIntegration()
        {
            isIntegrationStarted = true;

            btnStartIntegration.Enabled = false;
            btnStopIntegration.Enabled = false;

            dtpIntegrationDate.Enabled = false;
            btnSettings.Enabled = false;

            btnClearLogs.Enabled = false;
            btnSaveLogs.Enabled = false;

            btnLogout.Enabled = false;

            logMessages("Started! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n");

            isIntegrating = false;

            integrationTimer = new Timer();
            integrationTimer.Interval = 5000;
            integrationTimer.Tick += new EventHandler(integrationTimerTick);
            integrationTimer.Enabled = true;
        }

        public void integrationTimerTick(object sender, EventArgs e)
        {
            integrationTimer.Enabled = false;
            integrate();
        }

        public void integrate()
        {
            if (isIntegrating == false)
            {
                btnStopIntegration.Enabled = true;

                String apiUrlHost = txtDomain.Text;

                var sysSettings = from d in posdb.SysSettings select d;
                if (sysSettings.Any())
                {
                    var branchCode = sysSettings.FirstOrDefault().BranchCode;
                    var userCode = sysSettings.FirstOrDefault().UserCode;
                    var useItemPrice = sysSettings.FirstOrDefault().UseItemPrice;

                    isIntegrating = true;

                    isIntegratingCustomer = true;
                    isIntegratingItem = true;
                    isIntegratingSupplier = true;
                    isIntegratingCollection = true;
                    if (useItemPrice) { isIntegratingItemPrice = true; }
                    isIntegratingReceivingReceipt = true;
                    isIntegratingSalesReturn = true;
                    isIntegratingStockIn = true;
                    isIntegratingStockOut = true;
                    isIntegratingTransferIn = true;
                    isIntegratingTransferOut = true;

                    Controllers.ISPOSMstItemController objMstItem = new Controllers.ISPOSMstItemController(this, dtpIntegrationDate.Text);
                    Controllers.ISPOSMstCustomerController objMstCustomer = new Controllers.ISPOSMstCustomerController(this, dtpIntegrationDate.Text);
                    Controllers.ISPOSMstSupplierController objMstSupplier = new Controllers.ISPOSMstSupplierController(this, dtpIntegrationDate.Text);
                    Controllers.ISPOSTrnStockTransferInController objTrnStockTransferIn = new Controllers.ISPOSTrnStockTransferInController(this, dtpIntegrationDate.Text);
                    Controllers.ISPOSTrnStockTransferOutController objTrnStockTransferOut = new Controllers.ISPOSTrnStockTransferOutController(this, dtpIntegrationDate.Text);
                    Controllers.ISPOSTrnStockInController objTrnStockIn = new Controllers.ISPOSTrnStockInController(this, dtpIntegrationDate.Text);
                    Controllers.ISPOSTrnStockOutController objTrnStockOut = new Controllers.ISPOSTrnStockOutController(this, dtpIntegrationDate.Text);
                    Controllers.ISPOSTrnReceivingReceiptController objTrnReceivingReceipt = new Controllers.ISPOSTrnReceivingReceiptController(this, dtpIntegrationDate.Text);
                    Controllers.ISPOSTrnItemPriceController objTrnItemPrice = new Controllers.ISPOSTrnItemPriceController(this, dtpIntegrationDate.Text);

                    objMstCustomer.GetCustomer(apiUrlHost);
                    objMstSupplier.GetSupplier(apiUrlHost);
                    objMstItem.GetItem(apiUrlHost);
                    objTrnReceivingReceipt.GetReceivingReceipt(apiUrlHost, branchCode);
                    objTrnStockIn.GetStockIn(apiUrlHost, branchCode);
                    objTrnStockOut.GetStockOut(apiUrlHost, branchCode);
                    objTrnStockTransferIn.GetStockTransferIN(apiUrlHost, branchCode);
                    objTrnStockTransferOut.GetStockTransferOT(apiUrlHost, branchCode);
                    if (useItemPrice) { objTrnItemPrice.GetItemPrice(apiUrlHost, branchCode); }

                    if (ManualSalesIntegration == false)
                    {
                        Controllers.ISPOSTrnCollectionController objTrnCollection = new Controllers.ISPOSTrnCollectionController(this);
                        Controllers.ISPOSTrnSalesReturnController objTrnSalesReturn = new Controllers.ISPOSTrnSalesReturnController(this);

                        objTrnCollection.GetCollection(apiUrlHost, branchCode, userCode);
                        objTrnSalesReturn.GetSalesReturn(apiUrlHost, branchCode, userCode);
                    }
                }
            }
        }

        public void stopIntegration()
        {
            isIntegrationStarted = false;

            btnStartIntegration.Enabled = true;
            btnStopIntegration.Enabled = false;

            dtpIntegrationDate.Enabled = true;

            if (!isFolderMonitoringIntegrationStarted)
            {
                btnSettings.Enabled = true;
            }

            btnClearLogs.Enabled = true;
            btnSaveLogs.Enabled = true;

            btnLogout.Enabled = true;

            isIntegrating = true;
            integrationTimer.Enabled = false;
        }

        private void btnStopIntegration_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to stop integration?", "Stop Integration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                stopIntegration();
                logMessages("Stopped! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n");
            }
        }

        public void logMessages(String message)
        {
            String displayMessage = message;

            if (!message.Equals("Stopped! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n"))
            {
                if (message.Equals("Customer Integration Done.")) { isIntegratingCustomer = false; displayMessage = ""; }
                if (message.Equals("Item Integration Done.")) { isIntegratingItem = false; displayMessage = ""; }
                if (message.Equals("Supplier Integration Done.")) { isIntegratingSupplier = false; displayMessage = ""; }
                if (message.Equals("Collection Integration Done.")) { isIntegratingCollection = false; displayMessage = ""; }
                if (message.Equals("ItemPrice Integration Done.")) { isIntegratingItemPrice = false; displayMessage = ""; }
                if (message.Equals("Receiving Receipt Integration Done.")) { isIntegratingReceivingReceipt = false; displayMessage = ""; }
                if (message.Equals("Sales Return Integration Done.")) { isIntegratingSalesReturn = false; displayMessage = ""; }
                if (message.Equals("StockIn Integration Done.")) { isIntegratingStockIn = false; displayMessage = ""; }
                if (message.Equals("StockOut Integration Done.")) { isIntegratingStockOut = false; displayMessage = ""; }
                if (message.Equals("Stock Transfer In Integration Done.")) { isIntegratingTransferIn = false; displayMessage = ""; }
                if (message.Equals("Stock Transfer Out Integration Done.")) { isIntegratingTransferOut = false; displayMessage = ""; }

                if (isIntegratingCustomer == false &&
                    isIntegratingItem == false &&
                    isIntegratingSupplier == false &&
                    isIntegratingCollection == false &&
                    isIntegratingItemPrice == false &&
                    isIntegratingReceivingReceipt == false &&
                    isIntegratingSalesReturn == false &&
                    isIntegratingStockIn == false &&
                    isIntegratingStockOut == false &&
                    isIntegratingTransferIn == false &&
                    isIntegratingTransferOut == false)
                {
                    isIntegrating = false;
                    integrationTimer.Enabled = true;

                    logMessageCount++;

                    if (logMessageCount >= 50)
                    {
                        logMessageCount = 0;

                        if (!txtLogs.Text.Equals(""))
                        {
                            String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

                            String json;
                            using (StreamReader trmRead = new StreamReader(settingsPath)) { json = trmRead.ReadToEnd(); }

                            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                            Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(json);

                            File.WriteAllText(sysSettings.LogFileLocation + "\\" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt", txtLogs.Text);

                            txtLogs.Text = "";
                        }
                    }
                }
            }

            txtLogs.Text += displayMessage;

            txtLogs.Focus();
            txtLogs.SelectionStart = txtLogs.Text.Length;
            txtLogs.ScrollToCaret();
        }

        private void btnStartFolderMonitoringIntegration_Click(object sender, EventArgs e)
        {
            if (isFolderMonitoringIntegrationStarted == true)
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to close all application? This will close all running threads.", "Close Thread", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {
                    Environment.Exit(Environment.ExitCode);
                }
            }

            btnSettings.Enabled = false;
            btnStartFolderMonitoringIntegration.BackColor = Color.IndianRed;
            btnStartFolderMonitoringIntegration.Text = "Close";

            //btnStartFolderMonitoringIntegration.Enabled = false;

            logFolderMonitoringMessage("File Integration Started! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");

            isFolderMonitoringIntegrationStarted = true;

            if (bgwFolderMonitoringIntegration.IsBusy != true)
            {
                bgwFolderMonitoringIntegration.RunWorkerAsync();
            }
        }

        private void btnStopFolderMonitoringIntegration_Click(object sender, EventArgs e)
        {
            if (!isIntegrationStarted)
            {
                btnSettings.Enabled = true;
            }

            btnStartFolderMonitoringIntegration.Enabled = true;

            logFolderMonitoringMessage("Stopped! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n");

            isFolderMonitoringIntegrationStarted = false;
        }

        public void logFolderMonitoringMessage(String message)
        {
            if (txtFolderMonitoringLogs.InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    bool log = true;

                    if (message.Equals("SIIntegrationLogOnce") ||
                        message.Equals("ORIntegrationLogOnce") ||
                        message.Equals("RRIntegrationLogOnce") ||
                        message.Equals("CVIntegrationLogOnce") ||
                        message.Equals("JVIntegrationLogOnce") ||
                        message.Equals("INIntegrationLogOnce") ||
                        message.Equals("OTIntegrationLogOnce") ||
                        message.Equals("STIntegrationLogOnce"))
                    {
                        log = false;
                        txtFolderMonitoringLogs.Text = txtFolderMonitoringLogs.Text.Substring(0, txtFolderMonitoringLogs.Text.Trim().LastIndexOf(Environment.NewLine));
                    }

                    if (log)
                    {
                        txtFolderMonitoringLogs.Text += message;

                        txtFolderMonitoringLogs.Focus();
                        txtFolderMonitoringLogs.SelectionStart = txtFolderMonitoringLogs.Text.Length;
                        txtFolderMonitoringLogs.ScrollToCaret();
                    }

                    if (txtFolderMonitoringLogs.Lines.Length >= 1000)
                    {
                        txtFolderMonitoringLogs.Text = "";

                        String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

                        String json;
                        using (StreamReader trmRead = new StreamReader(settingsPath)) { json = trmRead.ReadToEnd(); }

                        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                        Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(json);

                        File.WriteAllText(sysSettings.LogFileLocation + "\\FM_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt", txtFolderMonitoringLogs.Text);
                    }
                });
            }
            else
            {
                txtFolderMonitoringLogs.Text += message;

                txtFolderMonitoringLogs.Focus();
                txtFolderMonitoringLogs.SelectionStart = txtFolderMonitoringLogs.Text.Length;
                txtFolderMonitoringLogs.ScrollToCaret();
            }
        }

        private void btnGetCSVTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = fbdGetCSVTemplate.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    String ORSourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"CSVTemplate\OR.csv");
                    String CVSourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"CSVTemplate\CV.csv");
                    String JVSourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"CSVTemplate\JV.csv");
                    String RRSourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"CSVTemplate\RR.csv");
                    String SISourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"CSVTemplate\SI.csv");
                    String INSourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"CSVTemplate\IN.csv");
                    String OTSourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"CSVTemplate\OT.csv");
                    String STSourcePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"CSVTemplate\ST.csv");

                    DialogResult getTemplateDialogResult = MessageBox.Show("This will overwrite all existing csv template files. Are you sure you want to continue?", "Download CSV Templates", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (getTemplateDialogResult == DialogResult.Yes)
                    {
                        if (Directory.Exists(fbdGetCSVTemplate.SelectedPath + "\\CSVTemplate_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "\\")) { Directory.Delete(fbdGetCSVTemplate.SelectedPath + "\\CSVTemplate_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "\\", true); }

                        String executingUser = WindowsIdentity.GetCurrent().Name;

                        DirectorySecurity securityRules = new DirectorySecurity();
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.Read, AccessControlType.Allow));
                        securityRules.AddAccessRule(new FileSystemAccessRule(executingUser, FileSystemRights.FullControl, AccessControlType.Allow));

                        DirectoryInfo createDirectoryORCSV = Directory.CreateDirectory(fbdGetCSVTemplate.SelectedPath + "\\CSVTemplate_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "\\OR\\", securityRules);
                        DirectoryInfo createDirectoryCVCSV = Directory.CreateDirectory(fbdGetCSVTemplate.SelectedPath + "\\CSVTemplate_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "\\CV\\", securityRules);
                        DirectoryInfo createDirectoryJVCSV = Directory.CreateDirectory(fbdGetCSVTemplate.SelectedPath + "\\CSVTemplate_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "\\JV\\", securityRules);
                        DirectoryInfo createDirectoryRRCSV = Directory.CreateDirectory(fbdGetCSVTemplate.SelectedPath + "\\CSVTemplate_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "\\RR\\", securityRules);
                        DirectoryInfo createDirectorySICSV = Directory.CreateDirectory(fbdGetCSVTemplate.SelectedPath + "\\CSVTemplate_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "\\SI\\", securityRules);
                        DirectoryInfo createDirectoryINCSV = Directory.CreateDirectory(fbdGetCSVTemplate.SelectedPath + "\\CSVTemplate_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "\\IN\\", securityRules);
                        DirectoryInfo createDirectoryOTCSV = Directory.CreateDirectory(fbdGetCSVTemplate.SelectedPath + "\\CSVTemplate_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "\\OT\\", securityRules);
                        DirectoryInfo createDirectorySTCSV = Directory.CreateDirectory(fbdGetCSVTemplate.SelectedPath + "\\CSVTemplate_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "\\ST\\", securityRules);

                        File.Copy(ORSourcePath, createDirectoryORCSV.FullName + "\\OR.csv", true);
                        File.Copy(CVSourcePath, createDirectoryCVCSV.FullName + "\\CV.csv", true);
                        File.Copy(JVSourcePath, createDirectoryJVCSV.FullName + "\\JV.csv", true);
                        File.Copy(RRSourcePath, createDirectoryRRCSV.FullName + "\\RR.csv", true);
                        File.Copy(SISourcePath, createDirectorySICSV.FullName + "\\SI.csv", true);
                        File.Copy(INSourcePath, createDirectoryINCSV.FullName + "\\IN.csv", true);
                        File.Copy(OTSourcePath, createDirectoryOTCSV.FullName + "\\OT.csv", true);
                        File.Copy(STSourcePath, createDirectorySTCSV.FullName + "\\ST.csv", true);

                        MessageBox.Show("CSV Templates are successfully saved!", "Save CSV Templates", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void fileSystemWatcherCSVFiles_Created(object sender, FileSystemEventArgs e)
        {
            //String[] documentPrefix = e.FullPath.Split('\\');
            //monitorControllers(documentPrefix[documentPrefix.Length - 2]);
        }

        public void monitorControllers(String documentPrefix)
        {
            //if (isFolderMonitoringIntegrationStarted)
            //{
            //    switch (documentPrefix)
            //    {
            //        case "SI":
            //            if (!bgwFolderMonitoringIntegration.IsBusy) { bgwFolderMonitoringIntegration.RunWorkerAsync(); }
            //            break;
            //        case "OR":
            //            if (!bgwCollection.IsBusy) { bgwCollection.RunWorkerAsync(); }
            //            break;
            //        case "RR":
            //            if (!bgwReceivingReceipt.IsBusy) { bgwReceivingReceipt.RunWorkerAsync(); }
            //            break;
            //        case "CV":
            //            if (!bgwDisbursement.IsBusy) { bgwDisbursement.RunWorkerAsync(); }
            //            break;
            //        case "JV":
            //            if (!bgwJournalVoucher.IsBusy) { bgwJournalVoucher.RunWorkerAsync(); }
            //            break;
            //        case "IN":
            //            if (!bgwStockIn.IsBusy) { bgwStockIn.RunWorkerAsync(); }
            //            break;
            //        case "OT":
            //            if (!bgwStockOut.IsBusy) { bgwStockOut.RunWorkerAsync(); }
            //            break;
            //        case "ST":
            //            if (!bgwStockTransfer.IsBusy) { bgwStockTransfer.RunWorkerAsync(); }
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        private void bgwFolderMonitoringIntegration_DoWork(object sender, DoWorkEventArgs e)
        {
            List<String> ext = new List<String> { ".csv" };

            while (isFolderMonitoringIntegrationStarted)
            {
                Task SITask = Task.Run(() =>
                {
                    runFolderMonitoringIntegrationSI(ext);
                });
                SITask.Wait();

                if (SITask.IsCompleted)
                {
                    Task ORTask = Task.Run(() =>
                    {
                        runFolderMonitoringIntegrationOR(ext);
                    });
                    ORTask.Wait();

                    if (ORTask.IsCompleted)
                    {
                        Task RRTask = Task.Run(() =>
                        {
                            runFolderMonitoringIntegrationRR(ext);
                        });
                        RRTask.Wait();

                        if (RRTask.IsCompleted)
                        {
                            Task CVTask = Task.Run(() =>
                            {
                                runFolderMonitoringIntegrationCV(ext);
                            });
                            CVTask.Wait();

                            if (CVTask.IsCompleted)
                            {
                                Task JVTask = Task.Run(() =>
                                {
                                    runFolderMonitoringIntegrationJV(ext);
                                });
                                JVTask.Wait();

                                if (JVTask.IsCompleted)
                                {
                                    Task INTask = Task.Run(() =>
                                    {
                                        runFolderMonitoringIntegrationIN(ext);
                                    });
                                    INTask.Wait();

                                    if (INTask.IsCompleted)
                                    {
                                        Task OTTask = Task.Run(() =>
                                        {
                                            runFolderMonitoringIntegrationOT(ext);
                                        });
                                        OTTask.Wait();

                                        if (OTTask.IsCompleted)
                                        {
                                            Task STTask = Task.Run(() =>
                                            {
                                                runFolderMonitoringIntegrationST(ext);
                                            });
                                            STTask.Wait();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                System.Threading.Thread.Sleep(5000);
            }
        }

        public void runFolderMonitoringIntegrationSI(List<String> ext)
        {
            FolderMonitoringTrnSalesInvoiceController folderMonitoringSI = new FolderMonitoringTrnSalesInvoiceController();

            List<String> SIFiles = new List<String>(Directory.EnumerateFiles(folderToMonitor + "\\SI\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (SIFiles.Any())
            {
                foreach (var SIFile in SIFiles)
                {
                    folderMonitoringSI.SendSalesInvoice(this, txtFolderMonitoringUserCode.Text, SIFile, domain);
                }
            }
        }

        public void runFolderMonitoringIntegrationOR(List<String> ext)
        {
            FolderMonitoringTrnCollectionController folderMonitoringOR = new FolderMonitoringTrnCollectionController();

            List<String> ORFiles = new List<String>(Directory.EnumerateFiles(folderToMonitor + "\\OR\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (ORFiles.Any())
            {
                foreach (var ORFile in ORFiles)
                {
                    folderMonitoringOR.SendCollection(this, txtFolderMonitoringUserCode.Text, ORFile, domain);
                }
            }
        }

        public void runFolderMonitoringIntegrationRR(List<String> ext)
        {
            FolderMonitoringTrnReceivingReceiptController folderMonitoringRR = new FolderMonitoringTrnReceivingReceiptController();

            List<String> RRFiles = new List<String>(Directory.EnumerateFiles(folderToMonitor + "\\RR\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (RRFiles.Any())
            {
                foreach (var RRFile in RRFiles)
                {
                    folderMonitoringRR.SendReceivingReceipt(this, txtFolderMonitoringUserCode.Text, RRFile, domain);
                }
            }
        }

        public void runFolderMonitoringIntegrationCV(List<String> ext)
        {
            FolderMonitoringTrnDisbursementController folderMonitoringCV = new FolderMonitoringTrnDisbursementController();

            List<String> CVFiles = new List<String>(Directory.EnumerateFiles(folderToMonitor + "\\CV\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (CVFiles.Any())
            {
                foreach (var CVFile in CVFiles)
                {
                    folderMonitoringCV.SendDisbursement(this, txtFolderMonitoringUserCode.Text, CVFile, domain);
                }
            }
        }

        public void runFolderMonitoringIntegrationJV(List<String> ext)
        {
            FolderMonitoringTrnJournalVoucherController folderMonitoringJV = new FolderMonitoringTrnJournalVoucherController();

            List<String> JVFiles = new List<String>(Directory.EnumerateFiles(folderToMonitor + "\\JV\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (JVFiles.Any())
            {
                foreach (var JVFile in JVFiles)
                {
                    folderMonitoringJV.SendJournalVoucher(this, txtFolderMonitoringUserCode.Text, JVFile, domain);
                }
            }
        }

        public void runFolderMonitoringIntegrationIN(List<String> ext)
        {
            FolderMonitoringTrnStockInController folderMonitoringIN = new FolderMonitoringTrnStockInController();

            List<String> INFiles = new List<String>(Directory.EnumerateFiles(folderToMonitor + "\\IN\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (INFiles.Any())
            {
                foreach (var INFile in INFiles)
                {
                    folderMonitoringIN.SendStockIn(this, txtFolderMonitoringUserCode.Text, INFile, domain);
                }
            }
        }

        public void runFolderMonitoringIntegrationOT(List<String> ext)
        {
            FolderMonitoringTrnStockOutController folderMonitoringOT = new FolderMonitoringTrnStockOutController();

            List<String> OTFiles = new List<String>(Directory.EnumerateFiles(folderToMonitor + "\\OT\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (OTFiles.Any())
            {
                foreach (var OTFile in OTFiles)
                {
                    folderMonitoringOT.SendStockOut(this, txtFolderMonitoringUserCode.Text, OTFile, domain);
                }
            }
        }

        public void runFolderMonitoringIntegrationST(List<String> ext)
        {
            FolderMonitoringTrnStockTransferController folderMonitoringST = new FolderMonitoringTrnStockTransferController();

            List<String> STFiles = new List<String>(Directory.EnumerateFiles(folderToMonitor + "\\ST\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (STFiles.Any())
            {
                foreach (var STFile in STFiles)
                {
                    folderMonitoringST.SendStockTransfer(this, txtFolderMonitoringUserCode.Text, STFile, domain);
                }
            }
        }

        public Boolean isManualSalesIntegrationStartIntegrating = false;

        public void logManualSalesIntegrationMessage(String message)
        {
            if (textBoxPOSManualSalesIntegrationLogs.InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    bool log = true;

                    if (message.Equals("ManualSIIntegrationLogOnce"))
                    {
                        log = false;
                        textBoxPOSManualSalesIntegrationLogs.Text = textBoxPOSManualSalesIntegrationLogs.Text.Substring(0, textBoxPOSManualSalesIntegrationLogs.Text.Trim().LastIndexOf(Environment.NewLine));
                    }

                    if (log)
                    {
                        textBoxPOSManualSalesIntegrationLogs.Text += message;

                        textBoxPOSManualSalesIntegrationLogs.Focus();
                        textBoxPOSManualSalesIntegrationLogs.SelectionStart = textBoxPOSManualSalesIntegrationLogs.Text.Length;
                        textBoxPOSManualSalesIntegrationLogs.ScrollToCaret();
                    }

                    if (textBoxPOSManualSalesIntegrationLogs.Lines.Length >= 1000)
                    {
                        textBoxPOSManualSalesIntegrationLogs.Text = "";

                        String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

                        String json;
                        using (StreamReader trmRead = new StreamReader(settingsPath)) { json = trmRead.ReadToEnd(); }

                        JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                        Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(json);

                        File.WriteAllText(sysSettings.LogFileLocation + "\\ManualSIIntegration_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt", textBoxPOSManualSalesIntegrationLogs.Text);
                    }
                });
            }
            else
            {
                textBoxPOSManualSalesIntegrationLogs.Text += message;

                textBoxPOSManualSalesIntegrationLogs.Focus();
                textBoxPOSManualSalesIntegrationLogs.SelectionStart = textBoxPOSManualSalesIntegrationLogs.Text.Length;
                textBoxPOSManualSalesIntegrationLogs.ScrollToCaret();
            }
        }

        private void buttonPOSManualSalesIntegrationStart_Click(object sender, EventArgs e)
        {
            dateTimePickerPOSManualSalesIntegrationDate.Enabled = false;
            comboBoxTerminal.Enabled = false;

            buttonPOSManualSalesIntegrationStart.Text = "Integrating...";
            buttonPOSManualSalesIntegrationStart.Enabled = false;

            isManualSalesIntegrationStartIntegrating = true;

            logManualSalesIntegrationMessage("Manual Sales Integration Started! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n");

            if (backgroundWorkerManualSalesIntegration.IsBusy != true)
            {
                backgroundWorkerManualSalesIntegration.RunWorkerAsync();
            }
        }

        private void backgroundWorkerManualSalesIntegration_DoWork(object sender, DoWorkEventArgs e)
        {
            while (isManualSalesIntegrationStartIntegrating)
            {
                Task ManualSIIntegrationTask = Task.Run(() =>
                {
                    ISPOSManualSalesIntegrationTrnCollectionController manualSalesIntegrationTrnCollectionController = new ISPOSManualSalesIntegrationTrnCollectionController();
                    manualSalesIntegrationTrnCollectionController.SendSalesInvoice(this, domain, dateTimePickerPOSManualSalesIntegrationDate.Value.ToShortDateString(), Convert.ToInt32(comboBoxTerminal.SelectedValue));
                });
                ManualSIIntegrationTask.Wait();

                if (ManualSIIntegrationTask.IsCompleted)
                {
                    Task ManualSIIntegrationTaskIsCompleted = Task.Run(() =>
                    {
                        AsyncManualSIIntegrationTaskIsCompleted();
                    });
                    ManualSIIntegrationTaskIsCompleted.Wait();
                }

                System.Threading.Thread.Sleep(5000);
            }
        }

        public async void AsyncManualSIIntegrationTaskIsCompleted()
        {
            await TaskManualSIIntegrationTaskIsCompleted();
        }

        public Task<Boolean> TaskManualSIIntegrationTaskIsCompleted()
        {
            if (buttonPOSManualSalesIntegrationStart.InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    dateTimePickerPOSManualSalesIntegrationDate.Enabled = true;
                    comboBoxTerminal.Enabled = true;

                    buttonPOSManualSalesIntegrationStart.Enabled = true;
                    buttonPOSManualSalesIntegrationStart.Text = "Integrate";

                    isManualSalesIntegrationStartIntegrating = false;
                });
            }

            return Task.FromResult(true);
        }

        public void GetTerminalList()
        {
            var terminals = from d in posdb.MstTerminals
                            select d;

            if (terminals.Any())
            {
                comboBoxTerminal.DataSource = terminals.ToList();
                comboBoxTerminal.ValueMember = "Id";
                comboBoxTerminal.DisplayMember = "Terminal";
            }
        }
    }
}