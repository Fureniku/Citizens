﻿
using System;
using UnityEngine;

public class TurningState : VehicleBaseState {

    public TurningState(VehicleAgent agent) {
        this.stateName = "Turning State";
        this.agent = agent;
    }

    public override Type StateUpdate() {
        Type waitingVehicle = CheckWaitingVehicle();
        Type obstruction = CheckObstructionVehicle();

        if (waitingVehicle != null) return waitingVehicle;
        if (obstruction != null) return obstruction;
        
        ScanAhead();
        
        float dist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);

        if (dist < 1) {
            return typeof(DriveState);
        }

        return null;
    }

    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        agent.IncrementDestination();
        return null;
    }
}
