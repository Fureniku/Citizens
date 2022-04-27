using System;
using UnityEngine;

public class DriveState : VehicleBaseState {
    
    private float lookOffset = 0f;
    private bool reverseDir = false;

    public DriveState(VehicleAgent agent) {
        this.stateName = "Drive State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        if (agent.GetAgent().speed < agent.GetMaxSpeed() - 2.0f) {
            return typeof(AccelerateState);
        }
        
        Type obstruction = CheckObstructionVehicle();

        if (obstruction != null) { return obstruction; }

        ScanAhead();

        float dist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestinationObject().transform.position);
        
        VehicleJunctionNode vjn = agent.GetCurrentDestinationObject().GetComponent<VehicleJunctionNode>();
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
    public override Type StateEnter() {
        return null;
    }

    public override Type StateExit() {
        return null;
    }
    
    private void ScanAhead() {
        Vector3 dir = new Vector3(Vector3.forward.x + lookOffset, Vector3.forward.y, Vector3.forward.z);
        agent.SetLookDirection(dir, true);
        if (reverseDir) {
            lookOffset -= 0.015f;
        } else {
            lookOffset += 0.015f;
        }

        if ((lookOffset > 0.075f && !reverseDir) || (lookOffset < -0.075f && reverseDir)) {
            reverseDir = !reverseDir;
        }
    }
}