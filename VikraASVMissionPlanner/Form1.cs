using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner;
using MissionPlanner.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VikraASVMissionPlanner.Managers;
using VikraASVMissionPlanner.Models;
using VikraASVMissionPlanner.Services;
//using LibVLCSharp.Shared;
//using LibVLCSharp.WinForms;
namespace VikraASVMissionPlanner
{
    public partial class Form1 : Form
    {
        private readonly ThemeColors darkTheme = ThemeColors.CreateDark();
        private readonly ThemeColors lightTheme = ThemeColors.CreateLight();
        private readonly MissionManager missionManager;
        private readonly IMissionPlannerAdapter missionPlannerAdapter;
        private Timer simulationTimer;
        private Timer hudTimer;
        private GMarkerGoogle boatMarker;
        private Button simulationPauseButton;
        private Button simulationClearButton;
        private Button simulationStartButton;
        private Label lblSimStatus;
        private Label lblSimWaypoint;
        private Label lblSimProgress;

        private Label lblDistanceLeft;
        private Label lblEta;
        private Panel stageIndicatorPanel;

        private Label lblStageCruise;
        private Label lblStageCruiseRest;

        private Label lblStageLoiter;
        private Label lblStageLoiterRest;

        private Label lblStageBurst;
        private Label lblStageBurstRest;

        private Label lblStageReturn;
        private Label lblStageReturnRest;

        private Dictionary<string, Panel>
            stageIndicatorGroups;

        private string lastActiveStageType = null;
        private bool simulationPaused;
        private bool simulationRunning;

        private List<MissionPoint> simulationPoints =
            new List<MissionPoint>();

        private int currentTargetIndex = 0;

        private double currentLat;
        private double currentLon;
        private readonly Timer clockTimer;
        private readonly List<Label> metricCaptionLabels = new List<Label>();
        private readonly List<Label> metricValueLabels = new List<Label>();
        private readonly List<Label> themeLabels = new List<Label>();
        private readonly List<Button> accentButtons;
        private readonly Dictionary<string, Label> dataValueLabels;
        private readonly Dictionary<string, Panel> dataTabPanels;
        private readonly Dictionary<string, Button> dataTabButtons;
        private Dictionary<string, Button> helpTopicButtons;
        //private Dictionary<string, Panel> helpTopics;

        private Label lblHelpTopicTitle;
        private Label lblHelpTopicSubtitle;
        private Dictionary<string, Control> helpTopicPages =
    new Dictionary<string, Control>();

        private Panel helpTopicHost;
        private readonly Dictionary<string, Button> headerTabs;
        private readonly Dictionary<string, ComboBox>
    telemetryCardDropdowns =
        new Dictionary<string, ComboBox>();

        private readonly Dictionary<string, Label>
            telemetryCardValues =
                new Dictionary<string, Label>();
        private readonly List<Button> neutralButtons;
        private readonly List<SectionPanel> sectionPanels;
        private readonly Dictionary<string, Button> stageButtons;
        private readonly List<ComboBox> comboBoxes;
        private readonly List<ThemeAwareControl> themeAwareControls;

        private ThemeColors currentTheme;
        private AppPage currentPage = AppPage.Mission;

        // Header / statusbar
        private Panel headerPanel;
        private Panel statusBarPanel;
        private Panel contentHost;
        private Panel dataMapHost;
        private Panel telemetryStripPanel;
        private Panel dataTabHost;
        private Panel secondaryViewportHost;
        private Panel cameraTopPanel;

        private Panel cameraPlaceholderPanel;
        private PictureBox cameraPictureBox;
        private PictureBox targetCameraPictureBox;
        private System.Net.Sockets.UdpClient targetUdp;
        private List<TargetData> targets =
    new List<TargetData>();

        private TargetData selectedTarget;
        private System.Net.IPEndPoint
    lastSenderEndpoint;
        private Button btnTestTarget;
        private Button btnLockTarget;
        private Button btnUnlockTarget;
        private Button btnClearTargets;
        private TextBox txtRtspUrl;
        private Button btnConnectRtsp;
        private Button btnUseWebcam;
        private Label lblCameraStatus;
        private OpenCvSharp.VideoCapture webcamCapture;
        private System.Windows.Forms.Timer webcamTimer;
        private OpenCvSharp.Mat webcamFrame;
        private bool isRtspMode = false;
        private string rtspUrl = "";
        private bool rtspConnected = false;
        private bool webcamConnected = false;
        //private LibVLC _libVlc;
        //private MediaPlayer _mediaPlayer;
        //private VideoView _videoView;

        private bool _videoInitialized = false;
        private Button btnSwap;
        private bool isMapPrimary = true;
        private MissionPlanner.Controls.HUD hud1;
        private Label lblHeaderTimeValue;
        private Panel missionMapHost;
        private Control missionPage;
        private Control dataPage;
        private Control simulationPage;
        private Panel targetModePage;
        private Control helpPage;
        private Panel simMapHost;

        // Left panel live labels
        private Label lblSelectedStageValue;
        private Label lblPatternStatusValue;
        private ComboBox cmbPattern;
        private Label lblMissionModeValue;
        private Label lblLoiterStageSubtitle;
        private Label lblCruiseStageSubtitle;
        private Label lblBurstStageSubtitle;
        private Label lblReturnCruiseStageSubtitle;
        private Label lblCruiseSpeed;
        private Label lblLoiterSpeed;
        private Label lblBurstSpeed;
        private Label lblReturnCruiseSpeed;

        // Right panel live labels
        private Label lblSummaryWaypointsValue;
        private Label lblSummaryDistanceValue;
        private Label lblSummaryDurationValue;
        private Label lblSummaryFuelValue;
        private Label lblStatusPanelReady;
        private Label lblStatusPanelGps;
        private Label lblStatusPanelStage;
        private Label lblStatusPanelArmed;
        private Label lblSurveyLinesValue;
        private Label lblSurveyCoverageValue;

        // Status bar
        private Label lblStatusBarReady;
        private Label lblStatusBarGps;
        private Label lblStatusBarBattery;
        private Label lblStatusBarMode;
        private Label lblStatusBarArmed;

        // Theme
        private ThemeToggleControl themeToggle;
        private bool isDarkMode = true;
        //private MissionPlanner.Controls.HUD hud1;

        // Sections
        private SectionPanel missionBuilderSection;
        private SectionPanel mapSection;
        private SectionPanel waypointSection;
        private SectionPanel missionSummarySection;

        // Survey heading input
        private NumericUpDown nudSurveyHeading;
        private NumericUpDown nudSurveySpacing;

        // Map
        private DataGridView dgvWaypoints;
        private GMapControl gmap;
        private GMapOverlay waypointOverlay;
        private GMapOverlay routeOverlay;
        private GMapOverlay polygonOverlay;
        private GMapOverlay surveyOverlay;
        private ContextMenuStrip waypointActionMenu;
        private MissionPoint selectedWaypoint;

        private string selectedStageName = "Cruise";
        private bool surveyPolygonMode;
        private bool isDarkModeFlag = true;
        private bool isConnected = false;

        private enum AppPage
        {
            Mission,
            Data,
            Simulation,
            TargetMode,
            Help
        }

        public Form1()
        {
            missionManager = new MissionManager();
            missionPlannerAdapter = new MissionPlannerAdapter();
            clockTimer = new Timer();
            accentButtons = new List<Button>();
            dataValueLabels = new Dictionary<string, Label>(StringComparer.OrdinalIgnoreCase);
            dataTabPanels = new Dictionary<string, Panel>(StringComparer.OrdinalIgnoreCase);
            dataTabButtons = new Dictionary<string, Button>(StringComparer.OrdinalIgnoreCase);
            headerTabs = new Dictionary<string, Button>(StringComparer.OrdinalIgnoreCase);
            neutralButtons = new List<Button>();
            sectionPanels = new List<SectionPanel>();
            stageButtons = new Dictionary<string, Button>(StringComparer.OrdinalIgnoreCase);
            comboBoxes = new List<ComboBox>();
            themeAwareControls = new List<ThemeAwareControl>();
            currentTheme = darkTheme;
            stageIndicatorGroups =
    new Dictionary<string, Panel>(
        StringComparer.OrdinalIgnoreCase);
            helpTopicButtons =
    new Dictionary<string, Button>(
        StringComparer.OrdinalIgnoreCase);

            //helpTopics =
            //    new Dictionary<string, Panel>(
            //        StringComparer.OrdinalIgnoreCase);
            InitializeComponent();
            missionManager.MissionChanged += MissionManager_MissionChanged;

            BuildUi();
            InitializeMap();
            BuildWaypointActionMenu();
            //SeedDemoMission();
            RefreshWaypointGrid();
            RefreshMissionSummary();
            RefreshMapFromMission();
            ApplyDarkTheme();
            StartTargetReceiver();


            //clockTimer.Interval = 1000;
            //clockTimer.Tick += ClockTimer_Tick;
            //clockTimer.Start();
            //UpdateUtcClock();
        }

        // ═══════════════════════════════════════════════════════════════
        // UI CONSTRUCTION
        // ═══════════════════════════════════════════════════════════════

        private void BuildUi()
        {
            //this.Icon = new Icon("mpdesktop.ico");
            cameraPlaceholderPanel =
    BuildCameraPlaceholderPanel();
            StartWebcam();
            //InitializeVideoPlayer();
            SuspendLayout();
            Text = "Vikra ASV Ground Control System";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1280, 800);
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;


            headerPanel = BuildHeader();
            //statusBarPanel = BuildStatusBar();


            TableLayoutPanel shell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 1,
                Padding = new Padding(8, 6, 8, 4)
            };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            contentHost = new Panel { Dock = DockStyle.Fill };
            missionPage = BuildMissionPage();
            dataPage = BuildDataPage();
            simulationPage = BuildSimulationPage();
            targetModePage = BuildTargetModePage();
            helpPage = BuildHelpPage();
            targetModePage.Visible = false;
            dataPage.Visible = false;
            simulationPage.Visible = false;
            helpPage.Visible = false;

            missionPage.Visible = true;

            contentHost.Controls.Add(helpPage);
            contentHost.Controls.Add(targetModePage);
            contentHost.Controls.Add(simulationPage);
            contentHost.Controls.Add(dataPage);
            contentHost.Controls.Add(missionPage);
            shell.Controls.Add(contentHost, 0, 0);

            Controls.Add(shell);
            //Controls.Add(statusBarPanel);
            Controls.Add(headerPanel);
            ResumeLayout();
            SwitchPage(AppPage.Mission);
        }

        // ───────────────────────────────────────────────────────────────
        // HEADER
        // ───────────────────────────────────────────────────────────────

        private Panel BuildHeader()
        {
            Panel panel = new Panel { Dock = DockStyle.Top, Height = 62, Padding = new Padding(12, 0, 12, 0) };

            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4 };
            layout.ColumnStyles.Clear();

            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    150F));

            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    260F));

            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            layout.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    360F));


            // Brand
            Panel brandPanel = new Panel { Dock = DockStyle.Fill };
            LogoControl logo = new LogoControl { Theme = currentTheme, Location = new Point(6, 18), Size = new Size(28, 20) };
            themeAwareControls.Add(logo);
            brandPanel.Controls.Add(logo);
            Label lblBrand = CreateLabel(
    "ASV/KAMIKAZE\nWAVEbot",
    9F,
    FontStyle.Bold,
    currentTheme.TextPrimary);
            lblBrand.Location = new Point(38, 16);
            brandPanel.Controls.Add(lblBrand);

            // Title
            Panel titlePanel = new Panel { Dock = DockStyle.Fill };
            Label lblTitle = CreateLabel("Vikra ASV Ground Control System", 10F, FontStyle.Bold, currentTheme.TextPrimary);
            lblTitle.Location = new Point(30, 10);
            titlePanel.Controls.Add(lblTitle);
            Label lblSubtitle = CreateLabel("Cruise \u2013 Loiter \u2013 Burst \u2013 Return Cruise", 8F, FontStyle.Regular, currentTheme.TextSecondary);
            lblSubtitle.Location = new Point(31, 32);
            titlePanel.Controls.Add(lblSubtitle);

            // Status indicators
            FlowLayoutPanel statusFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 4, 0, 0)
            };

            statusFlow.Controls.Add(CreateHeaderTab("MISSION", AppPage.Mission));
            statusFlow.Controls.Add(CreateHeaderTab("DATA", AppPage.Data));
            statusFlow.Controls.Add(CreateHeaderTab("SIMULATION", AppPage.Simulation));
            statusFlow.Controls.Add(CreateHeaderTab("TARGET MODE", AppPage.TargetMode));
            statusFlow.Controls.Add(CreateHeaderTab("HELP", AppPage.Help));
            //statusFlow.Controls.Add(CreateHeaderStatus("VEHICLE STATUS", "AUTO", currentTheme.Success));
            //statusFlow.Controls.Add(CreateHeaderStatus("MODE", "AUTO", currentTheme.TextPrimary));
            //statusFlow.Controls.Add(CreateHeaderStatus("ARMED", "ARMED", currentTheme.Success));
            //statusFlow.Controls.Add(CreateHeaderStatus("LINK", "OK", currentTheme.Success));
            //statusFlow.Controls.Add(CreateHeaderStatus("GPS", "18", currentTheme.Success));
            //statusFlow.Controls.Add(CreateHeaderStatus("TIME", "00:00:00 UTC", currentTheme.TextPrimary, true));

            // Actions
            FlowLayoutPanel actionFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 12, 0, 0)
            };

            themeToggle = new ThemeToggleControl
            {
                Theme = currentTheme,
                Checked = true,
                Margin = new Padding(0, 5, 8, 0)
            };
            themeToggle.ToggleChanged += ThemeToggle_ToggleChanged;
            themeAwareControls.Add(themeToggle);
            actionFlow.Controls.Add(themeToggle);

            Button btnSettings = CreateButton("Settings", currentTheme.PanelAlt, currentTheme.TextPrimary, 78, 34, false);
            btnSettings.Margin = new Padding(0, 2, 6, 0);
            btnSettings.Click += PlaceholderButton_Click;
            actionFlow.Controls.Add(btnSettings);

            Button btnConnect = CreateButton(
    "Connect",
    currentTheme.PanelAlt,
    currentTheme.TextPrimary,
    90,
    34,
    false);

            btnConnect.Name = "btnConnect";
            btnConnect.Margin = new Padding(0, 2, 6, 0);

            btnConnect.Click += async (s, e) =>
            {
                if (!isConnected)
                {
                    btnConnect.Text = "Connecting...";
                    btnConnect.Enabled = false;

                    bool connected =
                        await missionPlannerAdapter.ConnectAsync();

                    if (connected)
                    {
                        isConnected = true;

                        if (hudTimer == null)
                        {
                            hudTimer = new Timer();
                            hudTimer.Interval = 100;
                            hudTimer.Tick += HudTimer_Tick;
                        }

                        hudTimer.Start();

                        btnConnect.Text = "Disconnect";
                        btnConnect.Enabled = true;

                        MessageBox.Show(
                            "Pixhawk Connected",
                            "Connection",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        if (lblStatusBarReady != null)
                        {
                            lblStatusBarReady.Text = "Ready";
                            lblStatusBarReady.ForeColor = currentTheme.Success;
                        }

                        if (lblStatusPanelReady != null)
                        {
                            lblStatusPanelReady.Text = "Ready";
                            lblStatusPanelReady.ForeColor =
                                currentTheme.Success;
                        }

                        if (lblStatusPanelGps != null)
                        {
                            lblStatusPanelGps.Text = "3D Fix";
                            lblStatusPanelGps.ForeColor =
                                currentTheme.Success;
                        }

                        if (lblStatusPanelStage != null)
                        {
                            lblStatusPanelStage.Text =
                                selectedStageName.ToUpperInvariant();
                        }

                        if (lblStatusPanelArmed != null)
                        {
                            lblStatusPanelArmed.Text = "YES";
                            lblStatusPanelArmed.ForeColor =
                                currentTheme.Success;
                        }
                    }
                    else
                    {
                        btnConnect.Text = "Connect";
                        btnConnect.Enabled = true;

                        MessageBox.Show(
                            "Connection Failed",
                            "Connection",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        if (lblStatusBarReady != null)
                        {
                            lblStatusBarReady.Text = "Offline";
                            lblStatusBarReady.ForeColor =
                                currentTheme.AccentRed;
                        }

                        if (lblStatusPanelReady != null)
                        {
                            lblStatusPanelReady.Text = "Offline";
                            lblStatusPanelReady.ForeColor =
                                currentTheme.AccentRed;
                        }

                        if (lblStatusPanelGps != null)
                        {
                            lblStatusPanelGps.Text = "--";
                        }

                        if (lblStatusPanelStage != null)
                        {
                            lblStatusPanelStage.Text = "--";
                        }

                        if (lblStatusPanelArmed != null)
                        {
                            lblStatusPanelArmed.Text = "--";
                        }
                    }
                }
                else
                {
                    if (MainV2.comPort?.BaseStream != null &&
                        MainV2.comPort.BaseStream.IsOpen)
                    {
                        MainV2.comPort.BaseStream.Close();
                    }

                    isConnected = false;
                    hudTimer?.Stop();
                    btnConnect.Text = "Connect";

                    if (lblStatusBarReady != null)
                    {
                        lblStatusBarReady.Text = "Offline";
                        lblStatusBarReady.ForeColor =
                            currentTheme.AccentRed;
                    }

                    if (lblStatusPanelReady != null)
                    {
                        lblStatusPanelReady.Text = "Offline";
                        lblStatusPanelReady.ForeColor =
                            currentTheme.AccentRed;
                    }

                    if (lblStatusPanelGps != null)
                    {
                        lblStatusPanelGps.Text = "--";
                    }

                    if (lblStatusPanelStage != null)
                    {
                        lblStatusPanelStage.Text = "--";
                    }

                    if (lblStatusPanelArmed != null)
                    {
                        lblStatusPanelArmed.Text = "--";
                    }

                    MessageBox.Show(
                        "Pixhawk Disconnected",
                        "Connection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            };
            actionFlow.Controls.Add(btnConnect);

            Button btnDisarm = CreateButton("DISARM", currentTheme.AccentRed, Color.White, 80, 35, true);
            btnDisarm.Margin = new Padding(0, 1, 0, 0);
            btnDisarm.Click += PlaceholderButton_Click;
            actionFlow.Controls.Add(btnDisarm);

            layout.Controls.Add(brandPanel, 0, 0);
            layout.Controls.Add(titlePanel, 1, 0);
            layout.Controls.Add(statusFlow, 2, 0);
            layout.Controls.Add(actionFlow, 3, 0);
            panel.Controls.Add(layout);
            return panel;
        }

        private Control BuildMissionPage()
        {
            TableLayoutPanel shell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1
            };
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F));
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            shell.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));

            shell.Controls.Add(BuildLeftPanel(), 0, 0);
            shell.Controls.Add(BuildCenterPanel(), 1, 0);
            shell.Controls.Add(BuildRightPanel(), 2, 0);
            return shell;
        }

        private Control BuildDataPage()
        {
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 72F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            layout.Controls.Add(BuildDataTelemetrySidebar(), 0, 0);
            layout.Controls.Add(BuildDataMapPanel(), 1, 0);
            return layout;
        }
        private Control BuildSimulationPage()
        {
            TableLayoutPanel shell = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            shell.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    220F));

            shell.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            Panel sidebar = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };

            simulationStartButton = CreateButton(
                "▶ Start",
                currentTheme.AccentBlue,
                Color.White,
                0,
                44,
                true);

            simulationStartButton.Dock = DockStyle.Top;

            simulationStartButton.Click += (s, e) =>
            {
                if (boatMarker != null)
                    waypointOverlay.Markers.Remove(boatMarker);

                boatMarker = null;

                simulationPaused = false;

                simulationPauseButton.Text =
                    "|| Pause";

                simulationPauseButton.Enabled =
                    true;

                simulationClearButton.Enabled =
                    true;
                StartSimulation();
            };

            simulationPauseButton =
    CreateButton(
        "|| Pause",
        currentTheme.AccentBlue,
        Color.White,
        0,
        44,
        true);

            simulationPauseButton.Dock =
                DockStyle.Top;

            simulationPauseButton.Enabled =
                false;
            simulationClearButton =
    CreateButton(
        "Clear",
        currentTheme.AccentBlue,
        Color.White,
        0,
        44,
        true);

            simulationClearButton.Dock =
                DockStyle.Top;

            simulationClearButton.Enabled =
                false;
            UpdateSimulationButtons();

            simulationPauseButton.Click +=
    (s, e) =>
    {
        if (simulationTimer == null)
            return;

        if (!simulationPaused)
        {
            simulationTimer.Stop();

            simulationPaused = true;

            simulationPauseButton.Text =
                "▶ Continue";
        }
        else
        {
            simulationRunning = true;
            UpdateSimulationButtons();

            simulationTimer.Start();
            lblSimStatus.Text = "Running";

            lblSimWaypoint.Text =
                $"1 / {simulationPoints.Count}";

            lblSimProgress.Text = "0%";

            simulationPaused = false;

            simulationPauseButton.Text =
                "|| Pause";
        }
    };


            Panel futureWidgetsArea = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 20, 0, 0)
            };

            lblSimStatus = new Label
            {
                Text = "Status\nIdle",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 60
            };

            lblSimWaypoint = new Label
            {
                Name = "lblSimWaypoint",
                Text = "Waypoint\n0 / 0",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 60
            };

            lblSimProgress = new Label
            {
                Name = "lblSimProgress",
                Text = "Progress\n0%",
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 60
            };

            futureWidgetsArea.Controls.Add(CreateSimulationCard(
    "PROGRESS",
    out lblSimProgress,
    "0%"));

            futureWidgetsArea.Controls.Add(CreateSimulationCard(
                "WAYPOINT",
                out lblSimWaypoint,
                "0 / 0"));

            futureWidgetsArea.Controls.Add(CreateSimulationCard(
                "STATUS",
                out lblSimStatus,
                "Idle"));

            futureWidgetsArea.Controls.Add(CreateSimulationCard(
                "ETA",
                out lblEta,
                "00:00:00"));

            futureWidgetsArea.Controls.Add(CreateSimulationCard(
                "DISTANCE LEFT",
                out lblDistanceLeft,
                "0.00 km"));

            sidebar.Controls.Add(futureWidgetsArea);
            sidebar.Controls.Add(simulationClearButton);
            sidebar.Controls.Add(simulationPauseButton);
            sidebar.Controls.Add(simulationStartButton);
            simMapHost = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

            shell.Controls.Add(sidebar, 0, 0);
            shell.Controls.Add(simMapHost, 1, 0);

            return shell;
        }
        private Panel BuildTargetModePage()
        {
            Panel page = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };

            targetCameraPictureBox =
                new PictureBox
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.Black,
                    SizeMode = PictureBoxSizeMode.Zoom
                };

            targetCameraPictureBox.Paint +=
                TargetCameraPictureBox_Paint;
            targetCameraPictureBox.MouseDoubleClick +=
    TargetCameraPictureBox_MouseDoubleClick;

            Label lbl =
                new Label
                {
                    Text = "TARGET MODE",
                    ForeColor = Color.White,
                    Font = new Font(
                        "Segoe UI",
                        18,
                        FontStyle.Bold),
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    Location = new Point(20, 20)
                };

            btnTestTarget =
                new Button
                {
                    Text = "TEST TARGET",
                    Size = new Size(120, 40),
                    Location = new Point(20, 60)
                };

            btnTestTarget.Click +=
                BtnTestTarget_Click;

            page.Controls.Add(targetCameraPictureBox);
            page.Controls.Add(lbl);
            //page.Controls.Add(btnTestTarget);

            lbl.BringToFront();
            //btnTestTarget.BringToFront();
            btnLockTarget =
    new Button
    {
        Text = "LOCK TARGET",

        Size = new Size(150, 40),

        Anchor =
            AnchorStyles.Top |
            AnchorStyles.Right,

        Location =
            new Point(
                page.Width - 180,
                20),

        BackColor =
            Color.FromArgb(
                41,
                98,
                255),

        ForeColor =
            Color.White,

        FlatStyle =
            FlatStyle.Flat,

        Font =
            new Font(
                "Segoe UI",
                10,
                FontStyle.Bold)
    };

            btnLockTarget.FlatAppearance.BorderSize = 0;

            btnLockTarget.Click +=
                BtnLockTarget_Click;

            page.Controls.Add(btnLockTarget);
            btnUnlockTarget =
    new Button
    {
        Text = "UNLOCK TARGET",

        Size = new Size(150, 40),

        Anchor =
            AnchorStyles.Top |
            AnchorStyles.Right,

        Location =
            new Point(
                page.Width - 180,
                70),

        BackColor =
            Color.FromArgb(
                220,
                53,
                69),

        ForeColor =
            Color.White,

        FlatStyle =
            FlatStyle.Flat,

        Font =
            new Font(
                "Segoe UI",
                10,
                FontStyle.Bold)
    };

            btnUnlockTarget.FlatAppearance.BorderSize = 0;

            btnUnlockTarget.Click +=
                BtnUnlockTarget_Click;

            page.Controls.Add(
                btnUnlockTarget);

            btnUnlockTarget.BringToFront();
            btnClearTargets =
    new Button
    {
        Text = "CLEAR TARGETS",

        Size = new Size(150, 40),

        Anchor =
            AnchorStyles.Top |
            AnchorStyles.Right,

        Location =
            new Point(
                page.Width - 180,
                120),

        BackColor =
            Color.FromArgb(
                108,
                117,
                125),

        ForeColor =
            Color.White,

        FlatStyle =
            FlatStyle.Flat,

        Font =
            new Font(
                "Segoe UI",
                10,
                FontStyle.Bold)
    };

            btnClearTargets.FlatAppearance.BorderSize = 0;

            btnClearTargets.Click +=
                BtnClearTargets_Click;

            page.Controls.Add(
                btnClearTargets);

            btnClearTargets.BringToFront();

            btnLockTarget.BringToFront();

            return page;
        }
        private Control BuildHelpPage()
        {
            Panel page = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = currentTheme.PanelBackground
            };

            TableLayoutPanel shell =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                    Padding = new Padding(8)
                };

            shell.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute,
                    240F));

            shell.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    100F));

            // LEFT NAVIGATION

            SectionPanel navSection =
                CreateSection("Help Topics");

            FlowLayoutPanel navFlow =
                new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection =
                        FlowDirection.TopDown,
                    WrapContents = false,
                    AutoScroll = true
                };

            navFlow.Controls.Add(
                CreateHelpTopicButton(
                    "About",
                    "About"));

            //navFlow.Controls.Add(
            //    CreateHelpTopicButton(
            //        "QuickStart",
            //        "Quick Start"));

            navFlow.Controls.Add(
                CreateHelpTopicButton(
                    "Workflow",
                    "Mission Workflow"));

            navFlow.Controls.Add(
                CreateHelpTopicButton(
                    "Mission",
                    "Mission Page"));

            //navFlow.Controls.Add(
            //    CreateHelpTopicButton(
            //        "Data",
            //        "Data Page"));

            //navFlow.Controls.Add(
            //    CreateHelpTopicButton(
            //        "Simulation",
            //        "Simulation"));

            navFlow.Controls.Add(
                CreateHelpTopicButton(
                    "TargetMode",
                    "Target Mode"));

            //navFlow.Controls.Add(
            //    CreateHelpTopicButton(
            //        "Upload",
            //        "Mission Upload"));

            navFlow.Controls.Add(
                CreateHelpTopicButton(
                    "Troubleshooting",
                    "Troubleshooting"));

            navSection.Content.Controls.Add(
                navFlow);

            // RIGHT CONTENT

            Panel contentPanel =
                new Panel
                {
                    Dock = DockStyle.Fill
                };



            helpTopicHost =
    new Panel
    {
        Dock = DockStyle.Fill,
        Padding = new Padding(
            10,
            10,
            10,
            10),
        AutoScroll = true
    };

            contentPanel.Controls.Add(
                helpTopicHost);



            shell.Controls.Add(
                navSection,
                0,
                0);

            shell.Controls.Add(
                contentPanel,
                1,
                0);

            Control aboutPage = BuildAboutHelpPanel();

            helpTopicPages["About"] = aboutPage;
    //        Panel quickStartPage = new Panel
    //        {
    //            Dock = DockStyle.Fill,
    //            Visible = false,
    //            AutoScroll = true
    //        };
    //        Label quickTitle =
    //CreateLabel(
    //    "Quick Start Guide",
    //    32F,
    //    FontStyle.Bold,
    //    Color.White);

    //        quickTitle.Location =
    //new Point(20, 10);

    //        quickStartPage.Controls.Add(
    //            quickTitle);
    //        Panel quickLine = new Panel
    //        {
    //            Name = "quickLine",
    //            Size = new Size(120, 5),
    //            Location = new Point(20, 70),
    //            BackColor = currentTheme.AccentBlue
    //        };

    //        quickStartPage.Controls.Add(quickLine);
    //        quickLine.BringToFront();

    //        quickStartPage.Controls.Add(
    //            quickLine);
    //        Label quickIntro =
    //CreateLabel(
    //    "Follow these steps to plan, upload and monitor an ASV mission.",
    //    15F,
    //    FontStyle.Regular,
    //    Color.White);

    //        quickIntro.Location =
    //            new Point(20, 105);

    //        quickStartPage.Controls.Add(
    //            quickIntro);
    //        quickStartPage.Controls.Add(
    //CreateHelpFeature(
    //    "1",
    //    "Connect Pixhawk",
    //    "Connect the Pixhawk flight controller and verify communication.",
    //    140));

    //        quickStartPage.Controls.Add(
    //            CreateHelpFeature(
    //                "2",
    //                "Create Mission",
    //                "Build Cruise, Loiter, Burst and Return Cruise stages.",
    //                210));

    //        quickStartPage.Controls.Add(
    //            CreateHelpFeature(
    //                "3",
    //                "Validate Mission",
    //                "Review mission route and verify waypoint placement.",
    //                280));

    //        quickStartPage.Controls.Add(
    //            CreateHelpFeature(
    //                "4",
    //                "Upload Mission",
    //                "Upload the mission to the connected Pixhawk controller.",
    //                350));

    //        quickStartPage.Controls.Add(
    //            CreateHelpFeature(
    //                "5",
    //                "Open Data Page",
    //                "Monitor telemetry, camera feed and mission status.",
    //                420));

    //        quickStartPage.Controls.Add(
    //            CreateHelpFeature(
    //                "6",
    //                "Track Mission Progress",
    //                "Observe Cruise, Loiter, Burst and Return Cruise execution.",
    //                480));
    //        helpTopicPages["QuickStart"] =
    //quickStartPage;

    //        helpTopicHost.Controls.Add(
    //            quickStartPage);



            Panel workflowPage = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false,
                AutoScroll = true
            };
            Label workflowTitle =
    CreateLabel(
        "Mission Workflow",
        32F,
        FontStyle.Bold,
        Color.White);

            workflowTitle.Location =
                new Point(20, 10);

            workflowPage.Controls.Add(
                workflowTitle);
            
            Panel workflowLine = new Panel
            {
                Size = new Size(90, 4),
                Location = new Point(20, 65),
                BackColor = currentTheme.AccentBlue
            };

            workflowPage.Controls.Add(
                workflowLine);
            workflowLine.BringToFront();
            Label workflowIntro =
    CreateLabel(
        "Every ASV mission follows a structured operational sequence.",
        15F,
        FontStyle.Regular,
        Color.White);

            workflowIntro.Location =
                new Point(20, 90);

            workflowIntro.MaximumSize =
                new Size(900, 0);

            workflowPage.Controls.Add(
                workflowIntro);
            Label cruise =
    CreateLabel(
        "CRUISE",
        18F,
        FontStyle.Bold,
        currentTheme.AccentBlue);

            cruise.Location =
                new Point(20, 170);

            workflowPage.Controls.Add(cruise);

            Label arrow1 =
                CreateLabel(
                    "→",
                    22F,
                    FontStyle.Bold,
                    Color.White);

            arrow1.Location =
                new Point(150, 168);

            workflowPage.Controls.Add(arrow1);

            Label loiter =
                CreateLabel(
                    "LOITER",
                    18F,
                    FontStyle.Bold,
                    currentTheme.AccentYellow);

            loiter.Location =
                new Point(210, 170);

            workflowPage.Controls.Add(loiter);

            Label arrow2 =
                CreateLabel(
                    "→",
                    22F,
                    FontStyle.Bold,
                    Color.White);

            arrow2.Location =
                new Point(350, 168);

            workflowPage.Controls.Add(arrow2);

            Label burst =
                CreateLabel(
                    "BURST",
                    18F,
                    FontStyle.Bold,
                    currentTheme.AccentPurple);

            burst.Location =
                new Point(410, 170);

            workflowPage.Controls.Add(burst);

            Label arrow3 =
                CreateLabel(
                    "→",
                    22F,
                    FontStyle.Bold,
                    Color.White);

            arrow3.Location =
                new Point(540, 168);

            workflowPage.Controls.Add(arrow3);

            Label returnCruise =
                CreateLabel(
                    "RETURN CRUISE",
                    18F,
                    FontStyle.Bold,
                    currentTheme.Success);

            returnCruise.Location =
                new Point(600, 170);

            workflowPage.Controls.Add(returnCruise);
            Label workflowDesc =
    CreateLabel(
        "Cruise transports the vehicle to the operational area.\nLoiter performs survey operations.\nBurst provides optional high-priority inspection or sampling.\nReturn Cruise safely guides the vehicle back after mission completion.",
        13F,
        FontStyle.Regular,
        currentTheme.TextPrimary);

            workflowDesc.Location =
                new Point(20, 240);

            workflowDesc.MaximumSize =
                new Size(1000, 0);

            workflowPage.Controls.Add(
                workflowDesc);
            helpTopicPages["Workflow"] =
    workflowPage;

            helpTopicHost.Controls.Add(
                workflowPage);

            Panel missionHelpPage = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false,
                AutoScroll = true
            };
            Label missionTitle =
    CreateLabel(
        "Mission Page Guide",
        32F,
        FontStyle.Bold,
        Color.White);

            missionTitle.Location =
                new Point(20, 10);

            missionHelpPage.Controls.Add(
                missionTitle);
            
            Panel missionLine = new Panel
            {
                Size = new Size(90, 4),
                Location = new Point(20, 65),
                BackColor = currentTheme.AccentBlue
            };

            missionHelpPage.Controls.Add(
                missionLine);
            missionLine.BringToFront();
            Label missionIntro =
    CreateLabel(
        "The Mission Page is used to create, edit and review ASV missions before deployment.",
        15F,
        FontStyle.Regular,
        Color.White);

            missionIntro.Location =
                new Point(20, 85);

            missionIntro.MaximumSize =
                new Size(1000, 0);

            missionHelpPage.Controls.Add(
                missionIntro);
            missionHelpPage.Controls.Add(
    CreateHelpFeature(
        "🗺",
        "Mission Map",
        "Visualize waypoints, routes and survey areas on the mission map.",
        140));

            missionHelpPage.Controls.Add(
                CreateHelpFeature(
                    "📍",
                    "Cruise Waypoints",
                    "Create navigation waypoints used to move the ASV between locations.",
                    210));

            missionHelpPage.Controls.Add(
                CreateHelpFeature(
                    "🟨",
                    "Loiter Survey Area",
                    "Define survey operations within the selected region.",
                    280));

            missionHelpPage.Controls.Add(
                CreateHelpFeature(
                    "🟪",
                    "Burst Operations",
                    "Optional inspection or sampling stage for high-priority tasks.",
                    350));

            missionHelpPage.Controls.Add(
                CreateHelpFeature(
                    "🟩",
                    "Return Cruise",
                    "Guide the vehicle safely back after mission completion.",
                    420));

            missionHelpPage.Controls.Add(
                CreateHelpFeature(
                    "⬆",
                    "Mission Upload",
                    "Transfer the completed mission to the connected Pixhawk controller.",
                    490));
