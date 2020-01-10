using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    public partial class SysSettings : Form
    {
        private InnosoftPOSData.InnosoftPOSDataDataContext posdb = new InnosoftPOSData.InnosoftPOSDataDataContext(Controllers.SysGlobalSettings.getConnectionString());

        public TrnIntegrationForm trnInnosoftPOSIntegrationForm;
        public Boolean isFolderMonitoringOnly = false;

        public SysSettings(TrnIntegrationForm form)
        {
            InitializeComponent();
            trnInnosoftPOSIntegrationForm = form;

            getSettings();
        }

        private void btnCloseSettings_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void getSettings()
        {
            String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

            String json;
            using (StreamReader trmRead = new StreamReader(settingsPath)) { json = trmRead.ReadToEnd(); }

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(json);

            txtDomain.Text = sysSettings.Domain;
            txtLogFileLocation.Text = sysSettings.LogFileLocation;
            txtFolderToMonitor.Text = sysSettings.FolderToMonitor;
            txtFMUserCode.Text = sysSettings.FolderMonitoringUserCode;
            isFolderMonitoringOnly = sysSettings.IsFolderMonitoringOnly;
            txtFolderForSentFiles.Text = sysSettings.FolderForSentFiles;
            cbxManualSalesIntegration.Checked = sysSettings.ManualSalesIntegration;

            if (isFolderMonitoringOnly)
            {
                txtConnectionString.Text = "";

                tabPagePOSSettings.Enabled = false;
                tabPageConnection.Enabled = false;
                tabSettings.SelectedTab = tabPageSystem;

                tabSettings.TabPages.Remove(tabPagePOSSettings);
                tabSettings.TabPages.Remove(tabPageConnection);
            }
            else
            {
                txtConnectionString.Text = sysSettings.ConnectionString;

                tabPagePOSSettings.Enabled = true;
                tabPageConnection.Enabled = true;
                tabSettings.SelectedTab = tabPagePOSSettings;

                cboPostUser.DataSource = from d in posdb.MstUsers where d.IsLocked == true select d;
                cboPostUser.ValueMember = "Id";
                cboPostUser.DisplayMember = "Username";

                cboPostSupplier.DataSource = from d in posdb.MstSuppliers where d.IsLocked == true select d;
                cboPostSupplier.ValueMember = "Id";
                cboPostSupplier.DisplayMember = "Supplier";

                var settings = from d in posdb.SysSettings select d;
                if (settings.Any())
                {
                    txtBranchCode.Text = settings.FirstOrDefault().BranchCode;
                    txtUserCode.Text = settings.FirstOrDefault().UserCode;
                    cbxUseItemPrice.Checked = settings.FirstOrDefault().UseItemPrice;
                    cboPostUser.SelectedValue = settings.FirstOrDefault().PostUserId;
                    cboPostSupplier.SelectedValue = settings.FirstOrDefault().PostSupplierId;
                }
            }
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Save changes?", "Save Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                if (!isFolderMonitoringOnly)
                {
                    var settings = from d in posdb.SysSettings select d;
                    if (settings.Any())
                    {
                        var updateSettings = settings.FirstOrDefault();
                        updateSettings.BranchCode = txtBranchCode.Text;
                        updateSettings.UserCode = txtUserCode.Text;
                        updateSettings.UseItemPrice = cbxUseItemPrice.Checked;
                        updateSettings.PostUserId = Convert.ToInt32(cboPostUser.SelectedValue);
                        updateSettings.PostSupplierId = Convert.ToInt32(cboPostSupplier.SelectedValue);
                        posdb.SubmitChanges();
                    }
                }

                Entities.SysSettings newSysSettings = new Entities.SysSettings()
                {
                    ConnectionString = txtConnectionString.Text,
                    Domain = txtDomain.Text,
                    LogFileLocation = txtLogFileLocation.Text,
                    FolderToMonitor = txtFolderToMonitor.Text,
                    IsFolderMonitoringOnly = isFolderMonitoringOnly,
                    FolderMonitoringUserCode = txtFMUserCode.Text,
                    FolderForSentFiles = txtFolderForSentFiles.Text,
                    ManualSalesIntegration = cbxManualSalesIntegration.Checked
                };

                String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");
                String newJson = new JavaScriptSerializer().Serialize(newSysSettings);

                File.WriteAllText(settingsPath, newJson);

                trnInnosoftPOSIntegrationForm.getSettings();

                Close();
            }
        }

        private void btnLocateLogFileLocation_Click(object sender, EventArgs e)
        {
            fbdLogFileLocation.ShowDialog();
            txtLogFileLocation.Text = fbdLogFileLocation.SelectedPath;
        }

        private void btnLocateFolderToMonitor_Click(object sender, EventArgs e)
        {
            fbdFolderToMonitor.ShowDialog();
            txtFolderToMonitor.Text = fbdFolderToMonitor.SelectedPath;
        }

        private void btnLocateFolderForSentFiles_Click(object sender, EventArgs e)
        {
            fbdFolderForSentFiles.ShowDialog();
            txtFolderForSentFiles.Text = fbdFolderForSentFiles.SelectedPath;
        }
    }
}