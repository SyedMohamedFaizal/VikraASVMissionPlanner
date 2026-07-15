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

                    Font = new Font(
                        "Segoe UI",
                        10,
                        FontStyle.Bold)
                };

            TabPage tabGeneral =
                new TabPage("General");


            TabPage tabMission =
                new TabPage("Mission");

            TabPage tabTelemetry =
                new TabPage("Telemetry");

            TabPage tabCamera =
                new TabPage("Camera");

            TabPage tabSystem =
                new TabPage("System");


            tabGeneral.BackColor =
                Color.FromArgb(12, 18, 32);

            tabMission.BackColor =
                Color.FromArgb(12, 18, 32);

            tabTelemetry.BackColor =
                Color.FromArgb(12, 18, 32);

            tabCamera.BackColor =
                Color.FromArgb(12, 18, 32);

            tabSystem.BackColor =
                Color.FromArgb(12, 18, 32);

            tabGeneral.ForeColor = Color.White;
            tabMission.ForeColor = Color.White;
            tabTelemetry.ForeColor = Color.White;
            tabCamera.ForeColor = Color.White;
            tabSystem.ForeColor = Color.White;

            tabSettings.TabPages.Add(tabGeneral);
            tabSettings.TabPages.Add(tabMission);
            tabSettings.TabPages.Add(tabTelemetry);
            tabSettings.TabPages.Add(tabCamera);
            tabSettings.TabPages.Add(tabSystem);

            BuildGeneralTab(tabGeneral);
            Controls.Add(tabSettings);
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

            Button btnSave =
                new Button
                {
                    Text = "Save",
                    Width = 120,
                    Height = 36,
                    Location = new Point(600, 420)
                };
            btnSave.Click += BtnSave_Click;

            Button btnCancel =
                new Button
                {
                    Text = "Cancel",
                    Width = 120,
                    Height = 36,
                    Location = new Point(470, 420)
                };

            btnCancel.Click +=
                (s, e) => Close();

            page.Controls.Add(lblTheme);
            page.Controls.Add(cmbTheme);

            page.Controls.Add(lblStartup);
            page.Controls.Add(cmbStartup);

            page.Controls.Add(chkAutoConnect);

            page.Controls.Add(btnSave);
            page.Controls.Add(btnCancel);
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
    chkAutoConnect.Checked
            };

            MessageBox.Show(
                "Settings Saved",
                "Settings",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;

            Close();
        }
    }
}