//            Label uploadWorkflowTitle =
//CreateLabel(
//    "Upload & Validation Workflow",
//    18F,
//    FontStyle.Bold,
//    currentTheme.TextPrimary);

//            uploadWorkflowTitle.Location =
//                new Point(20, 500);

//            missionHelpPage.Controls.Add(
//                uploadWorkflowTitle);

//            Label uploadWorkflow =
//            CreateLabel(
//                "Validate Mission  →  Connect Vehicle  →  Upload Mission  →  Verify Upload  →  Deploy",
//                14F,
//                FontStyle.Bold,
//                currentTheme.AccentBlue);

//            uploadWorkflow.Location =
//                new Point(20, 540);

//            uploadWorkflow.MaximumSize =
//                new Size(950, 0);

//            missionHelpPage.Controls.Add(
//                uploadWorkflow);
            //        missionHelpPage.Controls.Add(
            //CreateHelpFeature(
            //    "✅",
            //    "Mission Validation",
            //    "Validate all waypoints and mission stages before upload.",
            //    560));

            //        missionHelpPage.Controls.Add(
            //            CreateHelpFeature(
            //                "🔌",
            //                "Vehicle Connection",
            //                "Connect and verify Pixhawk communication before mission upload.",
            //                630));

            //        missionHelpPage.Controls.Add(
            //            CreateHelpFeature(
            //                "📋",
            //                "Upload Verification",
            //                "Verify uploaded mission data by reading back mission waypoints.",
            //                700));

            //        missionHelpPage.Controls.Add(
            //            CreateHelpFeature(
            //                "🚀",
            //                "Mission Deployment",
            //                "After successful upload and verification, the vehicle is ready for execution.",
            //                770));

            helpTopicPages["Mission"] =
    missionHelpPage;

            helpTopicHost.Controls.Add(
                missionHelpPage);

            helpTopicHost.Controls.Add(aboutPage);

            page.Controls.Add(shell);

    //        Panel dataPage = new Panel
    //        {
    //            Dock = DockStyle.Fill,
    //            Visible = false
    //        };

    //        Label dataTitle =
    //CreateLabel(
    //    "Data Page Guide",
    //    30F,
    //    FontStyle.Bold,
    //    Color.White);

    //        dataTitle.Location =
    //            new Point(20, 10);

    //        dataPage.Controls.Add(
    //            dataTitle);
    //        Panel dataLine =
    //new Panel
    //{
    //    Size = new Size(80, 4),
    //    Location = new Point(20, 70),
    //    BackColor = currentTheme.AccentBlue
    //};

    //        dataPage.Controls.Add(
    //            dataLine);

    //        Label dataIntro =
    //CreateLabel(
    //    "The Data Page provides real-time monitoring of vehicle status, telemetry, mission progress and camera feeds.",
    //    16F,
    //    FontStyle.Regular,
    //    Color.White);


    //        dataIntro.Location =
    //            new Point(20, 95);

    //        dataPage.Controls.Add(
    //            dataIntro);

    //        dataPage.Controls.Add(
    //CreateHelpFeature(
    //    "📡",
    //    "Telemetry Monitoring",
    //    "View live navigation, GPS, battery and system telemetry.",
    //    150));

    //        dataPage.Controls.Add(
    //            CreateHelpFeature(
    //                "📷",
    //                "Camera Feed",
    //                "Monitor onboard RTSP camera streams in real time.",
    //                220));

    //        dataPage.Controls.Add(
    //            CreateHelpFeature(
    //                "🗺",
    //                "Mission Map",
    //                "Track vehicle position and mission progress on the map.",
    //                290));

    //        dataPage.Controls.Add(
    //            CreateHelpFeature(
    //                "🔵",
    //                "CLBR Mission Indicator",
    //                "Displays the currently active mission stage during execution.",
    //                360));

    //        dataPage.Controls.Add(
    //            CreateHelpFeature(
    //                "📍",
    //                "Vehicle Tracking",
    //                "Observe heading, location and movement throughout the mission.",
    //                430));

    //        dataPage.Controls.Add(
    //            CreateHelpFeature(
    //                "📊",
    //                "Mission Status",
    //                "Monitor progress, connectivity and mission completion status.",
    //                500));

    //        Label clbrTitle =
    //CreateLabel(
    //    "Mission Stage Indicator",
    //    18F,
    //    FontStyle.Bold,
    //    Color.White);

    //        clbrTitle.Location =
    //            new Point(20, 580);

    //        dataPage.Controls.Add(
    //            clbrTitle);
    //        Label clbrLegend =
    //CreateLabel(
    //    "🔵 Cruise     🟡 Loiter     🟣 Burst     🟢 Return Cruise",
    //    15F,
    //    FontStyle.Bold,
    //    Color.White);

    //        clbrLegend.Location =
    //            new Point(20, 615);

    //        dataPage.Controls.Add(
    //            clbrLegend);

            //helpTopicPages["Data"] = dataPage;
            //helpTopicHost.Controls.Add(dataPage);
            //Panel simulationPage = new Panel
            //{
            //    Dock = DockStyle.Fill,
            //    Visible = false
            //};

            //simulationPage.Controls.Add(
            //    CreateLabel(
            //        "Simulation Guide",
            //        28F,
            //        FontStyle.Bold,
            //        Color.White));

            //helpTopicPages["Simulation"] = simulationPage;
            //helpTopicHost.Controls.Add(simulationPage);
            Panel targetModePage = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false,
                AutoScroll = true
            };

            Label targetTitle =
            CreateLabel(
                "Target Mode Guide",
                30F,
                FontStyle.Bold,
                Color.White);

            targetTitle.Location =
                new Point(20, 10);

            targetModePage.Controls.Add(
                targetTitle);

            Panel targetLine =
            new Panel
            {
                Size = new Size(80, 4),
                Location = new Point(20, 65),
                BackColor = currentTheme.AccentBlue
            };

            targetModePage.Controls.Add(
                targetLine);

            Label targetIntro =
            CreateLabel(
                "Target Mode enables operators to identify, select, lock and manage detected targets using onboard camera feeds.",
                14F,
                FontStyle.Regular,
                Color.White);

            targetIntro.Location =
                new Point(20, 90);

            targetIntro.MaximumSize =
                new Size(950, 0);

            targetModePage.Controls.Add(
                targetIntro);

            targetModePage.Controls.Add(
            CreateHelpFeature(
                "🎯",
                "Target Detection",
                "Detect and visualize targets received through the target tracking system.",
                150));

            targetModePage.Controls.Add(
            CreateHelpFeature(
                "🖱",
                "Target Selection",
                "Double-click a target box to select it for further operations.",
                220));

            targetModePage.Controls.Add(
            CreateHelpFeature(
                "🔒",
                "Lock Target",
                "Send target coordinates and initiate lock operation.",
                290));

            targetModePage.Controls.Add(
            CreateHelpFeature(
                "🔓",
                "Unlock Target",
                "Release an active target lock.",
                360));

            targetModePage.Controls.Add(
            CreateHelpFeature(
                "🧹",
                "Clear Targets",
                "Remove all detected targets from the display.",
                430));

            targetModePage.Controls.Add(
            CreateHelpFeature(
                "📷",
                "Live Camera Feed",
                "Monitor live video while performing target operations.",
                500));

            helpTopicPages["TargetMode"] =
                targetModePage;

            helpTopicHost.Controls.Add(
                targetModePage);
    //        Panel uploadPage = new Panel
    //        {
    //            Dock = DockStyle.Fill,
    //            Visible = false
    //        };
    //        Label uploadTitle =
    //CreateLabel(
    //    "Mission Upload Guide",
    //    30F,
    //    FontStyle.Bold,
    //    Color.White);

    //        uploadTitle.Location =
    //            new Point(20, 10);

    //        uploadPage.Controls.Add(
    //            uploadTitle);
    //        Panel uploadLine =
    //new Panel
    //{
    //    Size = new Size(80, 4),
    //    Location = new Point(20, 65),
    //    BackColor = currentTheme.AccentBlue
    //};

    //        uploadPage.Controls.Add(
    //            uploadLine);
    //        Label uploadIntro =
    //CreateLabel(
    //    "The Mission Upload page transfers validated mission plans from the Ground Control System to the Pixhawk flight controller.",
    //    14F,
    //    FontStyle.Regular,
    //    Color.White);

    //        uploadIntro.Location =
    //            new Point(20, 90);

    //        uploadIntro.MaximumSize =
    //            new Size(950, 0);

    //        uploadPage.Controls.Add(
    //            uploadIntro);
    //        uploadPage.Controls.Add(
    //CreateHelpFeature(
    //    "✅",
    //    "Mission Validation",
    //    "Verify mission stages and waypoint configuration before upload.",
    //    150));
    //        uploadPage.Controls.Add(
    //CreateHelpFeature(
    //    "🔌",
    //    "Vehicle Connection",
    //    "Connect the ASV system to the Pixhawk flight controller.",
    //    220));

    //        uploadPage.Controls.Add(
    //            CreateHelpFeature(
    //                "⬆",
    //                "Mission Upload",
    //                "Transfer mission data from the Ground Control System to the vehicle.",
    //                290));

    //        uploadPage.Controls.Add(
    //            CreateHelpFeature(
    //                "📋",
    //                "Upload Status",
    //                "Monitor upload progress and confirmation messages.",
    //                360));

    //        uploadPage.Controls.Add(
    //            CreateHelpFeature(
    //                "🔄",
    //                "Mission Verification",
    //                "Confirm uploaded mission data matches the planned mission.",
    //                430));

    //        uploadPage.Controls.Add(
    //            CreateHelpFeature(
    //                "🚀",
    //                "Ready For Deployment",
    //                "Vehicle is ready for mission execution after successful upload.",
    //                500));
    //        Label uploadWorkflowTitle =
    //CreateLabel(
    //    "Recommended Upload Workflow",
    //    18F,
    //    FontStyle.Bold,
    //    Color.White);

    //        uploadWorkflowTitle.Location =
    //            new Point(20, 590);

    //        uploadPage.Controls.Add(
    //            uploadWorkflowTitle);
    //        Label uploadWorkflow =
    //CreateLabel(
    //    "Validate Mission  →  Connect Vehicle  →  Upload Mission  →  Verify Upload  →  Deploy",
    //    15F,
    //    FontStyle.Bold,
    //    Color.White);

    //        uploadWorkflow.Location =
    //            new Point(20, 625);

    //        uploadPage.Controls.Add(
    //            uploadWorkflow);

    //        helpTopicPages["Upload"] = uploadPage;
    //        helpTopicHost.Controls.Add(uploadPage);
            Panel troubleshootingPage = new Panel
            {
                Dock = DockStyle.Fill,
                Visible = false,
                AutoScroll = true
            };

            Label troubleTitle =
                CreateLabel(
                    "Troubleshooting",
                    30F,
                    FontStyle.Bold,
                    Color.White);

            troubleTitle.Location =
                new Point(20, 10);

            troubleshootingPage.Controls.Add(
                troubleTitle);

            Panel troubleLine =
                new Panel
                {
                    Size = new Size(80, 4),
                    Location = new Point(20, 65),
                    BackColor = currentTheme.AccentBlue
                };

            troubleshootingPage.Controls.Add(
                troubleLine);

            Label troubleIntro =
                CreateLabel(
                    "Common issues and recommended corrective actions.",
                    14F,
                    FontStyle.Regular,
                    Color.White);

            troubleIntro.Location =
                new Point(20, 90);

            troubleshootingPage.Controls.Add(
                troubleIntro);
            troubleshootingPage.Controls.Add(
    CreateHelpFeature(
        "🔌",
        "Vehicle Connection Failed",
        "Verify Pixhawk connection, COM port selection and cable integrity.",
        150));

            troubleshootingPage.Controls.Add(
                CreateHelpFeature(
                    "📡",
                    "No Telemetry Data",
                    "Ensure telemetry radio or serial link is active and connected.",
                    220));

            troubleshootingPage.Controls.Add(
                CreateHelpFeature(
                    "🛰",
                    "GPS Not Available",
                    "Move vehicle to open sky and wait for satellite lock.",
                    290));

            troubleshootingPage.Controls.Add(
                CreateHelpFeature(
                    "📷",
                    "Camera Feed Not Displayed",
                    "Check RTSP URL, network connectivity or camera power status.",
                    360));

            troubleshootingPage.Controls.Add(
                CreateHelpFeature(
                    "⬆",
                    "Mission Upload Failed",
                    "Validate mission and reconnect to the flight controller before retrying.",
                    430));

            troubleshootingPage.Controls.Add(
                CreateHelpFeature(
                    "⚠",
                    "Simulation Not Running",
                    "Confirm a mission is loaded and press Start Simulation.",
                    500));
            troubleshootingPage.Controls.Add(
    CreateHelpFeature(
        "🗺",
        "Map Not Loading",
        "Check internet connectivity and map provider availability.",
        570));

            troubleshootingPage.Controls.Add(
                CreateHelpFeature(
                    "📊",
                    "Telemetry Not Updating",
                    "Verify MAVLink communication and Pixhawk connection.",
                    640));

            troubleshootingPage.Controls.Add(
                CreateHelpFeature(
                    "🔒",
                    "Vehicle Not Arming",
                    "Check GPS lock, battery level and safety conditions.",
                    710));

            troubleshootingPage.Controls.Add(
                CreateHelpFeature(
                    "📷",
                    "Camera Feed Lost",
                    "Verify RTSP stream address and network connectivity.",
                    780));
            helpTopicPages["Troubleshooting"] =
    troubleshootingPage;

            helpTopicHost.Controls.Add(
                troubleshootingPage);

            SwitchHelpTopic("About");
            return page;
        }
        private void BtnClearTargets_Click(
    object sender,
    EventArgs e)
        {
            targets.Clear();

            selectedTarget = null;

            btnLockTarget.Text =
                "LOCK TARGET";

            btnLockTarget.BackColor =
                Color.FromArgb(
                    41,
                    98,
                    255);

            targetCameraPictureBox.Invalidate();
        }
        private Button CreateHelpTopicButton(
    string key,
    string text)
        {
            Button button =
                CreateButton(
                    text,
                    currentTheme.PanelAlt,
                    currentTheme.TextPrimary,
                    200,
                    32,
                    false);

            button.TextAlign =
                ContentAlignment.MiddleLeft;

            button.Click +=
    (s, e) =>
    {
        SwitchHelpTopic(key);
    };

            helpTopicButtons[key] = button;

            return button;
        }
        private Panel BuildAboutHelpPanel()
        {
            Panel panel =
    new Panel
    {
        Dock = DockStyle.Fill,
        BackColor = Color.Transparent
    };

            Label title =
                CreateLabel(
    "About Vikra ASV Ground Control System",
    30F,
    FontStyle.Bold,
    Color.White);

            title.Location =
    new Point(20, 0);
            title.MaximumSize =
    new Size(950, 0);

            Label intro =
    CreateLabel(
        "The Vikra ASV Ground Control System is a mission planning and monitoring application for Autonomous Surface Vehicles (ASVs).",
        16F,
        FontStyle.Regular,
        Color.White);

            intro.Location =
    new Point(20, 90);

            intro.MaximumSize =
    new Size(900, 0);

            panel.Controls.Add(title);
            Panel accentLine = new Panel
            {
                Size = new Size(80, 4),
                Location = new Point(20, 60),
                BackColor = currentTheme.AccentBlue
            };

            panel.Controls.Add(accentLine);
            panel.Controls.Add(intro);

            panel.Controls.Add(
    CreateHelpFeature(
        "📋",
        "Create and Manage Missions",
        "Design, organize and edit missions with multiple stages and waypoints.",
        170));

            panel.Controls.Add(
                CreateHelpFeature(
                    "🚀",
                    "Upload Missions to Pixhawk",
                    "Connect to your vehicle and upload missions securely to the flight controller.",
                    225));

            panel.Controls.Add(
                CreateHelpFeature(
                    "📡",
                    "Monitor Vehicle Telemetry",
                    "View real-time telemetry including navigation, GPS, battery and system health.",
                    280));

            panel.Controls.Add(
                CreateHelpFeature(
                    "📍",
                    "Track Mission Progress",
                    "Monitor mission execution and stage completion in real time.",
                    335));

            panel.Controls.Add(
                CreateHelpFeature(
                    "🖥",
                    "Run Mission Simulations",
                    "Validate mission routes and behavior before deployment.",
                    390));

            panel.Controls.Add(
                CreateHelpFeature(
                    "📷",
                    "Monitor Camera Feeds",
                    "View onboard camera streams and inspection footage during operations.",
                    445));

            panel.Controls.Add(
                CreateHelpFeature(
                    "⚗",
                    "Support Scientific Survey Operations",
                    "Built for marine research, bathymetry, inspection and environmental monitoring missions.",
                    500));

            return panel;
        }
        private void SwitchHelpTopic(string key)
        {
            foreach (Control page in helpTopicPages.Values)
            {
                page.Visible = false;
            }

            if (helpTopicPages.ContainsKey(key))
            {
                helpTopicPages[key].Visible = true;
            }
        }
        private Panel CreateHelpFeature(
    string icon,
    string titleText,
    string descriptionText,
    int y)
        {
            Panel feature =
                new Panel
                {
                    Location = new Point(40, y),
                    Size = new Size(950, 50),
                    BackColor = Color.Transparent
                };
            Label iconLabel =
    CreateLabel(
        icon,
        28F,
        FontStyle.Regular,
        currentTheme.AccentBlue);

            iconLabel.Location =
                new Point(0, 10);

            feature.Controls.Add(iconLabel);

            Label title =
                CreateLabel(
                    titleText,
                    16F,
                    FontStyle.Bold,
                    Color.White);

            title.Location =
                new Point(70, 0);

            Label desc =
                CreateLabel(
                    descriptionText,
                    12F,
                    FontStyle.Regular,
                    currentTheme.TextSecondary);

            desc.Location =
                new Point(70, 28);

            desc.MaximumSize =
                new Size(850, 0);

            feature.Controls.Add(title);
            feature.Controls.Add(desc);

            return feature;
        }
        private async void BtnUnlockTarget_Click(
    object sender,
    EventArgs e)
        {
            if (lastSenderEndpoint == null)
            {
                MessageBox.Show(
                    "No sender endpoint available.");

                return;
            }

            try
            {
                byte[] data =
                    Encoding.UTF8.GetBytes(
                        "UNLOCK");

                using (UdpClient udp =
                    new UdpClient())
                {
                    await udp.SendAsync(
                        data,
                        data.Length,
                        lastSenderEndpoint);
                }

                selectedTarget = null;

                btnLockTarget.Text =
                    "LOCK TARGET";

                btnLockTarget.BackColor =
                    Color.FromArgb(
                        41,
                        98,
                        255);

                targetCameraPictureBox.Invalidate();

                MessageBox.Show(
                    "Target Unlocked");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Unlock Error");
            }
        }
        private void TargetCameraPictureBox_Paint(
    object sender,
    PaintEventArgs e)
        {
            foreach (var target in targets)
            {
                Color boxColor = Color.Red;

                if (selectedTarget == target)
                {
                    boxColor = Color.Lime;
                }

                using (Pen pen =
                    new Pen(boxColor, 3))
                {
                    e.Graphics.DrawRectangle(
                        pen,
                        target.PixelX,
                        target.PixelY,
                        target.Width,
                        target.Height);
                }
            }
        }
        private void TargetCameraPictureBox_MouseDoubleClick(
    object sender,
    MouseEventArgs e)
        {
            foreach (var target in targets)
            {
                Rectangle rect =
                    new Rectangle(
                        target.PixelX,
                        target.PixelY,
                        target.Width,
                        target.Height);

                if (rect.Contains(e.Location))
                {
                    selectedTarget = target;

                    btnLockTarget.Text = "LOCK TARGET";

                    btnLockTarget.BackColor =
                        Color.FromArgb(41, 98, 255);

                    targetCameraPictureBox.Invalidate();

                    MessageBox.Show(
                        $"Target Selected\n\n" +
                        $"Lat: {target.XCoordinate}\n" +
                        $"Lon: {target.YCoordinate}");

                    break;
                }
            }
        }
        private int testTargetCounter = 0;

        private void BtnTestTarget_Click(
            object sender,
            EventArgs e)
        {
            targets.Add(
                new TargetData
                {
                    PixelX = 100 + (testTargetCounter * 250),
                    PixelY = 100 + (testTargetCounter * 50),

                    XCoordinate = 235 + testTargetCounter,
                    YCoordinate = 412 + testTargetCounter,

                    Width = 200,
                    Height = 150
                });

            testTargetCounter++;

            targetCameraPictureBox.Invalidate();
        }
        private async void BtnLockTarget_Click(
    object sender,
    EventArgs e)
        {
            if (selectedTarget == null)
            {
                MessageBox.Show(
                    "Please select a target first.");

                return;
            }

            if (lastSenderEndpoint == null)
            {
                MessageBox.Show(
                    "No sender endpoint available.");

                return;
            }

            try
            {
                btnLockTarget.Text =
                    "LOCKING TARGET";

                btnLockTarget.BackColor =
                    Color.DarkOrange;

                string message =
                    $"{selectedTarget.XCoordinate}," +
                    $"{selectedTarget.YCoordinate}";


                byte[] data =
                    Encoding.UTF8.GetBytes(
                        message);

                using (UdpClient udp =
                    new UdpClient())
                {
                    await udp.SendAsync(
                        data,
                        data.Length,
                        lastSenderEndpoint);
                }

                btnLockTarget.Text =
                    "TARGET LOCKED";

                btnLockTarget.BackColor =
                    Color.ForestGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lock Target Error");

                btnLockTarget.Text =
                    "LOCK TARGET";

                btnLockTarget.BackColor =
                    Color.FromArgb(
                        41,
                        98,
                        255);
            }
        }
        private async void StartTargetReceiver()
        {
            try
            {
                targetUdp =
                    new System.Net.Sockets.UdpClient(5005);

                while (true)
                {
                    var result =
                        await targetUdp.ReceiveAsync();
                    lastSenderEndpoint =
    result.RemoteEndPoint;

                    string message =
                        System.Text.Encoding.UTF8.GetString(
                            result.Buffer);
                    //MessageBox.Show(message);

                    string[] parts =
                        message.Split(',');

                    if (parts.Length < 6)
                        continue;

                    TargetData target =
    new TargetData
    {
        PixelX = int.Parse(parts[0]),
        PixelY = int.Parse(parts[1]),
        XCoordinate =
    int.Parse(parts[2]),

        YCoordinate =
    int.Parse(parts[3]),
        Width = int.Parse(parts[4]),
        Height = int.Parse(parts[5])
    };

                    targets.Add(target);

                    //MessageBox.Show(
                    //$"X={target.PixelX}\n" +
                    //$"Y={target.PixelY}\n" +
                    //$"W={target.Width}\n" +
                    //$"H={target.Height}");//

                    if (targetCameraPictureBox != null)
                    {
                        targetCameraPictureBox.Invoke(
                            new Action(() =>
                            {
                                targetCameraPictureBox.Invalidate();
                            }));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "UDP Error");
            }
        }

        private Control BuildDataTelemetrySidebar()
        {
            //MessageBox.Show("BuildDataTelemetrySidebar");
            SectionPanel sidebar = CreateSection("Data Telemetry");
            sidebar.Content.Padding = new Padding(10, 8, 10, 8);

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 240F)); // HUD
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Camera
            hud1 = new MissionPlanner.Controls.HUD
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            layout.Controls.Add(hud1, 0, 0);
            hud1.heading = 90;
            hud1.roll = 0;
            hud1.pitch = 0;
            hud1.groundspeed = 4.2f;
            hud1.batteryremaining = 78;
            hud1.batterylevel = 24.6f;
            hud1.mode = "AUTO";
            hud1.connected = true;

            TableLayoutPanel tabs = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                ColumnCount = 6,
                RowCount = 1
            };
            for (int column = 0; column < 6; column++)
            {
                tabs.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6667F));
            }
            tabs.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            tabs.Controls.Add(CreateDataSidebarTab("Quick"), 0, 0);
            tabs.Controls.Add(CreateDataSidebarTab("Navigation"), 1, 0);
            tabs.Controls.Add(CreateDataSidebarTab("Mission"), 2, 0);
            tabs.Controls.Add(CreateDataSidebarTab("Power"), 3, 0);
            tabs.Controls.Add(CreateDataSidebarTab("GPS"), 4, 0);
            tabs.Controls.Add(CreateDataSidebarTab("System"), 5, 0);

            dataTabHost = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = Padding.Empty
            };

            //hud1 = new HUD
            //{
            //Dock = DockStyle.Fill,
            //BackColor = Color.Black,
            //VSync = false,

            //heading = 90,
            //roll = 0,
            //pitch = 0,
            //groundspeed = 4.2f,

            //batteryremaining = 78,
            //batterylevel = 24.6f,

            //mode = "AUTO",
            //connected = true
            //};

            dataTabPanels["Quick"] = CreateQuickTelemetryTab();
            dataTabPanels["Navigation"] = CreateTelemetryConsoleTab(new[]
            {
                Tuple.Create("NavHeading", "086", "Heading"),
                Tuple.Create("NavCog", "091", "COG"),
                Tuple.Create("NavSog", "4.2", "SOG"),
                Tuple.Create("NavBearing", "102", "Bearing"),
                Tuple.Create("NavLat", "13.07", "Latitude"),
                Tuple.Create("NavLon", "80.27", "Longitude")
            });
            dataTabPanels["Mission"] = CreateTelemetryConsoleTab(new[]
            {
                Tuple.Create("MissionStage", "CRUISE", "Stage"),
                Tuple.Create("MissionWp", "C2", "Waypoint"),
                Tuple.Create("MissionDist", "184m", "Dist to WP"),
                Tuple.Create("MissionEta", "12:40", "ETA"),
                Tuple.Create("MissionProg", "38%", "Progress"),
                Tuple.Create("MissionState", "RUN", "Status")
            });
            dataTabPanels["Power"] = CreateTelemetryConsoleTab(new[]
            {
                Tuple.Create("PowerRemain", "78%", "Battery"),
                Tuple.Create("PowerVolt", "24.6V", "Voltage"),
                Tuple.Create("PowerAmp", "8.4A", "Current"),
                Tuple.Create("PowerLoad", "206W", "Power"),
                Tuple.Create("PowerTemp", "31C", "Temp"),
                Tuple.Create("PowerHealth", "GOOD", "Health")
            });
            dataTabPanels["GPS"] = CreateTelemetryConsoleTab(new[]
            {
                Tuple.Create("GpsFixTab", "3D", "GPS Fix"),
                Tuple.Create("GpsSatTab", "16", "Sat Count"),
                Tuple.Create("GpsHdopTab", "0.9", "HDOP"),
                Tuple.Create("GpsCourseTab", "091", "Course"),
                Tuple.Create("GpsSpeedTab", "4.2", "Speed"),
                Tuple.Create("GpsHomeTab", "LOCK", "Home")
            });
            dataTabPanels["System"] = CreateTelemetryConsoleTab(new[]
            {
                Tuple.Create("SysModeTab", "AUTO", "Mode"),
                Tuple.Create("SysArmedTab", "SAFE", "Armed"),
                Tuple.Create("SysHeartTab", "OK", "Heartbeat"),
                Tuple.Create("SysFwTab", "v2.4.1", "Firmware"),
                Tuple.Create("SysUpTab", "01:43", "Uptime"),
                Tuple.Create("SysStateTab", "NOM", "System")
            });

            foreach (Panel panel in dataTabPanels.Values)
            {
                panel.Visible = false;
                dataTabHost.Controls.Add(panel);
            }

            //layout.Controls.Add(hud1, 0, 0);
            //layout.Controls.Add(hud1, 0, 0);
            //layout.Controls.Add(tabs, 0, 1);
            //layout.Controls.Add(dataTabHost, 0, 2);
            secondaryViewportHost =
    new Panel
    {
        Dock = DockStyle.Fill,
        BackColor = Color.Black
    };
            btnSwap = new Button
            {
                Text = "⛶",
                Width = 36,
                Height = 36,

                FlatStyle = FlatStyle.Flat,

                BackColor =
        Color.FromArgb(
            20,
            25,
            40),

                ForeColor = Color.White,

                Anchor =
        AnchorStyles.Top |
        AnchorStyles.Right
            };

            btnSwap.FlatAppearance.BorderSize = 0;

            btnSwap.Click +=
                (s, e) => SwapViewports();

            secondaryViewportHost.Controls.Add(
                btnSwap);

            btnSwap.BringToFront();

            secondaryViewportHost.Resize +=
                (s, e) =>
                {
                    btnSwap.Location =
                        new Point(
                            secondaryViewportHost.Width - 42,
                            6);
                };

            btnSwap.Location =
                new Point(
                    secondaryViewportHost.Width - 42,
                    6);

            cameraPlaceholderPanel.Dock =
                DockStyle.Fill;

            secondaryViewportHost.Controls.Add(
                cameraPlaceholderPanel);

            layout.Controls.Add(
                secondaryViewportHost,
                0,
                1);

            sidebar.Content.Controls.Add(layout);


            Panel wrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 8, 0) };
            wrapper.Controls.Add(sidebar);

            SwitchDataTab("Quick");
            return wrapper;
        }

        private void BuildTelemetryStrip()
        {
            // Four independent floating cards directly on the map host.
            Control cardRoll = CreateTelemetryStripCard(
                "RollValue",
                "ROLL",
                "0°",
                currentTheme.AccentBlue);

            Control cardPitch = CreateTelemetryStripCard(
                "PitchValue",
                "PITCH",
                "0°",
                currentTheme.AccentYellow);

            Control cardYaw = CreateTelemetryStripCard(
                "YawValue",
                "YAW",
                "0°",
                currentTheme.Success);

            Control cardSpeed = CreateTelemetryStripCard(
                "SpeedValue",
                "SPEED",
                "0 kn",
                currentTheme.AccentPurple);

            dataMapHost.Controls.Add(cardRoll);
            dataMapHost.Controls.Add(cardPitch);
            dataMapHost.Controls.Add(cardYaw);
            dataMapHost.Controls.Add(cardSpeed);

            void LayoutCards()
            {
                int cardW = 130;
                int cardH = 90;
                int gap = 95;

                int totalW = (cardW * 4) + (gap * 3);
                int x0 = Math.Max(20, (dataMapHost.Width - totalW) / 2);
                int y = dataMapHost.Height - cardH - 35;

                cardRoll.Bounds = new Rectangle(x0, y, cardW, cardH);
                cardPitch.Bounds = new Rectangle(x0 + cardW + gap, y, cardW, cardH);
                cardYaw.Bounds = new Rectangle(x0 + (cardW + gap) * 2, y, cardW, cardH);
                cardSpeed.Bounds = new Rectangle(x0 + (cardW + gap) * 3, y, cardW, cardH);

                cardRoll.BringToFront();
                cardPitch.BringToFront();
                cardYaw.BringToFront();
                cardSpeed.BringToFront();
            }

            dataMapHost.SizeChanged += (s, e) => LayoutCards();
            LayoutCards();
        }
        //private Panel BuildTelemetryStrip()
        //{
        //    telemetryStripPanel = new Panel
        //    {
        //        Width = 900,
        //        Height = 120,
        //        BackColor = Color.Transparent,
        //        Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        //    };

        //    telemetryStripPanel.BorderStyle = BorderStyle.None;

        //    Control cardRoll =
        //        CreateTelemetryStripCard(
        //            "RollValue",
        //            "ROLL",
        //            "0°",
        //            currentTheme.AccentBlue);

        //    Control cardPitch =
        //        CreateTelemetryStripCard(
        //            "PitchValue",
        //            "PITCH",
        //            "0°",
        //            currentTheme.AccentYellow);

        //    Control cardYaw =
        //        CreateTelemetryStripCard(
        //            "YawValue",
        //            "YAW",
        //            "0°",
        //            currentTheme.Success);

        //    Control cardSpeed =
        //        CreateTelemetryStripCard(
        //            "SpeedValue",
        //            "SPEED",
        //            "0 kn",
        //            currentTheme.AccentPurple);

        //    // EXACTLY like your sketch

        //    cardRoll.Location =
        //        new Point(20, 10);

        //    cardPitch.Location =
        //        new Point(240, 10);

        //    cardYaw.Location =
        //        new Point(460, 10);

        //    cardSpeed.Location =
        //        new Point(680, 10);

        //    telemetryStripPanel.Controls.Add(cardRoll);
        //    telemetryStripPanel.Controls.Add(cardPitch);
        //    telemetryStripPanel.Controls.Add(cardYaw);
        //    telemetryStripPanel.Controls.Add(cardSpeed);

        //    return telemetryStripPanel;
        //}

        private Control CreateTelemetryStripCard(
string key,
string captionText,
string valueText,
Color valueColor)
        {
            RoundedPanel card = new RoundedPanel
            {
                Theme = currentTheme,
                FillColor = currentTheme.PanelAlt,
                Radius = 16,
                Width = 130,
                Height = 95,
                Margin = new Padding(0)
            };

            themeAwareControls.Add(card);

            Label valueLabel = CreateLabel(
                valueText,
                20F,
                FontStyle.Bold,
                valueColor);

            valueLabel.Dock = DockStyle.Top;
            valueLabel.Height = 50;
            valueLabel.AutoSize = false;
            valueLabel.TextAlign = ContentAlignment.MiddleCenter;

            dataValueLabels[key] = valueLabel;

            Label captionLabel = CreateLabel(
                captionText,
                10F,
                FontStyle.Regular,
                currentTheme.TextMuted);

            captionLabel.Dock = DockStyle.Fill;
            captionLabel.AutoSize = false;
            captionLabel.TextAlign = ContentAlignment.TopCenter;
            captionLabel.Padding =
    new Padding(0, 4, 0, 0);

            card.Controls.Add(captionLabel);
            card.Controls.Add(valueLabel);
            valueLabel.BackColor = Color.Transparent;
            captionLabel.BackColor = Color.Transparent;

            return card;
        }
        //    private Control CreateTelemetryStripCard(
        //string key,
        //string captionText,
        //string valueText,
        //Color valueColor)
        //    {
        //        RoundedPanel card = new RoundedPanel
        //        {
        //            Theme = currentTheme,
        //            FillColor = currentTheme.PanelAlt,
        //            Radius = 12,
        //            Width = 130,
        //            Height = 90,
        //            Margin = new Padding(2)
        //        };

        //        themeAwareControls.Add(card);

        //        Label valueLabel =
        //            CreateLabel(
        //                valueText,
        //                20F,
        //                FontStyle.Bold,
        //                valueColor);

        //        valueLabel.Dock = DockStyle.Top;
        //        valueLabel.Height = 40;
        //        valueLabel.AutoSize = false;
        //        valueLabel.TextAlign =
        //            ContentAlignment.BottomCenter;

        //        dataValueLabels[key] = valueLabel;

        //        Label captionLabel =
        //            CreateLabel(
        //                captionText,
        //                10F,
        //                FontStyle.Regular,
        //                currentTheme.TextMuted);

        //        captionLabel.Dock = DockStyle.Fill;
        //        captionLabel.AutoSize = false;
        //        captionLabel.TextAlign =
        //            ContentAlignment.TopCenter;


        //        card.Controls.Add(captionLabel);
        //        card.Controls.Add(valueLabel);

        //        return card;
        //    }
        private Control CreateTelemetryGroupCard(
    string key,
    string groupName,
    string valueText,
    Color valueColor,
    string[] metrics)
        {
            RoundedPanel card = new RoundedPanel
            {
                Theme = currentTheme,
                FillColor = Color.FromArgb(
    18,
    28,
    48),
                //FillColor = currentTheme.PanelAlt,
                Radius = 8,
                Width = 135,
                Height = 90,
                Margin = new Padding(12)
            };

            ComboBox cmbMetric = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 24,
                Margin = new Padding(2),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cmbMetric.Items.AddRange(metrics);
            cmbMetric.SelectedIndex = 0;

            telemetryCardDropdowns[key] = cmbMetric;

            Label valueLabel =
    CreateLabel(
        valueText,
        20F,
        FontStyle.Bold,
        valueColor);

            valueLabel.Dock = DockStyle.Fill;
            valueLabel.TextAlign =
                ContentAlignment.MiddleCenter;

            telemetryCardValues[key] =
                valueLabel;

            dataValueLabels[key] =
                valueLabel;

            cmbMetric.SelectedIndexChanged += (s, e) =>
            {
                switch (cmbMetric.SelectedItem?.ToString())
                {
                    case "Roll":
                        valueLabel.Text = "0.0°";
                        break;

                    case "Pitch":
                        valueLabel.Text = "0.0°";
                        break;

                    case "Yaw":
                        valueLabel.Text = "90.0°";
                        break;

                    case "Battery %":
                        valueLabel.Text = "78%";
                        break;

                    case "Voltage":
                        valueLabel.Text = "24.6V";
                        break;

                    case "Current":
                        valueLabel.Text = "8.4A";
                        break;

                    case "Remaining":
                        valueLabel.Text = "82%";
                        break;

                    case "Satellites":
                        valueLabel.Text = "16";
                        break;

                    case "Latitude":
                        valueLabel.Text = "13.074";
                        break;

                    case "Longitude":
                        valueLabel.Text = "80.270";
                        break;

                    case "HDOP":
                        valueLabel.Text = "0.8";
                        break;

                    case "Distance":
                        valueLabel.Text = "184m";
                        break;

                    case "ETA":
                        valueLabel.Text = "12:40";
                        break;

                    case "Waypoint":
                        valueLabel.Text = "C2";
                        break;

                    case "Progress":
                        valueLabel.Text = "38%";
                        break;

                    case "Status":
                        valueLabel.Text = "ONLINE";
                        break;

                    case "Signal Strength":
                        valueLabel.Text = "-68";
                        break;

                    case "Link Quality":
                        valueLabel.Text = "98%";
                        break;

                    case "Armed State":
                        valueLabel.Text = "YES";
                        break;

                    case "Heading":
                        valueLabel.Text = "90°";
                        break;

                    case "Ground Speed":
                        valueLabel.Text = "4.2";
                        break;

                    case "Course Over Ground":
                        valueLabel.Text = "91°";
                        break;

                    case "Turn Rate":
                        valueLabel.Text = "0.5";
                        break;
                }
            };

            Label groupLabel =
    CreateLabel(
        groupName,
        8.5F,
        FontStyle.Bold,
        Color.White);

            groupLabel.Dock = DockStyle.Top;
            groupLabel.Height = 20;
            groupLabel.TextAlign =
                ContentAlignment.MiddleLeft;

            card.Controls.Add(valueLabel);
            card.Controls.Add(cmbMetric);
            card.Controls.Add(groupLabel);

            return card;
        }

        private Control BuildDataMapPanel()
        {
            dataMapHost = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                BackColor = Color.Transparent
            };

            stageIndicatorPanel = BuildStageIndicatorPanel();
            dataMapHost.Controls.Add(stageIndicatorPanel);

            BuildTelemetryStrip(); // no strip panel anymore

            cameraPlaceholderPanel.Dock = DockStyle.Fill;

            dataMapHost.SizeChanged += (s, e) =>
            {
                PositionTelemetryStrip();
                PositionStageIndicatorPanel();
            };

            PositionTelemetryStrip();
            PositionStageIndicatorPanel();

            return dataMapHost;
        }
        //private Control BuildDataMapPanel()
        //{
        //    dataMapHost = new Panel
        //    {
        //        Dock = DockStyle.Fill,
        //        Padding = new Padding(0)
        //    };

        //    Panel strip = BuildTelemetryStrip();

        //    dataMapHost.Controls.Add(strip);

        //    stageIndicatorPanel =
        //        BuildStageIndicatorPanel();

        //    dataMapHost.Controls.Add(
        //        stageIndicatorPanel);

        //    stageIndicatorPanel.BringToFront();

        //    cameraPlaceholderPanel.Dock =
        //        DockStyle.Fill;

        //    dataMapHost.SizeChanged += (s, e) =>
        //    {
        //        PositionTelemetryStrip();
        //        PositionStageIndicatorPanel();
        //    };

        //    PositionTelemetryStrip();
        //    PositionStageIndicatorPanel();

        //    return dataMapHost;
        //}
        private Panel BuildCameraPlaceholderPanel()
        {
            Panel cameraPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black
            };
            cameraTopPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40
            };
            Panel topPanel = cameraTopPanel;

            Label lblRtspTitle = new Label
            {
                Text = "RTSP URL",
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font(
        "Segoe UI",
        9F,
        FontStyle.Bold),
                Top = 10
            };

            txtRtspUrl = new TextBox
            {
                Width = 280,
                Height = 28,
                Text = "rtsp://",
                Top = 6
            };

            btnConnectRtsp = new Button
            {
                Text = "Connect",
                Width = 75,
                Height = 24,
                Top = 6,
                ForeColor = Color.White,
                BackColor = currentTheme.AccentBlue,
                FlatStyle = FlatStyle.Flat
            };

            btnUseWebcam = new Button
            {
                Text = "Webcam",
                Width = 75,
                Height = 24,
                Top = 6,
                ForeColor = Color.White,
                BackColor = currentTheme.AccentBlue,
                FlatStyle = FlatStyle.Flat
            };
            btnConnectRtsp.Size = new Size(75, 24);
            btnUseWebcam.Size = new Size(75, 24);

            lblCameraStatus = new Label
            {
                Text = "Disconnected",
                AutoSize = true,
                ForeColor = Color.White,
                Top = 10
            };

            topPanel.Controls.Add(txtRtspUrl);
            topPanel.Controls.Add(btnConnectRtsp);
            topPanel.Controls.Add(btnUseWebcam);
            topPanel.Controls.Add(lblCameraStatus);
            topPanel.Controls.Add(lblRtspTitle);

            topPanel.Resize += (s, e) =>
            {
                bool compactMode =
                    topPanel.Width < 650;
                topPanel.Visible = !compactMode;

                lblRtspTitle.Visible =
                    !compactMode;

                txtRtspUrl.Visible =
                    !compactMode;

                btnConnectRtsp.Visible =
                    !compactMode;

                int rightMargin = 10;

                btnUseWebcam.Left =
                    topPanel.Width -
                    btnUseWebcam.Width -
                    rightMargin;

                if (!compactMode)
                {
                    btnConnectRtsp.Left =
                        btnUseWebcam.Left -
                        btnConnectRtsp.Width - 8;

                    txtRtspUrl.Left =
                        btnConnectRtsp.Left -
                        txtRtspUrl.Width - 8;

                    lblRtspTitle.Left =
                        txtRtspUrl.Left - 70;

                    lblCameraStatus.Left =
                        txtRtspUrl.Left;
                }

                lblCameraStatus.Top = 10;

            };

            cameraPanel.Controls.Add(topPanel);
            cameraPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Black
            };

            cameraPanel.Controls.Add(cameraPictureBox);
            cameraPictureBox.SendToBack();


            Label lbl = CreateLabel(
    "CAMERA FEED",
    18F,
    FontStyle.Bold,
    Color.White);

            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign =
                ContentAlignment.MiddleCenter;

            cameraPanel.Controls.Add(lbl);
            lbl.BringToFront();
            btnUseWebcam.Click += (s, e) =>
            {
                if (webcamConnected)
                {
                    StopCamera();
                    return;
                }

                StartWebcam();

                webcamConnected = true;
                rtspConnected = false;

                btnUseWebcam.Text = "Stop";
                btnConnectRtsp.Text = "Connect";

                lblCameraStatus.Text =
                    "Webcam Connected";
            };

            btnConnectRtsp.Click += (s, e) =>
            {
                if (rtspConnected)
                {
                    StopCamera();
                    return;
                }

                string rtspUrl =
                    txtRtspUrl.Text.Trim();

                if (string.IsNullOrWhiteSpace(rtspUrl))
                {
                    MessageBox.Show("Enter RTSP URL.");
                    return;
                }

                if (rtspUrl == "rtsp://")
                {
                    MessageBox.Show("Enter RTSP URL.");
                    return;
                }

                btnConnectRtsp.Text = "Connecting...";
                Application.DoEvents();

                StartCamera(rtspUrl);

                rtspConnected = true;
                webcamConnected = false;

                btnConnectRtsp.Text = "Stop";
                btnUseWebcam.Text = "Webcam";

                lblCameraStatus.Text =
                    "RTSP Connected";
            };
            return cameraPanel;
        }
        //private void InitializeVideoPlayer()
        //{
        //Core.Initialize();

        //_libVlc = new LibVLC();

        //_mediaPlayer =
        //new MediaPlayer(_libVlc);

        //_videoView =
        //new VideoView
        //{
        //Dock = DockStyle.Fill,
        //MediaPlayer = _mediaPlayer
        //};

        //cameraPlaceholderPanel.Controls.Add(
        //_videoView);

        //_videoView.BringToFront();

        //_videoInitialized = true;
        //}
        private void StartCamera(string source)
        {
            StopCamera();
            if (webcamCapture != null)
            {
                webcamCapture.Release();
                webcamCapture.Dispose();
            }

            int cameraIndex;

            if (int.TryParse(source, out cameraIndex))
            {
                webcamCapture =
                    new OpenCvSharp.VideoCapture(
                        cameraIndex);
            }
            else
            {
                webcamCapture =
    new OpenCvSharp.VideoCapture(
        source,
        OpenCvSharp.VideoCaptureAPIs.FFMPEG);
            }

            if (!webcamCapture.IsOpened())
            {
                MessageBox.Show(
                    "Unable to open camera.");
                return;
            }

            webcamFrame =
                new OpenCvSharp.Mat();

            if (webcamTimer == null)
            {
                webcamTimer =
                    new System.Windows.Forms.Timer();

                webcamTimer.Interval = 33;

                webcamTimer.Tick +=
                    WebcamTimer_Tick;
            }

            webcamTimer.Start();
        }
        private void StartWebcam()
        {
            StartCamera("0");
        }
        private void StopCamera()
        {
            webcamTimer?.Stop();

            if (webcamCapture != null)
            {
                webcamCapture.Release();
                webcamCapture.Dispose();
                webcamCapture = null;
            }

            if (cameraPictureBox.Image != null)
            {
                cameraPictureBox.Image.Dispose();
                cameraPictureBox.Image = null;
            }

            rtspConnected = false;
            webcamConnected = false;

            btnConnectRtsp.Text = "Connect";
            btnUseWebcam.Text = "Webcam";

            lblCameraStatus.Text = "Disconnected";
        }
        private void WebcamTimer_Tick(
    object sender,
    EventArgs e)
        {
            if (webcamCapture == null)
                return;

            if (!webcamCapture.Read(webcamFrame))
                return;

            if (webcamFrame.Empty())
                return;

            Bitmap frame =
                OpenCvSharp.Extensions.BitmapConverter
                    .ToBitmap(webcamFrame);

            // DATA PAGE CAMERA
            cameraPictureBox.Image?.Dispose();

            cameraPictureBox.Image =
                (Bitmap)frame.Clone();

            // TARGET MODE CAMERA
            if (targetCameraPictureBox != null)
            {
                targetCameraPictureBox.Image?.Dispose();

                targetCameraPictureBox.Image =
                    (Bitmap)frame.Clone();
            }

            frame.Dispose();
        }
        private void SwapViewports()
        {
            if (secondaryViewportHost == null)
                return;

            if (mapSection == null)
                return;

            if (isMapPrimary)
            {
                // Move map to left

                if (mapSection.Parent != null)
                    mapSection.Parent.Controls.Remove(
                        mapSection);

                secondaryViewportHost.Controls.Clear();

                secondaryViewportHost.Controls.Add(
                    mapSection);

                secondaryViewportHost.Controls.Add(
                    btnSwap);

                btnSwap.BringToFront();

                dataMapHost.Controls.Remove(
                    mapSection);

                dataMapHost.Controls.Add(
                    cameraPlaceholderPanel);

                telemetryStripPanel?.BringToFront();

                if (cameraTopPanel != null)
                {
                    cameraTopPanel.Visible = false;
                }
                isMapPrimary = false;
                cameraTopPanel.Visible = true;
                PositionStageIndicatorPanel();
            }
            else
            {
                // Move map back to right

                if (mapSection.Parent != null)
                    mapSection.Parent.Controls.Remove(
                        mapSection);

                dataMapHost.Controls.Remove(
                    cameraPlaceholderPanel);

                dataMapHost.Controls.Add(
                    mapSection);

                secondaryViewportHost.Controls.Clear();

                secondaryViewportHost.Controls.Add(
                    cameraPlaceholderPanel);

                secondaryViewportHost.Controls.Add(
                    btnSwap);

                btnSwap.Location =
                    new Point(
                        secondaryViewportHost.Width - 42,
                        6);

                btnSwap.BringToFront();

                telemetryStripPanel?.BringToFront();
                if (cameraTopPanel != null)
                {
                    cameraTopPanel.Visible = true;
                }
                isMapPrimary = true;
                cameraTopPanel.Visible = false;
                PositionStageIndicatorPanel();
            }
        }
        private void PositionTelemetryStrip()
        {
            if (telemetryStripPanel == null ||
                dataMapHost == null)
                return;

            telemetryStripPanel.Location =
                new Point(
                    40,
                    dataMapHost.Height - 140);

            telemetryStripPanel.BringToFront();
        }
        private void PositionStageIndicatorPanel()
        {
            if (stageIndicatorPanel == null ||
                dataMapHost == null)
                return;

            if (isMapPrimary)
            {
                stageIndicatorPanel.Location =
                    new Point(170, 10);
            }
            else
            {
                stageIndicatorPanel.Location =
                    new Point(260, 50);
            }

            stageIndicatorPanel.BringToFront();
        }
        private Panel CreateStageWordGroup(
    string key,
    string firstLetter,
    string rest)
        {
            FlowLayoutPanel group =
                new FlowLayoutPanel
                {
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = false,
                    Margin = new Padding(0, 0, 40, 0),
                    BackColor = Color.Transparent
                };

            Label big = new Label
            {
                Text = firstLetter,
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    14F,
                    FontStyle.Bold),
                ForeColor = currentTheme.TextMuted,
                BackColor = Color.Transparent,
                Margin = new Padding(0)
            };

            Label small = new Label
            {
                Text = rest,
                AutoSize = true,
                Font = new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Regular),
                ForeColor = currentTheme.TextMuted,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 5, 0, 0)
            };

            group.Controls.Add(big);
            group.Controls.Add(small);

            switch (key)
            {
                case "Cruise":
                    lblStageCruise = big;
                    lblStageCruiseRest = small;
                    break;

                case "Survey":
                    lblStageLoiter = big;
                    lblStageLoiterRest = small;
                    break;

                case "Burst":
                    lblStageBurst = big;
                    lblStageBurstRest = small;
                    break;

                case "Return Cruise":
                    lblStageReturn = big;
                    lblStageReturnRest = small;
                    break;
            }

            stageIndicatorGroups[key] = group;

            return group;
        }
        private Color GetStageAccentColor(string missionType)
        {
            switch (missionType)
            {
                case "Cruise":
                    return currentTheme.AccentBlue;

                case "Survey":
                    return currentTheme.AccentYellow;

                case "Burst":
                    return currentTheme.AccentPurple;

                case "Return Cruise":
                    return currentTheme.Success;

                default:
                    return currentTheme.TextMuted;
            }
        }

        private void SetActiveStageIndicator(
            string missionType)
        {
            Color muted =
                currentTheme.TextMuted;

            lblStageCruise.ForeColor = muted;
            lblStageCruiseRest.ForeColor = muted;

            lblStageLoiter.ForeColor = muted;
            lblStageLoiterRest.ForeColor = muted;

            lblStageBurst.ForeColor = muted;
            lblStageBurstRest.ForeColor = muted;

            lblStageReturn.ForeColor = muted;
            lblStageReturnRest.ForeColor = muted;

            Color activeColor =
                GetStageAccentColor(
                    missionType);
            lblStageCruise.Font =
    new Font("Segoe UI", 14F, FontStyle.Bold);

            lblStageCruiseRest.Font =
                new Font("Segoe UI", 9F, FontStyle.Regular);

            lblStageLoiter.Font =
                new Font("Segoe UI", 14F, FontStyle.Bold);

            lblStageLoiterRest.Font =
                new Font("Segoe UI", 9F, FontStyle.Regular);

            lblStageBurst.Font =
                new Font("Segoe UI", 14F, FontStyle.Bold);

            lblStageBurstRest.Font =
                new Font("Segoe UI", 9F, FontStyle.Regular);

            lblStageReturn.Font =
                new Font("Segoe UI", 14F, FontStyle.Bold);

            lblStageReturnRest.Font =
                new Font("Segoe UI", 9F, FontStyle.Regular);

            switch (missionType)
            {
                case "Cruise":
                    lblStageCruise.ForeColor = activeColor;
                    lblStageCruiseRest.ForeColor = activeColor;
                    break;

                case "Survey":
                    lblStageLoiter.ForeColor = activeColor;
                    lblStageLoiterRest.ForeColor = activeColor;
                    break;

                case "Burst":
                    lblStageBurst.ForeColor = activeColor;
                    lblStageBurstRest.ForeColor = activeColor;
                    break;

                case "Return Cruise":
                    lblStageReturn.ForeColor = activeColor;
                    lblStageReturnRest.ForeColor = activeColor;
                    break;
            }
        }
        private Panel BuildStageIndicatorPanel()
        {
            Panel panel = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent
            };
            FlowLayoutPanel flow =
    new FlowLayoutPanel
    {
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        FlowDirection = FlowDirection.LeftToRight,
        WrapContents = false,
        BackColor = Color.Transparent
    };
            flow.WrapContents = false;
            flow.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            flow.Controls.Add(CreateStageWordGroup("Cruise", "C", "ruise"));

            flow.Controls.Add(new Label
            {
                Width = 30
            });

            flow.Controls.Add(CreateStageWordGroup("Survey", "L", "oiter"));

            flow.Controls.Add(new Label
            {
                Width = 30
            });

            flow.Controls.Add(CreateStageWordGroup("Burst", "B", "urst"));

            flow.Controls.Add(new Label
            {
                Width = 30
            });

            flow.Controls.Add(CreateStageWordGroup("Return Cruise", "R", "eturn Cruise"));

            panel.Controls.Add(flow);

            return panel;
        }

        private Panel CreateQuickTelemetryTab()
        {
            return CreateTelemetryConsoleTab(new[]
            {
                Tuple.Create("BatteryRemaining", "78%", "Battery"),
                Tuple.Create("BatteryVoltage", "24.6V", "Voltage"),
                Tuple.Create("BatteryCurrent", "8.4A", "Current"),
                Tuple.Create("Heading", "086", "Heading"),
                Tuple.Create("Cog", "091", "COG"),
                Tuple.Create("Sog", "4.2", "SOG"),
                Tuple.Create("GpsFix", "3D", "GPS Fix"),
                Tuple.Create("SatelliteCount", "16", "Sat Count"),
                Tuple.Create("Hdop", "0.9", "HDOP"),
                Tuple.Create("CurrentWaypoint", "C2", "Waypoint"),
                Tuple.Create("DistanceToWaypoint", "184m", "Dist to WP"),
                Tuple.Create("MissionProgress", "38%", "Progress"),
                Tuple.Create("VehicleMode", "AUTO", "Mode"),
                Tuple.Create("ArmedStatus", "SAFE", "Armed"),
                Tuple.Create("Rssi", "-68", "RSSI"),
                Tuple.Create("HeartbeatStatus", "OK", "Heartbeat"),
                Tuple.Create("FirmwareVersion", "v2.4.1", "Firmware"),
                Tuple.Create("SystemStatus", "NOM", "System")
            });
        }

        private Panel CreateTelemetryConsoleTab(IEnumerable<Tuple<string, string, string>> metrics)
        {
            Panel page = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            RoundedPanel consolePanel = new RoundedPanel
            {
                Dock = DockStyle.Top,
                Theme = currentTheme,
                FillColor = Color.FromArgb(140, 10, 20, 40),
                Radius = 8,
                Height = 620
            };
            themeAwareControls.Add(consolePanel);

            Tuple<string, string, string>[] metricArray = metrics.ToArray();
            int rowCount = (int)Math.Ceiling(metricArray.Length / 2.0);

            TableLayoutPanel grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = rowCount,
                Padding = new Padding(10, 10, 10, 10)
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            for (int row = 0; row < rowCount; row++)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / rowCount));
            }

            for (int index = 0; index < metricArray.Length; index++)
            {
                int row = index / 2;
                int column = index % 2;
                grid.Controls.Add(CreateTelemetryMetricTile(metricArray[index].Item1, metricArray[index].Item2, metricArray[index].Item3), column, row);
            }

            consolePanel.Controls.Add(grid);
            page.Controls.Add(consolePanel);
            return page;
        }

        private Panel CreateTelemetryMetricTile(string key, string valueText, string captionText)
        {
            Panel tile = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(2),
                Padding = new Padding(2)
            };

            Label valueLabel = CreateLabel(valueText, 28F, FontStyle.Regular, currentTheme.TextPrimary);
            valueLabel.Dock = DockStyle.Top;
            valueLabel.Height = 46;
            valueLabel.AutoSize = false;
            valueLabel.TextAlign = ContentAlignment.BottomCenter;
            metricValueLabels.Add(valueLabel);
            dataValueLabels[key] = valueLabel;

            Label captionLabel = CreateLabel(captionText, 8F, FontStyle.Regular, currentTheme.TextMuted);
            captionLabel.Dock = DockStyle.Fill;
            captionLabel.AutoSize = false;
            captionLabel.TextAlign = ContentAlignment.TopCenter;
            metricCaptionLabels.Add(captionLabel);

            tile.Controls.Add(captionLabel);
            tile.Controls.Add(valueLabel);
            return tile;
        }

        private Button CreateDataSidebarTab(string text)
        {
            Button button = CreateButton(text, currentTheme.PanelAlt, currentTheme.TextPrimary, 0, 26, false);
            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(0, 0, 2, 0);
            button.FlatAppearance.BorderSize = 1;
            button.Font = new Font("Segoe UI", 7.5F, FontStyle.Regular);
            button.Click += (s, e) => SwitchDataTab(text);
            dataTabButtons[text] = button;
            return button;
        }

        private void SwitchDataTab(string tabName)
        {
            foreach (KeyValuePair<string, Panel> tab in dataTabPanels)
            {
                tab.Value.Visible = string.Equals(tab.Key, tabName, StringComparison.OrdinalIgnoreCase);
            }

            UpdateDataTabButtons(tabName);
        }

        private void UpdateDataTabButtons(string activeTabName)
        {
            foreach (KeyValuePair<string, Button> tab in dataTabButtons)
            {
                bool active = string.Equals(tab.Key, activeTabName, StringComparison.OrdinalIgnoreCase);
                tab.Value.BackColor = active ? currentTheme.AccentBlue : currentTheme.PanelAlt;
                tab.Value.ForeColor = active ? Color.White : currentTheme.TextPrimary;
                tab.Value.FlatAppearance.BorderColor = active ? currentTheme.AccentBlue : currentTheme.Border;
                tab.Value.FlatAppearance.MouseOverBackColor = active
                    ? ControlPaint.Light(currentTheme.AccentBlue)
                    : ControlPaint.Light(currentTheme.PanelAlt);
                tab.Value.FlatAppearance.MouseDownBackColor = active
                    ? ControlPaint.Dark(currentTheme.AccentBlue)
                    : ControlPaint.Dark(currentTheme.PanelAlt);
            }
        }

        // ───────────────────────────────────────────────────────────────
        // LEFT PANEL — Mission Builder
        // ───────────────────────────────────────────────────────────────

        private Control BuildLeftPanel()
        {
            missionBuilderSection = CreateSection("Mission Builder");
            missionBuilderSection.Content.Padding = new Padding(10, 8, 10, 8);

            FlowLayoutPanel content = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = false
            };
            int cardWidth = 245;

            // Stage cards
            content.Controls.Add(CreateSubheading("Mission Stages", cardWidth));
            content.Controls.Add(CreateStageCard("Cruise", "0 waypoints", "0 kn", currentTheme.AccentBlue, cardWidth));
            content.Controls.Add(CreateStageCard("Loiter", "Grid Pattern", "0 kn", currentTheme.AccentYellow, cardWidth));
            content.Controls.Add(CreateStageCard("Burst", "Guided Mode", "0 kn", currentTheme.AccentPurple, cardWidth));
            content.Controls.Add(CreateStageCard("Return Cruise", "0 waypoints", "0 kn", currentTheme.Success, cardWidth));

            // Stage settings
            content.Controls.Add(CreateSubheading("Stage Settings", cardWidth));
            content.Controls.Add(CreateInfoField("Stage", out lblSelectedStageValue, selectedStageName.ToUpperInvariant(), cardWidth));
            content.Controls.Add(CreateInfoField("Pattern Type", out lblPatternStatusValue, "WAYPOINT CAPTURE", cardWidth));

            // Pattern dropdown
            cmbPattern = new ComboBox
            {
                Width = cardWidth,
                Height = 32,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 0, 0, 6)
            };
            cmbPattern.Items.AddRange(new object[] { "Linear", "Grid", "Circular" });
            cmbPattern.SelectedIndex = 1;

            comboBoxes.Add(cmbPattern);
            content.Controls.Add(cmbPattern);
            Label lblDiameter = new Label
            {
                Text = "Circle Diameter (m)",
                AutoSize = true,
                ForeColor = currentTheme.TextPrimary,
                Margin = new Padding(0, 4, 0, 2)
            };
            themeLabels.Add(lblDiameter);

            TextBox txtCircleDiameter = new TextBox
            {
                Name = "txtCircleDiameter",
                Width = cardWidth,
                Text = "300"
            };

            content.Controls.Add(lblDiameter);
            content.Controls.Add(txtCircleDiameter);
            cmbPattern.SelectedIndexChanged += (s, e) =>
            {
                string selected =
                    cmbPattern.SelectedItem?.ToString();

                bool isCircular =
                    selected == "Circular";

                lblDiameter.Visible = isCircular;
                txtCircleDiameter.Visible = isCircular;
                string selectedPattern =
    cmbPattern.SelectedItem?.ToString();

                if (selectedStageName.Equals("Loiter",
                    StringComparison.OrdinalIgnoreCase))
                {
                    lblLoiterStageSubtitle.Text =
                        selectedPattern;
                }


                CmbPattern_SelectedIndexChanged(s, e);
            };
            lblDiameter.Visible = false;
            txtCircleDiameter.Visible = false;


            // Action buttons
            content.Controls.Add(CreateSubheading("Actions", cardWidth));
            Button btnSurveyMode = CreateButton("Draw Polygon Boundary", currentTheme.PanelAlt, currentTheme.TextPrimary, cardWidth, 34, false);
            btnSurveyMode.Margin = new Padding(0, 0, 0, 6);
            btnSurveyMode.Click += ToggleSurveyPolygonMode;
            content.Controls.Add(btnSurveyMode);

            FlowLayoutPanel footer = new FlowLayoutPanel
            {
                Width = cardWidth,
                Height = 40,
                WrapContents = false,
                Margin = new Padding(0, 8, 0, 4)
            };
            Button btnAddStage = CreateButton("Add Stage", currentTheme.AccentBlue, Color.White, 130, 34, true);
            btnAddStage.Click += PlaceholderButton_Click;
            footer.Controls.Add(btnAddStage);
            Button btnClear = CreateButton("Clear All", currentTheme.PanelAlt, currentTheme.TextPrimary, 130, 34, false);
            btnClear.Margin = new Padding(4, 0, 0, 0);
            btnClear.Click += ClearAllButton_Click;
            footer.Controls.Add(btnClear);
            content.Controls.Add(footer);

            missionBuilderSection.Content.Controls.Add(content);
            Panel wrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 8, 0) };
            wrapper.Controls.Add(missionBuilderSection);
            return wrapper;
        }

        // ───────────────────────────────────────────────────────────────
        // CENTER PANEL — Map + Waypoint Table
        // ───────────────────────────────────────────────────────────────

        private Control BuildCenterPanel()
        {
            Panel wrapper = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 8, 0) };

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            //hud1 = new MissionPlanner.Controls.HUD
            //{
            //Dock = DockStyle.Fill,
            //BackColor = Color.Black
            //};

            // Temporary demo values
            //hud1.heading = 90;
            //hud1.roll = 0;
            //hud1.pitch = 0;
            //hud1.groundspeed = 4.2f;
            //hud1.batteryremaining = 78;
            //hud1.batterylevel = 24.6f;
            //hud1.mode = "AUTO";
            //hud1.connected = true;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 70F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));

            mapSection = CreateSection("Mission Map");
            waypointSection = CreateSection("Mission Waypoint Table");
            BuildMapSection();
            BuildWaypointSection();

            missionMapHost = new Panel { Dock = DockStyle.Fill, Margin = Padding.Empty };
            missionMapHost.Controls.Add(mapSection);

            layout.Controls.Add(missionMapHost, 0, 0);
            layout.Controls.Add(waypointSection, 0, 1);
            wrapper.Controls.Add(layout);
            return wrapper;
        }

        // ───────────────────────────────────────────────────────────────
        // RIGHT PANEL — Mission Summary (full redesign)
        // ───────────────────────────────────────────────────────────────

        private Control BuildRightPanel()
        {
            missionSummarySection = CreateSection("Mission Summary");
            BuildMissionSummarySection();
            return missionSummarySection;
        }

        private void BuildMissionSummarySection()
        {
            Panel content = missionSummarySection.Content;
            content.Padding = new Padding(12, 8, 12, 10);

            // Use a TableLayoutPanel for the full-height layout:
            // Row 0: Mission Statistics   ~28%
            // Row 1: Survey Info          ~14%
            // Row 2: Mission Actions      ~26%
            // Row 3: Mission Status       ~20%
            // Row 4: Telemetry footer     ~12%
            TableLayoutPanel outer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3
            };
            outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            //outer.RowStyles.Add(new RowStyle(SizeType.Percent, 28F));
            //outer.RowStyles.Add(new RowStyle(SizeType.Percent, 14F));
            //outer.RowStyles.Add(new RowStyle(SizeType.Percent, 26F));
            //outer.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            //outer.RowStyles.Add(new RowStyle(SizeType.Percent, 12F));
            outer.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));
            outer.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            outer.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));

            // ── SECTION 0: Mission Statistics ──────────────────────────
            Panel statsBlock = CreateSubSection("Mission Statistics");
            TableLayoutPanel statsGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                Padding = new Padding(4, 2, 4, 4)
            };
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            statsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            for (int i = 0; i < 3; i++) statsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));

            statsGrid.Controls.Add(CreateMetricLabel("Total Waypoints"), 0, 0);
            statsGrid.Controls.Add(CreateMetricLabel("Distance"), 0, 1);
            statsGrid.Controls.Add(CreateMetricLabel("Est. Duration"), 0, 2);
            //statsGrid.Controls.Add(CreateMetricLabel("Fuel / Energy"), 0, 3);

            lblSummaryWaypointsValue = CreateMetricValue("0");
            lblSummaryDistanceValue = CreateMetricValue("0.0 km");
            lblSummaryDurationValue = CreateMetricValue("00:00:00");
            //lblSummaryFuelValue = CreateMetricValue("68 %");

            statsGrid.Controls.Add(lblSummaryWaypointsValue, 1, 0);
            statsGrid.Controls.Add(lblSummaryDistanceValue, 1, 1);
            statsGrid.Controls.Add(lblSummaryDurationValue, 1, 2);
            //statsGrid.Controls.Add(lblSummaryFuelValue, 1, 3);
            statsBlock.Controls.Add(statsGrid);
            outer.Controls.Add(statsBlock, 0, 0);

            // ── SECTION 1: Survey Info ──────────────────────────────────
            Panel surveyBlock = CreateSubSection("Survey Info");
            TableLayoutPanel surveyGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Padding = new Padding(4, 2, 4, 4)
            };
            surveyGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58F));
            surveyGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42F));
            surveyGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            surveyGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            surveyGrid.Controls.Add(CreateMetricLabel("Loiter Diameter"), 0, 0);
            surveyGrid.Controls.Add(CreateMetricLabel("Area Coverage"), 0, 1);
            lblSurveyLinesValue = CreateMetricValue("—");
            lblSurveyCoverageValue = CreateMetricValue("—");
            surveyGrid.Controls.Add(lblSurveyLinesValue, 1, 0);
            surveyGrid.Controls.Add(lblSurveyCoverageValue, 1, 1);
            surveyBlock.Controls.Add(surveyGrid);
            outer.Controls.Add(surveyBlock, 0, 1);

            // ── SECTION 2: Mission Actions ─────────────────────────────
            Panel actionsBlock = CreateSubSection("Mission Actions");
            TableLayoutPanel actionsGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(4, 4, 4, 4)
            };
            actionsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            actionsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            actionsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            actionsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            actionsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));

            Button btnSurvey = CreateActionBtn("Generate Loiter", currentTheme.AccentYellow, Color.Black);
            btnSurvey.Click += GenerateSurveyButton_Click;
            Button btnLoad = CreateActionBtn("Load Mission", currentTheme.PanelAlt, currentTheme.TextPrimary);
            btnLoad.Click += async (s, e) =>
            {
                MissionPlan downloadedMission =
                    await missionPlannerAdapter.DownloadMissionAsync();

                if (!downloadedMission.AllMissionPoints.Any())
                {
                    MessageBox.Show(
                        "No mission found in Pixhawk.",
                        "Load Mission");

                    return;
                }

                missionManager.MissionPlan.Stages.Clear();

                foreach (MissionStage stage in downloadedMission.Stages)
                {
                    missionManager.MissionPlan.Stages.Add(stage);
                }

                RefreshWaypointGrid();
                RefreshMissionSummary();
                RefreshMapFromMission();

                MessageBox.Show(
                    $"Loaded {downloadedMission.AllMissionPoints.Count()} waypoint(s).",
                    "Load Mission");
            };
            Button btnValidate = CreateActionBtn("\u2713 Validate Mission", currentTheme.PanelAlt, currentTheme.Success);
            btnValidate.Click += ValidateButton_Click;
            Button btnUpload = CreateActionBtn("Upload Mission", currentTheme.AccentBlue, Color.White);
            btnUpload.Click += UploadMissionButton_Click;

            actionsGrid.Controls.Add(btnSurvey, 0, 0);
            actionsGrid.Controls.Add(btnLoad, 0, 1);
            actionsGrid.Controls.Add(btnValidate, 0, 2);
            actionsGrid.Controls.Add(btnUpload, 0, 3);
            actionsBlock.Controls.Add(actionsGrid);
            outer.Controls.Add(actionsBlock, 0, 2);

            // ── SECTION 3: Mission Status ──────────────────────────────
            //Panel statusBlock = CreateSubSection("Mission Status");
            //TableLayoutPanel statusGrid = new TableLayoutPanel
            //{
            //    Dock = DockStyle.Fill,
            //    ColumnCount = 2,
            //    RowCount = 4,
            //    Padding = new Padding(4, 2, 4, 4)
            //};
            //statusGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            //statusGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            //for (int i = 0; i < 4; i++) statusGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));

            //statusGrid.Controls.Add(CreateMetricLabel("Status"), 0, 0);
            //statusGrid.Controls.Add(CreateMetricLabel("GPS"), 0, 1);
            //statusGrid.Controls.Add(CreateMetricLabel("Current Stage"), 0, 2);
            //statusGrid.Controls.Add(CreateMetricLabel("Armed"), 0, 3);

            //lblStatusPanelReady = CreateMetricValue("Ready");
            //lblStatusPanelReady.ForeColor = currentTheme.Success;
            //lblStatusPanelGps = CreateMetricValue("3D Fix");
            //lblStatusPanelGps.ForeColor = currentTheme.Success;
            //lblStatusPanelStage = CreateMetricValue("CRUISE");
            //lblStatusPanelArmed = CreateMetricValue("YES");
            //lblStatusPanelArmed.ForeColor = currentTheme.Success;

            //statusGrid.Controls.Add(lblStatusPanelReady, 1, 0);
            //statusGrid.Controls.Add(lblStatusPanelGps, 1, 1);
            //statusGrid.Controls.Add(lblStatusPanelStage, 1, 2);
            //statusGrid.Controls.Add(lblStatusPanelArmed, 1, 3);
            //statusBlock.Controls.Add(statusGrid);
            //outer.Controls.Add(statusBlock, 0, 3);

            // ── SECTION 4: Telemetry Footer ────────────────────────────
            //Panel telBlock = CreateSubSection("Telemetry");
            //TableLayoutPanel telGrid = new TableLayoutPanel
            //{
            //    Dock = DockStyle.Fill,
            //    ColumnCount = 3,
            //    RowCount = 2,
            //    Padding = new Padding(4, 2, 4, 4)
            //};
            //for (int i = 0; i < 3; i++) telGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            //telGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            //telGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            //telGrid.Controls.Add(CreateTelemetryCell("SOG", "12.4 kn", currentTheme.AccentBlue), 0, 0);
            //telGrid.Controls.Add(CreateTelemetryCell("COG", "089°", currentTheme.TextPrimary), 1, 0);
            //telGrid.Controls.Add(CreateTelemetryCell("BATT", "82%", currentTheme.Success), 2, 0);
            //telGrid.Controls.Add(CreateTelemetryCell("ALT", "2.1 m", currentTheme.AccentBlue), 0, 1);
            //telGrid.Controls.Add(CreateTelemetryCell("HDOP", "0.8", currentTheme.TextPrimary), 1, 1);
            //telGrid.Controls.Add(CreateTelemetryCell("RSSI", "-67dBm", currentTheme.TextPrimary), 2, 1);
            //telBlock.Controls.Add(telGrid);
            //outer.Controls.Add(telBlock, 0, 4);

            content.Controls.Add(outer);
        }

        // ───────────────────────────────────────────────────────────────
        // MAP SECTION
        // ───────────────────────────────────────────────────────────────

        private void BuildMapSection()
        {
            mapSection.Content.Padding = new Padding(6, 0, 6, 6);

            //Panel tabs = new Panel
            //{
            //    Dock = DockStyle.Top,
            //    Height = 28
            // };

            //tabs.Controls.Add(CreateTabLabel("MISSION MAP", 8, true));

            Panel host = new Panel { Dock = DockStyle.Fill };

            gmap = new GMapControl
            {
                Dock = DockStyle.Fill,
                EmptyTileColor = Color.Black,
                GrayScaleMode = false,
                HelperLineOption = HelperLineOptions.DontShow,
                CanDragMap = true,
                DragButton = MouseButtons.Left,
                MaxZoom = 19,
                MinZoom = 2,
                MouseWheelZoomEnabled = true,
                MouseWheelZoomType = MouseWheelZoomType.MousePositionAndCenter,
                ShowCenter = false,
                Bearing = 0f
            };
            gmap.MouseClick += Gmap_MouseClick;
            gmap.OnMarkerClick += Gmap_OnMarkerClick; ;

            Panel overlayTools = new Panel
            {
                Width = 38,
                Dock = DockStyle.Right,
                Padding = new Padding(4, 8, 0, 8),
                BackColor = Color.Transparent
            };
            FlowLayoutPanel toolStack = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            toolStack.Controls.Add(CreateMapToolButton(">", (s, e) => gmap.Bearing = 0f));
            toolStack.Controls.Add(CreateMapToolButton("+", (s, e) => gmap.Zoom += 1));
            toolStack.Controls.Add(CreateMapToolButton("-", (s, e) => gmap.Zoom -= 1));
            toolStack.Controls.Add(CreateMapToolButton("O", (s, e) => CenterMapOnMission()));
            toolStack.Controls.Add(CreateMapToolButton("[]", (s, e) => ToggleSurveyPolygonMode(s, e)));
            overlayTools.Controls.Add(toolStack);

            //host.Controls.Add(BuildMapLegend());
            host.Controls.Add(gmap);

            mapSection.Content.Controls.Add(host);

            //mapSection.Content.Controls.Add(tabs);
        }

        private void BuildWaypointSection()
        {
            waypointSection.Content.Padding = new Padding(6);
            dgvWaypoints = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                MultiSelect = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvWaypoints.Columns.Add("MissionType", "Mission Type");
            dgvWaypoints.Columns.Add("PointNumber", "Point Number");
            dgvWaypoints.Columns.Add("Latitude", "Latitude");
            dgvWaypoints.Columns.Add("Longitude", "Longitude");
            dgvWaypoints.Columns.Add("Altitude", "Altitude");
            dgvWaypoints.Columns.Add("Speed", "Speed");
            dgvWaypoints.Columns.Add("Heading", "Heading");
            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn();

            deleteColumn.Name = "Delete";
            deleteColumn.HeaderText = "Delete";
            deleteColumn.Text = "X";
            deleteColumn.UseColumnTextForButtonValue = true;

            dgvWaypoints.Columns.Add(deleteColumn);
            dgvWaypoints.CellClick += DgvWaypoints_CellClick;
            dgvWaypoints.CellDoubleClick += DgvWaypoints_CellDoubleClick;
            waypointSection.Content.Controls.Add(dgvWaypoints);
        }

        private Panel BuildStatusBar()
        {
            Panel panel = new Panel { Dock = DockStyle.Bottom, Height = 28, Padding = new Padding(14, 0, 14, 0) };
            TableLayoutPanel layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 9 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 6F));

            Label lblComm = CreateStatusBarLabel("● COMM: UDP");
            lblComm.ForeColor = currentTheme.Success;
            Label lblSrate = CreateStatusBarLabel("SRATE: 115200");
            Label lblVehicle = CreateStatusBarLabel("Vehicle: WAVEbot-01");
            lblVehicle.TextAlign = ContentAlignment.MiddleCenter;
            Label lblFirmware = CreateStatusBarLabel("Firmware: 4.5.7");
            lblFirmware.TextAlign = ContentAlignment.MiddleCenter;

            lblStatusBarReady = CreateStatusBarLabel("Ready");
            lblStatusBarReady.ForeColor = currentTheme.Success;
            lblStatusBarGps = CreateStatusBarLabel("GPS: 3D Fix");
            lblStatusBarGps.ForeColor = currentTheme.Success;
            lblStatusBarBattery = CreateStatusBarLabel("Battery: 82%");
            lblStatusBarBattery.ForeColor = currentTheme.AccentYellow;
            lblStatusBarMode = CreateStatusBarLabel("Mode: CRUISE");
            lblStatusBarArmed = CreateStatusBarLabel("Armed: YES");
            lblStatusBarArmed.ForeColor = currentTheme.Success;

            layout.Controls.Add(lblComm, 0, 0);
            layout.Controls.Add(lblSrate, 1, 0);
            layout.Controls.Add(lblVehicle, 2, 0);
            layout.Controls.Add(lblFirmware, 3, 0);
            layout.Controls.Add(lblStatusBarReady, 4, 0);
            layout.Controls.Add(lblStatusBarGps, 5, 0);
            layout.Controls.Add(lblStatusBarBattery, 6, 0);
            layout.Controls.Add(lblStatusBarMode, 7, 0);
            layout.Controls.Add(lblStatusBarArmed, 8, 0);
            panel.Controls.Add(layout);
            return panel;
        }
        private Panel CreateSimulationCard(
    string title,
    out Label valueLabel,
    string value)
        {
            Panel card = new Panel
            {
                Height = 90,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 12),
                BackColor = Color.FromArgb(10, 22, 45),
                BorderStyle = BorderStyle.None
            };

            Label lblTitle = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray,
                Font = new Font(
    "Segoe UI",
    8F,
    FontStyle.Regular)
            };

            valueLabel = new Label
            {
                TextAlign =
    ContentAlignment.MiddleCenter,
                Text = value,
                Dock = DockStyle.Fill,
                ForeColor = Color.DeepSkyBlue,
                Font = new Font(
    "Segoe UI",
    18F,
    FontStyle.Bold)
            };

            card.Controls.Add(valueLabel);
            card.Controls.Add(lblTitle);

            return card;
        }


        // ═══════════════════════════════════════════════════════════════
        // MAP INITIALISATION
        // ═══════════════════════════════════════════════════════════════

        private void InitializeMap()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gmap.MapProvider = BingSatelliteMapProvider.Instance;
            gmap.Position = new PointLatLng(13.074560, 80.270980);
            gmap.Zoom = 14;
            gmap.OnMapZoomChanged += Gmap_OnMapZoomChanged;

            waypointOverlay = new GMapOverlay("waypoints");
            routeOverlay = new GMapOverlay("routes");
            polygonOverlay = new GMapOverlay("polygons");
            surveyOverlay = new GMapOverlay("survey");

            gmap.Overlays.Clear();
            gmap.Overlays.Add(routeOverlay);
            gmap.Overlays.Add(polygonOverlay);
            gmap.Overlays.Add(surveyOverlay);
            gmap.Overlays.Add(waypointOverlay);
        }

        private void Gmap_OnMapZoomChanged()
        {
            RefreshMapFromMission();
        }

        private void BuildWaypointActionMenu()
        {
            waypointActionMenu = new ContextMenuStrip();
            waypointActionMenu.Items.Add("Move Up", null, (s, e) =>
            {
                if (selectedWaypoint != null) missionManager.MoveWaypointUp(selectedWaypoint.Id);
            });
            waypointActionMenu.Items.Add("Move Down", null, (s, e) =>
            {
                if (selectedWaypoint != null) missionManager.MoveWaypointDown(selectedWaypoint.Id);
            });
            waypointActionMenu.Items.Add("Delete", null, (s, e) =>
            {
                if (selectedWaypoint != null) missionManager.RemoveWaypoint(selectedWaypoint.Id);
            });
        }

        private Panel BuildMapLegend()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 46,
                Padding = new Padding(10, 8, 10, 8),
                BackColor = Color.FromArgb(170, 10, 16, 24)
            };
            FlowLayoutPanel legend = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            legend.Controls.Add(CreateLegendItem("C", "Cruise", currentTheme.AccentBlue));
            legend.Controls.Add(CreateLegendItem("L", "Loiter / Search", currentTheme.AccentYellow));
            legend.Controls.Add(CreateLegendItem("B", "Burst (GUIDED)", currentTheme.AccentPurple));
            legend.Controls.Add(CreateLegendItem("R", "Return Cruise", currentTheme.Success));
            legend.Controls.Add(CreateLegendItem("V", "Vehicle", Color.Silver));
            legend.Controls.Add(CreateLegendItem("H", "Home", currentTheme.AccentRed));

            Label scale = CreateLabel("500 m", 9F, FontStyle.Regular, Color.White);
            scale.Dock = DockStyle.Right; scale.TextAlign = ContentAlignment.MiddleRight;
            scale.AutoSize = false; scale.Width = 60;
            panel.Controls.Add(scale);
            panel.Controls.Add(legend);
            return panel;
        }

        private Panel CreateLegendItem(string badge, string text, Color badgeColor)
        {
            Panel item = new Panel
            {
                Width = 118,
                Height = 22,
                Margin = new Padding(0, 0, 6, 0),
                BackColor = Color.Transparent
            };
            Label b = CreateBadgeLabel(badge, badgeColor, new Size(16, 16));
            b.Location = new Point(0, 2);
            item.Controls.Add(b);
            Label t = CreateLabel(text, 8.5F, FontStyle.Regular, Color.White);
            t.Location = new Point(22, 2);
            item.Controls.Add(t);
            return item;
        }

        private Button CreateMapToolButton(string text, EventHandler click)
        {
            Button button = CreateButton(text, currentTheme.PanelAlt, currentTheme.TextPrimary, 28, 28, false);
            button.Margin = new Padding(0, 0, 0, 4);
            button.Click += click;
            return button;
        }

        // ═══════════════════════════════════════════════════════════════
        // SEED DATA
        // ═══════════════════════════════════════════════════════════════

        private void SeedDemoMission()
        {
            missionManager.AddWaypoint("Cruise", 13.072540, 80.268220);
            missionManager.AddWaypoint("Cruise", 13.073950, 80.270620);
            missionManager.AddWaypoint("Cruise", 13.075420, 80.273180);

            missionManager.AddSurveyPolygonPoint(13.076100, 80.275050);
            missionManager.AddSurveyPolygonPoint(13.076920, 80.275820);
            missionManager.AddSurveyPolygonPoint(13.077560, 80.276760);
            missionManager.AddSurveyPolygonPoint(13.076900, 80.277860);
            missionManager.AddSurveyPolygonPoint(13.075980, 80.277120);

            missionManager.AddWaypoint("Burst", 13.077980, 80.279540);
            missionManager.AddWaypoint("Burst", 13.076660, 80.280280);
            missionManager.AddWaypoint("Burst", 13.075780, 80.279420);

            missionManager.AddWaypoint("Return Cruise", 13.075980, 80.282640);
            missionManager.AddWaypoint("Return Cruise", 13.074280, 80.284160);
            missionManager.AddWaypoint("Return Cruise", 13.072880, 80.285840);
        }

        // ═══════════════════════════════════════════════════════════════
        // SURVEY GRID GENERATION  (Task 2 — full implementation)
        // ═══════════════════════════════════════════════════════════════

        /// <summary>
        /// Entry point wired to "Generate Survey" button and map tool.
        /// Reads the survey polygon boundary, generates a lawnmower pattern
        /// clipped strictly inside the polygon, and registers each sweep
        /// waypoint with the MissionManager.
        /// </summary>

        private void GenerateSimpleLinearSurvey()
        {
            IReadOnlyList<MissionPoint> pts =
                missionManager.GetSurveyPolygon();

            if (pts.Count < 2)
            {
                MessageBox.Show(
                    "Select exactly 2 points for Linear Survey.");
                return;
            }

            MissionStage surveyStage =
                missionManager.GetStage("Survey");

            surveyStage.Points.Clear();

            surveyOverlay.Routes.Clear();
            surveyOverlay.Markers.Clear();

            PointLatLng start =
                new PointLatLng(
                    pts[0].Latitude,
                    pts[0].Longitude);

            PointLatLng end =
                new PointLatLng(
                    pts[1].Latitude,
                    pts[1].Longitude);

            double hdg =
                CalculateBearing(
                    start.Lat,
                    start.Lng,
                    end.Lat,
                    end.Lng);

            surveyStage.Points.Add(
                new MissionPoint
                {
                    MissionType = "Survey",
                    PointNumber = 1,
                    Latitude = start.Lat,
                    Longitude = start.Lng,
                    AltitudeMeters = surveyStage.DefaultAltitudeMeters,
                    SpeedKnots = surveyStage.DefaultSpeedKnots,
                    HeadingDegrees = hdg
                });

            surveyStage.Points.Add(
                new MissionPoint
                {
                    MissionType = "Survey",
                    PointNumber = 2,
                    Latitude = end.Lat,
                    Longitude = end.Lng,
                    AltitudeMeters = surveyStage.DefaultAltitudeMeters,
                    SpeedKnots = surveyStage.DefaultSpeedKnots,
                    HeadingDegrees = hdg
                });

            GMapRoute route =
                new GMapRoute(
                    new List<PointLatLng>
                    {
                start,
                end
                    },
                    "LinearSurvey")
                {
                    Stroke = new Pen(
                        currentTheme.AccentYellow,
                        3)
                };

            surveyOverlay.Routes.Add(route);

            gmap.Refresh();

            RefreshWaypointGrid();
            RefreshMissionSummary();
        }

        private void GenerateGridSurvey()
        {
            MissionStage surveyStage =
                missionManager.GetStage("Survey");

            surveyStage.Points.Clear();

            surveyOverlay.Routes.Clear();
            surveyOverlay.Markers.Clear();

            IReadOnlyList<MissionPoint> surveyPts =
                missionManager.GetSurveyPolygon();

            if (surveyPts.Count < 3)
                return;

            double centLat =
                surveyPts.Average(p => p.Latitude);

            double centLon =
                surveyPts.Average(p => p.Longitude);

            List<PointD> poly =
                surveyPts
                .Select(p =>
                    LatLonToMeters(
                        p.Latitude,
                        p.Longitude,
                        centLat,
                        centLon))
                .ToList();
            double minX = poly.Min(p => p.X);
            double maxX = poly.Max(p => p.X);

            double minY = poly.Min(p => p.Y);
            double maxY = poly.Max(p => p.Y);

            double spacingMeters = 50;

            // HORIZONTAL STRIPS

            for (double y = minY;
                 y <= maxY;
                 y += spacingMeters)
            {
                PointD segA =
                    new PointD(minX, y);

                PointD segB =
                    new PointD(maxX, y);

                List<PointD> clipped =
                    ClipSegmentToPolygon(
                        segA,
                        segB,
                        poly);

                if (clipped.Count < 2)
                    continue;

                clipped.Sort(
                    (a, b) =>
                        a.X.CompareTo(b.X));

                PointLatLng startPoint =
    MetersToLatLon(
        clipped[0].X,
        clipped[0].Y,
        centLat,
        centLon);

                PointLatLng endPoint =
                    MetersToLatLon(
                        clipped[clipped.Count - 1].X,
                        clipped[clipped.Count - 1].Y,
                        centLat,
                        centLon);

                List<PointLatLng> line =
                    new List<PointLatLng>();

                line.Add(startPoint);
                line.Add(endPoint);

                surveyStage.Points.Add(
    new MissionPoint
    {
        MissionType = "Survey",
        PointNumber = surveyStage.Points.Count,
        Latitude = startPoint.Lat,
        Longitude = startPoint.Lng,
        AltitudeMeters = surveyStage.DefaultAltitudeMeters,
        SpeedKnots = surveyStage.DefaultSpeedKnots
    });

                surveyStage.Points.Add(
                    new MissionPoint
                    {
                        MissionType = "Survey",
                        PointNumber = surveyStage.Points.Count + 1,
                        Latitude = endPoint.Lat,
                        Longitude = endPoint.Lng,
                        AltitudeMeters = surveyStage.DefaultAltitudeMeters,
                        SpeedKnots = surveyStage.DefaultSpeedKnots
                    });

                GMapRoute route =
                    new GMapRoute(
                        line,
                        Guid.NewGuid().ToString());

                route.Stroke =
                    new Pen(
                        Color.Yellow,
                        1.5f);

                surveyOverlay.Routes.Add(route);
            }

            // VERTICAL STRIPS

            for (double x = minX;
                 x <= maxX;
                 x += spacingMeters)
            {
                PointD segA =
                    new PointD(x, minY);

                PointD segB =
                    new PointD(x, maxY);

                List<PointD> clipped =
                    ClipSegmentToPolygon(
                        segA,
                        segB,
                        poly);

                if (clipped.Count < 2)
                    continue;

                clipped.Sort(
                    (a, b) =>
                        a.Y.CompareTo(b.Y));

                List<PointLatLng> line =
                    new List<PointLatLng>();

                line.Add(
                    MetersToLatLon(
                        clipped[0].X,
                        clipped[0].Y,
                        centLat,
                        centLon));

                line.Add(
                    MetersToLatLon(
                        clipped[clipped.Count - 1].X,
                        clipped[clipped.Count - 1].Y,
                        centLat,
                        centLon));

                GMapRoute route =
                    new GMapRoute(
                        line,
                        Guid.NewGuid().ToString());

                route.Stroke =
                    new Pen(
                        Color.Yellow,
                        1.5f);

                surveyOverlay.Routes.Add(route);
            }

            gmap.Refresh();
        }

        private void GenerateCircularSurvey()
        {
            IReadOnlyList<MissionPoint> surveyPts =
                missionManager.GetSurveyPolygon();

            if (surveyPts.Count < 3)
                return;

            surveyOverlay.Routes.Clear();
            surveyOverlay.Markers.Clear();

            double centLat =
                surveyPts.Average(p => p.Latitude);

            double centLon =
                surveyPts.Average(p => p.Longitude);

            List<PointD> poly = surveyPts
                .Select(p => LatLonToMeters(
                    p.Latitude,
                    p.Longitude,
                    centLat,
                    centLon))
                .ToList();

            double minX = poly.Min(p => p.X);
            double maxX = poly.Max(p => p.X);

            double minY = poly.Min(p => p.Y);
            double maxY = poly.Max(p => p.Y);


            PointD center = new PointD(
    poly.Average(p => p.X),
    poly.Average(p => p.Y));

            TextBox txtDiameter =
    Controls.Find(
        "txtCircleDiameter",
        true)
    .FirstOrDefault() as TextBox;

            double diameterMeters = 300;

            if (txtDiameter != null)
            {
                double.TryParse(
                    txtDiameter.Text,
                    out diameterMeters);
            }


            double radius =
                diameterMeters / 2.0;

            List<PointLatLng> circle =
                new List<PointLatLng>();

            MissionStage surveyStage =
    missionManager.GetStage("Survey");

            surveyStage.Points.Clear();



            for (double angle = 0;
     angle <= Math.PI * 2;
     angle += 0.1)
            {
                double x =
                    center.X +
                    radius * Math.Cos(angle);

                double y =
                    center.Y +
                    radius * Math.Sin(angle);

                PointLatLng latLon =
                    MetersToLatLon(
                        x,
                        y,
                        centLat,
                        centLon);

                circle.Add(latLon);

                surveyStage.Points.Add(
                    new MissionPoint
                    {
                        MissionType = "Survey",
                        PointNumber =
                            surveyStage.Points.Count + 1,

                        Latitude =
                            latLon.Lat,

                        Longitude =
                            latLon.Lng,

                        AltitudeMeters =
                            surveyStage.DefaultAltitudeMeters,

                        SpeedKnots =
                            surveyStage.DefaultSpeedKnots
                    });
            }


            if (circle.Count > 1)
            {
                GMapRoute route =
                    new GMapRoute(
                        circle,
                        "CircleSurvey");

                route.Stroke =
                    new Pen(
                        Color.FromArgb(
                            220,
                            currentTheme.AccentYellow),
                        2f);

                surveyOverlay.Routes.Add(route);
            }

            RefreshWaypointGrid();

            RefreshMissionSummary();

            RefreshMapFromMission();

            gmap.Refresh();
        }
        private bool IsPointInsidePolygon(
    PointD point,
    List<PointD> polygon)
        {
            bool inside = false;

            for (int i = 0, j = polygon.Count - 1;
                 i < polygon.Count;
                 j = i++)
            {
                if (((polygon[i].Y > point.Y) !=
                     (polygon[j].Y > point.Y))
                    &&
                    (point.X <
                     (polygon[j].X - polygon[i].X)
                     * (point.Y - polygon[i].Y)
                     / (polygon[j].Y - polygon[i].Y)
                     + polygon[i].X))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        private void StartSimulation()
        {
            StopSimulation();

            simulationPoints =
                missionManager.GetAllWaypoints().ToList();
            for (int i = 0; i < simulationPoints.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"{i} : " +
                    $"{simulationPoints[i].Latitude}, " +
                    $"{simulationPoints[i].Longitude}");
            }

            if (simulationPoints.Count < 2)
            {
                MessageBox.Show("Need at least 2 waypoints.");
                return;
            }

            currentTargetIndex = 1;

            currentLat =
                simulationPoints[0].Latitude;

            currentLon =
                simulationPoints[0].Longitude;
            SetActiveStageIndicator(
    simulationPoints[0].MissionType);

            if (boatMarker != null)
                waypointOverlay.Markers.Remove(boatMarker);

            boatMarker =
    new GMarkerGoogle(
        new PointLatLng(
            currentLat,
            currentLon),
        GMarkerGoogleType.red);

            waypointOverlay.Markers.Add(
                boatMarker);

            if (simulationTimer == null)
            {
                simulationTimer = new Timer();
                simulationTimer.Interval = 100;
                simulationTimer.Tick +=
                    SimulationTimer_Tick;
            }
            simulationTimer.Start();
            simulationRunning = true;
            lblSimStatus.Text = "Idle";

            lblSimWaypoint.Text = "0 / 0";

            lblSimProgress.Text = "0%";

            lblEta.Text = "00:00:00";

            lblDistanceLeft.Text = "0.00 km";
            UpdateSimulationButtons();
        }

        private void SimulationTimer_Tick(
            object sender,
            EventArgs e)
        {
            this.Text =
    "Target = " +
    currentTargetIndex +
    "/" +
    simulationPoints.Count;

            if (currentTargetIndex >=
    simulationPoints.Count)
            {
                StopSimulation();
                UpdateSimulationButtons();
                return;
            }

            MissionPoint target =
    simulationPoints[
        currentTargetIndex];
            SetActiveStageIndicator(
    target.MissionType);

            double step = 0.00003;

            double dLat =
                target.Latitude - currentLat;

            double dLon =
                target.Longitude - currentLon;

            double distance =
                Math.Sqrt(
                    dLat * dLat +
                    dLon * dLon);

            if (distance < step)
            {
                currentLat =
                    target.Latitude;

                currentLon =
                    target.Longitude;

                currentTargetIndex++;
            }
            else
            {
                currentLat +=
                    dLat / distance * step;

                currentLon +=
                    dLon / distance * step;
            }
            lblSimStatus.Text =
    simulationPaused
        ? "Paused"
        : "Running";

            lblSimWaypoint.Text =
                $"{Math.Min(currentTargetIndex, simulationPoints.Count)} / {simulationPoints.Count}";

            double progress =
                ((double)currentTargetIndex /
                 simulationPoints.Count) * 100.0;

            lblSimProgress.Text =
                $"{progress:F0}%";
            double distanceLeft = 0;

            if (currentTargetIndex < simulationPoints.Count)
            {
                distanceLeft += distance;

                for (int i = currentTargetIndex;
                     i < simulationPoints.Count - 1;
                     i++)
                {
                    double dLat2 =
                        simulationPoints[i + 1].Latitude -
                        simulationPoints[i].Latitude;

                    double dLon2 =
                        simulationPoints[i + 1].Longitude -
                        simulationPoints[i].Longitude;

                    distanceLeft +=
                        Math.Sqrt(
                            dLat2 * dLat2 +
                            dLon2 * dLon2);
                }
            }

            lblDistanceLeft.Text =
                $"{distanceLeft:F4}";
            double ticksRemaining =
    distanceLeft / step;

            double secondsRemaining =
                ticksRemaining * 0.1;

            TimeSpan eta =
                TimeSpan.FromSeconds(
                    secondsRemaining);

            lblEta.Text =
                eta.ToString(@"hh\:mm\:ss");



            boatMarker.Position =
                new PointLatLng(
                    currentLat,
                    currentLon);

            gmap.Refresh();
        }
        private void UpdateSimulationButtons()
        {
            if (!simulationRunning)
            {
                simulationStartButton.Enabled = true;

                simulationPauseButton.Enabled = false;

                simulationClearButton.Enabled = false;
            }
            else
            {
                simulationStartButton.Enabled = false;

                simulationPauseButton.Enabled = true;

                simulationClearButton.Enabled = true;
            }
        }
        private void StopSimulation()
        {
            if (simulationTimer != null)
            {
                simulationTimer.Stop();
            }
            simulationPoints = new List<MissionPoint>();
            simulationRunning = false;
            simulationPaused = false;
            currentTargetIndex = 0;

            if (boatMarker != null)
            {
                waypointOverlay.Markers.Remove(boatMarker);
                boatMarker = null;
            }
            this.Text = "Vikra ASV Mission Planner";
            UpdateSimulationButtons();
            gmap?.Refresh();
        }
        private void GenerateRacetrackSurvey()
        {
            IReadOnlyList<MissionPoint> surveyPts =
                missionManager.GetSurveyPolygon();

            if (surveyPts.Count != 4)
            {
                MessageBox.Show(
                    "Racetrack requires exactly 4 points.");
                return;
            }

            surveyOverlay.Routes.Clear();
            surveyOverlay.Markers.Clear();

            double centLat =
                surveyPts.Average(p => p.Latitude);

            double centLon =
                surveyPts.Average(p => p.Longitude);

            List<PointD> pts = surveyPts
                .Select(p => LatLonToMeters(
                    p.Latitude,
                    p.Longitude,
                    centLat,
                    centLon))
                .ToList();

            PointD p1 = pts[0]; // top-left
            PointD p2 = pts[1]; // top-right
            PointD p3 = pts[2]; // bottom-right
            PointD p4 = pts[3]; // bottom-left

            List<PointLatLng> track =
                new List<PointLatLng>();

            // left side center
            PointD leftCenter = new PointD(
                (p1.X + p4.X) / 2.0,
                (p1.Y + p4.Y) / 2.0);

            // right side center
            PointD rightCenter = new PointD(
                (p2.X + p3.X) / 2.0,
                (p2.Y + p3.Y) / 2.0);

            double radius =
                Math.Sqrt(
                    Math.Pow(p1.X - leftCenter.X, 2) +
                    Math.Pow(p1.Y - leftCenter.Y, 2));

            // ---------- TOP STRAIGHT ----------

            track.Add(
                MetersToLatLon(
                    p1.X, p1.Y,
                    centLat, centLon));

            track.Add(
                MetersToLatLon(
                    p2.X, p2.Y,
                    centLat, centLon));

            // ---------- RIGHT HALF CIRCLE ----------

            for (double a = -90; a <= 90; a += 5)
            {
                double rad = a * Math.PI / 180.0;

                double x =
                    rightCenter.X +
                    radius * Math.Cos(rad);

                double y =
                    rightCenter.Y +
                    radius * Math.Sin(rad);

                track.Add(
                    MetersToLatLon(
                        x,
                        y,
                        centLat,
                        centLon));
            }

            // ---------- BOTTOM STRAIGHT ----------

            track.Add(
                MetersToLatLon(
                    p3.X, p3.Y,
                    centLat,
                    centLon));

            track.Add(
                MetersToLatLon(
                    p4.X, p4.Y,
                    centLat,
                    centLon));

            // ---------- LEFT HALF CIRCLE ----------

            for (double a = 90; a <= 270; a += 5)
            {
                double rad = a * Math.PI / 180.0;

                double x =
                    leftCenter.X +
                    radius * Math.Cos(rad);

                double y =
                    leftCenter.Y +
                    radius * Math.Sin(rad);

                track.Add(
                    MetersToLatLon(
                        x,
                        y,
                        centLat,
                        centLon));
            }

            // close loop
            track.Add(
                MetersToLatLon(
                    p1.X,
                    p1.Y,
                    centLat,
                    centLon));

            GMapRoute route =
                new GMapRoute(
                    track,
                    "RaceTrack");

            route.Stroke =
                new Pen(
                    Color.FromArgb(
                        220,
                        currentTheme.AccentYellow),
                    3f);

            surveyOverlay.Routes.Add(route);

            gmap.Refresh();
        }
        // ── Survey geometry helpers ─────────────────────────────────────

        /// <summary>Flat-earth projection: lat/lon → metres relative to origin.</summary>
        private static PointD LatLonToMeters(double lat, double lon, double originLat, double originLon)
        {
            const double earthR = 6378137.0;
            double x = (lon - originLon) * Math.PI / 180.0 * earthR * Math.Cos(originLat * Math.PI / 180.0);
            double y = (lat - originLat) * Math.PI / 180.0 * earthR;
            return new PointD(x, y);
        }

        /// <summary>Inverse flat-earth projection.</summary>
        private static PointLatLng MetersToLatLon(double x, double y, double originLat, double originLon)
        {
            const double earthR = 6378137.0;
            double lon = originLon + (x / (earthR * Math.Cos(originLat * Math.PI / 180.0))) * 180.0 / Math.PI;
            double lat = originLat + (y / earthR) * 180.0 / Math.PI;
            return new PointLatLng(lat, lon);
        }

        private static PointD RotatePoint(PointD p, double radians)
        {
            double cos = Math.Cos(radians), sin = Math.Sin(radians);
            return new PointD(p.X * cos - p.Y * sin, p.X * sin + p.Y * cos);
        }


        private static double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
        {
            double dLon = (lon2 - lon1) * Math.PI / 180.0;
            double rlat1 = lat1 * Math.PI / 180.0;
            double rlat2 = lat2 * Math.PI / 180.0;
            double y = Math.Sin(dLon) * Math.Cos(rlat2);
            double x = Math.Cos(rlat1) * Math.Sin(rlat2) - Math.Sin(rlat1) * Math.Cos(rlat2) * Math.Cos(dLon);
            return (Math.Atan2(y, x) * 180.0 / Math.PI + 360.0) % 360.0;
        }

        private static double ComputePolygonAreaSqMeters(List<PointD> poly)
        {
            double area = 0;
            int n = poly.Count;
            for (int i = 0; i < n; i++)
            {
                PointD a = poly[i], b = poly[(i + 1) % n];
                area += a.X * b.Y - b.X * a.Y;
            }
            return Math.Abs(area) / 2.0;
        }

        /// <summary>
        /// Clips a line segment (A→B) to the interior of a convex/non-convex polygon
        /// using Liang-Barsky-style edge intersection accumulation.
        /// Returns all intersection / interior points along the segment.
        /// </summary>
        private static List<PointD> ClipSegmentToPolygon(PointD segA, PointD segB, List<PointD> polygon)
        {
            List<double> tValues = new List<double> { 0.0, 1.0 };
            int n = polygon.Count;

            for (int i = 0; i < n; i++)
            {
                PointD e1 = polygon[i];
                PointD e2 = polygon[(i + 1) % n];

                double t;
                if (SegmentIntersectT(segA, segB, e1, e2, out t))
                {
                    if (t > 0.0 && t < 1.0) tValues.Add(t);
                }
            }

            tValues.Sort();

            List<PointD> result = new List<PointD>();
            for (int k = 0; k < tValues.Count - 1; k++)
            {
                double tMid = (tValues[k] + tValues[k + 1]) / 2.0;
                PointD midPt = new PointD(
                    segA.X + tMid * (segB.X - segA.X),
                    segA.Y + tMid * (segB.Y - segA.Y));

                if (IsPointInPolygon(midPt, polygon))
                {
                    // Include both endpoints of this sub-segment
                    PointD pA = new PointD(
                        segA.X + tValues[k] * (segB.X - segA.X),
                        segA.Y + tValues[k] * (segB.Y - segA.Y));
                    PointD pB = new PointD(
                        segA.X + tValues[k + 1] * (segB.X - segA.X),
                        segA.Y + tValues[k + 1] * (segB.Y - segA.Y));

                    if (result.Count == 0 || DistSq(result[result.Count - 1], pA) > 1e-10)
                        result.Add(pA);
                    result.Add(pB);
                }
            }
            return result;
        }

        private static bool SegmentIntersectT(PointD p1, PointD p2, PointD p3, PointD p4, out double t)
        {
            t = 0;
            double d1x = p2.X - p1.X, d1y = p2.Y - p1.Y;
            double d2x = p4.X - p3.X, d2y = p4.Y - p3.Y;
            double denom = d1x * d2y - d1y * d2x;
            if (Math.Abs(denom) < 1e-12) return false;

            double t1 = ((p3.X - p1.X) * d2y - (p3.Y - p1.Y) * d2x) / denom;
            double t2 = ((p3.X - p1.X) * d1y - (p3.Y - p1.Y) * d1x) / denom;
            t = t1;
            return t2 >= 0 && t2 <= 1;
        }

        private static bool IsPointInPolygon(PointD point, List<PointD> polygon)
        {
            bool inside = false;
            int n = polygon.Count;
            for (int i = 0, j = n - 1; i < n; j = i++)
            {
                PointD pi = polygon[i], pj = polygon[j];
                if (((pi.Y > point.Y) != (pj.Y > point.Y)) &&
                    (point.X < (pj.X - pi.X) * (point.Y - pi.Y) / (pj.Y - pi.Y) + pi.X))
                    inside = !inside;
            }
            return inside;
        }

        private static double DistSq(PointD a, PointD b)
        {
            double dx = a.X - b.X, dy = a.Y - b.Y;
            return dx * dx + dy * dy;
        }

        // ═══════════════════════════════════════════════════════════════
        // THEME
        // ═══════════════════════════════════════════════════════════════

        public void ApplyDarkTheme() { isDarkMode = true; currentTheme = darkTheme; ApplyTheme(); }
        public void ApplyLightTheme() { isDarkMode = false; currentTheme = lightTheme; ApplyTheme(); }
        public void ToggleTheme() { if (isDarkMode) ApplyLightTheme(); else ApplyDarkTheme(); }



        private void ApplyTheme()
        {
            BackColor = currentTheme.AppBackground;

            if (headerPanel != null)
                headerPanel.BackColor = currentTheme.HeaderBackground;

            if (statusBarPanel != null)
                statusBarPanel.BackColor = currentTheme.HeaderBackground;

            foreach (SectionPanel sp in sectionPanels)
                sp.Theme = currentTheme;

            foreach (Button b in accentButtons) b.FlatAppearance.BorderColor = currentTheme.Border;
            foreach (Button b in neutralButtons) { b.FlatAppearance.BorderColor = currentTheme.Border; b.BackColor = currentTheme.PanelAlt; }
            foreach (ComboBox c in comboBoxes) { c.BackColor = currentTheme.PanelAlt; c.ForeColor = currentTheme.TextPrimary; }

            themeToggle.Theme = currentTheme;
            themeToggle.Checked = isDarkMode;

            foreach (ThemeAwareControl ctrl in themeAwareControls) { ctrl.Theme = currentTheme; ctrl.Invalidate(); }

            dgvWaypoints.BackgroundColor = currentTheme.PanelBackground;
            dgvWaypoints.GridColor = currentTheme.Border;
            dgvWaypoints.DefaultCellStyle.BackColor = currentTheme.PanelBackground;
            dgvWaypoints.DefaultCellStyle.ForeColor = currentTheme.TextPrimary;
            dgvWaypoints.DefaultCellStyle.SelectionBackColor = ControlPaint.Light(currentTheme.PanelAlt);
            dgvWaypoints.DefaultCellStyle.SelectionForeColor = currentTheme.TextPrimary;
            dgvWaypoints.ColumnHeadersDefaultCellStyle.BackColor = currentTheme.PanelAlt;
            dgvWaypoints.ColumnHeadersDefaultCellStyle.ForeColor = currentTheme.TextPrimary;
            dgvWaypoints.RowTemplate.Height = 26;

            if (lblStatusBarReady != null)
                lblStatusBarReady.ForeColor = currentTheme.Success;

            if (lblStatusBarGps != null)
                lblStatusBarGps.ForeColor = currentTheme.Success;

            if (lblStatusBarBattery != null)
                lblStatusBarBattery.ForeColor = currentTheme.AccentYellow;

            if (lblStatusBarArmed != null)
                lblStatusBarArmed.ForeColor = currentTheme.Success;

            if (lblStatusPanelReady != null) lblStatusPanelReady.ForeColor = currentTheme.Success;
            if (lblStatusPanelGps != null) lblStatusPanelGps.ForeColor = currentTheme.Success;
            if (lblStatusPanelArmed != null) lblStatusPanelArmed.ForeColor = currentTheme.Success;

            RefreshStageSelectionVisuals();
            UpdateHeaderTabs();
            if (dataTabButtons.Count > 0)
            {
                KeyValuePair<string, Panel> visibleTab =
                    dataTabPanels.FirstOrDefault(item => item.Value.Visible);
                UpdateDataTabButtons(string.IsNullOrEmpty(visibleTab.Key) ? "Quick" : visibleTab.Key);
            }
            RefreshMapFromMission();
            foreach (Label lbl in themeLabels)
            {
                lbl.ForeColor = currentTheme.TextPrimary;
            }
            foreach (Label lbl in metricCaptionLabels)
            {
                lbl.ForeColor = currentTheme.TextSecondary;
            }
            foreach (Label lbl in metricValueLabels)
            {
                lbl.ForeColor = currentTheme.TextPrimary;
            }
            Invalidate(true);
        }

        // ═══════════════════════════════════════════════════════════════
        // MAP RENDERING
        // ═══════════════════════════════════════════════════════════════

        private void RefreshMapFromMission()
        {
            if (gmap == null) return;

            waypointOverlay.Markers.Clear();
            bool simulationRunning =
    boatMarker != null;
            routeOverlay.Routes.Clear();
            polygonOverlay.Polygons.Clear();
            polygonOverlay.Markers.Clear();

            AddStageRoute("Cruise", currentTheme.AccentBlue, GMarkerGoogleType.blue_small);
            AddStageRoute("Survey", currentTheme.AccentYellow, GMarkerGoogleType.yellow_small);
            AddStageRoute("Burst", currentTheme.AccentPurple, GMarkerGoogleType.purple_small);
            AddStageRoute("Return Cruise", currentTheme.Success, GMarkerGoogleType.green_small);
            DrawMasterMissionRoute();

            IReadOnlyList<MissionPoint> surveyPts = missionManager.GetSurveyPolygon();
            if (surveyPts.Count >= 2)
            {
                List<PointLatLng> polygonPts = surveyPts.Select(p => new PointLatLng(p.Latitude, p.Longitude)).ToList();
                GMapPolygon polygon = new GMapPolygon(polygonPts, "SurveyPolygon")
                {
                    Stroke = new Pen(currentTheme.AccentYellow, 2F),
                    Fill = new SolidBrush(Color.FromArgb(30, currentTheme.AccentYellow))
                };
                polygonOverlay.Polygons.Add(polygon);

                foreach (MissionPoint pt in surveyPts)
                {
                    GMarkerGoogle marker = new GMarkerGoogle(
    new PointLatLng(pt.Latitude, pt.Longitude),
    GMarkerGoogleType.yellow_small);

                    marker.ToolTipMode =
                        gmap.Zoom >= 17
                            ? MarkerTooltipMode.Always
                            : MarkerTooltipMode.Never;

                    marker.ToolTipText = "L" + pt.PointNumber;
                    marker.Tag = pt;

                    if (gmap.Zoom >= 17)
                    {
                        waypointOverlay.Markers.Add(marker);
                    }
                }
            }
            if (simulationRunning &&
    boatMarker != null)
            {
                waypointOverlay.Markers.Add(
                    boatMarker);
            }
            gmap.Refresh();
        }

        private void AddStageRoute(string stageName, Color color, GMarkerGoogleType markerType)
        {
            MissionStage stage = missionManager.GetStage(stageName);
            if (stage.Points.Count == 0) return;

            List<PointLatLng> points = new List<PointLatLng>();
            foreach (MissionPoint pt in stage.Points)
            {
                PointLatLng mp =
        new PointLatLng(
            pt.Latitude,
            pt.Longitude);

                points.Add(mp);

                string label = pt.DisplayLabel;
                if (stageName == "Cruise")
                {
                    if (pt ==
                        stage.Points.Last())
                    {
                        MissionStage surveyStage =
                            missionManager.GetStage("Survey");

                        if (surveyStage.Points.Count > 0)
                        {
                            label += "/S1";
                        }
                    }
                }

                GMarkerGoogle marker =
    new GMarkerGoogle(mp, markerType);

                marker.ToolTipText = label;
                marker.Tag = pt;

                if (gmap.Zoom >= 17)
                {
                    marker.ToolTipMode = MarkerTooltipMode.Always;
                }
                else
                {
                    marker.ToolTipMode = MarkerTooltipMode.Never;
                }

                waypointOverlay.Markers.Add(marker);
            }
            if (points.Count >= 2)
                routeOverlay.Routes.Add(new GMapRoute(points, stageName) { Stroke = new Pen(color, 2.2F) });
        }

        private void DrawMasterMissionRoute()
        {
            List<PointLatLng> routePoints = new List<PointLatLng>();
            MissionStage cruise =
    missionManager.GetStage("Cruise");

            MissionStage survey =
                missionManager.GetStage("Survey");

            MissionStage burst =
                missionManager.GetStage("Burst");

            MissionStage ret =
                missionManager.GetStage("Return Cruise");
            foreach (MissionPoint pt in cruise.Points)
            {
                routePoints.Add(
                    new PointLatLng(
                        pt.Latitude,
                        pt.Longitude));
            }
            foreach (MissionPoint pt in survey.Points)
            {
                routePoints.Add(
                    new PointLatLng(
                        pt.Latitude,
                        pt.Longitude));
            }
            foreach (MissionPoint pt in burst.Points)
            {
                routePoints.Add(
                    new PointLatLng(
                        pt.Latitude,
                        pt.Longitude));
            }
            foreach (MissionPoint pt in ret.Points)
            {
                routePoints.Add(
                    new PointLatLng(
                        pt.Latitude,
                        pt.Longitude));
            }
            if (routePoints.Count >= 2)
            {
                GMapRoute masterRoute =
                    new GMapRoute(routePoints, "MasterMission");

                masterRoute.Stroke =
                    new Pen(Color.White, 3f);

                routeOverlay.Routes.Add(masterRoute);
            }
        }


        // ═══════════════════════════════════════════════════════════════
        // DATA REFRESH
        // ═══════════════════════════════════════════════════════════════

        private void RefreshWaypointGrid()
        {
            dgvWaypoints.Rows.Clear();
            foreach (MissionPoint pt in missionManager.GetAllWaypoints())
            {
                int r = dgvWaypoints.Rows.Add(
                    pt.MissionType, pt.DisplayLabel,
                    pt.Latitude.ToString("F6"), pt.Longitude.ToString("F6"),
                    pt.AltitudeMeters.ToString("F1") + " m",
                    pt.SpeedKnots.ToString("F1") + " kn",
                    pt.HeadingDegrees.ToString("F0") + " deg", "X");
                dgvWaypoints.Rows[r].Tag = pt;
            }
        }

        private void RefreshMissionSummary()
        {
            if (lblSummaryWaypointsValue == null) return;

            int wc = missionManager.GetTotalWaypointCount();
            if (lblCruiseStageSubtitle != null)
            {
                lblCruiseStageSubtitle.Text =
                    wc + " waypoints";
            }
            double dist = missionManager.GetTotalDistanceKm();
            TimeSpan dur = missionManager.GetEstimatedDuration();

            //lblSummaryWaypointsValue.Text = wc.ToString();
            //lblSummaryDistanceValue.Text = dist.ToString("F1") + " km";
            //lblSummaryDurationValue.Text = dur.ToString(@"hh\:mm\:ss");
            //lblSummaryFuelValue.Text = "68 %";

            //lblStatusBarMode.Text = surveyPolygonMode ? "Mode: SURVEY POLYGON" : "Mode: " + selectedStageName.ToUpperInvariant();
            if (lblStatusPanelStage != null) lblStatusPanelStage.Text = selectedStageName.ToUpperInvariant();
        }

        private void RefreshStageSelectionVisuals()
        {
            foreach (KeyValuePair<string, Button> entry in stageButtons)
            {
                bool sel = string.Equals(entry.Key, selectedStageName, StringComparison.OrdinalIgnoreCase);
                entry.Value.FlatAppearance.BorderColor = sel ? currentTheme.AccentBlue : currentTheme.Border;
                entry.Value.FlatAppearance.BorderSize = sel ? 2 : 1;
            }
            if (lblSelectedStageValue != null) lblSelectedStageValue.Text = selectedStageName.ToUpperInvariant();
            if (lblPatternStatusValue != null) lblPatternStatusValue.Text = surveyPolygonMode ? "SURVEY POLYGON CAPTURE" : "WAYPOINT CAPTURE";
            if (lblMissionModeValue != null) lblMissionModeValue.Text = surveyPolygonMode ? "LEFT CLICK ADDS POLYGON VERTICES" : "LEFT CLICK ADDS WAYPOINTS";
            foreach (Button card in stageButtons.Values)
            {
                card.BackColor = currentTheme.PanelAlt;

                foreach (Control c in card.Controls)
                {
                    if (c.Name == "StageTitle")
                        c.ForeColor = currentTheme.TextPrimary;

                    else if (c.Name == "StageDetail")
                        c.ForeColor = currentTheme.TextSecondary;

                    else if (c.Name == "StageSpeed")
                        c.ForeColor = currentTheme.TextPrimary;
                }
            }
        }


        private void CenterMapOnMission()
        {
            IReadOnlyList<MissionPoint> pts = missionManager.GetAllWaypoints();
            gmap.Position = pts.Count == 0
                ? new PointLatLng(13.074560, 80.270980)
                : new PointLatLng(pts.Average(p => p.Latitude), pts.Average(p => p.Longitude));
        }

        private void SelectStage(string stageName)
        {
            selectedStageName = stageName;
            MessageBox.Show(stageName);
            if (!string.Equals(stageName, "Survey", StringComparison.OrdinalIgnoreCase))
                surveyPolygonMode = false;
            if (cmbPattern != null)
            {
                MessageBox.Show(
                    "Before: " +
                    (cmbPattern.SelectedItem?.ToString() ?? "NULL"));
                MessageBox.Show("Stage = " + stageName);
                switch (stageName.ToUpperInvariant())
                {
                    case "CRUISE":
                        cmbPattern.SelectedItem = "Linear";
                        break;

                    case "SURVEY":
                    case "LOITER":
                        cmbPattern.SelectedItem = "Grid";
                        break;

                    case "BURST":
                        cmbPattern.SelectedItem = "Linear";
                        break;

                    case "RETURN CRUISE":
                        cmbPattern.SelectedItem = "Linear";
                        break;
                }

                MessageBox.Show(
                    "After: " +
                    (cmbPattern.SelectedItem?.ToString() ?? "NULL"));
            }

            RefreshStageSelectionVisuals();
            RefreshMissionSummary();
        }


        // ═══════════════════════════════════════════════════════════════
        // EVENT HANDLERS
        // ═══════════════════════════════════════════════════════════════

        private void ToggleSurveyPolygonMode(object sender, EventArgs e)
        {
            SelectStage("Survey");
            surveyPolygonMode = !surveyPolygonMode;
            RefreshStageSelectionVisuals();
            RefreshMissionSummary();
        }


        private void CmbPattern_SelectedIndexChanged(object sender, EventArgs e)
        {
            MissionStage surveyStage = missionManager.GetStage("Survey");

            ComboBox cmbPattern = sender as ComboBox;

            if (cmbPattern != null && cmbPattern.SelectedItem != null)
            {
                surveyStage.SurveyPattern = cmbPattern.SelectedItem.ToString();
            }
        }


        private async void UploadMissionButton_Click(object sender, EventArgs e)
        {
            try
            {


                MessageBox.Show("Uploading Mission...");
                bool uploaded =
                    await missionPlannerAdapter.UploadMissionAsync(
                        missionManager.MissionPlan);



                if (lblStatusPanelReady != null && currentTheme != null)
                {
                    lblStatusPanelReady.Text =
                        uploaded ? "Mission Uploaded" : "Upload Failed";

                    lblStatusPanelReady.ForeColor =
                        uploaded ? currentTheme.Success :
                                   currentTheme.AccentRed;
                }

                if (lblStatusPanelReady != null)
                {
                    if (lblStatusBarReady != null)
                    {
                        lblStatusBarReady.Text =
                            uploaded ? "Mission Uploaded" : "Upload Failed";

                        lblStatusBarReady.ForeColor =
                            uploaded ? currentTheme.Success :
                                       currentTheme.AccentRed;
                    }

                    lblStatusPanelReady.ForeColor =
                        uploaded ? currentTheme.Success :
                                   currentTheme.AccentRed;
                }



                if (uploaded)
                {
                    MessageBox.Show("Mission Uploaded");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "UPLOAD DEBUG");
            }
        }
        private void ValidateButton_Click(object sender, EventArgs e)
        {
            int polyPts = missionManager.GetSurveyPolygon().Count;
            string msg = polyPts >= 3
                ? "Mission looks valid. Survey polygon captured."
                : "Mission valid for waypoint routing. Add at least 3 polygon points for survey capture.";
            MessageBox.Show(this, msg, "Mission Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void GenerateSurveyButton_Click(object sender, EventArgs e)
        {
            string pattern =
                missionManager.GetStage("Survey").SurveyPattern;

            switch (pattern)
            {
                case "Linear":
                    GenerateSimpleLinearSurvey();
                    break;

                case "Grid":
                    GenerateGridSurvey();
                    break;

                case "Circular":
                    GenerateCircularSurvey();
                    break;

                case "Racetrack":
                    MessageBox.Show("RACETRACK SELECTED");
                    GenerateRacetrackSurvey();
                    break;

                default:
                    GenerateSimpleLinearSurvey();
                    break;
            }
        }
        private void PlaceholderButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "This action is prepared for backend integration in a later phase.", "Action Ready", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ClearAllButton_Click(object sender, EventArgs e)
        {
            missionManager.ClearMission();

            waypointOverlay?.Markers.Clear();

            routeOverlay?.Routes.Clear();

            polygonOverlay?.Polygons.Clear();
            polygonOverlay?.Markers.Clear();

            surveyOverlay?.Routes.Clear();
            surveyOverlay?.Markers.Clear();

            if (lblSurveyLinesValue != null)
                lblSurveyLinesValue.Text = "—";

            if (lblSurveyCoverageValue != null)
                lblSurveyCoverageValue.Text = "—";

            gmap?.Refresh();
        }

        private void DgvWaypoints_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dgvWaypoints.Columns[e.ColumnIndex].Name != "Delete")
                return;

            MissionPoint pt = dgvWaypoints.Rows[e.RowIndex].Tag as MissionPoint;

            if (pt == null)
                return;

            missionManager.RemoveWaypoint(pt.Id);

            RefreshWaypointGrid();
            RefreshMissionSummary();
            RefreshMapFromMission();


            gmap?.Refresh();
        }


        private void DgvWaypoints_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show("Edit Waypoint coming next");
        }

        private void Gmap_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PointLatLng pt = gmap.FromLocalToLatLng(e.X, e.Y);
                if (surveyPolygonMode)
                    missionManager.AddSurveyPolygonPoint(pt.Lat, pt.Lng);
                else
                    missionManager.AddWaypoint(selectedStageName, pt.Lat, pt.Lng);
            }
            else if (e.Button == MouseButtons.Right && surveyPolygonMode && missionManager.GetSurveyPolygon().Count >= 3)
            {
                surveyPolygonMode = false;
                RefreshStageSelectionVisuals();
                RefreshMissionSummary();
                // Auto-generate on right-click close
                GenerateSimpleLinearSurvey();

            }
        }

        private void Gmap_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            selectedWaypoint = item.Tag as MissionPoint;

            if (selectedWaypoint == null)
                return;

            waypointActionMenu.Show(Cursor.Position);
        }

        private void MissionManager_MissionChanged(object sender, EventArgs e)
        {
            RefreshWaypointGrid();
            RefreshMissionSummary();
            RefreshMapFromMission();

            MissionStage cruiseStage =
                missionManager.GetStage("Cruise");

            if (lblCruiseStageSubtitle != null)
            {
                lblCruiseStageSubtitle.Text =
                    cruiseStage.Points.Count + " waypoints";
            }

            MissionStage returnStage =
                missionManager.GetStage("Return Cruise");

            if (lblReturnCruiseStageSubtitle != null)
            {
                lblReturnCruiseStageSubtitle.Text =
                    returnStage.Points.Count + " waypoints";
            }
            if (lblCruiseSpeed != null)
            {
                lblCruiseSpeed.Text = "12.0 kn";
            }

            if (lblLoiterSpeed != null)
            {
                lblLoiterSpeed.Text = "4.0 kn";
            }

            if (lblBurstSpeed != null)
            {
                lblBurstSpeed.Text = "25.0 kn";
            }

            if (lblReturnCruiseSpeed != null)
            {
                lblReturnCruiseSpeed.Text = "12.0 kn";
            }
        }

        private void ThemeToggle_ToggleChanged(object sender, EventArgs e) => ToggleTheme();
        private void ClockTimer_Tick(object sender, EventArgs e) => UpdateUtcClock();

        private void HudTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(
    "Packets = " +
    MainV2.comPort.MAV.packetsnotlost);
            System.Diagnostics.Debug.WriteLine(
    $"Roll={MainV2.comPort.MAV.cs.roll}  " +
    $"Pitch={MainV2.comPort.MAV.cs.pitch}  " +
    $"Yaw={MainV2.comPort.MAV.cs.yaw}");

            if (MainV2.comPort == null)
                return;

            if (MainV2.comPort.BaseStream == null)
                return;

            if (!MainV2.comPort.BaseStream.IsOpen)
                return;
            Console.WriteLine(
    $"Time={DateTime.Now:HH:mm:ss.fff}");

            Console.WriteLine(
$"{DateTime.Now:HH:mm:ss} | " +
$"Packets={MainV2.comPort.MAV.packetsnotlost} | " +
$"Roll={MainV2.comPort.MAV.cs.roll:F2} | " +
$"Pitch={MainV2.comPort.MAV.cs.pitch:F2} | " +
$"Yaw={MainV2.comPort.MAV.cs.yaw:F2}");

            var cs = MainV2.comPort.MAV.cs;
            System.Diagnostics.Debug.WriteLine(
    $"Roll={cs.roll} Pitch={cs.pitch} Yaw={cs.yaw}");

            hud1.roll = (float)cs.roll;
            hud1.pitch = (float)cs.pitch;

            hud1.heading = (float)cs.yaw;

            hud1.alt = (float)cs.alt;

            hud1.groundspeed = (float)cs.groundspeed;

            hud1.batteryremaining = cs.battery_remaining;

            hud1.batterylevel = (float)cs.battery_voltage;

            hud1.mode = cs.mode;

            hud1.connected = true;

            if (dataValueLabels.ContainsKey("RollValue"))
            {
                dataValueLabels["RollValue"].Text =
                    $"{cs.roll:F1}°";
            }

            if (dataValueLabels.ContainsKey("PitchValue"))
            {
                dataValueLabels["PitchValue"].Text =
                    $"{cs.pitch:F1}°";
            }

            if (dataValueLabels.ContainsKey("YawValue"))
            {
                dataValueLabels["YawValue"].Text =
                    $"{cs.yaw:F1}°";
            }

            if (dataValueLabels.ContainsKey("SpeedValue"))
            {
                dataValueLabels["SpeedValue"].Text =
                    $"{cs.groundspeed:F1} kn";
            }


            hud1.Invalidate();
        }


        private void UpdateUtcClock()
        {
            if (lblHeaderTimeValue != null)
            {
                lblHeaderTimeValue.Text =
                    DateTime.UtcNow.ToString("HH:mm:ss 'UTC'");
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // FACTORY HELPERS
        // ═══════════════════════════════════════════════════════════════

        private SectionPanel CreateSection(string title)
        {
            SectionPanel panel = new SectionPanel
            {
                Dock = DockStyle.Fill,
                Title = title,
                Theme = currentTheme,
                Margin = new Padding(0, 0, 0, 8)
            };
            sectionPanels.Add(panel);
            themeAwareControls.Add(panel);
            return panel;
        }

        /// <summary>Creates a titled sub-block used inside the right panel sections.</summary>
        private Panel CreateSubSection(string title)
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 0, 0, 2) };

            Label heading = CreateLabel(title.ToUpperInvariant(), 8F, FontStyle.Bold, currentTheme.TextMuted);
            heading.Dock = DockStyle.Top;
            heading.Height = 18;
            heading.AutoSize = false;
            heading.Padding = new Padding(2, 0, 0, 0);

            Panel inner = new Panel { Dock = DockStyle.Fill };
            panel.Controls.Add(inner);
            panel.Controls.Add(heading);

            // Return inner so callers add children to the inner panel
            // We store heading in the outer for layout; return inner.
            // Re-design: make outer a TableLayoutPanel to give heading fixed height
            Panel wrapper = new Panel { Dock = DockStyle.Fill };
            TableLayoutPanel tl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            tl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tl.RowStyles.Add(new RowStyle(SizeType.Absolute, 18F));
            tl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            Label headingInner = CreateLabel(title.ToUpperInvariant(), 7.5F, FontStyle.Bold, currentTheme.TextMuted);
            headingInner.Dock = DockStyle.Fill;
            headingInner.AutoSize = false;
            headingInner.Padding = new Padding(2, 2, 0, 0);
            headingInner.TextAlign = ContentAlignment.MiddleLeft;

            // Bottom border line
            Panel linePanel = new Panel { Dock = DockStyle.Fill };
            tl.Controls.Add(headingInner, 0, 0);
            tl.Controls.Add(linePanel, 0, 1);

            wrapper.Controls.Add(tl);
            return linePanel;   // callers add their grid into linePanel (== inner content area)
        }

        private FlowLayoutPanel CreateHeaderStatus(string title, string value, Color accent, bool isClock = false)
        {
            FlowLayoutPanel w = new FlowLayoutPanel
            {
                Width = isClock ? 150 : 82,
                Height = 52,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Margin = new Padding(0, 0, 8, 0)
            };
            Label tl = CreateLabel(title, 7F, FontStyle.Regular, currentTheme.TextMuted);
            tl.Margin = new Padding(0, 0, 0, 1);
            Label vl = CreateLabel(value, 9.5F, FontStyle.Bold, accent);
            if (isClock) lblHeaderTimeValue = vl;
            w.Controls.Add(tl); w.Controls.Add(vl);
            return w;
        }

        private Button CreateHeaderTab(string text, bool active = false)
        {
            Button btn = new Button
            {
                Text = text,
                Width = 100,
                Height = 42,
                FlatStyle = FlatStyle.Flat,
                BackColor = currentTheme.HeaderBackground,
                ForeColor = active ? currentTheme.AccentBlue : currentTheme.TextPrimary,
                Font = new Font("Segoe UI", 9F,
                    active ? FontStyle.Bold : FontStyle.Regular),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 4, 8, 0),
                TabStop = false
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor =
    ControlPaint.Light(currentTheme.HeaderBackground);

            btn.FlatAppearance.MouseDownBackColor =
                ControlPaint.Dark(currentTheme.HeaderBackground);

            return btn;
        }

        private Button CreateHeaderTab(string text, AppPage page)
        {
            Button btn = CreateHeaderTab(text, page == currentPage);
            btn.Click += (s, e) => SwitchPage(page);
            headerTabs[text] = btn;
            return btn;
        }

        private void SwitchPage(AppPage page)
        {
            currentPage = page;

            if (mapSection != null)
            {
                Panel targetHost =
                    page == AppPage.Data
                        ? dataMapHost
                        : page == AppPage.Simulation
                            ? simMapHost
                            : missionMapHost;

                if (targetHost != null &&
                    mapSection.Parent != targetHost)
                {
                    mapSection.Parent?.Controls.Remove(mapSection);
                    targetHost.Controls.Add(mapSection);
                }
            }

            if (targetModePage != null)
            {
                targetModePage.Visible =
                    page == AppPage.TargetMode;
            }
            if (missionPage != null)
            {
                missionPage.Visible =
                    page == AppPage.Mission;
            }

            if (dataPage != null)
            {
                dataPage.Visible =
                    page == AppPage.Data;
            }

            if (simulationPage != null)
            {
                simulationPage.Visible =
                    page == AppPage.Simulation;
            }
            if (helpPage != null)
            {
                helpPage.Visible =
                    page == AppPage.Help;
            }
            if (targetModePage != null)
            {
                targetModePage.Visible =
                    page == AppPage.TargetMode;
            }

            UpdateHeaderTabs();
        }

        private void UpdateHeaderTabs()
        {
            foreach (KeyValuePair<string, Button> tab in headerTabs)
            {
                bool active = false;

                switch (currentPage)
                {
                    case AppPage.Mission:
                        active = tab.Key == "MISSION";
                        break;

                    case AppPage.Data:
                        active = tab.Key == "DATA";
                        break;

                    case AppPage.Simulation:
                        active = tab.Key == "SIMULATION";
                        break;

                    case AppPage.TargetMode:
                        active = tab.Key == "TARGET MODE";
                        break;
                    case AppPage.Help:
                        active = tab.Key == "HELP";
                        break;
                }

                tab.Value.ForeColor =
                    active ? currentTheme.AccentBlue :
                    currentTheme.TextPrimary;

                tab.Value.Font =
                    new Font(
                        "Segoe UI",
                        9F,
                        active
                            ? FontStyle.Bold
                            : FontStyle.Regular);
            }
        }

        private Button CreateStageCard(string stageName, string subtext, string speed, Color accent, int cardWidth)
        {
            Button card = CreateButton(string.Empty, currentTheme.PanelAlt, currentTheme.TextPrimary, cardWidth, 45, false);
            card.TextAlign = ContentAlignment.MiddleLeft;
            card.Padding = new Padding(10, 0, 10, 0);
            card.Margin = new Padding(0, 0, 0, 6);
            card.FlatAppearance.BorderSize = 1;
            card.Click += (s, e) => SelectStage(stageName);

            Label badge = CreateBadgeLabel(stageName.Substring(0, 1).ToUpperInvariant(), accent, new Size(22, 22));
            badge.Location = new Point(10, 11);
            card.Controls.Add(badge);
            string displayName =
    stageName == "Burst"
        ? "BURST (OPTIONAL)"
        : stageName.ToUpperInvariant();

            Label titleLbl = CreateLabel(
                displayName,
                9.5F,
                FontStyle.Bold,
                currentTheme.TextPrimary);
            titleLbl.Name = "StageTitle";
            titleLbl.Location = new Point(38, 5);
            card.Controls.Add(titleLbl);
            Label detailLbl = CreateLabel(subtext, 8.5F, FontStyle.Regular, currentTheme.TextSecondary);

            if (stageName == "Cruise")
            {
                lblCruiseStageSubtitle = detailLbl;
            }
            else if (stageName == "Loiter")
            {
                lblLoiterStageSubtitle = detailLbl;
            }
            else if (stageName == "Burst")
            {
                lblBurstStageSubtitle = detailLbl;
            }
            else if (stageName == "Return Cruise")
            {
                lblReturnCruiseStageSubtitle = detailLbl;
            }

            detailLbl.Name = "StageDetail";
            detailLbl.Location = new Point(38, 22);
            card.Controls.Add(detailLbl);
            Label speedLbl = CreateLabel(speed, 9F, FontStyle.Bold, currentTheme.TextPrimary);
            speedLbl.Name = "StageSpeed";
            speedLbl.Location = new Point(cardWidth - 72, 13);
            card.Controls.Add(speedLbl);
            if (stageName == "Cruise")
            {
                lblCruiseSpeed = speedLbl;
            }
            else if (stageName == "Loiter")
            {
                lblLoiterSpeed = speedLbl;
            }
            else if (stageName == "Burst")
            {
                lblBurstSpeed = speedLbl;
            }
            else if (stageName == "Return Cruise")
            {
                lblReturnCruiseSpeed = speedLbl;
            }
            stageButtons[stageName] = card;
            return card;
        }

        private Panel CreateInfoField(string labelText, out Label valueLabel, string valueText, int fieldWidth)
        {
            Panel panel = new Panel { Width = fieldWidth, Height = 42, Margin = new Padding(0, 0, 0, 4) };
            Label lbl = CreateLabel(labelText, 8.5F, FontStyle.Regular, currentTheme.TextSecondary);
            lbl.Location = new Point(0, 0);
            themeLabels.Add(lbl);
            panel.Controls.Add(lbl);
            RoundedPanel box = new RoundedPanel
            {
                Theme = currentTheme,
                FillColor = currentTheme.PanelAlt,
                Radius = 6,
                Location = new Point(0, 17),
                Size = new Size(fieldWidth, 24)
            };
            themeAwareControls.Add(box);
            valueLabel = CreateLabel(valueText, 9F, FontStyle.Bold, currentTheme.TextPrimary);
            valueLabel.Location = new Point(8, 4);
            themeLabels.Add(valueLabel);
            box.Controls.Add(valueLabel);
            panel.Controls.Add(box);
            return panel;
        }

        private Panel CreateInfoField(string labelText, Label unused, string valueText, int fieldWidth)
        {
            Label ign; return CreateInfoField(labelText, out ign, valueText, fieldWidth);
        }

        private Panel CreateLabeledNumeric(string labelText, out NumericUpDown nud, int min, int max, int defaultVal, int fieldWidth)
        {
            Panel panel = new Panel { Width = fieldWidth, Height = 46, Margin = new Padding(0, 0, 0, 6) };
            Label lbl = CreateLabel(labelText, 8.5F, FontStyle.Regular, currentTheme.TextSecondary);
            lbl.Location = new Point(0, 0);
            panel.Controls.Add(lbl);
            nud = new NumericUpDown
            {
                Minimum = min,
                Maximum = max,
                Value = defaultVal,
                Location = new Point(0, 17),
                Size = new Size(fieldWidth, 26),
                BackColor = currentTheme.PanelAlt,
                ForeColor = currentTheme.TextPrimary,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            panel.Controls.Add(nud);
            return panel;
        }

        private Button CreateButton(string text, Color backColor, Color foreColor, int width, int height, bool accent)
        {
            Button button = new Button
            {
                Text = text,
                Width = width > 0 ? width : 80,
                Height = height > 0 ? height : 32,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = currentTheme.Border;
            button.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor);
            button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor);
            if (accent) accentButtons.Add(button); else neutralButtons.Add(button);
            return button;
        }

        /// <summary>Full-width action button for the right-panel Actions section.</summary>
        private Button CreateActionBtn(string text, Color backColor, Color foreColor)
        {
            Button btn = CreateButton(text, backColor, foreColor, 0, 0, backColor != currentTheme.PanelAlt);
            btn.Dock = DockStyle.Fill;
            btn.Margin = new Padding(2, 2, 2, 2);
            return btn;
        }

        private Label CreateBadgeLabel(string text, Color backColor, Size size)
        {
            return new Label
            {
                Text = text,
                AutoSize = false,
                Size = size,
                BackColor = backColor,
                ForeColor = currentTheme.TextPrimary,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private Label CreateLabel(string text, float size, FontStyle style, Color color)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = color,
                Font = new Font("Segoe UI", size, style)
            };
        }

        private Label CreateSubheading(string text, int width)
        {
            Label label = CreateLabel(text, 9.5F, FontStyle.Bold, currentTheme.TextPrimary);
            label.AutoSize = false; label.Width = width; label.Height = 18;
            label.Margin = new Padding(0, 8, 0, 4);
            themeLabels.Add(label);
            return label;
        }

        private Label CreateTabLabel(string text, int x, bool active)
        {
            Label label = CreateLabel(text, 9.5F, active ? FontStyle.Bold : FontStyle.Regular,
                active ? currentTheme.AccentBlue : currentTheme.TextSecondary);
            label.Location = new Point(x, 6);
            return label;
        }

        private Label CreateMetricLabel(string text)
        {
            Label label = CreateLabel(
        text,
        9F,
        FontStyle.Regular,
        currentTheme.TextSecondary);

            label.AutoSize = false;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Margin = Padding.Empty;
            metricCaptionLabels.Add(label);

            return label;
        }

        private Label CreateMetricValue(string text)
        {
            Label label = CreateLabel(
                text,
                10.5F,
                FontStyle.Bold,
                currentTheme.TextPrimary);

            label.AutoSize = false;
            label.Dock = DockStyle.Fill;
            label.TextAlign = ContentAlignment.MiddleRight;
            label.Margin = Padding.Empty;
            metricValueLabels.Add(label);

            return label;
        }
        private Label CreateStatusBarLabel(string text)
        {
            Label label = CreateLabel(text, 8.5F, FontStyle.Regular, currentTheme.TextPrimary);
            label.Dock = DockStyle.Fill; label.TextAlign = ContentAlignment.MiddleLeft;
            return label;
        }

        /// <summary>Creates a small two-line telemetry cell (title + value).</summary>
        private Panel CreateTelemetryCell(string title, string value, Color valueColor)
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(2) };
            RoundedPanel box = new RoundedPanel
            {
                Theme = currentTheme,
                FillColor = currentTheme.PanelAlt,
                Radius = 5,
                Dock = DockStyle.Fill
            };
            themeAwareControls.Add(box);

            Label titleLbl = CreateLabel(title, 7F, FontStyle.Regular, currentTheme.TextMuted);
            titleLbl.Location = new Point(5, 3);
            box.Controls.Add(titleLbl);

            Label valueLbl = CreateLabel(value, 9F, FontStyle.Bold, valueColor);
            valueLbl.Location = new Point(5, 16);
            box.Controls.Add(valueLbl);

            panel.Controls.Add(box);
            return panel;
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// GEOMETRY VALUE TYPE
// ═══════════════════════════════════════════════════════════════════

