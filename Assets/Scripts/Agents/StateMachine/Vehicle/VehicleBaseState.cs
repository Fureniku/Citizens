using System;

public abstract class VehicleBaseState : AgentBaseState {
    
    protected new VehicleAgent agent;
    
    public Type CheckWaitingVehicle() {
        if (agent.GetLastSeenAgent() != null && agent.GetLastSeenAgent() is VehicleAgent) {
            VehicleAgent seenAgent = (VehicleAgent) agent.GetLastSeenAgent();

            if (seenAgent.GetState().IsWaitableState()) {
                return typeof(WaitForVehicleState);
            }
        }

        return null;
    }

    public Type CheckObstructionVehicle() {
        if (agent.GetLastSeenObject() != null) {
            agent.SetLookDirection();
            if (agent.GetSeenObject().distance < 10 && agent.GetSeenObject().distance > 0) {
                return typeof(ObstructionSpottedState);
            }
        }

        return null;
    }
}