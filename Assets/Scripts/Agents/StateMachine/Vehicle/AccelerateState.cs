using System;
using UnityEngine;

public class AccelerateState : DriveState {
    
    private float acceleration = 0.2f;

    public AccelerateState(VehicleAgent agent) : base(agent) {
        this.stateName = "Accelerate State";
        this.agent = agent;
    }

    public override Type StateUpdate() {
        agent.SetSpeed(agent.GetAgent().speed + acceleration);

        if (agent.GetAgent().speed >= agent.GetMaxSpeed()) {
            return typeof(DriveState);
        }

        return Drive();
    }

    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        return null;
    }
}