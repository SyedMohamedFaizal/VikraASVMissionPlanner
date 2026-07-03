using System;
using System.Collections.Generic;
using System.Linq;
using VikraASVMissionPlanner.Models;

namespace VikraASVMissionPlanner.Managers
{
    public sealed class MissionManager
    {
        private readonly MissionPlan missionPlan;

        public MissionManager()
        {
            missionPlan = new MissionPlan();
            missionPlan.Stages.Add(new MissionStage { Name = "Cruise", MissionType = "Cruise", DefaultAltitudeMeters = 2.0, DefaultSpeedKnots = 12.0 });
            missionPlan.Stages.Add(new MissionStage { Name = "Survey", MissionType = "Survey", DefaultAltitudeMeters = 2.0, DefaultSpeedKnots = 4.0 });
            missionPlan.Stages.Add(new MissionStage { Name = "Burst", MissionType = "Burst", DefaultAltitudeMeters = 2.1, DefaultSpeedKnots = 25.0 });
            missionPlan.Stages.Add(new MissionStage { Name = "Return Cruise", MissionType = "Return Cruise", DefaultAltitudeMeters = 2.0, DefaultSpeedKnots = 12.0 });
            missionPlan.Stages.Add(new MissionStage { Name = "Home", MissionType = "Home", DefaultAltitudeMeters = 2.0, DefaultSpeedKnots = 0.0 });
        }

        public event EventHandler MissionChanged;

        public MissionPlan MissionPlan
        {
            get { return missionPlan; }
        }

        public MissionStage GetStage(string stageName)
        {
            if (string.Equals(stageName, "Loiter",
                StringComparison.OrdinalIgnoreCase))
            {
                stageName = "Survey";
            }

            return missionPlan.Stages.First(stage =>
                string.Equals(stage.Name, stageName,
                StringComparison.OrdinalIgnoreCase));
        }

        public MissionPoint AddWaypoint(string stageName, double latitude, double longitude)
        {
            MissionStage stage = GetStage(stageName);
            int nextPoint = stage.Points.Count + 1;
            double heading = 0.0;

            if (stage.Points.Count > 0)
            {
                MissionPoint previous = stage.Points[stage.Points.Count - 1];
                heading = CalculateHeading(previous.Latitude, previous.Longitude, latitude, longitude);
            }

            MissionPoint point = new MissionPoint
            {
                MissionType = stage.MissionType,
                PointNumber = nextPoint,
                Latitude = latitude,
                Longitude = longitude,
                AltitudeMeters = stage.DefaultAltitudeMeters,
                SpeedKnots = stage.DefaultSpeedKnots,
                HeadingDegrees = heading
            };

            stage.Points.Add(point);
            RaiseMissionChanged();
            return point;
        }

        public MissionPoint AddSurveyPolygonPoint(double latitude, double longitude)
        {
            MissionStage stage = GetStage("Survey");
            MissionPoint point = new MissionPoint
            {
                MissionType = "Survey Polygon",
                PointNumber = stage.SurveyPolygonPoints.Count + 1,
                Latitude = latitude,
                Longitude = longitude,
                AltitudeMeters = stage.DefaultAltitudeMeters,
                SpeedKnots = stage.DefaultSpeedKnots,
                IsSurveyPolygonVertex = true
            };

            stage.SurveyPolygonPoints.Add(point);
            RaiseMissionChanged();
            return point;
        }

        public void ClearSurveyPolygon()
        {
            GetStage("Survey").SurveyPolygonPoints.Clear();
            RaiseMissionChanged();
        }

        public void ClearMission()
        {
            foreach (MissionStage stage in missionPlan.Stages)
            {
                stage.Points.Clear();
                stage.SurveyPolygonPoints.Clear();
            }

            RaiseMissionChanged();
        }
        public void RemoveWaypoint(Guid pointId)
        {
            foreach (MissionStage stage in missionPlan.Stages)
            {
                MissionPoint point = stage.Points.FirstOrDefault(item => item.Id == pointId);
                if (point != null)
                {
                    stage.Points.Remove(point);
                    RenumberStage(stage);
                    RaiseMissionChanged();
                    return;
                }
            }
        }

