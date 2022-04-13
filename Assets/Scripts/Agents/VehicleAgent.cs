using System;
using System.Collections;
using System.Collections.Generic;
using Tiles.TileManagement;
using UnityEngine;

public class VehicleAgent : BaseAgent {

    private float maxSpeed;

    public float GetMaxSpeed() {
        return maxSpeed;
    }

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
        }

        SetAgentDestination(dests[0]);
        
        VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
        if (node != null) {
            shouldStop = node.GiveWay();
        }

        maxSpeed = agent.speed;
        initialized = true;
    }

    public void SetSpeed(float speed) {
        if (speed < maxSpeed) {
            agent.speed = speed;
        }
        else {
            agent.speed = maxSpeed;
        }
    }
    
    protected override void InitStateMachine() {
        stateMachine = GetComponent<AgentStateMachine>();
        Dictionary<Type, AgentBaseState> states = new Dictionary<Type, AgentBaseState>();
        
        states.Add(typeof(DriveState), new DriveState(this)); //Standard driving with observation in front
        states.Add(typeof(ObstructionSpottedState), new ObstructionSpottedState(this)); //Something ahead is blocking the vehicle
        states.Add(typeof(CrashedState), new CrashedState(this)); //Vehicle collided with something
        states.Add(typeof(JunctionExitWaitState), new JunctionExitWaitState(this)); //Waiting at a junction exit, checking if its safe to go
        states.Add(typeof(ApproachJunctionState), new ApproachJunctionState(this)); //Approaching a junction; slowing down
        states.Add(typeof(WaitForJunctionState), new WaitForJunctionState(this)); //Waiting behind another vehicle at a junction
        states.Add(typeof(WaitForVehicleState), new WaitForVehicleState(this)); //Waiting behind another vehicle in general
        states.Add(typeof(SlowForTurnState), new SlowForTurnState(this)); //Slow down when approaching a turn where we wouldn't have to completely stop
        states.Add(typeof(TurningState), new TurningState(this)); //In the process of turning in a junction (slower driving)
        states.Add(typeof(AccelerateState), new AccelerateState(this)); //Gradually increase vehicle speed (manual control is better than Unity's agent system)
        
        stateMachine.SetStates(states);
    }

    protected override void AgentUpdate() {
        
    }

    protected override void AgentNavigate() {

    }

    public override void IncrementDestination() {
        if (currentDest < dests.Count-1) { //count isn't zero-based
            currentDest++;
            SetAgentDestination(dests[currentDest]);

            VehicleJunctionNode node = dests[currentDest].GetComponent<VehicleJunctionNode>();
            if (node != null) {
                shouldStop = node.GiveWay();
            }
        } else {
            ReachedDestination();
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

    /*private void OnDrawGizmos() {
        if (drawGizmos) {
            Gizmos.color = Color.red;
            if (eyePos != null) {
                Gizmos.DrawRay(eyePos.transform.position, lookDirection * objectDistance);
            }
            else {
                Gizmos.DrawRay(transform.position, lookDirection * objectDistance);
            }
        }
    }*/
}
