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
        public Boolean isSalesIntegrationStarted = false;
        public String salesIntegrationLogFileLocation = "";
        public Int32 salesIntegrationLogMessageCount = 0;
        public Boolean isManualSalesIntegrationStarted = false;
        public Boolean isFolderMonitoringIntegrationStarted = false;
        public Boolean isFolderMonitoringOnly = false;
        public Boolean isMasterFileInventoryUpdating = false;
        public Boolean isManualMasterFileInventoryUpdating = false;
        public Boolean isManualSalesIntegration = false;

        public TrnIntegrationForm()
        {
            InitializeComponent();
            getSettings();
        }

        public void getLoginDetails(SysLoginForm form)
        {
            sysLoginForm = form;
            //lblCurrentUser.Text = sysLoginForm.currentUser;
        }

        public void getSettings()
        {
            String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

            String json;
            using (StreamReader trmRead = new StreamReader(settingsPath)) { json = trmRead.ReadToEnd(); }

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(json);

            if (sysSettings.IsFolderMonitoringOnly)
            {
                isFolderMonitoringOnly = true;

                tabPagePOSSalesIntegration.Enabled = false;
                tabPagePOSManualSalesIntegration.Enabled = false;
                tabPageFolderMonitoringIntegration.Enabled = true;

                tabIntegration.SelectedTab = tabPageFolderMonitoringIntegration;
                tabIntegration.TabPages.Remove(tabPagePOSSalesIntegration);
                tabIntegration.TabPages.Remove(tabPagePOSManualSalesIntegration);

                btnLogout.Visible = false;
            }
            else
            {
                if (sysSettings.ManualSalesIntegration == true)
                {
                    isManualSalesIntegration = true;

                    tabPagePOSSalesIntegration.Enabled = false;
                    tabPagePOSManualSalesIntegration.Enabled = true;
                    tabPageFolderMonitoringIntegration.Enabled = false;

                    tabIntegration.SelectedTab = tabPagePOSManualSalesIntegration;
                    tabIntegration.TabPages.Remove(tabPagePOSSalesIntegration);
                    tabIntegration.TabPages.Remove(tabPageFolderMonitoringIntegration);

                    btnLogout.Visible = false;
                    buttonManualSalesIntegrationStart.Enabled = true;
                    buttonManualSalesIntegrationStop.Enabled = false;
                    buttonUpdateManualMasterFileInventory.Enabled = false;

                    salesIntegrationLogFileLocation = sysSettings.LogFileLocation;
                    textBoxSalesIntegrationDomain.Text = sysSettings.Domain;
                    comboBoxManualSalesIntegrationTerminal.Enabled = true;

                    var settings = from d in posdb.SysSettings
                                   select d;

                    if (settings.Any())
                    {
                        posdb.Refresh(RefreshMode.OverwriteCurrentValues, settings);

                        textBoxSalesIntegrationBranchCode.Text = settings.FirstOrDefault().BranchCode;
                        textBoxSalesIntegrationUserCode.Text = settings.FirstOrDefault().UserCode;
                        checkBoxSalesIntegrationUseItemPrice.Checked = settings.FirstOrDefault().UseItemPrice;
                    }

                    GetTerminalList();
                    textBoxManualSalesIntegrationDomain.Text = sysSettings.Domain;
                }
                else
                {
                    salesIntegrationLogFileLocation = sysSettings.LogFileLocation;
                    textBoxSalesIntegrationDomain.Text = sysSettings.Domain;

                    var settings = from d in posdb.SysSettings
                                   select d;

                    if (settings.Any())
                    {
                        posdb.Refresh(RefreshMode.OverwriteCurrentValues, settings);

                        textBoxSalesIntegrationBranchCode.Text = settings.FirstOrDefault().BranchCode;
                        textBoxSalesIntegrationUserCode.Text = settings.FirstOrDefault().UserCode;
                        checkBoxSalesIntegrationUseItemPrice.Checked = settings.FirstOrDefault().UseItemPrice;
                    }

                    GetTerminalList();
                    textBoxManualSalesIntegrationDomain.Text = sysSettings.Domain;
                }
            }

            textBoxFolderMonitoringUserCode.Text = sysSettings.FolderMonitoringUserCode;
            textBoxFolderMonitoringDomain.Text = sysSettings.Domain;
            textBoxFolderToMonitor.Text = sysSettings.FolderToMonitor;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (isSalesIntegrationStarted)
            {
                MessageBox.Show("Please stop the sales integration first.", "EasyFIS Integration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (isManualSalesIntegrationStarted)
            {
                MessageBox.Show("Please stop the manual sales integration first.", "EasyFIS Integration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (isFolderMonitoringIntegrationStarted)
            {
                MessageBox.Show("Please stop the folder monitoring integration first.", "EasyFIS Integration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Hide();

                SysLoginForm sysLoginForm = new SysLoginForm();
                sysLoginForm.Show();
            }
        }

        private void TrnInnosoftPOSIntegrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Activate();

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to close this application?", "EasyFIS Integration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                if (isSalesIntegrationStarted)
                {
                    MessageBox.Show("Please stop the sales integration first.", "EasyFIS Integration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (isManualSalesIntegrationStarted)
                {
                    MessageBox.Show("Please stop the manual sales integration first.", "EasyFIS Integration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (isFolderMonitoringIntegrationStarted)
                {
                    MessageBox.Show("Please stop the folder monitoring integration first.", "EasyFIS Integration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Hide();

                    MainForm mainForm = new MainForm();
                    mainForm.Show();
                }
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

                MessageBox.Show("Save Log Successful!", "EasyFIS Integration", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (DirectoryNotFoundException drex)
            {
                MessageBox.Show(drex.Message, "EasyFIS Integration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to clear all logs? You can't undo changes anymore.", "EasyFIS Integration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                txtLogs.Text = "Press start button to integrate. \r\n\n" + "\r\n\n";
                txtFolderMonitoringLogs.Text = "Press start button to integrate. \r\n\n" + "\r\n\n";
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            SysSettings sysSettings = new SysSettings(this);
            sysSettings.ShowDialog();
        }

        public void salesIntegrationLogMessages(String message)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                String displayMessage = message;

                salesIntegrationLogMessageCount++;
                if (salesIntegrationLogMessageCount >= 1000)
                {
                    salesIntegrationLogMessageCount = 0;

                    if (!txtLogs.Text.Equals(""))
                    {
                        File.WriteAllText(salesIntegrationLogFileLocation + "\\" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt", txtLogs.Text);
                        txtLogs.Text = "";
                    }
                }

                txtLogs.Text += displayMessage;

                txtLogs.Focus();
                txtLogs.SelectionStart = txtLogs.Text.Length;
                txtLogs.ScrollToCaret();
            });
        }

        private void btnStartIntegration_Click(object sender, EventArgs e)
        {
            isSalesIntegrationStarted = true;

            buttonUpdateMasterFileInventory.Enabled = true;
            buttonSalesIntegrationStart.Enabled = false;
            buttonSalesIntegrationStop.Enabled = true;

            dateTimePickerSalesIntegrationDate.Enabled = false;

            btnSaveLogs.Enabled = false;
            btnClearLogs.Enabled = false;
            btnSettings.Enabled = false;

            btnLogout.Enabled = false;

            salesIntegrationLogMessages("Sales integrtion started! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n");

            tabPagePOSSalesIntegration.Enabled = true;
            tabPagePOSManualSalesIntegration.Enabled = false;
            tabPageFolderMonitoringIntegration.Enabled = false;

            if (backgroundWorkerSalesIntegration.IsBusy != true)
            {
                backgroundWorkerSalesIntegration.RunWorkerAsync();
            }
        }

        private void backgroundWorkerSalesIntegration_DoWork(object sender, DoWorkEventArgs e)
        {
            while (isSalesIntegrationStarted)
            {
                String apiUrlHost = textBoxSalesIntegrationDomain.Text;

                var sysSettings = from d in posdb.SysSettings select d;
                if (sysSettings.Any())
                {
                    var branchCode = sysSettings.FirstOrDefault().BranchCode;
                    var userCode = sysSettings.FirstOrDefault().UserCode;
                    var useItemPrice = sysSettings.FirstOrDefault().UseItemPrice;

                    if (isMasterFileInventoryUpdating == true)
                    {
                        Task taskCustomer = Task.Run(() =>
                        {
                            Controllers.ISPOSMstCustomerController objMstCustomer = new Controllers.ISPOSMstCustomerController(this, dateTimePickerSalesIntegrationDate.Text);
                            objMstCustomer.SyncCustomer(apiUrlHost);
                        });
                        taskCustomer.Wait();

                        if (taskCustomer.IsCompleted)
                        {
                            Task taskSupplier = Task.Run(() =>
                            {
                                Controllers.ISPOSMstSupplierController objMstSupplier = new Controllers.ISPOSMstSupplierController(this, dateTimePickerSalesIntegrationDate.Text);
                                objMstSupplier.SyncSupplier(apiUrlHost);
                            });
                            taskSupplier.Wait();

                            if (taskSupplier.IsCompleted)
                            {
                                Task taskItem = Task.Run(() =>
                                {
                                    Controllers.ISPOSMstItemController objMstItem = new Controllers.ISPOSMstItemController(this, dateTimePickerSalesIntegrationDate.Text);
                                    objMstItem.SyncItem(apiUrlHost);
                                });
                                taskItem.Wait();

                                if (taskItem.IsCompleted)
                                {
                                    Boolean isTaskItemPriceCompleted = false;

                                    if (useItemPrice)
                                    {
                                        Task taskItemPrice = Task.Run(() =>
                                        {
                                            Controllers.ISPOSTrnItemPriceController objTrnItemPrice = new Controllers.ISPOSTrnItemPriceController(this, dateTimePickerSalesIntegrationDate.Text);
                                            objTrnItemPrice.SyncItemPrice(apiUrlHost, branchCode);

                                        });
                                        taskItemPrice.Wait();

                                        if (taskItemPrice.IsCompleted)
                                        {
                                            isTaskItemPriceCompleted = true;
                                        }
                                    }
                                    else
                                    {
                                        isTaskItemPriceCompleted = true;
                                    }

                                    if (isTaskItemPriceCompleted == true)
                                    {
                                        Task taskReceivingReceipt = Task.Run(() =>
                                        {
                                            Controllers.ISPOSTrnReceivingReceiptController objTrnReceivingReceipt = new Controllers.ISPOSTrnReceivingReceiptController(this, dateTimePickerSalesIntegrationDate.Text);
                                            objTrnReceivingReceipt.SyncReceivingReceipt(apiUrlHost, branchCode);
                                        });
                                        taskReceivingReceipt.Wait();

                                        if (taskReceivingReceipt.IsCompleted)
                                        {
                                            Task taskStockIn = Task.Run(() =>
                                            {
                                                Controllers.ISPOSTrnStockInController objTrnStockIn = new Controllers.ISPOSTrnStockInController(this, dateTimePickerSalesIntegrationDate.Text);
                                                objTrnStockIn.SyncStockIn(apiUrlHost, branchCode);
                                            });
                                            taskStockIn.Wait();

                                            if (taskStockIn.IsCompleted)
                                            {
                                                Task taskStockOut = Task.Run(() =>
                                                {
                                                    Controllers.ISPOSTrnStockOutController objTrnStockOut = new Controllers.ISPOSTrnStockOutController(this, dateTimePickerSalesIntegrationDate.Text);
                                                    objTrnStockOut.SyncStockOut(apiUrlHost, branchCode);

                                                });
                                                taskStockOut.Wait();

                                                if (taskStockOut.IsCompleted)
                                                {
                                                    Task taskStockTransferIn = Task.Run(() =>
                                                    {
                                                        Controllers.ISPOSTrnStockTransferInController objTrnStockTransferIn = new Controllers.ISPOSTrnStockTransferInController(this, dateTimePickerSalesIntegrationDate.Text);
                                                        objTrnStockTransferIn.SyncStockTransferIN(apiUrlHost, branchCode);

                                                    });
                                                    taskStockTransferIn.Wait();

                                                    if (taskStockTransferIn.IsCompleted)
                                                    {
                                                        Task taskStockTransferOut = Task.Run(() =>
                                                        {
                                                            Controllers.ISPOSTrnStockTransferOutController objTrnStockTransferOut = new Controllers.ISPOSTrnStockTransferOutController(this, dateTimePickerSalesIntegrationDate.Text);
                                                            objTrnStockTransferOut.SyncStockTransferOT(apiUrlHost, branchCode);
                                                        });
                                                        taskStockTransferOut.Wait();

                                                        if (taskStockTransferOut.IsCompleted)
                                                        {
                                                            isMasterFileInventoryUpdating = false;
                                                            salesIntegrationLogMessages("Sync Completed! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Task taskCollection = Task.Run(() =>
                        {
                            Controllers.ISPOSTrnCollectionController objTrnCollection = new Controllers.ISPOSTrnCollectionController(this);
                            objTrnCollection.SyncCollection(apiUrlHost, branchCode, userCode);
                        });
                        taskCollection.Wait();

                        if (taskCollection.IsCompleted)
                        {
                            Task taskSalesReturn = Task.Run(() =>
                            {
                                Controllers.ISPOSTrnSalesReturnController objTrnSalesReturn = new Controllers.ISPOSTrnSalesReturnController(this);
                                objTrnSalesReturn.SyncSalesReturn(apiUrlHost, branchCode, userCode);
                            });
                            taskSalesReturn.Wait();

                            if (taskSalesReturn.IsCompleted)
                            {

                            }
                        }
                    }

                    if (buttonUpdateMasterFileInventory.InvokeRequired && buttonSalesIntegrationStop.InvokeRequired)
                    {
                        BeginInvoke((MethodInvoker)delegate
                        {
                            if (isMasterFileInventoryUpdating == false)
                            {
                                buttonUpdateMasterFileInventory.Enabled = true;
                                buttonSalesIntegrationStop.Enabled = true;
                            }
                        });
                    }
                }

                System.Threading.Thread.Sleep(5000);
            }
        }

        private void btnStopIntegration_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to stop sales integration?", "EasyFIS Integration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                isSalesIntegrationStarted = false;

                buttonUpdateMasterFileInventory.Enabled = false;
                buttonSalesIntegrationStart.Enabled = true;
                buttonSalesIntegrationStop.Enabled = false;

                dateTimePickerSalesIntegrationDate.Enabled = true;

                btnSaveLogs.Enabled = true;
                btnClearLogs.Enabled = true;
                btnSettings.Enabled = true;

                btnLogout.Enabled = true;

                salesIntegrationLogMessages("Sales integration stopped! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n");

                tabPagePOSSalesIntegration.Enabled = true;
                tabPagePOSManualSalesIntegration.Enabled = true;
                tabPageFolderMonitoringIntegration.Enabled = true;
            }
        }

        public void GetTerminalList()
        {
            var terminals = from d in posdb.MstTerminals
                            select d;

            if (terminals.Any())
            {
                comboBoxManualSalesIntegrationTerminal.DataSource = terminals.ToList();
                comboBoxManualSalesIntegrationTerminal.ValueMember = "Id";
                comboBoxManualSalesIntegrationTerminal.DisplayMember = "Terminal";
            }
        }

        public void manualSalesIntegrationLogMessages(String message)
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
            isManualSalesIntegrationStarted = true;

            dateTimePickerManualSalesIntegrationDate.Enabled = false;
            comboBoxManualSalesIntegrationTerminal.Enabled = false;

            buttonManualSalesIntegrationStart.Enabled = false;
            buttonManualSalesIntegrationStop.Enabled = true;
            buttonUpdateManualMasterFileInventory.Enabled = true;

            btnSaveLogs.Enabled = false;
            btnClearLogs.Enabled = false;
            btnSettings.Enabled = false;

            btnLogout.Enabled = false;

            manualSalesIntegrationLogMessages("Manual sales integration started! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n");

            tabPagePOSSalesIntegration.Enabled = false;
            tabPagePOSManualSalesIntegration.Enabled = true;
            tabPageFolderMonitoringIntegration.Enabled = false;

            if (backgroundWorkerManualSalesIntegration.IsBusy != true)
            {
                backgroundWorkerManualSalesIntegration.RunWorkerAsync();
            }
        }

        private void backgroundWorkerManualSalesIntegration_DoWork(object sender, DoWorkEventArgs e)
        {
            while (isManualSalesIntegrationStarted)
            {
                String apiUrlHost = textBoxSalesIntegrationDomain.Text;

                var sysSettings = from d in posdb.SysSettings select d;
                if (sysSettings.Any())
                {
                    if (isManualMasterFileInventoryUpdating == true)
                    {
                        var branchCode = sysSettings.FirstOrDefault().BranchCode;
                        var userCode = sysSettings.FirstOrDefault().UserCode;
                        var useItemPrice = sysSettings.FirstOrDefault().UseItemPrice;

                        Task taskCustomer = Task.Run(() =>
                        {
                            Controllers.ISPOSMstCustomerController objMstCustomer = new Controllers.ISPOSMstCustomerController(this, dateTimePickerSalesIntegrationDate.Text);
                            objMstCustomer.SyncCustomer(apiUrlHost);
                        });
                        taskCustomer.Wait();

                        if (taskCustomer.IsCompleted)
                        {
                            Task taskSupplier = Task.Run(() =>
                            {
                                Controllers.ISPOSMstSupplierController objMstSupplier = new Controllers.ISPOSMstSupplierController(this, dateTimePickerSalesIntegrationDate.Text);
                                objMstSupplier.SyncSupplier(apiUrlHost);
                            });
                            taskSupplier.Wait();

                            if (taskSupplier.IsCompleted)
                            {
                                Task taskItem = Task.Run(() =>
                                {
                                    Controllers.ISPOSMstItemController objMstItem = new Controllers.ISPOSMstItemController(this, dateTimePickerSalesIntegrationDate.Text);
                                    objMstItem.SyncItem(apiUrlHost);
                                });
                                taskItem.Wait();

                                if (taskItem.IsCompleted)
                                {
                                    Boolean isTaskItemPriceCompleted = false;

                                    if (useItemPrice)
                                    {
                                        Task taskItemPrice = Task.Run(() =>
                                        {
                                            Controllers.ISPOSTrnItemPriceController objTrnItemPrice = new Controllers.ISPOSTrnItemPriceController(this, dateTimePickerSalesIntegrationDate.Text);
                                            objTrnItemPrice.SyncItemPrice(apiUrlHost, branchCode);

                                        });
                                        taskItemPrice.Wait();

                                        if (taskItemPrice.IsCompleted)
                                        {
                                            isTaskItemPriceCompleted = true;
                                        }
                                    }
                                    else
                                    {
                                        isTaskItemPriceCompleted = true;
                                    }

                                    if (isTaskItemPriceCompleted == true)
                                    {
                                        Task taskReceivingReceipt = Task.Run(() =>
                                        {
                                            Controllers.ISPOSTrnReceivingReceiptController objTrnReceivingReceipt = new Controllers.ISPOSTrnReceivingReceiptController(this, dateTimePickerSalesIntegrationDate.Text);
                                            objTrnReceivingReceipt.SyncReceivingReceipt(apiUrlHost, branchCode);
                                        });
                                        taskReceivingReceipt.Wait();

                                        if (taskReceivingReceipt.IsCompleted)
                                        {
                                            Task taskStockIn = Task.Run(() =>
                                            {
                                                Controllers.ISPOSTrnStockInController objTrnStockIn = new Controllers.ISPOSTrnStockInController(this, dateTimePickerSalesIntegrationDate.Text);
                                                objTrnStockIn.SyncStockIn(apiUrlHost, branchCode);
                                            });
                                            taskStockIn.Wait();

                                            if (taskStockIn.IsCompleted)
                                            {
                                                Task taskStockOut = Task.Run(() =>
                                                {
                                                    Controllers.ISPOSTrnStockOutController objTrnStockOut = new Controllers.ISPOSTrnStockOutController(this, dateTimePickerSalesIntegrationDate.Text);
                                                    objTrnStockOut.SyncStockOut(apiUrlHost, branchCode);

                                                });
                                                taskStockOut.Wait();

                                                if (taskStockOut.IsCompleted)
                                                {
                                                    Task taskStockTransferIn = Task.Run(() =>
                                                    {
                                                        Controllers.ISPOSTrnStockTransferInController objTrnStockTransferIn = new Controllers.ISPOSTrnStockTransferInController(this, dateTimePickerSalesIntegrationDate.Text);
                                                        objTrnStockTransferIn.SyncStockTransferIN(apiUrlHost, branchCode);

                                                    });
                                                    taskStockTransferIn.Wait();

                                                    if (taskStockTransferIn.IsCompleted)
                                                    {
                                                        Task taskStockTransferOut = Task.Run(() =>
                                                        {
                                                            Controllers.ISPOSTrnStockTransferOutController objTrnStockTransferOut = new Controllers.ISPOSTrnStockTransferOutController(this, dateTimePickerSalesIntegrationDate.Text);
                                                            objTrnStockTransferOut.SyncStockTransferOT(apiUrlHost, branchCode);
                                                        });
                                                        taskStockTransferOut.Wait();

                                                        if (taskStockTransferOut.IsCompleted)
                                                        {
                                                            isManualMasterFileInventoryUpdating = false;
                                                            manualSalesIntegrationLogMessages("Sync Completed! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n");
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Task ManualSIIntegrationTask = Task.Run(() =>
                        {
                            ISPOSManualSalesIntegrationTrnCollectionController manualSalesIntegrationTrnCollectionController = new ISPOSManualSalesIntegrationTrnCollectionController();
                            manualSalesIntegrationTrnCollectionController.SendSalesInvoice(this, textBoxManualSalesIntegrationDomain.Text, dateTimePickerManualSalesIntegrationDate.Value.ToShortDateString(), Convert.ToInt32(comboBoxManualSalesIntegrationTerminal.SelectedValue));
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
                    }
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
            if (buttonManualSalesIntegrationStart.InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    isManualSalesIntegrationStarted = false;

                    dateTimePickerManualSalesIntegrationDate.Enabled = true;
                    comboBoxManualSalesIntegrationTerminal.Enabled = true;

                    buttonManualSalesIntegrationStart.Enabled = true;
                    buttonManualSalesIntegrationStop.Enabled = false;

                    btnSaveLogs.Enabled = true;
                    btnClearLogs.Enabled = true;
                    btnSettings.Enabled = true;

                    btnLogout.Enabled = true;

                    tabPagePOSSalesIntegration.Enabled = true;
                    tabPagePOSManualSalesIntegration.Enabled = true;
                    tabPageFolderMonitoringIntegration.Enabled = true;
                });
            }

            return Task.FromResult(true);
        }

        private void buttonManualSalesIntegrationStop_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to stop manual sales integration?", "EasyFIS Integration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                isManualSalesIntegrationStarted = false;

                dateTimePickerManualSalesIntegrationDate.Enabled = true;
                comboBoxManualSalesIntegrationTerminal.Enabled = true;

                buttonManualSalesIntegrationStart.Enabled = true;
                buttonManualSalesIntegrationStop.Enabled = false;
                buttonUpdateManualMasterFileInventory.Enabled = false;

                btnSaveLogs.Enabled = true;
                btnClearLogs.Enabled = true;
                btnSettings.Enabled = true;

                btnLogout.Enabled = true;

                manualSalesIntegrationLogMessages("Manual sales integration stopped! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n");

                tabPagePOSSalesIntegration.Enabled = true;
                tabPagePOSManualSalesIntegration.Enabled = true;
                tabPageFolderMonitoringIntegration.Enabled = true;
            }
        }

        public void folderMonitoringLogMessages(String message)
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

        private void btnStartFolderMonitoringIntegration_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBoxFolderToMonitor.Text) == true)
            {
                isFolderMonitoringIntegrationStarted = true;

                buttonFolderMonitoringIntegrationStart.Enabled = false;
                buttonFolderMonitoringIntegrationStop.Enabled = true;

                folderMonitoringLogMessages("Folder monitoring integration started! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n");

                btnSaveLogs.Enabled = false;
                btnClearLogs.Enabled = false;
                btnSettings.Enabled = false;

                btnLogout.Enabled = false;

                tabPagePOSSalesIntegration.Enabled = false;
                tabPagePOSManualSalesIntegration.Enabled = false;
                tabPageFolderMonitoringIntegration.Enabled = true;

                if (backgroundWorkerFolderMonitoringIntegration.IsBusy != true)
                {
                    backgroundWorkerFolderMonitoringIntegration.RunWorkerAsync();
                }
            }
            else
            {
                MessageBox.Show("Invalid folder to monitor directory!", "EasyFIS Integration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonFolderMonitoringIntegrationStop_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to stop folder monitoring integration?", "EasyFIS Integration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                isFolderMonitoringIntegrationStarted = false;

                buttonFolderMonitoringIntegrationStart.Enabled = true;
                buttonFolderMonitoringIntegrationStop.Enabled = false;

                btnSaveLogs.Enabled = false;
                btnClearLogs.Enabled = false;
                btnSettings.Enabled = true;

                btnLogout.Enabled = true;

                folderMonitoringLogMessages("Folder monitoring integration stopped! \r\n\nTime Stamp: " + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") + "\r\n\n\r\n\n");

                if (isFolderMonitoringOnly == false)
                {
                    tabPagePOSSalesIntegration.Enabled = true;
                    tabPagePOSManualSalesIntegration.Enabled = true;
                    tabPageFolderMonitoringIntegration.Enabled = true;
                }
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

                    DialogResult getTemplateDialogResult = MessageBox.Show("This will overwrite all existing csv template files. Are you sure you want to continue?", "EasyFIS Integration", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

                        MessageBox.Show("CSV Templates are successfully saved!", "EasyFIS Integration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "EasyFIS Integration", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public Task<String> runFolderMonitoringIntegrationSI(List<String> ext)
        {
            List<String> SIFiles = new List<String>(Directory.EnumerateFiles(textBoxFolderToMonitor.Text + "\\SI\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (SIFiles.Any())
            {
                foreach (var SIFile in SIFiles)
                {
                    FolderMonitoringTrnSalesInvoiceController folderMonitoringSI = new FolderMonitoringTrnSalesInvoiceController();
                    folderMonitoringSI.SendSalesInvoice(this, textBoxFolderMonitoringUserCode.Text, SIFile, textBoxFolderMonitoringDomain.Text);
                }
            }

            return Task.FromResult("");
        }

        public Task<String> runFolderMonitoringIntegrationOR(List<String> ext)
        {
            List<String> ORFiles = new List<String>(Directory.EnumerateFiles(textBoxFolderToMonitor.Text + "\\OR\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (ORFiles.Any())
            {
                foreach (var ORFile in ORFiles)
                {
                    FolderMonitoringTrnCollectionController folderMonitoringOR = new FolderMonitoringTrnCollectionController();
                    folderMonitoringOR.SendCollection(this, textBoxFolderMonitoringUserCode.Text, ORFile, textBoxFolderMonitoringDomain.Text);
                }
            }

            return Task.FromResult("");
        }

        public Task<String> runFolderMonitoringIntegrationRR(List<String> ext)
        {
            List<String> RRFiles = new List<String>(Directory.EnumerateFiles(textBoxFolderToMonitor.Text + "\\RR\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (RRFiles.Any())
            {
                foreach (var RRFile in RRFiles)
                {
                    FolderMonitoringTrnReceivingReceiptController folderMonitoringRR = new FolderMonitoringTrnReceivingReceiptController();
                    folderMonitoringRR.SendReceivingReceipt(this, textBoxFolderMonitoringUserCode.Text, RRFile, textBoxFolderMonitoringDomain.Text);
                }
            }

            return Task.FromResult("");
        }

        public Task<String> runFolderMonitoringIntegrationCV(List<String> ext)
        {
            List<String> CVFiles = new List<String>(Directory.EnumerateFiles(textBoxFolderToMonitor.Text + "\\CV\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (CVFiles.Any())
            {
                foreach (var CVFile in CVFiles)
                {
                    FolderMonitoringTrnDisbursementController folderMonitoringCV = new FolderMonitoringTrnDisbursementController();
                    folderMonitoringCV.SendDisbursement(this, textBoxFolderMonitoringUserCode.Text, CVFile, textBoxFolderMonitoringDomain.Text);
                }
            }

            return Task.FromResult("");
        }

        public Task<String> runFolderMonitoringIntegrationJV(List<String> ext)
        {
            List<String> JVFiles = new List<String>(Directory.EnumerateFiles(textBoxFolderToMonitor.Text + "\\JV\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (JVFiles.Any())
            {
                foreach (var JVFile in JVFiles)
                {
                    FolderMonitoringTrnJournalVoucherController folderMonitoringJV = new FolderMonitoringTrnJournalVoucherController();
                    folderMonitoringJV.SendJournalVoucher(this, textBoxFolderMonitoringUserCode.Text, JVFile, textBoxFolderMonitoringDomain.Text);
                }
            }

            return Task.FromResult("");
        }

        public Task<String> runFolderMonitoringIntegrationIN(List<String> ext)
        {
            List<String> INFiles = new List<String>(Directory.EnumerateFiles(textBoxFolderToMonitor.Text + "\\IN\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (INFiles.Any())
            {
                foreach (var INFile in INFiles)
                {
                    FolderMonitoringTrnStockInController folderMonitoringIN = new FolderMonitoringTrnStockInController();
                    folderMonitoringIN.SendStockIn(this, textBoxFolderMonitoringUserCode.Text, INFile, textBoxFolderMonitoringDomain.Text);
                }
            }

            return Task.FromResult("");
        }

        public Task<String> runFolderMonitoringIntegrationOT(List<String> ext)
        {
            List<String> OTFiles = new List<String>(Directory.EnumerateFiles(textBoxFolderToMonitor.Text + "\\OT\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (OTFiles.Any())
            {
                foreach (var OTFile in OTFiles)
                {
                    FolderMonitoringTrnStockOutController folderMonitoringOT = new FolderMonitoringTrnStockOutController();
                    folderMonitoringOT.SendStockOut(this, textBoxFolderMonitoringUserCode.Text, OTFile, textBoxFolderMonitoringDomain.Text);
                }
            }

            return Task.FromResult("");
        }

        public Task<String> runFolderMonitoringIntegrationST(List<String> ext)
        {
            List<String> STFiles = new List<String>(Directory.EnumerateFiles(textBoxFolderToMonitor.Text + "\\ST\\", "*.*", SearchOption.AllDirectories).Where(f => ext.Contains(Path.GetExtension(f))));
            if (STFiles.Any())
            {
                foreach (var STFile in STFiles)
                {
                    FolderMonitoringTrnStockTransferController folderMonitoringST = new FolderMonitoringTrnStockTransferController();
                    folderMonitoringST.SendStockTransfer(this, textBoxFolderMonitoringUserCode.Text, STFile, textBoxFolderMonitoringDomain.Text);
                }
            }

            return Task.FromResult("");
        }

        private void backgroundWorkerFolderMonitoringIntegration_DoWork(object sender, DoWorkEventArgs e)
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

                                            if (STTask.IsCompleted)
                                            {
                                                System.Threading.Thread.Sleep(5000);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void buttonUpdateMasterFileInventory_Click(object sender, EventArgs e)
        {
            isMasterFileInventoryUpdating = true;
            salesIntegrationLogMessages("Synching master files and inventory... \r\n\n");

            buttonUpdateMasterFileInventory.Enabled = false;
            buttonSalesIntegrationStop.Enabled = false;
        }

        private void buttonUpdateManualMasterFileInventory_Click(object sender, EventArgs e)
        {
            isManualMasterFileInventoryUpdating = true;
            manualSalesIntegrationLogMessages("Synching master files and inventory... \r\n\n");

            buttonUpdateManualMasterFileInventory.Enabled = false;
            buttonManualSalesIntegrationStop.Enabled = false;
        }
    }
}