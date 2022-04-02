using System;

public class ObstructionSpottedState : BaseState {
    
    private int waitTime = 10;
    
    public ObstructionSpottedState(BaseAgent agent) {
        this.stateName = "Obstruction Spotted State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        float distance = agent.GetSeenObject().distance;
        
        if (distance < 25 && distance > 0) {
            agent.PrintText("Seeing something, dist: " + agent.GetSeenObject().distance);
            agent.GetAgent().isStopped = true;
            agent.SetLookDirection((agent.GetSeenObject().transform.position - agent.transform.position).normalized);
            waitTime = 0;
        }

        if (waitTime >= 50) {
            return typeof(DriveState);
        }
        else {
            waitTime++;
        }

        return null;
    }

    public override Type StateEnter() {
        agent.PrintText("Seeing something, dist: " + agent.GetSeenObject().distance);
        return null;
    }

    public override Type StateExit() {
        agent.GetAgent().isStopped = false;
        return null;
    }
}