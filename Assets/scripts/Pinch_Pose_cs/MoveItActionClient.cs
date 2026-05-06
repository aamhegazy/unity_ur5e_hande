using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;
using RosMessageTypes.Std;
using RosMessageTypes.Ur5eMoveitActions;

public class MoveItActionClient : MonoBehaviour
{
    [Header("ROS Topics (matching unity_action_bridge.py)")]
    public string planGoalTopic = "plan_to_pose/goal";
    public string planResultTopic = "plan_to_pose/result";
    public string executeGoalTopic = "execute_plan/goal";
    public string executeResultTopic = "execute_plan/result";

    [Header("Planning Parameters")]
    public float planningTime = 5.0f;
    public float velocityScaling = 0.3f;
    public float accelerationScaling = 0.3f;
    public string plannerId = "RRTConnect";

    private ROSConnection ros;

    public class PlanToPoseResult
    {
        public bool success;
        public string planId;
        public Vector3[] tcpWaypoints;
        public string errorMessage;
    }

    public class ExecutePlanResult
    {
        public bool success;
        public string errorMessage;
    }

    private Action<PlanToPoseResult> currentPlanCallback;
    private Action<ExecutePlanResult> currentExecuteCallback;

    void Start()
    {
        ros = ROSConnection.GetOrCreateInstance();

        ros.RegisterPublisher<PlanToPoseGoalMsg>(planGoalTopic);
        ros.RegisterPublisher<ExecutePlanGoalMsg>(executeGoalTopic);

        ros.Subscribe<PlanToPoseResultMsg>(planResultTopic, OnPlanResponse);
        ros.Subscribe<ExecutePlanResultMsg>(executeResultTopic, OnExecuteResponse);

        Debug.Log("[MoveItActionClient] Initialized.");
    }

    public void PlanToPose(Vector3 position, UnityEngine.Quaternion rotation, Action<PlanToPoseResult> callback)
    {
        currentPlanCallback = callback;

        var rosPosition = new PointMsg(position.z, -position.x, position.y);
        var rosRotation = new RosMessageTypes.Geometry.QuaternionMsg(
            rotation.z, -rotation.x, rotation.y, -rotation.w);

        var poseStamped = new PoseStampedMsg
        {
            header = new HeaderMsg { frame_id = "base_link" },
            pose = new PoseMsg
            {
                position = rosPosition,
                orientation = rosRotation
            }
        };

        var goal = new PlanToPoseGoalMsg
        {
            target_pose = poseStamped,
            planning_time = planningTime,
            velocity_scaling = velocityScaling,
            acceleration_scaling = accelerationScaling,
            planning_id = plannerId
        };

        ros.Publish(planGoalTopic, goal);
        Debug.Log($"[MoveItActionClient] Plan goal sent: pos=({position.x:F2}, {position.y:F2}, {position.z:F2})");
    }

    public void ExecutePlan(string planId, Action<ExecutePlanResult> callback)
    {
        currentExecuteCallback = callback;
        var goal = new ExecutePlanGoalMsg { plan_id = planId };
        ros.Publish(executeGoalTopic, goal);
        Debug.Log($"[MoveItActionClient] Execute goal sent: planId={planId}");
    }

    private void OnPlanResponse(PlanToPoseResultMsg msg)
    {
        Debug.Log($"[MoveItActionClient] Plan result: success={msg.success}, planId={msg.plan_id}");

        var waypoints = new Vector3[msg.tcp_waypoints.Length];
        for (int i = 0; i < msg.tcp_waypoints.Length; i++)
        {
            var p = msg.tcp_waypoints[i];
            waypoints[i] = new Vector3(-(float)p.y, (float)p.z, (float)p.x);
        }

        var result = new PlanToPoseResult
        {
            success = msg.success,
            planId = msg.plan_id,
            tcpWaypoints = waypoints,
            errorMessage = msg.message
        };

        currentPlanCallback?.Invoke(result);
        currentPlanCallback = null;
    }

    private void OnExecuteResponse(ExecutePlanResultMsg msg)
    {
        Debug.Log($"[MoveItActionClient] Execute result: success={msg.success}");

        var result = new ExecutePlanResult
        {
            success = msg.success,
            errorMessage = msg.message
        };

        currentExecuteCallback?.Invoke(result);
        currentExecuteCallback = null;
    }
}