using System.Collections.Generic;

namespace VikraASVMissionPlanner.Models
{
    public sealed class MissionStage
    {
        public string Name { get; set; }

        public string MissionType { get; set; }

        public double DefaultSpeedKnots { get; set; }

        public double DefaultAltitudeMeters { get; set; }

        public List<MissionPoint> Points { get; } = new List<MissionPoint>();

        public List<MissionPoint> SurveyPolygonPoints { get; } = new List<MissionPoint>();

        public string SurveyPattern { get; set; } = "Grid";
    }
}
