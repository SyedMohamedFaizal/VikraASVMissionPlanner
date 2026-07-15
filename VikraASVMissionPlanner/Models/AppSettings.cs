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
    }
}
