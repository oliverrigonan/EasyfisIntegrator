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
    public partial class SysLoginForm : Form
    {
        private InnosoftPOSData.InnosoftPOSDataDataContext posdb = new InnosoftPOSData.InnosoftPOSDataDataContext(Controllers.SysGlobalSettings.getConnectionString());

        public String currentUser = "";

        public SysLoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            login();
        }

        public void login()
        {
            try
            {
                btnLogin.Enabled = false;
                btnCancel.Enabled = false;

                String username = txtUsername.Text;
                String password = txtPassword.Text;

                var user = from d in posdb.MstUsers
                           where d.UserName.Equals(username)
                           && d.Password.Equals(password)
                           select d;

                if (user.Any())
                {
                    currentUser = user.FirstOrDefault().FullName;

                    String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

                    String json;
                    using (StreamReader trmRead = new StreamReader(settingsPath)) { json = trmRead.ReadToEnd(); }

                    JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                    Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(json);

                    Entities.SysSettings newSysSettings = new Entities.SysSettings()
                    {
                        ConnectionString = sysSettings.ConnectionString,
                        Domain = sysSettings.Domain,
                        LogFileLocation = sysSettings.LogFileLocation,
                        FolderToMonitor = sysSettings.FolderToMonitor,
                        IsFolderMonitoringOnly = sysSettings.IsFolderMonitoringOnly,
                        FolderMonitoringUserCode = sysSettings.FolderMonitoringUserCode,
                        FolderForSentFiles = sysSettings.FolderForSentFiles,
                        ManualSalesIntegration = sysSettings.ManualSalesIntegration
                    };

                    String newJson = new JavaScriptSerializer().Serialize(newSysSettings);
                    File.WriteAllText(settingsPath, newJson);

                    TrnIntegrationForm trnInnosoftPOSIntegrationForm = new TrnIntegrationForm();
                    trnInnosoftPOSIntegrationForm.getLoginDetails(this);
                    trnInnosoftPOSIntegrationForm.Show();

                    Hide();
                }
                else
                {
                    MessageBox.Show("Incorrect Username or Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    btnLogin.Enabled = true;
                    btnCancel.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                btnLogin.Enabled = true;
                btnCancel.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Hide();

            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void SysLoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();

            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void txtUsername_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                login();
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                login();
            }
        }
    }
}