internal struct PointD
{
    public double X;
    public double Y;
    public PointD(double x, double y) { X = x; Y = y; }
    public override string ToString() => $"({X:F2}, {Y:F2})";
}

// ═══════════════════════════════════════════════════════════════════
// THEME COLORS
// ═══════════════════════════════════════════════════════════════════

internal sealed class ThemeColors
{
    public Color AppBackground { get; set; }
    public Color HeaderBackground { get; set; }
    public Color PanelBackground { get; set; }
    public Color PanelAlt { get; set; }
    public Color Border { get; set; }
    public Color TextPrimary { get; set; }
    public Color TextSecondary { get; set; }
    public Color TextMuted { get; set; }
    public Color AccentBlue { get; set; }
    public Color Success { get; set; }
    public Color AccentYellow { get; set; }
    public Color AccentPurple { get; set; }
    public Color AccentRed { get; set; }

    public static ThemeColors CreateDark() => new ThemeColors
    {
        AppBackground = ColorTranslator.FromHtml("#0A0F1A"),
        HeaderBackground = ColorTranslator.FromHtml("#070D17"),
        PanelBackground = ColorTranslator.FromHtml("#0F1620"),
        PanelAlt = ColorTranslator.FromHtml("#151E2E"),
        Border = ColorTranslator.FromHtml("#1D2B40"),
        TextPrimary = Color.White,
        TextSecondary = ColorTranslator.FromHtml("#B8C7D9"),
        TextMuted = ColorTranslator.FromHtml("#6B7A8E"),
        AccentBlue = ColorTranslator.FromHtml("#2563EB"),
        Success = ColorTranslator.FromHtml("#22C55E"),
        AccentYellow = ColorTranslator.FromHtml("#FACC15"),
        AccentPurple = ColorTranslator.FromHtml("#A855F7"),
        AccentRed = ColorTranslator.FromHtml("#EF4444")
    };

