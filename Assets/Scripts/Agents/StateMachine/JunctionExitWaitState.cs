using System;
using UnityEngine;

public class JunctionExitWaitState : AgentBaseState {
    
    private float lookOffset = 0f;
    private bool reverseDir = false;
    private int check = 0;
    
    public JunctionExitWaitState(BaseAgent agent) {
        this.stateName = "Junction Exit Wait State";
        this.agent = agent;
    }

    public override Type StateUpdate() {
        ScanJunction();
        if (agent.SeenAgent(agent.GetSeenObject())) {
            check = 0; //restart checking
        }

        if (check > 2) {
            return typeof(DriveState);
        }
        return null;
    }
    
    private void ScanJunction() {
        Vector3 dir = new Vector3(Vector3.forward.x + lookOffset, Vector3.forward.y, Vector3.forward.z);
        agent.SetLookDirection(dir);
        if (reverseDir) {
            lookOffset -= 0.1f;
        } else {
            lookOffset += 0.1f;
        }

        if ((lookOffset > 2.5f && !reverseDir) || (lookOffset < -2.5f && reverseDir)) {
            reverseDir = !reverseDir;
            check++;
        }
    }

    public override Type StateEnter() {
        agent.GetAgent().isStopped = true;
        return null;
    }

    public override Type StateExit() {
        agent.GetAgent().isStopped = false;
        return null;
    }
}