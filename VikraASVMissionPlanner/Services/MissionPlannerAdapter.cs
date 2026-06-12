using MissionPlanner;
using MissionPlanner.Comms;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VikraASVMissionPlanner.Models;

namespace VikraASVMissionPlanner.Services
{
    public interface IMissionPlannerAdapter
    {
        Task<bool> ConnectAsync();

        Task<bool> UploadMissionAsync(MissionPlan missionPlan);

        Task<MissionPlan> DownloadMissionAsync();

        Task<bool> GenerateSurveyAsync(IEnumerable<MissionPoint> polygonPoints);

        Task<string> GetTelemetrySnapshotAsync();
    }

    public sealed class MissionPlannerAdapter : IMissionPlannerAdapter
    {

        public Task<bool> ConnectAsync()
        {
            try
            {
                MainV2.comPort.BaseStream = new MissionPlanner.Comms.SerialPort();

                MainV2.comPort.BaseStream.PortName = "COM4";
                MainV2.comPort.BaseStream.BaudRate = 115200;

                MainV2.comPort.BaseStream.Open();

                System.Threading.Thread.Sleep(3000);

                MessageBox.Show(
                    "GPS Fix = " + MainV2.comPort.MAV.cs.gpsstatus +
                    "\nMode = " + MainV2.comPort.MAV.cs.mode);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Connection Error");
                return Task.FromResult(false);
            }
        }

        public Task<bool> UploadMissionAsync(MissionPlan missionPlan)
{
   
            return Task.FromResult(missionPlan != null);

}

        public Task<MissionPlan> DownloadMissionAsync()
        {
            return Task.FromResult(new MissionPlan());
        }

        public Task<bool> GenerateSurveyAsync(IEnumerable<MissionPoint> polygonPoints)
        {
            return Task.FromResult(polygonPoints != null);
        }

        public Task<string> GetTelemetrySnapshotAsync()
        {
            return Task.FromResult("AUTO | GPS 3D Fix | Battery 82%");
        }
    }
}