        public void MoveWaypointUp(Guid pointId)
        {
            MoveWaypoint(pointId, -1);
        }

        public void MoveWaypointDown(Guid pointId)
        {
            MoveWaypoint(pointId, 1);
        }

        public IReadOnlyList<MissionPoint> GetAllWaypoints()
        {
            return missionPlan.AllMissionPoints.ToList();
        }

        public IReadOnlyList<MissionPoint> GetSurveyPolygon()
        {
            return GetStage("Survey").SurveyPolygonPoints.ToList();
        }

        public int GetTotalWaypointCount()
        {
            return missionPlan.AllMissionPoints.Count();
        }

        public double GetTotalDistanceKm()
        {
            double total = 0;
            foreach (MissionStage stage in missionPlan.Stages)
            {
                for (int index = 1; index < stage.Points.Count; index++)
                {
                    MissionPoint start = stage.Points[index - 1];
                    MissionPoint end = stage.Points[index];
                    total += CalculateDistanceKm(start.Latitude, start.Longitude, end.Latitude, end.Longitude);
                }
            }

            return total;
        }

        public TimeSpan GetEstimatedDuration()
        {
            double hours = 0;
            foreach (MissionStage stage in missionPlan.Stages)
            {
                for (int index = 1; index < stage.Points.Count; index++)
                {
                    MissionPoint start = stage.Points[index - 1];
                    MissionPoint end = stage.Points[index];
                    double distanceKm = CalculateDistanceKm(start.Latitude, start.Longitude, end.Latitude, end.Longitude);
                    double speedKnots = Math.Max(0.1, end.SpeedKnots);
                    double speedKmPerHour = speedKnots * 1.852;
                    hours += distanceKm / speedKmPerHour;
                }
            }

            return TimeSpan.FromHours(hours);
        }

        private void MoveWaypoint(Guid pointId, int offset)
        {
            foreach (MissionStage stage in missionPlan.Stages)
            {
                int currentIndex = stage.Points.FindIndex(item => item.Id == pointId);
                if (currentIndex < 0)
                {
                    continue;
                }

                int newIndex = currentIndex + offset;
                if (newIndex < 0 || newIndex >= stage.Points.Count)
                {
                    return;
                }

                MissionPoint point = stage.Points[currentIndex];
                stage.Points.RemoveAt(currentIndex);
                stage.Points.Insert(newIndex, point);
                RenumberStage(stage);
                RaiseMissionChanged();
                return;
            }
        }

        private void RenumberStage(MissionStage stage)
        {
            for (int index = 0; index < stage.Points.Count; index++)
            {
                stage.Points[index].PointNumber = index + 1;
                if (index > 0)
                {
                    MissionPoint previous = stage.Points[index - 1];
                    MissionPoint current = stage.Points[index];
                    current.HeadingDegrees = CalculateHeading(previous.Latitude, previous.Longitude, current.Latitude, current.Longitude);
                }
            }
        }

        private void RaiseMissionChanged()
        {
            if (MissionChanged != null)
            {
                MissionChanged(this, EventArgs.Empty);
            }
        }

        private static double CalculateDistanceKm(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double earthRadiusKm = 6371.0;
            double dLat = ToRadians(latitude2 - latitude1);
            double dLon = ToRadians(longitude2 - longitude1);
            double lat1 = ToRadians(latitude1);
            double lat2 = ToRadians(latitude2);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadiusKm * c;
        }

        private static double CalculateHeading(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            double dLon = ToRadians(longitude2 - longitude1);
            double lat1 = ToRadians(latitude1);
            double lat2 = ToRadians(latitude2);

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) -
                       Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double heading = (ToDegrees(Math.Atan2(y, x)) + 360) % 360;
            return heading;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static double ToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }
    }
}
