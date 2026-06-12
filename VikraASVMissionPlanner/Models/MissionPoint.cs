using System;

namespace VikraASVMissionPlanner.Models
{
    public sealed class MissionPoint
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string MissionType { get; set; } = "Cruise";

        public int PointNumber { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double AltitudeMeters { get; set; } = 2.0;

        public double SpeedKnots { get; set; } = 12.0;

        public double HeadingDegrees { get; set; }

        public bool IsSurveyPolygonVertex { get; set; }

        public string DisplayLabel
        {
            get { return MissionType.Substring(0, 1).ToUpperInvariant() + PointNumber; }
        }
    }
}