    public static ThemeColors CreateLight() => new ThemeColors
    {
        AppBackground = ColorTranslator.FromHtml("#EEF2F7"),
        HeaderBackground = Color.White,
        PanelBackground = Color.White,
        PanelAlt = ColorTranslator.FromHtml("#E7EDF5"),
        Border = ColorTranslator.FromHtml("#C8D4E3"),
        TextPrimary = ColorTranslator.FromHtml("#111827"),
        TextSecondary = ColorTranslator.FromHtml("#334155"),
        TextMuted = ColorTranslator.FromHtml("#64748B"),
        AccentBlue = ColorTranslator.FromHtml("#2563EB"),
        Success = ColorTranslator.FromHtml("#22C55E"),
        AccentYellow = ColorTranslator.FromHtml("#FACC15"),
        AccentPurple = ColorTranslator.FromHtml("#A855F7"),
        AccentRed = ColorTranslator.FromHtml("#EF4444")
    };
}

// ═══════════════════════════════════════════════════════════════════
// DRAWING UTILITIES
// ═══════════════════════════════════════════════════════════════════

internal static class UiDrawing
{
    public static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
    {
        GraphicsPath path = new GraphicsPath();
        int d = radius * 2;
        Rectangle arc = new Rectangle(bounds.Location, new Size(d, d));
        path.AddArc(arc, 180, 90);
        arc.X = bounds.Right - d; path.AddArc(arc, 270, 90);
        arc.Y = bounds.Bottom - d; path.AddArc(arc, 0, 90);
        arc.X = bounds.Left; path.AddArc(arc, 90, 90);
        path.CloseFigure();
        return path;
    }
}

