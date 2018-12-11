using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace EasyfisIntegrator.Forms
{
    public partial class TrnInnosoftPOSIntegrationForm : Form
    {
        private InnosoftPOSData.InnosoftPOSDataDataContext posdb = new InnosoftPOSData.InnosoftPOSDataDataContext(Controllers.SysGlobalSettings.getConnectionString());

        public SysLoginForm sysLoginForm;
        public Boolean isIntegrationStarted = false;
        public Boolean isSettingsClicked = false;
        private Timer integrationTimer = new Timer();

        public static Boolean isIntegrating = false;
        public static Boolean isIntegratingCustomer = false;
        public static Boolean isIntegratingItem = false;
        public static Boolean isIntegratingSupplier = false;
        public static Boolean isIntegratingCollection = false;
        public static Boolean isIntegratingItemPrice = false;
        public static Boolean isIntegratingReceivingReceipt = false;
        public static Boolean isIntegratingSalesReturn = false;
        public static Boolean isIntegratingStockIn = false;
        public static Boolean isIntegratingStockOut = false;
        public static Boolean isIntegratingTransferIn = false;
        public static Boolean isIntegratingTransferOut = false;

        public Int32 logMessageCount = 0;

        public TrnInnosoftPOSIntegrationForm(SysLoginForm form)
        {
            InitializeComponent();
            sysLoginForm = form;

            isSettingsClicked = false;

            getPOSSettings();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (isIntegrationStarted)
            {
                MessageBox.Show("Please stop the integration first.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Hide();

                SysLoginForm sysLoginForm = new SysLoginForm();
                sysLoginForm.Show();
            }
        }

        public void getPOSSettings()
        {
            lblCurrentUser.Text = sysLoginForm.currentUser;

            var settings = from d in posdb.SysSettings select d;
            if (settings.Any())
            {
                posdb.Refresh(RefreshMode.OverwriteCurrentValues, settings);

                txtBranchCode.Text = settings.FirstOrDefault().BranchCode;
                txtUserCode.Text = settings.FirstOrDefault().UserCode;
                cbxUseItemPrice.Checked = settings.FirstOrDefault().UseItemPrice;
            }

            String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

            String json;
            using (StreamReader trmRead = new StreamReader(settingsPath)) { json = trmRead.ReadToEnd(); }

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(json);

            txtDomain.Text = sysSettings.Domain;

            if (!isSettingsClicked)
            {
                if (sysSettings.IsAutoStartIntegration)
                {
                    startIntegration();
                }
                else
                {
                    logMessages("Press start button to integrate. \r\n\n" + "\r\n\n");
                    stopIntegration();
                }
            }
        }

        private void TrnInnosoftPOSIntegrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to close this application?", "Close Integrator", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                if (isIntegrationStarted)
                {
                    MessageBox.Show("Please stop the integration first.", "Close Integrator", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    e.Cancel = true;
                    Activate();
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                e.Cancel = true;
                Activate();
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

                File.WriteAllText(sysSettings.LogFileLocation + "\\" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt", txtLogs.Text);

                MessageBox.Show("Save Log Successful!", "Save Logs", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (DirectoryNotFoundException drex)
            {
                MessageBox.Show(drex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenFolderMonitoring_Click(object sender, EventArgs e)
        {
            TrnFolderMonitoringIntegrationForm trnFilesIntegrationForm = new TrnFolderMonitoringIntegrationForm();
            trnFilesIntegrationForm.Show();
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

            integrationTimer = new Timer();
            integrationTimer.Interval = 5000;
            integrationTimer.Tick += new EventHandler(integrationTimerTick);
            integrationTimer.Enabled = true;
        }

        public void integrationTimerTick(object sender, EventArgs e)
        {
            integrationTimer.Enabled = false;
            btnStopIntegration.Enabled = true;

            integrate();
        }

        public void integrate()
        {
            if (isIntegrating == false)
            {
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
                    Controllers.ISPOSTrnCollectionController objTrnCollection = new Controllers.ISPOSTrnCollectionController(this);
                    Controllers.ISPOSTrnSalesReturnController objTrnSalesReturn = new Controllers.ISPOSTrnSalesReturnController(this);
                    Controllers.ISPOSTrnItemPriceController objTrnItemPrice = new Controllers.ISPOSTrnItemPriceController(this, dtpIntegrationDate.Text);

                    objMstCustomer.GetCustomer(apiUrlHost);
                    objMstSupplier.GetSupplier(apiUrlHost);
                    objMstItem.GetItem(apiUrlHost);
                    objTrnReceivingReceipt.GetReceivingReceipt(apiUrlHost, branchCode);
                    objTrnStockIn.GetStockIn(apiUrlHost, branchCode);
                    objTrnStockOut.GetStockOut(apiUrlHost, branchCode);
                    objTrnStockTransferIn.GetStockTransferIN(apiUrlHost, branchCode);
                    objTrnStockTransferOut.GetStockTransferOT(apiUrlHost, branchCode);
                    objTrnCollection.GetCollection(apiUrlHost, branchCode, userCode);
                    objTrnSalesReturn.GetSalesReturn(apiUrlHost, branchCode, userCode);
                    if (useItemPrice) { objTrnItemPrice.GetItemPrice(apiUrlHost, branchCode); }
                }
            }
        }

        public void stopIntegration()
        {
            isIntegrationStarted = false;

            btnStartIntegration.Enabled = true;
            btnStopIntegration.Enabled = false;

            dtpIntegrationDate.Enabled = true;
            btnSettings.Enabled = true;

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
    }
}