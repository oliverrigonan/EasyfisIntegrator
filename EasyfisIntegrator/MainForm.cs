using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyfisIntegrator
{
    public partial class MainForm : Form
    {
        private InnosoftPOSData.InnosoftPOSDataDataContext posdb;

        private Timer loadingComponentsTimer = new Timer();
        private Timer connectingToDatabaseTimer = new Timer();
        private Timer launchingLoginFormTimer = new Timer();

        public MainForm()
        {
            InitializeComponent();
        }

        private void btnMainIntegrate_Click(object sender, EventArgs e)
        {
            lblLoading.Text = "Loading... Please wait...";

            btnMainIntegrate.Enabled = false;
            btnIntegrateFiles.Enabled = false;

            lblLoading.Visible = true;

            pgbMainLoading.Visible = true;
            pgbMainLoading.Value = 30;

            loadingComponentsTimer = new Timer();
            connectingToDatabaseTimer = new Timer();
            launchingLoginFormTimer = new Timer();

            loadingComponentsTimer.Interval = 2000;
            loadingComponentsTimer.Tick += new EventHandler(loadingComponentsTimerTick);
            loadingComponentsTimer.Enabled = true;
        }

        public void loadingComponentsTimerTick(object sender, EventArgs e)
        {
            loadingComponentsTimer.Dispose();
            loadingComponentsTimer.Enabled = false;

            lblLoading.Text = "Connecting database...";

            connectingToDatabaseTimer.Interval = 2000;
            connectingToDatabaseTimer.Tick += new EventHandler(connectingToDatabaseTimerTick);
            connectingToDatabaseTimer.Enabled = true;
        }

        public void connectingToDatabaseTimerTick(object sender, EventArgs e)
        {
            connectingToDatabaseTimer.Dispose();
            connectingToDatabaseTimer.Enabled = false;

            Debug.WriteLine("Time");

            connect();
        }

        public void connect()
        {
            try
            {
                posdb = new InnosoftPOSData.InnosoftPOSDataDataContext(Controllers.SysGlobalSettings.getConnectionString());
                if (posdb.DatabaseExists())
                {
                    pgbMainLoading.Value = 100;
                    lblLoading.Text = "Connected!";

                    launchingLoginFormTimer.Interval = 1500;
                    launchingLoginFormTimer.Tick += new EventHandler(launchLoginFormTimerTick);
                    launchingLoginFormTimer.Enabled = true;
                }
                else
                {
                    pgbMainLoading.Value = 100;
                    lblLoading.Text = "Failed!";

                    MessageBox.Show("Connection Failed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    btnMainIntegrate.Enabled = true;
                    btnIntegrateFiles.Enabled = true;

                    pgbMainLoading.Value = 0;
                    pgbMainLoading.Visible = false;

                    lblLoading.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                btnMainIntegrate.Enabled = true;
                btnIntegrateFiles.Enabled = true;

                pgbMainLoading.Value = 0;
                pgbMainLoading.Visible = false;

                lblLoading.Visible = false;
            }
        }

        public void launchLoginFormTimerTick(object sender, EventArgs e)
        {
            launchingLoginFormTimer.Dispose();
            launchingLoginFormTimer.Enabled = false;

            Forms.SysLoginForm sysLoginForm = new Forms.SysLoginForm();
            sysLoginForm.Show();

            Hide();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void btnIntegrateFiles_Click(object sender, EventArgs e)
        {
            btnMainIntegrate.Enabled = false;
            btnIntegrateFiles.Enabled = false;

            Forms.TrnFolderMonitoringIntegrationForm trnFilesIntegrationForm = new Forms.TrnFolderMonitoringIntegrationForm();
            trnFilesIntegrationForm.Show();
            
            Hide();
        }
    }
}
