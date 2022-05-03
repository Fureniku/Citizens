using System;

public class CrashedState : VehicleBaseState {

    public CrashedState(VehicleAgent agent) {
        this.stateName = "Crashed State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        return null;
    }

    public override Type StateEnter() {
        agent.GetAgent().isStopped = true;
        agent.SetParked();
        return null;
    }

    public override Type StateExit() {
        return null;
    }
}