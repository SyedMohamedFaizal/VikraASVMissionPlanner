using MissionPlanner.Utilities;
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
                System.Diagnostics.Debug.WriteLine(
    "comPort null = " + (MainV2.comPort == null));
                MainV2.comPort.BaseStream = new MissionPlanner.Comms.SerialPort();
                System.Diagnostics.Debug.WriteLine(
    "BaseStream null = " +
    (MainV2.comPort.BaseStream == null));
                string[] ports = System.IO.Ports.SerialPort.GetPortNames();

                System.Diagnostics.Debug.WriteLine(
    "Detected Ports: " +
    string.Join(",", ports));

                if (ports.Length == 0)
                {
                    MessageBox.Show("No COM Ports Found");
                    return Task.FromResult(false);
                }

                MainV2.comPort.BaseStream.PortName = ports[0];
                MainV2.comPort.BaseStream.BaudRate = 115200;

                MainV2.comPort.BaseStream.Open();
                System.Diagnostics.Debug.WriteLine(
    "Port Opened = " +
    MainV2.comPort.BaseStream.IsOpen);

                System.Threading.Thread.Sleep(2000);

                var hb = MainV2.comPort.getHeartBeat();
                System.Diagnostics.Debug.WriteLine(
    "Heartbeat Length = " +
    hb.Length);

                if (hb.Length > 0)
                {
                    return Task.FromResult(true);
                }

                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Connection Error");
                return Task.FromResult(false);
            }

        }

        public Task<bool> UploadMissionAsync(MissionPlan missionPlan)
        {
            try
            {
                if (missionPlan == null)
                    return Task.FromResult(false);

                if (MainV2.comPort == null)
                    return Task.FromResult(false);

                if (MainV2.comPort.BaseStream == null)
                {
                    MessageBox.Show("Pixhawk is not connected");
                    return Task.FromResult(false);
                }

                if (!MainV2.comPort.BaseStream.IsOpen)
                {
                    MessageBox.Show("Pixhawk connection is closed");
                    return Task.FromResult(false);
                }

                var points = missionPlan.AllMissionPoints.ToList();

                if (points.Count == 0)
                {
                    MessageBox.Show("No mission points found.");
                    return Task.FromResult(false);
                }

                MainV2.comPort.setWPTotal((ushort)points.Count);

                for (ushort i = 0; i < points.Count; i++)
                {
                    var point = points[i];

                    Locationwp wp = new Locationwp();

                    wp.id = (ushort)MAVLink.MAV_CMD.WAYPOINT;
                    wp.frame = (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;

                    wp.lat = point.Latitude;
                    wp.lng = point.Longitude;
                    wp.alt = (float)point.AltitudeMeters;

                    var result = MainV2.comPort.setWP(
                        wp,
                        i,
                        MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT);

                    if (result != MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                    {
                        MessageBox.Show(
                            "Waypoint upload failed at index " + i +
                            "\nResult = " + result);

                        return Task.FromResult(false);
                    }
                }

                MessageBox.Show(
                    "Mission uploaded successfully.\n" +
                    "Waypoints = " + points.Count);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Upload Mission Error");

                return Task.FromResult(false);
            }
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
