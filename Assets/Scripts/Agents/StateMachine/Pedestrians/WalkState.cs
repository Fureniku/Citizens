using System;

public class WalkState : PedestrianBaseState {

    public WalkState(PedestrianAgent agent) {
        this.stateName = "Walking State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        if (agent.GetVehicle() != null) {
            return typeof(VehiclePassengerState);
        }

        agent.SetLookDirection();

        return EnteredRoad();
    }
    
    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        return null;
    }
}