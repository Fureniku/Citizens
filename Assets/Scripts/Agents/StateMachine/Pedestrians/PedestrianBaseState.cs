using System;

public abstract class PedestrianBaseState : AgentBaseState {
    
    protected new PedestrianAgent agent;

    protected Type EnteredRoad() {
        if (agent.HasEnteredRoad()) {
            agent.SetPreviousState(this.GetType());
            return typeof(WaitToCrossState);
        }

        return null;
    }
        
}