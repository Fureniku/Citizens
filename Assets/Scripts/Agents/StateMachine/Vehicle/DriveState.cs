using System;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class DriveState : VehicleBaseState {

    public DriveState(VehicleAgent agent) {
        this.stateName = "Drive State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        if (agent.IsParked()) {
            return typeof(ParkedState);
        }
        
        if (agent.GetAgent().speed < agent.GetMaxSpeed() - 2.0f) {
            return typeof(AccelerateState);
        }
        
        if (agent.GetCurrentDestination().GetComponent<DespawnerNode>() != null) {
            return typeof(DespawningState);
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
        Type obstruction = CheckObstruction();

        if (waitingVehicle != null) return waitingVehicle;
        if (obstruction != null) return obstruction;
        
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
        } else if (Vector3.Distance(agent.transform.position, agent.GetAgent().destination) < 0.75f) {
            agent.IncrementDestination();
        }

        return null;
    }
    
}