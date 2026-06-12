using System.Collections.Generic;
using System.Linq;

namespace VikraASVMissionPlanner.Models
{
    public sealed class MissionPlan
    {
        public List<MissionStage> Stages { get; } = new List<MissionStage>();

        public IEnumerable<MissionPoint> AllMissionPoints
        {
            get { return Stages.SelectMany(stage => stage.Points); }
        }

        public IEnumerable<MissionPoint> SurveyPolygonPoints
        {
            get { return Stages.SelectMany(stage => stage.SurveyPolygonPoints); }
        }
    }
}