internal abstract class ThemeAwareControl : Control
{
    private ThemeColors theme;

    public ThemeColors Theme
    {
        get => theme;
        set
        {
            theme = value;
            OnThemeChanged();
            Invalidate();
        }
    }

    protected virtual void OnThemeChanged()
    {
    }
}

// ═══════════════════════════════════════════════════════════════════
// ROUNDED PANEL
// ═══════════════════════════════════════════════════════════════════

internal sealed class RoundedPanel : ThemeAwareControl
{
    public Color FillColor { get; set; }
    public int Radius { get; set; } = 6;

    public RoundedPanel()
    {
        DoubleBuffered = true;

        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw,
            true);
    }
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        using (GraphicsPath path =
            UiDrawing.CreateRoundedRectangle(
                new Rectangle(0, 0, Width, Height),
                Radius))
        {
            Region = new Region(path);
        }
    }
    protected override void OnThemeChanged()
    {
        if (Theme != null)
            FillColor = Theme.PanelAlt;
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        if (Radius <= 0 || Width <= 2 || Height <= 2)
        {
            base.OnPaintBackground(e);
            return;
        }

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        using (GraphicsPath path = UiDrawing.CreateRoundedRectangle(
            new Rectangle(0, 0, Width - 1, Height - 1), Radius))
        using (SolidBrush brush = new SolidBrush(FillColor))
        //using (Pen pen = new Pen(Theme?.Border ?? Color.Gray, 1f))
        using (Pen pen = new Pen(
    Color.FromArgb(
        40,
        60,
        90),
    1f))
        {
            e.Graphics.FillPath(brush, path);
            e.Graphics.DrawPath(pen, path);
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// SECTION PANEL
// ═══════════════════════════════════════════════════════════════════

internal sealed class SectionPanel : ThemeAwareControl
{
    private readonly Panel headerPanel;
    private readonly Label titleLabel;
    public Panel Content { get; private set; }

    public SectionPanel()
    {
        DoubleBuffered = true;
        headerPanel = new Panel { Dock = DockStyle.Top, Height = 30, Padding = new Padding(12, 6, 12, 6) };
        titleLabel = new Label
        {
            Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            Location = new Point(10, 0),
            AutoSize = false,
            Height = 30
        };
        headerPanel.Controls.Add(titleLabel);
        titleLabel.BackColor = Color.Transparent;
        titleLabel.ForeColor = Color.White;
        Content = new Panel { Dock = DockStyle.Fill };
        Controls.Add(Content);
        Controls.Add(headerPanel);
    }

    public string Title { get { return titleLabel.Text; } set { titleLabel.Text = value.ToUpperInvariant(); } }

    protected override void OnLayout(LayoutEventArgs e)
    {
        base.OnLayout(e);
        if (titleLabel != null) titleLabel.Width = Width - 20;
    }
    public new ThemeColors Theme
    {
        get => base.Theme;
        set
        {
            base.Theme = value;

            if (value != null)
            {
                titleLabel.ForeColor = value.TextPrimary;
                headerPanel.BackColor = value.PanelBackground;
                Content.BackColor = value.PanelBackground;
            }

            Invalidate();
        }
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (Theme == null || Width <= 2 || Height <= 2) return;
        BackColor = Theme.PanelBackground;
        headerPanel.BackColor = Theme.PanelBackground;
        Content.BackColor = Theme.PanelBackground;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using (GraphicsPath path = UiDrawing.CreateRoundedRectangle(new Rectangle(0, 0, Width - 1, Height - 1), 8))
        using (Pen bp = new Pen(Theme.Border))
        using (SolidBrush bg = new SolidBrush(Theme.PanelBackground))
        {
            e.Graphics.FillPath(bg, path);
            e.Graphics.DrawPath(bp, path);
            e.Graphics.DrawLine(bp, 1, headerPanel.Height, Width - 2, headerPanel.Height);
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// LOGO
// ═══════════════════════════════════════════════════════════════════

internal sealed class LogoControl : ThemeAwareControl
{
    public LogoControl()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (Width <= 8 || Height <= 8 || Theme == null) return;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        using (Pen pen = new Pen(Theme.AccentBlue, 3.5F))
        {
            e.Graphics.DrawArc(pen, 2, 2, 22, 16, 205, 110);
            e.Graphics.DrawLine(pen, 6, 9, 26, 9);
            e.Graphics.DrawLine(pen, 7, 17, 23, 17);
        }
    }
}

// ═══════════════════════════════════════════════════════════════════
// THEME TOGGLE
// ═══════════════════════════════════════════════════════════════════

internal sealed class ThemeToggleControl : ThemeAwareControl
{
    private readonly Timer animationTimer;
    private float knobProgress;
    private bool isChecked;
    public event EventHandler ToggleChanged;

    public ThemeToggleControl()
    {
        Size = new Size(65, 26);
        Cursor = Cursors.Hand;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        animationTimer = new Timer { Interval = 15 };
        animationTimer.Tick += (s, e) =>
        {
            float target = isChecked ? 1F : 0F;
            float delta = target > knobProgress ? 0.16F : -0.16F;
            knobProgress += delta;
            if ((delta > 0 && knobProgress >= target) || (delta < 0 && knobProgress <= target))
            { knobProgress = target; animationTimer.Stop(); }
            Invalidate();
        };
    }

    public bool Checked { get { return isChecked; } set { isChecked = value; knobProgress = isChecked ? 1F : 0F; Invalidate(); } }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        isChecked = !isChecked;
        animationTimer.Start();
        ToggleChanged?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (Width <= 40 || Height <= 20 || Theme == null) return;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        Color back = isChecked ? Theme.AccentBlue : Theme.PanelAlt;
        using (GraphicsPath path = UiDrawing.CreateRoundedRectangle(new Rectangle(0, 0, Width - 1, Height - 1), Height / 2))
        using (SolidBrush brush = new SolidBrush(back))
        using (Pen pen = new Pen(Theme.Border))
        { e.Graphics.FillPath(brush, path); e.Graphics.DrawPath(pen, path); }
        using (SolidBrush sb = new SolidBrush(Theme.AccentYellow))
        using (SolidBrush mb = new SolidBrush(Color.White))
        {
            e.Graphics.FillEllipse(sb, 7, 7, 5, 5);
            e.Graphics.FillEllipse(mb, Width - 12, 7, 5, 5);
        }
        int kx = 3 + (int)((Width - 24) * knobProgress);

        using (SolidBrush kb = new SolidBrush(Color.White))
            e.Graphics.FillEllipse(kb, kx, 3, 18, 18);
    }
    
}