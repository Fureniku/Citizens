using System;
using UnityEngine;

public class JunctionExitWaitState : VehicleBaseState {
    
    private float lookOffset = 0f;
    private bool reverseDir = false;
    private int check = 0;
    private float agentDist = -1; //The distance to the agent saved from last frame
    
    public JunctionExitWaitState(VehicleAgent agent) {
        this.stateName = "Junction Exit Wait State";
        this.agent = agent;
        this.waitableState = true;
    }

    public override Type StateUpdate() {
        ScanJunction();
        if (agent.GetSeenObject().transform != null) {
            if (agent.SeenAgent(agent.GetSeenObject().transform.gameObject)) {
                float dist = Vector3.Distance(agent.transform.position, agent.GetSeenObject().transform.position);
                if (agentDist >= 0 && dist < agentDist) { //Only reset for approaching vehicles, not ones moving away.
                    check = 0; //restart checking
                }

                agentDist = dist;
            }
        }
        else {
            agentDist = -1;
        }

        if (check > 2) {
            agent.IncrementDestination();
            return typeof(DriveState);
        }
        return null;
    }
    
    private void ScanJunction() {
        Vector3 dir = new Vector3(Vector3.forward.x + lookOffset, Vector3.forward.y, Vector3.forward.z);
        agent.SetLookDirection(dir, true);
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
        agent.IncrementDestination();
        agent.GetAgent().isStopped = true;
        agentDist = -1;
        return null;
    }

    public override Type StateExit() {
        agent.GetAgent().isStopped = false;
        return null;
    }
}