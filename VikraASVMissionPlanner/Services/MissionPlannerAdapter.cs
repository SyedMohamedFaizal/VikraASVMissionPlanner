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
using System.Threading;
using System.Threading.Tasks;

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
        private CancellationTokenSource telemetryCts;
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
                System.Diagnostics.Debug.WriteLine("MainV2.instance = " + (MainV2.instance == null));
                System.Diagnostics.Debug.WriteLine("MainV2.comPort = " + (MainV2.comPort == null));
                System.Diagnostics.Debug.WriteLine("BaseStream = " + (MainV2.comPort?.BaseStream == null));

                MainV2.comPort.BaseStream.PortName = ports[0];
                MainV2.comPort.BaseStream.BaudRate = 115200;

                MainV2.comPort.Open(false, true, false);

                if (MainV2.comPort.BaseStream.IsOpen)
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

                ushort total = (ushort)(points.Count + 1);

                MainV2.comPort.setWPTotal(total);

                Locationwp home = new Locationwp();

                home.id =
                    (ushort)MAVLink.MAV_CMD.WAYPOINT;

                home.frame =
                    (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;

                home.lat = points[0].Latitude;
                home.lng = points[0].Longitude;
                home.alt = 0;

                var homeResult =
                    MainV2.comPort.setWP(
                        home,
                        0,
                        MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT);

                if (homeResult != MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                {
                    MainV2.comPort.setWPACK();

                    MessageBox.Show(
                        "HOME upload failed\nResult = " +
                        homeResult);

                    return Task.FromResult(false);
                }

                for (int i = 0; i < points.Count; i++)
                {
                    var point = points[i];

                    Locationwp wp = new Locationwp();

                    wp.id =
                        (ushort)MAVLink.MAV_CMD.WAYPOINT;

                    wp.frame =
                        (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT;

                    wp.lat = point.Latitude;
                    wp.lng = point.Longitude;
                    wp.alt = (float)point.AltitudeMeters;

                    ushort seq = (ushort)(i + 1);

                    var result =
                        MainV2.comPort.setWP(
                            wp,
                            seq,
                            MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT);

                    if (result != MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                    {
                        MainV2.comPort.setWPACK();

                        MessageBox.Show(
                            "Waypoint upload failed\n" +
                            "Seq = " + seq +
                            "\nResult = " + result);

                        return Task.FromResult(false);
                    }
                }

                MainV2.comPort.setWPACK();

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
            try
            {
                if (MainV2.comPort == null ||
                    MainV2.comPort.BaseStream == null ||
                    !MainV2.comPort.BaseStream.IsOpen)
                {
                    MessageBox.Show("Pixhawk is not connected.");
                    return Task.FromResult(new MissionPlan());
                }

                MissionPlan missionPlan = new MissionPlan();

                // Get number of waypoints stored in Pixhawk
                ushort count = MainV2.comPort.getWPCount();
                MessageBox.Show("Waypoint Count = " + count);

                MessageBox.Show($"Pixhawk contains {count} waypoint(s).");

                if (count == 0)
                    return Task.FromResult(missionPlan);

                // Create one stage to hold downloaded points
                MissionStage cruise = new MissionStage
                {
                    Name = "Cruise",
                    MissionType = "Cruise",
                    DefaultAltitudeMeters = 2,
                    DefaultSpeedKnots = 12
                };

                MissionStage survey = new MissionStage
                {
                    Name = "Survey",
                    MissionType = "Survey",
                    DefaultAltitudeMeters = 2,
                    DefaultSpeedKnots = 4
                };

                MissionStage burst = new MissionStage
                {
                    Name = "Burst",
                    MissionType = "Burst",
                    DefaultAltitudeMeters = 2,
                    DefaultSpeedKnots = 25
                };

                MissionStage returnCruise = new MissionStage
                {
                    Name = "Return Cruise",
                    MissionType = "Return Cruise",
                    DefaultAltitudeMeters = 2,
                    DefaultSpeedKnots = 12
                };

                MissionStage home = new MissionStage
                {
                    Name = "Home",
                    MissionType = "Home",
                    DefaultAltitudeMeters = 2,
                    DefaultSpeedKnots = 0
                };

                missionPlan.Stages.Add(cruise);
                missionPlan.Stages.Add(survey);
                missionPlan.Stages.Add(burst);
                missionPlan.Stages.Add(returnCruise);
                missionPlan.Stages.Add(home);

                for (ushort i = 0; i < count; i++)
                {
                    Locationwp wp = MainV2.comPort.getWP(i);

                    MissionPoint point = new MissionPoint
                    {
                        MissionType = "Cruise",
                        PointNumber = i + 1,
                        Latitude = wp.lat,
                        Longitude = wp.lng,
                        AltitudeMeters = wp.alt,
                        SpeedKnots = 12.0,
                        HeadingDegrees = 0
                    };

                    cruise.Points.Add(point);
                }

                return Task.FromResult(missionPlan);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Download Mission");

                return Task.FromResult(new MissionPlan());
            }
        }
        public Task<bool> GenerateSurveyAsync(IEnumerable<MissionPoint> polygonPoints)
        {
            return Task.FromResult(polygonPoints != null);
        }

        public Task<string> GetTelemetrySnapshotAsync()
        {
            return Task.FromResult("AUTO | GPS 3D Fix | Battery 82%");
        }
        public void StartTelemetryLoop()
        {
            if (telemetryCts != null)
                return;

            telemetryCts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (!telemetryCts.IsCancellationRequested)
                {
                    try
                    {
                        if (MainV2.comPort != null &&
                            MainV2.comPort.BaseStream != null &&
                            MainV2.comPort.BaseStream.IsOpen)
                        {
                            await MainV2.comPort.readPacketAsync();
                        }
                    }
                    catch
                    {
                    }

                    await Task.Delay(10);
                }
            });
        }
    }
}
