using System;
using UnityEngine;

public class WaitZebraCrossingState : PedestrianBaseState {
    
    private float lookOffset = 0f;
    private bool reverseDir = false;
    private int check = 0;
    private float agentDist = -1; //The distance to the agent saved from last frame
    private bool safeToCross = false;
    
    public WaitZebraCrossingState(PedestrianAgent agent) {
        this.stateName = "Wait For Zebra Crossing State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        TileData td = agent.GetCurrentTile();

        if (td.GetTile() == TileRegistry.ZEBRA_CROSSING_1x1) {
            CrossingController crossingController = td.GetComponent<CrossingController>();

            GameObject crossingPoint = crossingController.GetFurthestCrossing(agent.transform.position);
            agent.transform.LookAt(crossingPoint.transform);
        }
        
        ScanCrossing();
        if (agent.GetSeenObject().transform != null) {
            if (agent.GetSeenObject().transform.gameObject.CompareTag("Vehicle")) {
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

        if (safeToCross) {
            agent.IncrementDestination();
            return typeof(CrossingState);
        }
        return null;
    }
    
    public override Type StateEnter() {
        Debug.Log("Forcing to face opposite crossing");
        TileData td = agent.GetCurrentTile();

        if (td.GetTile() == TileRegistry.ZEBRA_CROSSING_1x1) {
            CrossingController crossingController = td.GetComponent<CrossingController>();

            GameObject crossingPoint = crossingController.GetFurthestCrossing(agent.transform.position);
            agent.transform.LookAt(crossingPoint.transform);
        }
        return null;
    }

    public override Type StateExit() {
        agent.GetAgent().isStopped = false;
        return null;
    }

    private int waitTime = 0;
    private float lookSpeed = 4.0f;
    private float maxLook = 80.0f;
    
    private void ScanCrossing() {
        Vector3 dir = new Vector3(Vector3.forward.x, Vector3.forward.y + lookOffset, Vector3.forward.z);
        agent.SetLookDirection(dir, true);
        if (waitTime == 0) {
            if (reverseDir) {
                lookOffset -= lookSpeed;
            } else {
                lookOffset += lookSpeed;
            }
            
            if (check > 1) {
                if (lookOffset < -lookSpeed) {
                    lookOffset += lookSpeed;
                } else if (lookOffset > lookSpeed) {
                    lookOffset -= lookSpeed;
                }
                else {
                    lookOffset = 0;
                    safeToCross = true;
                    agent.SetLookDirection(dir, true);
                }
            }
        }
        else {
            waitTime--;
        }

        if ((lookOffset > maxLook && !reverseDir) || (lookOffset < -maxLook && reverseDir)) {
            reverseDir = !reverseDir;
            waitTime = 30;
            check++;
        }
        
    }
}