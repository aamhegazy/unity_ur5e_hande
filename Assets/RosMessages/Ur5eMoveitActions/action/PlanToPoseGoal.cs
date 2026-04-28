using RosMessageTypes.StdMsgs;
using RosMessageTypes.Geometry_msgs;

namespace RosMessageTypes.Ur5eMoveitActions
{
    public class PlanToPoseGoal
    {
        public PoseStamped target_pose;
        public double planning_time;
        public double velocity_scaling;
        public double acceleration_scaling;
        public string planning_id;
    }
}