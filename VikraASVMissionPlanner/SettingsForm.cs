using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace VikraASVMissionPlanner
{
    public partial class SettingsForm : Form
    {
        private ComboBox cmbTheme;

        private ComboBox cmbStartup;

        private CheckBox chkAutoConnect;
        private CheckBox chkAutoTelemetry;

        private ComboBox cmbRefreshRate;
        private CheckBox chkAutoCamera;

        private CheckBox chkSaveSnapshots;
        private Button btnSave;

        private Button btnCancel;
        private Label lblVersion;
        private Label lblBuildInfo;
        private Button btnResetSettings;

        public AppSettings Settings { get; private set; }
        public SettingsForm()
        {
            InitializeComponent();
            BuildUi();

        }
        private void BuildUi()
        {
            Text = "Vikra ASV Settings";

            StartPosition =
                FormStartPosition.CenterParent;

            Size =
                new Size(800, 550);

            BackColor =
                Color.FromArgb(12, 18, 32);

            ForeColor = Color.White;

            TabControl tabSettings =
    new TabControl
    {
        Dock = DockStyle.Fill,
        Font = new Font("Segoe UI", 10, FontStyle.Bold)
    };

            Panel footerPanel =
                new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 70,
                    BackColor = Color.FromArgb(12, 18, 32)
                };

            TabPage tabGeneral =
                new TabPage("General");


            //TabPage tabMission =
            //    new TabPage("Mission");

            TabPage tabTelemetry =
                new TabPage("Telemetry");

            TabPage tabCamera =
                new TabPage("Camera");

            TabPage tabSystem =
                new TabPage("System");


            tabGeneral.BackColor =
                Color.FromArgb(12, 18, 32);

            ////tabMission.BackColor =
            //    Color.FromArgb(12, 18, 32);

            tabTelemetry.BackColor =
                Color.FromArgb(12, 18, 32);

            tabCamera.BackColor =
                Color.FromArgb(12, 18, 32);

            tabSystem.BackColor =
                Color.FromArgb(12, 18, 32);

            tabGeneral.ForeColor = Color.White;
            //tabMission.ForeColor = Color.White;
            tabTelemetry.ForeColor = Color.White;
            tabCamera.ForeColor = Color.White;
            tabSystem.ForeColor = Color.White;

            tabSettings.TabPages.Add(tabGeneral);
            //tabSettings.TabPages.Add(tabMission);
            tabSettings.TabPages.Add(tabTelemetry);
            tabSettings.TabPages.Add(tabCamera);
            tabSettings.TabPages.Add(tabSystem);

            BuildGeneralTab(tabGeneral);
            BuildTelemetryTab(tabTelemetry);
            BuildCameraTab(tabCamera);
            BuildSystemTab(tabSystem);
            btnCancel.Location =
    new Point(180, 10);

            btnSave.Location =
                new Point(310, 10);

            btnCancel.Anchor = AnchorStyles.Top;
            btnSave.Anchor = AnchorStyles.Top;
            footerPanel.Controls.Add(btnCancel);
            footerPanel.Controls.Add(btnSave);
            
            //tabTelemetry.Controls.Add(btnSave);
            //tabTelemetry.Controls.Add(btnCancel);

            //tabCamera.Controls.Add(btnSave);
            //tabCamera.Controls.Add(btnCancel);

            //tabSystem.Controls.Add(btnSave);
            //tabSystem.Controls.Add(btnCancel);

            Controls.Add(footerPanel);
            Controls.Add(tabSettings);
            footerPanel.BackColor =
    Color.FromArgb(12, 18, 32);
            btnSave.BackColor = Color.FromArgb(0, 153, 255);
            btnCancel.BackColor = Color.FromArgb(28, 38, 58);
            btnSave.Cursor = Cursors.Hand;
            btnCancel.Cursor = Cursors.Hand;
            //btnCancel.BackColor = SystemColors.Control;
            //btnSave.BackColor = SystemColors.Control;
            //Controls.Add(btnSave);
            //Controls.Add(btnCancel);
            //        btnSave.Anchor =
            //AnchorStyles.Bottom | AnchorStyles.Right;

            //        btnCancel.Anchor =
            //            AnchorStyles.Bottom | AnchorStyles.Right;


        }
        private void BuildGeneralTab(
    TabPage page)
        {
            Label lblTheme =
                new Label
                {
                    Text = "Application Theme",
                    ForeColor = Color.White,
                    Location = new Point(30, 30),
                    AutoSize = true
                };

            cmbTheme =
               new ComboBox
               {
                   Location = new Point(30, 60),
                   Width = 220,
                   DropDownStyle =
                       ComboBoxStyle.DropDownList
               };

            cmbTheme.Items.AddRange(
                new object[]
                {
            "Dark",
            "Light"
                });

            cmbTheme.SelectedIndex = 0;

            Label lblStartup =
                new Label
                {
                    Text = "Startup Page",
                    ForeColor = Color.White,
                    Location = new Point(30, 120),
                    AutoSize = true
                };

            cmbStartup =
                new ComboBox
                {
                    Location = new Point(30, 150),
                    Width = 220,
                    DropDownStyle =
                        ComboBoxStyle.DropDownList
                };

            cmbStartup.Items.AddRange(
                new object[]
                {
            "Mission",
            "Data",
            "Simulation",
            "Target Mode",
            "Help"
                });

            cmbStartup.SelectedIndex = 0;

            chkAutoConnect =
                new CheckBox
                {
                    Text =
                        "Auto Connect Pixhawk On Startup",

                    ForeColor = Color.White,

                    BackColor = Color.Transparent,

                    AutoSize = true,

                    Location =
                        new Point(30, 220)
                };

            btnSave =
                new Button
                {
                    Text = "Save",
                    Width = 120,
                    Height = 36,
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,

                    FlatStyle = FlatStyle.Flat
                    //Location = new Point(630, 12)
                };
            btnSave.Click += BtnSave_Click;
            btnSave.FlatAppearance.BorderSize = 0;

            btnCancel =
                new Button
                {
                    Text = "Cancel",
                    Width = 120,
                    Height = 36,
                    BackColor = Color.FromArgb(40, 50, 70),
                    ForeColor = Color.White,

                    FlatStyle = FlatStyle.Flat
                    //Location = new Point(500, 12)
                };
            btnCancel.FlatAppearance.BorderSize = 0;

            btnCancel.Click +=
                (s, e) => Close();

            page.Controls.Add(lblTheme);
            page.Controls.Add(cmbTheme);

            page.Controls.Add(lblStartup);
            page.Controls.Add(cmbStartup);

            page.Controls.Add(chkAutoConnect);

            //page.Controls.Add(btnSave);
            //page.Controls.Add(btnCancel);
        }
        private void BuildTelemetryTab(
    TabPage page)
        {
            chkAutoTelemetry =
    new CheckBox
    {
                    Text =
                        "Auto Start Telemetry",

                    ForeColor = Color.White,

                    BackColor = Color.Transparent,

                    AutoSize = true,

                    Location =
                        new Point(30, 40)
                };

            Label lblRefresh =
                new Label
                {
                    Text = "Refresh Rate",

                    ForeColor = Color.White,

                    AutoSize = true,

                    Location =
                        new Point(30, 100)
                };

            cmbRefreshRate =
    new ComboBox
    {
                    Width = 220,

                    Location =
                        new Point(30, 130),

                    DropDownStyle =
                        ComboBoxStyle.DropDownList
                };

            cmbRefreshRate.Items.AddRange(
                new object[]
                {
            "100 ms",
            "250 ms",
            "500 ms",
            "1000 ms"
                });

            cmbRefreshRate.SelectedIndex = 0;

            page.Controls.Add(
                chkAutoTelemetry);

            page.Controls.Add(
                lblRefresh);

            page.Controls.Add(
                cmbRefreshRate);
        }
        private void BuildCameraTab(
    TabPage page)
{
    chkAutoCamera =
        new CheckBox
        {
            Text = "Auto Start Camera",

            ForeColor = Color.White,

            BackColor = Color.Transparent,

            AutoSize = true,

            Location =
                new Point(30, 40)
        };

    chkSaveSnapshots =
        new CheckBox
        {
            Text = "Save Snapshots",

            ForeColor = Color.White,

            BackColor = Color.Transparent,

            AutoSize = true,

            Location =
                new Point(30, 90)
        };

    page.Controls.Add(
        chkAutoCamera);

    page.Controls.Add(
        chkSaveSnapshots);
}
        private void BuildSystemTab(
    TabPage page)
        {
            lblVersion =
                new Label
                {
                    Text =
                        "Version: Vikra ASV Mission Planner v1.0",

                    ForeColor = Color.White,

                    AutoSize = true,

                    Location =
                        new Point(30, 40)
                };

            lblBuildInfo =
                new Label
                {
                    Text =
                        "Build: Vikra Ocean Tech Pvt. Ltd",

                    ForeColor = Color.White,

                    AutoSize = true,

                    Location =
                        new Point(30, 90)
                };

            btnResetSettings =
                new Button
                {
                    Text = "Reset Settings",

                    Width = 180,

                    Height = 40,

                    Location =
                        new Point(30, 160)
                };

            btnResetSettings.Click +=
                BtnResetSettings_Click;

            page.Controls.Add(lblVersion);
            page.Controls.Add(lblBuildInfo);
            page.Controls.Add(btnResetSettings);
        }
        private void BtnResetSettings_Click(
    object sender,
    EventArgs e)
        {
            DialogResult result =
                MessageBox.Show(
                    "Reset all settings?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            Settings = new AppSettings();

            cmbTheme.SelectedIndex = 0;
            cmbStartup.SelectedIndex = 0;

            chkAutoConnect.Checked = false;

            chkAutoTelemetry.Checked = false;
            cmbRefreshRate.SelectedIndex = 0;

            chkAutoCamera.Checked = false;
            chkSaveSnapshots.Checked = false;

           
        }

        private void BtnSave_Click(
        object sender,
        EventArgs e)
        {
            Settings = new AppSettings
            {
                Theme =
                    cmbTheme.SelectedItem?.ToString(),

                StartupPage =
                    cmbStartup.SelectedItem?.ToString(),

                AutoConnectPixhawk =
    chkAutoConnect.Checked,
    AutoStartTelemetry =
    chkAutoTelemetry.Checked,

                TelemetryRefreshRate =
    int.Parse(
        cmbRefreshRate.SelectedItem
            .ToString()
            .Replace(" ms", "")),
    AutoStartCamera =
    chkAutoCamera.Checked,

                SaveSnapshots =
    chkSaveSnapshots.Checked,
            };

            

            DialogResult = DialogResult.OK;

            Close();
        }
    }
}


