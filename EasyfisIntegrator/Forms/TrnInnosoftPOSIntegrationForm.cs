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
    public partial class TrnInnosoftPOSIntegrationForm : Form
    {
        private InnosoftPOSData.InnosoftPOSDataDataContext posdb = new InnosoftPOSData.InnosoftPOSDataDataContext(Controllers.SysGlobalSettings.getConnectionString());

        public TrnInnosoftPOSIntegrationForm()
        {
            InitializeComponent();
            getPOSSettings();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            Hide();

            SysLoginForm sysLoginForm = new SysLoginForm();
            sysLoginForm.Show();
        }

        public void getPOSSettings()
        {
            var settings = from d in posdb.SysSettings select d;
            if (settings.Any())
            {
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
        }

        private void TrnInnosoftPOSIntegrationForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to exit this application?", "Exit Integrator?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
                Activate();
            }
        }
    }
}
