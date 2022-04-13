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
        
        if (agent.GetLastSeenObject() != null) {
            agent.SetLookDirection();
            if (agent.GetSeenObject().distance < 10 && agent.GetSeenObject().distance > 0) {
                return typeof(ObstructionSpottedState);
            }
        } else {
            ScanAhead();
        }

        float dist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestinationObject().transform.position);

        if (dist < SlowForTurnState.GetApproachDist() && !agent.ShouldStop()) {
            return typeof(SlowForTurnState);
        }
        
        if (dist < ApproachJunctionState.GetApproachDist()) {
            if (agent.ShouldStop() && agent.GetStateType() == typeof(DriveState)) {
                return typeof(ApproachJunctionState);
            }
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
        agent.SetLookDirection(dir, false);
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