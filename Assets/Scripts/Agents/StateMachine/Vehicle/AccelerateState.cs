using System;
using UnityEngine;

public class AccelerateState : VehicleBaseState {
    
    private static float approachDistance = 10;
    private float acceleration = 0.2f;

    public AccelerateState(VehicleAgent agent) {
        this.stateName = "Accelerate State";
        this.agent = agent;
    }

    public override Type StateUpdate() {
        Type waitingVehicle = CheckWaitingVehicle();
        Type obstruction = CheckObstructionVehicle();

        if (waitingVehicle != null) return waitingVehicle;
        if (obstruction != null) return obstruction;

        agent.SetSpeed(agent.GetAgent().speed + acceleration);

        if (agent.GetAgent().speed >= agent.GetMaxSpeed()) {
            return typeof(DriveState);
        }

        return null;
    }

    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        return null;
    }

    public static float GetApproachDist() {
        return approachDistance;
    }
}