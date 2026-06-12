using System;
using System.Windows.Forms;
using VikraASVMissionPlanner.Forms;

namespace VikraASVMissionPlanner
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SplashScreen());
        }
    }
}
