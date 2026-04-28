using UnityEngine;
using RosMessageTypes.Ur5eMoveitActions;
using RosMessageTypes.Geometry_msgs;

public class MoveItActionClient : MonoBehaviour
{
    // Simple Pose struct for results
    public struct Pose
    {
        public Vector3 position;
        public UnityEngine.Quaternion orientation;
    }

    public class PlanToPoseResult
    {
        public bool success;
        public string plan_id;
        public Vector3[] tcp_waypoints;
        public float plan_duration;
    }

    public class ExecutePlanResult
    {
        public bool success;
        public string message;
        public Pose final_pose;
    }

    public void PlanToPose(Vector3 targetPos, UnityEngine.Quaternion targetRot)
    {
        Debug.Log($"Planning to {targetPos} with orientation {targetRot}");
    }

    public void ExecutePlan(string planId)
    {
        Debug.Log($"Executing plan {planId}");
    }
}