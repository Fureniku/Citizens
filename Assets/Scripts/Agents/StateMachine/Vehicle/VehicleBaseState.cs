using System;
using UnityEngine;

public abstract class VehicleBaseState : AgentBaseState {
    
    protected new VehicleAgent agent;
    private float lookOffset = 0f;
    private bool reverseDir = false;
    
    public Type CheckWaitingVehicle() {
        if (agent.GetLastSeenAgent() != null && agent.GetLastSeenAgent() is VehicleAgent) {
            VehicleAgent seenAgent = (VehicleAgent) agent.GetLastSeenAgent();

            if (seenAgent.GetState().IsWaitableState() && seenAgent.GetRoadSide() == agent.GetRoadSide()) {
                return typeof(WaitForVehicleState);
            }
        }

        return null;
    }

    public Type CheckObstructionVehicle() {
        if (agent.GetLastSeenObject() != null) {
            agent.SetLookDirection();
            if (agent.GetSeenObject().distance < 10 && agent.GetSeenObject().distance > 0) {
                if (agent.GetLastSeenAgent().GetRoadSide() == agent.GetRoadSide()) {
                    return typeof(ObstructionSpottedState);
                }
            }
        }

        return null;
    }
    
    protected void ScanAhead() {
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