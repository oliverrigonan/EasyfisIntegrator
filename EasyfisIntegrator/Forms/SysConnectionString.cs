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
    public partial class SysConnectionString : Form
    {
        public MainForm mainForm;

        public SysConnectionString(MainForm form)
        {
            InitializeComponent();

            mainForm = form;

            String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

            String json;
            using (StreamReader trmRead = new StreamReader(settingsPath)) { json = trmRead.ReadToEnd(); }

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(json);

            txtConnectionString.Text = sysSettings.ConnectionString;
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Save changes?", "Save Settings", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                String settingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Settings.json");

                String json;
                using (StreamReader trmRead = new StreamReader(settingsPath)) { json = trmRead.ReadToEnd(); }

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                Entities.SysSettings sysSettings = javaScriptSerializer.Deserialize<Entities.SysSettings>(json);

                Entities.SysSettings newSysSettings = new Entities.SysSettings()
                {
                    ConnectionString = txtConnectionString.Text,
                    Domain = sysSettings.Domain,
                    LogFileLocation = sysSettings.LogFileLocation,
                    FolderToMonitor = sysSettings.FolderToMonitor,
                    IsFolderMonitoringOnly = false,
                    FolderMonitoringUserCode = sysSettings.FolderMonitoringUserCode,
                    FolderForSentFiles = sysSettings.FolderForSentFiles
                };

                String newJson = new JavaScriptSerializer().Serialize(newSysSettings);
                File.WriteAllText(settingsPath, newJson);

                mainForm.connectInnosoftPOSIntegration();

                Close();
            }
        }

        private void btnCloseSettings_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
