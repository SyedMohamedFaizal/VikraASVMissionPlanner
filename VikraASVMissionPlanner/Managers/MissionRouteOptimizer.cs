using System;
using System.Collections.Generic;
using System.Linq;
using VikraASVMissionPlanner.Models;

namespace VikraASVMissionPlanner.Managers
{
    public class MissionRouteOptimizer
    {
        /// <summary>
        /// Optimizes high-level mission transitions without changing
        /// the geometry of the generated survey pattern.
        /// </summary>
        public void OptimizeMission(MissionPlan missionPlan)
        {
            if (missionPlan == null)
                return;

            MissionStage cruise = FindStage(missionPlan, "Cruise");
            MissionStage survey = FindStage(missionPlan, "Survey");
            MissionStage burst = FindStage(missionPlan, "Burst");
            MissionStage returnCruise =
                FindStage(missionPlan, "Return Cruise");

            if (survey == null || survey.Points.Count < 2)
                return;

            MissionPoint previousPoint = null;
            MissionPoint nextPoint = null;

            // Mission stage before Survey
            if (cruise != null && cruise.Points.Count > 0)
            {
                previousPoint = cruise.Points.Last();
            }

            // Mission stage after Survey.
            // Burst takes priority when present.
            if (burst != null && burst.Points.Count > 0)
            {
                nextPoint = burst.Points.First();
            }
            else if (returnCruise != null &&
                     returnCruise.Points.Count > 0)
            {
                nextPoint = returnCruise.Points.First();
            }

            OptimizeSurveyDirection(
                survey,
                previousPoint,
                nextPoint);
        }

        private void OptimizeSurveyDirection(
            MissionStage survey,
            MissionPoint previousPoint,
            MissionPoint nextPoint)
        {
            if (survey == null || survey.Points.Count < 2)
                return;

            MissionPoint first = survey.Points.First();
            MissionPoint last = survey.Points.Last();

            double forwardCost = 0.0;
            double reverseCost = 0.0;

            // Previous stage -> Survey
            if (previousPoint != null)
            {
                forwardCost += Distance(previousPoint, first);
                reverseCost += Distance(previousPoint, last);
            }

            // Survey -> Next stage
            if (nextPoint != null)
            {
                forwardCost += Distance(last, nextPoint);
                reverseCost += Distance(first, nextPoint);
            }

            // Reverse the entire valid lawnmower path only when
            // reverse traversal produces shorter stage transitions.
            if (reverseCost < forwardCost)
            {
                survey.Points.Reverse();
            }

            Renumber(survey);
        }

        private MissionStage FindStage(
            MissionPlan missionPlan,
            string stageName)
        {
            return missionPlan.Stages.FirstOrDefault(
                s => string.Equals(
                    s.Name,
                    stageName,
                    StringComparison.OrdinalIgnoreCase));
        }

        private void Renumber(MissionStage stage)
        {
            for (int i = 0; i < stage.Points.Count; i++)
            {
                stage.Points[i].PointNumber = i + 1;
            }
        }

        private double Distance(
            MissionPoint a,
            MissionPoint b)
        {
            double dLat = a.Latitude - b.Latitude;
            double dLon = a.Longitude - b.Longitude;

            return Math.Sqrt(
                dLat * dLat +
                dLon * dLon);
        }
    }
}