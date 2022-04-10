using System;
using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;

public class VehicleAgent : BaseAgent {

    public override void Init() {
        GameObject finalDest = World.Instance.GetChunkManager().GetTile(DestinationRegistration.RoadDestinationRegistry.GetAtRandom()).gameObject;
        
        List<Node> path = aStar.RequestPath(gameObject, finalDest);

        if (path.Count > 2) {
            TileData tdStart = World.Instance.GetChunkManager().GetTile(new TilePos(path[0].x, path[0].y));
            TileData tdNext = World.Instance.GetChunkManager().GetTile(new TilePos(path[1].x, path[1].y));

            EnumDirection startingDirection = Direction.GetDirectionOffset(tdStart.GetGridPos(), tdNext.GetGridPos());

            if (tdStart.GetTile() == TileRegistry.STRAIGHT_ROAD_1x1) {
                switch (startingDirection) {
                    case EnumDirection.NORTH:
                        transform.position += new Vector3(-10f, 0, 0);
                        break;
                    case EnumDirection.EAST:
                        transform.position += new Vector3(0, 0, 10f);
                        break;
                    case EnumDirection.SOUTH:
                        transform.position += new Vector3(10f, 0, 0);
                        break;
                    case EnumDirection.WEST:
                        transform.position += new Vector3(0, 0, -10f);
                        break;
                }

                agent.Warp(transform.position);
            }
        }

        for (int i = 0; i < path.Count; i++) {
            TileData td = World.Instance.GetChunkManager().GetTile(new TilePos(path[i].x, path[i].y));
            if (td.gameObject.GetComponent<VehicleJunctionController>() != null) {
                VehicleJunctionController vjController = td.gameObject.GetComponent<VehicleJunctionController>();
                TileData entryTd = null;
                TileData exitTd = null;
                if (NodeInRange(i - 1, path.Count)) entryTd = World.Instance.GetChunkManager().GetTile(new TilePos(path[i-1].x, path[i-1].y));
                if (NodeInRange(i + 1, path.Count)) exitTd = World.Instance.GetChunkManager().GetTile(new TilePos(path[i+1].x, path[i+1].y));

                if (entryTd != null && exitTd != null) {
                    EnumDirection entry = Direction.GetDirectionOffset(entryTd.GetGridPos(), td.GetGridPos()); //Entry to current
                    EnumDirection exit = Direction.GetDirectionOffset(td.GetGridPos(), exitTd.GetGridPos()); //current to exit

                    GameObject entryGo = vjController.GetInNode(entry);
                    GameObject exitGo = vjController.GetOutNode(exit);
                    
                    dests.Add(entryGo);
                    dests.Add(exitGo);
                }
            }
        }

        if (finalDest.GetComponent<LocationNodeController>() != null) {
            LocationNodeController lnc = finalDest.GetComponent<LocationNodeController>();

            if (lnc.CanDestroyAfterDestination()) {
                Debug.Log("Destination and destruction");
                dests.Add(lnc.GetDestinationNode());
                dests.Add(lnc.GetDespawnerNode());
                destroyOnArrival = true;
            }
            else if (lnc.GetDestinationNode() != null) {
                Debug.Log("Destination only");
                dests.Add(lnc.GetDestinationNode());
            }
            else if (lnc.GetDespawnerNode() != null) {
                Debug.Log("Destruction only");
                dests.Add(lnc.GetDespawnerNode());
                destroyOnArrival = true;
            }
            else {
                Debug.Log("Node controller with no node");
                dests.Add(finalDest);
            }
        }
        else {
            Debug.Log("No node controller");
            dests.Add(finalDest);
        }
        
        Debug.Log("Generated final route with " + dests.Count + " nodes");
        
        agent.destination = dests[0].transform.position;
        
        VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
        if (node != null) {
            shouldStop = node.GiveWay();
        }
        initialized = true;
    }

    protected override void InitStateMachine() {
        stateMachine = GetComponent<AgentStateMachine>();
        Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();
        
        states.Add(typeof(DriveState), new DriveState(this));
        states.Add(typeof(ObstructionSpottedState), new ObstructionSpottedState(this));
        states.Add(typeof(CrashedState), new CrashedState(this));
        states.Add(typeof(JunctionExitWaitState), new JunctionExitWaitState(this));
        
        stateMachine.SetStates(states);
    }

    protected override void AgentUpdate() {
        
    }

    protected override void AgentNavigate() {
        if (dests[currentDest] != null) {
            currentDestGO = dests[currentDest];
            float dist = Vector3.Distance(transform.position, dests[currentDest].transform.position);
            if (dist < 5) {
                if (currentDest < dests.Count - 1) {
                    if (shouldStop) {
                        stateMachine.SwitchToState(typeof(JunctionExitWaitState));
                        IncrementDestination();
                    }
                    else {
                        IncrementDestination();
                    }
                }
                else {
                    ReachedDestination();
                }
            }
        }
        else {
            Debug.Log("Destination " + currentDest + " is null for " + gameObject.name);
        }
    }

    protected override void IncrementDestination() {
        currentDest++;
        agent.destination = dests[currentDest].transform.position;

        VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
        if (node != null) {
            shouldStop = node.GiveWay();
        }
    }

    protected override void AgentCollideEnter(Collision collision) {
        PrintText("Crashed into " + collision.collider.transform.gameObject.name);
        stateMachine.SwitchToState(typeof(CrashedState));
    }

    protected override void AgentCollideExit(Collision collision) { }

    protected override void AgentTriggerEnter(Collider other) {
        PrintText("Trigger enter into " + other.transform.gameObject.name);
        stateMachine.SwitchToState(typeof(CrashedState));
    }
    
    protected override void AgentTriggerExit(Collider other) { }

    private void OnDrawGizmos() {
        if (drawGizmos) {
            Gizmos.color = Color.red;
            if (eyePos != null) {
                Gizmos.DrawRay(eyePos.transform.position, lookDirection * objectDistance);
            }
            else {
                Gizmos.DrawRay(transform.position, lookDirection * objectDistance);
            }
        }
    }
}
