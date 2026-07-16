using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VikraASVMissionPlanner
{
    public class AppSettings
    {
        public string Theme { get; set; } = "Dark";

        public string StartupPage { get; set; } = "Mission";

        public bool AutoConnectPixhawk { get; set; }
        public bool AutoStartTelemetry { get; set; }

        public int TelemetryRefreshRate { get; set; } = 100;
        public bool AutoStartCamera { get; set; }

        public bool SaveSnapshots { get; set; }
    }
}
