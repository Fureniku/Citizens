using System;

public class VehiclePassengerState : PedestrianBaseState {

    public VehiclePassengerState(PedestrianAgent agent) {
        this.stateName = "Passenger State";
        this.agent = agent;
    }

    public override Type StateUpdate() {
        if (agent.GetVehicle() == null) {
            return typeof(WalkState);
        }
        return null;
    }

    public override Type StateEnter() {
        agent.GetAgent().enabled = false;
        return null;
    }

    public override Type StateExit() {
        return null;
    }
}