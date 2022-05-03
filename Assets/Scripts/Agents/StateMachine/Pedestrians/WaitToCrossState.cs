using System;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;

public class WaitToCrossState : PedestrianBaseState {
    
    private float lookOffset = 0f;
    private bool reverseDir = false;
    private int check = 0;
    private float agentDist = -1; //The distance to the agent saved from last frame
    private bool safeToCross = false;

    private List<GameObject> ignoredVehicles = new List<GameObject>();

    public WaitToCrossState(PedestrianAgent agent) {
        this.stateName = "Wait To Cross State";
        this.agent = agent;
    }
    
    public override Type StateUpdate() {
        TileData td = agent.GetCurrentTile();

        if (td.GetRotation() == EnumDirection.NORTH || td.GetRotation() == EnumDirection.SOUTH) {
            if (agent.GetRoadSide() == EnumDirection.EAST) {
                agent.transform.rotation = Quaternion.Euler(0, 270, 0);
            } else if (agent.GetRoadSide() == EnumDirection.WEST) {
                agent.transform.rotation = Quaternion.Euler(0, 90, 0);
            }
        } else {
            if (agent.GetRoadSide() == EnumDirection.NORTH) {
                agent.transform.rotation = Quaternion.Euler(0, 180, 0);
            } else if (agent.GetRoadSide() == EnumDirection.SOUTH) {
                agent.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        ScanCrossing();

        Transform seenObject = agent.GetSeenObject().transform;
        
        if (seenObject != null) {
            if (seenObject.gameObject.CompareTag("Vehicle")) {
                if (!ignoredVehicles.Contains(seenObject.gameObject)) {
                    float dist = Vector3.Distance(agent.transform.position, seenObject.position);
                    if (agentDist >= 0) {//Only reset for approaching vehicles, not ones moving away or not moving.
                        if (dist < agentDist) {
                            check = 0; //restart checking
                        } else {
                            //Vehicle is moving away (or more likely stationary). Add to ignore so we don't check it again later.
                            Debug.Log("Ignoring " + seenObject.gameObject.name);
                            ignoredVehicles.Add(seenObject.gameObject);
                        }
                    }
                    agentDist = dist;
                }
            }
        } else {
            agentDist = -1;
        }

        if (safeToCross) {
            return typeof(CrossingState);
        }
        return null;
    }
    
    public override Type StateEnter() {
        lookOffset = 0f;
        reverseDir = false;
        check = 0;
        agentDist = -1;
        safeToCross = false;
        agent.GetAgent().isStopped = true;
        ignoredVehicles.Clear();
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
        Vector3 dir = new Vector3(0, lookOffset, 0);
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
        } else {
            waitTime--;
        }

        if ((lookOffset > maxLook && !reverseDir) || (lookOffset < -maxLook && reverseDir)) {
            reverseDir = !reverseDir;
            waitTime = 30;
            check++;
        }
        
    }
}