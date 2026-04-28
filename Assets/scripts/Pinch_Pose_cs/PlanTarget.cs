using UnityEngine;

public class PlanTarget : MonoBehaviour
{
    
    public enum State { PLACED, PLANNING, PLANNED, EXECUTING, DONE, FAILED}

    private State currentState = State.PLACED;
    private PlanTargetUI ui;
    private TrajectoryPreview trajectoryPreview; 
    private MoveItActionClient moveItActionClient;

    private Vector3 targetPostion; 
    private Quaternion targetRotation; 


    void Start()
    {

        if(currentState != State.PLACED)
        {
            ui.ShowError("Already planning or Executed ..."); 
            return; 
        }        

        SetState(State.PLANNING);
        moveItActionClient.PlanToPose(targetPostion, targetRotation); 
    }

    public void Execute(string planID)
    {
        if (currentState != State.PLANNED)
        {
            ui.ShowError("no plan to execute ..."); 
            return;
        }

        SetState(State.EXECUTING);
        moveItActionClient.ExecutePlan(planID);
    }

    private void SetState(State newState)
    {
        currentState = newState;
        switch (newState)
        {
            case State.PLANNING:
                ui.SetStatus("Planning...");
                break;
            
            case State.PLANNED:
                ui.SetStatus("Plan Ready...");
                break;
                
            case State.EXECUTING:
                ui.SetStatus("Executing...");
                break;
            
            case State.DONE:
                ui.SetStatus("Completed...");
                break;
            
            case State.FAILED:
                ui.SetStatus("Failed");
                break;
        }

    }

    public void OnPlanSuccess(Vector3[] waypoints)
    {
        trajectoryPreview.DrawWaypoints(waypoints);
        SetState(State.PLANNED);
    }

    public void OnPlanFailed()
    {
        SetState(State.FAILED);
    }

    public void OnExecuteComplete()
    {
        SetState(State.DONE); 
    }


}
