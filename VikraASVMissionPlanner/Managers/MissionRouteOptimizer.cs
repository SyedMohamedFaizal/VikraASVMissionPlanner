using System;
using System.Collections.Generic;
using System.Linq;
using VikraASVMissionPlanner.Models;

namespace VikraASVMissionPlanner.Managers
{
    public class MissionRouteOptimizer
    {
        public void OptimizeSurvey(MissionStage surveyStage)
        {
            if (surveyStage == null)
                return;

            MissionPlan plan = new MissionPlan();

            plan.Stages.Add(surveyStage);

            OptimizeSurveyPattern(plan);
        }
        public void OptimizeMission(MissionPlan missionPlan)
        {
            if (missionPlan == null)
                return;

            OptimizeSurveyPattern(missionPlan);

            OptimizeStageTransitions(missionPlan);
        }

        private void OptimizeSurveyPattern(MissionPlan missionPlan)
        {
            MissionStage survey =
                missionPlan.Stages.FirstOrDefault();

            if (survey == null)
                return;

            if (survey.Points.Count < 2)
                return;

            List<MissionPoint> optimized =
                new List<MissionPoint>();

            for (int i = 0; i < survey.Points.Count; i += 2)
            {
                if (i + 1 >= survey.Points.Count)
                {
                    optimized.Add(survey.Points[i]);
                    break;
                }

                MissionPoint left = survey.Points[i];
                MissionPoint right = survey.Points[i + 1];

                if ((i / 2) % 2 == 0)
                {
                    optimized.Add(left);
                    optimized.Add(right);
                }
                else
                {
                    optimized.Add(right);
                    optimized.Add(left);
                }
            }

            survey.Points.Clear();
            survey.Points.AddRange(optimized);
        }

        private void OptimizeStageTransitions(MissionPlan missionPlan)
        {

        }
    }
}