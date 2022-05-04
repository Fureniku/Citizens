using System;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;

public class JunctionExitWaitState : VehicleBaseState {
    
    private float lookOffset = 0f;
    private bool reverseDir = false;
    private int check = 0;
    private int maxCheck = 2;

    private List<GameObject> seenVehicles = new List<GameObject>();
    private List<float> seenDistances = new List<float>();
    private List<GameObject> ignoredVehicles = new List<GameObject>();
    
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
        } else {
            return typeof(DriveState);
        }

        if (agent.isInJunctionBox) {
            agent.PrintWarn("Vehicle is somehow in junction box when waiting for junction; moving to drive state.");
            return typeof(DriveState);
        }

        Transform transform = agent.GetSeenObject().transform;
        if (transform != null) {
            if (transform.GetComponent<VehicleAgent>() != null) {
                VehicleAgent vehicleAgent = transform.GetComponent<VehicleAgent>();

                if (!ignoredVehicles.Contains(vehicleAgent.gameObject)) { //Don't do anything with ignored vehicles.
                    float dist = Vector3.Distance(agent.transform.position, transform.position);
                    if (seenVehicles.Contains(vehicleAgent.gameObject)) { //Seen this one before
                        int entry = seenVehicles.IndexOf(vehicleAgent.gameObject);
                        float prevDist = seenDistances[entry];

                        if (dist > prevDist) { //Vehicle is moving away. Add it to ignore.
                            seenVehicles.RemoveAt(entry);
                            seenDistances.RemoveAt(entry);
                            ignoredVehicles.Add(vehicleAgent.gameObject);
                        } else {
                            if (vehicleAgent.GetState() is JunctionExitWaitState || vehicleAgent.GetState() is ApproachJunctionState || vehicleAgent.GetState() is WaitForJunctionState || vehicleAgent.GetState() is WaitForVehicleState) {
                                if (agent.GetRoughFacingDirection() < vehicleAgent.GetRoughFacingDirection()) { //Priority based on facing direction
                                    check = 0;
                                } else if (agent.GetRoughFacingDirection() == vehicleAgent.GetRoughFacingDirection()) {
                                    float nodeDist = Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position);
                                    float otherNodeDist = Vector3.Distance(vehicleAgent.transform.position, vehicleAgent.GetCurrentDestination().transform.position);
                                    if (nodeDist > otherNodeDist) {
                                        check = 0;
                                    }
                                }
                            } else {
                                check = 0;
                            }
                        }
                    } else {
                        seenVehicles.Add(vehicleAgent.gameObject);
                        seenDistances.Add(dist);
                    }
                }
            }
        }

        if (check > maxCheck) {
            return typeof(TurningState);
        }

        if (Vector3.Distance(agent.transform.position, agent.GetCurrentDestination().transform.position) > 15f) {
            agent.PrintWarn("Agent was quite far from junction, resuming drive state.");
            return typeof(DriveState);
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
        check = 0;
        seenVehicles = new List<GameObject>();
        seenDistances = new List<float>();
        ignoredVehicles = new List<GameObject>();
        
        ignoredVehicles.Add(agent.gameObject);
        return null;
    }

    public override Type StateExit() {
        agent.GetAgent().isStopped = false;
        return null;
    }
}