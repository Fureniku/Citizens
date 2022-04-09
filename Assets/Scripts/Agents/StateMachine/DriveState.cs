using System;
using UnityEngine;

public class DriveState : AgentBaseState {
    
    private float lookOffset = 0f;
    private bool reverseDir = false;

    public DriveState(BaseAgent agent) {
        this.stateName = "Drive State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        if (agent.GetSeenObject().distance < 25 && agent.GetSeenObject().distance > 0) {
            return typeof(ObstructionSpottedState);
        }

        ScanAhead();
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
        agent.SetLookDirection(dir);
        if (reverseDir) {
            lookOffset -= 0.05f;
        } else {
            lookOffset += 0.05f;
        }

        if ((lookOffset > 0.2f && !reverseDir) || (lookOffset < -0.2f && reverseDir)) {
            reverseDir = !reverseDir;
        }
    }
}