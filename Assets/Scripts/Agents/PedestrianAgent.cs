using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianAgent : BaseAgent {

    [SerializeField] private GameObject head;

    public override void Init() {
        GameObject finalDest = World.Instance.GetChunkManager().GetTile(DestinationRegistration.RoadDestinationRegistry.GetAtRandom()).gameObject;
        
        //List<Node> path = aStar.RequestPath(gameObject, finalDest);

        /*if (path.Count > 2) {
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
                dests.Add(lnc.GetDestinationNode());
                dests.Add(lnc.GetDespawnerNode());
                destroyOnArrival = true;
            }
            else if (lnc.GetDestinationNode() != null) {
                dests.Add(lnc.GetDestinationNode());
            }
            else if (lnc.GetDespawnerNode() != null) {
                dests.Add(lnc.GetDespawnerNode());
                destroyOnArrival = true;
            }
            else {
                dests.Add(finalDest);
            }
        }
        else {
            dests.Add(finalDest);
        }*/
        dests.Add(finalDest);

        SetAgentDestination(finalDest);

        initialized = true;
    }
    
    protected override void InitStateMachine() {
        stateMachine = GetComponent<AgentStateMachine>();
        Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();
        
        states.Add(typeof(WalkState), new WalkState(this)); //Standard walking
        states.Add(typeof(ApproachZebraCrossingState), new ApproachZebraCrossingState(this)); //Approach a zebra crossing
        states.Add(typeof(WaitZebraCrossingState), new WaitZebraCrossingState(this)); //Wait at a zebra crossing
        states.Add(typeof(CrossingState), new CrossingState(this)); //Crossing the road (as to not switch back into an approach state)

        stateMachine.SetStates(states);
    }
    
    public new void SetLookDirection(Vector3 vec3, bool force) {
        if (GetLastSeenObject() != null && !force) {
            SetLookDirection();
        } else {
            head.transform.rotation = Quaternion.Euler(vec3);
        }
    }

    protected override void AgentUpdate() {}
    protected override void AgentNavigate() {}
    
    public override void IncrementDestination() {
        if (currentDest < dests.Count-1) { //count isn't zero-based
            currentDest++;
            SetAgentDestination(dests[currentDest]);

            VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
            if (node != null) {
                shouldStop = node.GiveWay();
            }
        } else {
            ReachedDestination(currentDestGO);
        }
    }

    public override void SetAgentDestruction(GameObject dest) {}

    protected override void AgentCollideEnter(Collision collision) {}
    protected override void AgentCollideExit(Collision collision) {}
    protected override void AgentTriggerEnter(Collider other) {}
    protected override void AgentTriggerExit(Collider other) {}

    
}
