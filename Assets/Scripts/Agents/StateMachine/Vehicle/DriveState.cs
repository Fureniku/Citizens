using System;
using UnityEngine;

public class DriveState : VehicleBaseState {
    
    

    public DriveState(VehicleAgent agent) {
        this.stateName = "Drive State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        if (agent.GetAgent().speed < agent.GetMaxSpeed() - 2.0f) {
            return typeof(AccelerateState);
        }

        return Drive();
    }
    
    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        return null;
    }

    protected Type Drive() {
        Type waitingVehicle = CheckWaitingVehicle();
        Type obstruction = CheckObstructionVehicle();

        if (waitingVehicle != null) return waitingVehicle;
        if (obstruction != null) { return obstruction; }
        
        ScanAhead();
        
        float dist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);
        
        VehicleJunctionNode vjn = agent.GetCurrentDestination().GetComponent<VehicleJunctionNode>();
        VehicleJunctionController vjc = vjn != null ? vjn.GetController() : null;

        if (vjn != null && vjc != null && vjn.IsIn()) {
            bool shouldStop = vjn.GiveWay() || vjc.TurningRight(vjn, agent.GetNextDestination()); //Giving way or turning right

            if (shouldStop) {
                if (dist < ApproachJunctionState.GetApproachDist()) {
                    return typeof(ApproachJunctionState);
                }
            } else if (!vjc.CrossingJunction(vjn, agent.GetNextDestination())) { //Already checked if we're going right, this means we're going left.
                return typeof(SlowForTurnState);
            }
        }

        if (dist < 1) {
            agent.IncrementDestination();
        }

        return null;
    }
    
}