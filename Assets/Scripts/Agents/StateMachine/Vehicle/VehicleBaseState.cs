using System;
using Tiles.TileManagement;
using UnityEngine;

public abstract class VehicleBaseState : AgentBaseState {
    
    protected new VehicleAgent agent;
    private float lookOffset = 0f;
    private bool reverseDir = false;
    
    public Type CheckWaitingVehicle() {
        if (agent.GetLastSeenAgent() != null && agent.GetLastSeenAgent() is VehicleAgent) {
            VehicleAgent seenAgent = (VehicleAgent) agent.GetLastSeenAgent();

            if (seenAgent.GetState().IsWaitableState() && !agent.IsOppositeRoadSide(seenAgent.GetRoadSide())) {
                if (agent.GetRoughFacingDirection().Opposite() != seenAgent.GetRoughFacingDirection()) {
                    return typeof(WaitForVehicleState);
                }
            }
        }

        return null;
    }

    public Type CheckObstructionVehicle() {
        if (agent.GetLastSeenObject() != null && agent.GetLastSeenAgent() is VehicleAgent) {
            VehicleAgent seenAgent = (VehicleAgent) agent.GetLastSeenAgent();
            agent.SetLookDirection();
            if (agent.GetSeenObject().distance < 10 && agent.GetSeenObject().distance > 0) {
                if (!agent.IsOppositeRoadSide(agent.GetLastSeenAgent().GetRoadSide())) {
                    if (agent.GetRoughFacingDirection().Opposite() != seenAgent.GetRoughFacingDirection()) {
                        return typeof(ObstructionSpottedState);
                    }
                }
            }
        }

        return null;
    }
    
    protected void ScanAhead() {
        Vector3 dir = new Vector3(Vector3.forward.x + lookOffset, Vector3.forward.y, Vector3.forward.z);
        agent.SetLookDirection(dir, true);
        if (reverseDir) {
            lookOffset -= 0.025f;
        } else {
            lookOffset += 0.025f;
        }

        if ((lookOffset > 0.075f && !reverseDir) || (lookOffset < -0.075f && reverseDir)) {
            reverseDir = !reverseDir;
        }
    }
}