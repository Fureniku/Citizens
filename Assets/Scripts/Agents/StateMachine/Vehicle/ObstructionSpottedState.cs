using System;

public class ObstructionSpottedState : AgentBaseState {
    
    private float approachDistance = 25;
    private float stopDistance = 3;
    
    private float maxSpeed;
    private float minSpeed = 2.0f;
    
    public ObstructionSpottedState(BaseAgent agent) {
        this.stateName = "Obstruction Spotted State";
        this.agent = agent;
        this.maxSpeed = agent.GetAgent().speed;
    }
    
    public override Type StateUpdate() {
        if (agent.GetLastSeenAgent() != null && agent.GetLastSeenAgent() is VehicleAgent) {
            VehicleAgent seenAgent = (VehicleAgent) agent.GetLastSeenAgent();

            if (seenAgent.GetStateType() == typeof(ApproachJunctionState) || seenAgent.GetStateType() == typeof(JunctionExitWaitState) || seenAgent.GetStateType() == typeof(WaitForJunctionState)) {
                return typeof(WaitForJunctionState);
            }
        }
        
        float distance = agent.GetSeenObject().distance;
        
        float deltaDist = approachDistance - stopDistance; //13
        float currentDeltaDist = distance - stopDistance; //7

        float distanceModifier = currentDeltaDist / deltaDist;
        float deltaSpeed = maxSpeed - minSpeed;

        
        if (distance < approachDistance && distance > 0) {
            if (distance < stopDistance) {
                agent.GetAgent().isStopped = true;
            }
            else {
                agent.GetAgent().isStopped = false;
                agent.GetAgent().speed = deltaSpeed * distanceModifier;
            }
            agent.SetLookDirection((agent.GetSeenObject().transform.position - agent.transform.position).normalized, false);
        }

        if (agent.GetLastSeenObject() == null) {
            return typeof(DriveState);
        }
        
        return null;
    }

    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        agent.GetAgent().isStopped = false;
        return null;
    }
}