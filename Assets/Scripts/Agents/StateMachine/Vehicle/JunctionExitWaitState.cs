using System;
using UnityEngine;

public class JunctionExitWaitState : VehicleBaseState {
    
    private float lookOffset = 0f;
    private bool reverseDir = false;
    private int check = 0;
    private int maxCheck = 2;
    private float agentDist = -1; //The distance to the agent saved from last frame
    
    public JunctionExitWaitState(VehicleAgent agent) {
        this.stateName = "Junction Exit Wait State";
        this.agent = agent;
        this.waitableState = true;
    }

    public override Type StateUpdate() {
        VehicleJunctionNode prevNode = agent.GetPreviousDestination().GetComponent<VehicleJunctionNode>();
        if (prevNode != null) {
            VehicleJunctionController vjc = prevNode.GetController();
        
            if (vjc.TurningRight(prevNode, agent.GetCurrentDestination())) {
                if (prevNode.GiveWay()) {
                    maxCheck = 4;
                    ScanJunctionBoth();
                } else {
                    maxCheck = 6;
                    ScanJunctionRight();
                }
            } else {
                if (prevNode.GiveWay()) {
                    maxCheck = 8;
                    if (check == maxCheck) {
                        reverseDir = true;
                        ScanJunctionLeft();
                    } else {
                        ScanJunctionRight();
                    }
                }
            }
        }

        if (agent.isInJunctionBox) {
            agent.PrintWarn("Vehicle is somehow in junction box when waiting for junction; moving to drive state.");
            return typeof(DriveState);
        }

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

        if (check > maxCheck) {
            return typeof(TurningState);
        }
        return null;
    }
    
    private void ScanJunctionBoth() {
        ScanJunction(-2.5f, 2.5f);
    }
    
    private void ScanJunctionRight() {
        ScanJunction(0, 2.5f);
    }
    
    private void ScanJunctionLeft() {
        ScanJunction(-2.5f, 0);
    }

    private void ScanJunction(float min, float max) {
        Vector3 dir = new Vector3(Vector3.forward.x + lookOffset, Vector3.forward.y, Vector3.forward.z);
        agent.SetLookDirection(dir, true);
        if (reverseDir) {
            lookOffset -= 0.1f;
        } else {
            lookOffset += 0.1f;
        }

        if ((lookOffset > max && !reverseDir) || (lookOffset < min && reverseDir)) {
            reverseDir = !reverseDir;
            lookOffset = 0;
            check++;
        }
    }

    public override Type StateEnter() {
        agent.IncrementDestination();
        agent.GetAgent().isStopped = true;
        agentDist = -1;
        check = 0;
        return null;
    }

    public override Type StateExit() {
        agent.GetAgent().isStopped = false;
        return null;
    }
}